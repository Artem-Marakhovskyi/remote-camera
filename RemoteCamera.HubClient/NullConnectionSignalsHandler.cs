using System;
using System.IO;
using RemoteCameraControl.Logger;

namespace RemoteCamera.HubClient
{
    public class NullConnectionSignalsHandler : ConnectionSignalsHandlerBase
    {
        private readonly ILogger _logger;

        public NullConnectionSignalsHandler(ILogger logger) : base(logger)
        {
            _logger = logger;
        }

        public override void OnControlMessageReceived(ControlMessage controlMessage)
        {
            base.OnControlMessageReceived(controlMessage);
        }

        public override void OnDataMessageReceived(DataMessage dataMessage)
        {
            base.OnDataMessageReceived(dataMessage);
        }

        public override void OnPartialDataMessageReceived(PartialDataMessage dataMessage)
        {
            base.OnPartialDataMessageReceived(dataMessage);
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

        public override void OnRcConnected()
        {
            base.OnRcConnected();
        }

        public override void OnSessionFinished()
        {
            base.OnSessionFinished();
        }

        public override void OnTextReceived(string text)
        {
            base.OnTextReceived(text);
        }

        public override void SetInner(IConnectionSignalsHandler inner)
        {
            base.SetInner(inner);
        }
    }
}
