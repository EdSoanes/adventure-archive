using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using BlackDragon.Core;

namespace BlackDragon.Core.DeepZoom
{
	public abstract class SeadragonTileSource
    {
		public const string LowResFolder = "LowRes";

		protected IFileAccessService FileAccessService { get; private set; }
		protected ILogService Log { get; private set; }

		private bool _isInitialized = false;
		private object _isInitializedLock = new object();

        public event EventHandler Initialized;
		public event EventHandler TilesInitialized;
		public event EventHandler<SeadragonTileDownloadedEventArgs> TileDownloaded;
		public event EventHandler<SeadragonOverlaySelectedEventArgs> OverlaySelected;

		public string BaseUrl
        {
            get;
            private set;
        }

        public SeadragonFileDescriptor Dzi
        {
            get;
            private set;
        }

		public List<SeadragonOverlay> Overlays
		{
			get;
			private set;
		}

		public bool HiRes
		{
			get;
			private set;
		}

		public SeadragonTileSource(IFileAccessService fileAccessService, ILogService log, bool hiRes = false)
        {
			FileAccessService = fileAccessService;
			Log = log;

            Dzi = new SeadragonFileDescriptor();
			HiRes = hiRes;
			Overlays = new List<SeadragonOverlay>();
        }

		public Action<FileCacheEntry, object> TileProcessAction
		{
			get;
			set;
		}

		protected abstract void GenerateTiles(object image, SeadragonTileIndex index);
		protected abstract object GetImage(byte[] imageData);

		public bool Initialize(string url)
        {
            Uri uri = new Uri(url);
            if (uri.Segments.Any() && uri.Segments.Last().ToLower().EndsWith(".xml"))
            {
				if (string.IsNullOrEmpty(Dzi.Url) || url.ToLower() != Dzi.Url.ToLower())
				{
					SetBaseUrl(uri);

					//Begin downloading the xml descriptor for the deep zoom tiled image
					FileAccessService.Request(uri.OriginalString, (fileCacheEntry) =>
					{
						try
						{
							Dzi.Url = fileCacheEntry.Url;
							var xml = fileCacheEntry.GetData<string>();
							if (!string.IsNullOrEmpty(xml))
							{
								Dzi.FromXml(xml);

								if (Initialized != null)
									Initialized.Invoke(this, new EventArgs());
							}
						}
						catch (Exception ex)
						{
							Log.Error("SeadragonTileSource.Initialize", ex);
						}
					});

					return false;
				}

				return true;
            }

			return false;
        }

		public void InitializeTiles(float minZoomScale)
		{
			var levelOfDetail = LevelOfDetailForScale(minZoomScale);

			//Download the tiles for the lowest levels of detail before raising the initialized event
			// This will make sure there is always something to display.

			var urls = new List<string>();
			var indexes = new List<SeadragonTileIndex>();

			for (int row = 0; row < Dzi.NoOfTilesY(levelOfDetail); row++)
				for (int col = 0; col < Dzi.NoOfTilesX(levelOfDetail); col++)
				{
					var tileUrl = TileUrl(col, row, minZoomScale);
					if (!string.IsNullOrEmpty(tileUrl))
					{
						urls.Add(tileUrl);
						indexes.Add(new SeadragonTileIndex(col, row, levelOfDetail));
					}
				}

			if (urls.Any())
			{
				FileAccessService.RequestMultiple(urls, (files) =>
				{
					//Next pre-name and crop/scale up the lowest res images and save them in the cache so we always have
					// a (low-res) tile set. As the full sized images are downloaded, the low-res tiles can be replaced.
					foreach (var fileCacheEntry in files.Values)
					{
						var idx = urls.IndexOf(fileCacheEntry.Url);
						var index = indexes[idx];
						var imageData = fileCacheEntry.GetData<byte[]>();
							var img = GetImage(imageData);

						GenerateTiles(img, index);
					}

					//Raise the initialized event and begin downloading the other images.
					lock (_isInitializedLock)
						_isInitialized = true;

					//To ensure the system knows whether a file has been properly downloaded, we need to place the temporary low-res
					// images in a separate folder in the cache. They can be deleted as and when the real high res images become available.
					if (TilesInitialized != null)
						TilesInitialized.Invoke(this, new EventArgs());
				});
			} 
		}

