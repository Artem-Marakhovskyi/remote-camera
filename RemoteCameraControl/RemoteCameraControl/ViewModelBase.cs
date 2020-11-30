using GalaSoft.MvvmLight.Views;
using RemoteCameraControl.Logger;
using XLabs.Ioc;

namespace RemoteCameraControl
{
    public class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        protected ILogger Logger { get; }
        protected INavigationService NavigationService { get; }
        
        public ViewModelBase()
        {
            Logger = Resolver.Resolve<ILogger>();
            NavigationService = Resolver.Resolve<INavigationService>();
        }
    }
}