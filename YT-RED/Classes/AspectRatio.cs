using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using YTR.Utils;

namespace YTR.Classes
{
    public class AspectRatio
    {
        private byte x;
        public byte X { get { return x; } }

        private byte y;
        public byte Y { get { return y; } }

        public AspectRatio(byte x, byte y)
        {
            this.x = x;
            this.y = y;
        }

        public static AspectRatio FromDimensions(Size size)
        {
            return FromDimensions(size.Width, size.Height);
        }

        public static AspectRatio FromDimensions(int width, int height)
        {
            var gcd = MathUtil.GreatestCommonDivisor(width, height);
            byte x = (byte)(width / gcd);
            byte y = (byte)(height / gcd);
            return new AspectRatio(x, y);
        }

        public decimal ToDecimal()
        {
            return (decimal)x / (decimal)y;
        }

        public override string ToString()
        {
            return $"{X}:{Y}";
        }
    }
}
