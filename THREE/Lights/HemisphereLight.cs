namespace THREE
{
	public class HemisphereLight : Light
	{
		public Color groundColor;
		public double intensity;

		public HemisphereLight(int skyColorHex = 0xFFFFFF, int groundColorHex = 0x888888, double intensity = 1.0) : base(skyColorHex)
		{
			groundColor = new Color(groundColorHex);

			position = new Vector3(0.0, 100.0, 0.0);

			this.intensity = intensity;
		}
	}
}
