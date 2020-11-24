using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.ObjectModel;
using System.Linq;
using Plugin.BLE.Abstractions.Contracts;

namespace RemoteCameraControl.Blue
{
    public class BluetoothDevicesObserver : IBluetoothObserver
    {   
        public ObservableCollection<IDevice> ConnectedDevices { get; private set; } = new ObservableCollection<IDevice>();
        public ObservableCollection<IDevice> DiscoveredDevices { get; private set; } = new ObservableCollection<IDevice>();

        public void OnDeviceDiscovered(IDevice device)
        {
            AddDeviceAsDiscovered(device);
        }

        public void OnDeviceDisconnected(IDevice device)
        {
            RemoveDeviceFromConnected(device);
        }

        public void OnDeviceConnected(IDevice device)
        {
            AddDeviceAsConnected(device);
        }

        private void AddDeviceAsDiscovered(IDevice device)
        {
            if (DiscoveredDevices.All(x => x.Id != device.Id))
            {
                DiscoveredDevices.Add(device);
            }
        }

        private void RemoveDeviceFromConnected(IDevice device)
        {
            var d = ConnectedDevices.FirstOrDefault(x => x.Id == device.Id);
            if (d != null)
            {
                ConnectedDevices.Remove(d);
            }            
        }

        private void AddDeviceAsConnected(IDevice device)
        {
            if (ConnectedDevices.All(x => x.Id != device.Id))
            {
                ConnectedDevices.Add(device);
            }

            var d = DiscoveredDevices.FirstOrDefault(x => x.Id == device.Id);
            if (d != null)
            {
                DiscoveredDevices.Remove(d);
            }
        }
    }
}