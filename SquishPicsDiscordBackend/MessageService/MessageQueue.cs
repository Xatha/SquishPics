using System.Collections.Concurrent;

namespace SquishPicsDiscordBackend.MessageService;

public class MessageQueue : ConcurrentQueue<IMessage>
{
    private const int MessageMillisecondDelay = 1000; 
    private bool _isRunning = false;

    public MessageQueue(IEnumerable<IMessage> messages) : base(messages)
    {
        
    }

    public async Task SendMessagesAsync()
    {
        _isRunning = true;
        while (TryDequeue(out var message) && _isRunning)
        {
            await message.SendAsync();
            await Task.Delay(MessageMillisecondDelay);
        }
    }

    public Task StopSendingAsync()
    {
        _isRunning = false;
        return Task.CompletedTask;
    }
}