using Accessibility;
using SquishPicsDiscordBackend;

namespace SquishPics.Controls;

public partial class ConnectingControl : UserControl
{
    private readonly DiscordClient _discordClient;
    private State _state;

    private ConnectingControl()
    {
        InitializeComponent();
    }

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
            button1.Text = "Connected!";

            Console.WriteLine("Connected");
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
            button1.Text = "Disconnected... Click to reconnect!";

            Console.WriteLine("Disconnected");
            return Task.CompletedTask;
        });
        return Task.CompletedTask;
    }

    private async void button1_Click(object sender, EventArgs e)
    {
        await _discordClient.RetryLoginAsync(GlobalSettings.Default.API_KEY);
    }

    private enum State
    {
        Connected,
        Disconnected
    }
}


