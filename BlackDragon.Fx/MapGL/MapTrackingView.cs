using System;
using System.Linq;
using MonoTouch.UIKit;
using System.Drawing;
using BlackDragon.Fx.Extensions;
using BlackDragon.Core.Entities;
using System.Collections.Generic;

namespace BlackDragon.Fx.MapGL
{
    public class MapTrackingView : UIScrollView
    {
        public MapTrackingView(RectangleF frame)
            : base(frame)
        {
            InitializeView();
        }

        private UIView TrackingView
        {
            get { return this.Child<UIView>(); }
        }

        private void InitializeView()
        {
            this.UserInteractionEnabled = true;
            this.MultipleTouchEnabled = true;
            this.ShowsVerticalScrollIndicator = false;
            this.ShowsHorizontalScrollIndicator = false;
            this.BouncesZoom = true;
			this.DecelerationRate = 0.990f;
            this.ContentInset = new UIEdgeInsets(this.Frame.Height / 2, this.Frame.Width / 2, this.Frame.Height / 2, this.Frame.Width / 2);
			this.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
        }

		public void Setup(Map map)
        {
			var mapSize = map.MapSize();
			this.ContentSize = mapSize;

            //Set up scroll view for tracking user interactions. If one already exists then resize according 
            // to the new map size
			var subviewFrame = new RectangleF(0, 0, mapSize.Width, mapSize.Height);
            if (TrackingView == null)
            {
                var trackingView = new UIView(subviewFrame);
                trackingView.BackgroundColor = UIColor.Clear;

                this.AddSubview(trackingView);
                this.ViewForZoomingInScrollView = delegate(UIScrollView scrollView)
                {
                    return trackingView;
                };
            }
            else
            {
                TrackingView.Frame = subviewFrame;
            }

            var minZoomX = this.Frame.Width / mapSize.Width;
            var minZoomY = this.Frame.Height / mapSize.Height;

            this.MinimumZoomScale = Math.Min(minZoomX, minZoomY);
            this.MaximumZoomScale = 1f;
            this.SetZoomScale(this.MinimumZoomScale, false);

			SetupMarkers(map);
        }

		private void SetupMarkers(Map map)
		{
			//Load a dictionary of marker images.
			var images = new Dictionary<string, UIImage>();
			foreach (var path in map.Markers.Select(x => x.ImagePath).Distinct())
			{
				var img = UIImage.FromFile(path);
				images.Add(path, img);
			}

			//Create the markers with the correct images
			foreach (var mapMarker in map.Markers)
			{
				var mv = new MapMarkerView(mapMarker, images[mapMarker.ImagePath], this.ZoomScale);
				TrackingView.AddSubview(mv);
			}
		}

		public void CenterMap()
		{
			var mx = (this.ContentSize.Width / this.ZoomScale) / 2;
			var my = (this.ContentSize.Height / this.ZoomScale) / 2;

			SetCenterPosition(new MapPosition(new PointF(mx, my), this.ZoomScale, this.MinimumZoomScale));
		}

		public MapPosition GetCurrentOffset()
		{
			return new MapPosition(this.ContentOffset, this.ZoomScale, this.MinimumZoomScale);
		}

		public MapPosition GetCenterPosition()
		{
			var cx = this.Bounds.Width / 2;
			var cy = this.Bounds.Height / 2;

			float x = (cx + this.ContentOffset.X) / this.ZoomScale;
			float y = (cy + this.ContentOffset.Y) / this.ZoomScale;

			var mapPoint = new PointF(x, y);

			return new MapPosition(mapPoint, ZoomScale, MinimumZoomScale);
		}

		public void SetCenterPosition(MapPosition position)
		{
			if (position != null)
			{
				this.SetZoomScale(position.Scale, false);

				var w = this.Bounds.Width / 2;
				var h = this.Bounds.Height / 2;

				var offsetX = (position.X * position.Scale) - w;
				var offsetY = (position.Y * position.Scale) - h;

				this.SetContentOffset(new PointF(offsetX, offsetY), false);
			}
			else
				CenterMap();
		}

        public void ShowDebugFrame()
        {
            if (TrackingView != null)
                TrackingView.DebugBorder(UIColor.Blue);
        }
    }
}

