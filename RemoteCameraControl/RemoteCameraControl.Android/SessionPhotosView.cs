
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
    [Activity]
    public class SessionPhotosView : ActivityBase<SessionPhotosViewModel>, IMenuItemOnMenuItemClickListener
    {
        private Button _anotherSessionButton;
        private global::Android.Support.V7.Widget.Toolbar _toolbar;
        private IMenuItem _menuItem;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.session_photos_view);

            RecyclerView gallery = FindViewById<RecyclerView>(Resource.Id.galleryGridView);
            _toolbar = FindViewById<global::Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);

            gallery.SetAdapter(new ImageAdapter(this, ViewModel.NavigateToPreview));
            gallery.SetLayoutManager(new GridLayoutManager(this, 2));

            SetSupportActionBar(_toolbar);
            Title = "Share last session photos";


        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.toolbar_menu_session, menu);

            _menuItem = menu.FindItem(Resource.Id.miNew);
            _menuItem.SetOnMenuItemClickListener(this);

            return true;
        }

        public bool OnMenuItemClick(IMenuItem item)
        {
            if (_menuItem == item)
            {
                ViewModel.ToRoot();

                return true;
            }

            return false;
        }
    }

    public class ImageAdapter : RecyclerView.Adapter
    {
        private List<string> images;
        private Action<string> _onPictureClick;
        private Dictionary<string, Action<string>> _actionsStorage = new Dictionary<string, Action<string>>();
        /** The context. */
        private SessionPhotosView context;

        /**
         * Instantiates a new image adapter.
         *
         * @param localContext
         *            the local context
         */
        public ImageAdapter(SessionPhotosView localContext, Action<string> onPictureClick)
        {
            context = localContext;
            images = getAllShownImagesPath(context);
            _onPictureClick = onPictureClick;
        }

        public override int ItemCount => images.Count;

        public override long GetItemId(int position)
        {
            return position;
        }

        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position)
        {
            var convertView = holder.ItemView;
            

            var picturesView = convertView.FindViewById<ImageView>(Resource.Id.imageView);
            picturesView.TooltipText = images[position];
            picturesView.Click -= PicturesView_Click;
            picturesView.Click += PicturesView_Click;

            picturesView.SetScaleType(ImageView.ScaleType.CenterCrop);

            Glide.With(context).Load(images[position])
                    .Into(picturesView);
        }

        private void PicturesView_Click(object sender, EventArgs e)
        {
            View s = (View)sender;

            GalleryImageView.Path = s.TooltipText;
            _onPictureClick(s.TooltipText);
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
