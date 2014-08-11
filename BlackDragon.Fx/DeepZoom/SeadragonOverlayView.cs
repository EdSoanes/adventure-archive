using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace BlackDragon.Fx.DeepZoom
{
	public class SeadragonOverlayView : UIView
    {
        public SeadragonOverlayView()
			: base()
        {
			InitializeView();
        }

		public SeadragonOverlayView(RectangleF frame)
			: base(frame)
		{
			InitializeView();
		}

		private void InitializeView()
		{
			BackgroundColor = UIColor.Clear;
		}

		public override float ContentScaleFactor {
			set {
				base.ContentScaleFactor = 1.0f;
			}
		}	
    }
}

