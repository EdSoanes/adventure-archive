using System;
using BlackDragon.Fx;
using BlackDragon.Fx.Extensions;
using System.Drawing;
using MonoTouch.UIKit;
using BlackDragon.Core.Entities;

namespace BlackDragon.Archive
{
	public class MenuAccessButton : BDGImageButton
    {
        private enum Elements
        {
			Panel = 1,
			SubPanel = 2,
			Title = 3,
			SubTitle = 4
        }

		private IdentifiableItem _item;

		public MenuAccessButton(RectangleF frame, UIImage img)
			: base(frame, img)
		{
			InitializeView();
		}

		public MenuAccessButton(RectangleF frame, UIImage placeholderImage, string remoteImageUrl)
			: base(frame, placeholderImage, remoteImageUrl)
		{
			InitializeView();
		}

		public MenuAccessButton(UIImage img)
			: base(img)
		{
			InitializeView();
		}

		public MenuAccessButton(UIImage placeholderImage, string remoteImageUrl)
			: base(placeholderImage, remoteImageUrl)
		{
			InitializeView();
		}

		private void InitializeView()
		{
			//Make sure these are never added twice. Make LoadImageAsync this is not called unnecessarily too! Seems to be...
			var titlePanel = new UIView();
			titlePanel.UserInteractionEnabled = false;
			titlePanel.Tag = (int)Elements.Panel;
			titlePanel.BackgroundColor = UIColor.Clear;
			titlePanel.Frame = this.Bounds.MoveY(this.Frame.Height - 60).NewHeight(60);
			titlePanel.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			titlePanel.Hidden = true;
			this.AddSubview(titlePanel);

			var subPanel = new UIView();
			subPanel.UserInteractionEnabled = false;
			subPanel.Tag = (int)Elements.SubPanel;
			subPanel.Frame = titlePanel.Bounds;
			subPanel.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			subPanel.BackgroundColor = UIColor.Black;
			subPanel.Alpha = 0.5f;
			titlePanel.AddSubview(subPanel);

			var title = new UILabel();
			title.UserInteractionEnabled = false;
			title.Tag = (int)Elements.Title;
			title.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			title.Text = "DUMMY TEXT";
			title.TextColor = Styles.TitleLight;
			title.Font = Styles.FontTitle18;
			title.SizeToFit();
			title.Frame = title.Frame.MoveTo(6, 4);
			titlePanel.AddSubview(title);

			var subtitle = new UILabel();
			subtitle.UserInteractionEnabled = false;
			subtitle.Tag = (int)Elements.SubTitle;
			subtitle.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
			subtitle.Text = "DUMMY TEXT";
			subtitle.TextColor = UIColor.White;
			subtitle.Font = Styles.FontSubtitle11;
			subtitle.LineBreakMode = UILineBreakMode.WordWrap;
			subtitle.Lines = 2;
			subtitle.SizeToFit();
			subtitle.Frame = title.Frame.MoveTo(8, 28);
			titlePanel.AddSubview(subtitle);
		}

		public void SetIdentifiableItem(IdentifiableItem item, string absoluteImageUrl, string placeholderImage)
		{
			_item = item;

			var titlePanel = this.Descendant<UIView>((int)Elements.Panel);
			if (titlePanel != null)
			{
				if (_item != null)
				{
					titlePanel.Hidden = false;

					var title = titlePanel.Descendant<UILabel>((int)Elements.Title);
					if (title != null)
					{
						title.Text = item.Title;
						title.SizeToFit();
						title.Frame = title.Frame.NewWidth(this.Frame.Width - 16);
					}

					var subtitle = titlePanel.Descendant<UILabel>((int)Elements.SubTitle);
					if (subtitle != null)
					{
						subtitle.Text = item.Subtitle;
						subtitle.SizeToFit();
						subtitle.Frame = subtitle.Frame.NewWidth(this.Frame.Width - 16);
					}

					if (!string.IsNullOrEmpty(absoluteImageUrl))
						LoadImageAsync(absoluteImageUrl);
					else
					{
						var placeholder = UIImage.FromFile(placeholderImage);
						SetBackgroundImage(placeholder, UIControlState.Normal);
						placeholder.Dispose();
					}
				}
				else
				{
					titlePanel.Hidden = true;
					var placeholder = UIImage.FromFile(placeholderImage);
					SetBackgroundImage(placeholder, UIControlState.Normal);
					placeholder.Dispose();
				}
			}

			SetNeedsLayout();
		}
    }
}

