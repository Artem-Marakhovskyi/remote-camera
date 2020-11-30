using RemoteCameraControl.Network;

namespace RemoteCameraControl.Android
{
    public class AppContext : IAppContext
    {
        public bool IsRc { get; set; }
        public bool IsCamera { get; set; }
        public DataStreamManager DataStreamManager { get; set; }
        public ControlStreamManager ControlStreamManager { get; set; }

        public void SetMode(bool isRc)
        {
            IsRc = isRc;
            IsCamera = !IsCamera;
        }
    }
}