using Android.App;
using ZXing.Mobile;

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
            MobileBarcodeScanner.Initialize(this);
            new DroidBootstrap(this).Execute();
        }
    }
}
