using System.Collections.Concurrent;
using Discord;

namespace SquishPicsDiscordBackend.MessageService;

public sealed class MessageQueue : ConcurrentQueue<IMessage>
{
    private const int MessageMillisecondDelay = 1000; 
    private readonly DiscordClient _client;
    private bool _isRunning;

    public event EventHandler? Finished;
    public event EventHandler<RetryTimeoutException>? Failed;

    public MessageQueue(DiscordClient client)
    {
        _client = client;
    }

    //TODO: Handle on disconnect from server.
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
                Console.WriteLine("Failed to send message... Will abort.");
                OnFailed(e);
                return;
            }
            await Task.Delay(MessageMillisecondDelay);
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
}