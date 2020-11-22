using System;
using System.Threading.Tasks;
using Android.App;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using Com.Airbnb.Lottie;
using RemoteCameraControl.Logger;
using RemoteCameraControl.RemoteCameraControl.Interaction;

namespace RemoteCameraControl.Android.Interaction
{
public class PlatformLoadingIndicator : IPlatformLoadingIndicator
    {
        private const int AnimationDuration = 200;

        private readonly ILoadingIndicatorConfig _loadingConfig;

        private readonly ILogger _logger;

        private Dialog _currentDialog;

        private object _synchro = new object();

        private bool _dialogPresented;

        private TextView _textView;

        public PlatformLoadingIndicator(ILoadingIndicatorConfig loadingConfig, ILogger logger)
        {
            _loadingConfig = loadingConfig;
            _logger = logger;
        }

        public Task ShowAsync(string status)
        {
            lock (_synchro)
            {
                if (_dialogPresented)
                {
                    global::Android.App.Application.SynchronizationContext.Send(_ => UpdatedDialog(status), null);
                }
                else
                {
                    global::Android.App.Application.SynchronizationContext.Send(_ => ShowDialog(status), null);
                    _dialogPresented = true;
                }

                return Task.Delay(AnimationDuration);
            }
        }

        public Task HideAsync()
        {
            lock (_synchro)
            {
                if (_currentDialog == null)
                {
                    return Task.CompletedTask;
                }

                Action closeAction
                    = () =>
                    {
                        _currentDialog.Hide();

                        try
                        {
                            _currentDialog.Dismiss();
                        }
                        catch (Java.Lang.IllegalArgumentException)
                        {
                            // regular java exception happened when dialog's window is already disposed
                        }

                        _dialogPresented = false;
                        _currentDialog = null;
                    };

                if (global::Android.App.Application.SynchronizationContext != null)
                {
                    global::Android.App.Application.SynchronizationContext.Send(_ => closeAction(), null);
                }
                else if (_currentDialog != null && _currentDialog.Window != null && _currentDialog.Window.Context != null)
                {
                    var activity = _currentDialog.Window.Context as Activity;

                    if (activity == null)
                    {
                        activity = _loadingConfig.GetActivity();
                    }

                    activity.RunOnUiThread(() => closeAction());
                }

                return Task.Delay(AnimationDuration);
            }
        }

        private void UpdatedDialog(string status)
        {
            if(_textView == null)
            {
                _logger.LogWarning("dialog textview is null");
                return;
            }

            _textView.Text = status;
        }

        private void ShowDialog(string status)
        {
            var context = _loadingConfig.GetActivity();
            if (context == null)
            {
                _logger.LogWarning("Dialog cannot be created because context is null.");
                return;
            }
            _currentDialog = new Dialog(context);

            _currentDialog.RequestWindowFeature((int)WindowFeatures.NoTitle);
            _currentDialog.Window.SetBackgroundDrawable(new global::Android.Graphics.Drawables.ColorDrawable(global::Android.Graphics.Color.Transparent));

            var inflater = LayoutInflater.FromContext(context);
            var view = inflater.Inflate(Resource.Layout.loading_indicator, null);
            var imageView = view.FindViewById<LottieAnimationView>(Resource.Id.spinner);
            _textView = view.FindViewById<TextView>(Resource.Id.status);

            _textView.Text = status;

            view.SetBackgroundResource(Resource.Drawable.roundedbg);
            var drawable = (GradientDrawable)view.Background;
            drawable.SetColor(Color.White);

            _currentDialog.SetContentView(view);

            imageView.SetAnimation("loader.json");
            imageView.Loop(true);
            imageView.PlayAnimation();
            _currentDialog.SetCancelable(false);

            _currentDialog.Show();
        }
    }
}