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
            Console.WriteLine("Red  ");
            StartStopButton.BackColor = Color.IndianRed;
            StartStopButton.Text = @"Stop Process";
        }
        else
        {
            Console.WriteLine("Green");
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
        if (_isRunning)
        {
            // Stop the process
            await CancelApiRequestAsync();
            /*var result = await CancelApiRequestAsync();
            if (!result) return; //TODO: Add exception/blowup.*/
            
            _isRunning = false;
            _isLocked = false;
            await Invoke(async ()=> await UpdateStyleAsync(false));
        }
        else
        {
            var server = ServerChannelSelectorControl.SelectedServer;
            var channel =  ServerChannelSelectorControl.SelectedTextChannel;
            
            if (_fileQueueControl.FileContents.Count <= 0)
            {
                MessageBox.Show(@"Please select files to send.");
                _isLocked = false;
                return;
            }
            
            if (server == null || channel == null)
            {
                MessageBox.Show(@"Please select a server and channel.");
                _isLocked = false;
                return;
            }
            
            
            
            
            
            
            if (MessageBox.Show(
                    $"Do you want to begin posting images> Images will be send in:\nServer: {server.Name}\nChannel: {channel.Name}",
                    @"Start Process?", MessageBoxButtons.YesNo) ==
                DialogResult.No)
            {
                _isLocked = false;
                return;
            }
            // If the request was successful, we start the process.
            // Start the process
            await Invoke(async ()=> await UpdateStyleAsync(true));
            
            var result = await StartApiRequestAsync();
            if (!result)
            {
                _isLocked = false;
                await Invoke(async ()=> await UpdateStyleAsync(false));
                return; // TODO: Add error message 
            }
            _isRunning = true;
        }
    }
    
    private async void _apiController_RequestFinished(object? sender, EventArgs e)
    {
        await Task.Delay(1000);
        _isRunning = false;
        _isLocked = false;
        await Invoke(async ()=> await UpdateStyleAsync(false));
    }

    private Task<bool> StartApiRequestAsync() => _apiController.StartProcessAsync(_fileQueueControl.FileContents);

    private Task CancelApiRequestAsync() => _apiController.CancelProcessAsync();
}