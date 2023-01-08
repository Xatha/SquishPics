namespace CompressionLibrary;

public static class Factory
{
    public static async Task<IImageCompressor> CreateImageCompressorAsync(List<FileInfo> imageFiles,
        long targetFileSize = (long)8.389e+6)
    {
        return await ImageCompressor.CreateAsync(imageFiles, targetFileSize);
    }
}