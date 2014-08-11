using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MonoTouch.Foundation;
using MonoTouch.UIKit;
using System.Drawing;

using BlackDragon.Fx.Extensions;
using BlackDragon.Fx;
using BlackDragon.Core.Entities;

namespace BlackDragon.Archive.Views
{
    public class MenuAccessView : UIView
    {
		private UserData _userData;

		public enum Buttons
		{
			CurrentAdventure = 1,
			CurrentWorld = 2,
			Bookmark = 3,
			FavouriteWorld1 = 4,
			FavouriteWorld2 = 5,
			News = 6
		}

		public event EventHandler<MenuItemSelectedEventArgs> MenuItemSelected;

        private static SizeF AdvThumbSize = new SizeF(140, 200);
        private static SizeF WldThumbSize = new SizeF(300, 200);
        private static SizeF NewsThumbSize = new SizeF(620, 200);
        private static float ThumbSpacing = 20f;
        private static float ThumbPadding = 3f;

        public static SizeF ViewSize = new SizeF((ThumbSpacing * 3) + (WldThumbSize.Width * 2), (ThumbSpacing * 4) + (WldThumbSize.Height * 3));// { get; private set; }

		public MenuAccessView()
        {
			Initialize();
        }

		public MenuAccessView(RectangleF bounds)
            : base(bounds)
        {
			Initialize();
        }

		void Initialize()
        {
            BackgroundColor = UIColor.Clear;
			//this.DebugBorder();

            var advImg = UIImage.FromFile("Images/adv-thumb.jpg");
            var wldImg = UIImage.FromFile("Images/wld-thumb.jpg");
            var newsImg = UIImage.FromFile("Images/news-thumb.jpg");


			var advButton = new MenuAccessButton(new RectangleF(new PointF(ThumbSpacing, ThumbSpacing), AdvThumbSize), advImg);
			SetButtonStyle(advButton, Buttons.CurrentAdventure);
            this.AddSubview(advButton);

			var bookmarkButton = new MenuAccessButton(new RectangleF(new PointF((ThumbSpacing * 2) + AdvThumbSize.Width, ThumbSpacing), AdvThumbSize), advImg);
			SetButtonStyle(bookmarkButton, Buttons.Bookmark);
            this.AddSubview(bookmarkButton);

			var wldButton = new MenuAccessButton(new RectangleF(new PointF((ThumbSpacing * 3) + (AdvThumbSize.Width * 2), ThumbSpacing), WldThumbSize), wldImg);
			SetButtonStyle(wldButton, Buttons.CurrentWorld);
            this.AddSubview(wldButton);

			var wldFav1Button = new MenuAccessButton(new RectangleF(new PointF(ThumbSpacing, (ThumbSpacing * 2) + WldThumbSize.Height), WldThumbSize), wldImg);
			SetButtonStyle(wldFav1Button, Buttons.FavouriteWorld1);
            this.AddSubview(wldFav1Button);

			var wldFav2Button = new MenuAccessButton(new RectangleF(new PointF((ThumbSpacing * 2) + WldThumbSize.Width, (ThumbSpacing * 2) + WldThumbSize.Height), WldThumbSize), wldImg);
			SetButtonStyle(wldFav2Button, Buttons.FavouriteWorld2);
            this.AddSubview(wldFav2Button);

			var newsButton = new MenuAccessButton(new RectangleF(new PointF(ThumbSpacing, (ThumbSpacing * 3) + (WldThumbSize.Height * 2)), NewsThumbSize), newsImg);
			SetButtonStyle(newsButton, Buttons.News);
            this.AddSubview(newsButton);

            advImg.Dispose();
            wldImg.Dispose();
            newsImg.Dispose();
        }

		public void SetUserData(UserData userData)
		{
			_userData = userData;
		}

