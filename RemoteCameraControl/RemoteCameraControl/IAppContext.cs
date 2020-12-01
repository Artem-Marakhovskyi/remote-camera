using RemoteCameraControl.File;
using RemoteCameraControl.Network;

namespace RemoteCameraControl.Android
{
    public interface IAppContext
    {
        bool IsRc { get; set; }

        bool IsCamera { get; set; }

        void SetMode(bool isRc);

        IRelatedFile CurrentPhoto { get; set; }

        DataStreamManager DataStreamManager { get; set; }

        ControlStreamManager ControlStreamManager { get; set; }
        object NavigationResult { get; set; }
    }
}