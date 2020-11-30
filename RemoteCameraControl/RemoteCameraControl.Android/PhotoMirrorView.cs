
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
using RemoteCameraControl.Photo;

namespace RemoteCameraControl.Android
{
    [Activity(Label = "Remote control")]
    public class PhotoMirrorView : ActivityBase<PhotoMirrorViewModel>
    {
        private Button _photoViewButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetTheme(Android.Resource.Style.Theme_AppCompat);

            SetContentView(Resource.Layout.photo_mirror_view);

            _photoViewButton = FindViewById<Button>(Resource.Id.take_photo_button);

            _photoViewButton.Click += _photoViewButton_Click;
        }

        private async void _photoViewButton_Click(object sender, EventArgs e)
        {
            await ViewModel.TakePhotoAsync();
        }
    }
}
