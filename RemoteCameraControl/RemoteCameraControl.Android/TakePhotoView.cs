
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using FFImageLoading;
using FFImageLoading.Views;
using GalaSoft.MvvmLight.Helpers;
using Java.IO;
using RemoteCamera.HubClient;
using RemoteCameraControl.Logger;
using RemoteCameraControl.Photo;
using XLabs.Ioc;

namespace RemoteCameraControl.Android
{
    [Activity(Label = "Camera", Theme = "@style/AppTheme")]
    public class TakePhotoView : ActivityBase<TakePhotoViewModel>
    {
        private const int PickImageId = 1000;

        private bool _working;
        private Button _skipButton;
        private ImageButton _backButton;
        private FrameLayout _cameraFrameLayout;
        private ProgressBar _progressBar;

        // Camera2 API has more capabilities but is more complex to use and not needed for current requirements of Umbella modules
#pragma warning disable 0618
        private global::Android.Hardware.Camera _camera;
        private CameraSurface _cameraSurface;

        private bool _isPhotoPicking;

        private Button _takePhotoButton;
        private Button _retakePhotoButton;
        private Button _usePhotoButton;
        private RelativeLayout _takenPhotoButtonsBar;
        private ImageViewAsync _takenPhotoImageView;
        private MemoryStream _photoStream;
        private SemaphoreSlim _takePhotoLock = new SemaphoreSlim(1, 1);

        protected ILogger Logger => XLabs.Ioc.Resolver.Resolve<ILogger>();

        protected string GalleryCacheDir => $"{CacheDir.AbsolutePath}/GalleryCache";

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.SetFlags(
                WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

            SetContentView(Resource.Layout.take_photo_view);

            FindViews();
            ApplyBindings();
            ApplyFonts();
        }

        protected override async void OnResume()
        {
            base.OnResume();
            _takePhotoButton.Visibility = ViewStates.Invisible;
            _working = true;
            if (ViewModel.IsCameraOverlayVisible)
            {
                if (await ViewModel.CheckCameraPermissionStatusAsync())
                {
                    OpenCameraWithLoading();
                    SubscribeToEvents();
                }
                else
                {
                    // activity will be paused by popup and resumed, so no need to do anything else here
                    await ViewModel.IsCameraGrantedAsync();
                }
            }

            // check if photo is not being picked to prevent opening multiple gallery activities
            else if (ViewModel.IsGallerySelectionVisible && !_isPhotoPicking && await ViewModel.IsGalleryGrantedAsync())
            {
                SetLoader(true);
                OpenGalleryWithLoading();
                SubscribeToTakenPhotoButtonsBarEvents();
            }
            else if (ViewModel.IsPhotoConfirmationVisible || _isPhotoPicking)
            {
                SubscribeToEvents();
            }

            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
        }

