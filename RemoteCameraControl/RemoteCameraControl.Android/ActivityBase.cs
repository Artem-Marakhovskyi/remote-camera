using Android.App;
using Android.Content.PM;
using Android.Support.V7.App;
using GalaSoft.MvvmLight.Views;
using Plugin.CurrentActivity;
using XLabs.Ioc;

namespace RemoteCameraControl.Android
{
    public class ActivityBase<T> : ActivityBase where T: ViewModelBase
    {
        protected T ViewModel { get; private set; }
        protected INavigationService NavigationService { get; private set; }
        
        /// <summary>
        /// If possible, discards the current page and displays the previous page
        /// on the navigation stack.
        /// </summary>


        /// <summary>
        /// Overrides <see cref="M:Android.App.Activity.OnResume" />. If you override
        /// this method in your own Activities, make sure to call
        /// base.OnResume to allow the <see cref="T:GalaSoft.MvvmLight.Views.NavigationService" />
        /// to work properly.
        /// </summary>
        protected override void OnResume()
        {
            ActivityBase.CurrentActivity = this;
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

    public class ActivityBase : AppCompatActivity
    {
        public static ActivityBase CurrentActivity { get; protected set; }


        public string ActivityKey { get; protected set; }

        public static string NextPageKey { get; set; }
        public static void GoBack()
        {
            if (ActivityBase.CurrentActivity == null)
                return;
            ActivityBase.CurrentActivity.OnBackPressed();
        }

    }

}