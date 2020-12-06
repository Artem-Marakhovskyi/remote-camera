using System;
using RemoteCameraControl.Network.DataTransfer;

namespace RemoteCameraControl.Hub
{
    public interface IDataSignalListener
    {
        void OnDataSignalReceived(DataSignal dataSignal);
    }
}
