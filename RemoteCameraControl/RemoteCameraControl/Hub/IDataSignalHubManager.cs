using System;
namespace RemoteCameraControl.Hub
{
    public interface IDataSignalHubManager
    {

        void AddListener(IDataSignalListener controlSignalListener);

        void RemoveListener(IDataSignalListener controlSignalListener);
    }
}
