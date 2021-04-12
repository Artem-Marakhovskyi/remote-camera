using System;
using System.Threading.Tasks;
using RemoteCameraControl.Logger;

namespace RemoteCamera.HubClient.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            try
            {
                var logger = new Logger();
                logger.AddSource(new ConsoleLogSource());
                logger.CurrentLevel = LogLevel.Debug;

                var baseUrl = "http://192.168.0.102:5000";

                var sessionClient = new SessionClient(baseUrl);
                var sessionName = await sessionClient.CreateNewSessionAsync();
                var success = await sessionClient.ConnectToSessionAsync(sessionName);
                System.Console.WriteLine(success);

            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
            System.Console.ReadLine();
        }
    }
}
