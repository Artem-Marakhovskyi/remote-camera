using System;
using System.IO;
using System.Threading.Tasks;
using RemoteCameraControl.Logger;

namespace RemoteCameraControl.Network
{
    public class EndlessStreamWriter : IDisposable
    {
        private ILogger _logger;
        private Stream _writingStream;
        private bool _active;

        public EndlessStreamWriter(
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

        public Task WriteControlSignalAsync(byte[] bytes)
        {
            if (!_active)
            {
                _logger.LogInfo("Stream is not active anymore. Writing is not possible.");
            }

            return _writingStream.WriteAsync(bytes, 0, bytes.Length);
        }
    }
}
