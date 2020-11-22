using System.Threading.Tasks;
using Plugin.BLE;
using Plugin.CurrentActivity;
using RemoteCameraControl.Android.RemoteCameraControl.Permissions;
using RemoteCameraControl.Blue;

namespace RemoteCameraControl.Home
{
    public class HomeViewModel : ViewModelBase
    {
        private IBluetooth _bluetooth;
        private IPermissionsRequestor _permissionsRequestor;

        public HomeViewModel(
            IBluetooth bluetooth,
            IPermissionsRequestor permissionsRequestor)
        {
            _bluetooth = bluetooth;
            _permissionsRequestor = permissionsRequestor;
        }

        public void StartBluetoothProcessing()
        {
            _bluetooth.Init();
            return await _bluetooth.GetStatusAsync();
        }
        
        public async Task<string> GetStatusAsync()
        {
            

        }
    }
}