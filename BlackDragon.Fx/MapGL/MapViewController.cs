using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

using BlackDragon.Core;
using BlackDragon.Core.Entities;
using BlackDragon.Core.IoC;
using BlackDragon.Fx.Extensions;

using MonoTouch.CoreFoundation;
using MonoTouch.Foundation;
using MonoTouch.GLKit;
using MonoTouch.OpenGLES;
using MonoTouch.UIKit;

using OpenTK.Graphics.ES20;

namespace BlackDragon.Fx.MapGL
{
	public class MapViewController : BDGViewController
	{
		MapPosition _oldMapOffset;
		MapPosition _restorePosition;

		EAGLContext _context;

		MapScene _scene = null;

		public Map Map
		{
			get;
			private set;
		}

		public MapViewController()
			: base()
		{
			Initialize();
		}

		private void Initialize()
		{
			_context = new EAGLContext(EAGLRenderingAPI.OpenGLES2, MapTileHelper.UploadContext.ShareGroup);
			EAGLContext.SetCurrentContext(_context);
			GL.BlendFunc(BlendingFactorSrc.One, BlendingFactorDest.OneMinusSrcAlpha);        
		}

		private MapTrackingView TrackingView
		{
			get { return View.Child<MapTrackingView>(); }
		}

		public override void LoadView()
		{
			base.LoadView ();
			var view = new GLKView(UIScreen.MainScreen.Bounds);

			//Initialize the map view. This is the main view 
			view.Context = _context;
			view.DrawableDepthFormat = GLKViewDrawableDepthFormat.Format24;
			view.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

			//Initialize the tracking view. This is an invisible view over the map
			// that will feed the map user inputs for scale and position
			var trackingView = new MapTrackingView(UIScreen.MainScreen.Bounds);
			view.DrawInRect += Draw;
			view.AddSubview(trackingView);

			View = view;
		}

		public override void ViewDidLoad()
		{
			base.ViewDidLoad();

			_scene = new MapScene(this.View.Frame.Size);
			_scene.ClearColor = UIColor.Black;
		}

		public void InitializeMap(Map map)
		{
			Map = map;
			if (Map != null)
			{
				var view = View.Child<MapTitleBarView>();
				if (view != null)
					view.SetTitleAndSubtitle(Map.Title, Map.Subtitle);

				//Set up the tracking view that will receive user interactions and will display map overlay objects
				TrackingView.Setup(Map);
				TrackingView.SetCenterPosition(_restorePosition);
				//TrackingView.ShowDebugFrame();

				var minLevelOfDetail = Settings.GetLevelOfDetail(TrackingView.MinimumZoomScale);

				//Generate the world map tiles based on the minimum level of detail
				Map.CreateMapTiles(minLevelOfDetail);

				//Create all the tiles for the scene.
				foreach (var tile in Map.Tiles)
				{
					var mapTile = new MapTile(tile);
					_scene.Shapes.Add(mapTile);
				}

				//Get all tiles that are at the initial level of detail and load them all before
				// displaying the map
				var mapTiles = _scene.Shapes.Where(x => x is MapTile)
					.Cast<MapTile>()
					.Where(x => x.MapTileData.LevelOfDetail == 2)
					.ToList();

				LoadAllTiles(mapTiles);
			}
		}

		public void Draw (object sender, GLKViewDrawEventArgs args)
		{
			if (_scene != null)
				_scene.Render();
		}

		private void OnRender(object sender, EventArgs e)
		{
			var mapOffset = TrackingView.GetCurrentOffset();

			//If the position of the map has changed then it will need drawing
			if (_oldMapOffset == null || !_oldMapOffset.IsSame(mapOffset))
			{
				_scene.Update(mapOffset);

				((GLKView)View).Display();

				_oldMapOffset = mapOffset;
			}
			else
			{
				_scene.DiscardTiles(mapOffset);
			}

			var tilesToLoad = _scene.GetTilesToLoad(mapOffset);
			if (tilesToLoad.Any())
			{
				LoadTiles(tilesToLoad);
			}

//			//If the position of the map has not changed then we can do some uploading
//			// or deleting of tiles as required
//			else
//			{
//				var tilesToLoad = _scene.GetTilesToLoad(mapOffset);
//				if (tilesToLoad.Any())
//					LoadTiles(tilesToLoad);
//
//				_scene.DiscardTiles(mapOffset);
//			}

		}

		private void LoadAllTiles(List<MapTile> tiles)
		{
			tiles.ForEach(x => x.SetDownloadingState());

			//Request all the images for the lowest level of detail. Start displaying the map once they have
			// all been downloaded (or loaded from the cache).
			DC.Get<IFileAccessService>().RequestMultiple(tiles.Select(x => x.MapTileData.ImageUrl).ToList(), (fileCacheEntries) =>
			{
				//Upload the images to OpenGL and create their shapes for the map scene
				InvokeOnMainThread(() =>
				{
					//Pass on the images to the owning tile.
					foreach (var kvp in fileCacheEntries)
					{
						var tile = tiles.FirstOrDefault(x => x.MapTileData.ImageUrl == kvp.Key);
						if (tile != null)
						{
							var img = kvp.Value.GetData<byte[]>().ToImage();
							UploadImageToOpenGLAsync(img, tile);
						}
								//UploadImageToOpenGLSync(kvp.Value, tile);
					}

					//This will force the view to redisplay
					ShowScene();
				});
			});		
		}

