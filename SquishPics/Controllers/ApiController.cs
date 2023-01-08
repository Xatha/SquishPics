using SquishPics.APIHelpers;
using SquishPicsDiscordBackend;

namespace SquishPics.Controllers;

public sealed class ApiController
{
    private readonly CompressionServiceHelper _compressionServiceHelper;
    private readonly DiscordClient _discordClient;
    private readonly MessageServiceHelper _messageServiceHelper;
    private DirectoryInfo? _directoryInfo;
    private bool _handlingRequest;

    public ApiController(DiscordClient client, MessageServiceHelper messageServiceHelper,
        CompressionServiceHelper compressionServiceHelper)
    {
        _discordClient = client;
        _messageServiceHelper = messageServiceHelper;
        _compressionServiceHelper = compressionServiceHelper;

        _compressionServiceHelper.FileCompressed += CompressionServiceHelperOnFileCompressed;
        _messageServiceHelper.MessageQueueStopped += MessageServiceHelperOnMessageQueueStopped;

        _discordClient.OnConnected += DiscordClientConnectionConnected;
        _discordClient.OnDisconnected += DiscordClientConnectionDisconnected;
    }

    public bool HasConnection { get; set; }
    public event EventHandler? RequestFinished;

    private Task DiscordClientConnectionConnected()
    {
        return Task.FromResult(HasConnection = true);
    }

    private Task DiscordClientConnectionDisconnected(Exception exception)
    {
        return Task.FromResult(HasConnection = false);
    }

    //TODO: Fix firing of event twice.
    private void MessageServiceHelperOnMessageQueueStopped(object? sender, EventArgs e)
    {
        Console.WriteLine(@"Message queue stopped. Deleting files..."); //TODO logging........
        if (Directory.Exists(_directoryInfo?.FullName)) _directoryInfo?.Delete(true);
        _handlingRequest = false;
        OnRequestFinished();
    }

    //TODO: Fix duplicate firing and retries.
    public async Task<bool> StartProcessAsync(List<FileInfo> files)
    {
        if (files.Count == 0 || _handlingRequest) return false;
        var maxFileSizeInBytes = await GlobalSettings.SafeGetSettingAsync<int>(SettingKeys.MAX_FILE_SIZE) * 1048576;
        var filesToProcess = files.Where(x => x.Length / 1024 > maxFileSizeInBytes / 1024).ToList();

        if (filesToProcess.Count > 0) files = await CompressFilesAsync(files, filesToProcess, maxFileSizeInBytes);

        //Start the message service.
        try
        {
            await _messageServiceHelper.SetupQueueAsync(files.Select(file => file.FullName));
        }
        catch (InvalidOperationException e)
        {
            Console.WriteLine(e);
            return false;
        }

        _messageServiceHelper.StartQueueForget();
        return true;
    }

    public Task CancelProcessAsync()
    {
        return _messageServiceHelper.StopQueueAsync();
    }

    private async Task<List<FileInfo>> CompressFilesAsync(IEnumerable<FileInfo> files, List<FileInfo> filesToProcess,
        int maxFileSizeInBytes)
    {
        (_directoryInfo, filesToProcess) = await CopyFilesToTempDirectoryAsync(filesToProcess);

        //Start the compression service.
        await _compressionServiceHelper.StartAsync(filesToProcess, maxFileSizeInBytes);

        //Some of our files have been copied to a new location, so we need to update the list.
        return files.Select(file => filesToProcess.Find(info => file.Name == info.Name) ?? file).ToList();
    }

    private Task<(DirectoryInfo, List<FileInfo>)> CopyFilesToTempDirectoryAsync(List<FileInfo> files)
    {
        var tempDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        var directory = Directory.CreateDirectory(tempDirectoryPath);

        //TODO: CW
        Console.WriteLine($@"Copying {files.Count} files to temp directory: {directory.FullName}.");

        var result = new List<FileInfo>();
        foreach (var file in files)
        {
            var newFilePath = Path.Combine(directory.FullName, file.Name);
            File.Copy(file.FullName, newFilePath);
            result.Add(new FileInfo(newFilePath));
        }

        return Task.FromResult((directory, result));
    }

    private void CompressionServiceHelperOnFileCompressed(object? sender, string e)
    {
        Console.WriteLine(@$"File compressed: {e}");
    }

    private void OnRequestFinished()
    {
        RequestFinished?.Invoke(this, EventArgs.Empty);
    }
}