using System;
namespace RemoteCameraControl.Network
{
    public class SignalStore
    {
        public static readonly byte[] StartMark = EnvironmentService.GetBytes("d_7a");
        public static readonly byte[] EndMark = EnvironmentService.GetBytes("dd1a");
    }
}
