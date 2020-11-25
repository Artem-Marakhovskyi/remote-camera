using System;
using System.Linq;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using RemoteCameraControl.Android;
using RemoteCameraControl.Logger;

namespace RemoteCameraControl.Blue
{
    public class Bluetooth : IBluetooth
    {
        private IBluetoothLE _plugin;
        private ILogger _logger;
        private IAdapter _adapter;
        private IBluetoothObserver _observer;
        private IBluetoothServer _bluetoothServer;

        public Bluetooth(
            ILogger logger,
            IBluetoothServer bluetoothServer)
        {
            _logger = logger;
            _bluetoothServer = bluetoothServer;
            _plugin = CrossBluetoothLE.Current;
            _adapter = _plugin.Adapter;
        }

        public void StartDeviceObservation(IBluetoothObserver observer)
        {
            _observer = observer;
            _adapter.DeviceDiscovered += OnDeviceDiscovered;
            _adapter.DeviceDisconnected += OnDeviceDisconnected;
            _adapter.DeviceAdvertised += OnDeviceAdvertised;
            _adapter.DeviceConnected += OnDeviceConnected;
            _adapter.DeviceConnectionLost += OnDeviceConnectionLost;
            _adapter.ScanMode = ScanMode.Balanced;
            _adapter.ScanTimeout = 100;
            _adapter.StartScanningForDevicesAsync();
        }

        private void OnDeviceDisconnected(object sender, DeviceEventArgs e)
        {
            _observer.OnDeviceDisconnected(e.Device);
        }

        private void OnDeviceAdvertised(object sender, DeviceEventArgs e)
        {
            _observer.OnDeviceDiscovered(e.Device);
        }

        private void OnDeviceConnected(object sender, DeviceEventArgs e)
        {
            _observer.OnDeviceConnected(e.Device);
        }

        private void OnDeviceConnectionLost(object sender, DeviceErrorEventArgs e)
        {
            _observer.OnDeviceDisconnected(e.Device);
        }

        public void FinishDeviceObservation()
        {
            _adapter.DeviceDiscovered -= OnDeviceDiscovered;
            _adapter.DeviceDisconnected -= OnDeviceDisconnected;
            _adapter.DeviceAdvertised -= OnDeviceAdvertised;
            _adapter.DeviceConnected -= OnDeviceConnected;
            _adapter.DeviceConnectionLost -= OnDeviceConnectionLost;
            _adapter.StopScanningForDevicesAsync();
        }

        public BluetoothState GetStatus()
        {
            var state = _plugin.State;

            return state;
        }

        public Task ListenToDeviceAsync(Guid deviceId)
        {
            var device = _observer.DiscoveredDevices.First(x => x.Id == deviceId);
            return _bluetoothServer.ListenTo(device, device.NativeDevice);
        }

        // public Task ConnectToDeviceAsync(Guid deviceId)
        // {
        //     var device = _observer.DiscoveredDevices.First(x => x.Id == deviceId);
        //     return _bluetoothServer.ListenTo(device, device.NativeDevice);
        // }

        private void OnDeviceDiscovered(object sender, DeviceEventArgs e)
        {
            _observer.OnDeviceDiscovered(e.Device);
        }
    }
}