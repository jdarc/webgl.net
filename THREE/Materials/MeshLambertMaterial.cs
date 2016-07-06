using WebGL;

namespace THREE
{
	public class MeshLambertMaterial : Material
	{
		public Color ambient;
		public Color emissive;
		public bool wrapAround;
		public Vector3 wrapRGB;
		public int combine;
		public double reflectivity;
		public double refractionRatio;
		public bool wireframe;
		public double wireframeLinewidth;
		public string wireframeLinecap;
		public string wireframeLinejoin;
		public bool skinning;
		public bool morphTargets;
		public bool morphNormals;

		public Color color;
		public dynamic map;
		public dynamic lightMap;
		public dynamic specularMap;
		public dynamic envMap;
		public dynamic fog;
		public dynamic shading;
		public dynamic vertexColors;

		public MeshLambertMaterial(dynamic parameters = null)
		{
			parameters = parameters ?? new JSObject();

			color = new Color((int)(parameters.color ?? 0xffffff));

			ambient = new Color((int)(parameters.ambient ?? 0xffffff));
			emissive = new Color((int)(parameters.emissive ?? 0x000000));

			wrapAround = (bool)(parameters.wrapAround ?? false);
			wrapRGB = new Vector3().copy(parameters.wrapRGB ?? new Vector3(1, 1, 1));

			map = parameters.map;
			lightMap = parameters.lightMap;
			specularMap = parameters.specularMap;
			envMap = parameters.envMap;

			combine = (int)(parameters.combine ?? THREE.MultiplyOperation);
			reflectivity = (double)(parameters.reflectivity ?? 1.0);
			refractionRatio = (double)(parameters.refractionRatio ?? 0.98);

			fog = (bool)(parameters.fog ?? true);

			shading = (int)(parameters.shading ?? THREE.SmoothShading);

			wireframe = (bool)(parameters.wireframe ?? false);
			wireframeLinewidth = (double)(parameters.wireframeLinewidth ?? 1.0);
			wireframeLinecap = (parameters.wireframeLinecap ?? "round").ToString();
			wireframeLinejoin = (parameters.wireframeLinejoin ?? "round").ToString();

			vertexColors = (int)(parameters.vertexColors ?? THREE.NoColors);

			skinning = (bool)(parameters.skinning ?? false);
			morphTargets = (bool)(parameters.morphTargets ?? false);
			morphNormals = (bool)(parameters.morphNormals ?? false);

			setValues(parameters);
		}

		public MeshLambertMaterial clone()
		{
			var material = (MeshLambertMaterial)clone(new MeshLambertMaterial());

			material.color.copy(color);
			material.ambient.copy(ambient);
			material.emissive.copy(emissive);

			material.wrapAround = wrapAround;
			material.wrapRGB.copy(wrapRGB);

			material.map = map;

			material.lightMap = lightMap;

			material.specularMap = specularMap;

			material.envMap = envMap;
			material.combine = combine;
			material.reflectivity = reflectivity;
			material.refractionRatio = refractionRatio;

			material.fog = fog;

			material.shading = shading;

			material.wireframe = wireframe;
			material.wireframeLinewidth = wireframeLinewidth;
			material.wireframeLinecap = wireframeLinecap;
			material.wireframeLinejoin = wireframeLinejoin;

			material.vertexColors = vertexColors;

			material.skinning = skinning;
			material.morphTargets = morphTargets;
			material.morphNormals = morphNormals;

			return material;
		}
	}
}
