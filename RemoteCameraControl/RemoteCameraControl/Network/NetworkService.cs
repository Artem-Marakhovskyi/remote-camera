using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace RemoteCameraControl.Network
{
    public static class NetworkService
    {
        public static async Task<IPAddress> GetRemoteAddressAsync(bool isRc)
        {
            IPAddress remoteAddress = null;

            if (isRc)
            {
                remoteAddress = await ReceiveDatagramAsync();
                await Task.Delay(TimeSpan.FromSeconds(10));
                await SendDatagramAsync();
            }
            else
            {
                await SendDatagramAsync();
                await Task.Delay(TimeSpan.FromSeconds(2));
                remoteAddress = await ReceiveDatagramAsync();
            }

            return remoteAddress;
        }

        public static async Task<IPAddress> GetLocalAddressAsync()
        {
            string hostName = Dns.GetHostName();

            var hostEntry = await Dns.GetHostEntryAsync(hostName);

            return hostEntry.AddressList[0];
        }

        private static async Task<IPAddress> ReceiveDatagramAsync()
        {
            var udpClient = new UdpClient(4343, AddressFamily.InterNetwork);
            udpClient.EnableBroadcast = true;
            var result = await udpClient.ReceiveAsync();

            udpClient.Close();
            udpClient.Dispose();

            return result.RemoteEndPoint.Address;
        }

        private static async Task SendDatagramAsync()
        {
            var udpClient = new UdpClient();
            udpClient.EnableBroadcast = true;

            var tcs = new CancellationTokenSource();
            tcs.CancelAfter(TimeSpan.FromSeconds(10));

            while (!tcs.IsCancellationRequested)
            {
                await Task.Delay(500);
                await udpClient.SendAsync(
                    new byte[] { 0 }, 1,
                    new IPEndPoint(IPAddress.Broadcast, 4343));
            }

            udpClient.Close();
            udpClient.Dispose();
        }
    }
}
