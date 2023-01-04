using Discord;

namespace SquishPicsDiscordBackend.MessageService;

public class TextMessage : IMessage
{
    public string Message { get; init; }
    public IMessageChannel Channel { get; init; }
    
    public TextMessage(string message, IMessageChannel channel)
    {
        Message = message;
        Channel = channel;
    }
    public Task SendAsync() => Channel.SendMessageAsync(Message);
}