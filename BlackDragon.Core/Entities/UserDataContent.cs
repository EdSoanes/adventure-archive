using System;
using System.Drawing;
using Newtonsoft.Json;

namespace BlackDragon.Core.Entities
{
	public class UserDataContent
	{
		public UserDataContent()
		{
			X = -1;
			Y = -1;
			ZoomScale = -1;
		}

        public string ContentPath
        {
            get;
            set;
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

		public float ZoomScale
		{
			get;
			set;
		}

		public bool HasLocation
		{
			get { return X > -1 && Y > -1 && ZoomScale > -1; }
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

		[JsonIgnore]
		public Point ZoomedScreenPoint
		{
			get
			{
				return new Point(X * ZoomScale, Y * ZoomScale);
			}
		}

		public override string ToString()
		{
			return string.Format("[ScreenLocation: X={0}, Y={1}, ZoomScale={2}, HasLocation={3}, Point={4}, ZoomedScreenPoint={5}]", X, Y, ZoomScale, HasLocation, Point, ZoomedScreenPoint);
		}
	}
}

