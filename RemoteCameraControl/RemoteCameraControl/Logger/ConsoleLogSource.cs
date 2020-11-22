using System;

namespace RemoteCameraControl.Logger
{
    public class ConsoleLogSource : ILogSource
    {        
        public void Log(LogMessage logMessage)
        {
            // Info: iOS console and Android Logcat already include timestamp, no need to duplicate information
            var textWriter = logMessage.LogLevel > LogLevel.Error ? Console.Error : Console.Out;
            var message = $"{logMessage.Severity} | {logMessage.Message} | Member:{logMessage.MemberName} | Line:{logMessage.SourceLineNumber}";
            
            if (logMessage.Exception != null)
            {
                message += $"{Environment.NewLine} --> {logMessage.Exception}";
            }
            
            textWriter.WriteLine(message);
            
        }
    }
}