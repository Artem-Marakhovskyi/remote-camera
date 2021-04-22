using System;
using RemoteCameraControl.File;

namespace RemoteCameraControl.Android
{
    public interface IAppContext
    {
        bool IsRc { get; set; }

        string SessionName { get; set; }

        DateTime SessionStart { get; set; }

        bool IsCamera { get; set; }

        void SetMode(bool isRc);

        IRelatedFile CurrentPhoto { get; set; }

        object NavigationResult { get; set; }
    }
}
