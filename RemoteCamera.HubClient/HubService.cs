using System;
using System.Threading.Tasks;

namespace RemoteCamera.HubClient
{
    public class HubService
    {
        private string _sessionName;
        private HubClient _hubClient;

        public HubService(HubClient hubClient, string sessionName)
        {
            _sessionName = sessionName;
            _hubClient = hubClient;
        }

        public async Task ConnectAsync()
        {
            if (!_hubClient.IsConnected)
            {
                await _hubClient.ConnectAsync();
            }
        }

        public async Task DisconnectAsync()
        {
            if (_hubClient.IsConnected)
            {
                await _hubClient.DisconnectAsync();
            }
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
