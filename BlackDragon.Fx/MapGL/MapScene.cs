using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

using BlackDragon.Core;

using MonoTouch.UIKit;

using OpenTK;
using OpenTK.Graphics.ES20;

namespace BlackDragon.Fx.MapGL
{
    public class MapScene
    {
        Matrix4 _projectionMatrix;
        Vector4 _clearColor = new Vector4(0, 0, 0, 1);

        MapBackground _background = null;
		bool _mapVisible = false;

        public UIColor ClearColor
        {
            get { return new UIColor(_clearColor.X, _clearColor.Y, _clearColor.Z, _clearColor.W); }
            set
            {
                value.GetRGBA(out _clearColor.X, out _clearColor.Y, out _clearColor.Z, out _clearColor.W);
            }
        }

        SizeF _screenSize;
        public SizeF ScreenSize
        {
            get { return _screenSize; }
            set
            {
                _screenSize = value;
                _projectionMatrix = GetProjectionMatrix();
            }
        }

        private List<MapShape> _shapes = new List<MapShape>();
        public List<MapShape> Shapes
        {
            get { return _shapes; }
        }

        public MapScene(SizeF screenSize)
        {
            _screenSize = screenSize;
            _projectionMatrix = GetProjectionMatrix();
        }

        public void SetBackground(string fileName)
        {
            var old = _background;

            var shape = new MapBackground(fileName);
            _background = shape;

            if (old != null)
                old.DiscardTexture();
        }

        public void Hide()
        {
            _mapVisible = false;
        }

        public void Show()
        {
            _mapVisible = true;
        }

		public void Update(MapPosition position)
        {
            var screen = new RectangleF(new PointF(0, 0), ScreenSize);
			var currentLevelOfDetail = Settings.GetLevelOfDetail(position.Scale);
			var minimumLevelOfDetail = Settings.GetLevelOfDetail(position.MinimumScale);

            //Loop through the map tiles and calculate the new positions for each
            var tilesToProcess = _shapes.Where(x => x is MapTile).Cast<MapTile>().ToList();
			tilesToProcess.ForEach(x => 
			{
				x.CalculateRenderFrame(new PointF(position.X, position.Y), position.Scale);
				x.IsToBeRendered = false;
			});

            //Loop through them again and decide which are to be displayed
            foreach (var shape in _shapes.Where(x => x is MapTile).Cast<MapTile>())
            {
                if (!tilesToProcess.Contains(shape))
                    continue;

                tilesToProcess.Remove(shape);

				//Figure out if the tile is to be rendered. If not then discard the image too
				// if not the minimum level of detail. We always keep the minimum level of detail
				// tiles so there is always something to display
				if (!shape.RenderFrame.IntersectsWith(screen))
				{
					//Keep the minimum detail level tiles alive
					if (shape.MapTileData.LevelOfDetail == minimumLevelOfDetail)
						shape.LRUTime = DateTime.Now;
				}
                else
                {
                    //If the tile is on another level of detail then don't show it
					if (shape.MapTileData.LevelOfDetail == currentLevelOfDetail)
                    {
                        //If the tile is to be displayed and there is and image then great!
						if (shape.State == MapShape.StateType.ReadyToRender)
						{
							shape.LRUTime = DateTime.Now;
							shape.IsToBeRendered = true;
						}
                        else
                        {
							//Otherwise, find an image with a lower level of detail to display in it's place
                            var lowerLevelOfDetail = currentLevelOfDetail - 1;
                            var newX = (int)Math.Floor((float)shape.MapTileData.X / 2);
                            var newY = (int)Math.Floor((float)shape.MapTileData.Y / 2);
							while (lowerLevelOfDetail >= minimumLevelOfDetail)
                            {
                                var altTile = tilesToProcess.FirstOrDefault(x => x.MapTileData.X == newX && x.MapTileData.Y == newY && x.MapTileData.LevelOfDetail == lowerLevelOfDetail && x.State == MapShape.StateType.ReadyToRender);
                                if (altTile != null)
                                {
									altTile.IsToBeRendered = true;

                                    //Make sure we don't process it again
                                    tilesToProcess.Remove(altTile);

                                    break;
                                }
                                else
                                {
                                    lowerLevelOfDetail--;
                                    newX = (int)Math.Floor((float)newX / 2);
                                    newY = (int)Math.Floor((float)newY / 2);
                                }
                            }
                        }
                    }
                }
            }
        }

