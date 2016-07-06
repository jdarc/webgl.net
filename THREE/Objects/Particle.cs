namespace THREE
{
	public class Particle : Object3D
	{
		public Particle(Material material)
		{
			this.material = material;
		}

		public Particle clone(Particle obj)
		{
			if (obj == null)
			{
				obj = new Particle(material);
			}

			return base.clone(obj) as Particle;
		}
	}
}
