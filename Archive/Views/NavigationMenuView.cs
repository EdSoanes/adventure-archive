using System;
using MonoTouch.UIKit;
using System.Drawing;
using System.Collections.Generic;
using System.Linq;
using BlackDragon.Fx.Extensions;

namespace BlackDragon.Archive
{
	public class NavigationMenuView : UIScrollView
	{
		public event EventHandler<NavigationItemSelectedEventArgs> ItemSelected;

		public NavigationMenuView() : base()
		{
			InitializeView();
		}

		public NavigationMenuView(RectangleF frame) : base(frame)
		{
			InitializeView();
		}

		private void InitializeView()
		{
			this.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleBottomMargin;
			this.UserInteractionEnabled = true;
			this.PagingEnabled = true;
			this.ShowsHorizontalScrollIndicator = false;
			this.ShowsVerticalScrollIndicator = false;
			this.Delegate = new PageNavControlScrollDelegate();
		}

		public void SetNavigationItems(IEnumerable<NavigationItem> items)
		{
			var subviews = this.Subviews.ToList();
			foreach (var subview in subviews)
			{
				subview.RemoveFromSuperview();
				subview.Dispose();
			}

			var btns = new List<NavigationMenuButton>();
			foreach (var item in items)
			{
				var btn = new NavigationMenuButton(item);
				btns.Add(btn);
			}

			var maxWidth = btns.Max(x => x.Frame.Width);

			this.ContentSize = new SizeF(maxWidth * btns.Count(), this.Frame.Height);
			this.Frame = this.Frame.NewWidth(maxWidth);

			float xPos = 0f;
			foreach (var btn in btns)
			{
				btn.Frame = this.Bounds.NewWidth(maxWidth).NewX(xPos);
				xPos += maxWidth;
			}

			this.AddSubviews(btns.ToArray());
		}

		public void Navigate()
		{
			foreach (var item in this.Subviews.Where(x => x is NavigationMenuButton).Select(x => x as NavigationMenuButton))
			{
				if (item.Frame.X - this.ContentOffset.X == 0)
				{
					if (ItemSelected != null)
						ItemSelected.Invoke(this, new NavigationItemSelectedEventArgs(item.Item, this.Tag));

					break;
				}
			}
		}

		public void SetActivePage(NavigationItem item)
		{
			var btn = this.Subviews.Where(x => x is NavigationMenuButton)
				.Select(x => x as NavigationMenuButton)
				.FirstOrDefault(x => x.Item == item);

			if (btn != null)
			{
				var point = new PointF(btn.Frame.X, this.ContentOffset.Y);
				this.SetContentOffset(point, true);
			}
		}
	}

	public class PageNavControlScrollDelegate : UIScrollViewDelegate
	{
		public PageNavControlScrollDelegate()
		{
		}

		public override void DecelerationEnded(UIScrollView scrollView)
		{
			var pageNav = scrollView as NavigationMenuView;
			if (pageNav != null)
				pageNav.Navigate();
		}

		public override void DraggingEnded(UIScrollView scrollView, bool willDecelerate)
		{
			if (!willDecelerate)
			{
				var pageNav = scrollView as NavigationMenuView;
				if (pageNav != null)
					pageNav.Navigate();
			}
		}
	}
}

