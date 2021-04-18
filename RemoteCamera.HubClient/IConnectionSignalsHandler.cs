namespace RemoteCamera.HubClient
{
    public interface IConnectionSignalsHandler
    {
        void SetInner(IConnectionSignalsHandler inner);
        void OnRcConnected();
        void OnSessionFinished();
        void OnControlMessageReceived(ControlMessage controlMessage);
        void OnDataMessageReceived(DataMessage dataMessage);
        void OnTextReceived(string text);
    }
}
