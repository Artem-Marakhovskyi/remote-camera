﻿using System;
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
        public bool FullFileReceived { get; set; }
        public string SessionName => _appContext.SessionName;

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


        internal async Task TakePhotoAsync(int count = 1)
        {
            if (count != 3)
            {
                await _remoteCameraService.SendControlMessageAsync(ControlMessage.From(ControlOperationKind.TakePhoto));
            }
            else
            {
                await _remoteCameraService.SendControlMessageAsync(ControlMessage.From(ControlOperationKind.TakePhoto3Delay1));
            }
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
            LatestPhotoTime = dataMessage.CreatedAt;

            if (dataMessage.IsFullFile)
            {
                FullFileReceived = dataMessage.IsFullFile;
                RaisePropertyChanged(nameof(FullFileReceived));
            }
            Payload = dataMessage.Payload;

        }

        internal void EndSession()
        {
            _connectionSignalsHandler.SetInner(null);
            NavigationService.NavigateTo(nameof(SessionPhotosViewModel));
        }
    }
}
