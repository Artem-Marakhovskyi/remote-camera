using System;
using System.IO;
using System.Threading.Tasks;
using RemoteCameraControl.Logger;

namespace RemoteCameraControl.Network
{
    public class EndlessStreamWriter
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

        public Task WriteControlSignalAsync(string json)
        {
            var payload = EnvironmentService.GetBytes(json);
            var wrappedBytes = new byte[json.Length + SignalStore.StartMark.Length + SignalStore.EndMark.Length];

            SignalStore.StartMark.CopyTo(wrappedBytes, 0);
            payload.CopyTo(wrappedBytes, 2);
            SignalStore.EndMark.CopyTo(wrappedBytes, wrappedBytes.Length - SignalStore.EndMark.Length);

            return _writingStream.WriteAsync(wrappedBytes, 0, wrappedBytes.Length);
        }
    }
}
