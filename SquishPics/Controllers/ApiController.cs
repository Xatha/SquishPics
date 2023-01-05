using SquishPics.APIHelpers;

namespace SquishPics.Controllers;

public sealed class ApiController
{
    private readonly CompressionServiceHelper _compressionServiceHelper;
    private readonly MessageServiceHelper _messageServiceHelper;
    private DirectoryInfo? _directoryInfo;

    public event EventHandler? RequestFinished;
    
    public ApiController(MessageServiceHelper messageServiceHelper, CompressionServiceHelper compressionServiceHelper)
    {
        _messageServiceHelper = messageServiceHelper;
        _compressionServiceHelper = compressionServiceHelper;
        
        _compressionServiceHelper.FileCompressed += CompressionServiceHelperOnFileCompressed;
        _messageServiceHelper.MessageQueueStopped += MessageServiceHelperOnMessageQueueStopped;
    }

    private void MessageServiceHelperOnMessageQueueStopped(object? sender, EventArgs e)
    {
        Console.WriteLine(@"Message queue stopped. Deleting files..."); //TODO logging........
        if (Directory.Exists(_directoryInfo?.FullName)) _directoryInfo?.Delete(true);
        OnRequestFinished();
    }

    public async Task<bool> StartProcessAsync(List<FileInfo> files)
    {
        if (files.Count == 0) return false;
        var maxFileSizeInBytes = await GlobalSettings.SafeGetSettingAsync<int>(SettingKeys.MAX_FILE_SIZE) * 1048576;
        var filesToProcess = files.Where(x => x.Length / 1024 > maxFileSizeInBytes / 1024).ToList();

        if (filesToProcess.Count > 0) files = await CompressFilesAsync(files, filesToProcess, maxFileSizeInBytes);

        //Start the message service.
        await _messageServiceHelper.SetupQueueAsync(files.Select(file => file.FullName));
        _messageServiceHelper.StartQueueForget();
        return true;
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

    public async Task CancelProcessAsync() => await _messageServiceHelper.StopQueueAsync();

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