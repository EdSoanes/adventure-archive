using System;
using System.Drawing;
using System.Linq;

using MonoTouch.CoreFoundation;
using MonoTouch.UIKit;
using MonoTouch.Foundation;
using BlackDragon.Fx;
using BlackDragon.Core;
using Newtonsoft.Json;
using BlackDragon.Core.Entities;
using BlackDragon.Fx.MapGL;
using BlackDragon.Fx.Extensions;
using System.Collections.Generic;
using BlackDragon.Archive.Views;
using BlackDragon.Fx.Gestures;
using BlackDragon.Fx.DeepZoom;
using BlackDragon.Core.DeepZoom;

namespace BlackDragon.Archive
{
    [Register("WorldViewController")]
	public class WorldViewController : BDGViewController
    {
		private enum Elements
		{
			Reference = 1,
            Content = 2,
            NavigationMenu = 3
		}

        private readonly IWorldService _worldService;
		private readonly IFileAccessService _fileService;
		private readonly ILogService _log;

		private SeadragonTileSourceiOS _tileSource = null;

        private TheArchiveButton ArchiveButton
        {
            get
            {
                return View.Descendant<TheArchiveButton>();
            }
        }

        private ArchiveToolbarView ArchiveToolbar
        {
            get
            {
                return View.Descendant<ArchiveToolbarView>();
            }
        }

