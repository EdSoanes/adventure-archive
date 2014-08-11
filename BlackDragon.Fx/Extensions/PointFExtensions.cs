using System;
using System.Drawing;

namespace BlackDragon.Fx.Extensions
{
	public static class PointFExtensions
	{
		public static PointF Offset(this PointF point, float offsetX, float offsetY)
		{
			return new PointF(point.X + offsetX, point.Y + offsetY);
		}

		public static RectangleF OffsetFromCentrePoint(this PointF point, RectangleF rect)
		{
			return new RectangleF(new PointF(point.X - (rect.Width / 2), point.Y - (rect.Height / 2)), rect.Size);
		}

        public static BlackDragon.Core.Entities.Point ToPoint(this PointF point)
        {
            return new BlackDragon.Core.Entities.Point(point.X, point.Y);
        }
	}
}

