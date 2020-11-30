using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Android.Bluetooth;
using Android.OS;
using Android.Runtime;
using Java.Lang.Reflect;
using Java.Util;
using Plugin.BLE.Abstractions.Contracts;
using RemoteCameraControl.Blue;
using RemoteCameraControl.Logger;

namespace RemoteCameraControl.Android
{
    public class BluetoothServer : IBluetoothServer
    {
        public BluetoothServer(
            ILogger logger)
        {
            _logger = logger;
        }

        private BluetoothSocket socket = null;
        private OutputStreamInvoker outStream = null;
        private InputStreamInvoker inStream = null;
        private ILogger _logger;

        public async Task ListenTo(IDevice pluginDevice, object nativeDevice)
        {
            var device = (BluetoothDevice) nativeDevice;
            
            BluetoothAdapter adapter = BluetoothAdapter.DefaultAdapter;
            if (adapter == null)
            {
                _logger.LogInfo("No Bluetooth adapter found.");
            }
            else if (!adapter.IsEnabled)
            {
                _logger.LogInfo("Bluetooth adapter is not enabled.");
            }

            if (device == null)
            {
                _logger.LogInfo("Named device not found.");
            }
            else
            {
                _logger.LogInfo("Device has been found: " + device.Name + " " + device.Address + " " +
                              device.BondState.ToString());
            }

            var uuids = device.GetUuids();
            if (uuids == null)
            {
                _logger.LogInfo("Device uuids is null.");
                return;
            }

            var serverSocket
                = adapter.ListenUsingInsecureRfcommWithServiceRecord(device.Name, uuids.First().Uuid);
            
            var socket = await serverSocket.AcceptAsync();
            
            if (socket != null && socket.IsConnected)
            {
                _logger.LogInfo("Connection successful!");
            }
            else
            {
                _logger.LogInfo("Connection failed!");
            }
            
            inStream = (InputStreamInvoker) socket.InputStream;
            outStream = (OutputStreamInvoker) socket.OutputStream;
            
            if (socket != null && socket.IsConnected)
            {
                Task t = new Task(async () => await Listen(inStream));
                t.Start();
            }
            else throw new Exception("Socket not existing or not connected.");
        }

        private async Task Listen(InputStreamInvoker inputStreamInvoker)
        {
            await Task.Delay(5000);
            _logger.LogInfo($"Listening. Length - {inputStreamInvoker.Length}");
        }
    }
}