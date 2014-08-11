using System;
using System.Drawing;

namespace BlackDragon.Fx
{
    public class MapPosition
    {
		private PointF _position;
		private float _scale;
		private float _minimumScale;

		public MapPosition(PointF position, float scale, float minimumScale)
        {
			Set(position, scale, minimumScale);
        }

		public float X
		{
			get { return _position.X; }
		}

		public float Y
		{
			get { return _position.Y; }
		}

		public float Scale
		{
			get { return _scale; }
		}

		public float MinimumScale
		{
			get { return _minimumScale; }
		}

		public void Set(PointF position, float scale, float minimumScale)
		{
			_position = position;
			_scale = scale;
			_minimumScale = minimumScale;
		}

		public bool IsSame(MapPosition position)
		{
			return _scale == position.Scale && X == position.X && Y == position.Y;
		}
    }
}

