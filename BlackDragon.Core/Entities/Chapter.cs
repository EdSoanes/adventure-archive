using System;
using System.Collections.Generic;

namespace BlackDragon.Core.Entities
{
	public class Chapter : IdentifiableItem
	{
		public Chapter ()
		{
			Pages = new List<Page>();
		}

		public List<Page> Pages
		{
			get;
			set;
		}
	}
}

