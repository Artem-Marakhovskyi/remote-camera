using System;
using System.Threading.Tasks;

namespace RemoteCamera.HubClient
{
    public class HubService
    {
        private string _sessionName;
        private HubClient _hubClient;

        public HubService(HubClient hubClient)
        {
            _hubClient = hubClient;
        }

        public async Task ConnectAsync(string sessionName, IConnectionSignalsHandler connectionSignalsHandler)
        {
            _sessionName = sessionName;

            await _hubClient.ConnectAsync(connectionSignalsHandler);
        }

        public async Task DisconnectAsync()
        {
            _sessionName = null;

            if (_hubClient.IsConnected)
            {
                await _hubClient.DisconnectAsync();
            }
        }

        public async Task SendMessage(string parameter)
        {
            await _hubClient.SendAsync(nameof(SendMessage), parameter);
        }

        public async Task StartCameraAwaitAsync()
        {
            await _hubClient.SendAsync(nameof(StartCameraAwaitAsync), _sessionName);
        }

        public async Task ConnectRcAsync()
        {
            await _hubClient.SendAsync(nameof(ConnectRcAsync), _sessionName);
        }

        public async Task FinishSessionAsync()
        {
            await _hubClient.SendAsync(nameof(FinishSessionAsync), _sessionName);
        }

        public async Task SendControlMessageAsync(ControlMessage controlMessage)
        {
            await _hubClient.SendAsync(nameof(SendControlMessageAsync), controlMessage, _sessionName);
        }

        public async Task SendDataMessageAsync(DataMessage dataMessage)
        {
            await _hubClient.SendAsync(nameof(SendDataMessageAsync), dataMessage, _sessionName);
        }
    }
}
