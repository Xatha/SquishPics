using log4net;

namespace SquishPicsDiscordBackend.Logging;

public static class LogProvider<T>
{
    public static ILog GetLogger() => LogManager.GetLogger(typeof(T).Name);
}