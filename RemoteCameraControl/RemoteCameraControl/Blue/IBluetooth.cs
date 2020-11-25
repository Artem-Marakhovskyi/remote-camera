using System;
using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;

namespace RemoteCameraControl.Blue
{
    public interface IBluetooth
    {
        void StartDeviceObservation(IBluetoothObserver observer);
        void FinishDeviceObservation();
        
        BluetoothState GetStatus();

        Task ConnectToDeviceAsync(Guid deviceId);
    }
}