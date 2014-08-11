using System;
using System.Drawing;

namespace BlackDragon.Fx.Extensions
{
	public static class SizeFExtensions
	{
		public static SizeF Inflate(this SizeF size, float val)
		{
			var newSize = new SizeF(size.Width + (val * 2), size.Height + (val * 2));
			if (newSize.Width > 0 && newSize.Height > 0)
				return newSize;
			else
				return size;
		}
		
		public static SizeF Inflate(this SizeF size, float width, float height)
		{
			var newSize = new SizeF(size.Width + (width * 2), size.Height + (height * 2));
			if (newSize.Width > 0 && newSize.Height > 0)
				return newSize;
			else
				return size;
		}

        public static SizeF RenderSize(this SizeF size, float scale, float levelOfDetailScale)
        {
            var sizeX = (size.Width * scale) / levelOfDetailScale;
            var sizeY = (size.Height * scale) / levelOfDetailScale;

            return new SizeF(sizeX, sizeY);
        }

        public static SizeF Scale(this SizeF size, float scale)
        {
            return new SizeF(size.Width * scale, size.Height * scale);
        }

        public static PointF Center(this SizeF size)
        {
            return new PointF(size.Width / 2, size.Height / 2);
        }
	}
}

