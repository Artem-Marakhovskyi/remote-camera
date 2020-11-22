using Android.App;
using Android.Content.PM;
using GalaSoft.MvvmLight.Views;
using Plugin.CurrentActivity;
using XLabs.Ioc;

namespace RemoteCameraControl.Android
{
    public class ActivityBase<T> : GalaSoft.MvvmLight.Views.ActivityBase where T: ViewModelBase
    {
        protected T ViewModel { get; private set; }
        protected INavigationService NavigationService { get; private set; }
        
        
        
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