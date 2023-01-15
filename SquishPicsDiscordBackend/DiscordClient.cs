using Discord;
using Discord.Rest;
using Discord.WebSocket;
using log4net;
using SquishPicsDiscordBackend.Logging;

[assembly: log4net.Config.XmlConfigurator(Watch = true, ConfigFile = "log4net.config")]
namespace SquishPicsDiscordBackend;

public class DiscordClient
{
    private readonly ILog _log;
    private readonly DiscordSocketClient _socketClient;

    public DiscordClient(ILog log)
    {
        _log = log;
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds
        };

        _socketClient = new DiscordSocketClient(config);
        _socketClient.Log += _log.LogClientAsync;

        _socketClient.Connected += () => Task.FromResult(ConnectionState = ConnectionState.Connected);
        _socketClient.Disconnected += _ => Task.FromResult(ConnectionState = ConnectionState.Disconnected);
    }

    public static ConnectionState ConnectionState { get; private set; } = ConnectionState.Disconnected;

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

    public async Task RetryLoginAsync(string apiKey)
    {
        await _log.DebugAsync("Retrying connecting to Discord client...");

        await _socketClient.StopAsync();
        await _socketClient.LoginAsync(TokenType.Bot, apiKey);
        await _socketClient.StartAsync();
    }

    public async Task StartAsync(string apiKey)
    {
        await _socketClient.LoginAsync(TokenType.Bot, apiKey);
        await _socketClient.StartAsync();
    }

    public async Task StopAsync()
    {
        await _log.DebugAsync("Stopping Discord client...");
        await _socketClient.StopAsync();
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
}