using RemoteCameraControl.RemoteCameraControl.Interaction;

namespace RemoteCameraControl.Android.Interaction
{
    public class CancellableActionSheetFragmentFactory : ICancellableActionSheetDialogFactory
    {
        public ICancellableActionSheetDialog GetDialog()
        {
            return new CancellableActionSheetFragment();
        }
    }
}