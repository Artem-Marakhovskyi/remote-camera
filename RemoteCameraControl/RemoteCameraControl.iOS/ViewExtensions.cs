using System;
using System.Threading.Tasks;
using CoreAnimation;
using CoreGraphics;
using Foundation;
using UIKit;

namespace RemoteCameraControl.iOS
{
public static class ViewExtensions
    {
        public static void AddShadowInside(this UIView view, float yOffset = 0, float opacity = 0.7f)
        {
            view.BackgroundColor = UIColor.Clear;
            view.Layer.ShadowColor = new CGColor(0, 0, 0);
            view.Layer.ShadowOffset = new CGSize(0, yOffset);
            view.Layer.ShadowRadius = 1.5f;
            view.Layer.ShadowOpacity = opacity;
            view.Layer.MasksToBounds = false;
            view.Layer.ShouldRasterize = true;
            view.Layer.RasterizationScale = UIScreen.MainScreen.Scale;
        }

        /// <summary>
        /// Creates a shadow.
        /// </summary>
        /// <param name="me">Target UIView to create shadow for.</param>
        /// <param name="radius">Radius of a shadow.</param>
        /// <param name="opacity">Opacity of a shadow (alpha of the colour).</param>
        /// <param name="offsetX">Offset by X axis.</param>
        /// <param name="offsetY">Offset by Y axis.</param>
        public static void AddShadowOutside(this UIView me, float radius = 3f, float opacity = 0.2f, float offsetX = 0, float offsetY = 0)
        {
            me.ClipsToBounds = true;
            me.Layer.ShadowColor = new CGColor(0, 0, 0, opacity);
            me.Layer.ShadowOpacity = 1.0f;
            me.Layer.ShadowOffset = new CGSize(offsetX, offsetY);
            me.Layer.ShadowRadius = radius;
            me.Layer.MasksToBounds = false;
        }

        public static void AddShadowAndBorder(this UIView view, bool roundedCorners)
        {
            view.ClipsToBounds = true;
            view.Layer.ShadowColor = UIColor.FromRGB(0, 0, 0).CGColor;
            view.Layer.BorderColor = UIColor.FromRGB(235, 235, 235).CGColor;
            view.Layer.BorderWidth = 1;
            view.Layer.CornerRadius = roundedCorners ? 4 : 0;
            view.Layer.MasksToBounds = false;
            view.Layer.ShadowOpacity = 0.1f;
            view.Layer.ShadowRadius = 4.0f;
            view.Layer.ShadowOffset = new CGSize(0.5, 1.0);
        }

        public static void Cornerize(this UIView view, nfloat radius)
        {
            view.Layer.CornerRadius = radius;
            view.Layer.MasksToBounds = true;
        }

        public static void AdjustForExtendedScreen(this UIView me, NSLayoutConstraint widthConstraint, int widthMargins = 32)
        {
            me.Layer.CornerRadius = 6f;
            widthConstraint.Constant -= 32f;
        }

        public static Task FadeAnimatedAsync(this UIView view, nfloat from, nfloat to, nfloat duration)
        {
            return UIView.AnimateAsync(
                duration,
                () =>
                {
                    view.Alpha = from;
                    view.Alpha = to;
                });
        }

        public static bool IsTransparent(this UIView view)
        {
            return view.Alpha < nfloat.Epsilon;
        }

        public static void RotateInfinite(this UIView view, float fullCircleDuration)
        {
            var rotationAnimation = new CABasicAnimation();
            rotationAnimation.KeyPath = "transform.rotation";
            rotationAnimation.From = new NSNumber(0);
            rotationAnimation.To = new NSNumber(Math.PI * 2);
            rotationAnimation.Duration = fullCircleDuration;
            rotationAnimation.RepeatCount = float.PositiveInfinity;
            view.Layer.AddAnimation(rotationAnimation, key: null);
        }

        public static NSLayoutConstraint SetHeight(this UIView view, nfloat height)
        {
            var constraint = NSLayoutConstraint.Create(
                view, NSLayoutAttribute.Height, NSLayoutRelation.Equal, 1, height);

            view.AddConstraint(constraint);

            constraint.Active = true;

            return constraint;
        }

        public static UIView FindFirstResponder(this UIView view)
        {
            if (view.IsFirstResponder)
            {
                return view;
            }

            foreach (UIView subView in view.Subviews)
            {
                var firstResponder = subView.FindFirstResponder();
                if (firstResponder != null)
                {
                    return firstResponder;
                }
            }

            return null;
        }
    }
}