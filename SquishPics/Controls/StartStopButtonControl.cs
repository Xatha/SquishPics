using Discord.Rest;
using Discord.WebSocket;
using SquishPics.Controllers;

namespace SquishPics.Controls;

public partial class StartStopButtonControl : UserControl
{
    private bool _isRunning;
    private bool _isLocked;
    
    private readonly ApiController _apiController;
    private readonly FileQueueControl _fileQueueControl;

    public StartStopButtonControl(ApiController apiController, FileQueueControl fileQueueControl)
    {
        _apiController = apiController;
        _fileQueueControl = fileQueueControl;
        _isRunning = false;
        _isLocked = false;
        
        InitializeComponent();
        StartStopButton.Click += StartStopButton_Click;
        _apiController.RequestFinished += _apiController_RequestFinished;
    }

    private Task UpdateStyleAsync(bool isRunning)
    {
        if (isRunning)
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
    
    private async Task StartRequestAsync()
    {
        if (!await ValidateStateAsync()) return;
        
        await Invoke(async () => await UpdateStyleAsync(true));

        if (await _apiController.StartProcessAsync(_fileQueueControl.Items))
        {
            _isRunning = true;
        }
        else
        {
            await Invoke(async () => await UpdateStyleAsync(false));
        }
    }

    private async Task CancelApiRequestAsync()
    {
        await Invoke(async () => await UpdateStyleAsync(false));
        _isRunning = false;
        await _apiController.CancelProcessAsync();
    } 
    private async Task<bool> ValidateStateAsync()
    {
        
        if (!_apiController.HasConnection)
        {
            await StandardResponses.NoConnectionAsync();
            _isLocked = false;
            return false;
        }

        if (_fileQueueControl.Items.Count <= 0)
        {
            await StandardResponses.NoFilesSelectedAsync();
            _isLocked = false;
            return false;
        }

        var server = ServerChannelSelectorControl.SelectedServer;
        var channel =  ServerChannelSelectorControl.SelectedTextChannel;
        if (server == null || channel == null)
        {
            await StandardResponses.NoChannelSelectedAsync();
            _isLocked = false;
            return false;
        }

        if (await StandardResponses.ConfirmationAsync(server.Name, channel.Name))
        {
            _isLocked = false;
            return false;
        }
        return true;
    }
    
    private async void StartStopButton_Click(object? sender, EventArgs e)
    {
        if (_isLocked) return;
        _isLocked = true;
        if (_isRunning)
        { 
            await CancelApiRequestAsync();
        }
        else
        {
            await StartRequestAsync();
        }
        await Task.Delay(200);
        _isLocked = false;
    }

    private async void _apiController_RequestFinished(object? sender, EventArgs e)
    {
        await Task.Delay(1000);
        await Invoke(async () => await UpdateStyleAsync(false));
        _isRunning = false;
    }
}