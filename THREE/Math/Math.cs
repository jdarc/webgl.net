using System;

namespace THREE
{
	public static class Math
	{
        private static readonly Random Rnd = new Random(Environment.TickCount);
		private const double __d2r = System.Math.PI / 180.0;
		private const double __r2d = 180.0 / System.Math.PI;
		public static double LN2 = System.Math.Log(2);

		public static double clamp(double x, double a, double b)
		{
			return (x < a) ? a : ((x > b) ? b : x);
		}

		public static double clampBottom(double x, double a)
		{
			return x < a ? a : x;
		}

		public static double mapLinear(double x, double a1, double a2, double b1, double b2)
		{
			return b1 + (x - a1) * (b2 - b1) / (a2 - a1);
		}

		public static double random16()
		{
			return (65280.0 * random() + 255.0 * random()) / 65535.0;
		}

		public static double randInt(double low, double high)
		{
			return low + System.Math.Floor(random() * (high - low + 1));
		}

		public static double randFloat(double low, double high)
		{
			return low + random() * (high - low);
		}

		public static double randFloatSpread(double range)
		{
			return range * (0.5 - random());
		}

		public static double sign(double x)
		{
			return (x < 0) ? -1 : ((x > 0) ? 1 : 0);
		}

		public static double degToRad(double degrees)
		{
			return degrees * __d2r;
		}

		public static double radToDeg(double radians)
		{
			return radians * __r2d;
		}

		public static double random()
		{
			return Rnd.NextDouble();
		}

	    public static double abs(double o)
	    {
	        return System.Math.Abs(o);
	    }

        public static double sin(double o)
	    {
	        return System.Math.Sin(o);
	    }

	    public static double sqrt(double o)
	    {
	        return System.Math.Sqrt(o);
	    }

        public static dynamic min(dynamic a, dynamic b)
	    {
            return System.Math.Min(a, b);
        }

	    public static dynamic max(dynamic a, dynamic b)
	    {
	        return System.Math.Max(a, b);
	    }

	    public static double floor(double d)
	    {
            return System.Math.Floor(d);
	    }

	    public static double round(double d)
	    {
            return System.Math.Round(d);
        }
	}
}
