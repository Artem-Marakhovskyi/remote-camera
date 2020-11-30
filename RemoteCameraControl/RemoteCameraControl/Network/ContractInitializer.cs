using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace RemoteCameraControl.Network
{
    public class ContractInitializer
    {
        private const int ControlPort = 13271;
        private const int DataPort = 13272;

        private Stream _dataStream;
        private TcpClient _dataClient;
        private Stream _controlStream;
        private TcpClient _controlClient;

        private IPAddress _remoteAddress;

        public async Task InitControlConnectionsAsync(bool isRc)
        {
            _remoteAddress = await NetworkService.GetRemoteAddressAsync();
            var localAddress = await NetworkService.GetLocalAddressAsync();
            var controlConnectionTask = InitControlConnectionAsync(isRc);
            var dataConnectionTask = InitDataConnectionAsync(isRc);

            await Task.WhenAll(controlConnectionTask, dataConnectionTask);

            (_controlClient, _controlStream) = controlConnectionTask.Result;
            (_dataClient, _dataStream) = dataConnectionTask.Result;
        }

        private Task<(TcpClient, Stream)> InitDataConnectionAsync(bool isRc)
        {
            return isRc
                ? BecomeServerAsync(DataPort)
                : BecomeClientAsync(_remoteAddress, DataPort);
        }

        private Task<(TcpClient, Stream)> InitControlConnectionAsync(bool isRc)
        {
            return isRc
                ? BecomeClientAsync(_remoteAddress, ControlPort)
                : BecomeServerAsync(DataPort);
        }

        private async Task<(TcpClient, Stream)> BecomeClientAsync(IPAddress remoteAddress, int port)
        {
            var client = new TcpClient();
            await client.ConnectAsync(remoteAddress, port);
            var stream = client.GetStream();

            return (client, stream);
        }

        private async Task<(TcpClient, Stream)> BecomeServerAsync(int port)
        {
            var local = await NetworkService.GetLocalAddressAsync();
            var listener = new TcpListener(local, port);

            listener.Start();
            var client = await listener.AcceptTcpClientAsync();
            var stream = client.GetStream();

            return (client, stream);
        }
    }
}
