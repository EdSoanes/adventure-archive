using System;
using MonoTouch.UIKit;
using System.Drawing;

namespace BlackDragon.Archive
{
	public class NavigationMenuButton : UILabel
	{
		public NavigationItem Item { get; private set; }

		public NavigationMenuButton() : base()
		{
			InitializeView();
		}

		public NavigationMenuButton(NavigationItem item) : base()
		{
			Item = item;
			InitializeView();
		}

		public NavigationMenuButton(RectangleF frame, NavigationItem item) : base(frame)
		{
			Item = item;
			InitializeView();
		}

		private void InitializeView()
		{
			this.Text = Item.Title;
			this.Font = Styles.FontTitle18;
			this.BackgroundColor = UIColor.Clear;
			this.TextColor = UIColor.Yellow;
			this.UserInteractionEnabled = true;
			this.SizeToFit();
		}
	}
}

