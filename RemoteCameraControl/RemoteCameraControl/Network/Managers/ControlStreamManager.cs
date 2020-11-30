using System;
using System.IO;
using RemoteCameraControl.Android;

namespace RemoteCameraControl.Network
{
    public class ControlStreamManager
    {
        private IAppContext _appContext;
        private Stream _stream;

        public ControlStreamManager(
            IAppContext appContext)
        {
            _appContext = appContext;
        }

        internal void SetSource(Stream controlStream)
        {
            _stream = controlStream;
        }
    }
}
