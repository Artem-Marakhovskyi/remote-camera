using Android.App;
using Android.OS;

namespace RemoteCameraControl.Android
{
    [Activity(Label = "View for HomeViewModel", MainLauncher = true)]
    public class HomeView : ActivityBase
    {
        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            SetContentView(Resource.Layout.home_view);
        }
        
    }
}