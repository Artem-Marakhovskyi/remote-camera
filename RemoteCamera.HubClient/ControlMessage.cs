using System;
namespace RemoteCamera.HubClient
{
    public class ControlMessage
    {
        public ControlOperationKind Kind { get; set; }

        public override string ToString()
        {
            return $"{Kind}";
        }

        public static ControlMessage From(ControlOperationKind kind)
        {
            return new ControlMessage { Kind = kind };
        }
    }
}