		public T GetTile<T>(int col, int row, float scale) where T : class, IDisposable
        {
			var tileUrl = TileUrl(col, row, scale);

			T img = default(T);

			if (!string.IsNullOrEmpty(tileUrl))
			{
				bool isInitialized = false;
				lock (_isInitializedLock)
					isInitialized = _isInitialized;

				if (isInitialized)
				{
					var imageData = FileAccessService.ReadFromCache(tileUrl) as byte[];
					img = GetImage(imageData) as T;

					if (img == null)
					{
						imageData = FileAccessService.ReadFromCache(tileUrl, LowResFolder) as byte[];
						img = GetImage(imageData) as T;

						FileAccessService.RequestSingle(tileUrl, (fileCacheEntry) =>
						{
							FileAccessService.DeleteFromCache(fileCacheEntry.Url, LowResFolder);
							var tileLevelOfDetail = LevelOfDetailForScale(scale);
							var index = new SeadragonTileIndex(col, row, tileLevelOfDetail);
							
							if (tileLevelOfDetail < Dzi.Levels)
							{
								using (var tile = GetImage(fileCacheEntry.GetData<byte[]>()) as T)
									GenerateTiles(tile, index);
							}

							if (TileDownloaded != null)
								TileDownloaded.Invoke(this, new SeadragonTileDownloadedEventArgs(index));
						});
					}
				}
			}

			return img;
        }

		public void SelectOverlay(SeadragonOverlay selectedOverlay)
		{
			foreach (var overlay in Overlays)
				overlay.IsSelected = selectedOverlay == overlay;

			if (Overlays.Contains(selectedOverlay) && OverlaySelected != null)
				OverlaySelected.Invoke(this, new SeadragonOverlaySelectedEventArgs(selectedOverlay));
		}

		protected int LevelOfDetailForScale(float scale)
		{
			int levelOfDetail = (int)Math.Round((Dzi.Levels - (1 / scale) + 1));
			if (levelOfDetail > Dzi.Levels)
				levelOfDetail = Dzi.Levels;

			return levelOfDetail;
		}

		protected float ScaleForLevelOfDetail(int levelOfDetail)
		{
			float scale = (float)(1 / Math.Pow(2, (Dzi.Levels - levelOfDetail)));
			return scale;
		}

		public string TileUrl(int col, int row, float scale)
        {
			var levelOfDetail = LevelOfDetailForScale(scale);
            if (!IsValidTile(col, row, levelOfDetail))
                return string.Empty;

			var tileUrl = String.Format("{0}{1}/{2}_{3}.{4}", BaseUrl, levelOfDetail, col, row, Dzi.Format);

            return tileUrl;
        }

        public bool IsValidTile(int col, int row, int levelOfDetail)
        {
			if (levelOfDetail > Dzi.Levels)
                return false;

            if (col < 0 || col >= Dzi.NoOfTilesX(levelOfDetail))
                return false;

            if (row < 0 || row >= Dzi.NoOfTilesY(levelOfDetail))
                return false;

            return true;
        }

        private void SetBaseUrl(Uri uri)
        {
            //Calculate the base url from the Uri object
            BaseUrl = string.Format("{0}://{1}", uri.Scheme, uri.Authority);
            foreach (var seg in uri.Segments.Where(x => x != uri.Segments.Last()))
                BaseUrl += seg;
            BaseUrl += uri.Segments.Last().Substring(0, uri.Segments.Last().Length - 4) + "_files/";
        }
    }
}
