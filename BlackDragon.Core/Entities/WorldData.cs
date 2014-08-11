using System;
using System.Collections.Generic;
using System.Linq;

namespace BlackDragon.Core.Entities
{
	public class WorldData
	{
		public WorldData ()
		{
			Worlds = new List<World>();
		}

		public List<World> Worlds
		{
			get;
			set;
		}
	}
}

