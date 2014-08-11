using System;
using System.Linq;
using MonoTouch.UIKit;
using System.Drawing;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using BlackDragon.Core;
using BlackDragon.Core.Entities;
using MonoTouch.Foundation;

namespace BlackDragon.Fx.Extensions
{
	public enum Orientation
	{
		Portrait,
		Landscape
	}

	public static class UIViewExtensions
	{
		public static bool IsUpsideDown(this UIView view)
		{
			var orientation = UIApplication.SharedApplication.StatusBarOrientation;
			return orientation == UIInterfaceOrientation.PortraitUpsideDown || orientation == UIInterfaceOrientation.LandscapeLeft; 
		}

		public static bool IsPortrait(this UIView view)
		{
			return IsPortrait(view, UIApplication.SharedApplication.StatusBarOrientation);
		}

		public static bool IsPortrait(this UIView view, UIInterfaceOrientation orientation)
		{
			return (orientation == UIInterfaceOrientation.Portrait || orientation == UIInterfaceOrientation.PortraitUpsideDown);
		}

		public static bool IsLandscape(this UIView view)
		{
			return !view.IsPortrait();
		}

		public static bool IsLandscape(this UIView view, UIInterfaceOrientation orientation)
		{
			return !view.IsPortrait(orientation);
		}

		public static T Descendant<T>(this UIView parentView, int? tag = null) where T : UIView
		{
			foreach (var subView in parentView.Subviews)
			{
				var found = _Descendant<T>(subView, tag);
				if (found != null)
					return found;
			}
			
			return null;
		}

		private static T _Descendant<T>(UIView parentView, int? tag = null) where T : UIView
		{
			if (parentView is T && (tag.HasValue ? parentView.Tag == tag : true))
				return parentView as T;

			foreach (var subView in parentView.Subviews)
			{
				var found = _Descendant<T>(subView, tag);
				if (found != null)
					return found;
			}

			return null;
		}

        public static T Ancestor<T>(this UIView childView, int? tag = null) where T : UIView
		{
            if (childView is T && (tag.HasValue ? childView.Tag == tag : true))
				return childView as T;

			if (childView.Superview != null)
				return Ancestor<T>(childView.Superview);

			return null;
		}

		public static T Child<T>(this UIView view, int? tag = null) where T : UIView
        {
            if (view != null)
            {
				var childView = view.Subviews.FirstOrDefault(x => x is T && (tag.HasValue ? x.Tag == tag : true)) as T;
                return childView;
            }

            return null;
        }

		public static bool IsThisOrSuperviewOf(this UIView view, UIView subview)
		{
			if (view == subview || view.Subviews.Contains(subview))
				return true;
			else
			{
				foreach (var sv in view.Subviews)
				{
					if (sv.IsThisOrSuperviewOf(subview))
						return true;
				}
			}

			return false;
		}

		public static Orientation GetOrientation(this UIView view)
		{
			var orientation = UIApplication.SharedApplication.StatusBarOrientation;
			return IsPortrait(view, orientation) ? Orientation.Portrait : Orientation.Landscape;
		}

		public static void DebugBorder(this UIView view)
		{
			DebugBorder(view, UIColor.Red);
		}

		public static void DebugBorder(this UIView view, UIColor color)
		{
			view.Layer.BorderColor = color.CGColor;
			view.Layer.BorderWidth = 1;
		}

		public static void DropShadow(this UIView view, float shadowRadius = 20f, UIColor color = null)
		{
			view.DropShadow(CGPath.FromRect(view.Bounds), shadowRadius, color);
		}

		public static void DropShadow(this UIView view, CGPath path, float shadowRadius = 20f, UIColor color = null)
		{
			view.Layer.ShadowColor = color == null ? UIColor.Black.CGColor : color.CGColor;
			view.Layer.ShadowOffset = new SizeF(0.0f, 0.0f);
			view.Layer.ShadowOpacity = 1.0f;
			view.Layer.ShadowRadius = shadowRadius;
			view.Layer.ShadowPath = path;
		}

		public static void AddPositionAnimation(this UIView view, double seconds, PointF position)
		{
			CAKeyFrameAnimation keyFrameAnimation = (CAKeyFrameAnimation)CAKeyFrameAnimation.GetFromKeyPath ("position");

			keyFrameAnimation.Duration = seconds;
			keyFrameAnimation.TimingFunction = CAMediaTimingFunction.FromName (CAMediaTimingFunction.EaseInEaseOut);
			view.Layer.Position = position;
			view.Layer.AddAnimation (keyFrameAnimation, "MoveImage");
		}

        public static void FadeOut(this UIView view, float alpha = 0.0f, double duration = 0.3f, NSAction postAnimationAction = null)
        {
            UIView.Animate(duration, 0, UIViewAnimationOptions.CurveEaseInOut,
                () =>
                {
                view.Alpha = alpha;
                },
                postAnimationAction
            );
        }

