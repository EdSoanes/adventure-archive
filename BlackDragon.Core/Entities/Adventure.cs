using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace BlackDragon.Core.Entities
{
	public class Adventure : IdentifiableItem
	{
		public Adventure ()
		{
			Chapters = new List<Chapter>();
			Maps = new List<Map>();
			References = new List<Reference>();
		}

        public string RpgSystem
        {
            get;
            set;
        }

		public string Copyright
		{
			get;
			set;
		}

		public List<Chapter> Chapters
		{
			get;
			set;
		}

		public List<Map> Maps
		{
			get;
			set;
		}

		public List<Reference> References
		{
			get;
			set;
		}

		internal List<Page> GetAllPages()
		{
			return Chapters.SelectMany(x => x.Pages).ToList();
		}

		internal Chapter GetChapterForPage(Page page)
		{
			return Chapters.FirstOrDefault(x => x.Pages.Select(y => y.ContentPath).Contains(page.ContentPath));
		}

		internal Page GetPageAfter(Page page)
		{
			var chapter = GetChapterForPage(page);
			if (chapter != null)
			{
				var pos = chapter.Pages.IndexOf(page);
				if (pos == chapter.Pages.Count - 1)
				{
					var pos2 = Chapters.IndexOf(chapter);
					if (pos2 < Chapters.Count - 1)
						return Chapters[pos2 + 1].Pages.FirstOrDefault();
					//Get next chapter
				}
				else
					return chapter.Pages[pos + 1];
			}

			return null;
		}

		internal Page GetPageBefore(Page page)
		{
			var chapter = GetChapterForPage(page);
			if (chapter != null)
			{
				var pos = chapter.Pages.IndexOf(page);
				if (pos == 0)
				{
					var pos2 = Chapters.IndexOf(chapter);
					if (pos2 > 0)
						return Chapters[pos2 - 1].Pages.LastOrDefault();
				}
				else
					return chapter.Pages[pos - 1];
			}

			return null;
		}
	}
}

