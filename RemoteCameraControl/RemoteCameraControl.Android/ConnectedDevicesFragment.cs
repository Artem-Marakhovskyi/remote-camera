using System.Collections.ObjectModel;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using Java.Lang;
using Plugin.BLE.Abstractions.Contracts;

namespace RemoteCameraControl.Android
{
    public class ConnectedDevicesFragment : Fragment
    {
        private ObservableCollection<IDevice> _connectedDevices;

        public static ConnectedDevicesFragment NewInstance(ObservableCollection<IDevice> connectedDevices) 
        {
            return new ConnectedDevicesFragment(connectedDevices);
        }

        public ConnectedDevicesFragment(ObservableCollection<IDevice> connectedDevices)
        {
            _connectedDevices = connectedDevices;
        }
        
        public override View OnCreateView(
            LayoutInflater inflater, 
            ViewGroup container,
            Bundle savedInstanceState) 
        {
            var view = inflater.Inflate(Resource.Layout.connected_devices_list, container, false);
            
            var connectedAdapter = _connectedDevices.GetAdapter(BindCell);
            var listView = view.FindViewById<ListView>(Resource.Id.connected_devices_list);
            listView.Adapter = connectedAdapter;
            
            return view;
        }        
        
        private View BindCell(int idx, IDevice device, View view)
        {
            return view;
        }
    }
}