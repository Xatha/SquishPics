using System.ComponentModel;
using SquishPics.Controls;

namespace SquishPics;

public partial class SquishPicsForm : Form
{
    private readonly ApiKeyForm _apiKeyForm;
    private readonly ControlsContainer _controls;

    public SquishPicsForm(ControlsContainer controls)
    {
        KeyPreview = false;
        _apiKeyForm = new ApiKeyForm();
        _controls = controls;
        InitializeComponent();

        _apiKeyForm.VisibleChanged += APIKeyForm_VisibleChanged;
        ApiKeyButton.Click += ApiKeyButton_Click;
        ExceptionButton.Click += ExceptionButton_Click;
        Closing += SquishPicsForm_Closing;
    }

    private void SquishPicsForm_Closing(object? sender, CancelEventArgs e)
    {
        _apiKeyForm.Dispose();
        _controls.Dispose();
    }

    private async void ExceptionButton_Click(object? sender, EventArgs e)
    {
        //await Program._client.StopAsync();
        //throw new Exception("Test Exception");
    }

    //TODO: Move these propagated events to a separate class
    private async void Form1_Load(object sender, EventArgs e)
    {
        await _controls.InitializeControlsAsync(this);
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