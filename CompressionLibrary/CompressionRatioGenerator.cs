namespace CompressionLibrary
{
    public static class CompressionRatioGenerator
    {
        //Calculate file compression with following eq. 
        // z(x) = 1/log2(8_000_000) * log2(x), where z(x) is compression ratio.
        public static ValueTask<double> CalculateCompressionRatioAsync(long fileSizeInBytes, long targetSize, long tolerance = 524288)
        {
            var adjustedTargetSize = targetSize - tolerance;

            var multiplier = Math.Sqrt((adjustedTargetSize / (double)fileSizeInBytes));
            var compressionRatio = (1 / ((double)((1 / Math.Log2(adjustedTargetSize))) * Math.Log2(fileSizeInBytes)) * multiplier);
            
            return ValueTask.FromResult(compressionRatio);
        }
    }
}
