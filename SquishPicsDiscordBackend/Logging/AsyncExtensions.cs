using Discord;
using log4net;

namespace SquishPicsDiscordBackend.Logging;

public static class AsyncExtensions
{
    public static async Task DebugAsync(this ILog log, object message)
    {
        await Task.Run(() => log.Debug(message));
    }

    public static async Task DebugAsync(this ILog log, object message, Exception exception)
    {
        await Task.Run(() => log.Debug(message, exception));
    }

    public static async Task InfoAsync(this ILog log, object message)
    {
        await Task.Run(() => log.Info(message));
    }

    public static async Task InfoAsync(this ILog log, object message, Exception exception)
    {
        await Task.Run(() => log.Info(message, exception));
    }

    public static async Task WarnAsync(this ILog log, object message)
    {
        await Task.Run(() => log.Warn(message));
    }

    public static async Task WarnAsync(this ILog log, object message, Exception exception)
    {
        await Task.Run(() => log.Warn(message, exception));
    }

    public static async Task ErrorAsync(this ILog log, object message)
    {
        await Task.Run(() => log.Error(message));
    }

    public static async Task ErrorAsync(this ILog log, object message, Exception exception)
    {
        await Task.Run(() => log.Error(message, exception));
    }

    public static async Task FatalAsync(this ILog log, object message)
    {
        await Task.Run(() => log.Fatal(message));
    }

    public static async Task FatalAsync(this ILog log, object message, Exception exception)
    {
        await Task.Run(() => log.Fatal(message, exception));
    }
    
    public static async Task LogClientAsync(this ILog log, LogMessage msg)
    {
        var callerName = msg.Source;
        var message = msg.Message;
        
                
        //log.InfoFormat("| [{0,-1}] {1, 0}", $"{callerName}", message);
        
        var formattedMessage = String.Format("| [{0,-1}] {1, 0}", $"{callerName}", message);

        if (msg.Exception is not null)
        {
            await LogClientMessageWithExceptionAsync(log, msg, formattedMessage);
        }
        else
        {
            await LogClientMessageWithoutExceptionAsync(log, msg, formattedMessage);
        }
    }

    private static async Task LogClientMessageWithoutExceptionAsync(ILog log, LogMessage msg, string formattedMessage)
    {
        if (msg.Severity == LogSeverity.Critical)
            await log.FatalAsync(formattedMessage);
        else if (msg.Severity == LogSeverity.Error)
            await log.ErrorAsync(formattedMessage);
        else if (msg.Severity == LogSeverity.Warning)
            await log.WarnAsync(formattedMessage);
        else if (msg.Severity == LogSeverity.Info)
            await log.InfoAsync(formattedMessage);
        else if (msg.Severity is LogSeverity.Debug or LogSeverity.Verbose)
            await log.DebugAsync(formattedMessage);
    }

    private static async Task LogClientMessageWithExceptionAsync(ILog log, LogMessage msg, string formattedMessage)
    {
        if (msg.Severity == LogSeverity.Critical)
            await log.FatalAsync(formattedMessage, msg.Exception);
        else if (msg.Severity == LogSeverity.Error)
            await log.ErrorAsync(formattedMessage, msg.Exception);
        else if (msg.Severity == LogSeverity.Warning)
            await log.WarnAsync(formattedMessage, msg.Exception);
        else if (msg.Severity == LogSeverity.Info)
            await log.InfoAsync(formattedMessage, msg.Exception);
        else if (msg.Severity is LogSeverity.Debug or LogSeverity.Verbose) 
            await log.DebugAsync(formattedMessage, msg.Exception);
    }
}