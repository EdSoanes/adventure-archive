using System;
using System.Drawing;
using System.Linq;

using BlackDragon.Fx;
using BlackDragon.Fx.Extensions;
using BlackDragon.Core;
using BlackDragon.Core.IoC;

using MonoTouch.UIKit;
using BlackDragon.Archive.Views;
using BlackDragon.Fx.Gestures;
using BlackDragon.Core.Entities;
using System.Threading.Tasks;

namespace BlackDragon.Archive
{
    public class MenuViewController : BDGViewController
    {
        private enum Elements
        {
			Container = 1,
			MenuAccess = 2,
			TabPopout = 3
        }

		private readonly IFileAccessService _imageSource;
        private readonly IUserDataService _userDataService;
        private readonly IWorldService _worldService;

        private WorldViewController _worldViewController;
        private ITabPopoutContentView _contentView;

        private float _worldManagementLeftX;
        private float _worldManagementRightX;

		public MenuViewController(IFileAccessService imageSource, IUserDataService userDataService, IWorldService worldService)
        {
            _imageSource = imageSource;
            _userDataService = userDataService;
            _worldService = worldService;

            LoadUserData();
        }

        public override void DidReceiveMemoryWarning()
        {
            // Releases the view if it doesn't have a superview.
            base.DidReceiveMemoryWarning();

            // Release any cached data, images, etc that aren't in use.
        }

        public override void ViewDidLoad()
        {
            View = CreateMenuView();

            base.ViewDidLoad();

            // Perform any additional setup after loading the view
        }

        public override void ViewWillAppear(bool animated)
        {
            CalculateWorldManagementBoundaries(View as MenuView);
        }

        public override void DidRotate(UIInterfaceOrientation fromInterfaceOrientation)
        {
            base.DidRotate(fromInterfaceOrientation);
            CalculateWorldManagementBoundaries(View as MenuView);
        }

        private MenuView CreateMenuView()
        {
            const float Padding = 40f;

            var menuView = new MenuView(UIScreen.MainScreen.Bounds);

            //Create the menu access view
			CalculateContainerPositions(menuView);
			var containerPos = new PointF(_worldManagementRightX, menuView.Frame.Height - MenuAccessView.ViewSize.Height - Padding);

            var containerView = new UIView();
			containerView.Frame = new RectangleF(containerPos, MenuAccessView.ViewSize).NewWidth(MenuAccessView.ViewSize.Width * 2);
            containerView.Tag = (int)Elements.Container;
            containerView.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleLeftMargin;
            menuView.AddSubview(containerView);

			var menuAccessView = new MenuAccessView();
			menuAccessView.Frame = new RectangleF(PointF.Empty, MenuAccessView.ViewSize);
            menuAccessView.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleLeftMargin;
            containerView.AddSubview(menuAccessView);
            menuView.MenuAccessView = menuAccessView;
			menuAccessView.MenuItemSelected += OnMenuItemSelected;

            //Create the tab popout
			var tabPopoutFrame = menuAccessView.Frame.MoveToX(MenuAccessView.ViewSize.Width);
			var worldManagementView = new TabPopoutView(tabPopoutFrame, new TabPopoutView.Tabs[] { TabPopoutView.Tabs.Worlds }, TabPopoutView.TabPosition.Left);
            worldManagementView.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleLeftMargin;
            containerView.AddSubview(worldManagementView);
            menuView.TabPopout = worldManagementView;

            //Calculate the popout positions
            CalculateWorldManagementBoundaries(menuView);

            //Add the tab popout drag gesture
			var dragHotspot = containerView.Bounds; //.NewWidth(TabPopoutView.Spacing + TabPopoutView.TabWidth).MoveToX(menuAccessView.Frame.Width);
			var dragGesture = new HorizontalViewDragGestureRecognizer(menuView, containerView, dragHotspot, _worldManagementLeftX, _worldManagementRightX);
			dragGesture.CancelsTouchesInView = false;

            dragGesture.DragBegin += OnWorldManagementViewDragBegin;
            dragGesture.Dragging += OnWorldManagementViewDragging;
            dragGesture.DragEnd += OnWorldManagementViewDragEnd;
            
			containerView.AddGestureRecognizer(dragGesture);

            return menuView;
        }

