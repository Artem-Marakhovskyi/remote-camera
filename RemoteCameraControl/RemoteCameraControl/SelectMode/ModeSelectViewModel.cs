using RemoteCameraControl.Network;
using RemoteCameraControl.Photo;

namespace RemoteCameraControl.Android.SelectMode
{
    public class ModeSelectViewModel : ViewModelBase
    {
        private IAppContext _appContext;
        private ContractInitializer _contractInitializer;


        public  ModeSelectViewModel(
            IAppContext appContext,
            ContractInitializer contractInitializer)
        {
            _appContext = appContext;
            _contractInitializer = contractInitializer;
        }

        public async void BecomeCamera()
        {
            _appContext.SetMode(isRc: false);

            //await _contractInitializer
            //    .InitControlConnectionsAsync();

            NavigationService.NavigateTo(nameof(PhotoViewModel));
        }

        public async void BecomeRc()
        {
            _appContext.SetMode(isRc: true);

            //await _contractInitializer
            //    .InitControlConnectionsAsync();

            NavigationService.NavigateTo(nameof(PhotoMirrorViewModel));
        }
    }
}