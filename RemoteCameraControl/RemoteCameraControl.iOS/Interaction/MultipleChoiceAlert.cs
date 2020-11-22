using System.Threading.Tasks;
using RemoteCameraControl.RemoteCameraControl.Interaction;
using UIKit;

namespace RemoteCameraControl.iOS.Interaction
{
    public class MultipleChoiceAlert : IMultipleChoiceDialog
    {
        public Task<bool?> ShowAsync(string message, string title, params string[] buttons)
        {
            var taskCompletionSource = new TaskCompletionSource<bool?>();
            var alertController = UIAlertController.Create(title, message, UIAlertControllerStyle.Alert);

            if (buttons.Length > 0)
            {
                var action = UIAlertAction.Create(buttons[0], UIAlertActionStyle.Default,
                    (e) => { taskCompletionSource.TrySetResult(true); });
                alertController.AddAction(action);
            }

            if (buttons.Length > 1)
            {
                var action = UIAlertAction.Create(buttons[1], UIAlertActionStyle.Default,
                    (e) => { taskCompletionSource.TrySetResult(false); });
                alertController.AddAction(action);
            }

            if (buttons.Length > 2)
            {
                var action = UIAlertAction.Create(buttons[2], UIAlertActionStyle.Default,
                    (e) => { taskCompletionSource.TrySetResult(null); });
                alertController.AddAction(action);
            }

            var topViewController = UIApplication.SharedApplication.KeyWindow.RootViewController.GetTopViewController();

            topViewController.PresentViewController(alertController, true, null);

            return taskCompletionSource.Task;
        }
    }
}