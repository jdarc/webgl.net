using WebGL;

namespace THREE
{
	public class MeshPhongMaterial : Material
	{
		public Color ambient;
		public Color emissive;
		public Color specular;
		public dynamic shininess;
		public dynamic metal;
		public dynamic perPixel;
		public dynamic wrapAround;
		public Vector3 wrapRGB;
		public dynamic bumpScale;
		public Vector2 normalScale;
		public dynamic combine;
		public dynamic reflectivity;
		public dynamic refractionRatio;
		public dynamic wireframe;
		public dynamic wireframeLinewidth;
		public dynamic wireframeLinecap;
		public dynamic wireframeLinejoin;
		public dynamic skinning;
		public dynamic morphTargets;
		public dynamic morphNormals;

		public Color color;
		public dynamic map;
		public dynamic lightMap;
		public dynamic bumpMap;
		public dynamic normalMap;
		public dynamic specularMap;
		public dynamic envMap;
		public dynamic fog;
		public dynamic shading;
		public dynamic vertexColors;

		public MeshPhongMaterial(dynamic parameters = null)
		{
			parameters = parameters ?? new JSObject();

			color = new Color(parameters.color ?? 0xffffff);
			ambient = new Color(parameters.ambient ?? 0xffffff);
			emissive = new Color(parameters.emissive ?? 0x000000);
			specular = new Color(parameters.specular ?? 0x111111);
			shininess = parameters.shininess ?? 30.0;

			metal = parameters.metal ?? false;
			perPixel = parameters.perPixel ?? true;

			wrapAround = parameters.wrapAround ?? false;
			wrapRGB = new Vector3().copy(parameters.wrapRGB ?? new Vector3(1, 1, 1));

			map = parameters.map;

			lightMap = parameters.lightMap;

			bumpMap = parameters.bumpMap;
			bumpScale = parameters.bumpScale ?? 1.0;

			normalMap = parameters.normalMap;
			normalScale = new Vector2().copy(parameters.normalScale ?? new Vector2(1, 1));

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
			morphNormals = parameters.morphNormals ?? false;

			setValues(parameters);
		}

		public MeshPhongMaterial clone()
		{
			var material = (MeshPhongMaterial)clone(new MeshPhongMaterial());

			material.color.copy(color);
			material.ambient.copy(ambient);
			material.emissive.copy(emissive);
			material.specular.copy(specular);
			material.shininess = shininess;

			material.metal = metal;
			material.perPixel = perPixel;

			material.wrapAround = wrapAround;
			material.wrapRGB.copy(wrapRGB);

			material.map = map;

			material.lightMap = lightMap;

			material.bumpMap = bumpMap;
			material.bumpScale = bumpScale;

			material.normalMap = normalMap;
			material.normalScale.copy(normalScale);

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
