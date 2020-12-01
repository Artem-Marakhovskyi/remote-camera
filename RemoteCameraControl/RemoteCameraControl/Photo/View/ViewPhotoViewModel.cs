using RemoteCameraControl.Android;
using RemoteCameraControl.File;

namespace RemoteCameraControl.Photo.View
{
    public class ViewPhotoViewModel : ViewModelBase
    {
        private IAppContext _appContext;

        public ViewPhotoViewModel(
            IAppContext appContext)
        {
            _appContext = appContext;
            Photo = _appContext.CurrentPhoto;
        }

        public IRelatedFile Photo { get; private set; }

    }
}
