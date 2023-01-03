using Discord;
using Discord.Rest;
using Discord.WebSocket;

namespace SquishPicsDiscordBackend;

public class DiscordClient
{
    private readonly DiscordSocketClient _socketClient;

    public DiscordClient(string APIKey)
    {
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds
        };

        _socketClient = new DiscordSocketClient(config);
        _socketClient.Log += Log;

        _socketClient.LoginAsync(TokenType.Bot, APIKey);
        _socketClient.StartAsync();
    }

    public event Func<Task> OnConnected
    {
        add => _socketClient.Connected += value;
        remove => _socketClient.Connected -= value;
    }

    public event Func<Exception, Task> OnDisconnected
    {
        add => _socketClient.Disconnected += value;
        remove => _socketClient.Disconnected -= value;
    }

    public async Task RetryLoginAsync(string APIKey)
    {
        await _socketClient.StopAsync();
        Console.WriteLine("Retry");
        await _socketClient.LoginAsync(TokenType.Bot, APIKey);
        await _socketClient.StartAsync();
    }
    
    public Task<IReadOnlyCollection<RestGuild>> GetServersAsync() => _socketClient.Rest.GetGuildsAsync();

    public Task<IEnumerable<SocketTextChannel>> GetChannelsAsync(RestGuild server)
    {
        var socketGuild = _socketClient.GetGuild(server.Id);
        if (socketGuild is null) return Task.FromResult(Enumerable.Empty<SocketTextChannel>());

        var channels = socketGuild.TextChannels.Where(textChannel =>
            socketGuild.GetUser(_socketClient.CurrentUser.Id).GetPermissions(textChannel) is
                { SendMessages: true, ViewChannel: true, AttachFiles: true, EmbedLinks: true }
            && socketGuild.VoiceChannels.All(voiceChannel => voiceChannel.Name != textChannel.Name));

        return Task.FromResult(channels);
    }

    private static Task Log(LogMessage arg)
    {
        Console.WriteLine(arg);
        return Task.CompletedTask;
    }
}