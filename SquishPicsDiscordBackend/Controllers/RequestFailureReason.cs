namespace SquishPicsDiscordBackend.Controllers;

public enum RequestFailureReason
{
    AlreadyHandlingRequest,
    InvalidData,
    MessageServiceError
}