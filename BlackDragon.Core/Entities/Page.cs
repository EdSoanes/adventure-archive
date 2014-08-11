using System;
using Newtonsoft.Json;

namespace BlackDragon.Core.Entities
{
	public class Page : IdentifiableItem
	{
		public Page ()
		{
		}

		public string MapContentPath
		{
			get;
			set;
		}

		[JsonIgnore]
		public Map Map
		{
			get;
			set;
		}
	}
}

