using Discord;
using Microsoft.VisualStudio.Threading;
using SquishPics.Controls;
using SquishPicsDiscordBackend;
using SquishPicsDiscordBackend.MessageService;

namespace SquishPics.APIHelpers;

public class MessageServiceHelper
{
    private readonly DiscordClient _client;
    public MessageQueue? MessageQueue { get; private set; }

    public event EventHandler? MessageQueueStarted;
    public event EventHandler? MessageQueuePause;
    public event EventHandler? MessageQueueStopped;
    
    public MessageServiceHelper(DiscordClient client)
    {
        _client = client;
    }
    
    //TODO: Add link posting support.
    public async Task SetupQueueAsync(IEnumerable<string> filePaths)
    {
        if (MessageQueue != null) throw new Exception("Current queue is not finished.");
        
        var selectedMessageChannel = await GetSelectedMessageChannelAsync();
        var messages = filePaths.Select(path => new AttachmentMessage(path, selectedMessageChannel));
        MessageQueue = new MessageQueue(messages);
        
        MessageQueue.Finished += (_, _) =>
        {
            MessageQueue = null;
            OnMessageQueueStopped();
        };
    }

    //TODO: Add better error handling.
    public void StartQueueForget() => MessageQueue?.StartSendingAsync().Forget();

    public async Task PauseQueueAsync()
    {
        if (MessageQueue is null) throw new Exception("Message queue is not initialized.");
        await MessageQueue.StopSendingAsync();
        OnMessageQueuePause();
    }

    public async Task StopQueueAsync()
    {
        if (MessageQueue is null) throw new Exception("Message queue is not initialized.");
        await MessageQueue.StopSendingAsync();
        OnMessageQueueStopped();
    }

    private async Task<IMessageChannel> GetSelectedMessageChannelAsync()
    {
        var selectedServer = ServerChannelSelectorControl.SelectedServer;
        var selectedChannel = ServerChannelSelectorControl.SelectedTextChannel;
        
        if (selectedServer is null || selectedChannel is null)
            throw new Exception("Server or channel is not selected.");
        
        var server  = (await _client.GetServersAsync()).FirstOrDefault(server => server.Name == selectedServer.Name);
        if (server == null) throw new Exception("Server not found."); //TODO: Proper exception.

        var messageChannel = (await _client.GetChannelsAsync(server)).FirstOrDefault(messageChannel => messageChannel.Name == selectedChannel.Name);
        return messageChannel ?? throw new Exception("Channel not found."); //TODO: Proper exception.
    }

    protected virtual void OnMessageQueueStarted()
    {
        MessageQueueStarted?.Invoke(this, EventArgs.Empty);
    }
    
    protected virtual void OnMessageQueuePause()
    {
        MessageQueuePause?.Invoke(this, EventArgs.Empty);
    }
    
    protected virtual void OnMessageQueueStopped()
    {
        MessageQueueStopped?.Invoke(this, EventArgs.Empty);
    }
}