using WebGL;

namespace THREE
{
	public class Curve
	{
		public JSArray cacheArcLengths;
		public bool needsUpdate;

		public virtual dynamic getPoint(double t)
		{
			JSConsole.log("Warning, getPoint() not implemented!");
			return null;
		}

		public virtual dynamic getPointAt(double u)
		{
			return getPoint(getUtoTmapping(u));
		}

		public virtual JSArray getPoints(double divisions = double.NaN, bool closedPath = false)
		{
			if (double.IsNaN(divisions))
			{
				divisions = 5.0;
			}

			var pts = new JSArray();

			for (var d = 0; d <= divisions; d++)
			{
				pts.push(getPoint(d / divisions));
			}

			return pts;
		}

		public virtual JSArray getSpacedPoints(double divisions = double.NaN, bool closedPath = false)
		{
			if (double.IsNaN(divisions))
			{
				divisions = 5.0;
			}
			var pts = new JSArray();

			for (double d = 0; d <= divisions; d++)
			{
				pts.push(getPointAt(d / divisions));
			}

			return pts;
		}

		public virtual double getLength()
		{
			var lengths = getLengths();
			return lengths[lengths.length - 1];
		}

		public virtual JSArray getLengths(double divisions = double.NaN)
		{
			if (double.IsNaN(divisions))
			{
				divisions = 200.0;
			}

			if (cacheArcLengths != null && (cacheArcLengths.length == divisions + 1) && !needsUpdate)
			{
				return cacheArcLengths;
			}

			needsUpdate = false;

			var cache = new JSArray();
			var last = getPoint(0);
			double sum = 0;

			cache.push(0);

			for (var p = 1; p <= divisions; p++)
			{
				var current = getPoint(p / divisions);
				sum += current.distanceTo(last);
				cache.push(sum);
				last = current;
			}

			cacheArcLengths = cache;

			return cache;
		}

		public virtual void updateArcLengths()
		{
			needsUpdate = true;
			getLengths();
		}

		public virtual double getUtoTmapping(double u, double distance = double.NaN)
		{
			var arcLengths = getLengths();

			int i;
			var il = arcLengths.length;

			double targetArcLength;

			if (!double.IsNaN(distance))
			{
				targetArcLength = distance;
			}
			else
			{
				targetArcLength = u * arcLengths[il - 1];
			}

			var low = 0;
			var high = il - 1;

			while (low <= high)
			{
				i = (int)System.Math.Floor(low + (high - low) / 2.0);

				var comparison = arcLengths[i] - targetArcLength;

				if (comparison < 0)
				{
					low = i + 1;
					continue;
				}
				else if (comparison > 0)
				{
					high = i - 1;
					continue;
				}
				else
				{
					high = i;
					break;
				}
			}

			i = high;

			if (arcLengths[i] == targetArcLength)
			{
				return i / (double)(il - 1);
			}

			double lengthBefore = arcLengths[i];
			double lengthAfter = arcLengths[i + 1];

			var segmentLength = lengthAfter - lengthBefore;

			var segmentFraction = (targetArcLength - lengthBefore) / segmentLength;

			return (i + segmentFraction) / (il - 1);
		}

		public virtual dynamic getTangent(double t)
		{
			const double delta = 0.0001;
			var t1 = t - delta;
			var t2 = t + delta;

			if (t1 < 0.0)
			{
				t1 = 0.0;
			}
			if (t2 > 1.0)
			{
				t2 = 1.0;
			}

			var pt1 = getPoint(t1);
			var pt2 = getPoint(t2);

			return pt2.clone().sub(pt1).normalize();
		}

		public virtual dynamic getTangentAt(double u)
		{
			return getTangent(getUtoTmapping(u));
		}

		public static class Utils
		{
			public static double tangentQuadraticBezier(double t, double p0, double p1, double p2)
			{
				return 2 * (1 - t) * (p1 - p0) + 2 * t * (p2 - p1);
			}

			public static double tangentCubicBezier(double t, double p0, double p1, double p2, double p3)
			{
				return -3 * p0 * (1 - t) * (1 - t) +
				       3 * p1 * (1 - t) * (1 - t) - 6 * t * p1 * (1 - t) +
				       6 * t * p2 * (1 - t) - 3 * t * t * p2 +
				       3 * t * t * p3;
			}

			public static double tangentSpline(double t, double p0, double p1, double p2, double p3)
			{
				var h00 = 6 * t * t - 6 * t;
				var h10 = 3 * t * t - 4 * t + 1;
				var h01 = -6 * t * t + 6 * t;
				var h11 = 3 * t * t - 2 * t;

				return h00 + h10 + h01 + h11;
			}

			public static double interpolate(double p0, double p1, double p2, double p3, double t)
			{
				var v0 = (p2 - p0) * 0.5;
				var v1 = (p3 - p1) * 0.5;
				var t2 = t * t;
				var t3 = t * t2;
				return (2 * p1 - 2 * p2 + v0 + v1) * t3 + (-3 * p1 + 3 * p2 - 2 * v0 - v1) * t2 + v0 * t + p1;
			}
		}
	}
}