		public override void LayoutSubviews()
		{
			base.LayoutSubviews();

			if (_userData != null)
			{
				var advUrl = _userData.SelectedAdventure != null ? _userData.SelectedAdventure.AbsoluteImageUrl : string.Empty;
				HookupButtonEvent(Buttons.CurrentAdventure, OnCurrentAdventureSelected, advUrl, _userData.SelectedAdventure, "Images/adv-thumb.jpg");

				var bookmarkUrl = string.Empty;
				HookupButtonEvent(Buttons.Bookmark, OnCurrentAdventureSelected, bookmarkUrl, null, "Images/adv-thumb.jpg");

				var wldUrl = _userData.SelectedWorld != null ? _userData.SelectedWorld.AbsoluteImageUrl : string.Empty;
				HookupButtonEvent(Buttons.CurrentWorld, OnCurrentWorldSelected, wldUrl, _userData.SelectedWorld, "Images/wld-thumb.jpg");

				var fav1 = _userData.FavouriteWorlds.Any() ? _userData.FavouriteWorlds[0] : null;
				var wldFav1Url = fav1 != null ? fav1.AbsoluteImageUrl : string.Empty;
				HookupButtonEvent(Buttons.FavouriteWorld1, OnFav1WorldSelected, wldFav1Url, fav1, "Images/wld-thumb.jpg");

				var fav2 = _userData.FavouriteWorlds.Count() > 1 ? _userData.FavouriteWorlds[1] : null;
				var wldFav2Url = fav2 != null ? fav2.AbsoluteImageUrl : string.Empty;
				HookupButtonEvent(Buttons.FavouriteWorld2, OnFav2WorldSelected, wldFav2Url, fav2, "Images/wld-thumb.jpg");
			}
		}

		private void HookupButtonEvent(Buttons button, EventHandler evt, string imageUrl, IdentifiableItem item, string placeholderPath)
		{
			var btn = this.Descendant<MenuAccessButton>((int)button);
			if (btn != null)
			{
				btn.TouchUpInside -= evt;
				btn.SetIdentifiableItem(item, imageUrl, placeholderPath);
				if (!string.IsNullOrEmpty(imageUrl))
					btn.TouchUpInside += evt;
			}
		}

		private void OnCurrentAdventureSelected(object sender, EventArgs e)
		{
			if (MenuItemSelected != null)
				MenuItemSelected.Invoke(this, new MenuItemSelectedEventArgs(_userData.SelectedAdventure));
		}

		private void OnBookmarkSelected(object sender, EventArgs e)
		{
//			if (MenuItemSelected != null)
//				MenuItemSelected.Invoke(this, new MenuItemSelectedEventArgs(_userData.Bookmark));
		}

		private void OnCurrentWorldSelected(object sender, EventArgs e)
		{
			if (MenuItemSelected != null)
				MenuItemSelected.Invoke(this, new MenuItemSelectedEventArgs(_userData.SelectedWorld));
		}

		private void OnFav1WorldSelected(object sender, EventArgs e)
		{
			if (MenuItemSelected != null)
				MenuItemSelected.Invoke(this, new MenuItemSelectedEventArgs(_userData.FavouriteWorlds[0]));
		}

		private void OnFav2WorldSelected(object sender, EventArgs e)
		{
			if (MenuItemSelected != null)
				MenuItemSelected.Invoke(this, new MenuItemSelectedEventArgs(_userData.FavouriteWorlds[1]));
		}

		private void SetButtonStyle(BDGImageButton btn, Buttons buttonId)
        {
            btn.Layer.BorderColor = UIColor.White.CGColor;
            btn.Layer.BorderWidth = ThumbPadding;
            btn.Layer.CornerRadius = 2f;
			btn.Tag = (int)buttonId;
            btn.DropShadow(10f);
        }
    }

	public class MenuItemSelectedEventArgs : EventArgs
	{
		public IdentifiableItem Item
		{
			get;
			private set;
		}

		public MenuItemSelectedEventArgs(IdentifiableItem item)
		{
			Item = item;
		}
	}
}