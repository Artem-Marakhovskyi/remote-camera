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

        public override void GoBack(object parameter = null)
        {
            NavigationService.GoBack();
        }


        internal async Task TakePhotoAsync()
        {
            // await _controlStreamManager.SendControlSignalAsync();
        }

        internal async void TimerClicked()
        {
            // await _controlStreamManager.SendControlSignalAsync(ControlSignal.FromTimer());
        }

        internal async void FocusClicked()
        {
            // await _controlStreamManager.SendControlSignalAsync(ControlSignal.FromFocus());
        }
    }
}
