using System;
using System.IO;
using System.Threading.Tasks;
using RemoteCameraControl.Logger;

namespace RemoteCameraControl.Network
{
    public class EnslessStreamWriter
    {
        private ILogger _logger;
        private Stream _writingStream;
        private bool _active;

        public EnslessStreamWriter(
            Stream writingStream,
            ILogger logger)
        {
            _logger = logger;
            _writingStream = writingStream;
        }

        public void Dispose()
        {
            _active = true;
        }

        public Task WriteControlSignalAsync(byte[] data)
        {
            var wrappedBytes = new byte[data.Length + 8];
            SignalStore.StartMark.CopyTo(wrappedBytes, 0);
            data.CopyTo(wrappedBytes, 2);
            SignalStore.EndMark.CopyTo(wrappedBytes, wrappedBytes.Length - SignalStore.EndMark.Length);

            return _writingStream.WriteAsync(wrappedBytes, 0, wrappedBytes.Length);
        }
    }
}
