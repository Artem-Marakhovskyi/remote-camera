using System;
using System.IO;
using RemoteCameraControl.Logger;

namespace RemoteCamera.HubClient
{
    public class NullConnectionSignalsHandler : ConnectionSignalsHandlerBase
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

        public void OnPartialDataMessageReceived(PartialDataMessage dataMessage)
        {
            _logger.LogInfo($"Partial data message received: {dataMessage}");
        }

        public async override void OnPartialDataMessageCompleted(byte[] bytes, string filename)
        {
            try
            {
                using (var f = File.Create(Path.Combine("/Users/amara/Desktop", filename + ".png")))
                {
                    await f.WriteAsync(bytes, 0, bytes.Length);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e);
            }
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
