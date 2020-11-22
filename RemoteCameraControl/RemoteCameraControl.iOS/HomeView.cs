using System;
using RemoteCameraControl.Home;

namespace RemoteCameraControl.iOS
{
    public partial class HomeView : ViewControllerBase<HomeViewModel>
    {
        public HomeView(IntPtr handle) : base(handle)
        {
        }

        public async override void ViewDidLoad()
        {
            base.ViewDidLoad();

            var r = await ViewModel.GetStatusAsync();
        }
    }
}