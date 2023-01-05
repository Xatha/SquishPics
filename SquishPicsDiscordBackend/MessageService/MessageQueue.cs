using System.Collections.Concurrent;

namespace SquishPicsDiscordBackend.MessageService;

public sealed class MessageQueue : ConcurrentQueue<IMessage>
{
    private const int MessageMillisecondDelay = 1000; 
    private bool _isRunning;

    public event EventHandler? Finished;
    
    public MessageQueue(IEnumerable<IMessage> messages) : base(messages)
    {
        
    }

    public async Task StartSendingAsync()
    {
        _isRunning = true;
        while (TryDequeue(out var message) && _isRunning)
        {
            await message.SendAsync();
            await Task.Delay(MessageMillisecondDelay);
        }

        OnFinished();
    }

    public Task StopSendingAsync()
    {
        _isRunning = false;
        return Task.CompletedTask;
    }

    private void OnFinished()
    {
        Finished?.Invoke(this, EventArgs.Empty);
    }
}