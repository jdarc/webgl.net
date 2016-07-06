namespace THREE
{
	public class Gyroscope : Object3D
	{
		public static readonly Vector3 translationWorld = new Vector3();
		public static readonly Vector3 translationObject = new Vector3();
		public static readonly Quaternion rotationWorld = new Quaternion();
		public static readonly Quaternion rotationObject = new Quaternion();
		public static readonly Vector3 scaleWorld = new Vector3();
		public static readonly Vector3 scaleObject = new Vector3();

		public override void updateMatrixWorld(bool force = false)
		{
			if (matrixAutoUpdate)
			{
				updateMatrix();
			}

			if (matrixWorldNeedsUpdate || force)
			{
				if (parent != null)
				{
					matrixWorld.multiplyMatrices(parent.matrixWorld, matrix);

					matrixWorld.decompose(translationWorld, rotationWorld, scaleWorld);
					matrix.decompose(translationObject, rotationObject, scaleObject);

					matrixWorld.compose(translationWorld, rotationObject, scaleWorld);
				}
				else
				{
					matrixWorld.copy(matrix);
				}

				matrixWorldNeedsUpdate = false;

				force = true;
			}

			for (var i = 0; i < children.length; i++)
			{
				children[i].updateMatrixWorld(force);
			}
		}
	}
}
