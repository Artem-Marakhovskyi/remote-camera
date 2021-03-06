using RemoteCameraControl.File;
using RemoteCameraControl.Hub;
using RemoteCameraControl.Logger;
using RemoteCameraControl.Network;

namespace RemoteCameraControl.Android
{
    public class AppContext : IAppContext
    {
        private ILogger _logger;

        public bool IsRc { get; set; }
        public bool IsCamera { get; set; }
        public DataStreamManager DataStreamManager { get; set; }
        public ControlStreamManager ControlStreamManager { get; set; }
        public IRelatedFile CurrentPhoto { get; set; }
        public object NavigationResult { get; set; }

        public AppContext(
            IDataSignalPublisher dataSignalPublisher,
            IControlSignalPublisher publisher, ILogger logger)
        {
            _logger = logger;

            DataStreamManager = new DataStreamManager(dataSignalPublisher,this, _logger);
            ControlStreamManager = new ControlStreamManager(publisher, _logger, this);
        }

        public void SetMode(bool isRc)
        {
            IsRc = isRc;
            IsCamera = !isRc;
        }
    }
}