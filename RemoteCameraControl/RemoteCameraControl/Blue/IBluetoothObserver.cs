using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using Plugin.BLE.Abstractions.Contracts;

namespace RemoteCameraControl.Blue
{
    public interface IBluetoothObserver
    {
        ObservableCollection<IDevice> ConnectedDevices { get; }
        
        ObservableCollection<IDevice> DiscoveredDevices { get; }
            
        void OnDeviceDiscovered(IDevice device);
        
        void OnDeviceDisconnected(IDevice device);

        void OnDeviceConnected(IDevice device);
    }
}