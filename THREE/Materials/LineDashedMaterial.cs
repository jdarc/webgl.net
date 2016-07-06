using WebGL;

namespace THREE
{
	public class LineDashedMaterial : Material
	{
		public Color color;
		public dynamic linewidth;
		public dynamic scale;
		public dynamic dashSize;
		public dynamic gapSize;
		public dynamic vertexColors;
		public dynamic fog;

		public LineDashedMaterial(dynamic parameters = null)
		{
			parameters = parameters ?? new JSObject();

			color = new Color(parameters.color ?? 0xffffff);

			linewidth = parameters.linewidth ?? 1;

			scale = parameters.scale ?? 1;
			dashSize = parameters.dashSize ?? 3;
			gapSize = parameters.gapSize ?? 1;

			vertexColors = parameters.vertexColors ?? false;

			fog = parameters.fog ?? true;

			setValues(parameters);
		}

		public LineDashedMaterial clone()
		{
			var material = (LineDashedMaterial)clone(new LineDashedMaterial());

			material.color.copy(color);

			material.linewidth = linewidth;

			material.scale = scale;
			material.dashSize = dashSize;
			material.gapSize = gapSize;

			material.vertexColors = vertexColors;

			material.fog = fog;

			return material;
		}
	}
}
