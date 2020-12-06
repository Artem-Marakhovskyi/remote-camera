using System;
using RemoteCameraControl.Network.DataTransfer;

namespace RemoteCameraControl.Hub
{
    public interface IDataSignalPublisher
    {
        void PublishDataSignal(DataSignal dataSignal);
    }
}
