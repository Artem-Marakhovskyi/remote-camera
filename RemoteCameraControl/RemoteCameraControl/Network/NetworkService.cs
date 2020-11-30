using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace RemoteCameraControl.Network
{
    public static class NetworkService
    {
        public static async Task<IPAddress> GetRemoteAddressAsync()
        {
            var receiveDatagramTask = Task.Run<IPAddress>(async() => await ReceiveDatagram());
            var sendDatagramTask = Task.Run(async () => await SendDatagramAsync(receiveDatagramTask));

            await Task.WhenAll(receiveDatagramTask, sendDatagramTask);

            return receiveDatagramTask.Result;
        }

        public static async Task<IPAddress> GetLocalAddressAsync()
        {
            string hostName = Dns.GetHostName();

            var hostEntry = await Dns.GetHostEntryAsync(hostName);

            return hostEntry.AddressList[0];
        }

        private static async Task<byte[]> GetBroacastDatagramAsync()
        {
            var localAddress = await GetLocalAddressAsync();

            return EnvironmentService.GetBytes(localAddress.ToString());
        }

        private static async Task<IPAddress> ReceiveDatagram()
        {
            var udpClient = new UdpClient(4343, AddressFamily.InterNetwork);
            udpClient.EnableBroadcast = true;
            var result = await udpClient.ReceiveAsync();

            return result.RemoteEndPoint.Address;
        }

        private static async Task SendDatagramAsync(Task receiveDatagramTask)
        {
            //UdpClient udpClient = new UdpClient();
            //udpClient.EnableBroadcast = true;

            //var datagram = await GetBroacastDatagramAsync();
            //do
            //{
            //    await udpClient.SendAsync(
            //        datagram, datagram.Length,
            //        new IPEndPoint(IPAddress.Broadcast, 4343));
            //}
            //while (!receiveDatagramTask.IsCompleted);
        }
    }
}
