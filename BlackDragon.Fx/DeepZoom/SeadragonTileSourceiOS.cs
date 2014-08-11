using System;
using System.Drawing;
using BlackDragon.Core;
using BlackDragon.Core.DeepZoom;
using MonoTouch.Foundation;
using MonoTouch.UIKit;
using BlackDragon.Fx.Extensions;

namespace BlackDragon.Fx.DeepZoom
{
	public class SeadragonTileSourceiOS : SeadragonTileSource
    {
		private enum TileLocation
		{
			TopLeft,
			TopRight,
			BottomLeft,
			BottomRight
		}

		public SeadragonTileSourceiOS(IFileAccessService fileAccessService, ILogService log, bool hiRes = false)
			: base(fileAccessService, log, hiRes)
        {
        }

		protected override object GetImage(byte[] imageData)
		{
			return imageData.ToImage();
		}

		protected override void GenerateTiles(object image, SeadragonTileIndex index)
		{
			var tile = image as UIImage;
			if (tile != null && index != null && index.LevelOfDetail < Dzi.Levels)
			{
				using (var baseTile = tile.ResizeImage(tile.Size.Width * 2, tile.Size.Height * 2))
				{
					if (baseTile != null)
					{
						CreateTile(baseTile, index, TileLocation.TopLeft);
						CreateTile(baseTile, index, TileLocation.TopRight);
						CreateTile(baseTile, index, TileLocation.BottomLeft);
						CreateTile(baseTile, index, TileLocation.BottomRight);
					}
				}
			}
		}

		private void CreateTile(UIImage baseTile, SeadragonTileIndex baseTileIdx, TileLocation loc)
		{
			try
			{
				var baseTileRect = new RectangleF(0, 0, baseTile.Size.Width, baseTile.Size.Height);

				var x = baseTileIdx.Col == 0 ? 0 : Dzi.Overlap * 2;
				var y = baseTileIdx.Row == 0 ? 0 : Dzi.Overlap * 2;

				x += (loc == TileLocation.TopRight || loc == TileLocation.BottomRight ? Dzi.TileSize : 0);
				y += (loc == TileLocation.BottomLeft || loc == TileLocation.BottomRight ? Dzi.TileSize : 0);
				var cropRect = new RectangleF(x - Dzi.Overlap, y - Dzi.Overlap, Dzi.TileSize + (2 * Dzi.Overlap), Dzi.TileSize + (2 * Dzi.Overlap));
				cropRect.Intersect(baseTileRect);

				if (cropRect.Width > 0 && cropRect.Height > 0)
				{
					//Create the new seadragon index object for the new tile
					var newTileCol = (2 * baseTileIdx.Col) + (loc == TileLocation.TopRight || loc == TileLocation.BottomRight ? 1 : 0);
					var newTileRow = (2 * baseTileIdx.Row) + (loc == TileLocation.BottomLeft || loc == TileLocation.BottomRight ? 1 : 0);
					var newLevel = baseTileIdx.LevelOfDetail + 1;
					var newTileIdx = new SeadragonTileIndex(newTileCol, newTileRow, newLevel);

					//Create the new tile name
					var newTileName = TileUrl(newTileCol, newTileRow, this.ScaleForLevelOfDetail(newTileIdx.LevelOfDetail));

					//Write new tile to disk
					using (var newTile = baseTile.CropImage((int)cropRect.X, (int)cropRect.Y, (int)cropRect.Width, (int)cropRect.Height))
					{
						if (newTile != null)
						{
							//CHECK IF THERE IS ALREADY A TILE IN THE NORMAL, HIRES CACHE. DO NOT SAVE IN LOWRES IF
							// THERE IS ALREADY THE HIRES VERSION...
							if (!FileAccessService.ExistsInCache(newTileName))
							{
								using (var stream = newTile.ToStream(Dzi.Format))
									FileAccessService.WriteToCache(newTileName, LowResFolder, stream);
							}

							Console.WriteLine(newTileName);

							if (newTileIdx.LevelOfDetail < Dzi.Levels)
							{
								using (var newBaseTile = newTile.ResizeImage(newTile.Size.Width * 2, newTile.Size.Height * 2))
								{
									if (newBaseTile != null)
									{
										CreateTile(newBaseTile, newTileIdx, TileLocation.TopLeft);
										CreateTile(newBaseTile, newTileIdx, TileLocation.TopRight);
										CreateTile(newBaseTile, newTileIdx, TileLocation.BottomLeft);
										CreateTile(newBaseTile, newTileIdx, TileLocation.BottomRight);
									}
								}
							}
						}
					}
				}
			}
			catch (Exception esx)
			{
				Console.WriteLine(esx.ToString());
			}
		}
    }
}

