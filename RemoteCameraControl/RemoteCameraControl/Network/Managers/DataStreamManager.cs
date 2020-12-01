using RemoteCameraControl.Android;
using RemoteCameraControl.Logger;

namespace RemoteCameraControl.Network
{
    public class DataStreamManager
    {
        private ILogger _logger;

        public DataStreamManager(
            IAppContext appContext,
            ILogger logger)
        {
            _logger = logger;
        }

       
    }
}
