using Discord;
using SquishPics.Controls;
using SquishPicsDiscordBackend;
using SquishPicsDiscordBackend.MessageService;
using SquishPicsDiscordBackend.RetryHelpers;
using MessageBox = System.Windows.Forms.MessageBox;

namespace SquishPics.APIHelpers;

public sealed class MessageServiceHelper
{
    private readonly DiscordClient _client;
    private readonly MessageQueue _messageQueue;
    public event EventHandler? MessageQueueStopped;
    
    public MessageServiceHelper(DiscordClient client)
    {
        _client = client;
        _messageQueue = new MessageQueue(client);
        _messageQueue.Finished += MessageQueue_Finished;
        _messageQueue.Failed += MessageQueue_Failed;
    }

    //TODO: Add link posting support.
    public async Task SetupQueueAsync(IEnumerable<string> filePaths)
    {
        if (!_messageQueue.IsEmpty) throw new InvalidOperationException("The queue is not emptied yet.");

        var selectedMessageChannel = await GetSelectedMessageChannelAsync();
        var messages = filePaths.Select(path => new AttachmentMessage(path, selectedMessageChannel)).ToList();
        messages.ForEach(_messageQueue.Enqueue);
    }
    
    public void StartQueueForget() => Task.Run(() => _messageQueue.StartSendingAsync()).ConfigureAwait(false);
    
    public async Task StopQueueAsync()
    {
        await _messageQueue.StopSendingAsync(); 
        _messageQueue.Clear();
    }
    
    private void MessageQueue_Finished(object? o, EventArgs eventArgs) => OnMessageQueueStopped();

    //TODO: Add logging.
    private void MessageQueue_Failed(object? o, RetryTimeoutException retryTimeoutException)
    {
        MessageBox.Show(@"Failed to send message. This is likely because the application could not establish connecting to Discord for a prolonged period.",
            @"Error - Failed to send messages.", MessageBoxButtons.OK, MessageBoxIcon.Error);
        OnMessageQueueStopped();
    }

    private async Task<IMessageChannel> GetSelectedMessageChannelAsync()
    {
        var selectedServer = ServerChannelSelectorControl.SelectedServer;
        var selectedChannel = ServerChannelSelectorControl.SelectedTextChannel;
        
        if (selectedServer is null || selectedChannel is null)
            throw new NullReferenceException("Server or channel is not selected.");
        
        var server  = (await _client.GetServersAsync()).FirstOrDefault(server => server.Name == selectedServer.Name);
        if (server == null) throw new NullReferenceException("Server not found."); //TODO: Proper exception.

        var messageChannel = (await _client.GetChannelsAsync(server)).FirstOrDefault(messageChannel => messageChannel.Name == selectedChannel.Name);
        return messageChannel ?? throw new NullReferenceException("Channel not found."); //TODO: Proper exception.
    }

    private void OnMessageQueueStopped()
    {
        MessageQueueStopped?.Invoke(this, EventArgs.Empty);
    }
}