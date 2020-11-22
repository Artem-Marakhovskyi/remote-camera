using System.Threading.Tasks;
using Airbnb.Lottie;
using Foundation;
using RemoteCameraControl.Logger;
using RemoteCameraControl.RemoteCameraControl.Interaction;
using UIKit;

namespace RemoteCameraControl.iOS.Interaction
{
public class PlatformLoadingIndicator : IPlatformLoadingIndicator
    {
        private const int ContentViewWidth = 140;
        private const int Padding = 22;
        private const float FadeAnimationDuration = 0.2f;

        private readonly ILogger _logger;
        private readonly object _synchro = new object();

        private UIView _currentMainLoadingContainer;
        private UILabel _label;
        private LOTAnimationView _animationView;
        private bool _dialogPresented;

        public PlatformLoadingIndicator(ILogger logger)
        {
            _logger = logger;
            UIApplication.Notifications.ObserveDidBecomeActive(AppMovedForeground);
            UIApplication.Notifications.ObserveDidEnterBackground(AppMovedBackground);
        }

        public Task ShowAsync(string loadingStatus)
        {
            lock (_synchro)
            {
                if (_dialogPresented)
                {
                    UpdateDialog(loadingStatus);
                    return Task.CompletedTask;
                }

                UIView container = null;
                UIApplication.SharedApplication.InvokeOnMainThread(
                    () =>
                    {
                        container = GetIndicatorView(loadingStatus);
                        UIApplication.SharedApplication.KeyWindow.AddSubview(container);
                    });

                var task = Task.CompletedTask;
                UIApplication.SharedApplication.InvokeOnMainThread(() => task = FadeIn(container));

                _dialogPresented = true;
                return task;
            }
        }

        public Task HideAsync()
        {
            lock (_synchro)
            {
                if (_currentMainLoadingContainer == null)
                {
                    _dialogPresented = false;
                    return Task.CompletedTask;
                }

                var hideTask = Task.CompletedTask;

                UIApplication.SharedApplication.InvokeOnMainThread(() =>
                {
                    hideTask = FadeOut(_currentMainLoadingContainer);
                    _currentMainLoadingContainer.Hidden = true;
                    _currentMainLoadingContainer.RemoveFromSuperview();
                    _currentMainLoadingContainer.Dispose();

                    _currentMainLoadingContainer = null;
                });

                _dialogPresented = false;
                return hideTask;
            }
        }

        private UIView GetIndicatorView(string loadingStatus)
        {
            var container = GetCurrentMainLoadingContainer();
            container.BackgroundColor = UIColor.FromRGBA(0, 0, 0, 50);
            var spinnerContentView = new UIView { TranslatesAutoresizingMaskIntoConstraints = false };
            spinnerContentView.BackgroundColor = UIColor.White;
            spinnerContentView.Layer.CornerRadius = 10;
            AddSpinnerContentViewToParentWithConstraints(container, spinnerContentView);
            _animationView = LOTAnimationView.AnimationNamed("loader.json");
            _animationView.TranslatesAutoresizingMaskIntoConstraints = false;
            _animationView.LoopAnimation = true;
            spinnerContentView.ClipsToBounds = true;
            _label = new UILabel
            {
                Text = loadingStatus,
                TextColor = UIColor.FromRGB(57, 57, 57),
                TextAlignment = UITextAlignment.Center,
                Lines = 0,
                TranslatesAutoresizingMaskIntoConstraints = false,
            };
            _label.SizeToFit();
            AddLoaderToParentWithConstraints(spinnerContentView, _animationView, _label);
            _animationView.Play();

            return container;
        }

        private void AddSpinnerContentViewToParentWithConstraints(UIView parent, UIView spinnerContentView)
        {
            parent.AddSubview(spinnerContentView);

            var constraints = new[]
            {
                spinnerContentView.CenterXAnchor.ConstraintEqualTo(parent.CenterXAnchor),
                spinnerContentView.CenterYAnchor.ConstraintEqualTo(parent.CenterYAnchor),
                spinnerContentView.WidthAnchor.ConstraintLessThanOrEqualTo(parent.Bounds.Width - 40f),
                spinnerContentView.WidthAnchor.ConstraintGreaterThanOrEqualTo(ContentViewWidth),
                spinnerContentView.HeightAnchor.ConstraintLessThanOrEqualTo(parent.Bounds.Height - 40f),
            };

            NSLayoutConstraint.ActivateConstraints(constraints);
        }

        private void AddLoaderToParentWithConstraints(UIView parent, UIView animationView, UILabel loaderStatus)
        {
            parent.AddSubviews(animationView, loaderStatus);

            var constraints = new[]
            {
                animationView.TopAnchor.ConstraintEqualTo(parent.TopAnchor, -4f),
                animationView.CenterXAnchor.ConstraintEqualTo(parent.CenterXAnchor),
                animationView.WidthAnchor.ConstraintEqualTo(140f),
                animationView.HeightAnchor.ConstraintEqualTo(120f),

                loaderStatus.TopAnchor.ConstraintEqualTo(animationView.BottomAnchor, -Padding),
                loaderStatus.LeadingAnchor.ConstraintEqualTo(parent.LeadingAnchor, 15f),
                loaderStatus.TrailingAnchor.ConstraintEqualTo(parent.TrailingAnchor, -15f),
                loaderStatus.BottomAnchor.ConstraintEqualTo(parent.BottomAnchor, -10f),
                loaderStatus.CenterXAnchor.ConstraintEqualTo(parent.CenterXAnchor),
            };

            NSLayoutConstraint.ActivateConstraints(constraints);
        }

        private void UpdateDialog(string status)
        {
            if (_label == null)
            {
                _logger.LogError("dialog label is null");
                return;
            }

            _label.Text = status;
        }

        private UIView GetCurrentMainLoadingContainer()
        {
            if (_currentMainLoadingContainer == null || _currentMainLoadingContainer.Bounds != UIScreen.MainScreen.Bounds)
            {
                _currentMainLoadingContainer = new UIView(UIScreen.MainScreen.Bounds);
            }

            return _currentMainLoadingContainer;
        }

        private Task FadeOut(UIView view) => view.FadeAnimatedAsync(1, 0, FadeAnimationDuration);

        private Task FadeIn(UIView view) => view.FadeAnimatedAsync(0, 1, FadeAnimationDuration);

        private void AppMovedForeground(object sender, NSNotificationEventArgs args)
        {
            _animationView?.Play();
        }

        private void AppMovedBackground(object sender, NSNotificationEventArgs args)
        {
            _animationView?.Stop();
        }
    }
}