using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;

namespace RemoteCameraControl.Blue
{
    public interface IBluetoothServer
    {
        Task ListenTo(IDevice device, object bluetoothDevice);
    }
}