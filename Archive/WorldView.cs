using System;
using System.Drawing;

using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using BlackDragon.Fx.Extensions;

namespace BlackDragon.Archive
{
    [Register("WorldView")]
    public class WorldView : UIView
    {
        public WorldView()
        {
            Initialize();
        }

        public WorldView(RectangleF bounds)
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