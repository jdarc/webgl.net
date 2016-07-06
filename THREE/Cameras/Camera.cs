namespace THREE
{
	public class Camera : Object3D
	{
		public readonly Matrix4 matrixWorldInverse;
		public readonly Matrix4 projectionMatrix;
		public readonly Matrix4 projectionMatrixInverse;

		public Camera()
		{
			matrixWorldInverse = new Matrix4();
			projectionMatrix = new Matrix4();
			projectionMatrixInverse = new Matrix4();
		}

		public override void lookAt(Vector3 vector)
		{
			matrix.lookAt(position, vector, up);

			if (rotationAutoUpdate)
			{
				if (useQuaternion == false)
				{
					rotation.setEulerFromRotationMatrix(matrix, eulerOrder);
				}
				else
				{
					quaternion.copy(matrix.decompose()[1]);
				}
			}
		}
	}
}
