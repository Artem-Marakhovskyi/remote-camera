using System;
using System.IO;
using System.Threading.Tasks;
using RemoteCameraControl.Hub;
using RemoteCameraControl.Logger;
using RemoteCameraControl.Network.DataTransfer;

namespace RemoteCameraControl.Network.LowStreams
{
    public class EndlessDataStreamReader
    {
        private bool _active = true;
        private readonly ILogger _logger;
        private readonly Stream _readingStream;
        private readonly IDataSignalPublisher _controlHubPuslisher;
        public int ChunkSize = 4096 * 100;

        public EndlessDataStreamReader(
            IDataSignalPublisher dataHubPublisher,
            Stream readingStream,
            ILogger logger)
        {
            _logger = logger;
            _readingStream = readingStream;
            _controlHubPuslisher = dataHubPublisher;
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
                        _logger.LogInfo("Reading data from stream...");
                        var readCount = await _readingStream.ReadAsync(buffer, 0, ChunkSize);
                        _logger.LogInfo($"{readCount} bytes read");

                        byte[] res = new byte[readCount];
                        Array.Copy(buffer, res, readCount);
                        var dataSignal = DataSignalSerializer.ToSignal(res);
                        _controlHubPuslisher.PublishDataSignal(dataSignal);
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
