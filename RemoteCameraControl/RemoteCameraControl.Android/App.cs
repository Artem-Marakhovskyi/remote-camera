using System.Net;
using System.Net.Sockets;
using Android.App;

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
            
            new DroidBootstrap(this).Execute();
        }
    }
}
