using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using RemoteCameraControl.IO;

namespace RemoteCameraControl.Logger
{
        /// <summary>
    /// File log writer that will roll the file that reach a limit of 1 mb and will limit the total limit of files in the log folder to 5.
    ///
    /// (the file size limitation is not strict for performance reasons)
    /// </summary>
    public class FileLogSource : ILogSource, IDisposable
    {
        private const int RetainFilesCount = 5;
        private const long FileLengthThreshold = 1 << 20; // 1 MB

        private readonly string _logFileDirectory;
        private string _currentLogFilePath;

        private bool _disposed;
        private readonly object _lockObject = new object();

        public FileLogSource(string logFolder)
        {
            if (string.IsNullOrEmpty(logFolder))
            {
                throw new ArgumentException("Cannot be null or empty", nameof(logFolder));
            }

            _logFileDirectory = logFolder;

            Init();
        }

        public void Log(LogMessage message)
        {
            lock (_lockObject)
            {
                if (_disposed)
                {
                    return;
                }

                try
                {
                    bool needsRoll;
                    using (var streamWriter = new StreamWriter(_currentLogFilePath, true))
                    {
                        streamWriter.WriteLine(GetString(message));
                        needsRoll = streamWriter.BaseStream.Length > FileLengthThreshold;
                    }

                    if (needsRoll)
                    {
                        RollCurrentFile();
                        ClearLogDirectory();
                    }
                }
                catch (Exception ex)
                {
                    Console.Error.WriteLine($"FATAL: failed writing to log file --> {ex.StackTrace}");
                }
            }
        }

        private string GetString(LogMessage logMessage)
        {
            var hasException = logMessage.Exception != null;
            var result = $"{logMessage.TimeStamp:yyyy-MM-dd HH:mm:ss:fff} | {logMessage.Severity} | {logMessage.Message}";
            if (hasException)
            {
                return result + Environment.NewLine + logMessage.Exception;
            }

            return result;
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
            }
        }

        private void Init()
        {
            EnsureLogDirectory();
            InitCurrentFile();
        }

        private void InitCurrentFile()
        {
            var files = GetOrderedLogFiles();
            if (files.Any())
            {
                var file = files.First();
                _currentLogFilePath = file.FullName;

                if (file.Length <= FileLengthThreshold)
                {
                    return;
                }

                RollCurrentFile();
                ClearLogDirectory();
            }
            else
            {
                RollCurrentFile();
            }
        }

        private void RollCurrentFile()
        {
            var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss", CultureInfo.InvariantCulture);
            _currentLogFilePath = Path.Combine(_logFileDirectory, $"Log_{timestamp}.log");
            System.IO.File.Create(_currentLogFilePath).Dispose();
        }

        private void EnsureLogDirectory()
        {
            Directory.CreateDirectory(_logFileDirectory);
        }

        private void ClearLogDirectory()
        {
            var files = GetOrderedLogFiles();
            if (files.Count() <= RetainFilesCount)
            {
                return;
            }

            var filesToDelete = files.Skip(RetainFilesCount);
            foreach (var fileToDelete in filesToDelete)
            {
                fileToDelete.Delete();
            }
        }

        private ICollection<FileInfo> GetOrderedLogFiles()
        {
            return FileService.GetFilesOrderedByLastModified(_logFileDirectory).ToList();
        }
    }
}