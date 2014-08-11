using System;
using System.Drawing;

namespace BlackDragon.Fx.Extensions
{
	public static class RectangleFExtensions
	{
		public static RectangleF NewX(this RectangleF rect, float newX)
		{
			return new RectangleF(newX, rect.Y, rect.Width, rect.Height);
		}

		public static RectangleF NewY(this RectangleF rect, float newY)
		{
			return new RectangleF(rect.X, newY, rect.Width, rect.Height);
		}
		
		public static RectangleF NewWidth(this RectangleF rect, float width)
		{
			return new RectangleF(rect.X, rect.Y, width, rect.Height);
		}

		public static RectangleF NewHeight(this RectangleF rect, float height)
		{
			return new RectangleF(rect.X, rect.Y, rect.Width, height);
		}

        public static RectangleF NewSize(this RectangleF rect, SizeF size)
        {
            return new RectangleF(rect.Location, size);
        }

		public static PointF Center(this RectangleF rect)
		{
			return new PointF(rect.X + (rect.Width / 2), rect.Y + (rect.Height / 2));
		}
		
		public static RectangleF Deflate(this RectangleF rect, float val)
		{
			if (rect != RectangleF.Empty && rect.Width > val * 2 && rect.Height > val * 2)
			{
				var newRect = new RectangleF(rect.X + val, rect.Y + val, rect.Width - (2 * val), rect.Height - (2 * val));
				return newRect;
			}
			
			return RectangleF.Empty;
		}
		
        public static RectangleF DeflateHeight(this RectangleF rect, float val)
        {
            if (rect != RectangleF.Empty && rect.Height > val * 2)
            {
                var newRect = new RectangleF(rect.X, rect.Y + val, rect.Width, rect.Height - (2 * val));
                return newRect;
            }

            return RectangleF.Empty;
        }

        public static RectangleF DeflateWidth(this RectangleF rect, float val)
        {
            if (rect != RectangleF.Empty && rect.Width > val * 2 )
            {
                var newRect = new RectangleF(rect.X + val, rect.Y, rect.Width - (2 * val), rect.Height);
                return newRect;
            }

            return RectangleF.Empty;
        }

		public static RectangleF Inflate(this RectangleF rect, float val)
		{
			if (rect != RectangleF.Empty)
			{
				var newRect = new RectangleF(rect.X - val, rect.Y - val, rect.Width + (2 * val), rect.Height + (2 * val));
				return newRect;
			}
			
			return RectangleF.Empty;
		}

        public static RectangleF MoveTo(this RectangleF rect, float x, float y)
        {
            return new RectangleF(x, y, rect.Width, rect.Height);
        }

        public static RectangleF MoveToX(this RectangleF rect, float x)
        {
            return new RectangleF(x, rect.Y, rect.Width, rect.Height);
        }

        public static RectangleF MoveToY(this RectangleF rect, float y)
        {
            return new RectangleF(rect.X, y, rect.Width, rect.Height);
        }

        public static RectangleF Move(this RectangleF rect, float x, float y)
        {
            return new RectangleF(new PointF(rect.X + x, rect.Y + y), rect.Size);
        }

		public static RectangleF Move(this RectangleF rect, PointF position)
		{
			return rect.Move(position.X, position.Y);
		}

		public static RectangleF MoveX(this RectangleF rect, float x, float? minX = null, float? maxX = null)
		{
			if (minX != null && rect.X + x < minX)
				return new RectangleF(minX.Value, rect.Y, rect.Width, rect.Height);

			if (maxX != null && rect.X + x > maxX)
				return new RectangleF(maxX.Value, rect.Y, rect.Width, rect.Height);

			return rect.Move(x, 0);
		}

		public static RectangleF MoveY(this RectangleF rect, float y)
		{
			return rect.Move(0, y);
		}

		public static RectangleF Shrink(this RectangleF rect, float width, float height)
		{
			return new RectangleF(rect.X, rect.Y, rect.Width - width, rect.Height - height);
		}

		public static RectangleF ExpandWidth(this RectangleF rect, float width, float? minWidth = null, float? maxWidth = null)
		{
			if (minWidth != null && rect.Width + width < minWidth.Value)
				return new RectangleF(rect.X, rect.Y, minWidth.Value, rect.Height);

			if (maxWidth != null && rect.Width + width > maxWidth.Value)
				return new RectangleF(rect.X, rect.Y, maxWidth.Value, rect.Height);

			return rect.Shrink(-width, 0);
		}

		public static RectangleF ShrinkWidth(this RectangleF rect, float width)
		{
			return rect.Shrink(width, 0);
		}

		public static RectangleF ShrinkHeight(this RectangleF rect, float height)
		{
			return rect.Shrink(0, height);
		}

        public static RectangleF CenterOnX(this RectangleF rect, float x)
        {
            var newX = x - (rect.Width / 2);

            return new RectangleF(newX, rect.Y, rect.Width, rect.Height);
        }

        public static RectangleF CenterOnY(this RectangleF rect, float y)
        {
            var newY = y - (rect.Width / 2);

            return new RectangleF(rect.X, newY, rect.Width, rect.Height);
        }

        public static RectangleF CenterOn(this RectangleF rect, PointF centerPoint)
        {
            var x = centerPoint.X - (rect.Width / 2);
            var y = centerPoint.Y - (rect.Height / 2);

            return new RectangleF(x, y, rect.Width, rect.Height);
        }

        public static RectangleF Scale(this RectangleF rect, float scale)
        {
            return new RectangleF(rect.X, rect.Y, rect.Width * scale, rect.Height * scale);
        }
	}
}

