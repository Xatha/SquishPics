using System.Windows.Forms;

namespace SquishPics.Controls;

public partial class StartStopButtonControl : UserControl
{
    private bool _isRunning;
    private bool _isLocked;
    
    public StartStopButtonControl()
    {
        _isRunning = false;
        
        InitializeComponent();
        StartStopButton.Click += StartStopButton_Click;
    }

    private Task UpdateStyleAsync()
    {
        if (_isRunning)
        {
            StartStopButton.BackColor = Color.IndianRed;
            StartStopButton.Text = @"Stop Process";
        }
        else
        {
            StartStopButton.BackColor = Color.PaleGreen;
            StartStopButton.Text = @"Start Process";
        }
        return Task.CompletedTask;
    }
    private async void StartStopButton_Click(object? sender, EventArgs e)
    {
        // We lock the button since we don't want the user to be able to spam the button or click it while it's processing a request.
        if (_isLocked) return;

        _isLocked = true;
        _isRunning = !_isRunning;
        await Task.Delay(2000);
        await UpdateStyleAsync();
        _isLocked = false;
    }
    
    
}