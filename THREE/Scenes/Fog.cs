namespace THREE
{
	public class Fog
	{
		public string name;
		public Color color;
		public double near;
		public double far;

		public Fog(int hex, double near = 1.0, double far = 1000.0)
		{
			name = "";
			color = new Color(hex);
			this.near = near;
			this.far = far;
		}

		public Fog clone()
		{
			return new Fog(color.getHex(), near, far);
		}
	}
}
