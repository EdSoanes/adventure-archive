using System;
using MonoTouch.UIKit;
using MonoTouch.Foundation;

namespace BlackDragon.Fx
{
	public static class ByteArrayExtensions
    {
		public static UIImage ToImage(this byte[] imageData)
		{
			if (imageData != null && imageData.Length > 0)
			{
				var nsData = NSData.FromArray(imageData);
				var image = UIImage.LoadFromData(nsData);
				return image;
			}

			return null;
		}
    }
}

