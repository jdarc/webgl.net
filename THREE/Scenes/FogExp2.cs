namespace THREE
{
	public class FogExp2
	{
		public string name;
		public Color color;
		public double density;

		public FogExp2(int hex, double density = 0.00025)
		{
			name = "";
			color = new Color(hex);
			this.density = density;
		}

		public FogExp2 clone()
		{
			return new FogExp2(color.getHex(), density);
		}
	}
}
