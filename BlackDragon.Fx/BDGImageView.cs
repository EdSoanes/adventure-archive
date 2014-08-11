using System.Drawing;

using BlackDragon.Core;

using MonoTouch.UIKit;
using BlackDragon.Core.IoC;

namespace BlackDragon.Fx
{
    public class BDGImageView : UIImageView
    {
        private object _lockObject = new object();
        private bool _disposed = false;

        public BDGImageView(RectangleF frame, string remoteImageUrl)
            : base(frame)
        {
            LoadImageAsync(remoteImageUrl);
        }

        public BDGImageView(UIImage placeholderImage, string remoteImageUrl) 
            : base(placeholderImage)
        {
            LoadImageAsync(remoteImageUrl);
        }

        private void LoadImageAsync(string remoteImageUrl)
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
                                this.Image = img;
							}
                        }
                    });            
                });
            }
        }

        protected override void Dispose(bool disposing)
        {
            lock (_lockObject)
            {
                _disposed = true;
                base.Dispose(disposing);
            }
        }
    }
}
