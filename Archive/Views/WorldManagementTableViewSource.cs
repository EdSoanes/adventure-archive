using System;
using System.Collections.Generic;
using System.Linq;

using MonoTouch.UIKit;
using BlackDragon.Core.Entities;
using BlackDragon.Fx;
using MonoTouch.Foundation;
using System.Drawing;
using BlackDragon.Fx.Extensions;
using MonoTouch.CoreGraphics;
using BlackDragon.Core;

namespace BlackDragon.Archive
{
	public class WorldManagementTableViewSource : UITableViewSource
    {
		public event EventHandler<ToggleEventArgs> FavouriteToggling;
		public event EventHandler<ToggleEventArgs> FavouriteToggled;

		private UIImage _thumb;
		private UIImage _favouriteBtnOn;
		private UIImage _favouriteBtnOff;

		private IUserDataService _userDataService;
		private const string LoadingCellId = "WorldLoadingCellId";
		private const string CellId = "WorldCellId";

		public event EventHandler<WorldSelectedEventArgs> WorldSelected;

		public WorldManagementTableViewSource(IUserDataService userDataService)
        {
			_userDataService = userDataService;
			_thumb = UIImage.FromFile("Images/wld-mgmt-thumb.jpg");

			_favouriteBtnOn = UIImage.FromFile("Images/btn-favourite-wld-mgmt-on.png");
			_favouriteBtnOff = UIImage.FromFile("Images/btn-favourite-wld-mgmt-off.png");
        }

		public override int RowsInSection (UITableView tableview, int section)
		{
			return _userDataService.UserData.Worlds.Count;
		}

		public override UITableViewCell GetCell (UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			var world = _userDataService.UserData.Worlds[indexPath.Row];
			UITableViewCell cell = null;

			if (world.LastAccessedStatusCode.HasValue && world.LastAccessedStatusCode == StatusCode.Ok)
			{
				// if there are no cells to reuse, create a new one
				cell = tableView.DequeueReusableCell(CellId);
				if (cell == null)
				{
					var worldMgmtCell = new WorldManagementTableViewCell(UITableViewCellStyle.Subtitle, CellId, world);
					worldMgmtCell.FavouriteToggling += (s1, e1) =>
					{
						if (FavouriteToggling != null)
							FavouriteToggling.Invoke(s1, e1);
					};
					worldMgmtCell.FavouriteToggled += (s2, e2) =>
					{
						if (FavouriteToggled != null)
							FavouriteToggled.Invoke(s2, e2);
					};

					cell = worldMgmtCell;
				}

				var isFavourite = _userDataService.IsFavouriteWorld(world);
				var wldMgmtCell = cell as WorldManagementTableViewCell;
				if (wldMgmtCell != null)
				{
					wldMgmtCell.SetCellData(isFavourite, world, _favouriteBtnOn, _favouriteBtnOff);
					wldMgmtCell.ImageView.Image = _thumb;
					wldMgmtCell.TextLabel.Text = world.Title;
					wldMgmtCell.DetailTextLabel.Text = world.Subtitle;
					wldMgmtCell.LoadImageAsync(world.AbsoluteImageUrl);
				}
				return cell;
			}
			else
			{
				cell = tableView.DequeueReusableCell(LoadingCellId);
				if (cell == null)
					cell = new WorldManagementTableViewLoadingCell(UITableViewCellStyle.Default, LoadingCellId);
				cell.ImageView.Image = _thumb;
			}


			return cell;
		}

		public override float GetHeightForRow(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			return _thumb.Size.Height + 10;
		}

		public override void RowSelected(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			var world = _userDataService.UserData.Worlds[indexPath.Row];
			if (WorldSelected != null)
				WorldSelected.Invoke(this, new WorldSelectedEventArgs(world));
		}

		public override void CommitEditingStyle (UITableView tableView, UITableViewCellEditingStyle editingStyle, MonoTouch.Foundation.NSIndexPath indexPath)
		{
			switch (editingStyle) {
				case UITableViewCellEditingStyle.Delete:
					// remove the item from the underlying data source
					_userDataService.UserData.Worlds.RemoveAt(indexPath.Row);
					// delete the row from the table
					tableView.DeleteRows (new NSIndexPath[] { indexPath }, UITableViewRowAnimation.Fade);
					break;
				case UITableViewCellEditingStyle.None:
					Console.WriteLine ("CommitEditingStyle:None called");
					break;
			}
		}

		public override bool CanEditRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true; // return false if you wish to disable editing for a specific indexPath or for all rows
		}

		public override bool CanMoveRow (UITableView tableView, NSIndexPath indexPath)
		{
			return true; // return false if you don't allow re-ordering
		}

		public override UITableViewCellEditingStyle EditingStyleForRow (UITableView tableView, NSIndexPath indexPath)
		{
			return UITableViewCellEditingStyle.Delete; // this example doesn't suppport Insert
		}

		public override void MoveRow (UITableView tableView, NSIndexPath sourceIndexPath, NSIndexPath destinationIndexPath)
		{
			_userDataService.MoveWorld(sourceIndexPath.Row, destinationIndexPath.Row);
//			var world = _userDataService.UserData.Worlds[sourceIndexPath.Row];
//			int deleteAt = sourceIndexPath.Row;
//			int insertAt = destinationIndexPath.Row;
//			// are we inserting 
//			if (destinationIndexPath.Row < sourceIndexPath.Row) 
//			{
//				// add one to where we delete, because we're increasing the index by inserting
//				deleteAt += 1;
//			} 
//			else 
//			{
//				// add one to where we insert, because we haven't deleted the original yet
//				insertAt += 1;
//			}
//			_userDataService.UserData.Worlds.Insert (insertAt, world);
//			_userDataService.UserData.Worlds.RemoveAt (deleteAt);
		}

		protected override void Dispose(bool disposing)
		{
			if (_thumb != null)
			{
				_thumb.Dispose();
				_thumb = null;
			}

			if (_favouriteBtnOn != null)
			{
				_favouriteBtnOn.Dispose();
				_favouriteBtnOn = null;
			}

			if (_favouriteBtnOff != null)
			{
				_favouriteBtnOff.Dispose();
				_favouriteBtnOff = null;
			}

			base.Dispose(disposing);
		}
    }

	public class WorldSelectedEventArgs : EventArgs
	{
		public UserDataWorld World
		{
			get;
			private set;
		}

		public WorldSelectedEventArgs(UserDataWorld world)
		{
			World = world;
		}
	}
}

