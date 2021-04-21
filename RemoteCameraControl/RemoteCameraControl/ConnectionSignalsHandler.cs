using System;
using RemoteCamera.HubClient;
using RemoteCameraControl.RemoteCameraControl.Interaction;

namespace RemoteCameraControl
{
    public class ConnectionSignalsHandler : ConnectionSignalsHandlerBase
    {
        private IConnectionSignalsHandler _inner;

        public override void SetInner(IConnectionSignalsHandler connectionSignalsHandler)
        {
            _inner = connectionSignalsHandler;
        }

        public override void OnControlMessageReceived(ControlMessage controlMessage)
        {
            _inner?.OnControlMessageReceived(controlMessage);
        }

        public override void OnDataMessageReceived(DataMessage dataMessage)
        {
            _inner?.OnDataMessageReceived(dataMessage);
        }

        public override void OnRcConnected()
        {
            _inner?.OnRcConnected();
        }

        public override void OnSessionFinished()
        {
            _inner?.OnSessionFinished();
        }

        public override void OnTextReceived(string text)
        {
            _inner?.OnTextReceived(text);
        }

        public override void OnPartialDataMessageReceived(PartialDataMessage dataMessage)
        {
            base.OnPartialDataMessageReceived(dataMessage);
        }

        public override void OnPartialDataMessageCompleted(byte[] bytes, string filename)
        {
            _inner?.OnDataMessageReceived(new DataMessage()
            {
                CreatedAt = DateTime.Now,
                Payload = bytes,
                IsFullFile = true
            });
        }
    }
}
