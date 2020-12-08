
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace RemoteCameraControl.Android
{
    [Activity(Label = "Remote Camera", MainLauncher = true, Theme = "@style/SplashTheme")]
    public class SplashView : ActivityBase<SplashViewModel>
    {
        protected override void OnCreate(Bundle savedInstanceState)
        {
            ActivityBase.CurrentActivity = this;
            if (string.IsNullOrEmpty(this.ActivityKey))
            {
                this.ActivityKey = NextPageKey;
                NextPageKey = (string)null;
            }

            base.OnCreate(savedInstanceState);

            ViewModel.NavigateToSelectMode();
        }
    }
}
