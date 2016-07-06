using WebGL;

namespace THREE
{
	public class MeshDepthMaterial : Material
	{
		public dynamic wireframe;
		public dynamic wireframeLinewidth;

		public MeshDepthMaterial(dynamic parameters = null)
		{
			parameters = parameters ?? new JSObject();

			wireframe = parameters.wireframe ?? false;
			wireframeLinewidth = parameters.wireframeLinewidth ?? 1;

			this.setValues(parameters);
		}

		public MeshDepthMaterial clone()
		{
			var material = (MeshDepthMaterial)clone(new MeshDepthMaterial());

			material.wireframe = wireframe;
			material.wireframeLinewidth = wireframeLinewidth;

			return material;
		}
	}
}
