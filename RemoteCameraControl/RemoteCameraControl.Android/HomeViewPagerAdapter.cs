using System;
using System.Collections.ObjectModel;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Views;
using GalaSoft.MvvmLight.Command;
using Java.Lang;
using Plugin.BLE.Abstractions.Contracts;
using Object = Java.Lang.Object;

namespace RemoteCameraControl.Android
{
    public class HomeViewPagerAdapter : FragmentPagerAdapter
    {
        private ObservableCollection<IDevice> _discoveredDevices;
        private ObservableCollection<IDevice> _connectedDevices;
        private RelayCommand<Guid> _disconnectFromDevice;
        private RelayCommand<Guid> _connectToDevice;

        public HomeViewPagerAdapter(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
        {
        }

        public HomeViewPagerAdapter(
            FragmentManager fm,
            ObservableCollection<IDevice> connectedDevices,
            ObservableCollection<IDevice> discoveredDevices,
            RelayCommand<Guid> connectToDevice,
            RelayCommand<Guid> disconnectFromDevice) : base(fm)
        {
            _connectedDevices = connectedDevices;
            _discoveredDevices = discoveredDevices;
            _connectToDevice = connectToDevice;
            _disconnectFromDevice = disconnectFromDevice;
        }

        public override int Count { get; } = 2;
        
        public override Fragment GetItem(int position)
        {
            if (position == 0)
            {
                return ConnectedDevicesFragment.NewInstance(_connectedDevices, _disconnectFromDevice);
            }
            else
            {
                return DiscoveredDevicesFragment.NewInstance(_discoveredDevices, _connectToDevice);
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