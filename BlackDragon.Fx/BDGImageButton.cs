using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using BlackDragon.Core.IoC;
using BlackDragon.Core;
using BlackDragon.Fx.Extensions;
using System.Drawing;

namespace BlackDragon.Fx
{
    public class BDGImageButton : UIButton
    {
        private object _lockObject = new object();
        private bool _disposed = false;

        public BDGImageButton(RectangleF frame, UIImage img)
            : base(frame)
        {
            Initialize(img);
        }

        public BDGImageButton(RectangleF frame, UIImage placeholderImage, string remoteImageUrl)
            : base(frame)
        {
            lock (_lockObject)
                Initialize(placeholderImage);

            LoadImageAsync(remoteImageUrl);
        }

        public BDGImageButton(UIImage img)
        {
            Initialize(img);
        }

        public BDGImageButton(UIImage placeholderImage, string remoteImageUrl)
        {
            lock (_lockObject)
                Initialize(placeholderImage);

            LoadImageAsync(remoteImageUrl);
        }

        private void Initialize(UIImage img)
        {
            this.Frame = this.Frame.NewSize(img.Size);
			this.SetBackgroundImage(img, UIControlState.Normal);
        }

        public void LoadImageAsync(string remoteImageUrl)
        {
            if (!string.IsNullOrEmpty(remoteImageUrl))
            {
				DC.Get<IFileAccessService>().Request(remoteImageUrl, (fileCacheEntry) =>
                {
                    BeginInvokeOnMainThread(() =>
                    {
                        lock (_lockObject)
                        {
                            if (!_disposed)
							{
								var img = fileCacheEntry.GetData<byte[]>().ToImage();
								this.SetBackgroundImage(img, UIControlState.Normal);
								OnAsyncImageLoaded();
							}
                        }
                    });
                });
            }
        }

		protected virtual void OnAsyncImageLoaded()
		{
		}
    }
}