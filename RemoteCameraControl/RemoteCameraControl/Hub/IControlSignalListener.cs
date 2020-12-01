using System;
using RemoteCameraControl.Network.DataTransfer;

namespace RemoteCameraControl.Hub
{
    public interface IControlSignalListener
    {
        void OnControlSignalReceived(ControlSignal controlSignal);
    }
}
