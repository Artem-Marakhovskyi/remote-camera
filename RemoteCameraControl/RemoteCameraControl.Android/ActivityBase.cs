using Android.Content.PM;
using Android.Runtime;
using Android.Support.V7.App;
using Android.Views;
using GalaSoft.MvvmLight.Views;
using XLabs.Ioc;

namespace RemoteCameraControl.Android
{
    public class ActivityBase<T> : ActivityBase where T: ViewModelBase
    {
        protected T ViewModel { get; private set; }
        protected INavigationService NavigationService { get; private set; }
        
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

            ViewModel.Resume();
            ViewModel.Resumed();
        }

        protected override void OnStart()
        {
            base.OnStart();

            ViewModel.Start();
        }

        protected override void OnPause()
        {
            base.OnPause();

            ViewModel.Pause();
        }

        protected override void OnStop()
        {
            ViewModel.Stop();

            base.OnStop();
        }

        protected override void OnDestroy()
        {
            ViewModel.Dispose();

            base.OnDestroy();
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

    public class ActivityBase : AppCompatActivity, View.IOnClickListener
    {
        public static ActivityBase CurrentActivity { get; protected set; }


        public string ActivityKey { get; protected set; }

        public static string NextPageKey { get; set; }
        public bool IsBackAlreadyClicked { get; private set; }

        public static void GoBack()
        {
            if (ActivityBase.CurrentActivity == null)
                return;
            ActivityBase.CurrentActivity.OnBackPressed();
        }

        // can't override 'OnBackPressed' because it's called dawn in methods chain invocation
        public override bool OnKeyDown([GeneratedEnum] global::Android.Views.Keycode keyCode, KeyEvent e)
        {
            if (keyCode == global::Android.Views.Keycode.Back)
            {
                OnClick(null);
                return true;
            }

            return base.OnKeyDown(keyCode, e);
        }

        public virtual void OnClick(View v)
        {
            if (!IsBackAlreadyClicked)
            {
                IsBackAlreadyClicked = true;
                GoBack();
            }
        }

    }

}