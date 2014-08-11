using System;
using MonoTouch.UIKit;
using System.Drawing;
using BlackDragon.Core.Entities;
using BlackDragon.Fx.Extensions;

namespace BlackDragon.Fx.MapGL
{
	public class MapMarkerView : UIView
	{
		public const float ControlWidth = 100;

		private PointF _origin;
		private SizeF _size;

		public PointF TouchDelta
		{
			get;
			private set;
		}

		public MapMarker Marker
		{
			get;
			private set;
		}

		public MapMarkerView() : base()
		{
		}

		public MapMarkerView(IntPtr handle) : base(handle)
		{

		}

		public MapMarkerView(MapMarker marker, UIImage image)
			: this(marker, image, -1)
		{
		}

		public MapMarkerView(MapMarker marker, UIImage image, float zoomScale = -1)
		{
			Initialize(marker, image, zoomScale);
		}

		public void Initialize(MapMarker marker, UIImage image, float zoomScale = -1)
		{
			if (zoomScale == -1)
				zoomScale = UIScreen.MainScreen.Scale;

			Marker = marker;

			this.Layer.MasksToBounds = false;
			this.DebugBorder();

			var markerView = new UIImageView(image);
			markerView.Frame = new RectangleF(new PointF(0, 0), markerView.Frame.Size).CenterOnX(ControlWidth / 2);
			this.AddSubview(markerView);			

			var title = new UITextView(new RectangleF(0, 0, ControlWidth, 10).MoveToY(markerView.Frame.Height));
			title.UserInteractionEnabled = false;
			title.BackgroundColor = UIColor.Clear;
			title.Text = Marker.Title;
			title.Font = UIFont.FromName("Georgia", 16);
			title.TextColor = UIColor.Black;
			title.TextAlignment = UITextAlignment.Center;
			this.AddSubview(title);
			title.Frame = title.Frame.NewHeight(title.ContentSize.Height);

			var ds = new UIView(title.Frame.Shrink(10, 10).Move(5, 5));
			ds.BackgroundColor = UIColor.Clear;
			ds.Layer.CornerRadius = 5;
			ds.DropShadow(5f, new UIColor(255f, 255f, 255f, 0.6f));
			this.AddSubview(ds);
			this.SendSubviewToBack(ds);

			var height = markerView.Frame.Height + title.Frame.Height;
			_size = new SizeF(ControlWidth, height);
			_origin = new PointF(ControlWidth / 2, _size.Height - title.Frame.Height);

			SetFrame(zoomScale);
		}

		public void Persist()
		{
			throw new NotImplementedException();
		}

		public void SetFrame(float zoomScale = 1)
		{
			var x = (Marker.X * zoomScale) - _origin.X;
			var y = (Marker.Y * zoomScale) - _origin.Y;

			this.Frame = new RectangleF(new PointF(x, y), _size);
		}

		public void SetTouchDelta(PointF touchLocation)
		{
			var deltaX = _origin.X - (touchLocation.X - this.Frame.X);
			var deltaY = _origin.Y - (touchLocation.Y - this.Frame.Y);

			this.TouchDelta = new PointF(deltaX, deltaY); 
		}

		public void SetCentredTouchDelta()
		{
			this.TouchDelta = new PointF(_origin.X - (this.Frame.Width / 2), _origin.Y - this.Frame.Height / 2);
		}
	}
}

