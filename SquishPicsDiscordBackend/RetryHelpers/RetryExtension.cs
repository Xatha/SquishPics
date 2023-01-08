namespace SquishPicsDiscordBackend.RetryHelpers;

public static class RetryExtensions
{
    public static async Task RetryAsync(this Task task, int maxRetries, TimeSpan delay)
    {
        var retries = 0;

        while (true)
            try
            {
                await task;
                return;
            }
            catch (Exception ex)
            {
                if (++retries > maxRetries) throw new RetryTimeoutException("Max retries exceeded", ex);
                await Task.Delay(delay);
            }
    }
}