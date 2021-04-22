using System;
using RemoteCameraControl.Android;

namespace RemoteCameraControl
{
    public class SessionPhotosViewModel : ViewModelBase
    {
        public string SessionName { get; set; }
        public DateTime SessionStartTime { get; set; } = DateTime.MinValue;

        public SessionPhotosViewModel(
            IAppContext appContext)
        {
            SessionName = appContext.SessionName;
            SessionStartTime = appContext.SessionStart;
        }
    }
}
