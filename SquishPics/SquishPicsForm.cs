using SquishPics.Controls;
using SquishPicsDiscordBackend;

namespace SquishPics;

public partial class SquishPicsForm : Form
{
    private readonly APIKeyForm _APIKeyForm;
    private readonly DiscordClient _client;
    
    public SquishPicsForm(DiscordClient client)
    {
        _APIKeyForm = new APIKeyForm();
        _client = client;
        InitializeComponent();
        
        _APIKeyForm.VisibleChanged += APIKeyForm_VisibleChanged;
        ApiKeyButton.Click += ApiKeyButton_Click;
    }

    //TODO: Move these propagated events to a separate class
    private async void Form1_Load(object sender, EventArgs e)
    {
        await new ControlsContainer(this, _client).InitializeControls();
    }

    private void ApiKeyButton_Click(object? sender, EventArgs e)
    {
        if (_APIKeyForm.Visible && Enabled) Enabled = false;

        if (_APIKeyForm.Visible) return;
        _APIKeyForm.Location = new Point(Location.X + 40, Location.Y + 40);
        _APIKeyForm.Show();
        _APIKeyForm.Focus();
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