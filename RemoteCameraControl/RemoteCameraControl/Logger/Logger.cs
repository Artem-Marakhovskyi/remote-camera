using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace RemoteCameraControl.Android.RemoteCameraControl.Logger
{
    public class Logger : ILogger
    {
        private readonly List<ILogSource> _logSources = new List<ILogSource>();

        public LogLevel CurrentLevel { get; set; }

        public void AddSource(ILogSource logSource)
        {
            _logSources.Add(logSource);
        }

        public void LogDebug(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogToSources(message, LogLevel.Debug, null, memberName, sourceFilePath, sourceLineNumber);
        }

        public void LogInfo(
            string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogToSources(message, LogLevel.Info, null, memberName, sourceFilePath, sourceLineNumber);
        }

        public void LogWarning(
            string message,
            Exception ex = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogToSources(message, LogLevel.Warning, ex, memberName, sourceFilePath, sourceLineNumber);
        }

        public void LogError(
            string message,
            Exception ex = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogToSources(message, LogLevel.Error, ex, memberName, sourceFilePath, sourceLineNumber);
        }

        public void LogFatal(
            string message,
            Exception ex = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0)
        {
            LogToSources(message, LogLevel.Fatal, ex, memberName, sourceFilePath, sourceLineNumber);
        }

        private void LogToSources(
            string message,
            LogLevel logLevel,
            Exception ex,
            string memberName,
            string sourceFilePath,
            int sourceLineNumber)
        {
            if (!string.IsNullOrWhiteSpace(message) && logLevel >= CurrentLevel)
            {
                var logMessage = new LogMessage(
                    DateTime.Now,
                    logLevel,
                    message,
                    ex,
                    string.Empty,
                    memberName,
                    sourceFilePath,
                    sourceLineNumber);

                Parallel.ForEach(_logSources, source => source.Log(logMessage));
            }
        }
    }
}