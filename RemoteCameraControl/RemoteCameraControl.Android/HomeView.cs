using Android.App;
using Android.Bluetooth;
using Android.OS;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using RemoteCameraControl.Home;

namespace RemoteCameraControl.Android
{
    [Activity(Label = "View for HomeViewModel")]
    public class HomeView : ActivityBase<HomeViewModel>
    {
        private TextView? _bluetoothStateTextView;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetTheme(Android.Resource.Style.Theme_AppCompat);
            SetContentView(Resource.Layout.home_view);

            _bluetoothStateTextView = FindViewById<TextView>(Resource.Id.bluetooth_state_text_view);
            this.SetBinding(() => ViewModel.BluetoothState, () => _bluetoothStateTextView.Text, BindingMode.OneWay)
                .ConvertSourceToTarget(x => $"Bluetooth state: {x}");

        var viewPager = FindViewById<ViewPager>(Resource.Id.view_pager);
            viewPager.Adapter = new HomeViewPagerAdapter(
                SupportFragmentManager,
                ViewModel.ConnectedDevices,
                ViewModel.DiscoveredDevices,
                ViewModel.ConnectToDeviceCommand,
                ViewModel.DisconnectFromDeviceCommand);
            viewPager.CurrentItem = 0;
//"64:BC:0C:E4:56:FD"
            var s = GetSystemService(Service.BluetoothService);
            // var address = s.Address;
            // var name = s.Name;
            // var state = s.State;
            
        }

        protected async override void OnResume()
        {
            base.OnResume();
            
            await ViewModel.StartBluetoothProcessing();
        }

        protected override void OnPause()
        {
            base.OnPause();
            
            
            ViewModel.StopBluetoothProcessing();

        }
    }
}