using CompressionLibrary;
using SquishPics.Controllers;

namespace SquishPics.APIHelpers;

public sealed class CompressionServiceHelper
{
    public event EventHandler<Progress>? FileCompressed;

    public async Task StartAsync(List<FileInfo> files, int maxFileSizeInBytes)
    {
        // Convert MiB to bytes so MiB * (1024^2) or 1,048,576.
        var imageCompressor = await ImageCompressor.CreateAsync(files, maxFileSizeInBytes);
        imageCompressor.FileCompressed += OnFileCompressed;
        await imageCompressor.StartCompressionAsync();
        imageCompressor.FileCompressed -= OnFileCompressed;
    }

    private void OnFileCompressed(object? sender, Progress s)
    {
        FileCompressed?.Invoke(this, s);
    }
}