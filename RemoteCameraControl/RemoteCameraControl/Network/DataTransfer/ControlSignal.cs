namespace RemoteCameraControl.Network.DataTransfer
{
    public class ControlSignal
    {
        public ControlSignal()
        {
        }

        public bool TakePhoto { get; set; }

        public bool Focus { get; set; }

        public bool Timer { get; set; }

        internal static ControlSignal FromTakePhoto()
        {
            return new ControlSignal
            {
                TakePhoto = true
            };
        }

        internal static ControlSignal FromFocus()
        {
            return new ControlSignal
            {
                Focus = true
            };
        }

        internal static ControlSignal FromTimer()
        {
            return new ControlSignal
            {
                Timer = true
            };
        }
    }
}
