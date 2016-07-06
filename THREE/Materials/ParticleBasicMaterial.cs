using WebGL;

namespace THREE
{
	public class ParticleBasicMaterial : Material
	{
		public Color color;
		public dynamic map;
		public dynamic vertexColors;
		public dynamic fog;
		public dynamic size;
		public dynamic sizeAttenuation;

		public ParticleBasicMaterial(dynamic parameters = null)
		{
			parameters = parameters ?? new JSObject();

			color = new Color(parameters.color ?? 0xffffff);

			map = parameters.map;

			size = parameters.size ?? 1.0;
			sizeAttenuation = parameters.sizeAttenuation ?? true;

			vertexColors = parameters.vertexColors ?? false;

			fog = parameters.fog ?? true;

			this.setValues(parameters);
		}

		public ParticleBasicMaterial clone()
		{
			var material = (ParticleBasicMaterial)clone(new ParticleBasicMaterial());

			material.color.copy(color);

			material.map = map;

			material.size = size;
			material.sizeAttenuation = sizeAttenuation;

			material.vertexColors = vertexColors;

			material.fog = fog;

			return material;
		}
	}
}
