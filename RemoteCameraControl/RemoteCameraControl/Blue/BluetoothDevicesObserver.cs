using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Plugin.BLE.Abstractions.Contracts;

namespace RemoteCameraControl.Blue
{
    public class BluetoothDevicesObserver : IBluetoothObserver
    {   
        public IReadOnlyList<IDevice> ConnectedDevices { get; } = ImmutableList<IDevice>.Empty;
        public IReadOnlyList<IDevice> DiscoveredDevices { get; } = ImmutableList<IDevice>.Empty;

        public void OnDeviceDiscovered()
        {
            throw new System.NotImplementedException();
        }

        public void OnDeviceDisconnected()
        {
            throw new System.NotImplementedException();
        }

        public void OnDeviceConnected()
        {
            throw new System.NotImplementedException();
        }

        private void AddDeviceAsConnected(IDevice device)
        {
            if (!ConnectedDevices.Any(x => x.Id == device.Id))
            {
            }
        }
    }
}