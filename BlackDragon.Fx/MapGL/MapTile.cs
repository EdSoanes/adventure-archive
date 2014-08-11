using System;
using System.Drawing;
using BlackDragon.Core;
using BlackDragon.Core.Entities;
using BlackDragon.Core.IoC;
using BlackDragon.Fx.Extensions;
using MonoTouch.UIKit;
using OpenTK;

namespace BlackDragon.Fx.MapGL
{
    public class MapTile : MapShape
    {
        public MapTileData MapTileData
        {
            get;
            private set;
        }

        public RectangleF RenderFrame
        {
            get;
            private set;
        }

        public float RenderScale
        {
            get;
            private set;
        }

        /// <summary>
        /// Default constructor creates a 256x256 sized tile
        /// </summary>
        public MapTile(MapTileData mapTileData)
            : base()
        {
            MapTileData = mapTileData;

            SetShapeVertices();
            SetTextureVertices();
        }

		private void SetShapeVertices()
        {
			_shapeVertices.Add(new Vector2(0, 0));
			_shapeVertices.Add(new Vector2(MapTileData.ImageWidth, 0));
			_shapeVertices.Add(new Vector2(0, MapTileData.ImageHeight));
			_shapeVertices.Add(new Vector2(MapTileData.ImageWidth, MapTileData.ImageHeight));
        }

        private void SetTextureVertices()
        {
            _textureVertices.Add(new Vector2(0, 0));
            _textureVertices.Add(new Vector2(1, 0));
            _textureVertices.Add(new Vector2(0, 1));
            _textureVertices.Add(new Vector2(1, 1));
        }

		public void CalculateRenderFrame(PointF offset, float scale)
        {
            var levelOfDetailScale = Settings.LevelOfDetailScale(MapTileData.LevelOfDetail);
            Scale = scale / levelOfDetailScale;

//			var posX = (MapTileData.X * (Map.TileWidth * Scale)) - offset.X + (MapTileData.ImageOffsetX * Scale);
//			var posY = (MapTileData.Y * (Map.TileHeight * Scale)) - offset.Y + (MapTileData.ImageOffsetY * Scale);
			var posX = (((MapTileData.X * Map.TileWidth) + MapTileData.ImageOffsetX) * Scale) - offset.X;
			var posY = (((MapTileData.Y * Map.TileHeight) + MapTileData.ImageOffsetY) * Scale) - offset.Y;

			Position = new PointF(posX, posY);
			RenderFrame = new RectangleF(Position, new SizeF(MapTileData.ImageWidth * Scale, MapTileData.ImageHeight * Scale));
        }

		public bool Discardable(int minLevelOfDetail, int currLevelOfDetail, RectangleF screen)
		{
			return State == StateType.ReadyToRender &&
				   (this.MapTileData.LevelOfDetail != minLevelOfDetail && this.MapTileData.LevelOfDetail != currLevelOfDetail) ||
				   (this.MapTileData.LevelOfDetail == currLevelOfDetail && !this.RenderFrame.IntersectsWith(screen));
		}

		public bool Discardable(int minLevelOfDetail, RectangleF screen)
		{
			return 
				State == StateType.ReadyToRender &&
				this.MapTileData.LevelOfDetail != minLevelOfDetail &&
				!this.RenderFrame.IntersectsWith(screen);
		}
    }
}

