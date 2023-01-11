using SquishPics.Controllers;
using SquishPicsDiscordBackend.RetryHelpers;
using Timer = System.Windows.Forms.Timer;

namespace SquishPics.Controls;

public partial class StatusControl : UserControl
{
    private readonly ApiController _apiController;
    
    public StatusControl(ApiController apiController)
    {
        _apiController = apiController;
        _apiController.StatusChanged += ApiControllerOnStatusChanged;
        InitializeComponent();
        StatusProgressBar.Style = ProgressBarStyle.Blocks;
    }

    private void TimerOnTick(object? sender, EventArgs e)
    {
        Invoke(()=> StatusProgressBar.Value += 10);
    }

    private void ApiControllerOnStatusChanged(object? sender, Status e)
    {
        Invoke(() => StatusProgressBarLabel.Text = $"Status: {e.Message}");
        if (e.WorkRemaining == 0)
        {
            Invoke(() => StatusProgressBar.Value = 0);
            return;
        }
        
        var progress = 100 * (float)(e.WorkRemaining - Math.Abs(e.WorkDone - e.WorkRemaining)) / e.WorkRemaining;
        Invoke(() => StatusProgressBar.Value = (int)progress);
    }

    ~StatusControl() => _apiController.StatusChanged -= ApiControllerOnStatusChanged;
}