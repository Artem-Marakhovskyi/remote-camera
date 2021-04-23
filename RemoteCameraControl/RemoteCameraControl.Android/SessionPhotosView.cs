
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Content.PM;
using Android.Graphics;
using Android.OS;
using Android.Provider;
using Android.Runtime;
using Android.Support.V4.Content;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Com.Bumptech.Glide;
using static Android.Provider.MediaStore;

namespace RemoteCameraControl.Android
{
    [Activity(Label = "SessionPhotosView")]
    public class SessionPhotosView : ActivityBase<SessionPhotosViewModel>
    {
        private Button _anotherSessionButton;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.session_photos_view);

            _anotherSessionButton = FindViewById<Button>(Resource.Id.start_another_session_button);
            _anotherSessionButton.Click += _anotherSessionButton_Click;
            RecyclerView gallery = FindViewById<RecyclerView>(Resource.Id.galleryGridView);

            gallery.SetAdapter(new ImageAdapter(this));
            gallery.SetLayoutManager(new LinearLayoutManager(this));
        }

        private void _anotherSessionButton_Click(object sender, EventArgs e)
        {

            var launchPackage = PackageManager.GetLaunchIntentForPackage(BaseContext.PackageName);
            launchPackage.AddFlags(ActivityFlags.ClearTask | ActivityFlags.NewTask | ActivityFlags.NoAnimation);
            Finish();
            OverridePendingTransition(0, 0);
            StartActivity(launchPackage);

            // INFO: A more elegant and faster way would be to not kill the process,
            //       but it will require preventing the execution to continue to the OnCreate()
            //       of the callers - the children of this class - so they all will need code changes 
            //       to check for IsFinishing and avoid execution
            System.Environment.Exit(0);
        }
    }

    public class ImageAdapter : RecyclerView.Adapter
    {
        private List<string> images;

        /** The context. */
        private SessionPhotosView context;

        /**
         * Instantiates a new image adapter.
         *
         * @param localContext
         *            the local context
         */
        public ImageAdapter(SessionPhotosView localContext)
        {
            context = localContext;
            images = getAllShownImagesPath(context);
        }

        public override int ItemCount => images.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var convertView = holder.ItemView;
            
            var button = convertView.FindViewById<Button>(Resource.Id.share_btn);
            button.Click -= Button_Click;
            button.Click += Button_Click;
            button.Tag = images[position];

            var picturesView = convertView.FindViewById<ImageView>(Resource.Id.imageView);
            picturesView.SetScaleType(ImageView.ScaleType.FitCenter);

            Glide.With(context).Load(images[position])
                    .Into(picturesView);
        }

        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent, int viewType)
        {
            return new RecyclerViewHolder(context.LayoutInflater.Inflate(Resource.Layout.photo_item_view, null));
        }

        private void Button_Click(object sender, EventArgs e)
        {
            Intent share = new Intent(Intent.ActionSend);
            share.SetType("image/jpeg");
            var uri =FileProvider.GetUriForFile(context, context.ApplicationContext.PackageName+".provider", new Java.IO.File((string)((Button)sender).Tag));
            ;
            share.PutExtra(Intent.ExtraStream, uri);

            var chooser = Intent.CreateChooser(share, "Share Image");
            IList<ResolveInfo> resInfoList = context.PackageManager.QueryIntentActivities(chooser, PackageInfoFlags.MatchDefaultOnly);

            foreach (ResolveInfo resolveInfo in resInfoList)
            {
                String packageName = resolveInfo.ActivityInfo.PackageName;
                context.GrantUriPermission(packageName, uri, ActivityFlags.GrantWriteUriPermission | ActivityFlags.GrantReadUriPermission);
            }



            context.StartActivity(chooser);

        }


        /**
         * Getting All Images Path.
         *
         * @param activity
         *            the activity
         * @return ArrayList with images Path
         */
        private List<String> getAllShownImagesPath(Activity activity)
        {
            int column_index_data, column_index_display_name, column_index_date_added;
            List<String> listOfAllImages = new List<String>();
            String absolutePathOfImage = null;
            var uri = MediaStore.Images.Media.ExternalContentUri;

            String[] projection = { MediaColumns.Data, MediaColumns.DisplayName, MediaColumns.DateAdded };

            var cursor = activity.ContentResolver.Query(uri, projection, null,
                    null, null);

            column_index_data = cursor.GetColumnIndexOrThrow(MediaColumns.Data);
            column_index_display_name = cursor.GetColumnIndexOrThrow(MediaColumns.DisplayName);
            column_index_date_added = cursor.GetColumnIndexOrThrow(MediaColumns.DateAdded);
            while (cursor.MoveToNext())
            {
                absolutePathOfImage = cursor.GetString(column_index_data);

                var dateAddedLong = cursor.GetLong(column_index_date_added);
                var dateAdded = DateTime.MinValue.AddYears(1970).AddSeconds(dateAddedLong);
                if (dateAdded > context.ViewModel.SessionStartTime)
                {
                    listOfAllImages.Add(absolutePathOfImage);
                }
            }
            return listOfAllImages;
        }

        public class RecyclerViewHolder : RecyclerView.ViewHolder
        {
            public RecyclerViewHolder(View itemView) : base(itemView)
            {
            }

            public RecyclerViewHolder(IntPtr javaReference, JniHandleOwnership transfer) : base(javaReference, transfer)
            {
            }
        }
    }
}
