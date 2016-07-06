namespace THREE
{
	public class SceneUtils
	{
		public static dynamic createMultiMaterialObject(dynamic geometry, dynamic materials)
		{
			var group = new Object3D();

			for (var i = 0; i < materials.length; i++)
			{
				group.add(new Mesh(geometry, materials[i]));
			}

			return group;
		}
	}
}
