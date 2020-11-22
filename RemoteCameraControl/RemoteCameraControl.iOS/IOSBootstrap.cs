using GalaSoft.MvvmLight.Views;
using RemoteCameraControl.Android.RemoteCameraControl;
using RemoteCameraControl.Home;
using RemoteCameraControl.iOS.Interaction;
using RemoteCameraControl.RemoteCameraControl.Interaction;

namespace RemoteCameraControl.iOS
{
    public class IOSBootstrap : Bootstrap
    {
        protected override void RegisterPlatformSpecifics()
        {
            RegisterType<ICancellableActionSheetDialog, CancellableActionSheetAlert>();
            RegisterType<ICancellableActionSheetDialogFactory, CancellableActionSheetAlertFactory>();
            RegisterType<IMultipleChoiceDialog, MultipleChoiceAlert>();
            RegisterType<IPlatformLoadingIndicator, PlatformLoadingIndicator>();
            
            RegisterInstance<INavigationService>(GetConfiguredNavService());
        }
        
        
        private INavigationService GetConfiguredNavService()
        {
            var navigationService = new NavigationService();
            
            navigationService.Configure(nameof(HomeViewModel), typeof(HomeView));

            return navigationService;
        }
    }
}