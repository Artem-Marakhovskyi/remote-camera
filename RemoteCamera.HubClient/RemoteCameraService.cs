using System;
using System.Threading.Tasks;
using RemoteCameraControl.Logger;

namespace RemoteCamera.HubClient
{
    public class RemoteCameraService
    {
        private readonly SessionClient _sessionClient;
        private readonly ILogger _logger;
        private HubService _hubService;
        private readonly IConnectionSignalsHandler _connectionSignalsHandler;

        public RemoteCameraService(
            IConnectionSignalsHandler connectionSignalsHandler,
            HubService hubService,
            SessionClient sessionClient,
            ILogger logger)
        {
            _hubService = hubService;
            _connectionSignalsHandler = connectionSignalsHandler;
            _sessionClient = sessionClient;
            _logger = logger;
        }

        public async Task<string> CreateSessionAndConnectAsync()
        {
            var sessionName = await _sessionClient.CreateNewSessionAsync();
            await ConnectToSessionAsync(sessionName);
            await _hubService.StartCameraAwaitAsync();

            return sessionName;
        }

        public async Task ConnectRcAsync(string sessionName)
        {
            await ConnectToSessionAsync(sessionName);
            await _hubService.ConnectRcAsync();
        }

        public async Task FinishSessionAsync()
        {
            await _hubService.FinishSessionAsync();
        }

        public async Task SendControlMessageAsync(ControlMessage controlMessage)
        {
            await _hubService.SendControlMessageAsync(controlMessage);
        }

        public async Task SendDataMessageAsync(DataMessage dataMessage)
        {
            await _hubService.SendDataMessageAsync(dataMessage);
        }

        public async Task SendPartialDataMessageAsync(byte[] bytes)
        {
            var packageSize = 32000;
            var partialTemplate = new PartialDataMessage
            {
                PhotoId = Guid.NewGuid(),
                TotalPartsCount = bytes.Length / packageSize + (bytes.Length % packageSize > 0 ? 1 : 0)
            };

            for (var i = 0; i < partialTemplate.TotalPartsCount; i++)
            {
                var packagesSent = i;
                var dst = new byte[
                    i < partialTemplate.TotalPartsCount - 1
                        ? packageSize
                        : bytes.Length - (packageSize * packagesSent)];
                Array.Copy(bytes, packageSize * packagesSent, dst, 0, dst.Length);

                var dataMessage = new PartialDataMessage
                {
                    PhotoId = partialTemplate.PhotoId,
                    TotalPartsCount = partialTemplate.TotalPartsCount,
                    CurrentPartNumber = i,
                    Payload = dst
                };
                await _hubService.SendPartialDataMessageAsync(dataMessage);
            }

        }

        public async Task SendMessageAsync(string text)
        {
            await _hubService.SendMessage(text);
        }

        private async Task ConnectToSessionAsync(string sessionName)
        {
            if (await _sessionClient.ConnectToSessionAsync(sessionName))
            {
                await _hubService.ConnectAsync(sessionName, _connectionSignalsHandler);

                _logger.LogInfo($"Connected to session '{sessionName}'.");
            }
            else
            {
                _logger.LogWarning($"Couldn't connect to session {sessionName}");
            }
        }

        public async Task Disconnect()
        {
            try
            {
                await _hubService.DisconnectAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}
