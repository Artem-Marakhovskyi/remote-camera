using System;

namespace RemoteCameraControl.Android.RemoteCameraControl.Logger
{
public class LogMessage
    {
        //DateTimeStamp | Severity | Message | CallerName | SourceFileLineNumber
        private const string MessageTemplate = "{0:yyyy-MM-dd HH:mm:ss:fff} | {1} | {2} | Member:{3} | Line:{4}";

        //MessageTemplate --> StackTrace
        private static readonly string MessageTemplateWithStackTrace = "{0}" + Environment.NewLine + "--> {1}";

        public readonly DateTime TimeStamp;
        public readonly string Message;
        public readonly string StackTrace;
        public readonly string DeviceDetails;

        public LogLevel LogLevel { get; private set; }

        public string Severity { get; private set; }

        public string MemberName { get; private set; }

        public string SourceFilePath { get; private set; }

        public int SourceLineNumber { get; private set; }

        public Exception Exception { get; private set; }

        public LogMessage(
            DateTime timeStamp,
            LogLevel severity,
            string message,
            Exception ex, 
            string deviceDetails,
            string memberName,
            string sourceFilePath,
            int sourceLineNumber)
        {
            TimeStamp = timeStamp;
            Message = message;
            StackTrace = ex?.StackTrace;
            Exception = ex;
            DeviceDetails = deviceDetails;
            SourceFilePath = sourceFilePath;
            MemberName = memberName;
            SourceLineNumber = sourceLineNumber;
            LogLevel = severity;
            Severity = severity.ToString().ToUpperInvariant();
        }

        public string GetString()
        {
            return Exception == null ? GetUsualString() : GetExceptionString();
        }

        private string GetUsualString()
        {
            return string.Format(MessageTemplate, TimeStamp, Severity, Message, MemberName, SourceLineNumber);
        }

        private string GetExceptionString()
        {
            return string.Format(MessageTemplateWithStackTrace, GetUsualString(), Exception);
        }

        public override string ToString()
        {
            return GetString();
        }
    }
}