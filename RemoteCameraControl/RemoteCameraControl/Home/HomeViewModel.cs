using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Command;
using Plugin.BLE;
using Plugin.BLE.Abstractions.Contracts;
using Plugin.BLE.Abstractions.Exceptions;
using Plugin.BLE.Abstractions.Extensions;
using Plugin.CurrentActivity;
using RemoteCameraControl.Android.RemoteCameraControl.Permissions;
using RemoteCameraControl.Blue;
using RemoteCameraControl.RemoteCameraControl.Interaction;

namespace RemoteCameraControl.Home
{
    public class HomeViewModel : ViewModelBase
    {
        private IBluetooth _bluetooth;
        private IPermissionsRequestor _permissionsRequestor;

        private IBluetoothObserver _bluetoothObserver;
        private RelayCommand<Guid> _connectToDeviceCommand;
        private RelayCommand<Guid> _disconnectFromDeviceCommand;
        private IDialogs _dialogs;

        public HomeViewModel(
            IBluetooth bluetooth,
            IPermissionsRequestor permissionsRequestor,
            IDialogs dialogs)
        {
            _dialogs = dialogs;
            _bluetooth = bluetooth;
            _permissionsRequestor = permissionsRequestor;
            _bluetoothObserver = new BluetoothDevicesObserver();
        }

        public BluetoothState BluetoothState => _bluetooth.GetStatus();

        public RelayCommand<Guid> ConnectToDeviceCommand
            => (_connectToDeviceCommand ??= new RelayCommand<Guid>(ConnectToDevice));

        public RelayCommand<Guid> DisconnectFromDeviceCommand
            => _disconnectFromDeviceCommand ??= new RelayCommand<Guid>(DisonnectToDevice);

        public IDevice CurrentDevice { get; private set; }
        
        private void DisonnectToDevice(Guid obj)
        {
            throw new NotImplementedException();
        }


        private async void ConnectToDevice(Guid deviceId)
        {
            _dialogs.ShowLoading("Connecting to device");
            StopBluetoothProcessing();
            try
            {
                await _bluetooth.ConnectToDeviceAsync(deviceId);
            }
            catch (DeviceConnectionException e)
            {
                _dialogs.HideLoading();
                await _dialogs.AlertAsync("An error occured during device connecting.", "Oops!", "OK");
                throw;
            }
            CurrentDevice = DiscoveredDevices
                .FirstOrDefault(
                    x => x.Id == deviceId) 
                            ?? ConnectedDevices.FirstOrDefault(x => x.Id == deviceId);

            var services = await CurrentDevice.GetServicesAsync();
            foreach (var service in services)
            {
                Logger.LogInfo($"Service found: name - {service.Name}, is primary - {service.IsPrimary}, id - {service.Id}");
                var characteristics = await service.GetCharacteristicsAsync();
                foreach (var characteristic in characteristics)
                {
                    Logger.LogInfo($"\t\tCharacteristic found: name - {characteristic.Name}," +
                                   $" id - {characteristic.Id}, uuid - {characteristic.Uuid}, " +
                                   $"can read - {characteristic.CanRead}, can write - {characteristic.CanWrite}");

                    var descriptors = await characteristic.GetDescriptorsAsync();
                    foreach (var descriptor in descriptors)
                    {
                        Logger.LogInfo($"\t\t\t\tDescriptor: id - {descriptor.Id}, name - {descriptor.Name}, value - {descriptor.Value}");
                    }
                }
            }
            
            _dialogs.HideLoading();
        }

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