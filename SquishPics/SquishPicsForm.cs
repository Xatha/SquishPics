using SquishPics.Controls;

namespace SquishPics;

public partial class SquishPicsForm : Form
{
    private readonly APIKeyForm _APIKeyForm;
    
    public SquishPicsForm()
    {
        _APIKeyForm = new APIKeyForm();
        InitializeComponent();
        InitializeEvents();
    }

    private void InitializeEvents()
    {
        _APIKeyForm.VisibleChanged += APIKeyForm_VisibleChanged;
    }

    //TODO: Move these propagated events to a separate class
    private async void Form1_Load(object sender, EventArgs e)
    {
        await ControlsInitializer.InitializeControls(this);
    }

    private void APIKeyForm_VisibleChanged(object? sender, EventArgs e)
    {
        if (_APIKeyForm.Visible && Enabled)
        {
            Enabled = false;
            return;
        }
        Enabled = true;
    }

    private void ApiKeyButton_Click(object sender, EventArgs e)
    {
        if (_APIKeyForm.Visible && Enabled) Enabled = false;

        if (_APIKeyForm.Visible) return;
        _APIKeyForm.Location = new Point(Location.X + 40, Location.Y + 40);
        _APIKeyForm.Show();
        _APIKeyForm.Focus();
    }
}