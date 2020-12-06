using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using RemoteCameraControl.Android;
using RemoteCameraControl.Hub;
using RemoteCameraControl.Logger;
using RemoteCameraControl.Network.DataTransfer;

namespace RemoteCameraControl.Network
{
    public class ControlStreamManager
    {
        private ILogger _logger;
        private EndlessStreamWriter _endlessStreamWriter;
        private EndlessStreamReader _endlessStreamReader;
        private int _counter = 0;

        private IAppContext _appContext;
        private IControlSignalPublisher _publisher;

        public ControlStreamManager(
            IControlSignalPublisher publisher,
            ILogger logger,
            IAppContext appContext)
        {
            _appContext = appContext;
            _logger = logger;
            _publisher = publisher;
        }

        public void SetSource(Stream stream)
        {
            if (_appContext.IsRc)
            {
                _endlessStreamWriter = new EndlessStreamWriter(
                    stream,
                    _logger);
            }
            else
            {
                _endlessStreamReader = new EndlessStreamReader(
                    4096,
                    _publisher,
                    stream,
                    _logger);
                _endlessStreamReader.Start();
            }
        }

        public async Task SendControlSignalAsync(ControlSignal controlSignal)
        {
            try
            {
                Interlocked.Increment(ref _counter);

                _logger.LogInfo($"[{_counter}] Writing control signal: {controlSignal}");

                var bytes = ControlSignalSerializer.ToBytes(controlSignal);
                await _endlessStreamWriter.WriteControlSignalAsync(bytes);

                _logger.LogInfo($"[{_counter}] Writing control signal: {controlSignal}");
            }
            catch (Exception ex)
            {
                _logger.LogError("An error occured during stream writing", ex);
            }
        }
    }
}
