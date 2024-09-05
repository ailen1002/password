using System;
using NLog;

namespace password.Services;

public static class LogService
{
    private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
    
    static LogService()
    {
        ConfigureLogging();
    }
    private static void ConfigureLogging()
    {
        var config = new NLog.Config.LoggingConfiguration();
        var logfile = new NLog.Targets.FileTarget("logfile")
        {
            FileName = $"logs/{DateTime.Now:yyyy-MM-dd}.txt",
            Layout = "${longdate}|${level:uppercase=true}|${message}|${exception}"
        };
        var consoleTarget = new NLog.Targets.ConsoleTarget("logconsole")
        {
            Layout = "${longdate}|${level:uppercase=true}|${message}|${exception}"
        };
        
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, consoleTarget);
        config.AddRule(LogLevel.Debug, LogLevel.Fatal, logfile);
        
        LogManager.Configuration = config;
    }
    public static void Debug(string message) => Logger.Debug(message);
    public static void Info(string message) => Logger.Info(message);
    public static void Warn(string message) => Logger.Warn(message);
    public static void Error(string message) => Logger.Error(message);
    public static void Error(Exception ex, string message) => Logger.Error(ex, message);
    public static void Fatal(string message) => Logger.Fatal(message);
    public static void Fatal(Exception ex, string message) => Logger.Fatal(ex, message);
}