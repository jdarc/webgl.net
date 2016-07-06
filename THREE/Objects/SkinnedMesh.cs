using WebGL;

namespace THREE
{
	public class SkinnedMesh : Mesh
	{
		public static Matrix4 offsetMatrix = new Matrix4();

		public bool useVertexTexture;
		public Matrix4 identityMatrix;
		public JSArray bones;
		public dynamic boneMatrices;
		public int boneTextureWidth;
		public int boneTextureHeight;
		public DataTexture boneTexture;
		public dynamic boneInverses;

		public SkinnedMesh(Geometry geometry, Material material, bool useVertexTexture = true) : base(geometry, material)
		{
			this.useVertexTexture = useVertexTexture;

			identityMatrix = new Matrix4();

			bones = new JSArray();
			boneMatrices = new JSArray();

			if (this.geometry != null && this.geometry.bones != null)
			{
				int b;
				for (b = 0; b < this.geometry.bones.length; b++)
				{
					var gbone = this.geometry.bones[b];

					var p = gbone.pos;
					var q = gbone.rotq;
					var s = gbone.scl;

					var bone = addBone();

					bone.name = gbone.name;
					bone.position.set((double)p[0], (double)p[1], (double)p[2]);
					bone.quaternion.set((double)q[0], (double)q[1], (double)q[2], (double)q[3]);
					bone.useQuaternion = true;

					if (s != null)
					{
						bone.scale.set((double)s[0], (double)s[1], (double)s[2]);
					}
					else
					{
						bone.scale.set(1, 1, 1);
					}
				}

				for (b = 0; b < bones.length; b++)
				{
					var gbone = this.geometry.bones[b];
					var bone = bones[b];

					if (gbone.parent == -1)
					{
						this.add(bone);
					}
					else
					{
						bones[gbone.parent].add(bone);
					}
				}

				var nBones = bones.length;

				if (this.useVertexTexture)
				{
					int size;

					if (nBones > 256)
					{
						size = 64;
					}
					else if (nBones > 64)
					{
						size = 32;
					}
					else if (nBones > 16)
					{
						size = 16;
					}
					else
					{
						size = 8;
					}

					boneTextureWidth = size;
					boneTextureHeight = size;

					boneMatrices = new Float32Array(boneTextureWidth * boneTextureHeight * 4);
					boneTexture = new DataTexture(boneMatrices, boneTextureWidth, boneTextureHeight, THREE.RGBAFormat, THREE.FloatType)
					{
						minFilter = THREE.NearestFilter,
						magFilter = THREE.NearestFilter,
						generateMipmaps = false,
						flipY = false
					};
				}
				else
				{
					boneMatrices = new Float32Array(16 * nBones);
				}

				pose();
			}
		}

		public dynamic addBone(dynamic bone = null)
		{
			if (bone == null)
			{
				bone = new Bone(this);
			}

			bones.push(bone);

			return bone;
		}

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
				}
				else
				{
					matrixWorld.copy(matrix);
				}

				matrixWorldNeedsUpdate = false;
			}

			for (var i = 0; i < children.length; i++)
			{
				var child = children[i];

				if (child is Bone)
				{
					child.update(identityMatrix, false);
				}
				else
				{
					child.updateMatrixWorld(true);
				}
			}

			if (boneInverses == null)
			{
				boneInverses = new JSArray();

				for (var b = 0; b < bones.length; b++)
				{
					var inverse = new Matrix4();

					inverse.getInverse(bones[b].skinMatrix);

					boneInverses.push(inverse);
				}
			}

			for (var b = 0; b < bones.length; b++)
			{
				offsetMatrix.multiplyMatrices(bones[b].skinMatrix, boneInverses[b]);

				offsetMatrix.flattenToArrayOffset(boneMatrices, b * 16);
			}

			if (useVertexTexture)
			{
				boneTexture.needsUpdate = true;
			}
		}

		public void pose()
		{
			updateMatrixWorld(true);

			for (var i = 0; i < geometry.skinIndices.length; i++)
			{
				var sw = geometry.skinWeights[i];

				var sca = 1.0 / sw.lengthManhattan();
				if (sca != double.PositiveInfinity)
				{
					sw.multiplyScalar(sca);
				}
				else
				{
					sw.set(1);
				}
			}
		}

		public SkinnedMesh clone(SkinnedMesh obj = null)
		{
			if (obj == null)
			{
				obj = new SkinnedMesh(geometry, material, useVertexTexture);
			}

			return base.clone(obj) as SkinnedMesh;
		}
	}
}
