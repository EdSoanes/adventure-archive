using System;
using MonoTouch.UIKit;
using System.Drawing;
using BlackDragon.Core.DeepZoom;
using BlackDragon.Fx.Extensions;

namespace BlackDragon.Fx.DeepZoom
{
	public class SeadragonView : UIView
    {
		private string _backgroundVertical;
		private string _backgroundHorizontal;

		private SeadragonOverlayView _seadragonOverlayView;

		public SeadragonView(RectangleF frame, string backgroundVertical, string backgroundHorizontal)
			: base(frame)
        {
			InitializeView(backgroundVertical, backgroundHorizontal);
        }

		private void InitializeView(string backgroundVertical, string backgroundHorizontal)
		{
			_backgroundVertical = backgroundVertical;
			_backgroundHorizontal = backgroundHorizontal;

			var scrollView = new SeadragonScrollView(this.Frame);
			scrollView.Alpha = 0.0f;
			scrollView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			this.AddSubview(scrollView);

			if (!string.IsNullOrEmpty(_backgroundVertical))
				this.BackgroundColor = UIColor.FromPatternImage(UIImage.FromFile(_backgroundVertical));
		}

		public void Setup(SeadragonTileSource tileSource)
		{
			var scrollView = this.Child<SeadragonScrollView>();
			if (scrollView != null)
				scrollView.Setup(tileSource);

			tileSource.TilesInitialized += (sender, e) => 
			{
				InvokeOnMainThread(() => 
				{
					if (scrollView != null)
						scrollView.FadeIn();
				});
			};

			if (_seadragonOverlayView != null)
			{
				_seadragonOverlayView.RemoveFromSuperview();
				_seadragonOverlayView.Dispose();
				_seadragonOverlayView = null;
			}
		}

		public void Show()
		{
			var scrollView = this.Child<SeadragonScrollView>();
			if (scrollView != null)
				scrollView.FadeIn();
		}

		public void Hide()
		{
			var scrollView = this.Child<SeadragonScrollView>();
			if (scrollView != null)
				scrollView.FadeOut();
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			if (this.IsPortrait() && !string.IsNullOrEmpty(_backgroundVertical))
				this.BackgroundColor = UIColor.FromPatternImage(UIImage.FromFile(_backgroundVertical));
			else if (this.IsLandscape() && !string.IsNullOrEmpty(_backgroundHorizontal))
				this.BackgroundColor = UIColor.FromPatternImage(UIImage.FromFile(_backgroundHorizontal));
		}
    }
}

