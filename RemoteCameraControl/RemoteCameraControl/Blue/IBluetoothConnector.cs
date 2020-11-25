using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;

namespace RemoteCameraControl.Blue
{
    public interface IBluetoothConnector
    {
        Task ListenTo(IDevice device, object bluetoothDevice);
    }
}