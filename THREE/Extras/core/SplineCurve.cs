using WebGL;

namespace THREE
{
	public class SplineCurve : Curve
	{
		public JSArray points;

		public SplineCurve(JSArray points = null)
		{
			this.points = points ?? new JSArray();
		}

		public override dynamic getPoint(double t)
		{
			var v = new Vector2();
			var c = new JSArray();
			var point = (points.length - 1) * t;

			var intPoint = (int)System.Math.Floor(point);
			var weight = point - intPoint;

			c[0] = intPoint == 0 ? intPoint : intPoint - 1;
			c[1] = intPoint;
			c[2] = intPoint > points.length - 2 ? points.length - 1 : intPoint + 1;
			c[3] = intPoint > points.length - 3 ? points.length - 1 : intPoint + 2;

			v.x = Utils.interpolate(points[c[0]].x, points[c[1]].x, points[c[2]].x, points[c[3]].x, weight);
			v.y = Utils.interpolate(points[c[0]].y, points[c[1]].y, points[c[2]].y, points[c[3]].y, weight);

			return v;
		}
	}
}