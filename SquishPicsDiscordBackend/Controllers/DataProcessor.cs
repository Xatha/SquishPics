using CompressionLibrary;
using log4net;
using SquishPicsDiscordBackend.Logging;

namespace SquishPicsDiscordBackend.Controllers;

public class DataProcessor
{
    private readonly ILog _log;
    private const int MEGABYTE_TO_BYTE_FACTOR = 1000000;
    private int _maxFileLengthInBytes;
    private DirectoryInfo? _tempDirectory;

    public DataProcessor(ILog log)
    {
        _log = log;
    }

    public List<string>? Data { get; private set; }
    public bool IsValid { get; private set; }

    public event EventHandler<Status>? FileProcessed;

    public async Task ProcessAsync(List<FileInfo> files, int maxFileLengthInMb)
    {
        if (files.Count == 0) return;
        _maxFileLengthInBytes = maxFileLengthInMb * MEGABYTE_TO_BYTE_FACTOR;
            
        var filesToProcess = files.Where(x => x.Length > _maxFileLengthInBytes).ToList();
        Data = filesToProcess.Count > 0 
            ? (await ProcessFilesAsync(files)).Select(f => f.FullName).ToList() 
            : files.Select(f => f.FullName).ToList();
        
        IsValid = true; 
    }
    
    public Task CleanUpAsync()
    {
        _tempDirectory?.Delete(true);
        _tempDirectory = null;
        return Task.CompletedTask;
    }
    
    private async Task<List<FileInfo>> ProcessFilesAsync(List<FileInfo> files)
    {
        var copiedFiles = await CopyFilesToTempDirectoryAsync(files);
        
        OnFileProcessed(new Status
        {
            Message = "Processing files... This may take a while...",
            WorkDone = 1,
            TotalWork = 1
        });
        
        var imageCompressor = await ImageCompressor.CreateAsync(copiedFiles, _maxFileLengthInBytes);
        await imageCompressor.StartCompressionAsync();
        
        //Some of our files have been copied to a new location, so we need to update the list.
        return files.Select(file => copiedFiles.Find(info => file.Name == info.Name) ?? file).ToList();
    }
    
    private async Task<List<FileInfo>> CopyFilesToTempDirectoryAsync(List<FileInfo> files)
    {
        var tempDirectoryPath = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
        _tempDirectory = Directory.CreateDirectory(tempDirectoryPath);
        await _log.DebugAsync($@"Copying {files.Count} files to temp directory: {_tempDirectory.FullName}.");
        
        var result = new List<FileInfo>();
        var filesProcessed = 0;
        foreach (var file in files)
        {
            OnFileProcessed(new Status
            {
                Message = "Copying files to temp directory...",
                WorkDone = filesProcessed + 1,
                TotalWork = files.Count
            });

            var newFilePath = Path.Combine(_tempDirectory.FullName, file.Name);
            File.Copy(file.FullName, newFilePath);
            result.Add(new FileInfo(newFilePath));
        }

        return result;
    }

    protected virtual void OnFileProcessed(Status e) => FileProcessed?.Invoke(this, e);
}