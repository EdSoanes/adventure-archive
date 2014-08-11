using System;
using System.Collections.Generic;

namespace BlackDragon.Core.Entities
{
	public class World : IdentifiableItem
	{
		public World ()
		{
            Pages = new List<Page>();
            Adventures = new List<Adventure>();
            References = new List<Reference>();
		}

		public string Copyright
		{
			get;
			set;
		}

		public Map Map 
		{
			get;
			set;
		}

        public List<Page> Pages
        {
            get;
            set;
        }

        public List<Adventure> Adventures
        {
            get;
            set;
        }

		public List<Reference> References
		{
			get;
			set;
		}
	}
}

