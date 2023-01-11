using CompressionLibrary;
using log4net;
using SquishPics.APIHelpers;
using SquishPicsDiscordBackend;
using SquishPicsDiscordBackend.Logging;

namespace SquishPics.Controllers;

public sealed class ApiController
{
    private readonly ILog _log = LogProvider.GetLogger<ApiController>();
    private readonly CompressionServiceHelper _compressionServiceHelper;
    private readonly DiscordClient _discordClient;
    private readonly MessageServiceHelper _messageServiceHelper;
    private DirectoryInfo? _directoryInfo;
    private bool _handlingRequest;
    
    public bool HasConnection { get; private set; }
    
    public event EventHandler? RequestFinished;
    public event EventHandler<Status>? StatusChanged; 
    private Status _currentStatus;

    public ApiController(DiscordClient client, MessageServiceHelper messageServiceHelper,
        CompressionServiceHelper compressionServiceHelper)
    {
        _discordClient = client;
        _messageServiceHelper = messageServiceHelper;
        _compressionServiceHelper = compressionServiceHelper;

        _compressionServiceHelper.FileCompressed += CompressionServiceHelperOnFileCompressed;
        _messageServiceHelper.MessageQueueStopped += MessageServiceHelperOnMessageQueueStopped;
        _messageServiceHelper.MessageSent += MessageServiceHelperOnMessageSent;
            
        _discordClient.OnConnected += DiscordClientConnectionConnected;
        _discordClient.OnDisconnected += DiscordClientConnectionDisconnected;
    }

    ~ApiController()
    {
        _compressionServiceHelper.FileCompressed -= CompressionServiceHelperOnFileCompressed;
        _messageServiceHelper.MessageQueueStopped -= MessageServiceHelperOnMessageQueueStopped;

        _discordClient.OnConnected -= DiscordClientConnectionConnected;
        _discordClient.OnDisconnected -= DiscordClientConnectionDisconnected;
    }
    
    //TODO: Fix duplicate firing and retries.
    public async Task<bool> StartProcessAsync(List<FileInfo> files)
    {
        if (files.Count == 0 || _handlingRequest) return false;
        var maxFileSizeInBytes = await GlobalSettings.SafeGetSettingAsync<int>(SettingKeys.MAX_FILE_SIZE) * 1048576;
        var filesToProcess = files.Where(x => x.Length / 1024 > maxFileSizeInBytes / 1024).ToList();

        if (filesToProcess.Count > 0)
        {
            files = await BeginCompressionServiceAsync(files, filesToProcess, maxFileSizeInBytes);
        }
        
        //Start the message service.
        try
        {
            await _messageServiceHelper.SetupQueueAsync(files.Select(file => file.FullName));
        }
        catch (InvalidOperationException e)
        {
            await _log.WarnAsync("Failed to start message service.", e);
            return false;
        }
        
        _messageServiceHelper.StartQueueForget();
        return true;
    }

    public Task CancelProcessAsync() => _messageServiceHelper.StopQueueAsync();

    private async Task<List<FileInfo>> BeginCompressionServiceAsync(IEnumerable<FileInfo> files, 
        List<FileInfo> filesToProcess, int maxFileSizeInBytes)
    {
        OnStatusChanged(new Status
        {
            Message = "Beginning compression process...",
            WorkDone = 0,
            WorkRemaining = 0
        });
        
        (_directoryInfo, filesToProcess) = await CopyFilesToTempDirectoryAsync(filesToProcess);
        
        //Start the compression service.
        await _compressionServiceHelper.StartAsync(filesToProcess, maxFileSizeInBytes);

        //Some of our files have been copied to a new location, so we need to update the list.
        return files.Select(file => filesToProcess.Find(info => file.Name == info.Name) ?? file).ToList();
    }

    private async Task<(DirectoryInfo, List<FileInfo>)> CopyFilesToTempDirectoryAsync(List<FileInfo> files)
    {
        var tempDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var directory = Directory.CreateDirectory(tempDirectoryPath);
        
        await _log.DebugAsync($@"Copying {files.Count} files to temp directory: {directory.FullName}.");
        
        var result = new List<FileInfo>();
        foreach (var file in files)
        {
            OnStatusChanged(new Status
            {
                Message = "Copying files to temp directory...",
                WorkDone = _currentStatus.WorkDone + 1,
                WorkRemaining = files.Count
            });
            
            var newFilePath = Path.Combine(directory.FullName, file.Name);
            File.Copy(file.FullName, newFilePath);
            result.Add(new FileInfo(newFilePath));
        }

        return (directory, result);
    }
    
    private void OnRequestFinished() 
    {
        RequestFinished?.Invoke(this, EventArgs.Empty);
    }
    
    private void OnStatusChanged(Status e)
    {
        _currentStatus = e;
        StatusChanged?.Invoke(this, e);
    }

    #region Events
    private Task DiscordClientConnectionConnected() => Task.FromResult(HasConnection = true);
    
    private Task DiscordClientConnectionDisconnected(Exception exception) => Task.FromResult(HasConnection = false);
    
    private async void CompressionServiceHelperOnFileCompressed(object? sender, Progress e)
    {
        OnStatusChanged(new Status
        {
            Message = $"Compressed {e.FileProcessed}...",
            WorkDone = 0,
            WorkRemaining = e.FilesRemaining 
        });
        await _log.DebugAsync(@$"File compressed: {e}");
    }

    private async void MessageServiceHelperOnMessageQueueStopped(object? sender, EventArgs e)
    {
        await _log.InfoAsync("Finished sending messages.");
        if (Directory.Exists(_directoryInfo?.FullName)) _directoryInfo?.Delete(true);
        
        
        OnStatusChanged(new Status
        {
            Message = "Finished!",
            WorkDone = 0,
            WorkRemaining = 0
        });
        _handlingRequest = false;
        OnRequestFinished();
    }
    
    private void MessageServiceHelperOnMessageSent(object? sender, MessageProgress e)
    {
        StatusChanged?.Invoke(this, new Status
        {
            Message = $"Sending messages... Remaining: {e.MessagesTotal - e.MessagesSent}",
            WorkDone = e.MessagesSent,
            WorkRemaining = e.MessagesTotal
        });
    }

    #endregion
}