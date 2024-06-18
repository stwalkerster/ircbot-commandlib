namespace Stwalkerster.Bot.CommandLib.ExtensionMethods;

using System;
using Microsoft.Extensions.Logging;

public static class ILoggerExtensions
{
    public static void FatalFormat(this ILogger logger, string message, params object[] args)
    {
        logger.LogCritical(message, args);
    }
    public static void Fatal(this ILogger logger, string message, Exception e = null)
    {
        logger.LogCritical(e, message);
    }
    public static void ErrorFormat(this ILogger logger, string message, params object[] args)
    {
        logger.LogError(message, args);
    }
    public static void Error(this ILogger logger, string message, Exception e = null)
    {
        logger.LogError(e, message);
    }
    public static void WarnFormat(this ILogger logger, string message, params object[] args)
    {
        logger.LogWarning(message, args);
    }
    public static void Warn(this ILogger logger, string message, Exception e = null)
    {
        logger.LogWarning(e, message);
    }
    public static void InfoFormat(this ILogger logger, string message, params object[] args)
    {
        logger.LogInformation(message, args);
    }
    public static void Info(this ILogger logger, string message, Exception e = null)
    {
        logger.LogInformation(e, message);
    }
    public static void DebugFormat(this ILogger logger, string message, params object[] args)
    {
        logger.LogDebug(message, args);
    }
    public static void Debug(this ILogger logger, string message, Exception e = null)
    {
        logger.LogDebug(e, message);
    }
}   