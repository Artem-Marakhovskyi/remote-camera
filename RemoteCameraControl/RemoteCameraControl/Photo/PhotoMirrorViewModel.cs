using System;
using System.Threading.Tasks;
using RemoteCameraControl.Android;
using RemoteCameraControl.Network;
using RemoteCameraControl.Network.DataTransfer;

namespace RemoteCameraControl.Photo
{
    public class PhotoMirrorViewModel : ViewModelBase
    {
        private IAppContext _appContext;
        private ControlStreamManager _controlStreamManager;

        public PhotoMirrorViewModel(
            IAppContext appContext,
            ControlStreamManager controlStreamManager)
        {
            _appContext = appContext;
            _controlStreamManager = controlStreamManager;
        }

        internal async Task TakePhotoAsync()
        {
            await _controlStreamManager.SendControlSignalAsync(ControlSignal.FromTakePhoto());
        }
    }
}
