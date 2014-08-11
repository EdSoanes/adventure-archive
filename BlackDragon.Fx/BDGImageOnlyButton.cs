using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace BlackDragon.Fx
{
	public class BDGImageOnlyButton : UIButton
    {
        public BDGImageOnlyButton()
			: base()
        {
			InitializeView();
        }

		public BDGImageOnlyButton(RectangleF frame)
			: base(frame)
		{
			InitializeView();
		}

		public BDGImageOnlyButton(UIImage image, UIImage imageDown)
			: base()
		{
			InitializeView(image, imageDown);
		}

		public void InitializeView(UIImage image = null, UIImage imageDown = null)
		{
			this.BackgroundColor = UIColor.Clear;
			this.TintColor = UIColor.Clear;

			if (image != null)
			{
				this.Frame = new RectangleF(0, 0, image.Size.Width, image.Size.Height);
				this.SetBackgroundImage(image, UIControlState.Normal);
			}

			if (imageDown != null)
				this.SetBackgroundImage(imageDown, UIControlState.Highlighted);
		}
    }
}

