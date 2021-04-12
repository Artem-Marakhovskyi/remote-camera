using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR.Client;

namespace RemoteCamera.HubClient
{
    public class HubClient
    {
        private readonly HubConnection _connection;

        public HubClient(string url)
        {
            _connection = new HubConnectionBuilder()
                .WithUrl("http://192.168.0.102:5000/hub")
                .Build();
        }

        private async Task ConnectAsync()
        {
            await _connection.StartAsync();

            _connection.On<string>("J", val => UIApplication.SharedApplication.InvokeOnMainThread(()
                =>
            {
                var dtGot = DateTime.Now;
                textLabel.Text += $"Sent: {sentDt}, got: {dtGot:O}{Environment.NewLine}";
                Console.WriteLine($"Roundtrip: {(sentDt - dtGot).TotalMilliseconds}ms");
            }));
        }
    }
}
