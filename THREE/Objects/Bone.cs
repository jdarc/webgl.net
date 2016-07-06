namespace THREE
{
	public class Bone : Object3D
	{
		public Mesh skin;
		public Matrix4 skinMatrix;

		public Bone(Mesh belongsToSkin)
		{
			skin = belongsToSkin;
			skinMatrix = new Matrix4();
		}

		public void update(Matrix4 parentSkinMatrix, bool forceUpdate = false)
		{
			if (matrixAutoUpdate)
			{
				updateMatrix();
				forceUpdate = true;
			}

			if (forceUpdate || matrixWorldNeedsUpdate)
			{
				if (parentSkinMatrix != null)
				{
					skinMatrix.multiplyMatrices(parentSkinMatrix, matrix);
				}
				else
				{
					skinMatrix.copy(matrix);
				}

				matrixWorldNeedsUpdate = false;
				forceUpdate = true;
			}

			for (var i = 0; i < children.length; i++)
			{
				children[i].update(skinMatrix, forceUpdate);
			}
		}
	}
}
