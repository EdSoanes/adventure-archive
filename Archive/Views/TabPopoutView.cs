using System;
using MonoTouch.UIKit;
using System.Drawing;
using BlackDragon.Fx.Extensions;
using BlackDragon.Fx;
using MonoTouch.CoreGraphics;
using System.Collections.Generic;
using System.Linq;
using BlackDragon.Fx.Gestures;

namespace BlackDragon.Archive.Views
{
	public class TabPopoutView : UIView
    {
        private UIView _contentView;
		private Tabs[] _tabs;
		private TabPosition _tabPosition;

		public enum TabPosition
		{
			Left,
			Right
		}

		public enum Tabs
		{
            None,
			Worlds,
			Library,
			Favourites,
			Content
		}

		const float ViewHeight = 600f;
		const float ViewWidth = 600f;

        public const float Spacing = 20f;
		public const float TabWidth = 40f;
		public const float TabHeight = 80f;
		public const float TabSpacing = 5f;

		private Tabs _selectedTab = Tabs.None;
		public Tabs SelectedTab
		{
			get { return _selectedTab; }
			set { _selectedTab = value; }
		}

		public TabPopoutView(RectangleF frame, Tabs[] tabs, TabPosition tabPosition)
			: base(frame)
		{
			Initialize(tabs, tabPosition);
		}

		void Initialize(Tabs[] tabs, TabPosition tabPosition)
		{
			_tabs = tabs;
			_tabPosition = tabPosition;

			for (int i = 0; i < tabs.Length; i++)
			{
                if (i == 0)
                    SelectedTab = tabs[i];

				var tabX = _tabPosition == TabPosition.Left ? Spacing : this.Frame.Width - Spacing - TabWidth;
				var tabView = new UIView(new RectangleF(tabX, Spacing, TabWidth, TabHeight));
				tabView.Tag = (int)tabs[i];
				tabView.BackgroundColor = i == 0 ? UIColor.White : UIColor.Blue;
				tabView.AutoresizingMask = (tabPosition == TabPosition.Right ? UIViewAutoresizing.FlexibleLeftMargin : UIViewAutoresizing.FlexibleRightMargin) | UIViewAutoresizing.FlexibleBottomMargin;
				this.AddSubview(tabView);
			}
			this.DropShadow(ShadowPath(), 10f);
		}

        public T CreateContentView<T>() where T : UIView
        {
            if (!typeof(ITabPopoutContentView).IsAssignableFrom(typeof(T)))
                throw new ArgumentException("Specified type T must implement ITabPopoutContentView");

            if (_contentView != null && _contentView.GetType() != typeof(T))
                DestroyContentView();

            if (_contentView == null)
            {
				var frame = this.Bounds.ShrinkWidth(Spacing + TabWidth)
					.ShrinkHeight(Spacing * 2);

				if (_tabPosition == TabPosition.Left)
					frame = frame.Move(Spacing + TabWidth, Spacing);
				else
					frame = frame.MoveY(Spacing);

                _contentView = Activator.CreateInstance<T>();
                _contentView.Frame = frame;
                _contentView.BackgroundColor = UIColor.White;
				_contentView.AutoresizingMask = UIViewAutoresizing.FlexibleDimensions;

                this.AddSubview(_contentView);
            }

            return _contentView as T;
        }

        public void DestroyContentView()
        {
            if (_contentView != null)
            {
                _contentView.RemoveFromSuperview();
                _contentView.Dispose();
                _contentView = null;
            }
        }

        private Type GetSelectedTabViewType()
        {
            switch (SelectedTab)
            {
                case Tabs.Worlds: return typeof(UIView);
                case Tabs.Favourites: return typeof(BDGWebView);
                default: return typeof(UIView);
            }
        }

		private CGPath ShadowPath()
		{
			var path = new CGPath();

			var points = new List<PointF>();

			if (_tabPosition == TabPosition.Left)
			{
				points.Add(this.Bounds.Location.Offset(Spacing, Spacing));
				points.Add(this.Bounds.Location.Offset(this.Bounds.Width, Spacing));
				points.Add(this.Bounds.Location.Offset(this.Bounds.Width, this.Bounds.Height - Spacing));
				points.Add(this.Bounds.Location.Offset(Spacing + TabWidth, this.Bounds.Height - Spacing));

				var yPos = Spacing + ((TabHeight + Spacing) * _tabs.Length) - Spacing;
				points.Add(this.Bounds.Location.Offset(Spacing + TabWidth, yPos));

				for (int i = _tabs.Length; i > 0; i--)
				{
					points.Add(this.Bounds.Location.Offset(Spacing, yPos));
					points.Add(this.Bounds.Location.Offset(Spacing, yPos - TabHeight));

					if (i > 1)
					{
						points.Add(this.Bounds.Location.Offset(Spacing + TabWidth, yPos - TabHeight));
						points.Add(this.Bounds.Location.Offset(Spacing + TabWidth, yPos - TabHeight - TabSpacing));
					}

					yPos -= (TabHeight + TabSpacing);
				}
			}
			else
			{
			}

			path.AddLines(points.ToArray());

			return path;
		}
    }
}

