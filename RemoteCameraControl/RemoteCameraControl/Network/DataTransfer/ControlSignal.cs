namespace RemoteCameraControl.Network.DataTransfer
{
    public class ControlSignal
    {
        public ControlSignal()
        {
        }

        public bool TakePhoto { get; set; }

        internal static ControlSignal FromTakePhoto()
        {
            return new ControlSignal
            {
                TakePhoto = true
            };
        }
    }
}