        private void DestroyMenuView()
        {
            var menuView = View as MenuView;
            if (menuView != null)
            {
                var worldManagementView = menuView.TabPopout;
                if (worldManagementView != null)
                {
                    var dragGesture = worldManagementView.GestureRecognizers.FirstOrDefault(x => x is HorizontalViewDragGestureRecognizer) as HorizontalViewDragGestureRecognizer;
                    if (dragGesture != null)
                    {
                        dragGesture.DragBegin -= OnWorldManagementViewDragBegin;
                        dragGesture.Dragging -= OnWorldManagementViewDragging;
                        dragGesture.DragEnd -= OnWorldManagementViewDragEnd;
                    }
                }

                View.Dispose();
                View = null;
            }
        }

        private void LoadUserData()
        {
			this.TaskFactory.StartNew(_userDataService.LoadAction((errorCode) => 
			{
				this.InvokeOnMainThread(() =>
				{
					var menuView = View as MenuView;
					if (menuView != null)
					{
						menuView.MenuAccessView.SetUserData(_userDataService.UserData);
						menuView.MenuAccessView.SetNeedsLayout();
					}
				});
			}));
        }

		private void CalculateContainerPositions(MenuView menuView)
		{
			if (menuView != null)
			{
				_worldManagementRightX = menuView.Frame.Width - MenuAccessView.ViewSize.Width - (TabPopoutView.TabWidth + TabPopoutView.Spacing);
				_worldManagementLeftX = menuView.Frame.Width - (MenuAccessView.ViewSize.Width * 2);
			}
		}

        private void CalculateWorldManagementBoundaries(MenuView menuView)
        {
            if (menuView != null)
            {
				var containerView = menuView.Descendant<UIView>((int)Elements.Container);
				if (containerView != null)
				{
					CalculateContainerPositions(menuView);
					var dragGesture = containerView.GestureRecognizers != null ? containerView.GestureRecognizers.FirstOrDefault(x => x is HorizontalViewDragGestureRecognizer) as HorizontalViewDragGestureRecognizer : null;
					if (dragGesture != null)
						dragGesture.SetBoundaries(_worldManagementLeftX, _worldManagementRightX);
				}
            }
        }

        private void OnWorldManagementViewDragBegin(object sender, HorizontalViewDragEventArgs e)
        {
            if (e.CurrentX == e.RightViewX)
            {
                var menuView = View as MenuView;
                if (menuView != null)
                {
                    var worldManagementView = menuView.TabPopout;
                    if (worldManagementView != null)
                    {
                        var tableView = worldManagementView.CreateContentView<WorldManagementView>();
                        _contentView = tableView;
                        if (_contentView != null)
                            _contentView.ActionRaised += OnContentViewActionRaised;

                        this.TaskFactory.StartNew(() =>
                        {
                            InvokeOnMainThread(() =>
                            {
								var source = DC.Get<WorldManagementTableViewSource>();
								source.FavouriteToggling += OnWorldManagementFavouritesChanging;
								source.FavouriteToggled += OnWorldManagementFavouritesChanged;
								tableView.LoadContent(source);
                                tableView.SetNeedsLayout();
                            });
                        });
                    }
                }
            }
        }

        private void OnWorldManagementViewDragging(object sender, HorizontalViewDragEventArgs e)
        {
			var menuView = View as MenuView;
			if (menuView != null)
			{
				var pixelsPerPercent = e.Span / 70;
				var alpha = (float)((e.CurrentX - e.LeftViewX) / pixelsPerPercent) / 100;
				menuView.MenuAccessView.Alpha = alpha + 0.3f;
			}
        }

        private void OnWorldManagementViewDragEnd(object sender, HorizontalViewDragEventArgs e)
        {
			var menuView = View as MenuView;
			if (menuView != null)
			{
				var containerView = View.Descendant<UIView>((int)Elements.Container);
				if (containerView != null)
				{
					RectangleF newFrame = containerView.Frame.MoveToX(!e.Expanding ? _worldManagementRightX : _worldManagementLeftX);
					float alpha = !e.Expanding ? 1f : 0.3f;

					AnimateContainerToFrame(newFrame, alpha);
				}
			}
        }

		private void OnWorldManagementFavouritesChanging(object sender, ToggleEventArgs e)
		{
			var togglingWorld = e.Data as UserDataWorld;
			if (togglingWorld != null)
			{
				if (e.IsOn && !_userDataService.IsFavouriteWorld(togglingWorld))
					e.Cancel = true;
				else if (!e.IsOn && !_userDataService.CanAddWorldToFavourites(togglingWorld))
					e.Cancel = true;
			}
		}

