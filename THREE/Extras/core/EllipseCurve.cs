namespace THREE
{
	public class EllipseCurve : Curve
	{
		public double aX;
		public double aY;
		public double xRadius;
		public double yRadius;
		public double aStartAngle;
		public double aEndAngle;
		public bool aClockwise;

		public EllipseCurve(double aX, double aY, double xRadius, double yRadius,
		                    double aStartAngle, double aEndAngle, bool aClockwise)
		{
			this.aX = aX;
			this.aY = aY;

			this.xRadius = xRadius;
			this.yRadius = yRadius;

			this.aStartAngle = aStartAngle;
			this.aEndAngle = aEndAngle;

			this.aClockwise = aClockwise;
		}

		public override dynamic getPoint(double t)
		{
			var deltaAngle = aEndAngle - aStartAngle;

			if (!aClockwise)
			{
				t = 1.0 - t;
			}

			var angle = aStartAngle + t * deltaAngle;
			return new Vector2(aX + xRadius * System.Math.Cos(angle), aY + yRadius * System.Math.Sin(angle));
		}
	}
}