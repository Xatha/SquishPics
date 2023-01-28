using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace CompressionLibrary;

//TODO: Refactor everything here.
public class ImageCompressor : IImageCompressor
{
    private readonly long _targetFileSize;
    private List<FileInfo> _imageFiles;

    private ImageCompressor(List<FileInfo> imageFiles, long targetFileSize)
    {
        _imageFiles = imageFiles;
        _targetFileSize = targetFileSize;
    }

    public async Task StartCompressionAsync()
    {
        // We will use Pingo to do some lossless optimization of the images.
        using var pingo = await StartPingoProcessAsync();
        await pingo.WaitForExitAsync();

        //Pingo annoyingly outputs converted files as .jpg even if the original file was a .png
        //So we have a post processing step to rename the files to their original extension and update the list of files to compress
        _imageFiles = await PingoPostProcessAsync();
        if (_imageFiles.Count != 0) await CompressImagesToTargetSize();
    }

    public event EventHandler<Progress>? FileCompressed;

    public static Task<ImageCompressor> CreateAsync(List<FileInfo> imageFiles, long targetFileSize)
    {
        var imageCompressor = new ImageCompressor(imageFiles, targetFileSize);
        return imageCompressor.InitAsync();
    }

    private Task<ImageCompressor> InitAsync()
    {
        return Task.FromResult(this);
    }

    private Task<List<FileInfo>> PingoPostProcessAsync()
    {
        var newListOfFiles = new List<FileInfo>();
        foreach (var imageFile in _imageFiles)
            //Check if the file was .png has been converted to .jpg by Pingo.
            if (imageFile.FullName.EndsWith(".png") && File.Exists(Path.ChangeExtension(imageFile.FullName, ".jpg")))
            {
                var compressedFilePath = Path.ChangeExtension(imageFile.FullName, ".jpg");
                //Replace the old file with the compressed file renamed with .png extension.
                File.Replace(compressedFilePath, imageFile.FullName, null);
                //Add the new file to the list of files to compress.
                newListOfFiles.Add(new FileInfo(imageFile.FullName));
            }
            else
            {
                newListOfFiles.Add(imageFile);
            }

        //Filter out any files that are below the target size. We don't want to compress these.
        return Task.FromResult(newListOfFiles.Where(file => file.Length / 1024 > _targetFileSize / 1024).ToList());
    }

    private Task<Process> StartPingoProcessAsync()
    {
        return Task.FromResult(Process.Start(new ProcessStartInfo
        {
            FileName = "libs\\pingo",
            Arguments = $"pingo -auto=100 {_imageFiles.First().Directory?.FullName}",
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden,
            CreateNoWindow = true
        }) ?? throw new Exception("Could not start pingo")); //TODO: Add proper exception.
    }

    private async Task CompressImagesToTargetSize()
    {
        var imagesToProcess = FilesUtil.GetAllFilesPathsFromFileList(_imageFiles);
        do
        {
            var compressedImages = await CompressImagesAsync(imagesToProcess);

            foreach (var image in compressedImages.Where(image => image.Size <= _targetFileSize))
            {
                imagesToProcess.Remove(image.Path);
            }
        } while (imagesToProcess.Count > 0);
    }

    private async Task<List<(string Path, long Size)>> CompressImagesAsync(List<string> images)
    {
        List<(string Path, long Size)> results = new();

        //foreach (var filePath in images)
        for (var i = 0; i < images.Count; i++)
        {
            var filePath = images[i];
            //await _logger.InfoAsync($"Processing {filePath}.");
            var file = new FileInfo(filePath); //???

            OnOnFileCompressed(new Progress
            {
                FileProcessed = file.Name,
                FilesProcessed = i,
                FilesRemaining = images.Count
            });

            //Creates a compressed image and then replaces it. 
            using (var processedImage = await CompressImageAsync((filePath, file.Length)))
            {
                await FilesUtil.ReplaceImageAsync(filePath, processedImage);
            }

            var newFileSize = new FileInfo(filePath).Length;

            results.Add((filePath, newFileSize));
            //await _logger.DebugAsync($"Previous Size: [{fileSize}] | New Size: [{newFileSize}] | Compression ratio: [{_compressionRatioGenerator.PreviousRatio}]");
        }

        return results;
    }

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    private async Task<Bitmap> CompressImageAsync((string filePath, long fileSize) file)
    {
        var ratio = await CompressionRatioGenerator.CalculateCompressionRatioAsync(file.fileSize, _targetFileSize);

        //Compresses the image by scaling it.
        using var image = Image.FromFile(file.filePath);
        return await ScaleImage(image, ratio);
    }

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility", Justification = "<Pending>")]
    private static Task<Bitmap> ScaleImage(Image image, double ratio)
    {
        //double ratio = height / image.Height;
        var newWidth = (int)Math.Floor(image.Width * ratio);
        var newHeight = (int)Math.Floor(image.Height * ratio);

        var newImage = new Bitmap(newWidth, newHeight, image.PixelFormat);

        using (var graphics = Graphics.FromImage(newImage))
        {
            graphics.DrawImage(image, 0, 0, newWidth, newHeight);
        }

        return Task.FromResult(newImage);
    }

    protected virtual void OnOnFileCompressed(Progress e)
    {
        FileCompressed?.Invoke(this, e);
    }
}