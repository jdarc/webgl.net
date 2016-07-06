namespace THREE
{
	public class PointLight : Light
	{
		public double intensity;
		public double distance;

		public PointLight(int hex = 0xFFFFFF, double intensity = 1.0, double distance = 0.0) : base(hex)
		{
			position = new Vector3(0.0, 0.0, 0.0);
			this.intensity = intensity;
			this.distance = distance;
		}
	}
}