        public static void FadeIn(this UIView view, double duration = 0.3f, NSAction postAnimationAction = null)
		{
            UIView.Animate(duration, 0, UIViewAnimationOptions.CurveEaseInOut,
				() =>
				{
					view.Alpha = 1.0f;
				},
				postAnimationAction
			);
		}

		public static void AnimateFrame(this UIView view, RectangleF frame, double duration = 0.3f, NSAction postAnimationAction = null)
		{
			UIView.Animate(duration, 0, UIViewAnimationOptions.CurveEaseInOut,
				() =>
				{
					view.Frame = frame;
				},
				postAnimationAction
			);
		}

		public static UIImageView CreateImageOfView(this UIView view)
		{
			//Create the image view that will be temporarily used to simulate the
			// moving page
			UIGraphics.BeginImageContext(view.Frame.Size); 

			view.Layer.RenderInContext(UIGraphics.GetCurrentContext()); 
			UIImage image = UIGraphics.GetImageFromCurrentImageContext();

			UIGraphics.EndImageContext();

			//Show the image view of the moving page
			var imageView = new UIImageView(image);

			return imageView;
		}

		public static PointF ContentPointFromViewPoint(this UIScrollView scrollView, PointF viewPoint)
		{
			float x = (viewPoint.X + scrollView.ContentOffset.X) / scrollView.ZoomScale;
			float y = (viewPoint.Y + scrollView.ContentOffset.Y) / scrollView.ZoomScale;
			
			var point = new PointF(x, y);
			
			return point;
		}

//        public static ScreenLocation GetScreenLocation(this UIScrollView scrollView)
//        {
//            var screenLocation = new ScreenLocation();

//            screenLocation.Point = scrollView.ContentOffset.ToPoint();
//            screenLocation.ZoomScale = scrollView.ZoomScale;

////			Console.WriteLine(screenLocation.ToString());
//            return screenLocation;
//        }

//        public static void SetScreenLocation(this UIScrollView scrollView, ScreenLocation screenLocation)
//        {
//            scrollView.ZoomScale = screenLocation.ZoomScale;
//            scrollView.ContentOffset = screenLocation.Point.ToPointF();

////			Console.WriteLine(screenLocation.ToString());
//        }

		public static void ScrollContentPointToViewPoint(this UIScrollView scrollView, PointF mapPoint, float zoomScale, PointF viewPoint)
		{
			if (scrollView != null)
			{
				scrollView.SetZoomScale(zoomScale, true);
				
				float x = (mapPoint.X * zoomScale) - viewPoint.X;
				float y = (mapPoint.Y * zoomScale) - viewPoint.Y;	
				
				var maxX = (scrollView.ContentSize.Width * zoomScale) - scrollView.Frame.Width;
				var maxY = (scrollView.ContentSize.Height * zoomScale) - scrollView.Frame.Height;
				
				maxX = maxX < 0 ? 0 : maxX;
				maxY = maxY < 0 ? 0 : maxY;
				
				if (x < 0) x = 0;
				if (y < 0) y = 0;
				if (x > maxX) x = maxX;
				if (y > maxY) y = maxY;
				
				scrollView.SetContentOffset(new PointF(x, y), true);
			}
		}

		public static void MoveFrameTo(this UIView view, float x, float y)
		{
			view.Frame = new RectangleF(x, y, view.Frame.Width, view.Frame.Height);
		}

		public static UIImage CreateBitmap(this UIView view)
		{
			// before swaping the views, we'll take a "screenshot" of the current view
			// by rendering its CALayer into the an ImageContext then saving that off to a UIImage
			var viewSize = view.Bounds.Size;
			UIGraphics.BeginImageContextWithOptions(viewSize, false, Settings.IsRetina ? 2.0f : 1.0f);
			view.Layer.RenderInContext(UIGraphics.GetCurrentContext());
			
			// Read the UIImage object
			UIImage image = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			
			return image;

//			//Get the size of the screen
//			var screenRect = UIScreen.MainScreen.Bounds;
//			
//			//Create a bitmap-based graphics context and make
//			//it the current context passing in the screen size
//			UIGraphics.BeginImageContext(screenRect.Size);
//			
//			var ctx = UIGraphics.GetCurrentContext();
//			ctx.SetFillColor(UIColor.Black.CGColor);
//			ctx.FillRect(screenRect);
//
//			//render the receiver and its sublayers into the specified context
//			//choose a view or use the window to get a screenshot of the
//			//entire device
//			view.Layer.RenderInContext(ctx);
//			
//			UIImage newImage = UIGraphics.GetImageFromCurrentImageContext();
//			
//			//End the bitmap-based graphics context
//			UIGraphics.EndImageContext();		
//
//			return newImage;
		}

        public static void SafeDispose(this UIView view)
        {
            if (view != null)
            {
                if (view.Superview != null)
                    view.RemoveFromSuperview();

                view.Dispose();
                view = null;
            }
        }
	}
}

