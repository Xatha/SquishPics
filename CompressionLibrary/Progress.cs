namespace CompressionLibrary;

public readonly struct Progress
{
    public required string FileProcessed { get; init; }
    public required int FilesProcessed { get; init; }
    public required int FilesRemaining { get; init; }
}