namespace SquishPicsDiscordBackend.Controllers;

public readonly struct Status
{
    public required string Message { get; init; }
    public required int WorkDone { get; init; }
    public required int TotalWork { get; init; }
}