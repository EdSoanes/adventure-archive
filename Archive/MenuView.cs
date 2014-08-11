using System;
using System.Drawing;

using MonoTouch.CoreGraphics;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

using BlackDragon.Fx.Extensions;
using BlackDragon.Archive.Views;
using BlackDragon.Core.Entities;

namespace BlackDragon.Archive
{
    [Register("MenuView")]
    public class MenuView : UIView
    {
        private WeakReference<MenuAccessView> _menuAccessView;
        public MenuAccessView MenuAccessView
        {
            get
            {
                MenuAccessView vw;
                _menuAccessView.TryGetTarget(out vw);
                return vw;
            }
            set
            {
                _menuAccessView = new WeakReference<MenuAccessView>(value);
            }
        }

		private WeakReference<TabPopoutView> _tabPopout;
		public TabPopoutView TabPopout
		{
			get
			{
				TabPopoutView vw;
				_tabPopout.TryGetTarget(out vw);
				return vw;
			}
			set
			{
				_tabPopout = new WeakReference<TabPopoutView>(value);
			}
		}

        private const float Padding = 40f;
        public MenuView()
        {
            Initialize();
        }

        public MenuView(RectangleF bounds)
            : base(bounds)
        {
            Initialize();
        }

        void Initialize()
        {
            //var size = MenuAccessView.ViewSize;
            //var pos = new PointF(Padding, this.Frame.Height - size.Height - Padding);
            
            //var menuAccessView = new MenuAccessView();
            //menuAccessView.Frame = new RectangleF(pos, size);
            //menuAccessView.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleLeftMargin;
            //this.AddSubview(menuAccessView);

            //MenuAccessView = menuAccessView;

            //var frame = new RectangleF(pos.Offset(size.Width, 0), size);
            //var worldManagementView = new WorldManagementView(frame, new WorldManagementView.Tabs[] { WorldManagementView.Tabs.Worlds });
            //worldManagementView.AutoresizingMask = UIViewAutoresizing.FlexibleTopMargin | UIViewAutoresizing.FlexibleLeftMargin;
            //this.AddSubview(worldManagementView);

            //WorldManagementView = worldManagementView;
        }

        public override void LayoutSubviews()
        {
            string src = this.IsLandscape() ? "Images/bg-h.jpg" : "Images/bg-v.jpg";
            using (UIImage img = UIImage.FromFile(src))
                BackgroundColor = UIColor.FromPatternImage(img);

            base.LayoutSubviews();
        }

        public void SetUserData(UserData userData)
        {
        }
    }
}