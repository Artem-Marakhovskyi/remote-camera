using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using RemoteCameraControl.Logger;
using XLabs.Ioc;

namespace RemoteCameraControl.Network
{
    public static class NetworkService
    {
        public static async Task<IPAddress> GetRemoteAddressAsync(bool isRc)
        {
            IPAddress remoteAddress = null;
            var logger = Resolver.Resolve<ILogger>();
            if (isRc)
            {
                logger.LogInfo("Started as RC");
                logger.LogInfo("Receiving datagrams...");
                remoteAddress = await ReceiveDatagramAsync();
                logger.LogInfo("Remote address is received.");
                await Task.Delay(TimeSpan.FromSeconds(10));
                logger.LogInfo("Sending datagrams...");
                await SendDatagramAsync();
                logger.LogInfo("Datagrams sent");
            }
            else
            {

                logger.LogInfo("Started as Camera");
                logger.LogInfo("Sending datagrams...");
                await SendDatagramAsync();
                logger.LogInfo("Datagrams sent...");
                await Task.Delay(TimeSpan.FromSeconds(2));
                logger.LogInfo("Receiving datagrams...");
                remoteAddress = await ReceiveDatagramAsync();
                logger.LogInfo("Remote address is received.");
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
            try
            {
                var udpClient = new UdpClient(4343, AddressFamily.InterNetwork);
                udpClient.EnableBroadcast = true;
                var result = await udpClient.ReceiveAsync();

                udpClient.Close();
                udpClient.Dispose();

                return result.RemoteEndPoint.Address;

            }
            catch (Exception ex)
            {
                Resolver.Resolve<ILogger>().LogError("Error while receiving datagrams", ex);
            }
            return null;
        }

        private static async Task SendDatagramAsync()
        {
            try
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
            catch (Exception ex)
            {
                Resolver.Resolve<ILogger>().LogError("Error while sending datagrams", ex);
            }
        }
    }
}
