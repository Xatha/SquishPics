using Discord;

namespace SquishPicsDiscordBackend.Messaging;

public interface IMessage
{
    string Message { get; }
    IMessageChannel Channel { get; }

    Task SendAsync();
}