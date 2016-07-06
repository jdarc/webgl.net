namespace THREE
{
	public class Sprite : Object3D
	{
		public Vector3 rotation3d;

		public Sprite(SpriteMaterial material = null)
		{
			this.material = material ?? new SpriteMaterial();

			rotation3d = rotation;
			rotation = 0.0;
		}

		public override void updateMatrix()
		{
			matrix.setPosition(position);

			rotation3d.set(0, 0, rotation);
			matrix.setRotationFromEuler(rotation3d);

			if (scale.x != 1.0 || scale.y != 1.0)
			{
				matrix.scale(scale);
			}

			matrixWorldNeedsUpdate = true;
		}

		public Sprite clone(Sprite obj = null)
		{
			if (obj == null)
			{
				obj = new Sprite(material as SpriteMaterial);
			}
			obj.rotation3d.copy(rotation3d);
			obj.rotation = rotation;
			return base.clone(obj) as Sprite;
		}
	}
}
