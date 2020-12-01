using System;
using System.IO;
using System.Threading.Tasks;

namespace RemoteCameraControl.File
{
    public interface IRelatedFile
    {
        string Id { get; }
        /// <value>
        /// Name of the original file (including its extension).
        /// </value>
        string Filename { get; }
      
        /// <value>
        /// Stream with the file content, that can be null.
        /// </value>
        Stream ContentStream { get; }
      
        /// <value>
        /// File extension (e.g. jpeg) without dot.
        /// </value>
        string FileExtension { get; }

        /// <value>
        /// Creation date (remote)
        /// </value>
        DateTimeOffset? CreatedAt { get; }

        /// <value>
        /// Creation date (local)
        /// </value>
        DateTimeOffset? LocalCreatedAt { get; }

        /// <summary>
        /// File mime type (e.g. image/jpeg)
        /// </summary>
        string FileMimeType { get; }

        /// <value>
        /// True if the file was uploaded to remote backend.
        /// </value>
        bool Uploaded { get; }

        string RelatedId { get; }

        /// <summary>
        /// Returns a stream with the file content.
        /// </summary>
        /// <returns>A task that will complete with stream to the file content.</returns>
        Task<Stream> GetContentStreamAsync();

        /// <summary>
        /// Returns a local path for the file.
        /// </summary>
        /// <remarks>Prefer to use the <see cref="GetContentStreamAsync()"/> method, this is still preliminary.</remarks>
        /// <returns>A task that will complete with the local path the file content.</returns>
        /// <exception cref="NotSupportedException">if the underline implementation do not support this feature.</exception>
        Task<string> GetLocalPathAsync();
    }
}
