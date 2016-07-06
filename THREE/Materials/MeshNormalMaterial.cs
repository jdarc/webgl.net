using WebGL;

namespace THREE
{
	public class MeshNormalMaterial : Material
	{
		public dynamic shading;
		public bool wireframe;
		public double wireframeLinewidth;

		public MeshNormalMaterial(dynamic parameters = null)
		{
			parameters = parameters ?? new JSObject();

			shading = parameters.shading ?? THREE.FlatShading;

			wireframe = parameters.wireframe ?? false;
			wireframeLinewidth = parameters.wireframeLinewidth ?? 1.0;

			this.setValues(parameters);
		}

		public MeshNormalMaterial clone()
		{
			var material = (MeshNormalMaterial)clone(new MeshNormalMaterial());
			material.shading = shading;

			material.wireframe = wireframe;
			material.wireframeLinewidth = wireframeLinewidth;

			return material;
		}
	}
}
