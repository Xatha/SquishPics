using System.Diagnostics.CodeAnalysis;
using System.Drawing;

namespace CompressionLibrary;

internal static class FilesUtil
{
    public static List<string> GetAllFilesPathsFromFileList(IEnumerable<FileInfo> files)
    {
        var filePaths = new List<string>();

        foreach (var file in files)
        {
            var filePath = file.FullName;
            filePaths.Add(filePath);
        }

        return filePaths;
    }

    [SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
    public static Task ReplaceImageAsync(string destinationPath, Image processedImage)
    {
        File.Delete(destinationPath);
        processedImage.Save(destinationPath);
        return Task.CompletedTask;
    }
}