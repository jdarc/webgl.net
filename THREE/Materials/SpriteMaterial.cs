using WebGL;

namespace THREE
{
	public class SpriteMaterial : Material
	{
		public bool useScreenCoordinates;
		public bool sizeAttenuation;
		public bool scaleByViewport;
		public Vector2 alignment;
		public Vector2 uvOffset;
		public Vector2 uvScale;

		public Color color;
		public dynamic map;
		public dynamic fog;

		public SpriteMaterial(dynamic parameters = null)
		{
			parameters = parameters ?? new JSObject();

			color = new Color(parameters.color ?? 0xffffff);
			map = parameters.map ?? new Texture();

			useScreenCoordinates = parameters.useScreenCoordinates ?? true;
			depthTest = parameters.depthTest ?? !useScreenCoordinates;
			sizeAttenuation = parameters.sizeAttenuation ?? !useScreenCoordinates;
			scaleByViewport = parameters.scaleByViewport ?? !sizeAttenuation;
			alignment = new Vector2().copy(parameters.alignment ?? THREE.SpriteAlignment.center);

			fog = parameters.fog ?? false;

			uvOffset = parameters.uvOffset ?? new Vector2(0, 0);
			uvScale = parameters.uvScale ?? new Vector2(1, 1);

			setValues(parameters);

			if (parameters.depthTest == null)
			{
				depthTest = !useScreenCoordinates;
			}
			if (parameters.sizeAttenuation == null)
			{
				sizeAttenuation = !useScreenCoordinates;
			}
			if (parameters.scaleByViewport == null)
			{
				scaleByViewport = !sizeAttenuation;
			}
		}

		public SpriteMaterial clone()
		{
			var material = (SpriteMaterial)base.clone(new SpriteMaterial());

			material.color.copy(color);
			material.map = map;

			material.useScreenCoordinates = useScreenCoordinates;
			material.sizeAttenuation = sizeAttenuation;
			material.scaleByViewport = scaleByViewport;
			material.alignment.copy(alignment);

			material.uvOffset.copy(uvOffset);
			material.uvScale.copy(uvScale);

			material.fog = fog;

			return material;
		}
	}
}
