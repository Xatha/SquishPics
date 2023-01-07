using SquishPics.Controllers;
using SquishPics.Controls;
using SquishPicsDiscordBackend;

namespace SquishPics;

public partial class SquishPicsForm : Form
{
    private readonly APIKeyForm _apiKeyForm;
    private readonly DiscordClient _client;
    private readonly ApiController _apiController;

    public SquishPicsForm(DiscordClient client, ApiController apiController)
    {
        KeyPreview = false;
        _apiKeyForm = new APIKeyForm();
        _client = client;
        _apiController = apiController;
        InitializeComponent();
        
        _apiKeyForm.VisibleChanged += APIKeyForm_VisibleChanged;
        ApiKeyButton.Click += ApiKeyButton_Click;
        ExceptionButton.Click += ExceptionButton_Click;
    }

    private async void ExceptionButton_Click(object? sender, EventArgs e)
    {
        await _client.StopAsync();
        //throw new Exception("Test Exception");
    }

    //TODO: Move these propagated events to a separate class
    private async void Form1_Load(object sender, EventArgs e)
    {
        await new ControlsContainer(this, _client, _apiController).InitializeControlsAsync();
    }

    private void ApiKeyButton_Click(object? sender, EventArgs e)
    {
        if (_apiKeyForm.Visible && Enabled) Enabled = false;

        if (_apiKeyForm.Visible) return;
        _apiKeyForm.Location = new Point(Location.X + Width/2 - _apiKeyForm.Width/2, Location.Y + Height/2 - _apiKeyForm.Height/2);
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