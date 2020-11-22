using System;
using GalaSoft.MvvmLight.Views;
using UIKit;
using XLabs.Ioc;

namespace RemoteCameraControl.iOS
{
    public abstract class ViewControllerBase<T> : UIViewController where T : ViewModelBase
    {
        protected T ViewModel { get; private set; }
        protected INavigationService NavigationService { get; private set; }
        
        public ViewControllerBase(IntPtr handle) : base(handle)
        {
            ViewModel = Resolver.Resolve<T>();
            NavigationService = Resolver.Resolve<INavigationService>();
        }
    }
}