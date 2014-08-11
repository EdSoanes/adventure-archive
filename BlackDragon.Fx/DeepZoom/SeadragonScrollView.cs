using System;
using System.Drawing;
using System.Linq;
using BlackDragon.Core;
using BlackDragon.Core.DeepZoom;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;

namespace BlackDragon.Fx.DeepZoom
{
	public class SeadragonScrollView : UIScrollView
	{
		UIView _containerView;
		SeadragonTileView _seadragonTileView;
		SeadragonOverlayView _seadragonOverlayView;
		SeadragonTileSource _tileSource;

		public SeadragonScrollView (RectangleF frame) : base (frame)
		{
			ShowsVerticalScrollIndicator = false;
			ShowsHorizontalScrollIndicator = false;
			BouncesZoom = true;
			DecelerationRate = 0.990f; //UIScrollViewDecelerationRateFast;
			BackgroundColor = UIColor.Clear;
			#region UIScrollView delegate methods
			ViewForZoomingInScrollView = 
				delegate (UIScrollView scrollView)
			{
				return _containerView;
			};
			#endregion

			_containerView = new UIView();
			_containerView.BackgroundColor = UIColor.Clear;
			this.AddSubview(_containerView);
			this.DidZoom += (sender, e) => 
			{
				if (_seadragonOverlayView != null)
				{
					foreach (var btn in _seadragonOverlayView.Subviews.Where(x => x is SeadragonOverlayButton).Cast<SeadragonOverlayButton>())
					{
						btn.SetFrame(this.ZoomScale);
					}
				}

				if (_seadragonTileView != null)
					_seadragonTileView.SetNeedsDisplay();
			};
		}

		#region Override layoutSubviews to center content
		public override void LayoutSubviews ()
		{
			base.LayoutSubviews ();

			if (_containerView != null)
			{
				// center the image as it becomes smaller than the size of the screen
				var boundsSize = this.Bounds.Size;
				var frameToCenter = _containerView.Frame;

				// center horizontally
				if (frameToCenter.Size.Width < boundsSize.Width)
					frameToCenter.X = (boundsSize.Width - frameToCenter.Size.Width) / 2;
				else
					frameToCenter.X = 0;

				// center vertically
				if (frameToCenter.Size.Height < boundsSize.Height)
					frameToCenter.Y = (boundsSize.Height - frameToCenter.Size.Height) / 2;
				else
					frameToCenter.Y = 0;

				_containerView.Frame = frameToCenter;

				if (_seadragonTileView != null)
				{
					// to handle the interaction between CATiledLayer and high resolution screens, we need to manually set the
					// tiling view's contentScaleFactor to 1.0. (If we omitted this, it would be 2.0 on high resolution screens,
					// which would cause the CATiledLayer to ask us for tiles of the wrong scales.)
					if (_seadragonTileView.RespondsToSelector(new Selector("scale")))
						_seadragonTileView.ContentScaleFactor = 1.0f;		// beware pre-iOS4 [CD]
				}

				if (_seadragonOverlayView != null)
				{
					// to handle the interaction between CATiledLayer and high resolution screens, we need to manually set the
					// tiling view's contentScaleFactor to 1.0. (If we omitted this, it would be 2.0 on high resolution screens,
					// which would cause the CATiledLayer to ask us for tiles of the wrong scales.)
					if (_seadragonOverlayView.RespondsToSelector(new Selector("scale")))
						_seadragonOverlayView.ContentScaleFactor = 1.0f;		// beware pre-iOS4 [CD]
				}
			}
		}
		#endregion

		#region Configure scrollView to display new image (tiled or not)

		public void Setup(SeadragonTileSource tileSource)
		{
			//Clear the old tile source if there is one
			if (_tileSource != null)
			{
				_tileSource.TilesInitialized -= OnTilesInitialized;
				_tileSource.TileDownloaded -= OnTileDownloaded;
				_tileSource = null;
			}

			_tileSource = tileSource;
			_tileSource.TilesInitialized += OnTilesInitialized;
			_tileSource.TileDownloaded += OnTileDownloaded;

			// clear the previous imageView
			if (_seadragonTileView != null)
			{
				_seadragonTileView.RemoveFromSuperview();
				_seadragonTileView.Dispose();
				_seadragonTileView = null; // Not sure we need this [CD]
			}

			if (_seadragonOverlayView != null)
			{
				_seadragonOverlayView.RemoveFromSuperview();
				_seadragonOverlayView.Dispose();
				_seadragonOverlayView = null;
			}

			// reset our zoomScale to 1.0 before doing any further calculations
			this.ZoomScale = 1.0f;

			_seadragonTileView = new SeadragonTileView(_tileSource);
			_containerView.Frame = _seadragonTileView.Frame;

			this._containerView.AddSubview(_seadragonTileView);

			this.ContentSize = new SizeF(tileSource.Dzi.Width, tileSource.Dzi.Height);
			this.SetMaxMinZoomScalesForCurrentBounds();
			this.ZoomScale = this.MinimumZoomScale;

			//Load the initial tile set

			//CAN WE PREVENT THE VIEW FROM DRAWING ANYTHING UNTIL THE TILES ARE INITIALIZED???
			tileSource.InitializeTiles(this.MinimumZoomScale);
		}

