using System;
using System.Drawing;
using Newtonsoft.Json;

namespace BlackDragon.Core.Entities
{
	public class MapMarker : LocatableItem
	{
		public MapMarker ()
		{
		}

		public int Number
		{
			get;
			set;
		}

		public string Type
		{
			get;
			set;
		}

		[JsonIgnore]
		public string ImagePath
		{
			get;
			set;
		}
	}
}

