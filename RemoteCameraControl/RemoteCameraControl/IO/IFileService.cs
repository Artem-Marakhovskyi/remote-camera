using System.Threading.Tasks;

namespace RemoteCameraControl.IO
{
    public interface IFileService
    {
        /// <summary>
        /// Delete the specified path.
        /// All exceptions are caught and swallowed.
        /// </summary>
        /// <returns>void.</returns>
        /// <param name="path">Path.</param>
        void Delete(string path);

        /// <summary>
        /// Creates file, saves it with a content of data
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="data"></param>
        /// <returns>Full path to new file</returns>
        Task<string> CreateAsync(byte[] data, string filename = null);

        /// <summary>
        /// Creates empty directory if it does not exist.
        /// </summary>
        /// <param name="path"></param>
        void CreateDirectory(string path);

        /// <summary>
        /// Deletes all data inside the folder
        /// </summary>
        /// <param name="originalDirectoryPath"> Path to the directory to be cleaned</param>
        /// <param name="skipChildDirectoriesPaths"> Paths to the childs directories that does not need to be cleared </param>
        void ClearFolder(string originalDirectoryPath, string[] skipChildDirectoriesPaths = null);
    }
}