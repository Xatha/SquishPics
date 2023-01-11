using log4net;

namespace SquishPicsDiscordBackend.Logging;

public static class LogProvider
{
    public static ILog GetLogger<T>() => LogManager.GetLogger(typeof(T).Name);
}