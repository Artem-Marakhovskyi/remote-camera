using Acr.UserDialogs;
using Android.App;
using Plugin.CurrentActivity;

namespace RemoteCameraControl.Android
{
    [Application(Debuggable = true)]
    public class App : Application
    {
        public App(System.IntPtr handle, global::Android.Runtime.JniHandleOwnership jni) : base(handle, jni)
        {
                
        }
        
        
        public override void OnCreate()
        {
            base.OnCreate();
            
            UserDialogs.Init(this);
            CrossCurrentActivity.Current.Init(this);
        }
    }
}