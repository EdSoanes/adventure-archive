using System;
using System.Drawing;
using MonoTouch.GLKit;

namespace BlackDragon.Fx.MapGL
{
    public class MapTileView : GLKView
    {
        public MapTileView(RectangleF frame)
            : base(frame)
        {
        }

        public override void Draw(RectangleF rect)
        {
            base.Draw(rect);
        }
    }
}

