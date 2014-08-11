using System;
using System.Drawing;
using OpenTK;

namespace BlackDragon.Fx.MapGL
{
    public class MapBackground : MapShape
    {
        public MapBackground(string fileName)
        {
            SetTextureVertices();
            LoadImage(fileName);

            if (_texture != null)
                SetShapeVertices(new SizeF(_texture.Width, _texture.Height));
        }

        /// <summary>
        /// Constructor for tile of arbitrary size
        /// </summary>
        /// <param name="tileSize">Tile size.</param>
        public MapBackground(SizeF tileSize)
            : base()
        {
            SetShapeVertices(tileSize);
            SetTextureVertices();
        }

        private void SetShapeVertices(SizeF size)
        {
            _shapeVertices.Add(new Vector2(0, 0));
            _shapeVertices.Add(new Vector2(size.Width, 0));
            _shapeVertices.Add(new Vector2(0, size.Height));
            _shapeVertices.Add(new Vector2(size.Width, size.Height));
        }

        private void SetTextureVertices()
        {
            _textureVertices.Add(new Vector2(0, 0));
            _textureVertices.Add(new Vector2(1, 0));
            _textureVertices.Add(new Vector2(0, 1));
            _textureVertices.Add(new Vector2(1, 1));
        }
    }
}

