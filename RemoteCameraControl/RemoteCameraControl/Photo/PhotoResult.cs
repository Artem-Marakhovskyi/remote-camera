using System;
using System.IO;

namespace RemoteCameraControl.Photo
{
    public class PhotoResult : IDisposable
    {
        public MemoryStream PhotoStream { get; }

        public TakePhotoResultType State { get; private set; }

        public PhotoResult(MemoryStream photoStream)
        {
            PhotoStream = photoStream;
            State = TakePhotoResultType.Success;
        }

        public PhotoResult(bool canceled)
        {
            State = canceled
                ? TakePhotoResultType.Canceled
                : TakePhotoResultType.Skipped;
        }

        private PhotoResult() { }

        public static PhotoResult FromError()
        {
            return new PhotoResult
            {
                State = TakePhotoResultType.Error
            };
        }

        public void Dispose()
        {
            PhotoStream?.Dispose();
        }
    }
}
