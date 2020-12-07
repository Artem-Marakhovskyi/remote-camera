using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using RemoteCameraControl.Android;
using RemoteCameraControl.Logger;
using RemoteCameraControl.RemoteCameraControl.Interaction;

namespace RemoteCameraControl.Network
{
    public class ContractInitializer
    {
        private const int ControlPort = 13271;
        private const int DataPort = 13272;

        private TcpClient _dataClient;
        private TcpClient _controlClient;
        private ILogger _logger;
        private DataStreamManager _dataStreamManager;
        private IPAddress _remoteAddress;
        private IPAddress _localAddress;
        private IDialogs _dialogs;
        private IAppContext _appContext;
        private ControlStreamManager _controlStreamManager;

        public ContractInitializer(
            IDialogs dialogs,
            DataStreamManager dataStreamManager,
            ControlStreamManager controlStreamManager,
            IAppContext appContext,
            ILogger logger)
        {
            _dialogs = dialogs;
            _appContext = appContext;
            _dataStreamManager = dataStreamManager;
            _controlStreamManager = controlStreamManager;
            _appContext.ControlStreamManager = _controlStreamManager;
            _appContext.DataStreamManager = _dataStreamManager;
            _logger = logger;
        }

        public async Task InitControlConnectionsAsync()
        {
            try
            {
                _logger.LogInfo("Connection Init started");
                _localAddress = await NetworkService.GetLocalAddressAsync();
                _logger.LogInfo($"Local address: {_localAddress}");

                //_remoteAddress = IPAddress.Parse("192.168.0.100");
                _remoteAddress = await NetworkService.GetRemoteAddressAsync(_appContext.IsRc);

                _dialogs.ShowLoading("Remote address found");

                _logger.LogInfo($"Remote address: {_remoteAddress}"); _logger.LogInfo($"Remote address: {_remoteAddress}");

                var controlConnectionTask = InitControlConnectionAsync();
                var dataConnectionTask = InitDataConnectionAsync();

                await Task.WhenAll(controlConnectionTask, dataConnectionTask);

                _dialogs.ShowLoading("Connection is established.");

                var (_dataClient, dataStream) = dataConnectionTask.Result;
                var (_controlClient, controlStream) = controlConnectionTask.Result;

                _appContext.DataStreamManager.SetSource(dataStream);
                _appContext.ControlStreamManager.SetSource(controlStream);
            }
            catch (Exception ex)
            {
                _logger.Equals(ex);
            }
        }

        private Task<(TcpClient, Stream)> InitDataConnectionAsync()
        {
            return _appContext.IsRc
                ? BecomeServerAsync(DataPort)
                : BecomeClientAsync(_remoteAddress, DataPort);
        }

        private Task<(TcpClient, Stream)> InitControlConnectionAsync()
        {
            return _appContext.IsRc
                ? BecomeClientAsync(_remoteAddress, ControlPort)
                : BecomeServerAsync(ControlPort);
        }

        private async Task<(TcpClient, Stream)> BecomeClientAsync(IPAddress remoteAddress, int port)
        {
            var client = new TcpClient();
            try
            {
                _logger.LogInfo($"Becoming a client of {remoteAddress}:{port}");

                while (!await TryConnectClientAsync(client, remoteAddress, port))
                {
                    _logger.LogInfo($"Timeout 1000ms for client connection of {remoteAddress}:{port}");
                    await Task.Delay(1000);
                }

                var stream = client.GetStream();

                _logger.LogInfo($"Connected to {remoteAddress}:{port}");

                return (client, stream);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An Error occured when becoming a clint of {remoteAddress}:{port}", ex);

                return (null, null);
            }
        }

        private async Task<bool> TryConnectClientAsync(TcpClient client, IPAddress remoteAddress, int port)
        {
            try
            {
                await client.ConnectAsync(remoteAddress, port);

                return true;
            }
            catch (SocketException e)
            {
                return false;
            }
        }

        private async Task<(TcpClient, Stream)> BecomeServerAsync(int port)
        {
            var listener = new TcpListener(_localAddress, port);

            try
            {
                _logger.LogInfo($"Becoming a server for :{_localAddress}:{port}");
                listener.Start();
                var client = await listener.AcceptTcpClientAsync();
                var stream = client.GetStream();
                _logger.LogInfo($"Accepted a client for :{_localAddress}:{port}");

                return (client, stream);
            }
            catch (Exception ex)
            {
                _logger.LogError($"An Error occured when becoming a server for :{_localAddress}:{port}", ex);

                return (null, null);
            }
        }
    }
}
