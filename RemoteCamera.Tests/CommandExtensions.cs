using RemoteCameraControl.AsyncCommands;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace RemoteCamera.Tests
{
    public static class CommandExtensions
    {
        public static async Task ExecuteAsync(this ICommand command, object param = null)
        {
            if (command is AsyncRelayCommand asyncRelayCommand)
            {
                await asyncRelayCommand.ExecuteAsync(param);
            }
            else
            {
                command.Execute(param);
            }
        }
    }
}
