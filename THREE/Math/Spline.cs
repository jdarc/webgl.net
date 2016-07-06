using WebGL;

namespace THREE
{
	public class Spline
	{
		public JSArray points;

		public Spline(JSArray points)
		{
			this.points = points;
		}

		public void initFromArray(double[][] a)
		{
			points = new JSArray(a.Length);

			for (var i = 0; i < a.Length; i++)
			{
				points[i] = new Vector3(a[i][0], a[i][1], a[i][2]);
			}
		}

		public Vector3 getPoint(double k)
		{
			var point = (points.length - 1) * k;
			var intPoint = (int)System.Math.Floor(point);
			var weight = point - intPoint;

			var c = new int[4];
			c[0] = intPoint == 0 ? intPoint : intPoint - 1;
			c[1] = intPoint;
			c[2] = intPoint > points.length - 2 ? points.length - 1 : intPoint + 1;
			c[3] = intPoint > points.length - 3 ? points.length - 1 : intPoint + 2;

			var pa = points[c[0]];
			var pb = points[c[1]];
			var pc = points[c[2]];
			var pd = points[c[3]];

			var w2 = weight * weight;
			var w3 = weight * w2;

			return new Vector3
			{
				x = interpolate(pa.x, pb.x, pc.x, pd.x, weight, w2, w3),
				y = interpolate(pa.y, pb.y, pc.y, pd.y, weight, w2, w3),
				z = interpolate(pa.z, pb.z, pc.z, pd.z, weight, w2, w3)
			};
		}

		public Vector3[] getControlPointsArray()
		{
			var coords = new Vector3[points.length];
			for (var i = 0; i < points.length; i++)
			{
				var p = points[i];
				coords[i] = new Vector3(p.x, p.y, p.z);
			}

			return coords;
		}

		public dynamic getLength(int nSubDivisions = 100)
		{
			var oldIntPoint = 0;
			var oldPosition = new Vector3();
			var tmpVec = new Vector3();
			var chunkLengths = new JSArray();
			double totalLength = 0;

			chunkLengths[0] = 0;

			var nSamples = points.length * nSubDivisions;

			oldPosition.copy(points[0]);

			for (var i = 1; i < nSamples; i++)
			{
				var index = i / nSamples;

				var position = getPoint(index);
				tmpVec.copy(position);

				totalLength += tmpVec.distanceTo(oldPosition);

				oldPosition.copy(position);

				var point = (points.length - 1) * index;
				var intPoint = (int)System.Math.Floor((double)point);

				if (intPoint != oldIntPoint)
				{
					chunkLengths[intPoint] = totalLength;
					oldIntPoint = intPoint;
				}
			}

			chunkLengths[chunkLengths.length] = totalLength;

			return new {chunks = chunkLengths, total = totalLength};
		}

		public void reparametrizeByArcLength(double samplingCoef)
		{
			var newpoints = new JSArray();
			var tmpVec = new Vector3();
			var sl = getLength();

			newpoints.push((dynamic[])tmpVec.copy(points[0]).clone());

			for (var i = 1; i < points.length; i++)
			{
				var realDistance = sl.chunks[i] - sl.chunks[i - 1];

				var sampling = (int)System.Math.Ceiling(samplingCoef * realDistance / sl.total);

				var indexCurrent = (i - 1) / (points.length - 1);
				var indexNext = i / (points.length - 1);

				for (var j = 1; j < sampling - 1; j++)
				{
					var index = indexCurrent + j * (1 / sampling) * (indexNext - indexCurrent);
					var position = getPoint(index);
					newpoints.push(tmpVec.copy(position).clone());
				}

				newpoints.push((dynamic[])tmpVec.copy(points[i]).clone());
			}

			points = newpoints;
		}

		private static double interpolate(double p0, double p1, double p2, double p3, double t, double t2, double t3)
		{
			double v0 = (p2 - p0) * 0.5, v1 = (p3 - p1) * 0.5;
			return (2 * (p1 - p2) + v0 + v1) * t3 + (- 3 * (p1 - p2) - 2 * v0 - v1) * t2 + v0 * t + p1;
		}
	}
}
