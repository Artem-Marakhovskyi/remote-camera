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
        private readonly IConnectionSignalsHandler _connectionSignalsHandler;
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
            _connectionSignalsHandler = connectionSignalsHandler;
            connectionSignalsHandler.SetInner(this);

        }

        protected async override void OnResume()
        {
            base.OnResume();

            await _permissionService.RequestPermissionAsync(FeaturePermission.Storage);
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

        public override void OnDataMessageReceived(DataMessage dataMessage)
        {
            DialogService.HideLoading();
            _connectionSignalsHandler.SetInner(null);
            NavigationService.NavigateTo(nameof(PhotoMirrorViewModel));
        }

        public override void OnRcConnected()
        {
            if (_appContext.IsCamera)
            {
                NavigationService.NavigateTo(nameof(TakePhotoViewModel));
            }
            else if (_appContext.IsRc)
            {
                DialogService.ShowLoading("Waiting for data to be sent...");
            }
        }
    }
}
