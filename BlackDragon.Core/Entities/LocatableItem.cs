using System;
using Newtonsoft.Json;
using System.Drawing;

namespace BlackDragon.Core.Entities
{
	public class LocatableItem : IdentifiableItem
	{
		public LocatableItem ()
		{

		}

		public float X
		{
			get;
			set;
		}
		
		public float Y
		{
			get;
			set;
		}

		public bool Locked
		{
			get;
			set;
		}
		
		[JsonIgnore]
		public Point Point
		{
			get
			{
				return new Point(X, Y);
			}
			set
			{
				X = value.X;
				Y = value.Y;
			}
		}
	}
}

