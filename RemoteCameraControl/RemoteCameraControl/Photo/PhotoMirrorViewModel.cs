using System;
using System.Threading.Tasks;
using RemoteCameraControl.Android;

namespace RemoteCameraControl.Photo
{
    public class PhotoMirrorViewModel : ViewModelBase
    {
        private IAppContext _appContext;

        public PhotoMirrorViewModel(
            IAppContext appContext)
        {
            _appContext = appContext;
        }

        internal Task TakePhotoAsync()
        {
            
        }
    }
}
