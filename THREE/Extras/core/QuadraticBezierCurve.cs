namespace THREE
{
	public class QuadraticBezierCurve : Curve
	{
		public Vector2 v0;
		public Vector2 v1;
		public Vector2 v2;

		public QuadraticBezierCurve(Vector2 v0, Vector2 v1, Vector2 v2)
		{
			this.v0 = v0;
			this.v1 = v1;
			this.v2 = v2;
		}

		public override dynamic getPoint(double t)
		{
			var tx = Shape.Utils.b2(t, v0.x, v1.x, v2.x);
			var ty = Shape.Utils.b2(t, v0.y, v1.y, v2.y);

			return new Vector2(tx, ty);
		}

		public override dynamic getTangent(double t)
		{
			var tx = Utils.tangentQuadraticBezier(t, v0.x, v1.x, v2.x);
			var ty = Utils.tangentQuadraticBezier(t, v0.y, v1.y, v2.y);

			return new Vector2(tx, ty).normalize();
		}
	}
}