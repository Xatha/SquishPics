using Discord;
using log4net;
using SquishPicsDiscordBackend.Controllers;
using SquishPicsDiscordBackend.Logging;
using SquishPicsDiscordBackend.RetryHelpers;

namespace SquishPicsDiscordBackend.Messaging;

public class MessageService
{
    private const int MESSAGE_MILLISECOND_DELAY = 1000;
    private readonly ILog _log;

    public event EventHandler<Status>? MessageSent;
    public event EventHandler<MessageServiceCompleteState>? Completed;
    
    private readonly DiscordClient _client;
    private readonly MessageQueue _messageQueue;
    private bool _isSending;
    private int _messageCount;
    private int _messagesSent;
    private MessageServiceCompleteState _completeState;

    public MessageService(ILog log, DiscordClient client, MessageQueue messageQueue)
    {
        _log = log;
        _client = client;
        _messageQueue = messageQueue;

        _messageQueue.MessageSent += MessageQueue_MessageSent;
        _messageQueue.Finished += MessageQueue_Finished;
        _messageQueue.Failed += MessageQueue_Failed;
    }
    
    public Task SetupAsync(IEnumerable<string> filePaths, IMessageChannel channel)
    {
        if (!_messageQueue.IsEmpty) throw new InvalidOperationException("The queue is not empty.");

        _completeState = MessageServiceCompleteState.Success;
        var messages = filePaths.Select(path => new AttachmentMessage(path, channel)).ToList();
        _messageCount = messages.Count;
        _messagesSent = 0;
        messages.ForEach(_messageQueue.Enqueue);
        return Task.CompletedTask;
    }

    public void StartForget() => Task.Run(StartSendingAsync).ConfigureAwait(false);

    public Task StopAsync()
    {
        _messageQueue.Clear();
        _completeState = MessageServiceCompleteState.Cancelled;
        return Task.CompletedTask;
    }
    
    private async Task StartSendingAsync()
    {
        _isSending = true;
        while (_messageQueue.TryDequeue(out var message) && _isSending)
        {
            try
            {
                await SendMessageWithRetry(message, TimeSpan.FromSeconds(2), 5);
            }
            catch (RetryTimeoutException e)
            {
                await _log.ErrorAsync("Failed to send message... Will abort.", e);
                OnCompleted(MessageServiceCompleteState.Failed);
                return;
            }
            
            _messagesSent++;
            OnMessageSent(new Status
            {
                Message = "Message sent",
                WorkDone = _messagesSent,
                TotalWork = _messageCount
            });
            await Task.Delay(MESSAGE_MILLISECOND_DELAY);
        }

        OnCompleted(_completeState);
    }

    private async Task SendMessageWithRetry(IMessage message, TimeSpan waitTime, int maxRetries)
    {
        for (var attempt = 0; attempt < maxRetries; attempt++)
        {
            if (DiscordClient.ConnectionState != ConnectionState.Connected)
            {
                await Task.Delay(waitTime);
                continue;
            }

            await message.SendAsync();
            return;
        }

        throw new RetryTimeoutException($"Failed to send message after {maxRetries} attempts.");
    }
    
    protected virtual void OnMessageSent(Status e) => MessageSent?.Invoke(this, e);

    protected virtual void OnCompleted(MessageServiceCompleteState e)
    {
        _isSending = false;
        Completed?.Invoke(this, e);
    }

    #region Events
    private void MessageQueue_Finished(object? sender, EventArgs e)
    {
        OnCompleted(MessageServiceCompleteState.Success);
    }

    private void MessageQueue_MessageSent(object? sender, EventArgs e)
    {
        _messagesSent++;
        OnMessageSent(new Status
        {
            Message = "Message sent",
            WorkDone = _messagesSent,
            TotalWork = _messageCount
        });
    }
    
    private void MessageQueue_Failed(object? sender, RetryTimeoutException e)
    {
        OnCompleted(MessageServiceCompleteState.Failed);
    }

    #endregion
}