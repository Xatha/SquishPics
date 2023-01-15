using Discord;

namespace SquishPicsDiscordBackend.Messaging;

public class AttachmentMessage : IMessage
{
    public AttachmentMessage(string message, IMessageChannel channel)
    {
        Message = message;
        Channel = channel;
    }

    public string Message { get; }
    public IMessageChannel Channel { get; }

    public Task SendAsync()
    {
        return Channel.SendFileAsync(Message);
    }
}