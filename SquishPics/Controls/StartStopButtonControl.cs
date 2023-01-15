using Discord;
using SquishPicsDiscordBackend;
using SquishPicsDiscordBackend.Controllers;
using Color = System.Drawing.Color;

namespace SquishPics.Controls;

public partial class StartStopButtonControl : UserControl
{
    private readonly RequestController _requestController;
    private readonly DataProcessor _dataProcessor;
    private readonly FileQueueControl _fileQueueControl;
    private bool _isLocked;
    private bool _isRunning;

    public StartStopButtonControl(RequestController requestController, DataProcessor dataProcessor, FileQueueControl fileQueueControl)
    {
        _requestController = requestController;
        _dataProcessor = dataProcessor;
        _fileQueueControl = fileQueueControl;
        _isRunning = false;
        _isLocked = false;

        InitializeComponent();
        StartStopButton.Click += StartStopButton_Click;
        _requestController.RequestCancelled += RequestControllerOnRequestCancelled;
        _requestController.RequestFailed += RequestControllerOnRequestFailed;
        _requestController.RequestCompleted += RequestControllerOnRequestFinished;
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

        //if (await _apiController.StartProcessAsync(_fileQueueControl.Items))
        var maxFileSize = await GlobalSettings.SafeGetSettingAsync<int>(SettingKeys.MAX_FILE_SIZE);
        await _dataProcessor.ProcessAsync(_fileQueueControl.Items, maxFileSize);
        if (await _requestController.SendRequestAsync(
                ServerChannelSelectorControl.SelectedTextChannel!))
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
        await _requestController.CancelRequestAsync();
        _isRunning = false;
    }

    private async Task<bool> ValidateStateAsync()
    {
        //if (!_apiController.HasConnection)
        if (DiscordClient.ConnectionState != ConnectionState.Connected)
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
        var channel = ServerChannelSelectorControl.SelectedTextChannel;
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
            await CancelApiRequestAsync();
        else
            await StartRequestAsync();
        await Task.Delay(200);
        _isLocked = false;
    }

    #region Events
    
    private async void RequestControllerOnRequestFinished(object? sender, EventArgs e)
    {
        Console.WriteLine("Request finished");
        await Task.Delay(1000);
        await Invoke(async () => await UpdateStyleAsync(false));
        _isRunning = false;
    }
    
    private async void RequestControllerOnRequestCancelled(object? sender, EventArgs e)
    {
        Console.WriteLine("Request cancelled");
        await Task.Delay(1000);
        await Invoke(async () => await UpdateStyleAsync(false));
        _isRunning = false;
    }

    private async void RequestControllerOnRequestFailed(object? sender, RequestFailureReason e)
    {
        MessageBox.Show($"Request failed: {e}", "Request failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        await Task.Delay(1000);
        await Invoke(async () => await UpdateStyleAsync(false));
        _isRunning = false;
    }
    #endregion
}