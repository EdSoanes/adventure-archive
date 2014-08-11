using MonoTouch.Foundation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BlackDragon.Fx.Extensions
{
    public static class NSDataExtensions
    {
        public static byte[] ToByteArray(this NSData data)
        {
            var dataBytes = new byte[data.Length];
            System.Runtime.InteropServices.Marshal.Copy(data.Bytes, dataBytes, 0, Convert.ToInt32(data.Length));
            return dataBytes;
        }
    }
}
