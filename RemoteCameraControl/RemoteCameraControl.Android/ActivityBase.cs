using Android.App;
using Android.Content.PM;
using Android.Support.V7.App;
using GalaSoft.MvvmLight.Views;
using Plugin.CurrentActivity;
using XLabs.Ioc;

namespace RemoteCameraControl.Android
{
    public class ActivityBase<T> : AppCompatActivity where T: ViewModelBase
    {
        protected T ViewModel { get; private set; }
        protected INavigationService NavigationService { get; private set; }
        
        public static AppCompatActivity CurrentActivity { get; private set; }

        internal string ActivityKey { get; private set; }

        internal static string NextPageKey { get; set; }

        /// <summary>
        /// If possible, discards the current page and displays the previous page
        /// on the navigation stack.
        /// </summary>
        public static void GoBack()
        {
            if (ActivityBase.CurrentActivity == null)
                return;
            ActivityBase.CurrentActivity.OnBackPressed();
        }

        /// <summary>
        /// Overrides <see cref="M:Android.App.Activity.OnResume" />. If you override
        /// this method in your own Activities, make sure to call
        /// base.OnResume to allow the <see cref="T:GalaSoft.MvvmLight.Views.NavigationService" />
        /// to work properly.
        /// </summary>
        protected override void OnResume()
        {
            CurrentActivity = this;
            if (string.IsNullOrEmpty(this.ActivityKey))
            {
                this.ActivityKey = NextPageKey;
                NextPageKey = (string) null;
            }
            base.OnResume();
        }
        
        public ActivityBase()
        {
            ViewModel = Resolver.Resolve<T>();
            NavigationService = Resolver.Resolve<INavigationService>();
        }
        
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}