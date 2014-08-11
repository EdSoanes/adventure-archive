using System;
using BlackDragon.Core.Entities;
using System.Drawing;

namespace BlackDragon.Fx
{
    public static class EntityExtensions
    {
        public static SizeF MapSize(this Map map)
        {
            var width = (float)Math.Ceiling((float)map.Width / Map.TileWidth) * Map.TileWidth;
            var height = (float)Math.Ceiling((float)map.Height / Map.TileHeight) * Map.TileHeight;
            return new SizeF(width, height);
        }
    }
}

