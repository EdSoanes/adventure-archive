using System;

namespace BlackDragon.Core
{
    public static class IntExtensions
    {
        public static bool IsOdd(this int value)
        {
            return value % 2 != 0;
        } 
    }
}

