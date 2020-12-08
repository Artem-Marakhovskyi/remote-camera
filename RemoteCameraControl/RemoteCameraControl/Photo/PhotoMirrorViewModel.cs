using System;
using System.Threading.Tasks;
using RemoteCameraControl.Android;
using RemoteCameraControl.Hub;
using RemoteCameraControl.Network;
using RemoteCameraControl.Network.DataTransfer;

namespace RemoteCameraControl.Photo
{
    public class PhotoMirrorViewModel : ViewModelBase, IDataSignalListener
    {
        private IAppContext _appContext;
        private ControlStreamManager _controlStreamManager;
        private IDataSignalHubManager _dataSignalHubManager;

        public PhotoMirrorViewModel(
            IDataSignalHubManager dataSignalHubManager,
            IAppContext appContext,
            ControlStreamManager controlStreamManager)
        {
            _appContext = appContext;
            _controlStreamManager = controlStreamManager;
            _dataSignalHubManager = dataSignalHubManager;

            _dataSignalHubManager.AddListener(this);
        }

        public override void GoBack(object parameter = null)
        {
            NavigationService.GoBack();
        }

        private DataSignal _dataSignal;
        public DataSignal DataSignal
        {
            get
            {
                return _dataSignal;
            }
            set
            {
                Set(nameof(DataSignal), ref _dataSignal, value);
            }
        }

        internal async Task TakePhotoAsync()
        {
            await _controlStreamManager.SendControlSignalAsync(ControlSignal.FromTakePhoto());
        }

        public void OnDataSignalReceived(DataSignal dataSignal)
        {
            DataSignal = dataSignal;
        }

        internal async void TimerClicked()
        {
            await _controlStreamManager.SendControlSignalAsync(ControlSignal.FromTimer());
        }

        internal async void FocusClicked()
        {
            await _controlStreamManager.SendControlSignalAsync(ControlSignal.FromFocus());
        }
    }
}
