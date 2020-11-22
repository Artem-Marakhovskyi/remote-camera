using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace RemoteCameraControl.Logger
{
    public interface ILogger
    {
        LogLevel CurrentLevel { get; set; }

        void AddSource(ILogSource logSource);

        void LogDebug(
            [Localizable(false)] string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void LogInfo(
            [Localizable(false)] string message,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void LogWarning(
            [Localizable(false)] string message,
            Exception ex = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void LogError(
            [Localizable(false)] string message,
            Exception ex = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);

        void LogFatal(
            [Localizable(false)] string message,
            Exception ex = null,
            [CallerMemberName] string memberName = "",
            [CallerFilePath] string sourceFilePath = "",
            [CallerLineNumber] int sourceLineNumber = 0);
    }
}