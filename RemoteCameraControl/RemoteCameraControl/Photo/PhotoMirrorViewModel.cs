using System;
using System.Threading.Tasks;
using RemoteCamera.HubClient;
using RemoteCameraControl.Android;

namespace RemoteCameraControl.Photo
{
    public class PhotoMirrorViewModel : ViewModelBase, IConnectionSignalsHandler
    {
        private IAppContext _appContext;
        private readonly RemoteCameraService _remoteCameraService;
        private readonly IConnectionSignalsHandler _connectionSignalsHandler;

        public byte[] Payload { get; private set; }
        public DateTime LatestPhotoTime { get; private set; }

        public PhotoMirrorViewModel(
            IAppContext appContext,
            IConnectionSignalsHandler connectionSignalsHandler,
            RemoteCameraService remoteCameraService)
        {
            _appContext = appContext;
            _remoteCameraService = remoteCameraService;
            _connectionSignalsHandler = connectionSignalsHandler;
            _connectionSignalsHandler.SetInner(this);
        }

        public override void GoBack(object parameter = null)
        {
            NavigationService.GoBack();
        }


        internal async Task TakePhotoAsync()
        {
            await _remoteCameraService.SendControlMessageAsync(ControlMessage.From(ControlOperationKind.TakePhoto));
        }

        internal async void TimerClicked()
        {
            await _remoteCameraService.SendControlMessageAsync(ControlMessage.From(ControlOperationKind.Timer3));
        }

        internal async void FocusClicked()
        {
            await _remoteCameraService.SendControlMessageAsync(ControlMessage.From(ControlOperationKind.Focus));
        }

        public override void OnDataMessageReceived(DataMessage dataMessage)
        {
            Payload = dataMessage.Payload;
            LatestPhotoTime = dataMessage.CreatedAt;
        }
    }
}
