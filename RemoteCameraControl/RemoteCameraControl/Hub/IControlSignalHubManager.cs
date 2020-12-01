namespace RemoteCameraControl.Hub
{
    public interface IControlSignalHubManager
    {
        void AddListener(IControlSignalListener controlSignalListener);

        void RemoveListener(IControlSignalListener controlSignalListener);
    }
}