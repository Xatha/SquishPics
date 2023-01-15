using Discord;

namespace SquishPicsDiscordBackend.Messaging;

public class TextMessage : IMessage
{
    public TextMessage(string message, IMessageChannel channel)
    {
        Message = message;
        Channel = channel;
    }

    public string Message { get; init; }
    public IMessageChannel Channel { get; init; }

    public Task SendAsync()
    {
        return Channel.SendMessageAsync(Message);
    }
}