using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace BlackDragon.Archive
{
    public class TheArchiveButton : UIButton
    {
        public TheArchiveButton() 
            : base()
        {
            InitializeView();
        }

        public TheArchiveButton(RectangleF frame) 
            : base(frame)
        {
            InitializeView();
        }

        private void InitializeView()
        {
            using (var img = UIImage.FromFile("Images/the-archive-button.png"))
            {
                this.SetImage(img, UIControlState.Normal);
                this.Frame = new RectangleF(PointF.Empty, img.Size);
            }
        }
    }
}

