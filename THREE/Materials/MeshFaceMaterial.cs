using WebGL;

namespace THREE
{
	public class MeshFaceMaterial : Material
	{
		public JSArray materials;

		public MeshFaceMaterial(dynamic materials = null)
		{
			this.materials = materials != null && materials is JSArray ? materials : new JSArray();
		}

		public MeshFaceMaterial clone()
		{
			return new MeshFaceMaterial(materials.slice(0));
		}
	}
}
