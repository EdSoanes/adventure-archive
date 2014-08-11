using System;
using System.Drawing;

using BlackDragon.Core.DeepZoom;
using MonoTouch.CoreAnimation;
using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
using MonoTouch.UIKit;
using BlackDragon.Fx.Extensions;

namespace BlackDragon.Fx.DeepZoom
{
	public class SeadragonTileView : UIView
	{
		[Export ("layerClass")]
		public static Class LayerClass ()
		{    
			return new Class (typeof (CATiledLayer));
		}

		SeadragonTileSource TileSource { get; set; }

		public SeadragonTileView (SeadragonTileSource tileSource) : 
			base (new RectangleF (PointF.Empty, tileSource.HiRes ? new SizeF(tileSource.Dzi.Width * 2, tileSource.Dzi.Height * 2) : new SizeF(tileSource.Dzi.Width, tileSource.Dzi.Height)))
		{
			this.BackgroundColor = UIColor.Clear;
			TileSource = tileSource;
			var tiledLayer = (CATiledLayer) this.Layer; 
			tiledLayer.LevelsOfDetail = 4;

			if (TileSource.HiRes)
				tiledLayer.TileSize = new SizeF(512f, 512f);
		}

		// to handle the interaction between CATiledLayer and high resolution screens, we need to always keep the
		// tiling view's contentScaleFactor at 1.0. UIKit will try to set it back to 2.0 on retina displays, which is the
		// right call in most cases, but since we're backed by a CATiledLayer it will actually cause us to load the
		// wrong sized tiles.

		public override float ContentScaleFactor {
			set {
				base.ContentScaleFactor = 1.0f;
			}
		}	

		public override void Draw (RectangleF rect)
		{
			var context = UIGraphics.GetCurrentContext ();
			// get the scale from the context by getting the current transform matrix, then asking for
			// its "a" component, which is one of the two scale components. We could also ask for "d".
			// This assumes (safely) that the view is being scaled equally in both dimensions.
			//var scale = (float)(1 / Math.Round(1 / context.GetCTM ().xx));
			var scale = context.GetCTM().xx;
			CATiledLayer tiledLayer = (CATiledLayer) this.Layer; 
			var tileSize = tiledLayer.TileSize;

			// Even at scales lower than 100%, we are drawing into a rect in the coordinate system of the full
			// image. One tile at 50% covers the width (in original image coordinates) of two tiles at 100%. 
			// So at 50% we need to stretch our tiles to double the width and height; at 25% we need to stretch 
			// them to quadruple the width and height; and so on.
			// (Note that this means that we are drawing very blurry images as the scale gets low. At 12.5%, 
			// our lowest scale, we are stretching about 6 small tiles to fill the entire original image area. 
			// But this is okay, because the big blurry image we're drawing here will be scaled way down before 
			// it is displayed.)
			tileSize.Width /= scale;
			tileSize.Height /= scale;

			// calculate the rows and columns of tiles that intersect the rect we have been asked to draw
			int firstCol = (int) Math.Floor(rect.GetMinX () / tileSize.Width);
			int lastCol = (int) Math.Floor((rect.GetMaxX () - 1) / tileSize.Width);
			int firstRow = (int) Math.Floor(rect.GetMinY () / tileSize.Height);
			int lastRow = (int) Math.Floor((rect.GetMaxY () - 1) / tileSize.Height);

			for (int row = firstRow; row <= lastRow; row++) 
			{
				for (int col = firstCol; col <= lastCol; col++) 
				{
					var tile = TileSource.GetTile<UIImage>(col, row, scale);
					if (tile != null)
					{
						Console.WriteLine("Requested tile: " + TileSource.TileUrl(col, row, scale) + ". Size: " + tile.Size );

						int offX = col == 0 ? 0 : TileSource.Dzi.Overlap;
						int offY = row == 0 ? 0 : TileSource.Dzi.Overlap;

						float x = tileSize.Width * col - (offX / scale);
						float y = tileSize.Height * row - (offY / scale);
						var tileRect = new RectangleF(x, y, tile.Size.Width / scale, tile.Size.Height / scale);

						if (TileSource.HiRes)
							tileRect = tileRect.Scale(2); 

						Console.WriteLine("TileRect: " + tileRect.ToString());
						// if the tile would stick outside of our bounds, we need to truncate it so as to avoid
						// stretching out the partial tiles at the right and bottom edges
						tileRect.Intersect(this.Bounds);
						tile.Draw(tileRect);
					}
				}
			}
		}
	}
}