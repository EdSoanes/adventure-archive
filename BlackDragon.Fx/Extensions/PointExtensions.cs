using System;
using System.Drawing;

namespace BlackDragon.Fx.Extensions
{
    public static class PointExtensions
    {
        public static PointF ToPointF(this BlackDragon.Core.Entities.Point point)
        {
            return new PointF(point.X, point.Y);
        }
    }
}

