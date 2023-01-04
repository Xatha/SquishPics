using CompressionLibrary;
using SquishPicsDiscordBackend;

namespace SquishPics.Controls;

public partial class StartStopButtonControl : UserControl
{
    private bool _isRunning;
    private bool _isLocked;
    private readonly FileQueueControl _fileQueueControl;
    private readonly DiscordClient _client;
    
    public StartStopButtonControl(DiscordClient client, FileQueueControl fileQueueControl)
    {
        _isRunning = false;
        _isLocked = false;
        _fileQueueControl = fileQueueControl;
        _client = client;
        
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
        
        if (_isRunning)
        {
            // Stop the process
            var result = await CancelApiRequest();
            if (!result) return; //TODO: Add exception/blowup.
        
            _isRunning = false;
            await UpdateStyleAsync();
        }
        else
        {
            // Start the process
            var result = await StartApiRequest();
            if (!result) return; // TODO: Add error message
            
            // If the request was successful, we start the process.
            _isRunning = true;
            await UpdateStyleAsync();
            
        }
        _isLocked = false;
    }

    private async Task<bool> StartApiRequest()
    {
        var files = _fileQueueControl.FileContents;
        if (files.Count == 0) return false;
        
        //We convert MiB to KiB.
        var maximumFileSize = await GlobalSettings.SafeGetSettingAsync<int>(SettingKeys.MAX_FILE_SIZE) * 1024;
        var filesToCompress = files.Where(x => x.Length / 1024 > maximumFileSize).ToList();
        if (filesToCompress.Count == 0) return false;

        // ImageCompressor accepts maximumFileSize in bits.
        var imageCompressor = await ImageCompressor.CreateAsync(filesToCompress, maximumFileSize * 1024);
        
        imageCompressor.FileCompressed += (_, s) =>
        {
            Console.WriteLine($@"Compressed {s}");
        };
        //Copy files that need to be compressed to a temp directory.
        var tempDirectory = await imageCompressor.CopyFilesToTempDirectoryAsync();

        await imageCompressor.StartCompressionAsync();
        
        
        //tempDirectory.Delete(true);
        
        
        //throw new NotImplementedException();
        return true;
    }

    private Task<bool> CancelApiRequest()
    {
        return Task.FromResult(true);
    }
}