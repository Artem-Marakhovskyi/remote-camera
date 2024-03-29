﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;
using RemoteCamera.HubClient;
using RemoteCameraControl.AsyncCommands;
using RemoteCameraControl.Permissions;

namespace RemoteCameraControl.Photo
{
    public class TakePhotoViewModel : ViewModelBase, IConnectionSignalsHandler
    {
        private readonly IConnectionSignalsHandler _connectionSignalsHandler;
        private readonly IPermissionService _permissionService;
        private readonly RemoteCameraService _remoteCameraService;
        private Type _navigationViewModelType;
        private ICommand _skipNavigationCommand;
        private AsyncRelayCommand<MemoryStream> _usePhotoCommand;
        private AsyncRelayCommand<MemoryStream> _takeOrSelectPhotoCommand;
        private ICommand _retakePhotoCommand;
        private ICommand _backNavigationCommand;
        private List<PhotoResult> _takePhotoResults = new List<PhotoResult>();
        private bool _isPhotoConfirmationVisible = false;

        public TakePhotoViewModel(
            IPermissionService permissionService,
            RemoteCameraService remoteCameraService,
            IConnectionSignalsHandler connectionSignalsHandler)
        {
            _connectionSignalsHandler = connectionSignalsHandler;
            _permissionService = permissionService;
            _remoteCameraService = remoteCameraService;
            _connectionSignalsHandler.SetInner(this);
        }

        public string SkipText => "Skip";
        public string UsePhotoText { get; set; }
        public string RetakePhotoText { get; set; }
        public bool CanSkip { get; set; }
        public readonly float CompressQuality = 1;

        public ControlOperationKind ControlMessageKind { get; set; }

        public PhotoSourceType SourceType { get; set; } = PhotoSourceType.Camera;

        public bool IsPhotoConfirmationVisible
        {
            get
            {
                return _isPhotoConfirmationVisible;
            }
            private set
            {
                _isPhotoConfirmationVisible = value;
                RaisePropertyChanged(nameof(IsPhotoConfirmationVisible));
                RaisePropertyChanged(nameof(IsCameraOverlayVisible));
                RaisePropertyChanged(nameof(IsSkipVisible));
            }
        }

        public bool IsSkipVisible => CanSkip && IsCameraOverlayVisible || true;

        public bool ConfirmPhotoAutomatically { get; private set; }

        public bool IsCameraOverlayVisible => !IsPhotoConfirmationVisible && SourceType == PhotoSourceType.Camera;

        public bool IsGallerySelectionVisible => !IsPhotoConfirmationVisible && SourceType == PhotoSourceType.Gallery;

        private bool ShouldReturnValue => _navigationViewModelType == null;

        public ICommand SkipNavigationCommand
            => _skipNavigationCommand ?? (_skipNavigationCommand = new RelayCommand(Cancel));

        public AsyncRelayCommand<MemoryStream> UsePhotoCommand
        => _usePhotoCommand ?? (_usePhotoCommand = new AsyncRelayCommand<MemoryStream>(UsePhoto));

        public ICommand BackNavigationCommand
            => _backNavigationCommand ?? (_backNavigationCommand = new RelayCommand(Cancel));

        public AsyncRelayCommand<MemoryStream> TakeOrSelectPhotoCommand
        => _takeOrSelectPhotoCommand ?? (_takeOrSelectPhotoCommand = new AsyncRelayCommand<MemoryStream>(TakeOrSelectPhoto));

        public ICommand RetakePhotoCommand
        => _retakePhotoCommand ?? (_retakePhotoCommand = new RelayCommand(RetakePhoto));

        //protected override void OnStop()
        //{
        //    // do not cancel task because additional screen can be opened to select photo from gallery
        //    // or device can be locked on camera screen
        //    return;
        //}

        #region User Actions

        private Task TakeOrSelectPhoto(MemoryStream photoStream)
        {
            if (ConfirmPhotoAutomatically)
            {
                IsPhotoConfirmationVisible = false;
                UsePhoto(photoStream);
            }
            IsPhotoConfirmationVisible = true;

            return Task.CompletedTask;
        }

        private void RetakePhoto()
        {
            IsPhotoConfirmationVisible = false;
        }

        private void Cancel()
        {
            NavigationService.NavigateTo(nameof(SessionPhotosViewModel));
        }

        private Task UsePhoto(MemoryStream photoStream)
        {
            //FinishPhotoTaking(new PhotoResult(photoStream));

            return Task.CompletedTask;
        }

        #endregion

        public async Task<bool> CheckCameraPermissionStatusAsync()
        {
            return await _permissionService.CheckPermissionStatusAsync(FeaturePermission.Camera) == PermissionStatus.Granted;
        }

        public async Task<bool> IsCameraGrantedAsync()
        {
            var isGranted = await _permissionService.RequestPermissionAsync(FeaturePermission.Camera) == PermissionStatus.Granted;
            if (!isGranted)
            {
                await DialogService.AlertAsync("Allow using camera to take photo", "Camera usage", "OK");
                Cancel();
            }

            return isGranted;
        }

        internal Task SendPartialPhotoAsync(byte[] bytes)
        {
            return _remoteCameraService.SendPartialDataMessageAsync(bytes);
        }

        public async Task<bool> IsGalleryGrantedAsync()
        {
            var isGranted = await _permissionService.RequestPermissionAsync(FeaturePermission.Photos) == PermissionStatus.Granted;
            if (!isGranted)
            {
                await DialogService.AlertAsync("Allow using media library to pick photo", "Library usage", "OK");
                Cancel();
            }

            return isGranted;
        }

        public override void GoBack(object parameter = null)
        {
            Cancel();
        }

        internal async Task SendPhotoAsync(byte[] bytes)
        {
            await _remoteCameraService.SendDataMessageAsync(
new DataMessage() { CreatedAt = DateTime.Now, Payload = bytes });
        }

        public override void OnControlMessageReceived(ControlMessage controlMessage)
        {
            ControlMessageKind = controlMessage.Kind;
            RaisePropertyChanged(nameof(ControlMessageKind));
        }

        //private void FinishPhotoTaking(PhotoResult result)
        //{
        //    _takePhotoResults.Add(result);
        //    if (ShouldReturnValue)
        //    {
        //        CompleteWithResult(_takePhotoResults);
        //    }
        //    else
        //    {
        //        _navigationParameter.Push(_takePhotoResults);
        //        NavigationService.NavigateTo(_navigationViewModelType, parameter: _navigationParameter, removeCurrentFromStack: true);

        //        ResultCompletionSource?.TrySetResult(_takePhotoResults);
        //    }
        //}
    }
}
