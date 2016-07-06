namespace THREE
{
	public class LineCurve : Curve
	{
		public Vector2 v1;
		public Vector2 v2;

		public LineCurve(Vector2 v1, Vector2 v2)
		{
			this.v1 = v1;
			this.v2 = v2;
		}

		public override dynamic getPoint(double t)
		{
			return v2.clone().sub(v1).multiplyScalar(t).add(v1);
		}

		public override dynamic getPointAt(double u)
		{
			return getPoint(u);
		}

		public override dynamic getTangent(double t)
		{
			return v2.clone().sub(v1).normalize();
		}
	}
}