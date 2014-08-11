using System;
using MonoTouch.UIKit;
using System.Drawing;
using BlackDragon.Fx.Extensions;

namespace BlackDragon.Fx
{
    public class MapTitleBarView : UIView
    {
        public MapTitleBarView(RectangleF frame)
            : base(frame)
        {
            InitializeView();
        }

        private void InitializeView()
        {
            this.BackgroundColor = UIColor.Black;
            this.Layer.Opacity = 0.5f;

            var title = new UITextView(this.Bounds.NewHeight(40));
            title.Tag = 1;
            title.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            title.UserInteractionEnabled = false;
            title.BackgroundColor = UIColor.Clear;
            title.Font = UIFont.FromName("Georgia", 24);//_settings.MarkerTextFont;
            title.TextColor = UIColor.White;
            title.TextAlignment = UITextAlignment.Right;
            this.AddSubview(title);

            var subtitle = new UITextView(this.Bounds.NewHeight(20).MoveToY(20));
            subtitle.Tag = 2;
            subtitle.AutoresizingMask = UIViewAutoresizing.FlexibleWidth;
            subtitle.UserInteractionEnabled = false;
            subtitle.BackgroundColor = UIColor.Clear;
            subtitle.Font = UIFont.FromName("Georgia", 16);//_settings.MarkerTextFont;
            subtitle.TextColor = UIColor.White;
            subtitle.TextAlignment = UITextAlignment.Right;
            this.AddSubview(subtitle);
        }

        public void SetTitleAndSubtitle(string title, string subtitle)
        {
            var tv = this.ViewWithTag(1) as UITextView;
            if (tv != null)
                tv.Text = title;

            var stv = this.ViewWithTag(2) as UITextView;
            if (stv != null)
                stv.Text = subtitle;
        }
    }
}