        private async void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.ControlMessageKind))
            {
                if (ViewModel.ControlMessageKind == ControlOperationKind.Focus)
                {
                    FocusCamera(this, EventArgs.Empty);
                }
                else if (ViewModel.ControlMessageKind == ControlOperationKind.TakePhoto)
                {
                    TakePhotoSilently(1, 0);
                }
                else if (ViewModel.ControlMessageKind == ControlOperationKind.Timer3)
                {
                    TakePhotoSilently(1, 3);
                }
                else if (ViewModel.ControlMessageKind == ControlOperationKind.Timer10)
                {
                    TakePhotoSilently(1, 10);
                }
                else if (ViewModel.ControlMessageKind == ControlOperationKind.TakePhoto3Delay1)
                {
                    TakePhotoSilently(3, 1);
                }
                else if (ViewModel.ControlMessageKind == ControlOperationKind.TakePhoto3Delay3)
                {
                    TakePhotoSilently(3, 3);
                }
            }
        }

        private async void TakePhotoSilently(int photoCount, int delayInSeconds)
        {
            var logger = Resolver.Resolve<ILogger>();
            for (var i = 0; i < photoCount; i++)
            {
                if (photoCount == 1 && delayInSeconds != 0)
                {
                    await Task.Delay(delayInSeconds * 1000);
                }

                var bytes = await _cameraSurface.TakePhotoAsync();
                _camera.StartPreview();
                logger.LogInfo($"Bytes received from camera: {bytes.Length}"); 

                await ViewModel.SendPartialPhotoAsync(bytes);

                if (photoCount != 0 && i < photoCount - 1)
                {
                    await Task.Delay(delayInSeconds * 1000);
                }
            }
        }

        protected override async void OnPause()
        {
            base.OnPause();
            _working = false;
            if (_camera != null && _cameraSurface != null)
            {
                await _takePhotoLock.WaitAsync();

                _cameraFrameLayout?.RemoveView(_cameraSurface);
                _cameraSurface.ReleaseResources();
                _cameraSurface = null;

                ReleaseCamera();
                UnsubscribeFromEvents();

                _takePhotoLock.Release();
            }
            else
            {
                UnsubscribeFromTakenPhotoButtonsBarEvents();
            }

            ViewModel.PropertyChanged -= ViewModel_PropertyChanged;
        }

        protected override void OnDestroy()
        {
            ReleaseResources();

            base.OnDestroy();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                ReleaseResources();
            }

            base.Dispose(disposing);
        }

        protected override async void OnActivityResult(int requestCode, [GeneratedEnum] Result resultCode, Intent data)
        {
            if (requestCode == PickImageId)
            {
                await HandlePickImageResponse(resultCode, data);
            }
            else
            {
                base.OnActivityResult(requestCode, resultCode, data);
            }
        }

        #region open camera/gallery

        private void OpenCamera()
        {
            if (_cameraSurface == null)
            {
                _cameraSurface = new CameraSurface(this);
                _cameraFrameLayout.AddView(_cameraSurface);
            }

            if (_camera == null)
            {
                // Camera2 API has more capabilities but is more complex to use and not needed for current requirements of Umbella modules
#pragma warning disable 0618
                _camera = global::Android.Hardware.Camera.Open();
                _cameraSurface.OpenCamera(_camera);
            }
            else
            {
                _camera.StartPreview();

            }

            _ = StartSendingPhotoAsync();
        }

        private async Task StartSendingPhotoAsync()
        {
            try
            {
                await Task.Delay(500);
                while (_working)
                {
                    await Task.Delay(500);

                    var bytes = await _cameraSurface.TakePhotoAsync();
                    _camera.StartPreview();

                    Resolver.Resolve<ILogger>().LogInfo($"Bytes received from camera: {bytes.Length}");
                    await ViewModel.SendPhotoAsync(bytes);
                }
            }
            catch (Exception ex)
            {
                Resolver.Resolve<ILogger>().LogError("Error during photo taking", ex);
            }
        }

        private void OpenCameraWithLoading() => ExecuteWithLoading(OpenCamera);

        private void OpenGalleryWithLoading() => ExecuteWithLoading(OpenGallery);

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Reliability", "CA2000:Dispose objects before losing scope", Justification = "Method shouldn't manage the lifecycle of elements")]
        private void OpenGallery()
        {
            // Define the Intent for getting images
            var intent = new Intent();
            intent.SetType("image/*");
            intent.SetAction(Intent.ActionGetContent);

            StartActivityForResult(Intent.CreateChooser(intent, "Select Photo"), PickImageId);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Exception is logged")]
        private void ExecuteWithLoading(Action action)
        {
            try
            {
                SetLoader(true);
                action();
            }
            catch (Exception e)
            {
                Logger.LogError("Error occurred while executing action", e);
            }
            finally
            {
                SetLoader(false);
            }
        }

        #endregion

        #region Actions

        private async void TakePhoto(object sender, EventArgs ea)
        {
            // unsubscribe to prevent double-tap and case when camera preview hasn't started yet
            _takePhotoButton.Click -= TakePhoto;

            _takePhotoButton.Visibility = ViewStates.Invisible;
            _skipButton.Visibility = ViewStates.Invisible;
            _takenPhotoImageView.Visibility = ViewStates.Visible;

            SetLoader(true);

            await _takePhotoLock.WaitAsync();
            var bytes = await _cameraSurface.TakePhotoAsync();
            _takePhotoLock.Release();

            await PresentPhotoAsync(bytes);

            SetLoader(false);

            ViewModel.TakeOrSelectPhotoCommand.Execute(_photoStream);
        }

        private void RetakePhoto(object sender, EventArgs ea)
        {
            ViewModel.RetakePhotoCommand.Execute(null);
            _takenPhotoImageView.SetImageDrawable(null);
            _photoStream?.Dispose();

            if (ViewModel.IsCameraOverlayVisible)
            {
                OpenCameraWithLoading();

                // re-subscribe to enable _takePhotoButton
                // and handle case when app goes from bg at confirmation screen
                _takePhotoButton.Click -= TakePhoto;
                _takePhotoButton.Click += TakePhoto;
            }

            if (ViewModel.IsGallerySelectionVisible)
            {
                OpenGalleryWithLoading();
            }
        }

        private void UsePhoto(object sender, EventArgs ea)
        {
            if (_photoStream != null)
            {
                ViewModel.UsePhotoCommand.Execute(_photoStream);
            }

            ReleaseResources();
        }

        private void Skip(object sender, EventArgs ea)
        {
            ViewModel.SkipNavigationCommand.Execute(null);
            ReleaseResources();
        }

        private void Back(object sender, EventArgs ea)
        {
            ViewModel.BackNavigationCommand.Execute(null);
            ReleaseResources();
        }

        private void FocusCamera(object sender, EventArgs e)
        {
            if (ViewModel.IsCameraOverlayVisible)
            {
                //_camera.AutoFocus(null);
            }
        }

        #endregion

        #region photo presenting

        private Task PresentPhotoAsync(byte[] bytes)
        {
            Glide.With(this).AsBitmap().Load(bytes).Into(_takenPhotoImageView);

            _photoStream?.Dispose();

            _photoStream = new MemoryStream(bytes);

            return Task.CompletedTask;
        }

        private async Task PresentPhotoAsync(global::Android.Net.Uri uri)
        {
            Glide.With(this).AsBitmap().Load(uri).Into(_takenPhotoImageView);

            var stream = await uri.RotatePhotoAsync(
                ContentResolver,
                Resources.DisplayMetrics.HeightPixels,
                Resources.DisplayMetrics.WidthPixels);

            _photoStream?.Dispose();
            stream.Position = 0;

            _photoStream = stream;
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Exception is logged")]
        private async Task PresentPhotoAsync(MemoryStream stream, Bitmap bitmap)
        {
            _photoStream?.Dispose();
            stream.Position = 0;
            _photoStream = stream;

            try
            {
                if (bitmap == null)
                {
                    await ImageService.Instance
                        .LoadStream((cancellationToken) => Task.FromResult<Stream>(_photoStream))
                        .WithCache(FFImageLoading.Cache.CacheType.None)
                        .IntoAsync(_takenPhotoImageView);
                }
                else
                {
                    _takenPhotoImageView.SetImageBitmap(bitmap);
                }
            }
            catch (Exception e)
            {
                Logger.LogWarning("Failed loading image", e);
            }
        }

        #endregion

        #region init view methods

        private void ApplyFonts()
        {
        }

        private void FindViews()
        {
            _skipButton = FindViewById<Button>(Resource.Id.skip_button);
            _takePhotoButton = FindViewById<Button>(Resource.Id.take_photo_button);
            _backButton = FindViewById<ImageButton>(Resource.Id.back_button);
            _cameraFrameLayout = FindViewById<FrameLayout>(Resource.Id.camera_frame_layout);
            _progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            _takenPhotoImageView = FindViewById<ImageViewAsync>(Resource.Id.taken_photo_image_view);
            _takenPhotoButtonsBar = FindViewById<RelativeLayout>(Resource.Id.taken_photo_buttons_bar);
            _retakePhotoButton = FindViewById<Button>(Resource.Id.retake_photo_button);
            _usePhotoButton = FindViewById<Button>(Resource.Id.use_photo_button);

            _takePhotoButton.Visibility = ViewStates.Gone;
        }

        private void ApplyBindings()
        {
            this.SetBinding(
                () => ViewModel.SkipText,
                () => _skipButton.Text,
                BindingMode.OneTime);

            this.SetBinding(
                    () => ViewModel.IsSkipVisible,
                    () => _skipButton.Visibility)
                    .ConvertSourceToTarget(ConvertationSource.ConvertVisibility);

            this.SetBinding(
                () => ViewModel.UsePhotoText,
                () => _usePhotoButton.Text);

            this.SetBinding(
                () => ViewModel.RetakePhotoText,
                () => _retakePhotoButton.Text);

            this.SetBinding(
                    () => ViewModel.IsPhotoConfirmationVisible,
                    () => _takenPhotoImageView.Visibility)
                    .ConvertSourceToTarget(ConvertationSource.ConvertVisibility);

            this.SetBinding(
                    () => ViewModel.IsPhotoConfirmationVisible,
                    () => _takenPhotoButtonsBar.Visibility)
                    .ConvertSourceToTarget(ConvertationSource.ConvertVisibility);

            this.SetBinding(
                    () => ViewModel.IsCameraOverlayVisible,
                    () => _takePhotoButton.Visibility)
                    .ConvertSourceToTarget(ConvertationSource.ConvertVisibility);

            this.SetBinding(
                    () => ViewModel.IsCameraOverlayVisible,
                    () => _backButton.Visibility)
                    .ConvertSourceToTarget(ConvertationSource.ConvertVisibility);

        }

        private void SetLoader(bool show)
        {
            _progressBar.Visibility = show ? ViewStates.Visible : ViewStates.Invisible;
        }

        #endregion

        #region resource management

        private void ReleaseResources()
        {
            if (_camera != null)
            {
                ReleaseCamera();
            }

            _photoStream?.Dispose();

            UnsubscribeFromEvents();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Exception is logged")]
        private void ReleaseCamera()
        {
            try
            {
                _camera.StopPreview();
                _camera.Release();
                _camera.Dispose();
                _camera = null;
            }
            catch (Exception ex)
            {
                Logger.LogWarning("Failed to release camera, probably it is already released", ex);
            }
        }

        private void UnsubscribeFromEvents()
        {
            UnsubscribeFromTakenPhotoButtonsBarEvents();

            _takePhotoButton.Click -= TakePhoto;
            _skipButton.Click -= Skip;
            _backButton.Click -= Back;
            _cameraFrameLayout.Click -= FocusCamera;
        }

        private void UnsubscribeFromTakenPhotoButtonsBarEvents()
        {
            _retakePhotoButton.Click -= RetakePhoto;
            _usePhotoButton.Click -= UsePhoto;
        }

        private void SubscribeToEvents()
        {
            SubscribeToTakenPhotoButtonsBarEvents();

            _takePhotoButton.Click += TakePhoto;
            _skipButton.Click += Skip;
            _backButton.Click += Back;
            _cameraFrameLayout.Click += FocusCamera;
        }

        private void SubscribeToTakenPhotoButtonsBarEvents()
        {
            _retakePhotoButton.Click += RetakePhoto;
            _usePhotoButton.Click += UsePhoto;
        }

        #endregion

        #region process photo from gallery

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "Exception is logged")]
        private async Task HandlePickImageResponse([GeneratedEnum] Result resultCode, Intent data)
        {
            _isPhotoPicking = true;
            SetLoader(true);
            try
            {
                if ((resultCode == Result.Ok) && (data != null))
                {
                    var uri = data.Data;
                    await ProcessPickedPhotoAsync(uri);
                }
                else
                {
                    Back(null, null);
                }
            }
            catch (Exception e)
            {
                Logger.LogError("An error occurred while processing photo from gallery", e);
            }
            finally
            {
                SetLoader(false);
                _isPhotoPicking = false;
            }
        }

        private async Task ProcessPickedPhotoAsync(global::Android.Net.Uri uri)
        {
            await PresentPhotoAsync(uri);
            ViewModel.TakeOrSelectPhotoCommand.Execute(_photoStream);
        }

        #endregion
    }

    public static class ConvertationSource
    {
        public static ViewStates ConvertVisibility(bool visible)
        {
            return visible
                ? ViewStates.Visible
                : ViewStates.Gone;
        }

        public static ViewStates ConvertVisibilityInverse(bool visible)
        {
            return ConvertVisibility(!visible);
        }
    }
}
