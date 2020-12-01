using System;
using System.Threading.Tasks;
using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Views;
using Android.Widget;
using FFImageLoading;
using FFImageLoading.Cache;
using FFImageLoading.Views;
using FFImageLoading.Work;
using RemoteCameraControl.Android;
using RemoteCameraControl.File;
using RemoteCameraControl.Photo.View;

namespace Coins.Common.Droid.Photo
{
    [Activity(Label = "ViewPhoto", ScreenOrientation = ScreenOrientation.Portrait, LaunchMode = LaunchMode.SingleTop)]
    public class PhotoView : ActivityBase<ViewPhotoViewModel>
    {
        ImageButton _backButton;
        ImageViewAsync _imageView;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            RequestWindowFeature(WindowFeatures.NoTitle);
            Window.SetFlags(WindowManagerFlags.Fullscreen, WindowManagerFlags.Fullscreen);

            SetContentView(Resource.Layout.photo_view);

            _backButton = FindViewById<ImageButton>(Resource.Id.back_button);
            _backButton.Click += Back;

            _imageView = FindViewById<ImageViewAsync>(Resource.Id.photo_image_view);

            LoadSyncFile(ImageService.Instance, ViewModel?.Photo).Into(_imageView);
        }

        private void Back(object sender, EventArgs ea)
        {
            ViewModel.CloseCommand.Execute(null);
            _backButton.Click -= Back;
            _imageView.Dispose();
        }

        public static TaskParameter LoadSyncFile(IImageService imageService, IRelatedFile file, bool onlyCached = false)
        {
            TaskParameter result;

                result = imageService.LoadStream(cancellationToken => Task.FromResult(file.ContentStream));

                if (!string.IsNullOrEmpty(file.Id))
                {
                    result = result.WithCache(CacheType.Memory).CacheKey($"SyncFile_{file.Id}");
                }

            return result;
        }
    }
}
