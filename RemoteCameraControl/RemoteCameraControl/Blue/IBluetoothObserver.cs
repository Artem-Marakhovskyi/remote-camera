using System.Collections.Generic;
using Plugin.BLE.Abstractions.Contracts;

namespace RemoteCameraControl.Blue
{
    public interface IBluetoothObserver
    {
        IReadOnlyList<IDevice> ConnectedDevices { get; }
        
        IReadOnlyList<IDevice> DiscoveredDevices { get; }
            
        void OnDeviceDiscovered();
        
        void OnDeviceDisconnected();

        void OnDeviceConnected();
    }
}