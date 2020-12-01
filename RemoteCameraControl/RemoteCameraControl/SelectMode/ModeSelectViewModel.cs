using RemoteCameraControl.Network;
using RemoteCameraControl.Permissions;
using RemoteCameraControl.Photo;

namespace RemoteCameraControl.Android.SelectMode
{
    public class ModeSelectViewModel : ViewModelBase
    {
        private IAppContext _appContext;
        private IPermissionService _permissionService;
        private ContractInitializer _contractInitializer;


        public  ModeSelectViewModel(
            IAppContext appContext,
            ContractInitializer contractInitializer,
            IPermissionService permissionService)
        {
            _appContext = appContext;
            _permissionService = permissionService;
            _contractInitializer = contractInitializer;
        }

        public async void BecomeCamera()
        {
            await _permissionService.RequestPermissionAsync(FeaturePermission.Camera);
            _appContext.SetMode(isRc: false);

            //await _contractInitializer
            //    .InitControlConnectionsAsync();

            NavigationService.NavigateTo(nameof(TakePhotoViewModel));
        }

        public async void BecomeRc()
        {
            _appContext.SetMode(isRc: true);

            await _contractInitializer
                .InitControlConnectionsAsync();

            NavigationService.NavigateTo(nameof(PhotoMirrorViewModel));
        }
    }
}