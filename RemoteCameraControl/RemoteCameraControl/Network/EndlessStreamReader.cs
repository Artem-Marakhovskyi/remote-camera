using System;
using System.IO;
using System.Threading.Tasks;
using RemoteCameraControl.Logger;

namespace RemoteCameraControl.Network
{
    public class EndlessStreamReader : IDisposable
    {
        private bool _active = true;
        private readonly ILogger _logger;
        private readonly Stream _readingStream;
        private const int ChunkSize = 1024;

        public EndlessStreamReader(
            Stream readingStream,
            ILogger logger)
        {
            _logger = logger;
            _readingStream = readingStream;
        }

        public void Dispose()
        {
            _active = true;
        }

        public void Start()
        {
            Task.Run(async () =>
            {
                while (_active)
                {
                    byte[] buffer = new byte[ChunkSize];
                    var read = await _readingStream.ReadAsync(buffer, 0, ChunkSize);
                    _logger.LogInfo($"{read} bytes read");
                }
            });
        }
    }
}