		private void LoadTiles(List<MapTile> tiles)
		{
			foreach (var tile in tiles)
			{
				tile.SetDownloadingState();
				//Begin a download from the remote source
				DC.Get<IFileAccessService>().Request(tile.MapTileData.ImageUrl, (fileCacheEntry) =>
				{
					InvokeOnMainThread(() =>
					{
						var img = fileCacheEntry.GetData<byte[]>().ToImage();
						if (img != null)
							UploadImageToOpenGLAsync(img, tile);
					});
				});
			}
		}

		void UploadImageToOpenGLSync(UIImage img, MapTile tile)
		{
			NSDictionary options = NSDictionary.FromObjectAndKey(NSNumber.FromBoolean(false), GLKTextureLoader.OriginBottomLeft);
			NSError error;
			var texture = GLKTextureLoader.FromImage(img.CGImage, options, out error);

			img.Dispose();

			if (error == null)
				tile.SetTexture(texture);
			else
				Console.WriteLine("Error uploading texture to opengl. Error {0}", error);
		}

		void UploadImageToOpenGLAsync(UIImage img, MapTile tile)
		{
			NSDictionary options = NSDictionary.FromObjectAndKey(NSNumber.FromBoolean(false), GLKTextureLoader.OriginBottomLeft);
			var textureLoader = new GLKTextureLoader(MapTileHelper.UploadContext.ShareGroup);

			//Async upload the texture to opengl
			var cgImage = img.CGImage;
			textureLoader.BeginTextureLoad(cgImage, options, DispatchQueue.MainQueue, (textureInfo, error) => 
			{
				cgImage.Dispose();
				img.Dispose();
				tile.SetTexture(textureInfo);
				ShowScene();
			});
		}

		public override void WillRotate(UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			HideScene();
			base.WillRotate(toInterfaceOrientation, duration);
		}

		public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate(fromInterfaceOrientation);
			ResetSceneAspect();
		}

		public override void ViewWillAppear(bool animated)
		{
			Console.WriteLine("ViewWillAppear");
			base.ViewWillAppear(animated);
			if (_scene != null)
			{
				ResetSceneAspect();

				BDGDisplayLink.Link.RegisterRunHandler(OnRender);
				BDGDisplayLink.Link.RequestStart();
			}
		}

		public override void ViewDidAppear(bool animated)
		{
			Console.WriteLine("ViewDidAppear");
			base.ViewDidAppear(animated);
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			HideScene();
		}

		public override void ViewDidDisappear(bool animated)
		{
			Console.WriteLine("ViewDidDisappear");

			base.ViewDidDisappear(animated);
			EndScene();
		}

		public override void DidReceiveMemoryWarning()
		{
			Console.WriteLine("DidReceiveMemoryWarning");

			base.DidReceiveMemoryWarning();
			ReduceSceneMemory();
		}

		void HideScene()
		{
			if (_scene != null)
			{
				_scene.Hide();
				_oldMapOffset = null;
			}
			_restorePosition = TrackingView.GetCenterPosition();
		}

		void ShowScene()
		{
			if (_scene != null)
			{
				_scene.Show();
				_oldMapOffset = null;
			}
		}

		void ReduceSceneMemory()
		{
			if (_scene != null)
			{
				var mapPosition = TrackingView.GetCenterPosition();
				_scene.DiscardAllTiles(mapPosition);
			}
		}

		void ResetSceneAspect()
		{
			if (_scene != null)
			{
				var h = UIScreen.MainScreen.Bounds.Height;
				var w = UIScreen.MainScreen.Bounds.Width;
				if (!this.View.IsPortrait())
				{
					_scene.ScreenSize = new SizeF(Math.Max(w, h), Math.Min(w, h));
					_scene.SetBackground("Images/bg-h.jpg");
				}
				else
				{
					_scene.ScreenSize = new SizeF(Math.Min(w, h), Math.Max(w, h));
					_scene.SetBackground("Images/bg-v.jpg");
				}
				TrackingView.SetCenterPosition(_restorePosition);
				_oldMapOffset = null;
				_scene.Show();
			}
		}

		void EndScene()
		{
			//BDGDisplayLink.Link.Stop();
			BDGDisplayLink.Link.UnregisterRunHandler(OnRender);

			if (_scene != null)
			{
				_scene.EndScene();
				((GLKView)View).DeleteDrawable();
			}
		}
	} 
}

