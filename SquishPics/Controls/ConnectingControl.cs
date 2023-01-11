using log4net;
using SquishPicsDiscordBackend;
using SquishPicsDiscordBackend.Logging;

namespace SquishPics.Controls;

public partial class ConnectingControl : UserControl
{
    private readonly ILog _log = LogProvider.GetLogger<ConnectingControl>();
    private readonly DiscordClient _discordClient;
    private State _state;

    public ConnectingControl(DiscordClient discordClient)
    {
        InitializeComponent();
        _discordClient = discordClient;
        _state = State.Disconnected;

        _discordClient.OnConnected += discordClient_OnConnected;
        _discordClient.OnDisconnected += _discordClient_OnDisconnected;
    }

    private Task discordClient_OnConnected()
    {
        Invoke(() =>
        {
            if (_state == State.Connected) return Task.CompletedTask;

            _state = State.Connected;
            button1.Enabled = false;
            button1.BackColor = Color.PaleGreen;
            button1.Text = @"Connected!";
            return Task.CompletedTask;
        });
        return Task.CompletedTask;
    }

    private Task _discordClient_OnDisconnected(Exception arg)
    {
        Invoke(() =>
        {
            if (_state == State.Disconnected) return Task.CompletedTask;

            _state = State.Disconnected;
            button1.Enabled = true;
            button1.BackColor = Color.IndianRed;
            button1.Text = @"Disconnected... Click to reconnect!";
            return Task.CompletedTask;
        });
        return Task.CompletedTask;
    }

    private async void button1_Click(object sender, EventArgs e)
    {
        if (await GlobalSettings.SafeGetSettingAsync<string>(SettingKeys.API_KEY) is var key && key is null)
        {
            await _log.WarnAsync("No API key set, cannot connect to Discord");
            return;
        }

        await _discordClient.RetryLoginAsync(key);
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            components?.Dispose();
            _discordClient.OnConnected -= discordClient_OnConnected;
            _discordClient.OnDisconnected -= _discordClient_OnDisconnected;
        }

        base.Dispose(disposing);
    }

    private enum State
    {
        Connected,
        Disconnected
    }
}