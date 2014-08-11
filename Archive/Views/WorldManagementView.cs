using System;
using MonoTouch.UIKit;
using System.Drawing;
using System.Linq;
using BlackDragon.Fx;
using System.Collections.Generic;
using BlackDragon.Core.Entities;
using BlackDragon.Fx.Extensions;
using MonoTouch.Foundation;
using BlackDragon.Core.IoC;

namespace BlackDragon.Archive
{
	public class WorldManagementView : UIView, ITabPopoutContentView
    {
		public event EventHandler AddWorld;

		private enum Buttons
		{
			Add,
			Edit
		}

		private UITableView TableView
		{
			get { return this.Subviews.FirstOrDefault(x => x is UITableView) as UITableView; }
		}

        public WorldManagementView()
			: base()
        {
			InitializeView();
        }

		public WorldManagementView(RectangleF frame)
			: base(frame)
		{
			InitializeView();
		}

		private void InitializeView()
		{
			this.Layer.BorderColor = UIColor.White.CGColor;
			this.Layer.BorderWidth = 5f;
			this.Layer.CornerRadius = 2f;

			var tableView = new WorldManagementTableView();
			tableView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			this.AddSubview(tableView);

			var addBtn = new UIButton();
			addBtn.Tag = (int)Buttons.Add;
			addBtn.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin;
			addBtn.SetTitle("Add", UIControlState.Normal);
			addBtn.BackgroundColor = UIColor.White;
			addBtn.SetTitleColor(UIColor.Black, UIControlState.Normal);
			addBtn.TouchUpInside += OnAddWorld;
			this.AddSubview(addBtn);

			var editBtn = new UIButton();
			editBtn.Tag = (int)Buttons.Edit;
			editBtn.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin;
			editBtn.SetTitle("Edit", UIControlState.Normal);
			editBtn.BackgroundColor = UIColor.White;
			editBtn.SetTitleColor(UIColor.Black, UIControlState.Normal);
			editBtn.TouchUpInside += (s1, e1) =>
			{
				var tV = TableView;
				if (tV != null)
				{
					tV.SetEditing(!tV.Editing, true);

					var aB = this.Child<UIButton>((int)Buttons.Add);
					if (aB != null)
						aB.Enabled = !tV.Editing;

					if (tV.Editing)
						editBtn.SetTitle("Done", UIControlState.Normal);
					else
						editBtn.SetTitle("Edit", UIControlState.Normal);
				}
			};
			this.AddSubview(editBtn);
		}

		public void InsertRow(UserDataWorld world)
		{
			var tableView = TableView;
			if (tableView != null)
			{
				tableView.BeginUpdates();
				tableView.InsertRows(new NSIndexPath[] { NSIndexPath.FromItemSection(0, 0) }, UITableViewRowAnimation.Left);
				tableView.EndUpdates();
			}
		}

		public void ReloadData()
		{
			var tableView = TableView;
			if (tableView != null)
				tableView.ReloadData();
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			var tableView = TableView;
			if (tableView != null)
			{
				tableView.Frame = this.Bounds.MoveToY(60).ShrinkHeight(60);
				tableView.SetNeedsLayout();
			}

			var editBtn = this.Child<UIButton>((int)Buttons.Edit);
			if (editBtn != null)
				editBtn.Frame = new RectangleF(this.Bounds.Width - 125, 5, 120, 50);

			var addBtn = this.Child<UIButton>((int)Buttons.Add);
			if (addBtn != null)
				addBtn.Frame = new RectangleF(this.Bounds.Width - 250, 5, 120, 50);
		}

		private void OnAddWorld(object sender, EventArgs e)
		{
			if (ActionRaised != null)
				ActionRaised.Invoke(sender, new TabPopoutActionEventArgs(null));
		}

		#region ITabPopoutContentView implementation

		public event EventHandler ContentLoaded;

		public event EventHandler<TabPopoutActionEventArgs> ActionRaised;

		public void LoadContent(object content)
		{
			var source = content as WorldManagementTableViewSource;
			if (source != null)
			{
				source.WorldSelected += (sender, e) =>  
				{
					if (ActionRaised != null)
						ActionRaised.Invoke(this, new TabPopoutActionEventArgs(e.World));
				};

				var tableView = TableView;
				if (tableView != null)
					tableView.Source = source;
			}
		}

		public bool ShouldLoadContent(object content)
		{
			var worlds = content as List<UserDataWorld>;
			return (worlds != null);
		}

		#endregion
    }
}

