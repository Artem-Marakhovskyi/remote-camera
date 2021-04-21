using System;
namespace RemoteCamera.HubClient
{
    public class DataMessage
    {
        public byte[] Payload { get; set; }

        public DateTime CreatedAt { get; set; }

        public override string ToString()
        {
            return $"Created - {CreatedAt}, Payload - {Payload.Length} bytes";
        }

        public bool IsFullFile { get; set; }
    }
}
