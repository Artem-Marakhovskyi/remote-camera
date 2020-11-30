using System;
namespace RemoteCameraControl.Network
{
    public class SignalStore
    {
        public static readonly byte[] StartMark = new byte[] { 0, 1, 2, 3 };
        public static readonly byte[] EndMark = new byte[] { 3, 2, 1, 0 };
    }
}
