using Acr.UserDialogs;
using Coins.Common.Droid.Photo;
using GalaSoft.MvvmLight.Views;
using Plugin.CurrentActivity;
using RemoteCamera.HubClient;
using RemoteCameraControl.Android.Interaction;
using RemoteCameraControl.Android.RemoteCameraControl;
using RemoteCameraControl.Android.SelectMode;
using RemoteCameraControl.Photo;
using RemoteCameraControl.RemoteCameraControl.Interaction;

namespace RemoteCameraControl.Android
{
    public class DroidBootstrap : Bootstrap
    {
        private App _context;

        public DroidBootstrap(Android.App app)
        {
            _context = app;
        }
        
        protected override void RegisterPlatformSpecifics()
        {
            UserDialogs.Init(_context);
            CrossCurrentActivity.Current.Init(_context);
            RegisterType<ICancellableActionSheetDialog, CancellableActionSheetFragment>();
            RegisterType<ICancellableActionSheetDialogFactory, CancellableActionSheetFragmentFactory>();
            RegisterType<IMultipleChoiceDialog, MultipleChoiceFragment>();
            RegisterType<IPlatformLoadingIndicator, PlatformLoadingIndicator>();
            RegisterType<ILoadingIndicatorConfig, LoadingIndicatorConfig>();

            RegisterInstance<INavigationService>(GetConfiguredNavService());
        }

        private INavigationService GetConfiguredNavService()
        {
            var navigationService = new NavigationService();
            
            navigationService.Configure(nameof(ModeSelectViewModel), typeof(ModeSelectView));
            navigationService.Configure(nameof(PhotoViewModel), typeof(PhotoView));
            navigationService.Configure(nameof(PhotoMirrorViewModel), typeof(PhotoMirrorView));
            navigationService.Configure(nameof(TakePhotoViewModel), typeof(TakePhotoView));
            navigationService.Configure(nameof(SplashViewModel), typeof(SplashView));

            return navigationService;
        }
    }
}