		private void OnWorldManagementFavouritesChanged(object sender, ToggleEventArgs e)
		{
			var togglingWorld = e.Data as UserDataWorld;
			if (e.IsOn)
				_userDataService.AddWorldToFavourites(togglingWorld);
			else
				_userDataService.RemoveWorldFromFavourites(togglingWorld);

			var menuView = View as MenuView;
			if (menuView != null)
				menuView.MenuAccessView.SetNeedsLayout();
		}

		private void AnimateContainerToFrame(RectangleF newFrame, float alpha)
		{
			var menuView = View as MenuView;
			if (menuView != null)
			{
				var containerView = View.Descendant<UIView>((int)Elements.Container);
				if (containerView != null)
				{
					UIView.Animate(0.15f, 0, UIViewAnimationOptions.CurveEaseOut,
						() =>
						{ 
							containerView.Frame = newFrame; 
							menuView.MenuAccessView.Alpha = alpha;
						},
						() =>
						{ 
							var tabPopout = menuView.TabPopout;
							if (tabPopout != null && containerView.Frame.X == _worldManagementRightX)
							{
								if (_contentView != null)
									_contentView.ActionRaised -= OnContentViewActionRaised;

								tabPopout.DestroyContentView();
							}
						}
					);
				}
			}
		}

        private void OnContentViewActionRaised(object sender, TabPopoutActionEventArgs e)
        {
			if (e.Action is UserDataWorld)
			{
				var world = e.Action as UserDataWorld;
				_userDataService.UserData.SelectedWorld = world;

				var menuView = View as MenuView;
                if (menuView != null)
                {
					menuView.MenuAccessView.SetNeedsLayout();
					var containerView = menuView.ViewWithTag((int)Elements.Container);
					if (containerView != null)
					{
						var newFrame = containerView.Frame.MoveToX(_worldManagementRightX);
						var alpha = 1f;
						AnimateContainerToFrame(newFrame, alpha);
					}
                }

				NavigateToWorld();
			}
			else if (e.Action == null)
			{
				UIAlertView alert = new UIAlertView
				{
					Title = "Add a World", 
					Message = "Add the url to a new World",
				};
				alert.AddButton("Cancel");
				alert.AddButton("Ok");
				alert.AlertViewStyle = UIAlertViewStyle.PlainTextInput;
				alert.GetTextField(0).Text = "www.blackdragongames.org/worlds/blackdragon";
				alert.Clicked += (s1, e1) =>
				{
					if (e1.ButtonIndex == 1)
					{
						var worldUrl = alert.GetTextField(0).Text;
						if (!string.IsNullOrWhiteSpace(worldUrl))
						{
							var worldManagementView = _contentView as WorldManagementView;
							if (worldManagementView != null)
							{
								Action loadAction = _userDataService.LoadNewWorldAction(worldUrl);
								if (loadAction != null)
								{
									var userDataWorld = _userDataService.UserData.Worlds.FirstOrDefault(x => x.Url.ToLower() == worldUrl.ToLower());
									worldManagementView.InsertRow(userDataWorld);

									//Load the world on background
									this.TaskFactory.StartNew(loadAction);

									//Then update the view once loaded
									this.TaskFactory.StartNew(() =>
										{
											var wmv = _contentView as WorldManagementView;
											if (wmv != null)
											{
												this.InvokeOnMainThread(() => 
													{
														wmv.ReloadData();
													});
											}
										});
								}
							}
						}
					}
				};
				alert.Show();
			}
        }

		private void OnMenuItemSelected(object sender, MenuItemSelectedEventArgs e)
		{
			if (e.Item is UserDataWorld)
			{
				_userDataService.UserData.SelectedWorld = e.Item as UserDataWorld;
				_userDataService.UserData.SelectedWorld.LastAccessed = DateTime.Now;

				var mV = View as MenuView;
				if (mV != null)
					mV.MenuAccessView.SetNeedsLayout();

				NavigateToWorld();
			}
		}

        void NavigateToWorld()
        {
            if (_worldViewController == null)
                _worldViewController = DC.Get<WorldViewController>();

			_worldViewController.LoadWorld(_userDataService.UserData.SelectedWorld.AbsoluteContentPath);

            this.NavigationController.PushViewController(_worldViewController, true);
        }
    }
}
