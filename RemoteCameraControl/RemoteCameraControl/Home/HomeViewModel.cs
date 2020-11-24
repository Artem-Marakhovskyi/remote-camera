using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.CurrentActivity;
using RemoteCameraControl.Android.RemoteCameraControl.Permissions;
using RemoteCameraControl.Blue;

namespace RemoteCameraControl.Home
{
    public class HomeViewModel : ViewModelBase
    {
        private IBluetooth _bluetooth;
        private IPermissionsRequestor _permissionsRequestor;

        private IBluetoothObserver _bluetoothObserver;
        
        public HomeViewModel(
            IBluetooth bluetooth,
            IPermissionsRequestor permissionsRequestor)
        {
            _bluetooth = bluetooth;
            _permissionsRequestor = permissionsRequestor;
            _bluetoothObserver = new BluetoothDevicesObserver();
        }

        public BluetoothState BluetoothState => _bluetooth.GetStatus();
        
        public ObservableCollection<IDevice> ConnectedDevices => _bluetoothObserver.ConnectedDevices;

        public ObservableCollection<IDevice> DiscoveredDevices => _bluetoothObserver.DiscoveredDevices;

        public async Task StartBluetoothProcessing()
        {
            await _permissionsRequestor.RequestInitiallyRequiredAsync();
            _bluetooth.StartDeviceObservation(_bluetoothObserver);
        }
        
        public void StopBluetoothProcessing()
        {
            _bluetooth.FinishDeviceObservation();
        }
        
    }
}