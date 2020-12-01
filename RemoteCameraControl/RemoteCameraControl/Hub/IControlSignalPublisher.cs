using RemoteCameraControl.Network.DataTransfer;

namespace RemoteCameraControl.Hub
{
    public interface IControlSignalPublisher
    {
        void PublishControlSignal(ControlSignal controlSignal);
    }
}