using System;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;

namespace BlackDragon.Core.Entities
{
	public class Map : IdentifiableItem
	{
        public const int TileWidth = 256;
        public const int TileHeight = 256;
		public const int Overlap = 1;

		public Map()
		{
			Markers = new List<MapMarker>();
            Tiles = new List<MapTileData>();
		}

		public int Width
		{
			get;
			set;
		}
		
		public int Height
		{
			get;
			set;
		}

		public string GeneratorApp
		{
			get;
			set;
		}

		public string FileExtension
		{
			get;
			set;
		}
		
        private string _dziFilePath = "";
        public string DziFilePath
        {
            get { return _dziFilePath; }
            set
            {        
                if (value.ToLower().EndsWith(".aspx"))
                    _dziFilePath = value.Substring(0, value.Length - 5);
                else
                    _dziFilePath = value;
            }
        }

		public List<MapMarker> Markers
		{
			get;
			set;
		}

        public List<MapTileData> Tiles
        {
            get;
            private set;
        }

        public string GetSeadragonMapTilePath(int zoomLevel, int row, int col)
        {
            int scaleFolder = 12;

            if (zoomLevel == 4)
                scaleFolder = 12;
            else if (zoomLevel == 3)
                scaleFolder = 11;
            else if (zoomLevel == 2)
                scaleFolder = 10;
            else
                scaleFolder = 9;

            var mapPath = DziFilePath.CombineUrl(String.Format("{0}/{1}_{2}.{3}", scaleFolder, col, row, FileExtension));
            return mapPath;
        }

		public string GetAbsoluteMapTilePath(Single scale, int row, int col)
		{
            var zoomFactor = Settings.GetZoomFactor(scale) * 1000;
			if (GeneratorApp == "Tilen")
			{
				var mapPath = DziFilePath.CombineUrl(String.Format("{0}/x{1}y{2}.{3}", zoomFactor, col + 1, row + 1, FileExtension));
				return mapPath;
			}
			else if (GeneratorApp == "Seadragon")
			{
				int scaleFolder = 12;

				if (zoomFactor == 1000) 
					scaleFolder = 12;
				else if (zoomFactor == 500)
					scaleFolder = 11;
				else if (zoomFactor == 250)
					scaleFolder = 10;
				else
					scaleFolder = 9;

				var mapPath = DziFilePath.CombineUrl(String.Format("{0}/{1}_{2}.{3}", scaleFolder, col, row, FileExtension));
				return mapPath;
			}

			return string.Empty;
		}

        public int NoOfTilesX(int levelOfDetail = 4)
        {
            var tilesX = Math.Ceiling((float)Width / TileWidth);
            switch (levelOfDetail)
            {
                case 1: return Convert.ToInt32(Math.Ceiling(tilesX / 8));
                case 2: return Convert.ToInt32(Math.Ceiling(tilesX / 4));
                case 3: return Convert.ToInt32(Math.Ceiling(tilesX / 2));
                default: return Convert.ToInt32(tilesX);
            }
        }

        public int NoOfTilesY(int levelOfDetail = 4)
        {
            var tilesY = Math.Ceiling((float)Height / TileHeight);
            switch (levelOfDetail)
            {
                case 1: return Convert.ToInt32(Math.Ceiling(tilesY / 8));
                case 2: return Convert.ToInt32(Math.Ceiling(tilesY / 4));
                case 3: return Convert.ToInt32(Math.Ceiling(tilesY / 2));
                default: return Convert.ToInt32(tilesY);
            }
        }

        public void CreateMapTiles(int minLevelOfDetail)
        {
			if (Tiles == null || !Tiles.Any())
			{
				var tiles = new List<MapTileData>();
				//Initialize Tile list
				for (int i = minLevelOfDetail; i <= 4; i++)
				{
					var noOfTilesX = NoOfTilesX(i);
					var noOfTilesY = NoOfTilesY(i);

					for (int y = 0; y < noOfTilesY; y++)
					{
						for (int x = 0; x < noOfTilesX; x++)
						{
							var name = GetSeadragonMapTilePath(i, y, x);

							float xOffset;
							float yOffset;
							var imageWidth = ImageWidth(x, i, noOfTilesX, out xOffset);
							var imageHeight = ImageHeight(y, i, noOfTilesY, out yOffset);

							var tile = new MapTileData(this, x, y, i, name, xOffset, yOffset, imageWidth, imageHeight);

							Console.WriteLine("Tile: {0}:{1}:{2}. xoff:{3}. yoff:{4}. w:{5}. h:{6}", x, y, i, xOffset, yOffset, imageWidth, imageHeight);

							tiles.Add(tile);
						}
					}
				}
				Tiles = tiles.OrderByDescending(x => x.LevelOfDetail).ToList();        
			}
        }

		private float ImageWidth(int x, int levelOfDetail, int noOfTilesX, out float xOffset)
		{
			var levelOfDetailScale = Settings.LevelOfDetailScale(levelOfDetail);

			float width;
			xOffset = -Overlap;
			if (x == noOfTilesX - 1)
			{
				width = Width - ((x * TileWidth) / levelOfDetailScale);
				width = (float)Math.Ceiling(width * levelOfDetailScale) + Overlap;
			}
			else
			{
				width = TileWidth + (2 * Overlap);
			}

			if (x == 0)
			{
				xOffset = 0;
				width -= Overlap;
			}

			return width;
		}

		private float ImageHeight(int y, int levelOfDetail, int noOfTilesY, out float yOffset)
		{
			var levelOfDetailScale = Settings.LevelOfDetailScale(levelOfDetail);
			float height;

			yOffset = -Overlap;
			if (y == noOfTilesY - 1)
			{
				height = Height - ((y * TileHeight) / levelOfDetailScale);
				height = (float)Math.Ceiling(height * levelOfDetailScale) + Overlap;
			}
			else
			{
				height = TileWidth + (2 * Overlap);
			}

			if (y == 0)
			{
				height -= Overlap;
				yOffset = 0;
			}

			return height;
		}
	}
}

