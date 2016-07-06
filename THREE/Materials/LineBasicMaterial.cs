using WebGL;

namespace THREE
{
	public class LineBasicMaterial : Material
	{
		public Color color;
		public double linewidth;
		public string linecap;
		public string linejoin;
		public dynamic vertexColors;
		public bool fog;

		public LineBasicMaterial(dynamic parameters = null)
		{
			parameters = parameters ?? new JSObject();

			color = new Color(parameters.color ?? 0xffffff);

			linewidth = parameters.linewidth ?? 1.0;
			linecap = parameters.linecap ?? "round";
			linejoin = parameters.linejoin ?? "round";

			vertexColors = parameters.vertexColors ?? false;

			fog = parameters.fog ?? true;

			this.setValues(parameters);
		}

		public LineBasicMaterial clone()
		{
			var material = (LineBasicMaterial)clone(new LineBasicMaterial());

			material.color.copy(color);

			material.linewidth = linewidth;
			material.linecap = linecap;
			material.linejoin = linejoin;

			material.vertexColors = vertexColors;

			material.fog = fog;

			return material;
		}
	}
}
