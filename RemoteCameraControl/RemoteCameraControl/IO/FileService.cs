using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using System.Threading.Tasks;
using RemoteCameraControl.Logger;

namespace RemoteCameraControl.IO
{
    public class FileService : IFileService
    {
        private readonly ILogger _logger;

        public FileService(
            ILogger logger)
        {
            _logger = logger;
        }

        public static void Create(string path)
        {
            if (!File.Exists(path))
            {
                // .Dispose() is called
                // File.Create(path) returns FileStream which needs Disposing or Closing in order to release resource.
                File.Create(path).Dispose();
            }
        }

        public async Task<string> CreateAsync(byte[] data, string filename = null)
        {
            var fileName = filename ?? Guid.NewGuid().ToString();

            var path = System.IO.Path.Combine(
                System.Environment.GetFolderPath(
                    System.Environment.SpecialFolder.Personal),
                fileName);

            FileStream fileOutputStream = null;
            try
            {
                fileOutputStream = new FileStream(path, FileMode.Create);
                await fileOutputStream.WriteAsync(data, 0, data.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError("Exception was thrown while writing data to a new file", ex);
            }
            finally
            {
                fileOutputStream?.Dispose();
            }

            return path;
        }

        public static IEnumerable<FileInfo> GetFilesOrderedByLastModified(string directoryPath)
        {
            var dir = new DirectoryInfo(directoryPath);
            if (!dir.Exists)
            {
                return Array.Empty<FileInfo>();
            }

            var files = dir.GetFiles()
                .OrderByDescending(f => f.LastWriteTimeUtc);

            return files;
        }

        public void Delete(string path)
        {
            try
            {
                File.Delete(path);
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning("path is null.", ex);
            }
            catch (ArgumentException ex)
            {
                _logger.LogWarning(
                    "path is a zero-length string, contains only white space, or contains one or more implementation-specific invalid characters.",
                    ex);
            }
            catch (DirectoryNotFoundException ex)
            {
                _logger.LogWarning("The directory information specified in path was not found.", ex);
            }
            catch (PathTooLongException ex)
            {
                _logger.LogWarning(
                    "The length of path or the absolute path information for path exceeds the system-defined maximum length.",
                    ex);
            }
            catch (IOException ex)
            {
                _logger.LogWarning($"The specified file is in use: {path}", ex);
            }
            catch (SecurityException ex)
            {
                _logger.LogWarning($"The caller does not have the required permission for {path}.", ex);
            }
            catch (UnauthorizedAccessException ex)
            {
                _logger.LogWarning($"The caller does not have the required permission for {path}.", ex);
            }
        }

        public void ClearFolder(string originalDirectoryPath, string[] skipChildDirectoriesPaths = null)
        {
            try
            {
                var childDirectories = skipChildDirectoriesPaths == null
                    ? Directory.EnumerateDirectories(originalDirectoryPath)
                    : Directory.EnumerateDirectories(originalDirectoryPath)
                        .Where(child => !skipChildDirectoriesPaths.Contains(child));

                var childFiles = Directory.GetFiles(originalDirectoryPath);

                foreach (var childFile in childFiles)
                {
                    Delete(childFile);
                }

                foreach (var childDirectory in childDirectories)
                {
                    Directory.Delete(childDirectory, true);
                    _logger.LogInfo(
                        $"Directory is deleted with any subdirectories and files. Deleted directory path: {childDirectory}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogWarning("Error when trying to clear folder.", ex);
            }
        }

        public void CreateDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                return;
            }

            Directory.CreateDirectory(path);
        }
    }
}
