using System;
using System.Collections.ObjectModel;
using Android.OS;
using Android.Support.V4.App;
using Android.Views;
using Android.Widget;
using GalaSoft.MvvmLight.Helpers;
using Plugin.BLE.Abstractions.Contracts;

namespace RemoteCameraControl.Android
{
    public class ConnectedDevicesFragment : Fragment
    {
        private ObservableCollection<IDevice> _connectedDevices;
        private TextView _noDevicesFoundTextView;

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

            _noDevicesFoundTextView = view.FindViewById<TextView>(Resource.Id.no_devices_found);
            
            return view;
        }        
        
        private View BindCell(int idx, IDevice device, View view)
        {
            _noDevicesFoundTextView.Visibility = ViewStates.Gone;
            view =  view ?? LayoutInflater.Inflate(Resource.Layout.connected_device_cell, null);

            var textView = view.FindViewById<TextView>(Resource.Id.connected_device_name);
            textView.Text = $"{device.Name}, state is {device.State}";

            var connectButton = view.FindViewById<Button>(Resource.Id.disconnect_button);
            connectButton.Tag = device.Id.ToString();
            connectButton.Click -= OnButtonClick;
            connectButton.Click += OnButtonClick;

            return view;
        }

        private void OnButtonClick(object sender, EventArgs e)
        {
            
        }
    }
}