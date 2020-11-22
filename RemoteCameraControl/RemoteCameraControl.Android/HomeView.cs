using Android.App;
using Android.OS;
using RemoteCameraControl.Home;

namespace RemoteCameraControl.Android
{
    [Activity(Label = "View for HomeViewModel", MainLauncher = true)]
    public class HomeView : ActivityBase<HomeViewModel>
    {
        protected async override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            
            SetContentView(Resource.Layout.home_view);

                var s = await ViewModel.GetStatusAsync();
        }
        
    }
}