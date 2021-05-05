using System;
using RemoteCameraControl.Android.SelectMode;

namespace RemoteCameraControl
{
    public class SplashViewModel : ViewModelBase
    {
        public SplashViewModel()
        {
        }

        public void NavigateToSelectMode()
        {
            NavigationService.NavigateTo(nameof(ModeSelectViewModel));
            //NavigationService.NavigateTo(nameof(SessionPhotosViewModel));
        }
    }
}
