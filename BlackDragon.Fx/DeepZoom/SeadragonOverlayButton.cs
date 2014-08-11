using System;
using MonoTouch.UIKit;
using System.Drawing;
using BlackDragon.Core.DeepZoom;

namespace BlackDragon.Fx.DeepZoom
{
	public class SeadragonOverlayButton : UIButton
    {
		public SeadragonOverlay Overlay
		{
			get;
			private set;
		}

		public SeadragonOverlayButton(SeadragonOverlay overlay, float scale)
			: base()
        {
			InitializeView(overlay, scale);
        }

		private void InitializeView(SeadragonOverlay overlay, float scale)
		{
			Overlay = overlay;

			var img = Overlay.Image as UIImage;
			this.SetBackgroundImage(img, UIControlState.Normal);

			SetFrame(scale);
		}

		public void SetFrame(float scale)
		{
			var img = Overlay.Image as UIImage;

			var width = img.Size.Width / scale;
			var height = img.Size.Height / scale;

			var x = Overlay.X - (Overlay.OriginX / scale);
			var y = Overlay.Y - (Overlay.OriginY / scale);

			this.Frame = new RectangleF(x, y, width, height);
		}
    }
}

