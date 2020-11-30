using RemoteCameraControl.Home;

namespace RemoteCameraControl.Android.SelectMode
{
    public class ModeSelectViewModel : ViewModelBase
    {
        private IAppContext _appContext;

        public  ModeSelectViewModel(
            IAppContext appContext)
        {
            _appContext = appContext;
        }

        public void BecomeCamera()
        {
            _appContext.SetMode(isRc: false);
            NavigationService.NavigateTo(nameof(HomeViewModel));
        }

        public void BecomeRc()
        {
            _appContext.SetMode(isRc: true);
            NavigationService.NavigateTo(nameof(HomeViewModel));
        }
    }
}