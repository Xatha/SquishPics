using Discord;

namespace SquishPicsDiscordBackend.MessageService;

public class AttachmentMessage : IMessage
{
    public string Message { get; }
    public IMessageChannel Channel { get; }
    
    public AttachmentMessage(string message, IMessageChannel channel)
    {
        Message = message;
        Channel = channel;
    }
    
    public Task SendAsync() => Channel.SendFileAsync(Message);
}