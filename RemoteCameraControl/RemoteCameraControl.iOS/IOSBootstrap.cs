using RemoteCameraControl.Android.RemoteCameraControl;
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
        }
    }
}