		private void OnTilesInitialized(object sender, EventArgs e)
		{
			InvokeOnMainThread(() =>
			{
				if (_tileSource.Overlays.Any())
				{
					_seadragonOverlayView = new SeadragonOverlayView(_seadragonTileView.Frame);
					_seadragonOverlayView.BackgroundColor = UIColor.Clear;
					this._containerView.AddSubview(_seadragonOverlayView);

					foreach (var overlay in _tileSource.Overlays)
					{
						var btn = new SeadragonOverlayButton(overlay, this.ZoomScale);
						btn.TouchUpInside += (s1, e1) => 
						{
							var selectedBtn = s1 as SeadragonOverlayButton;
							if (selectedBtn != null)
								_tileSource.SelectOverlay(selectedBtn.Overlay);
						};
						_seadragonOverlayView.AddSubview(btn);
					}

					if (_seadragonTileView != null)
						_seadragonTileView.SetNeedsDisplay();
				}
			});
		}

		public void OnTileDownloaded(object sender, SeadragonTileDownloadedEventArgs e)
		{
			InvokeOnMainThread(() =>
			{
				if (_seadragonTileView != null)
				{
					var x = (e.Index.Col * _tileSource.Dzi.TileSize) / this.ZoomScale;
					var y = (e.Index.Row * _tileSource.Dzi.TileSize) / this.ZoomScale;
					var sz = _tileSource.Dzi.TileSize / this.ZoomScale;
					var rect = new RectangleF(x, y, sz, sz);
					_seadragonTileView.SetNeedsDisplayInRect(rect);
				}
			});
		}

		public void SetMaxMinZoomScalesForCurrentBounds()
		{
			var boundsSize = this.Bounds.Size;
			var imageSize = _tileSource.HiRes ? new SizeF(_tileSource.Dzi.Width * 2, _tileSource.Dzi.Height * 2) : new SizeF(_tileSource.Dzi.Width, _tileSource.Dzi.Height);

			float xScale = boundsSize.Width / imageSize.Width;    // the scale needed to perfectly fit the image width-wise
			float yScale = boundsSize.Height / imageSize.Height;  // the scale needed to perfectly fit the image height-wise
			float minScale = Math.Min(xScale, yScale);            // use minimum of these to allow the image to become fully visible

			// on high resolution screens we have double the pixel density, so we will be seeing every pixel if we limit the
			// maximum zoom scale to 0.5.
			float maxScale = 1.0f / UIScreen.MainScreen.Scale; // beware pre-iOS4 [CD]

			// don't let minScale exceed maxScale. (If the image is smaller than the screen, we don't want to force it to be zoomed.) 
			if (minScale > maxScale)
			{
				minScale = maxScale;
			}

			this.MaximumZoomScale = maxScale;
			this.MinimumZoomScale = minScale;
		}
		#endregion

		#region Methods called during rotation to preserve the zoomScale and the visible portion of the image
		// returns the center point, in image coordinate space, to try to restore after rotation. 
		public PointF PointToCenterAfterRotation()
		{
			var boundsCenter = new PointF(this.Bounds.GetMidX(), this.Bounds.GetMidY());
			return this.ConvertPointToView (boundsCenter, _seadragonTileView);
		}

		// returns the zoom scale to attempt to restore after rotation. 
		public float ScaleToRestoreAfterRotation ()
		{
			var contentScale = this.ZoomScale;

			// If we're at the minimum zoom scale, preserve that by returning 0, which will be converted to the minimum
			// allowable scale when the scale is restored.
			if (contentScale <= this.MinimumZoomScale + float.Epsilon)
				contentScale = 0;

			return contentScale;
		}

		private PointF MaximumContentOffset 
		{
			get
			{
				var contentSize = this.ContentSize;
				var boundsSize = this.Bounds.Size;
				return new PointF(contentSize.Width - boundsSize.Width, contentSize.Height - boundsSize.Height);
			}
		}

		private PointF MinimumContentOffset
		{
			get
			{
				return new PointF(0,0); //PointF.Empty; // zero? [CD]
			}
		}

		// Adjusts content offset and scale to try to preserve the old zoomscale and center.
		public void RestoreCenterPoint (PointF oldCenter, float oldScale)
		{
			// Step 1: restore zoom scale, first making sure it is within the allowable range.
			this.ZoomScale = Math.Min(this.MaximumZoomScale, Math.Max(this.MinimumZoomScale, oldScale));


			// Step 2: restore center point, first making sure it is within the allowable range.

			// 2a: convert our desired center point back to our own coordinate space
			var boundsCenter = this.ConvertPointFromView(oldCenter,_seadragonTileView);
			// 2b: calculate the content offset that would yield that center point
			var offset = new PointF (boundsCenter.X - this.Bounds.Size.Width / 2.0f, 
				boundsCenter.Y - this.Bounds.Size.Height / 2.0f);
			// 2c: restore offset, adjusted to be within the allowable range
			var maxOffset = this.MaximumContentOffset;
			var minOffset = this.MinimumContentOffset;
			offset.X = Math.Max(minOffset.X, Math.Min(maxOffset.X, offset.X));
			offset.Y = Math.Max(minOffset.Y, Math.Min(maxOffset.Y, offset.Y));
			this.ContentOffset = offset;
		}
		#endregion
	}
}