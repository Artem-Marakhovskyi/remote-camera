using System.Threading.Tasks;
using Plugin.BLE.Abstractions.Contracts;
using RemoteCameraControl.Logger;

namespace RemoteCameraControl.Blue
{
    public class Bluetooth : IBluetooth
    {
        private readonly IBluetoothLE _plugin;
        private ILogger _logger;

        public Bluetooth(
            IBluetoothLE plugin,
            ILogger logger)
        {
            _plugin = plugin;
            _logger = logger;
        }
        
        public Task GetStatusAsync()
        {
            var state = _plugin.State;


            return Task.CompletedTask;
        }
    }
}