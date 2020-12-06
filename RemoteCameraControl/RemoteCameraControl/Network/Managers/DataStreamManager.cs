using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RemoteCameraControl.Android;
using RemoteCameraControl.Hub;
using RemoteCameraControl.Logger;
using RemoteCameraControl.Network.DataTransfer;
using RemoteCameraControl.Network.LowStreams;

namespace RemoteCameraControl.Network
{
    public class DataStreamManager
    {
        private IAppContext _appContext;
        private ILogger _logger;
        private IDataSignalPublisher _publisher;
        private EndlessStreamWriter _endlessStreamWriter;
        private EndlessDataStreamReader _endlessStreamReader;
        private int _counter = 0;

        public DataStreamManager(
            IDataSignalPublisher dataSignalPublisher,
            IAppContext appContext,
            ILogger logger)
        {
            _appContext = appContext;
            _logger = logger;
            _publisher = dataSignalPublisher;
        }

        public void SetSource(Stream stream)
        {
            if (_appContext.IsCamera)
            {
                _endlessStreamWriter = new EndlessStreamWriter(
                    stream,
                    _logger);
            }
            else
            {
                _endlessStreamReader = new EndlessDataStreamReader(
                    _publisher,
                    stream,
                    _logger);
                _endlessStreamReader.Start();
            }
        }


        public async Task SendDataSignalAsync(DataSignal dataSignal)
        {
            try
            {
                Interlocked.Increment(ref _counter);

                _logger.LogInfo($"[{_counter}] Writing control signal: {dataSignal}");

                var bytes = DataSignalSerializer.ToBytes(dataSignal);
                await _endlessStreamWriter.WriteControlSignalAsync(bytes);

                _logger.LogInfo($"[{_counter}] Writing control signal: {dataSignal}");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured during stream writing", ex);
            }
        }
    }
}
