using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Android.Bluetooth;
using Plugin.BLE.Abstractions.Contracts;
using RemoteCameraControl.Blue;

namespace RemoteCameraControl.Android
{
    public class BluetoothClient : IBluetoothClient
    {
        public async Task ListenTo(IDevice device, object nativeDevice)
        {
            var bluetoothDevice = (BluetoothDevice)nativeDevice;

            var bluetoothSocket = bluetoothDevice.CreateInsecureRfcommSocketToServiceRecord(bluetoothDevice.GetUuids().First().Uuid);

            await bluetoothSocket.ConnectAsync();

            var isConnected = bluetoothSocket.IsConnected;
            await bluetoothSocket.OutputStream.WriteAsync(new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 }, 0, 8, default(CancellationToken));
            bluetoothSocket.Close();
        }
    }
}