using System;
using System.Collections.ObjectModel;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using Java.Lang;
using Plugin.BLE.Abstractions.Contracts;
using Object = Java.Lang.Object;

namespace RemoteCameraControl.Android
{
    public class HomeViewPagerAdapter : FragmentPagerAdapter
    {
        private ObservableCollection<IDevice> _discoveredDevices;
        private ObservableCollection<IDevice> _connectedDevices;

        public HomeViewPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public HomeViewPagerAdapter(
            FragmentManager fm,
            ObservableCollection<IDevice> connectedDevices,
            ObservableCollection<IDevice> discoveredDevices) : base(fm)
        {
            _connectedDevices = connectedDevices;
            _discoveredDevices = discoveredDevices;
        }

        public override int Count { get; } = 2;
        
        public override Fragment GetItem(int position)
        {
            if (position == 0)
            {
                return ConnectedDevicesFragment.NewInstance(_connectedDevices);
            }
            else
            {
                return DiscoveredDevicesFragment.NewInstance(_discoveredDevices);
            }
        }

        public override ICharSequence GetPageTitleFormatted(int position)
        {
            if (position == 0)
            {
                return new Java.Lang.String("Connected");
            }
            
            return new Java.Lang.String("Discovered");
        }
    }
}