namespace THREE
{
	public class CubicBezierCurve : Curve
	{
		public Vector2 v0;
		public Vector2 v1;
		public Vector2 v2;
		public Vector2 v3;

		public CubicBezierCurve(Vector2 v0, Vector2 v1, Vector2 v2, Vector2 v3)
		{
			this.v0 = v0;
			this.v1 = v1;
			this.v2 = v2;
			this.v3 = v3;
		}

		public override dynamic getPoint(double t)
		{
			return new Vector2(Shape.Utils.b3(t, v0.x, v1.x, v2.x, v3.x),
			                   Shape.Utils.b3(t, v0.y, v1.y, v2.y, v3.y));
		}

		public override dynamic getTangent(double t)
		{
			return new Vector2(Utils.tangentCubicBezier(t, v0.x, v1.x, v2.x, v3.x),
			                   Utils.tangentCubicBezier(t, v0.y, v1.y, v2.y, v3.y)).normalize();
		}
	}
}
