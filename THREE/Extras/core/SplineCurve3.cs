using WebGL;

namespace THREE
{
	public class SplineCurve3 : Curve
	{
		public JSArray points;

		public SplineCurve3(JSArray points = null)
		{
			this.points = points ?? new JSArray();
		}

		public override dynamic getPoint(double t)
		{
			var v = new Vector3();
			var c = new JSArray();
			var point = (points.length - 1) * t;

			var intPoint = (int)System.Math.Floor(point);
			var weight = point - intPoint;

			c[0] = intPoint == 0 ? intPoint : intPoint - 1;
			c[1] = intPoint;
			c[2] = intPoint > points.length - 2 ? points.length - 1 : intPoint + 1;
			c[3] = intPoint > points.length - 3 ? points.length - 1 : intPoint + 2;

			var pt0 = points[c[0]];
			var pt1 = points[c[1]];
			var pt2 = points[c[2]];
			var pt3 = points[c[3]];

			v.x = Utils.interpolate(pt0.x, pt1.x, pt2.x, pt3.x, weight);
			v.y = Utils.interpolate(pt0.y, pt1.y, pt2.y, pt3.y, weight);
			v.z = Utils.interpolate(pt0.z, pt1.z, pt2.z, pt3.z, weight);

			return v;
		}
	}
}