namespace SquishPics.APIHelpers;

public readonly struct MessageProgress
{
    public required int MessagesSent { get; init; }
    public required int MessagesTotal { get; init; }
}