using System;
using RemoteCameraControl.Android;
using RemoteCameraControl.Android.SelectMode;

namespace RemoteCameraControl
{
    public class SessionPhotosViewModel : ViewModelBase
    {
        public string SessionName { get; set; }
        public DateTime SessionStartTime { get; set; } = DateTime.MinValue;

        public SessionPhotosViewModel(
            IAppContext appContext)
        {
            SessionName = appContext.SessionName;
            SessionStartTime = appContext.SessionStart;
        }

        public void NavigateToPreview(string absolutePath)
        {
            NavigationService.NavigateTo(nameof(GalleryImageViewModel), absolutePath);
        }

        internal void ToRoot()
        {
            NavigationService.NavigateTo(nameof(ModeSelectViewModel));
        }
    }
}
