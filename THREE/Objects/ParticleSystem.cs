namespace THREE
{
	public class ParticleSystem : Object3D
	{
		public bool sortParticles;

		public ParticleSystem(dynamic geometry, dynamic material = null)
		{
			this.geometry = geometry;
			this.material = material ?? new ParticleBasicMaterial(create(new {color = Math.random() * 0xffffff}));

			sortParticles = false;

			if (this.geometry != null)
			{
				if (this.geometry.boundingSphere == null)
				{
					this.geometry.computeBoundingSphere();
				}
			}

			frustumCulled = false;
		}

		public ParticleSystem clone(ParticleSystem obj = null)
		{
			if (obj == null)
			{
				obj = new ParticleSystem(geometry, material as ParticleBasicMaterial);
			}
			obj.sortParticles = sortParticles;

			return base.clone(obj) as ParticleSystem;
		}
	}
}
