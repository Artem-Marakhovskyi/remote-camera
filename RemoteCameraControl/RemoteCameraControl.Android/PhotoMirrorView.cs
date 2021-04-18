
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using FFImageLoading.Views;
using FFImageLoading;
using RemoteCameraControl.Photo;
using FFImageLoading.Work;
using RemoteCameraControl.File;
using System.Threading.Tasks;
using GalaSoft.MvvmLight.Helpers;
using System.IO;
using Android.Graphics;

namespace RemoteCameraControl.Android
{
    [Activity(Label = "Remote control", Theme = "@style/AppTheme")]
    public class PhotoMirrorView : ActivityBase<PhotoMirrorViewModel>
    {
        private Button _photoViewButton;
        private Button _photoViewButtonFocus;
        private Button _photoViewButtonTimer;
        private ImageButton _back_button;
        private ImageViewAsync _imageView;
        private ProgressBar _progressBar;


        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.SetFlags(
                WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);
            SetContentView(Resource.Layout.photo_mirror_view);

            _photoViewButton = FindViewById<Button>(Resource.Id.take_photo_button);
            _photoViewButtonFocus = FindViewById<Button>(Resource.Id.take_photo_button_focus);
            _photoViewButtonTimer = FindViewById<Button>(Resource.Id.take_photo_button_timer);
            _progressBar = FindViewById<ProgressBar>(Resource.Id.progressBar);
            _back_button = FindViewById<ImageButton>(Resource.Id.back_button);
            _imageView = FindViewById<ImageViewAsync>(Resource.Id.photo_image_view);


            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            _back_button.Click += _back_button_Click;
            _photoViewButton.Click += _photoViewButton_Click;
            _photoViewButtonFocus.Click += _photoViewButtonFocus_Click;
            _photoViewButtonTimer.Click += _photoViewButtonTimer_Click;
        }

        private void _photoViewButtonTimer_Click(object sender, EventArgs e)
        {
            ViewModel.TimerClicked();
        }

        private void _photoViewButtonFocus_Click(object sender, EventArgs e)
        {
            ViewModel.FocusClicked();
        }

        private void _back_button_Click(object sender, EventArgs e)
        {
            ViewModel.GoBack();
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.Payload))
            {
                OnDataSignal();
            }
        }

        private async void OnDataSignal()
        {
            if (ViewModel.Payload != null)
            {
                _imageView.SetImageBitmap(await BitmapFactory.DecodeByteArrayAsync(ViewModel.Payload, 0, ViewModel.Payload.Length));

                _imageView.Rotation = 0;
                _progressBar.Visibility = ViewStates.Invisible;
                ViewModel.Logger.LogInfo($"New photo is loaded, {ViewModel.LatestPhotoTime}");
            }
        }

        private async void _photoViewButton_Click(object sender, EventArgs e)
        {
            await ViewModel.TakePhotoAsync();
        }

        public static TaskParameter LoadSyncFile(IImageService imageService, byte[] data, bool onlyCached = false)
        {
            TaskParameter result;

            result = imageService.LoadStream(cancellationToken => Task.FromResult<Stream>(new MemoryStream(data)));

            return result;
        }
    }
}
