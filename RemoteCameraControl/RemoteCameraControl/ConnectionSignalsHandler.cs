using System;
using RemoteCamera.HubClient;
using RemoteCameraControl.RemoteCameraControl.Interaction;

namespace RemoteCameraControl
{
    public class ConnectionSignalsHandler : ConnectionSignalsHandlerBase
    {
        private IConnectionSignalsHandler _inner;

        public void SetInner(IConnectionSignalsHandler connectionSignalsHandler)
        {
            _inner = connectionSignalsHandler;
        }

        public void OnControlMessageReceived(ControlMessage controlMessage)
        {
            _inner?.OnControlMessageReceived(controlMessage);
        }

        public void OnDataMessageReceived(DataMessage dataMessage)
        {
            _inner?.OnDataMessageReceived(dataMessage);
        }

        public void OnRcConnected()
        {
            _inner?.OnRcConnected();
        }

        public void OnSessionFinished()
        {
            _inner?.OnSessionFinished();
        }

        public void OnTextReceived(string text)
        {
            _inner?.OnTextReceived(text);
        }

        public void OnPartialDataMessageReceived(PartialDataMessage dataMessage)
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
