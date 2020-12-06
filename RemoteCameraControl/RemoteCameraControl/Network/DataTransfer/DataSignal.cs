using System;
namespace RemoteCameraControl.Network.DataTransfer
{
    public class DataSignal
    {
        public DataSignal()
        {
        }

        public byte[] Payload { get; set; }

        public static DataSignal FromBytes(byte[] payload)
        {
            return new DataSignal()
            {
                Payload = payload
            };
        }
    }
}
