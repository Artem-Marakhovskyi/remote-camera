
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;

namespace RemoteCameraControl.Android
{
    [Activity]
    public class GalleryImageView : ActivityBase<GalleryImageViewModel>, IMenuItemOnMenuItemClickListener
    {
        private global::Android.Support.V7.Widget.Toolbar _toolbar;
        private ImageView _image;
        private IMenuItem _menuItem;
            
        public static string Path { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.gallery_image_view);

            _toolbar = FindViewById<global::Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            _image = FindViewById<ImageView>(Resource.Id.main_image);

            _image.SetScaleType(ImageView.ScaleType.CenterCrop);

            Glide.With(this).Load(GalleryImageView.Path)
                    .Into(_image);

            SetSupportActionBar(_toolbar);
            SupportActionBar.SetDisplayHomeAsUpEnabled(true);
            SupportActionBar.SetDisplayShowHomeEnabled(true);
            _toolbar.SetNavigationOnClickListener(this);
            Title = "Share the photo";
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu_gallery, menu);

            _menuItem = menu.FindItem(Resource.Id.miShare);
            _menuItem.SetOnMenuItemClickListener(this);

            return true;
        }

        public override void OnClick(View v)
        {
            base.OnClick(v);

            this.Finish();
        }

        public void Send(string absolutePath)
        {
            Intent share = new Intent(Intent.ActionSend);
            share.SetType("image/jpeg");
            var uri = FileProvider.GetUriForFile(this, this.ApplicationContext.PackageName + ".provider", new Java.IO.File(absolutePath));
            ;
            share.PutExtra(Intent.ExtraStream, uri);

            var chooser = Intent.CreateChooser(share, "Share Image");
            IList<ResolveInfo> resInfoList = this.PackageManager.QueryIntentActivities(chooser, PackageInfoFlags.MatchDefaultOnly);

            foreach (ResolveInfo resolveInfo in resInfoList)
            {
                String packageName = resolveInfo.ActivityInfo.PackageName;
                this.GrantUriPermission(packageName, uri, ActivityFlags.GrantWriteUriPermission | ActivityFlags.GrantReadUriPermission);
            }

            this.StartActivity(chooser);
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            if (item == _menuItem)
            {
                Send(Path);

                return true;
            }

            return false;
        }
    }
}
