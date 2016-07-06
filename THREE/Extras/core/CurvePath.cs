using System;
using WebGL;

namespace THREE
{
	public class CurvePath : Curve
	{
		public JSArray curves;
		public JSArray bends;
		public bool autoClose;
		public JSArray cacheLengths;

		public CurvePath()
		{
			curves = new JSArray();
			bends = new JSArray();

			autoClose = false;
		}

		public virtual void add(Curve curve)
		{
			curves.push(curve);
		}

		public virtual void checkConnection()
		{
		}

		public virtual void closePath()
		{
			var startPoint = curves[0].getPoint(0);
			var endPoint = curves[curves.length - 1].getPoint(1);

			if (!startPoint.equals(endPoint))
			{
				curves.push(new LineCurve(endPoint, startPoint));
			}
		}

		public override dynamic getPoint(double t)
		{
			var d = t * getLength();
			var curveLengths = getCurveLengths();
			var i = 0;

			while (i < curveLengths.length)
			{
				if (curveLengths[i] >= d)
				{
					double diff = curveLengths[i] - d;
					var curve = curves[i];
					return curve.getPointAt(1.0 - diff / curve.getLength());
				}
				i++;
			}

			return null;
		}

		public override double getLength()
		{
			var lens = getCurveLengths();
			return lens[lens.length - 1];
		}

		public virtual JSArray getCurveLengths()
		{
			if (cacheLengths != null && cacheLengths.length == curves.length)
			{
				return cacheLengths;
			}

			double sums = 0;
			var lengths = new JSArray();
			for (var i = 0; i < curves.length; i++)
			{
				sums += curves[i].getLength();
				lengths.push(sums);
			}

			cacheLengths = lengths;

			return lengths;
		}

		public virtual dynamic getBoundingBox()
		{
			var points = getPoints();

			double maxY;
			double maxZ;
			double minY;
			double minZ;

			var maxX = maxY = maxZ = double.NegativeInfinity;
			var minX = minY = minZ = double.PositiveInfinity;

			var v3 = points[0] is Vector3;

			var sum = v3 ? (dynamic)new Vector3() : new Vector2();

			var il = points.length;
			for (var i = 0; i < il; i++)
			{
				var p = points[i];

				if (p.x > maxX)
				{
					maxX = p.x;
				}
				else if (p.x < minX)
				{
					minX = p.x;
				}

				if (p.y > maxY)
				{
					maxY = p.y;
				}
				else if (p.y < minY)
				{
					minY = p.y;
				}

				if (v3)
				{
					if (p.z > maxZ)
					{
						maxZ = p.z;
					}
					else if (p.z < minZ)
					{
						minZ = p.z;
					}
				}

				sum.add(p);
			}

			var ret = JSObject.create(new {minX, minY, maxX, maxY, centroid = sum.divideScalar(il)});
			if (v3)
			{
				ret.maxZ = maxZ;
				ret.minZ = minZ;
			}

			return ret;
		}

		public virtual Geometry createPointsGeometry(double divisions = double.NaN)
		{
			return createGeometry(getPoints(divisions, true));
		}

		public virtual Geometry createSpacedPointsGeometry(double divisions)
		{
			return createGeometry(getSpacedPoints(divisions, true));
		}

		public virtual Geometry createGeometry(JSArray points)
		{
			var geometry = new Geometry();

			for (var i = 0; i < points.length; i++)
			{
				var point = points[i];
				if (point is Vector2)
				{
					geometry.vertices.push(new Vector3(point.x, point.y, 0.0));
				}
				else
				{
					geometry.vertices.push(new Vector3(point.x, point.y, point.z));
				}
			}

			return geometry;
		}

		public virtual void addWrapPath(dynamic bendpath)
		{
			throw new NotImplementedException();

			bends.push(bendpath);
		}

		public virtual JSArray getTransformedPoints(double segments, JSArray bendz = null)
		{
			var oldPts = getPoints(segments);

			if (bendz == null)
			{
				bendz = bends;
			}

			for (int i = 0, il = bendz.length; i < il; i++)
			{
				oldPts = this.getWrapPoints(oldPts, bendz[i]);
			}

			return oldPts;
		}

		public virtual JSArray getTransformedSpacedPoints(double segments, JSArray bendz = null)
		{
			throw new NotImplementedException();

			var oldPts = getSpacedPoints(segments);

			dynamic i, il;

			if (bendz == null)
			{
				bendz = bends;
			}

			for (i = 0, il = bendz.length; i < il; i++)
			{
				oldPts = this.getWrapPoints(oldPts, bendz[i]);
			}

			return oldPts;
		}

		public virtual JSArray getWrapPoints(JSArray oldPts, Path path)
		{
			throw new NotImplementedException();
		}
	}
}
