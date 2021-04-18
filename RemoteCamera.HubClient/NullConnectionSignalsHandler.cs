using System;
using RemoteCameraControl.Logger;

namespace RemoteCamera.HubClient
{
    public class NullConnectionSignalsHandler : IConnectionSignalsHandler
    {
        private readonly ILogger _logger;

        public NullConnectionSignalsHandler(ILogger logger)
        {
            _logger = logger;
        }

        public void OnControlMessageReceived(ControlMessage controlMessage)
        {
            _logger.LogInfo($"Control message received: {controlMessage}");
        }

        public void OnDataMessageReceived(DataMessage dataMessage)
        {
            _logger.LogInfo($"Data message received: {dataMessage}");
        }

        public void OnRcConnected()
        {
            _logger.LogInfo($"RC connected");
        }

        public void OnSessionFinished()
        {
            _logger.LogInfo($"Session finished");
        }

        public void OnTextReceived(string text)
        {
            _logger.LogInfo($"Text received: {text}");
        }

        public void SetInner(IConnectionSignalsHandler inner)
        {
            throw new NotImplementedException();
        }
    }
}
