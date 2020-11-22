using RemoteCameraControl.RemoteCameraControl.Interaction;

namespace RemoteCameraControl.iOS.Interaction
{
    public class CancellableActionSheetAlertFactory : ICancellableActionSheetDialogFactory
    {
        private CancellableActionSheetAlert _cancellableActionSheetAlert;

        public ICancellableActionSheetDialog GetDialog()
        {
            if (_cancellableActionSheetAlert == null)
            {
                _cancellableActionSheetAlert = new CancellableActionSheetAlert();
            }

            return _cancellableActionSheetAlert;
        }
    }
}