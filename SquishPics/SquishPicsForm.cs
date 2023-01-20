using System.ComponentModel;
using Discord;
using SquishPics.Controls;
using SquishPics.Forms;
using SquishPicsDiscordBackend;

namespace SquishPics;

public partial class SquishPicsForm : Form
{
    private readonly ApiKeyForm _apiKeyForm;
    private readonly WebPopup _webPopup;
    private readonly ControlsContainer _controls;
    private DiscordClient _client;

    public SquishPicsForm(ControlsContainer controls, DiscordClient client, WebPopup webPopup, ApiKeyForm apiKeyForm)
    {
        _controls = controls;
        _client = client;
        _webPopup = webPopup;
        _apiKeyForm = apiKeyForm;
        InitializeComponent();

        _apiKeyForm.VisibleChanged += APIKeyForm_VisibleChanged;
        ApiKeyButton.Click += ApiKeyButton_Click;
        ExceptionButton.Click += ExceptionButton_Click;
        Closing += SquishPicsForm_Closing;
        _client.AuthenticationNeeded += Client_AuthenticationNeeded;
    }
    

    private async void Client_AuthenticationNeeded(object sender, Func<Task> callback)
    {
        Enabled = false;
        await Task.Delay(50); // This makes sure the form is fully loaded before showing the popup.
        _webPopup.ShowDialog();
        if (_webPopup.DialogResult == DialogResult.OK) await callback();
        Enabled = true;
    }

    private void SquishPicsForm_Closing(object? sender, CancelEventArgs e)
    {
        _apiKeyForm.Dispose();
        _controls.Dispose();
    }

    private async void ExceptionButton_Click(object? sender, EventArgs e)
    {
    }

    //TODO: Move these propagated events to a separate class
    private async void Form1_Load(object sender, EventArgs e)
    {
        await _controls.InitializeControlsAsync(this);
        var key = await GlobalSettings.SafeGetSettingAsync<string>(SettingKeys.API_KEY);
        if (key != null) await _client.StartAsync(key);
    }

    private void ApiKeyButton_Click(object? sender, EventArgs e)
    {
        if (_apiKeyForm.Visible && Enabled) Enabled = false;

        if (_apiKeyForm.Visible) return;
        _apiKeyForm.Location = new Point(Location.X + Width / 2 - _apiKeyForm.Width / 2,
            Location.Y + Height / 2 - _apiKeyForm.Height / 2);
        _apiKeyForm.Show();
        _apiKeyForm.Focus();
    }

    private void APIKeyForm_VisibleChanged(object? sender, EventArgs e)
    {
        if (Visible && Enabled)
        {
            Enabled = false;
            return;
        }

        Enabled = true;
    }
}