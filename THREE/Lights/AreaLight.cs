namespace THREE
{
	public class AreaLight : Light
	{
		public Vector3 normal;
		public Vector3 right;
		public double intensity;
		public double width;
		public double height;
		public double constantAttenuation;
		public double linearAttenuation;
		public double quadraticAttenuation;

		public AreaLight(int hex = 0xFFFFFF, double intensity = 1) : base(hex)
		{
			normal = new Vector3(0, -1, 0);
			right = new Vector3(1, 0, 0);

			this.intensity = intensity;

			width = 1.0;
			height = 1.0;

			constantAttenuation = 1.5;
			linearAttenuation = 0.5;
			quadraticAttenuation = 0.1;
		}
	}
}
