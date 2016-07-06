namespace THREE
{
	public class Ribbon : Object3D
	{
		public Ribbon(Geometry geometry, Material material)
		{
			this.geometry = geometry;
			this.material = material;
		}

		public Ribbon clone(Ribbon obj = null)
		{
			if (obj == null)
			{
				obj = new Ribbon(geometry, material);
			}
			return base.clone(obj) as Ribbon;
		}
	}
}
