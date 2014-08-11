using System;

namespace BlackDragon.Core.Entities
{
    public class MapTileData
    {
        public Map Map 
        { 
            get; 
            private set; 
        }

        public int X
        {
            get;
            private set;
        }

        public int Y
        {
            get;
            private set;
        }

		public int Width
		{
			get;
			private set;
		}

		public int Height
		{
			get;
			private set;
		}

        public int LevelOfDetail
        {
            get;
            private set;
        }

        public string ImageUrl
        {
            get;
            private set;
        }

		public float ImageOffsetX
		{
			get;
			private set;
		}

		public float ImageOffsetY
		{
			get;
			private set;
		}

		public float ImageWidth
		{
			get;
			private set;
		}

		public float ImageHeight
		{
			get;
			private set;
		}

        private object _lock = new object();
        private object _imageData = null;
        public object ImageData
        {
            get 
            { 
                lock (_lock)
                    return _imageData; 
            }
            set
            {
                lock (_lock)
                    _imageData = value;
            }
        }

		public MapTileData(Map map, int x, int y, int levelOfDetail, string imageUrl, float imageOffsetX, float imageOffsetY, float imageWidth, float imageHeight)
        {
            Map = map;
            X = x;
            Y = y;
            LevelOfDetail = levelOfDetail;
            ImageUrl = imageUrl;
			ImageOffsetX = imageOffsetX;
			ImageOffsetY = imageOffsetY;
			ImageWidth = imageWidth;
			ImageHeight = imageHeight;
        }
    }
}

