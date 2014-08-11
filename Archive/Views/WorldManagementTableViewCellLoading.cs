using System;
using MonoTouch.UIKit;
using BlackDragon.Fx;
using BlackDragon.Core;
using BlackDragon.Core.IoC;
using BlackDragon.Fx.Extensions;
using System.Drawing;

namespace BlackDragon.Archive
{
	public class WorldManagementTableViewLoadingCell : UITableViewCell
	{
		public WorldManagementTableViewLoadingCell(UITableViewCellStyle style, string cellId)
			: base(style, cellId)
		{
			InitializeView();
		}

		public WorldManagementTableViewLoadingCell(IntPtr handle)
			: base(handle)
		{
			InitializeView();
		}

		private void InitializeView()
		{
			this.TextLabel.Font = Styles.FontTitle18;
			this.TextLabel.Text = "Loading...";

			var activityIndicator = new UIActivityIndicatorView(UIActivityIndicatorViewStyle.WhiteLarge);
			activityIndicator.Frame = activityIndicator.Frame.MoveTo(40, (this.Bounds.Height - activityIndicator.Frame.Height) / 2);
			activityIndicator.StartAnimating();
			this.AddSubview(activityIndicator);
		}
	}
}

