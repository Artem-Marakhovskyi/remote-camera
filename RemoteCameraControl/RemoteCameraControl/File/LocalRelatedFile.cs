using System;
using System.IO;
using System.Threading.Tasks;

namespace RemoteCameraControl.File
{
    internal class LocalRelatedFile : IRelatedFile
    {
        private readonly string _filePath;

        internal LocalRelatedFile(string filePath, string relatedGuid)
        {
            _filePath = filePath;

            var extension = Path.GetExtension(_filePath);

            if (string.IsNullOrEmpty(extension) || string.IsNullOrEmpty(_filePath))
            {
                throw new InvalidOperationException("file name is not valid");
            }

            extension = extension.Substring(1);

            Filename = Path.GetFileName(_filePath);
            FileExtension = extension;

            RelatedGuid = relatedGuid;

            var now = DateTimeOffset.Now;
            CreatedAt = now;
            LocalCreatedAt = now;
        }

        public string Id { get; }
        public string Filename { get; }
        public int FileId { get; }
        public Stream ContentStream => System.IO.File.OpenRead(_filePath);
        public string RelatedGuid { get; }
        public string FileExtension { get; }
        public DateTimeOffset? CreatedAt { get; }
        public DateTimeOffset? LocalCreatedAt { get; }
        public string FileMimeType => FileTypeResolver.GetFileMimeType(this.FileExtension);
        public bool Uploaded => false;
        public bool HasThumbnail => false;

        public string RelatedId => string.Empty;

        public Task<Stream> GetContentStreamAsync()
        {
            return Task.FromResult<Stream>(System.IO.File.OpenRead(_filePath));
        }

        public Task<string> GetLocalPathAsync()
        {
            return Task.FromResult(_filePath);
        }
    }
}
