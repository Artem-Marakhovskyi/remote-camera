using RemoteCameraControl.File;
using RemoteCameraControl.Logger;

namespace RemoteCameraControl.Android
{
    public class AppContext : IAppContext
    {
        private ILogger _logger;

        public bool IsRc { get; set; }
        public bool IsCamera { get; set; }
        public IRelatedFile CurrentPhoto { get; set; }
        public object NavigationResult { get; set; }
        public string SessionName { get; set; }

        public AppContext(ILogger logger)
        {
            _logger = logger;
        }

        public void SetMode(bool isRc)
        {
            IsRc = isRc;
            IsCamera = !isRc;
        }
    }
}
