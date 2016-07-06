using WebGL;

namespace THREE
{
	public class ShaderMaterial : Material
	{
		public JSObject defines;
		public bool wireframe;
		public int wireframeLinewidth;
		public bool lights;
		public bool skinning;
		public bool morphTargets;
		public bool morphNormals;

		public dynamic fragmentShader;
		public dynamic vertexShader;
		public dynamic attributes;
		public dynamic uniforms;
		public dynamic shading;
		public dynamic fog;
		public dynamic vertexColors;

		public ShaderMaterial(dynamic parameters = null)
		{
			parameters = parameters ?? new JSObject();

			fragmentShader = parameters.fragmentShader ?? "void main() {}";
			vertexShader = parameters.vertexShader ?? "void main() {}";
			uniforms = parameters.uniforms ?? new JSObject();
			defines = parameters.defines ?? new JSObject();
			attributes = parameters.attributes;

			shading = parameters.shading ?? THREE.SmoothShading;

			wireframe = parameters.wireframe ?? false;
			wireframeLinewidth = parameters.wireframeLinewidth ?? 1;

			fog = parameters.fog ?? false;

			lights = parameters.lights ?? false;

			vertexColors = parameters.vertexColors ?? THREE.NoColors;

			skinning = parameters.skinning ?? false;

			morphTargets = parameters.morphTargets ?? false;
			morphNormals = parameters.morphNormals ?? false;

			this.setValues(parameters);
		}

		public ShaderMaterial clone()
		{
			var material = (ShaderMaterial)base.clone(new ShaderMaterial());

			material.fragmentShader = fragmentShader;
			material.vertexShader = vertexShader;

			material.uniforms = WebGLShaders.UniformsUtils.clone(uniforms);

			material.attributes = attributes;
			material.defines = defines;

			material.shading = shading;

			material.wireframe = wireframe;
			material.wireframeLinewidth = wireframeLinewidth;

			material.fog = fog;

			material.lights = lights;

			material.vertexColors = vertexColors;

			material.skinning = skinning;

			material.morphTargets = morphTargets;
			material.morphNormals = morphNormals;

			return material;
		}
	}
}
