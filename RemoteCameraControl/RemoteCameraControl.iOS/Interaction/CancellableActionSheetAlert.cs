using System.Threading.Tasks;
using RemoteCameraControl.RemoteCameraControl.Interaction;
using UIKit;

namespace RemoteCameraControl.iOS.Interaction
{
    public class CancellableActionSheetAlert :
        ICancellableActionSheetDialog,
        ICancellableActionSheetDialogFactory
    {


        public ICancellableActionSheetDialog GetDialog()
        {
            return new CancellableActionSheetAlert();
        }

        public Task<string> ShowCancellableActionSheetAsync(string title, string cancel, params string[] buttons)
        {
            var taskCompletionSource = new TaskCompletionSource<string>();

            var alertController = UIAlertController.Create(title, null, UIAlertControllerStyle.ActionSheet);

            foreach (var button in buttons)
            {
                alertController.AddAction(UIAlertAction.Create(button, UIAlertActionStyle.Default, x => { taskCompletionSource.TrySetResult(button); }));
            }

            alertController.AddAction(UIAlertAction.Create(cancel, UIAlertActionStyle.Cancel, x => { taskCompletionSource.TrySetResult(cancel); }));

            var topViewController = UIApplication.SharedApplication.KeyWindow.RootViewController.GetTopViewController();

            topViewController.PresentViewController(alertController, true, null);

            return taskCompletionSource.Task;
        }
    }
}