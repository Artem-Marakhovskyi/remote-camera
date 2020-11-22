using System;
using Android.App;

namespace RemoteCameraControl.Android.Interaction
{
    public interface ILoadingIndicatorConfig
    {
        Func<Activity> GetActivity { get; }
    }
}