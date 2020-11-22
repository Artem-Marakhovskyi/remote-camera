using Acr.UserDialogs;
using Plugin.CurrentActivity;
using RemoteCameraControl.Android.Interaction;
using RemoteCameraControl.Android.RemoteCameraControl;
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
        }
    }
}