using Discord;

namespace SquishPicsDiscordBackend.MessageService;

public interface IMessage
{
    string Message { get; }
    IMessageChannel Channel { get; }
    
    Task SendAsync();
}