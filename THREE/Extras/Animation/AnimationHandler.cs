using WebGL;

namespace THREE
{
	public static class AnimationHandler
	{
		public const int LINEAR = 0;
		public const int CATMULLROM = 1;
		public const int CATMULLROM_FORWARD = 2;

		private static readonly dynamic playing = new JSArray();
		private static readonly dynamic library = new JSObject();

		public static void update(double deltaTimeMs)
		{
			for (var i = 0; i < playing.length; i++)
			{
				playing[i].update(deltaTimeMs);
			}
		}

		public static void addToUpdate(dynamic animation)
		{
			if (playing.indexOf(animation) == -1)
			{
				playing.push(animation);
			}
		}

		public static void removeFromUpdate(dynamic animation)
		{
			var index = playing.indexOf(animation);

			if (index != -1)
			{
				playing.splice(index, 1);
			}
		}

		public static void add(dynamic data)
		{
			if (library[data.name] != null)
			{
				JSConsole.log("THREE.AnimationHandler.add: Warning! " + data.name + " already exists in library. Overwriting.");
			}

			library[data.name] = data;
			initData(data);
		}

		public static dynamic get(dynamic name)
		{
			if (name is string)
			{
				if (JSObject.eval(library[name]))
				{
					return library[name];
				}
				JSConsole.log("THREE.AnimationHandler.get: Couldn't find animation " + name);
				return null;
			}

			return null;
		}

		public static JSArray parse(dynamic root)
		{
			var hierarchy = new JSArray();

			if (root is SkinnedMesh)
			{
				for (var b = 0; b < root.bones.length; b++)
				{
					hierarchy.push(root.bones[b]);
				}
			}
			else
			{
				parseRecurseHierarchy(root, hierarchy);
			}

			return hierarchy;
		}

		private static void parseRecurseHierarchy(dynamic root, dynamic hierarchy)
		{
			hierarchy.push(root);

			for (var c = 0; c < root.children.length; c++)
			{
				parseRecurseHierarchy(root.children[c], hierarchy);
			}
		}

		private static void initData(dynamic data)
		{
			if (data.initialized == true)
			{
				return;
			}

			for (var h = 0; h < data.hierarchy.length; h++)
			{
				for (var k = 0; k < data.hierarchy[h].keys.length; k++)
				{
					if (data.hierarchy[h].keys[k].time < 0)
					{
						data.hierarchy[h].keys[k].time = 0;
					}

					if (data.hierarchy[h].keys[k].rot != null && !(data.hierarchy[h].keys[k].rot is Quaternion))
					{
						var quat = data.hierarchy[h].keys[k].rot;
						data.hierarchy[h].keys[k].rot = new Quaternion((double)quat[0], (double)quat[1], (double)quat[2], (double)quat[3]);
					}
				}

				if (data.hierarchy[h].keys.length > 0 && data.hierarchy[h].keys[0].morphTargets != null)
				{
					var usedMorphTargets = new JSObject();

					for (var k = 0; k < data.hierarchy[h].keys.length; k++)
					{
						for (var m = 0; m < data.hierarchy[h].keys[k].morphTargets.length; m++)
						{
							var morphTargetName = data.hierarchy[h].keys[k].morphTargets[m];
							usedMorphTargets[morphTargetName] = -1;
						}
					}

					data.hierarchy[h].usedMorphTargets = usedMorphTargets;

					for (var k = 0; k < data.hierarchy[h].keys.length; k++)
					{
						var influences = new JSObject();

						foreach (var morphTargetName in usedMorphTargets)
						{
							var m = 0;
							for (; m < data.hierarchy[h].keys[k].morphTargets.length; m++)
							{
								if (data.hierarchy[h].keys[k].morphTargets[m] == morphTargetName)
								{
									influences[morphTargetName] = data.hierarchy[h].keys[k].morphTargetsInfluences[m];
									break;
								}
							}

							if (m == data.hierarchy[h].keys[k].morphTargets.length)
							{
								influences[morphTargetName] = 0;
							}
						}

						data.hierarchy[h].keys[k].morphTargetsInfluences = influences;
					}
				}

				for (var k = 1; k < data.hierarchy[h].keys.length; k++)
				{
					if (data.hierarchy[h].keys[k].time == data.hierarchy[h].keys[k - 1].time)
					{
						data.hierarchy[h].keys.splice(k, 1);
						k--;
					}
				}

				for (var k = 0; k < data.hierarchy[h].keys.length; k++)
				{
					data.hierarchy[h].keys[k].index = k;
				}
			}

			var lengthInFrames = (int)(data.length * data.fps);

			data.JIT = new JSObject();
			data.JIT.hierarchy = new JSArray();

			for (var h = 0; h < data.hierarchy.length; h++)
			{
				data.JIT.hierarchy.push(new JSArray(lengthInFrames));
			}

			data.initialized = true;
		}
	}
}
