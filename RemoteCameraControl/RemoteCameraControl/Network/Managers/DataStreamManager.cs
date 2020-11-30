using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using RemoteCameraControl.Android;
using RemoteCameraControl.Logger;
using RemoteCameraControl.Network.DataTransfer;

namespace RemoteCameraControl.Network
{
    public class DataStreamManager
    {
        private ILogger _logger;
        private EndlessStreamWriter _endlessStreamWriter;
        private EndlessStreamReader _endlessStreamReader;
        private int _counter = 0;

        public DataStreamManager(
            IAppContext appContext,
            ILogger logger)
        {
            _logger = logger;
        }

        public void SetSource(Stream stream)
        {
            _endlessStreamWriter = new EndlessStreamWriter(stream, _logger);
            _endlessStreamReader = new EndlessStreamReader(
                _readingQueue,
                stream,
                _logger);
        }

        public async Task SendControlSignalAsync(ControlSignal controlSignal)
        {
            var json = JsonConvert.SerializeObject(controlSignal);
            
            Interlocked.Increment(ref _counter);

            _logger.LogInfo($"[{_counter}] Writing control signal: {controlSignal}");

            await _endlessStreamWriter.WriteControlSignalAsync(json);

            _logger.LogInfo($"[{_counter}] Writing control signal: {controlSignal}");
        }

        public async Task ReadControlSignalAsync()
        {
            _endlessStreamReader.Start();
        }
    }
}
