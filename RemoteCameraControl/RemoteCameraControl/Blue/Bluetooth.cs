using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.EventArgs;
using RemoteCameraControl.Logger;

namespace RemoteCameraControl.Blue
{
    public class Bluetooth : IBluetooth
    {
        private IBluetoothLE _plugin;
        private ILogger _logger;
        private IAdapter _adapter;
        private IBluetoothObserver _observer;

        public Bluetooth(
            ILogger logger)
        {
            _logger = logger;
            
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
            _adapter.StartScanningForDevicesAsync();
        }

        private void OnDeviceDisconnected(object sender, DeviceEventArgs e)
        {
            
        }

        private void OnDeviceAdvertised(object sender, DeviceEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void OnDeviceConnected(object sender, DeviceEventArgs e)
        {
            throw new System.NotImplementedException();
        }

        private void OnDeviceConnectionLost(object sender, DeviceErrorEventArgs e)
        {
            throw new System.NotImplementedException();
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

        public BluetoothState GetStatusAsync()
        {
            throw new System.NotImplementedException();
        }
        
        private void OnDeviceDiscovered(object sender, DeviceEventArgs e)
        {
            throw new System.NotImplementedException();
        }
    }
}