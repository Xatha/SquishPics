using Discord;
using Discord.Rest;
using Discord.WebSocket;
using log4net;
using SquishPicsDiscordBackend.Logging;
using SquishPicsDiscordBackend.OAuth2;

[assembly: log4net.Config.XmlConfigurator(Watch = true, ConfigFile = "log4net.config")]
namespace SquishPicsDiscordBackend;

public class DiscordClient
{
    private readonly ILog _log;
    private readonly DiscordSocketClient _socketClient;
    private readonly DiscordOAuth2 _authentication;
    public event EventHandler<Func<Task>>? AuthenticationNeeded;


    public DiscordClient(ILog log, DiscordOAuth2 authentication)
    {
        _log = log;
        _authentication = authentication;
        
        var config = new DiscordSocketConfig
        {
            GatewayIntents = GatewayIntents.Guilds | GatewayIntents.GuildMembers,
            AlwaysDownloadUsers = true
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
        TokenUtils.ValidateToken(TokenType.Bot, apiKey);
        await _log.DebugAsync("Retrying connecting to Discord client...");

        await _socketClient.StopAsync();
        await StartAsync(apiKey);
    }

    public async Task StartAsync(string apiKey)
    {
        if (_authentication.TokenExpired)
        {
            OnAuthenticationRequired( () => StartAsync(apiKey));
            return;
        }
        
        await _socketClient.LoginAsync(TokenType.Bot, apiKey);
        await _socketClient.StartAsync();
    }

    public async Task StopAsync()
    {
        await _log.DebugAsync("Stopping Discord client...");
        await _socketClient.StopAsync();
    }

    public async Task<IReadOnlyCollection<RestGuild>?> GetServersAsync()
    {
        if (_socketClient.ConnectionState != ConnectionState.Connected || _authentication.Token == null) return null;

        var guilds = await _socketClient.Rest.GetGuildsAsync();
        
        var accessibleGuilds = new List<RestGuild>();
        foreach (var guild in guilds)
        {
            var socketGuild = _socketClient.GetGuild(guild.Id);
            await socketGuild.DownloadUsersAsync();
            if (socketGuild.GetUser(_authentication.Token.UserId) != null) accessibleGuilds.Add(guild);
        }

        return accessibleGuilds;
    } 

    public Task<IEnumerable<SocketTextChannel>> GetChannelsAsync(RestGuild server)
    {
        if (_socketClient.ConnectionState != ConnectionState.Connected) return Task.FromResult(Enumerable.Empty<SocketTextChannel>());
        
        var socketGuild = _socketClient.GetGuild(server.Id);
        if (socketGuild is null) return Task.FromResult(Enumerable.Empty<SocketTextChannel>());

        /*var channels = socketGuild.TextChannels.Where(textChannel =>
            socketGuild.GetUser(_socketClient.CurrentUser.Id).GetPermissions(textChannel) is
                { SendMessages: true, ViewChannel: true, AttachFiles: true, EmbedLinks: true }
            && socketGuild.VoiceChannels.All(voiceChannel => voiceChannel.Name != textChannel.Name));*/

        if (_authentication.Token == null) return null!; 
    
        var botUser = socketGuild.GetUser(_socketClient.CurrentUser.Id);
        var loggedInUser = socketGuild.GetUser(_authentication.Token.UserId);

        IEnumerable<SocketTextChannel> channelQuery = 
            from textChannel in socketGuild.TextChannels
            where loggedInUser.GetPermissions(textChannel) is { SendMessages: true, ViewChannel: true, AttachFiles: true, EmbedLinks: true }
            where botUser.GetPermissions(textChannel)      is { SendMessages: true, ViewChannel: true, AttachFiles: true, EmbedLinks: true }
            where socketGuild.VoiceChannels.All(voiceChannel => voiceChannel.Name != textChannel.Name)
            select textChannel;
    
        return Task.FromResult(channelQuery);
    }

    protected virtual void OnAuthenticationRequired(Func<Task> e)
    {
        AuthenticationNeeded?.Invoke(this, e);
    }
}