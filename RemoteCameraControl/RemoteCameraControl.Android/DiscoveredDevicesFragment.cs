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
    public class DiscoveredDevicesFragment : Fragment
    {
        private ObservableCollection<IDevice> _discoveredDevices;
        private TextView _noDevicesFoundTextView;

        public static DiscoveredDevicesFragment NewInstance(ObservableCollection<IDevice> discoveredDevices) 
        {
            return new DiscoveredDevicesFragment(discoveredDevices);
        }

        public DiscoveredDevicesFragment(ObservableCollection<IDevice> discoveredDevices)
        {
            _discoveredDevices = discoveredDevices;
        }
        
        public override View OnCreateView(
            LayoutInflater inflater, 
            ViewGroup container,
            Bundle savedInstanceState) 
        {
            var view = inflater.Inflate(Resource.Layout.discovered_devices_list, container, false);
            
            var connectedAdapter = _discoveredDevices.GetAdapter(BindCell);
            var listView = view.FindViewById<ListView>(Resource.Id.discovered_devices_list);
            listView.Adapter = connectedAdapter;
            
            _noDevicesFoundTextView = view.FindViewById<TextView>(Resource.Id.no_devices_found);
            
            return view;
        }

        private View BindCell(int idx, IDevice device, View view)
        {
            _noDevicesFoundTextView.Visibility = ViewStates.Gone;
            view =  view ?? LayoutInflater.Inflate(Resource.Layout.discovered_device_cell, null);

            var textView = view.FindViewById<TextView>(Resource.Id.discovered_device_name);
            textView.Text = $"{device.Name}, state is {device.State}";

            var connectButton = view.FindViewById<Button>(Resource.Id.connect_button);
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