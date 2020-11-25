namespace RemoteCameraControl.Android
{
    public interface IAppContext
    {
        bool IsRc { get; set; }
        bool IsCamera { get; set; }

        void SetMode(bool isRc);
    }
}