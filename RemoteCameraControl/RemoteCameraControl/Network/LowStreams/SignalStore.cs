using System;
namespace RemoteCameraControl.Network
{
    public class SignalStore
    {
        public static readonly string StartText = "d_7f1saa";
        public static readonly string EndText = "dd1a12d_";
        public static readonly byte[] StartMark = EnvironmentService.GetBytes(StartText);
        public static readonly byte[] EndMark = EnvironmentService.GetBytes(EndText);
    }
}
