using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlackDragon.Core.Entities
{
    public struct Point
    {
        public Point(float x, float y)
            : this()
        {
            X = x;
            Y = y;
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
    }

}
