using System.Collections.Concurrent;
using Discord;
using log4net;
using SquishPicsDiscordBackend.Logging;
using SquishPicsDiscordBackend.RetryHelpers;

namespace SquishPicsDiscordBackend.MessageService;

public sealed class MessageQueue : ConcurrentQueue<IMessage>
{
    private readonly ILog _log = LogProvider.GetLogger<MessageQueue>();
    private const int MESSAGE_MILLISECOND_DELAY = 1000;
    private readonly DiscordClient _client;
    private bool _isRunning;

    public MessageQueue(DiscordClient client)
    {
        _client = client;
    }

    public event EventHandler? Finished;
    public event EventHandler<RetryTimeoutException>? Failed;
    public event EventHandler? MessageSent;
    
    public async Task StartSendingAsync()
    {
        _isRunning = true;
        while (TryDequeue(out var message) && _isRunning)
        {
            try
            {
                await SendMessageWithRetry(message, TimeSpan.FromSeconds(2), 5);
            }
            catch (RetryTimeoutException e)
            {
                await _log.ErrorAsync("Failed to send message... Will abort.", e);
                OnFailed(e);
                return;
            }

            OnMessageSent();
            await Task.Delay(MESSAGE_MILLISECOND_DELAY);
        }

        OnFinished();
    }

    public Task StopSendingAsync()
    {
        _isRunning = false;
        return Task.CompletedTask;
    }

    private async Task SendMessageWithRetry(IMessage message, TimeSpan waitTime, int maxRetries)
    {
        for (var attempt = 0; attempt < maxRetries; attempt++)
        {
            if (_client.ConnectionState != ConnectionState.Connected)
            {
                //Retry
                await Task.Delay(waitTime);
                continue;
            }

            await message.SendAsync();
            return;
        }

        throw new RetryTimeoutException($"Failed to send message after {maxRetries} attempts.");
    }

    private void OnFinished() => Finished?.Invoke(this, EventArgs.Empty);
    private void OnFailed(RetryTimeoutException e) => Failed?.Invoke(this, e);

    private void OnMessageSent()
    {
        MessageSent?.Invoke(this, EventArgs.Empty);
    }
}