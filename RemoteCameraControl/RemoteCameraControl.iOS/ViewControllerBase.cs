using System;
using UIKit;

namespace RemoteCameraControl.iOS
{
    public abstract class ViewControllerBase : UIViewController
    {
        public ViewControllerBase(IntPtr handle) : base(handle)
        {
        }
    }
}