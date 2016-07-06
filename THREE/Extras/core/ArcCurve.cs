namespace THREE
{
	public class ArcCurve : EllipseCurve
	{
		public ArcCurve(double aX, double aY, double aRadius, double aStartAngle, double aEndAngle, bool aClockwise)
			: base(aX, aY, aRadius, aRadius, aStartAngle, aEndAngle, aClockwise)
		{
		}
	}
}