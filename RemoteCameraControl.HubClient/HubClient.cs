using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace RemoteCameraControl.HubClient
{
    public class HubClient
    {
        private object _connection;

        public HubClient(string url)
        {
            _connection = new HubConnectionBuilder()
            .WithUrl(url)
            .Build();
        }

        public async Task Connect(
        {

        }
    }
}
