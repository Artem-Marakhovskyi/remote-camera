using RemoteCameraControl.File;

namespace RemoteCameraControl.Android
{
    public interface IAppContext
    {
        bool IsRc { get; set; }

        bool IsCamera { get; set; }

        void SetMode(bool isRc);

        IRelatedFile CurrentPhoto { get; set; }

        object NavigationResult { get; set; }
    }
}
