using UIKit;

namespace RemoteCameraControl.iOS
{
    public static class ViewControllerExtensions
    {
        public static UIViewController GetTopViewController(this UIViewController uIViewController)
        {
            while (uIViewController.PresentedViewController != null)
            {
                uIViewController = uIViewController.PresentedViewController;
            }

            return uIViewController;
        }
    }
}