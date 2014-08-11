using System;
using System.Drawing;
using BlackDragon.Core;
using BlackDragon.Core.Entities;
using BlackDragon.Core.IoC;
using BlackDragon.Fx;
using BlackDragon.Fx.Extensions;
using MonoTouch.UIKit;

namespace BlackDragon.Archive
{
	public class WorldManagementTableViewCell : UITableViewCell
    {
		public event EventHandler<ToggleEventArgs> FavouriteToggling;
		public event EventHandler<ToggleEventArgs> FavouriteToggled;

		private UserDataWorld _world;
		private enum Elements
		{
			ToggleBtn = 1,
		}

		public WorldManagementTableViewCell(UITableViewCellStyle style, string cellId, UserDataWorld world)
			: base(style, cellId)
        {
			_world = world;
			InitializeView();
        }

		public WorldManagementTableViewCell(IntPtr handle)
			: base(handle)
		{
			InitializeView();
		}

		private void InitializeView()
		{
			this.TextLabel.Font = Styles.FontTitle18;

			this.DetailTextLabel.Font = Styles.FontSubtitle11;
			this.DetailTextLabel.LineBreakMode = UILineBreakMode.WordWrap;
			this.DetailTextLabel.Lines = 2;

			var scrollView = this.Descendant<UIScrollView>();
			if (scrollView != null)
			{
				var favouriteButton = new BDGImageToggleButton(_world);
				favouriteButton.Tag = (int)Elements.ToggleBtn;
				favouriteButton.Frame = new RectangleF(scrollView.Frame.Width - 30, 0, 26, 26);
				favouriteButton.AutoresizingMask = UIViewAutoresizing.FlexibleBottomMargin | UIViewAutoresizing.FlexibleLeftMargin;
				favouriteButton.DebugBorder();
				favouriteButton.Toggling += (s1, e1) =>
				{
					if (FavouriteToggling != null)
						FavouriteToggling.Invoke(this, e1);
				};
				favouriteButton.Toggled += (s2, e2) =>
				{
					if (FavouriteToggled != null)
						FavouriteToggled.Invoke(this, e2);
				};

				scrollView.AddSubview(favouriteButton);
			}
//			this.AddSubview(favouriteButton);
		}

		public void SetToggle(bool toggleOn)
		{
			var btn = this.Descendant<BDGImageToggleButton>((int)Elements.ToggleBtn);
			if (btn != null)
				btn.SetToggle(toggleOn);
		}

		public void SetCellData(bool toggleOn, IdentifiableItem item, UIImage toggleOnImg = null, UIImage toggleOffImg = null)
		{
			var btn = this.Descendant<BDGImageToggleButton>((int)Elements.ToggleBtn);
			if (btn != null)
			{
				btn.SetButtonData(toggleOn, item, toggleOnImg, toggleOffImg);
			}
		}

		public void LoadImageAsync(string remoteImageUrl)
		{
			var fileService = DC.Get<IFileAccessService>();
			if (!string.IsNullOrEmpty(remoteImageUrl))
			{
				var imgData = fileService.ReadFromCache(remoteImageUrl) as byte[];
				if (imgData != null)
					SetImageFromData(imgData);
				else
				{
					fileService.Request(remoteImageUrl, (fileCacheEntry) =>
					{
						imgData = fileCacheEntry.GetData<byte[]>();
						SetImageFromData(imgData);
					});
				}
			}
		}

		private void SetImageFromData(byte[] imgData)
		{
			using (var img = imgData.ToImage())
			{
				var scaledImage = img.Scale(new SizeF(img.Size.Width / 2, img.Size.Height / 2));
				InvokeOnMainThread(() => this.ImageView.Image = scaledImage);
			}
		}


		public override void LayoutSubviews()
		{
			base.LayoutSubviews();
			var btn = this.Descendant<BDGImageToggleButton>((int)Elements.ToggleBtn);
			if (btn != null)
			{
				btn.Frame = new RectangleF(0, 0, 26, 26);
				btn.Frame = btn.Frame.NewX(btn.Superview.Frame.Width - btn.Frame.Width - 4);
			}
		}
    }
}

