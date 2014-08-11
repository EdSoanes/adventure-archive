using System;

namespace BlackDragon.Fx.Extensions
{
	public static class FloatExtensions
	{
		public static float DegreesToRadians(this float degrees)
		{
			return (((float)Math.PI) * degrees / 180);
		}
	}
}

