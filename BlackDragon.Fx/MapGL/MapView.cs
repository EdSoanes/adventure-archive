using System;
using MonoTouch.UIKit;
using System.Drawing;
using BlackDragon.Fx.Extensions;

namespace BlackDragon.Fx.MapGL
{
    public class MapView : UIView
    {
        public MapView(RectangleF frame) 
            : base(frame)
        {
            InitializeView();
        }

        public MapTileView MapTileView
        {
            get { return this.Child<MapTileView>(); }
        }

        private void InitializeView()
        {
            this.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            this.DebugBorder();

            var mapTileView = new MapTileView(this.Frame);
            mapTileView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            this.AddSubview(mapTileView);
        }
    }
}

