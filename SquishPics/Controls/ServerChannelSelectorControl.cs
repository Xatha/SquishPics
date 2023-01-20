using Discord.Rest;
using Discord.WebSocket;
using SquishPicsDiscordBackend;

namespace SquishPics.Controls;

public partial class ServerChannelSelectorControl : UserControl
{
    private readonly DiscordClient _client;
    private IReadOnlyCollection<RestGuild>? _guilds;
    private IEnumerable<SocketTextChannel>? _textChannels;

    public ServerChannelSelectorControl(DiscordClient client)
    {
        _client = client;
        InitializeComponent();

        Load += ServerChannelSelectorControl_Load;
        _client.OnConnected += ClientOnOnConnectedAsync;
        _client.OnDisconnected += ClientOnOnDisconnectedAsync;
    }

    //TODO: Technically this is a bad idea, but we will never have more than one instance of this control.
    public static RestGuild? SelectedServer { get; private set; }
    public static SocketTextChannel? SelectedTextChannel { get; private set; }

    private Task ClientOnOnDisconnectedAsync(Exception arg)
    {
        _guilds = null;
        _textChannels = null;
        SelectedServer = null;
        SelectedTextChannel = null;
        Invoke(() =>
        {
            ChannelListBox.Items.Clear();
            ServerListBox.Items.Clear();
        });
        return Task.CompletedTask;
    }

    private async Task ClientOnOnConnectedAsync() => await LoadServersAndChannelsAsync();

    private async void ServerChannelSelectorControl_Load(object? sender, EventArgs e) 
        => await LoadServersAndChannelsAsync();

    private async void ServerListBox_SelectedIndexChanged(object sender, EventArgs e) 
        => await LoadChannelsAsync();

    private async Task LoadServersAsync()
    {
        _guilds = await _client.GetServersAsync();
        if (_guilds is null) return;
        Invoke(() =>
        {
            ServerListBox.Items.Clear();
            ServerListBox.Items.AddRange(_guilds.Select(guild => guild.Name).ToArray<object>());
        });
    }

    private async Task LoadChannelsAsync()
    {
        var selectedServer = Invoke(() => ServerListBox.SelectedItem?.ToString());
        if (selectedServer is null || selectedServer == SelectedServer?.Name) return;

        SelectedTextChannel = null;
        SelectedServer = _guilds?.FirstOrDefault(guild => guild.Name == selectedServer);

        if (SelectedServer is null) return;

        _textChannels = await _client.GetChannelsAsync(SelectedServer);

        var textChannels = await _client.GetChannelsAsync(SelectedServer);
        Invoke(() =>
        {
            ChannelListBox.Items.Clear();
            ChannelListBox.Items.AddRange(textChannels.Select(textChannel => textChannel.Name).ToArray<object>());
        });
    }

    private async Task LoadServersAndChannelsAsync()
    {
        //if (_guilds is null) await LoadServersAsync();
        //if (_textChannels is null) await LoadChannelsAsync();
        await LoadServersAsync();
        await LoadChannelsAsync();
    }

    private void ChannelListBox_SelectedIndexChanged(object sender, EventArgs e)
    {
        var selectedChannel = ChannelListBox.SelectedItem?.ToString();
        if (selectedChannel is null || selectedChannel == SelectedTextChannel?.Name) return;
        SelectedTextChannel = _textChannels?.FirstOrDefault(textChannel => textChannel.Name == selectedChannel);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
            _client.OnConnected -= ClientOnOnConnectedAsync;
            _client.OnDisconnected -= ClientOnOnDisconnectedAsync;
        }

        base.Dispose(disposing);
    }
}