using System;
using System.Collections.Generic;
using MonoTouch.UIKit;
using BlackDragon.Core.Entities;

namespace BlackDragon.Archive
{
    public class ContentMenuTableViewSource : UITableViewSource
    {
        private const string CellId = "ContentNavCellId";

        private List<Chapter> _chapters;
        public event EventHandler<PageSelectedEventArgs> PageSelected;

        public ContentMenuTableViewSource(List<Chapter> chapters)
        {
            _chapters = chapters;
        }

        public override int NumberOfSections(UITableView tableView)
        {
            return _chapters.Count;
        }

        public override string TitleForHeader(UITableView tableView, int section)
        {
            return _chapters[section].Title;
        }

        public override int RowsInSection (UITableView tableview, int section)
        {
            var chapter = _chapters[section];
            return chapter.Pages.Count;
        }

        public override void RowSelected(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
        {
            var chapter = _chapters[indexPath.Section];
            var page = chapter.Pages[indexPath.Row];
            if (PageSelected != null)
                PageSelected.Invoke(this, new PageSelectedEventArgs(page));
        }

        public override UITableViewCell GetCell(UITableView tableView, MonoTouch.Foundation.NSIndexPath indexPath)
        {
            var chapter = _chapters[indexPath.Section];
            var page = chapter.Pages[indexPath.Row];

            // if there are no cells to reuse, create a new one
            var cell = tableView.DequeueReusableCell(CellId) as ContentMenuTableViewPageCell;
            if (cell == null)
                cell = new ContentMenuTableViewPageCell(UITableViewCellStyle.Subtitle, CellId, page);
            else
                cell.SetPage(page);

            return cell;
        }
    }


    public class PageSelectedEventArgs : EventArgs
    {
        public Page Page
        {
            get;
            private set;
        }

        public PageSelectedEventArgs(Page page)
        {
            Page = page;
        }
    }
}

