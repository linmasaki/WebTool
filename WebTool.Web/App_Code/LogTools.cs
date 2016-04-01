using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using NLog;

/// <summary>
/// Summary description for LogTools
/// </summary>
public static class LogTools
{
    private static Logger _logger = LogManager.GetLogger("LogTools");

    public static void Log(string log, LogLevel level = null)
    {
        level = level ?? LogLevel.Info;
        _logger.Log(level, log);
    }
    public static void Info(string log)
    {
        _logger.Log(LogLevel.Info, log);
    }
    public static void Error(string log)
    {
        _logger.Log(LogLevel.Error, log);
    }
    public static void Exception(string log, string message)
    {
        _logger.Log(LogLevel.Error, string.Format("Log: {0};Ex: {1}", log, message));
    }
}