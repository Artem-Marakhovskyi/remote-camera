using Android.Content.PM;

namespace RemoteCameraControl.Android
{
    public class ActivityBase : GalaSoft.MvvmLight.Views.ActivityBase
    {
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, Permission[] grantResults)
        {
            Plugin.Permissions.PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}