using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using System.Drawing;
using MonoTouch.CoreGraphics;

namespace BlackDragon.Fx.Extensions
{
	public enum AnimateTilt
	{
		Left,
		Right
	}

	public static class UIImageViewExtensions
	{
		public static void AnimateSlide(this UIImageView imageView, RectangleF targetFrame, AnimateTilt direction, NSAction completion = null)
		{
			//Animate the movement of the page from content area to notebook
			UIView.Animate(0.25, 0, UIViewAnimationOptions.CurveEaseInOut, () =>
            {
				imageView.Frame = targetFrame;
				UIView.Animate(0.15, 0, UIViewAnimationOptions.BeginFromCurrentState, () =>
                {
					float degrees = direction == AnimateTilt.Left ? -10f : 10f;
					imageView.Transform = CGAffineTransform.MakeRotation(degrees.DegreesToRadians());
					UIView.Animate(0.1, 0.15, UIViewAnimationOptions.BeginFromCurrentState, () =>
					{
						imageView.Transform = CGAffineTransform.MakeRotation(0f.DegreesToRadians());
					}, 
					completion);
				}, null);
			}, null);		
		}
	}
}

