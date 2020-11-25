using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;

namespace RemoteCameraControl.Blue
{
    public interface IBluetoothClient
    {
        Task ListenTo(IDevice device, object bluetoothDevice);
    }
}