		public List<MapTile> GetTilesToLoad(MapPosition position)
		{
			var tilesToLoad = new List<MapTile>();

			var screen = new RectangleF(new PointF(0, 0), ScreenSize);
			var currentLevelOfDetail = Settings.GetLevelOfDetail(position.Scale);
			var minimumLevelOfDetail = Settings.GetLevelOfDetail(position.MinimumScale);

			//Loop through them again and decide which are to be displayed
			foreach (var shape in _shapes.Where(x => x is MapTile).Cast<MapTile>())
			{
				if (shape.RenderFrame.IntersectsWith(screen) && shape.MapTileData.LevelOfDetail == currentLevelOfDetail && shape.State == MapShape.StateType.Initialized)
					tilesToLoad.Add(shape);
			}

			return tilesToLoad;
		}

		public void DiscardTiles(MapPosition position)
		{
			var screen = new RectangleF(new PointF(0, 0), ScreenSize);
			var currentLevelOfDetail = Settings.GetLevelOfDetail(position.Scale);
			var minimumLevelOfDetail = Settings.GetLevelOfDetail(position.MinimumScale);

			var uploadedCount = _shapes.Count(x => x.State == MapShape.StateType.ReadyToRender);
			var noToDiscard = uploadedCount - 50;
			if (noToDiscard > 0)
			{
				var ids = _shapes.Where(x => x is MapTile)
					.Cast<MapTile>()
					.Where(x => x.Discardable(minimumLevelOfDetail, currentLevelOfDetail, screen))
					.OrderBy(x => x.LRUTime)
					.Take(noToDiscard)
					.Select(x => x.GetTextureToDiscard())
					.Where(x => x > 0).Distinct().ToArray();

				if (ids.Any())
				{
					GL.Flush();
					GL.DeleteTextures(ids.Length, ids);

					var err = GL.GetError();
					if (err != 0)
						Console.WriteLine("DiscardTextures: Failed with error {0}", err);
					else
						Console.WriteLine("Discarded {0} textures", ids.Length);
				}
			}
		}

		public void DiscardAllTiles(MapPosition position)
		{
			var screen = new RectangleF(new PointF(0, 0), ScreenSize);
			var minimumLevelOfDetail = Settings.GetLevelOfDetail(position.MinimumScale);

			var ids = _shapes.Where(x => x is MapTile)
				.Cast<MapTile>()
				.Where(x => x.Discardable(minimumLevelOfDetail, screen))
				.Select(x => x.GetTextureToDiscard())
				.Where(x => x > 0).Distinct().ToArray();

			if (ids.Any())
			{
				GL.Flush();
				GL.DeleteTextures(ids.Length, ids);

				var err = GL.GetError();
				if (err != 0)
					Console.WriteLine("DiscardTextures: Failed with error {0}", err);
				else
					Console.WriteLine("Discarded {0} textures", ids.Length);
			}
		}

        public void Render()
        {
            GL.Enable(EnableCap.Blend);

            GL.ClearColor (_clearColor.X, _clearColor.Y, _clearColor.Z, _clearColor.W);
            GL.Clear((ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit));

			if (_background != null)
            	_background.Render(_projectionMatrix);

            if (_mapVisible)
            {
                foreach (var shape in _shapes.Where(x => x.IsToBeRendered).Reverse())
                    shape.Render(_projectionMatrix);
            }

            GL.Disable(EnableCap.Blend);
        }

        public void DiscardUnusedTiles()
        {
            Shapes.Where(x => !x.IsToBeRendered)
                  .ToList()
                  .ForEach(x => x.DiscardTexture());
        }

        public void EndScene()
        {
            if (_background != null)
                _background.DiscardTexture();

            Shapes.ForEach(x => x.DiscardTexture());
            Shapes.Clear();

            GL.Flush();
        }

        private Matrix4 GetProjectionMatrix()
        {
            return Matrix4.CreateOrthographicOffCenter(0, _screenSize.Width, _screenSize.Height, 0, 1, -1);
        }
    }
}

