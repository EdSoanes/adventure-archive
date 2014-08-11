using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackDragon.Archive
{
	public class NavigationItem
	{
		public string Title { get; private set; }
		public object Content { get; private set; }
		public List<NavigationItem> Items { get; private set; }
		public bool IsSelected { get; set; }

		public NavigationItem(string title, object content)
		{
			Title = title;
			Content = content;
		}

		public NavigationItem(string title, object content, IEnumerable<NavigationItem> items)
			: this(title, content)
		{
			Items = items.ToList();
		}
	}

	public class NavigationItemSelectedEventArgs : EventArgs
	{
		public NavigationItem Item
		{
			get;
			private set;
		}

		public int Level
		{
			get;
			private set;
		}

		public NavigationItemSelectedEventArgs(NavigationItem item, int level)
		{
			Item = item;
			Level = level;
		}
	}
}

