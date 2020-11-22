namespace RemoteCameraControl.RemoteCameraControl.Interaction
{
    public interface ICancellableActionSheetDialogFactory
    {
        /// <summary>
        /// Method returns instance of <see cref="ICancellableActionSheetDialog"/>
        /// which can be shown on the screen.
        /// Please note: implementation of Android and iOS factories are different.
        /// Android returns new instance, iOS caches the dialog.
        /// </summary>
        /// <returns></returns>
        ICancellableActionSheetDialog GetDialog();
    }
}