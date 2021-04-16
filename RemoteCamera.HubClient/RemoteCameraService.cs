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

        public async Task CreateSessionAndConnectAsync()
        {
            var sessionName = await _sessionClient.CreateNewSessionAsync();
            await ConnectToSessionAsync(sessionName);
            await _hubService.StartCameraAwaitAsync();
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

    }
}
