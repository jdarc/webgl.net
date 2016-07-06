using WebGL;

namespace THREE
{
	public class Mesh : Object3D
	{
		public int morphTargetBase;
		public JSArray morphTargetForcedOrder;
		public JSArray morphTargetInfluences;
		public JSObject morphTargetDictionary;

		public Mesh(dynamic geometry, Material material = null)
		{
			this.geometry = geometry;
			this.material = material ?? new MeshBasicMaterial(create(new {color = Math.random() * 0xffffff, wireframe = true}));

			if (this.geometry != null)
			{
				if (this.geometry.boundingSphere == null)
				{
					this.geometry.computeBoundingSphere();
				}

				updateMorphTargets();
			}
		}

		public void updateMorphTargets()
		{
			if (geometry.morphTargets.length > 0)
			{
				morphTargetBase = -1;
				morphTargetForcedOrder = new JSArray();
				morphTargetInfluences = new JSArray();
				morphTargetDictionary = new JSObject();

				for (int m = 0, ml = geometry.morphTargets.length; m < ml; m++)
				{
					morphTargetInfluences.push(0);
					morphTargetDictionary[geometry.morphTargets[m].name] = m;
				}
			}
		}

		public virtual int getMorphTargetIndexByName(string nameArg)
		{
			if (morphTargetDictionary[nameArg] != null)
			{
				return morphTargetDictionary[nameArg];
			}

			JSConsole.log("THREE.Mesh.getMorphTargetIndexByName: morph target " + nameArg + " does not exist. Returning 0.");

			return 0;
		}

		public virtual Mesh clone(Mesh obj = null)
		{
			if (obj == null)
			{
				obj = new Mesh(geometry, material);
			}

			return base.clone(obj) as Mesh;
		}
	}
}
