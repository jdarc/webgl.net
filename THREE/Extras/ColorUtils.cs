namespace THREE
{
	public static class ColorUtils
	{
		public static void adjustHSV(Color color, double h, double s, double v)
		{
			var hsv = new double[3];

			color.getHSV(hsv);

			hsv[0] = Math.clamp(hsv[0] + h, 0, 1);
			hsv[1] = Math.clamp(hsv[1] + s, 0, 1);
			hsv[2] = Math.clamp(hsv[2] + v, 0, 1);

			color.setHSV(hsv[0], hsv[1], hsv[2]);
		}
	}
}
