namespace THREE
{
	public class Light : Object3D
	{
		public Color color;
		public bool onlyShadow;

		public Light() : this(0xFFFFFF)
		{
		}

		public Light(int hex)
		{
			color = new Color(hex);
		}
	}
}
