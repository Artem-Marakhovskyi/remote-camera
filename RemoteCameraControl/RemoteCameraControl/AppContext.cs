namespace RemoteCameraControl.Android
{
    public class AppContext : IAppContext
    {
        public bool IsRc { get; set; }
        public bool IsCamera { get; set; }
        public void SetMode(bool isRc)
        {
            IsRc = isRc;
            IsCamera = !IsCamera;
        }
    }
}