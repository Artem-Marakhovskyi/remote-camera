using System;
namespace RemoteCamera.HubClient
{
    public class PartialDataMessage
    {
        public Guid PhotoId { get; set; }
        public byte[] Payload { get; set; }
        public int CurrentPartNumber { get; set; }
        public int TotalPartsCount { get; set; }

        public override string ToString()
        {
            return $"Partial data: {Payload.Length} bytes, id = {PhotoId}, ({CurrentPartNumber}/{TotalPartsCount})";
        }
    }
}
