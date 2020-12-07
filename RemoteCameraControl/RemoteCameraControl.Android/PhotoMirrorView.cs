
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
using RemoteCameraControl.Network.DataTransfer;
using System.IO;

namespace RemoteCameraControl.Android
{
    [Activity(Label = "Remote control")]
    public class PhotoMirrorView : ActivityBase<PhotoMirrorViewModel>
    {
        private Button _photoViewButton;
        private ImageViewAsync _imageView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetTheme(Android.Resource.Style.Theme_AppCompat);

            SetContentView(Resource.Layout.photo_mirror_view);

            _photoViewButton = FindViewById<Button>(Resource.Id.take_photo_button);

            _photoViewButton.Click += _photoViewButton_Click;

            _imageView = FindViewById<ImageViewAsync>(Resource.Id.photo_image_view);


            ViewModel.PropertyChanged += ViewModel_PropertyChanged;
            
        }

        private void ViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ViewModel.DataSignal))
            {
                OnDataSignal();
            }
        }

        private void OnDataSignal()
        {
            if (ViewModel.DataSignal != null)
            {
                LoadSyncFile(ImageService.Instance, ViewModel.DataSignal).Into(_imageView);
                ViewModel.Logger.LogInfo("New photo is loaded");
            }
        }

        private async void _photoViewButton_Click(object sender, EventArgs e)
        {
            await ViewModel.TakePhotoAsync();
        }

        public static TaskParameter LoadSyncFile(IImageService imageService, DataSignal dataSignal, bool onlyCached = false)
        {
            if (dataSignal == null)
            {
                return null;
            }

            TaskParameter result;

            result = imageService.LoadStream(cancellationToken => Task.FromResult<Stream>(new MemoryStream(dataSignal.Payload)));

            return result;
        }
    }
}
