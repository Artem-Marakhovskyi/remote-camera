using System;
using Android.App;

namespace RemoteCameraControl.Android.Interaction
{
    public class LoadingIndicatorConfig : ILoadingIndicatorConfig
    {
        public Func<Activity> GetActivity => () =>
        {
            Activity currentActivity = Plugin.CurrentActivity.CrossCurrentActivity.Current.Activity;
            return currentActivity;
        };
    }
}