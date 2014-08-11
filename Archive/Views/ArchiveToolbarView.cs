using System;
using System.Collections.Generic;
using System.Linq;
using MonoTouch.UIKit;
using System.Drawing;
using BlackDragon.Fx.Extensions;
using BlackDragon.Fx;
using BlackDragon.Core.Entities;

namespace BlackDragon.Archive
{
	public class ArchiveToolbarView : UIView
    {
		public event EventHandler<NavigationItemSelectedEventArgs> ItemSelected;
		public event EventHandler HomeButtonClicked;

		private enum Elements
		{
			Level1 = 1,
			Level2 = 2,
			Background = 3,
			Foreground = 4,
			HomeButton = 5,
			Title = 6,
			BookmarkButton = 7
		}

        private World _world;

        public ArchiveToolbarView()
			: base()
        {
			InitializeView();
        }

		public ArchiveToolbarView(RectangleF frame)
			: base(frame)
		{
			InitializeView();
		}

		private void InitializeView()
		{
			this.AutoresizingMask = UIViewAutoresizing.FlexibleWidth | UIViewAutoresizing.FlexibleBottomMargin;
			this.UserInteractionEnabled = true;

            var bg = new UIView(this.Bounds);
			bg.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			bg.BackgroundColor = UIColor.Black;
            bg.Alpha = 0.7f;
			bg.Tag = (int)Elements.Background;
			this.AddSubview(bg);

            var fg = new UIView(this.Bounds);
			fg.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			fg.BackgroundColor = UIColor.Clear;
			fg.Tag = (int)Elements.Foreground;
			this.AddSubview(fg);

			var homeImg = UIImage.FromFile("Images/btn-home.png");
			var homeDownImg = UIImage.FromFile("Images/btn-home-highlight.png");
			var homeBtn = new BDGImageOnlyButton(homeImg, homeDownImg);
			homeBtn.Tag = (int)Elements.HomeButton;
			homeBtn.TouchUpInside += (s1, e1) =>
			{
				if (HomeButtonClicked != null)
					HomeButtonClicked.Invoke(this, new EventArgs());
			};
			fg.AddSubview(homeBtn);
			homeImg.Dispose();
			homeDownImg.Dispose();

			using (var bookmarkImg = UIImage.FromFile("Images/btn-bookmark.png"))
			{
				var bookmarkBtn = new UIImageView(bookmarkImg);
				bookmarkBtn.Tag = (int)Elements.BookmarkButton;
				bookmarkBtn.Frame = bookmarkBtn.Frame.MoveTo(this.Frame.Width - bookmarkBtn.Frame.Width - 4, 0);
				bookmarkBtn.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin;
				fg.AddSubview(bookmarkBtn);
			}

			var title = new UILabel();
			title.Text = "Blackdragonia";
			title.Tag = (int)Elements.Title;
			title.Frame = title.Frame.MoveTo(60, 8);
			title.TextColor = UIColor.White;
			title.Font = Styles.FontTitle18;
			title.SizeToFit();
			fg.AddSubview(title);
		}

        public void SetWorld(World world)
		{
            _world = world;
			SetNeedsLayout();
		}

		public override void LayoutSubviews()
		{
			var title = this.Descendant<UILabel>((int)Elements.Title);
            if (title != null && _world != null)
			{
                title.Text = _world.Title;
				title.SizeToFit();
			}

			base.LayoutSubviews();
		}
    }
}

