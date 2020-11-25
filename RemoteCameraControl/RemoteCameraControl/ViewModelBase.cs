using RemoteCameraControl.Logger;
using XLabs.Ioc;

namespace RemoteCameraControl
{
    public class ViewModelBase : GalaSoft.MvvmLight.ViewModelBase
    {
        protected ILogger Logger { get; }
        
        public ViewModelBase()
        {
            Logger = Resolver.Resolve<ILogger>();
        }
    }
}