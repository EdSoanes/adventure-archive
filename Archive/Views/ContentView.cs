using System;
using MonoTouch.UIKit;
using System.Drawing;
using BlackDragon.Fx.Extensions;
using BlackDragon.Fx;

namespace BlackDragon.Archive.Views
{
    public class ContentView : UIView
    {
        public const float NavBGWidth = 300f;
        public const float Padding = 4f;
        public const float TabWidth = 30f;

        public float PosLeft { get { return -(this.Frame.Width - TabWidth - Padding); } }
        public const float PosMiddle = -NavBGWidth;
        public const float PosRight = 0f;

        private enum Elements
        {
            Content = 1,
            ContentPanel = 2,
            Menu = 3,
            MenuPanel = 4
        }

        public UIView ContentPanel
        {
            get { return this.Descendant<UIView>((int)Elements.ContentPanel); }
        }

        public BDGWebView Content
        {
            get { return this.Descendant<BDGWebView>((int)Elements.Content); }
        }

        public UIView MenuPanel
        {
            get { return this.Descendant<UIView>((int)Elements.MenuPanel); }
        }

        public ContentMenuTableView Menu
        {
            get { return this.Descendant<ContentMenuTableView>((int)Elements.Menu); }
        }

        public ContentView()
            : base()
        {
            InitializeView();
        }

        public ContentView(RectangleF frame)
            : base(frame)
        {
            InitializeView();
        }

        private void InitializeView()
        {
            this.BackgroundColor = UIColor.Clear;

            var navContainer = new UIView();
            navContainer.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleHeight;
            navContainer.Tag = (int)Elements.MenuPanel;
            navContainer.BackgroundColor = UIColor.Clear;
            navContainer.Frame = NavContainerFrame();

            //Create the menu
            var navPanel = new UIView();
            navPanel.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            navPanel.BackgroundColor = UIColor.White;
            navPanel.Frame = navContainer.Bounds.DeflateHeight(Padding).ShrinkWidth(TabWidth + Padding);
            navContainer.AddSubview(navPanel);

            var navTab = new UIView();
            navTab.BackgroundColor = UIColor.White;
            navTab.Frame = new RectangleF(navPanel.Bounds.Width, Padding, 15f, 80f);
            navTab.AutoresizingMask = UIViewAutoresizing.FlexibleRightMargin | UIViewAutoresizing.FlexibleBottomMargin;
            navContainer.AddSubview(navTab);

            var navTbl = new ContentMenuTableView(navPanel.Bounds.Deflate(Padding));
            navTbl.Tag = (int)Elements.Menu;
            navTbl.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            navPanel.AddSubview(navTbl);

            this.AddSubview(navContainer);

            //Create the content area
            var contentContainer = new UIView();
            contentContainer.Tag = (int)Elements.ContentPanel;
            contentContainer.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            contentContainer.BackgroundColor = UIColor.Clear;
            contentContainer.Frame = ContentContainerFrame();
           
            var contentPanel = new UIView();
            contentPanel.BackgroundColor = UIColor.White;
            contentPanel.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            contentPanel.Frame = contentContainer.Bounds.Deflate(Padding).ShrinkWidth(TabWidth - Padding);
            contentContainer.AddSubview(contentPanel);

            var contentTab = new UIView();
            contentTab.BackgroundColor = UIColor.White;
            contentTab.Frame = new RectangleF(contentPanel.Bounds.Width + contentPanel.Frame.X, Padding, 15f, 80f);
            contentTab.AutoresizingMask = UIViewAutoresizing.FlexibleLeftMargin | UIViewAutoresizing.FlexibleBottomMargin;
            contentContainer.AddSubview(contentTab);

            var contentView = new BDGWebView();
            contentView.Tag = (int)Elements.Content;
            contentView.Frame = contentPanel.Bounds.Deflate(Padding);
            contentView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;
            contentView.BackgroundColor = UIColor.White;
            contentPanel.AddSubview(contentView);

            this.AddSubview(contentContainer);
        }

        public override void LayoutSubviews()
        {
            base.LayoutSubviews();
        }

        private RectangleF NavContainerFrame()
        {
            var height = this.Frame.Height - (Padding * 2);
            var rect = new RectangleF(0, Padding, NavBGWidth + TabWidth + Padding, height);
            return rect;
        }

        private RectangleF ContentContainerFrame()
        {
            var x = NavBGWidth + TabWidth + Padding;
            var height = this.Frame.Height - (Padding * 2);
            var width = this.Frame.Width - x - Padding;
            var rect = new RectangleF(x, Padding, width, height);
            return rect;
        }
    }
}

