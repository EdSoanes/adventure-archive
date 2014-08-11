using System;
using System.Drawing;

using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BlackDragon.Archive
{
    [Register("AdventureView")]
    public class AdventureView : UIView
    {
        public AdventureView()
        {
            Initialize();
        }

        public AdventureView(RectangleF bounds)
            : base(bounds)
        {
            Initialize();
        }

        void Initialize()
        {
            BackgroundColor = UIColor.Red;
        }
    }
}