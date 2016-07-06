namespace THREE
{
	public class Line : Object3D
	{
		public int type;

		public Line(dynamic geometry, Material material = null, int type = 0)
		{
			this.geometry = geometry;
			this.material = material ?? new LineBasicMaterial(create(new {color = Math.random() * 0xffffff}));
			this.type = type;

			if (this.geometry != null)
			{
				if (this.geometry.boundingSphere == null)
				{
					this.geometry.computeBoundingSphere();
				}
			}
		}

		public Line clone(Line obj = null)
		{
			if (obj == null)
			{
				obj = new Line(geometry, material, type);
			}

			return base.clone(obj) as Line;
		}
	}
}
