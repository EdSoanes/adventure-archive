using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace BlackDragon.Fx.Extensions
{
    public static class UIImageExtensions
    {
		/// <summary>
		/// resize the image to be contained within a maximum width and height, keeping aspect ratio
		/// </summary>
		/// <returns>The resized image.</returns>
		/// <param name="sourceImage">Source image.</param>
		/// <param name="maxWidth">Max width.</param>
		/// <param name="maxHeight">Max height.</param>
		public static UIImage MaxResizeImage(this UIImage sourceImage, float maxWidth, float maxHeight)
		{
			var sourceSize = sourceImage.Size;
			var maxResizeFactor = Math.Max(maxWidth / sourceSize.Width, maxHeight / sourceSize.Height);
			if (maxResizeFactor > 1) return sourceImage;
			var width = maxResizeFactor * sourceSize.Width;
			var height = maxResizeFactor * sourceSize.Height;
			UIGraphics.BeginImageContext(new SizeF(width, height));
			sourceImage.Draw(new RectangleF(0, 0, width, height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return resultImage;
		}

		/// <summary>
		/// resize the image (without trying to maintain aspect ratio)
		/// </summary>
		/// <returns>The image.</returns>
		/// <param name="sourceImage">Source image.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public static UIImage ResizeImage(this UIImage sourceImage, float width, float height)
		{
			UIGraphics.BeginImageContext(new SizeF(width, height));
			sourceImage.Draw(new RectangleF(0, 0, width, height));
			var resultImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return resultImage;
		}

		/// <summary>
		/// Crops the image without resizing
		/// </summary>
		/// <returns>The image.</returns>
		/// <param name="sourceImage">Source image.</param>
		/// <param name="crop_x">Crop_x.</param>
		/// <param name="crop_y">Crop_y.</param>
		/// <param name="width">Width.</param>
		/// <param name="height">Height.</param>
		public static UIImage CropImage(this UIImage sourceImage, int crop_x, int crop_y, int width, int height)
		{
			var imgSize = sourceImage.Size;
			UIGraphics.BeginImageContext(new SizeF(width, height));
			var context = UIGraphics.GetCurrentContext();
			var clippedRect = new RectangleF(0, 0, width, height);
			context.ClipToRect(clippedRect);
			var drawRect = new RectangleF(-crop_x, -crop_y, imgSize.Width, imgSize.Height);
			sourceImage.Draw(drawRect);
			var modifiedImage = UIGraphics.GetImageFromCurrentImageContext();
			UIGraphics.EndImageContext();
			return modifiedImage;
		}  

		public static Stream ToStream(this UIImage image, string extension)
		{
			NSData data = extension.ToLower() == "png" ? image.AsPNG() : image.AsJPEG();
			if (data != null)
			{
				var stream = data.AsStream();
				return stream;
			}

			return null;
		}

		public static byte[] ToBytes(this UIImage image, string extension)
		{
			NSData data = extension.ToLower() == "png" ? image.AsPNG() : image.AsJPEG();
			if (data != null)
			{
				Byte[] imageData = new Byte[data.Length];
				System.Runtime.InteropServices.Marshal.Copy(data.Bytes, imageData, 0, Convert.ToInt32(data.Length));
				return imageData;
			}

			return null;
		}
    }
}
