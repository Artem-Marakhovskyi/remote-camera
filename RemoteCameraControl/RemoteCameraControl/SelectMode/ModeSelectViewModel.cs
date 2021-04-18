using RemoteCamera.HubClient;
using RemoteCameraControl.Permissions;
using RemoteCameraControl.Photo;

namespace RemoteCameraControl.Android.SelectMode
{
    public class ModeSelectViewModel : ViewModelBase, IConnectionSignalsHandler
    {
        private IAppContext _appContext;
        private IPermissionService _permissionService;
        private readonly RemoteCameraService _remoteCameraService;

        private string _cameraAwaitSessionName;
        public string CameraAwaitSessionName
        {
            get => _cameraAwaitSessionName;
            set => Set(nameof(CameraAwaitSessionName), ref _cameraAwaitSessionName, value);
        }

        private string _sessionName;
        public string SessionName
        {
            get => _sessionName;
            set => Set(nameof(SessionName), ref _sessionName, value);
        }

        public  ModeSelectViewModel(
            IAppContext appContext,
            IPermissionService permissionService,
            RemoteCameraService remoteCameraService,
            IConnectionSignalsHandler connectionSignalsHandler)
        {
            _appContext = appContext;
            _permissionService = permissionService;
            _remoteCameraService = remoteCameraService;
            connectionSignalsHandler.SetInner(this);

        }

        public async void BecomeCamera()
        {
            await _permissionService.RequestPermissionAsync(FeaturePermission.Camera);
            _appContext.SetMode(isRc: false);

            DialogService.ShowLoading("Connecting to the server...");

            var sessionName = await _remoteCameraService.CreateSessionAndConnectAsync();
            CameraAwaitSessionName = sessionName.ToUpperInvariant();

            DialogService.HideLoading();
        }

        public async void BecomeRc()
        {
            _appContext.SetMode(isRc: true);

            DialogService.ShowLoading($"Connecting to a session \"{SessionName}\"...");
            await _remoteCameraService.ConnectRcAsync(SessionName);

        }

        public override void OnRcConnected()
        {
            if (_appContext.IsCamera)
            {
                NavigationService.NavigateTo(nameof(TakePhotoViewModel));
            }
            else if (_appContext.IsRc)
            {
                DialogService.HideLoading();
                NavigationService.NavigateTo(nameof(PhotoMirrorViewModel));
            }
        }
    }
}