		public WorldViewController(IWorldService worldService, IFileAccessService fileSource, ILogService log)
			: base()
        {
            _worldService = worldService;
            _fileService = fileSource;
			_log = log;
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

		public override void LoadView()
		{
			var view = new SeadragonView(UIScreen.MainScreen.Bounds, "Images/bg-v.jpg", "Images/bg-h.jpg");

            var contentFrame = ContentFrame(view, true, 0);
            var content = new ContentView(contentFrame);
            content.Frame = content.Frame.MoveToX(content.PosLeft);
            content.Tag = (int)Elements.Content;

            var dragHotspot = content.Bounds;
            var dragGesture = new HorizontalViewDragGestureRecognizer(view, content, dragHotspot, content.PosLeft, ContentView.PosRight);
            dragGesture.CancelsTouchesInView = false;

            dragGesture.ShouldDrag += OnContentPopoutShouldDrag;
            dragGesture.DragBegin += OnContentPopoutDragBegin;
            dragGesture.Dragging += OnContentPopoutDragging;
            dragGesture.DragEnd += OnContentPopoutDragEnd;

            content.AddGestureRecognizer(dragGesture);
            view.AddSubview(content);

            var mapScroll = view.Child<SeadragonScrollView>();
            if (mapScroll != null)
            {
                foreach (var gesture in mapScroll.GestureRecognizers)
                    gesture.RequireGestureRecognizerToFail(dragGesture);
            }

            var theArchiveButton = new TheArchiveButton();
            theArchiveButton.Frame = theArchiveButton.Frame.MoveTo(view.Frame.Width, view.Frame.Height).Move(-theArchiveButton.Frame.Width, -theArchiveButton.Frame.Height).Move(-5, -5);
            theArchiveButton.BackgroundColor = UIColor.Clear;
            theArchiveButton.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleTopMargin;
            theArchiveButton.TouchUpInside += (s1, e1) =>
            {
                if (ArchiveToolbar != null)
                    HideArchiveToolbar();
                else
                    ShowArchiveToolbar();
            };

            view.AddSubview(theArchiveButton);

			this.View = view;
		}

        private RectangleF ContentFrame(UIView mainView, bool isPortrait, float currentX)
        {
            var x = currentX;
            var width = isPortrait ? Math.Min(mainView.Bounds.Width, mainView.Bounds.Height) : (Math.Max(mainView.Bounds.Width, mainView.Bounds.Height) / 2) + 100;
            var height = isPortrait ? Math.Max(mainView.Bounds.Width, mainView.Bounds.Height) : Math.Min(mainView.Bounds.Width, mainView.Bounds.Height);

            var contentFrame = new RectangleF(0, 0, width, height).ExpandWidth(-ContentView.PosMiddle);

            if (currentX != ContentView.PosRight && currentX != ContentView.PosMiddle)
                x = -contentFrame.Width + ContentView.TabWidth + ContentView.Padding;

            return contentFrame.MoveToX(x);
        }

        private void ShowArchiveToolbar()
        {
            if (ArchiveToolbar == null)
            {
                var frame = View.Bounds.NewHeight(40).NewY(View.Bounds.Height);

                var toolbar = new ArchiveToolbarView(frame);
                toolbar.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleTopMargin;
                View.InsertSubviewBelow(toolbar, ArchiveButton);
                toolbar.AnimateFrame(frame.MoveY(-frame.Height), 0.15f);
            }
        }

        private void HideArchiveToolbar()
        {
            if (ArchiveToolbar != null)
            {
                var toolbar = ArchiveToolbar;
                var frame = toolbar.Frame.MoveY(toolbar.Frame.Height);
                toolbar.AnimateFrame(frame, 0.15f, () =>
                {
                    toolbar.RemoveFromSuperview();
                    toolbar.Dispose();
                });
            }
        }

        public override void ViewDidLoad()
        {
			const float Spacing = 8f;

			base.ViewDidLoad();

			if (_tileSource == null)
			{
				TaskFactory.StartNew(() =>
				{
					InvokeOnMainThread(() =>
					{
                        var content = this.View.Descendant<ContentView>((int)Elements.Content);
                        if (content != null)
                        {
                            var world = _worldService.SelectedWorld;
                            var chapter = new Chapter();
                            chapter.Title = world.Title;
                            chapter.Subtitle = world.Subtitle;
                            chapter.Pages = world.Pages;

                            var source = new ContentMenuTableViewSource(new List<Chapter> { chapter });
                            source.PageSelected += (sender, e) => 
                            {
                                content.Content.LoadContent(e.Page.AbsoluteContentPath);
                            };
                            content.Menu.Source = source;
                        }
					});
				});
			}
        }

		public override void ViewWillAppear(bool animated)
		{
			base.ViewWillAppear(animated);
		}

		public override void ViewWillDisappear(bool animated)
		{
			base.ViewWillDisappear(animated);
			var seadragonView = this.View as SeadragonView;
			if (seadragonView != null)
				seadragonView.Hide();
		}

		public override void WillAnimateRotation(UIInterfaceOrientation toInterfaceOrientation, double duration)
		{
			base.WillAnimateRotation(toInterfaceOrientation, duration);

            var content = this.View.Descendant<ContentView>((int)Elements.Content);
            if (content != null)
            {
                var portrait = toInterfaceOrientation == UIInterfaceOrientation.Portrait || toInterfaceOrientation == UIInterfaceOrientation.PortraitUpsideDown;
                var newFrame = ContentFrame(this.View, portrait, content.Frame.X);
                content.AnimateFrame(newFrame, duration);
            }
		}

		public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
		{
			base.DidRotate(fromInterfaceOrientation);
		}

		private void OnNavigationItemSelected(object sender, NavigationItemSelectedEventArgs e)
		{
			var item = e.Item; 
		}

		private void OnHomeButtonClicked(object sender, EventArgs e)
		{
			this.NavigationController.PopViewControllerAnimated(true);
		}

		public void LoadWorld(string remoteWorldUrl)
		{
			TaskFactory.StartNew(_worldService.LoadAction((res) =>
			{
				if (res == StatusCode.Ok && _worldService.SelectedWorld != null && _worldService.SelectedWorld.Map != null)
				{
					if (_tileSource == null)
					{
						_tileSource = new SeadragonTileSourceiOS(_fileService, _log, Settings.IsRetina);

						_tileSource.Initialized += (s1, e1) =>
						{
							var img = UIImage.FromFile("Images/marker-adventure.png");
							var oX = img.Size.Width / 2;
							var oY = img.Size.Height / 2;
							var imgRect = new RectangleF(0, 0, _tileSource.Dzi.Width, _tileSource.Dzi.Height);
							foreach (var marker in _worldService.SelectedWorld.Map.Markers)
							{
								if (imgRect.Contains(new PointF(marker.X, marker.Y)))
								{
									var overlay = new SeadragonOverlay { X = (int)marker.X, Y = (int)marker.Y, Image = img, OriginX = (int)oX, OriginY = (int)oY, Title = marker.Title };
									_tileSource.Overlays.Add(overlay);
								}
							}

							this.InvokeOnMainThread(() =>
							{
								var seadragonView = this.View as SeadragonView;
								if (seadragonView != null)
									seadragonView.Setup(_tileSource);
							});
						};

						_tileSource.OverlaySelected += (sender, e) => 
						{
							InvokeOnMainThread(() =>
							{
								if (e.Overlay != null)
								{
									var alert = new UIAlertView("Clicked!", e.Overlay.Title, null, "OK");
									alert.Show();
								}
							});
						};
					}
					var url = @"http://www.blackdragongames.org/mapmedia/island%20map.xml";
					if (_tileSource.Initialize(url))
					{
						InvokeOnMainThread(() =>
						{
							var seadragonView = this.View as SeadragonView;
							if (seadragonView != null)
								seadragonView.Show();
						});
					}
				}
			}));
		}

		#region Content popout management

        private void OnContentPopoutShouldDrag(object sender, ShouldDragEventArgs e)
        {
            var content = this.View.Descendant<ContentView>((int)Elements.Content);
            if (!e.Cancel && content != null)
            {
                if (content.Content != null)
                {
                    var point = content.ConvertPointToView(e.Location, content.Content);
                    e.Cancel = content.Content.Bounds.Contains(point) && content.ContentPanel.Alpha == 1.0f;
                }

                if (!e.Cancel && content.Menu != null)
                {
                    var point = content.ConvertPointToView(e.Location, content.Menu);
                    e.Cancel = content.Menu.Bounds.Contains(point);
                }
            }
        }

		private void OnContentPopoutDragBegin(object sender, HorizontalViewDragEventArgs e)
		{
            var content = this.View.Descendant<ContentView>((int)Elements.Content);
            if (content != null && content.ContentPanel != null)
            {
                content.Content.UserInteractionEnabled = false;
            }
		}

		private void OnContentPopoutDragging(object sender, HorizontalViewDragEventArgs e)
		{
            var content = this.View.Descendant<ContentView>((int)Elements.Content);
            if (content != null && content.ContentPanel != null)
            {
                var alpha = 1f;
                if (e.CurrentX > ContentView.PosMiddle)
                {
                    var pixelsPerPercent = Math.Abs(ContentView.PosMiddle) / 70;
                    alpha = (float)(Math.Abs(e.CurrentX) / pixelsPerPercent) / 100;
                    alpha += 0.3f;
                }
                content.ContentPanel.Alpha = alpha;
            }
		}

		private void OnContentPopoutDragEnd(object sender, HorizontalViewDragEventArgs e)
		{
            var content = this.View.Descendant<ContentView>((int)Elements.Content);
			if (content != null)
            {
                RectangleF newFrame;
                float alpha = 1f;

                //animate to left pos
                if (e.Expanding && e.CurrentX < ContentView.PosMiddle)
                    newFrame = content.Frame.NewX(content.PosLeft);

                //animate to right pos
                else if (!e.Expanding && e.CurrentX > ContentView.PosMiddle)
                {
                    alpha = 0.3f;
                    newFrame = content.Frame.NewX(ContentView.PosRight);
                }

                //animate to middle pos
                else
                    newFrame = content.Frame.NewX(ContentView.PosMiddle);

                content.AnimateFrame(newFrame, 0.15f);
                content.ContentPanel.FadeOut(alpha, 0.15f);
                content.Content.UserInteractionEnabled = alpha == 1.0f;
			}
		}

		private void OnContentViewActionRaised(object sender, TabPopoutActionEventArgs e)
		{
		}

		#endregion Content popout management

    }
}