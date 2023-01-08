namespace SquishPicsDiscordBackend.RetryHelpers;

public class RetryTimeoutException : Exception
{
    public RetryTimeoutException()
    {
    }

    public RetryTimeoutException(string message) : base(message)
    {
    }

    public RetryTimeoutException(string message, Exception inner) : base(message, inner)
    {
    }
}