using System;
using System.IO;
using System.Threading.Tasks;
using RemoteCameraControl.Hub;
using RemoteCameraControl.Logger;
using RemoteCameraControl.Network.DataTransfer;
using RemoteCameraControl.Network.Managers;

namespace RemoteCameraControl.Network
{
    public class EndlessStreamReader : IDisposable
    {
        private bool _active = true;
        private readonly ILogger _logger;
        private readonly Stream _readingStream;
        private readonly IControlSignalPublisher _controlHubPuslisher;
        public const int ChunkSize = 4096;

        public EndlessStreamReader(
            IControlSignalPublisher controlHubPublisher,
            Stream readingStream,
            ILogger logger)
        {
            _logger = logger;
            _readingStream = readingStream;
            _controlHubPuslisher = controlHubPublisher;
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
                    try
                    {
                        byte[] buffer = new byte[ChunkSize];
                        var readCount = await _readingStream.ReadAsync(buffer, 0, ChunkSize);
                        _logger.LogInfo($"{readCount} bytes read");

                        byte[] res = new byte[readCount];
                        Array.Copy(buffer, res, readCount);
                        var controlSignal = ControlSignalSerializer.ToSignal(res);
                        _controlHubPuslisher.PublishControlSignal(controlSignal);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError("An error occured during reading from stream", ex);
                    }
                }
            });
        }
    }
}
