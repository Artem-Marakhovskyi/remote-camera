using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;
using RemoteCameraControl.Logger;

namespace RemoteCamera.HubClient
{
    public class HubClient
    {
        private readonly string _url;
        private readonly HubConnection _connection;
        private readonly ILogger _logger;

        public HubClient(
            string url,
            ILogger logger)
        {
            _url = url;
            _connection = new HubConnectionBuilder()
                .WithUrl(url)
                .Build();
            _logger = logger;
        }

        public bool IsConnected => _connection.State == HubConnectionState.Connected;

        public async Task ConnectAsync(IConnectionSignalsHandler handler)
        {
            await _connection.StartAsync();
            _logger.LogInfo($"Connected to a server: {_url}");

            _connection.On("OnRcConnected", handler.OnRcConnected);
            _connection.On("OnSessionFinished", handler.OnSessionFinished);
            _connection.On<ControlMessage>("OnControlMessageReceived", handler.OnControlMessageReceived);
            _connection.On<DataMessage>("OnDataMessageReceived", handler.OnDataMessageReceived);
            _connection.On<PartialDataMessage>("OnPartialDataMessageReceived", handler.OnPartialDataMessageReceived);
            _connection.On<string>("OnTextReceived", handler.OnTextReceived);
        }

        public async Task DisconnectAsync()
        {
            await _connection.StopAsync();
            _logger.LogInfo($"Disconnected from the server: {_url}");
        }

        public async Task SendAsync<T>(string methodName, T parameter)
        {
            await _connection.SendAsync(methodName, parameter);
            _logger.LogInfo($"Sent message '{methodName}': {parameter}");
        }

        public async Task SendAsync<T, U>(string methodName, T paramT, U paramU)
        {
            await _connection.SendAsync(methodName, paramT, paramU);
            _logger.LogInfo($"Sent message '{methodName}': {paramT}, {paramU}");
        }
    }
}
