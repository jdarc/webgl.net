using WebGL;

namespace THREE
{
	public class MeshBasicMaterial : Material
	{
		public int combine;
		public double reflectivity;
		public double refractionRatio;
		public bool wireframe;
		public double wireframeLinewidth;
		public string wireframeLinecap;
		public string wireframeLinejoin;
		public bool skinning;
		public bool morphTargets;

		public Color color;
		public dynamic map;
		public dynamic lightMap;
		public dynamic specularMap;
		public dynamic envMap;
		public dynamic fog;
		public dynamic shading;
		public dynamic vertexColors;

		public MeshBasicMaterial(dynamic parameters = null)
		{
			parameters = parameters ?? new JSObject();

			color = new Color(parameters.color ?? 0xffffff);

			map = parameters.map;

			lightMap = parameters.lightMap;

			specularMap = parameters.specularMap;

			envMap = parameters.envMap;
			combine = parameters.combine ?? THREE.MultiplyOperation;
			reflectivity = parameters.reflectivity ?? 1.0;
			refractionRatio = parameters.refractionRatio ?? 0.98;

			fog = parameters.fog ?? true;

			shading = parameters.shading ?? THREE.SmoothShading;

			wireframe = parameters.wireframe ?? false;
			wireframeLinewidth = parameters.wireframeLinewidth ?? 1.0;
			wireframeLinecap = parameters.wireframeLinecap ?? "round";
			wireframeLinejoin = parameters.wireframeLinejoin ?? "round";

			vertexColors = parameters.vertexColors ?? THREE.NoColors;

			skinning = parameters.skinning ?? false;
			morphTargets = parameters.morphTargets ?? false;

			setValues(parameters);
		}

		public MeshBasicMaterial clone()
		{
			var material = (MeshBasicMaterial)clone(new MeshBasicMaterial());

			material.color.copy(color);

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

			return material;
		}
	}
}
