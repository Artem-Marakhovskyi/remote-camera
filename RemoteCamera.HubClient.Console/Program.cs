using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RemoteCamera.HubClient.Console;
using RemoteCameraControl.Logger;

namespace RemoteCamera.HubClient.Cnsl
{
    class Program
    {
        private Dictionary<string, Func<Task>> _actionsSwitch
            = new Dictionary<string, Func<Task>>(StringComparer.CurrentCultureIgnoreCase);

        private ParamsHolder _paramsHolder;

        private RemoteCameraService _remoteCameraService;

        private Lazy<IConnectionSignalsHandler> _connectionSignalsHandlerLazy;

        private readonly Logger _logger;

        public Program()
        {
            var logger = new Logger();
            logger.AddSource(new ConsoleLogSource());
            logger.CurrentLevel = LogLevel.Debug;

            _logger = logger;
            _connectionSignalsHandlerLazy = new Lazy<IConnectionSignalsHandler>(() => new NullConnectionSignalsHandler(_logger));

            var baseUrl = "https://remotecamera.azurewebsites.net/";
            _remoteCameraService = new RemoteCameraService(new NullConnectionSignalsHandler(_logger), new HubService(new HubClient(baseUrl + "/hub", _logger)), new SessionClient(baseUrl), _logger);
            _paramsHolder = new ParamsHolder();
            InitializeBranches(_remoteCameraService);
        }

        public static async Task Main(string[] args)
        {
            try
            {
                var executor = new Program();
                await executor.RunCycleAsync();
            }
            catch (Exception ex)
            {
                System.Console.WriteLine(ex);
            }
            System.Console.ReadLine();
        }

        private async Task RunCycleAsync()
        {
            System.Console.WriteLine("Available commands: rc <session_name>, camera, finish, control, data, reset, text <message>");
            _paramsHolder.Update(ReadLn());
            while (_paramsHolder.Any() && _paramsHolder[0] != "END")
            {
                if (_actionsSwitch.ContainsKey(_paramsHolder[0]))
                {
                    await _actionsSwitch[_paramsHolder[0]]();
                }
                else
                {
                    System.Console.WriteLine("Check command name, please.");
                }
                _paramsHolder.Update(ReadLn());

            }
        }

        private void InitializeBranches(RemoteCameraService service)
        {
            _actionsSwitch["camera"] = () => service.CreateSessionAndConnectAsync();
            _actionsSwitch["rc"] = () => service.ConnectRcAsync(_paramsHolder[1]);
            _actionsSwitch["finish"] = () => service.FinishSessionAsync();
            _actionsSwitch["control"] = () => service.SendControlMessageAsync(new ControlMessage() { Kind = ControlOperationKind.TakePhoto });
            _actionsSwitch["data"] = () => service.SendDataMessageAsync(new DataMessage() { Payload = GetPayload(), CreatedAt = DateTime.Now });
            _actionsSwitch["text"] = () => service.SendMessageAsync(_paramsHolder[1]);
            _actionsSwitch["reset"] = () => service.Disconnect();
            _actionsSwitch["partial_data"] = () => SendPartialAsync(service);
        }

        private async Task SendPartialAsync(RemoteCameraService service)
        {
            var bytes = GetPayload();
            await service.SendPartialDataMessageAsync(bytes);
        }

        private byte[] GetPayload()
        {
            var bytes = File.ReadAllBytes("/Users/amara/Documents/GitHub/remote-camera/RemoteCamera.HubClient.Console/photo.png");
            return bytes;
        }

        private static string ReadLn() => System.Console.ReadLine();
    }
}
