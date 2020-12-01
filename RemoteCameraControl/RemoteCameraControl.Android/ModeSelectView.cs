
using Android.App;
using Android.OS;
using Android.Widget;
using RemoteCameraControl.Android.SelectMode;
using RemoteCameraControl.Network;

namespace RemoteCameraControl.Android
{
    [Activity(Label = "ModeSelectView", MainLauncher = true)]
    public class ModeSelectView : ActivityBase<ModeSelectViewModel>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetTheme(Resource.Style.Theme_AppCompat);

            SetContentView(Resource.Layout.mode_select_view);

            var cameraButton = FindViewById<Button>(Resource.Id.be_camera_button);
            cameraButton.Click += CameraButton_Click;

            var rcButton = FindViewById<Button>(Resource.Id.be_rc_button);
            rcButton.Click += RcButton_Click;
        }

        private void RcButton_Click(object sender, System.EventArgs e)
        {
            ViewModel.BecomeRc();
        }

        private void CameraButton_Click(object sender, System.EventArgs e)
        {
            ViewModel.BecomeCamera();
        }
    }
}
