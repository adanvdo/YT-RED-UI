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
        private int x;
        public int X { get { return x; } }

        private int y;
        public int Y { get { return y; } }

        public AspectRatio(int x, int y)
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
            int x = width / gcd;
            int y = height / gcd;
            return new AspectRatio(x, y);
        }

        public decimal ToDecimal()
        {
            return Math.Round((decimal)x / (decimal)y, 3);
        }

        public override string ToString()
        {
            return $"{X}:{Y}";
        }
    }
}
