using System;
using MonoTouch.UIKit;
using BlackDragon.Core.Entities;

namespace BlackDragon.Archive
{
    public class ContentMenuTableViewPageCell : UITableViewCell
    {
        public Page Page { get; private set; }

        public ContentMenuTableViewPageCell(UITableViewCellStyle style, string cellId, Page page)
            : base(style, cellId)
        {
            InitializeView();
            SetPage(page);
        }

        public ContentMenuTableViewPageCell(IntPtr handle)
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
        }

        public void SetPage(Page page)
        {
            Page = page;
            TextLabel.Text = Page.Title;
            DetailTextLabel.Text = Page.Subtitle;
        }
    }
}

