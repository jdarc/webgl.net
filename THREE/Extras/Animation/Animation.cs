using WebGL;

namespace THREE
{
	public class Animation
	{
		public double offset;
		public Mesh root;
		public dynamic data;
		public JSArray hierarchy;
		public double currentTime;
		public double timeScale;
		public bool isPlaying;
		public bool isPaused;
		public bool loop;
		public int interpolationType;
		public JSArray points;
		public Vector3 target;

		public Animation(Mesh root, string name, int interpolationType = AnimationHandler.LINEAR)
		{
			this.root = root;
			data = AnimationHandler.get(name);
			hierarchy = AnimationHandler.parse(root);

			currentTime = 0;
			timeScale = 1;

			isPlaying = false;
			isPaused = true;
			loop = true;

			this.interpolationType = interpolationType;

			points = new JSArray();
			target = new Vector3();
		}

		public virtual void play(bool loopArg = true, double startTimeMs = 0.0)
		{
			if (isPlaying == false)
			{
				isPlaying = true;
				loop = loopArg;
				currentTime = startTimeMs;

				var hl = hierarchy.length;

				for (var h = 0; h < hl; h ++)
				{
					var obj = hierarchy[h];

					if (interpolationType != AnimationHandler.CATMULLROM_FORWARD)
					{
						obj.useQuaternion = true;
					}

					obj.matrixAutoUpdate = true;

					if (obj.animationCache == null)
					{
						obj.animationCache = new JSObject();
						obj.animationCache.prevKey = JSObject.create(new {pos = 0, rot = 0, scl = 0});
						obj.animationCache.nextKey = JSObject.create(new {pos = 0, rot = 0, scl = 0});
						obj.animationCache.originalMatrix = obj is Bone ? obj.skinMatrix : obj.matrix;
					}

					var prevKey = obj.animationCache.prevKey;
					var nextKey = obj.animationCache.nextKey;

					prevKey.pos = data.hierarchy[h].keys[0];
					prevKey.rot = data.hierarchy[h].keys[0];
					prevKey.scl = data.hierarchy[h].keys[0];

					nextKey.pos = getNextKeyWith("pos", h, 1);
					nextKey.rot = getNextKeyWith("rot", h, 1);
					nextKey.scl = getNextKeyWith("scl", h, 1);
				}

				update(0);
			}

			isPaused = false;

			AnimationHandler.addToUpdate(this);
		}

		public virtual void pause()
		{
			if (isPaused)
			{
				AnimationHandler.addToUpdate(this);
			}
			else
			{
				AnimationHandler.removeFromUpdate(this);
			}

			isPaused = !isPaused;
		}

		public virtual void stop()
		{
			isPlaying = false;
			isPaused = false;
			AnimationHandler.removeFromUpdate(this);
		}

		public virtual void update(dynamic deltaTimeMs)
		{
			// early out

			if (isPlaying == false)
			{
				return;
			}

			// vars

			var types = new JSArray("pos", "rot", "scl");

			currentTime += deltaTimeMs * timeScale;

			var unloopedCurrentTime = currentTime;
			var time = currentTime = currentTime % (double)data.length;

			for (int h = 0, hl = hierarchy.length; h < hl; h ++)
			{
				var obj = hierarchy[h];
				var animationCache = obj.animationCache;

				// loop through pos/rot/scl

				for (var t = 0; t < 3; t ++)
				{
					// get keys

					var type = types[t];
					var prevKey = animationCache.prevKey[type];
					var nextKey = animationCache.nextKey[type];

					// switch keys?

					if ((double)nextKey.time <= unloopedCurrentTime)
					{
						// did we loop?

						if (time < unloopedCurrentTime)
						{
							if (loop)
							{
								prevKey = data.hierarchy[h].keys[0];
								nextKey = this.getNextKeyWith(type, h, 1);

								while ((double)nextKey.time < time)
								{
									prevKey = nextKey;
									nextKey = this.getNextKeyWith(type, h, nextKey.index + 1);
								}
							}
							else
							{
								stop();
								return;
							}
						}
						else
						{
							do
							{
								prevKey = nextKey;
								nextKey = this.getNextKeyWith(type, h, nextKey.index + 1);
							} while ((double)nextKey.time < time);
						}

						animationCache.prevKey[type] = prevKey;
						animationCache.nextKey[type] = nextKey;
					}

					obj.matrixAutoUpdate = true;
					obj.matrixWorldNeedsUpdate = true;

					var scale = (time - (double)prevKey.time) / ((double)nextKey.time - (double)prevKey.time);
					var prevXYZ = prevKey[type];
					var nextXYZ = nextKey[type];

					// check scale error

					if (scale < 0 || scale > 1)
					{
						JSConsole.log("THREE.Animation.update: Warning! Scale out of bounds:" + scale + " on bone " + h);
						scale = scale < 0 ? 0 : 1;
					}

					// interpolate

					if (type == "pos")
					{
						var vector = obj.position;

						if (interpolationType == AnimationHandler.LINEAR)
						{
							vector.x = (double)prevXYZ[0] + (double)(nextXYZ[0] - prevXYZ[0]) * scale;
							vector.y = (double)prevXYZ[1] + (double)(nextXYZ[1] - prevXYZ[1]) * scale;
							vector.z = (double)prevXYZ[2] + (double)(nextXYZ[2] - prevXYZ[2]) * scale;
						}
						else if (interpolationType == AnimationHandler.CATMULLROM || interpolationType == AnimationHandler.CATMULLROM_FORWARD)
						{
							points[0] = this.getPrevKeyWith("pos", h, prevKey.index - 1)["pos"];
							points[1] = prevXYZ;
							points[2] = nextXYZ;
							points[3] = this.getNextKeyWith("pos", h, nextKey.index + 1)["pos"];

							scale = scale * 0.33 + 0.33;

							var currentPoint = interpolateCatmullRom(points, scale);

							vector.x = currentPoint[0];
							vector.y = currentPoint[1];
							vector.z = currentPoint[2];

							if (interpolationType == AnimationHandler.CATMULLROM_FORWARD)
							{
								var forwardPoint = interpolateCatmullRom(points, scale * 1.01);

								target.set(forwardPoint[0], forwardPoint[1], forwardPoint[2]);
								target.sub(vector);
								target.y = 0;
								target.normalize();

								var angle = System.Math.Atan2(target.x, target.z);
								obj.rotation.set(0, angle, 0);
							}
						}
					}
					else if (type == "rot")
					{
						Quaternion.slerp(prevXYZ, nextXYZ, obj.quaternion, scale);
					}
					else if (type == "scl")
					{
						Vector3 vector = obj.scale;

						vector.x = prevXYZ[0] + (nextXYZ[0] - prevXYZ[0]) * scale;
						vector.y = prevXYZ[1] + (nextXYZ[1] - prevXYZ[1]) * scale;
						vector.z = prevXYZ[2] + (nextXYZ[2] - prevXYZ[2]) * scale;
					}
				}
			}
		}

		public virtual dynamic interpolateCatmullRom(dynamic pointsArg, dynamic scale)
		{
			dynamic c = new JSArray();
			dynamic v3 = new JSArray();

			var point = (pointsArg.length - 1) * scale;
			var intPoint = Math.floor(point);
			var weight = point - intPoint;

			c[0] = intPoint == 0 ? intPoint : intPoint - 1;
			c[1] = intPoint;
			c[2] = intPoint > pointsArg.length - 2 ? intPoint : intPoint + 1;
			c[3] = intPoint > pointsArg.length - 3 ? intPoint : intPoint + 2;

			var pa = pointsArg[c[0]];
			var pb = pointsArg[c[1]];
			var pc = pointsArg[c[2]];
			var pd = pointsArg[c[3]];

			var w2 = weight * weight;
			var w3 = weight * w2;

			v3[0] = this.interpolate(pa[0], pb[0], pc[0], pd[0], weight, w2, w3);
			v3[1] = this.interpolate(pa[1], pb[1], pc[1], pd[1], weight, w2, w3);
			v3[2] = this.interpolate(pa[2], pb[2], pc[2], pd[2], weight, w2, w3);

			return v3;
		}

		public virtual dynamic interpolate(dynamic p0, dynamic p1, dynamic p2, dynamic p3, dynamic t, dynamic t2, dynamic t3)
		{
			var v0 = (p2 - p0) * 0.5;
			var v1 = (p3 - p1) * 0.5;

			return (2 * (p1 - p2) + v0 + v1) * t3 + (- 3 * (p1 - p2) - 2 * v0 - v1) * t2 + v0 * t + p1;
		}

		public virtual dynamic getNextKeyWith(dynamic type, dynamic h, dynamic key)
		{
			var keys = data.hierarchy[h].keys;

			if (interpolationType == AnimationHandler.CATMULLROM || interpolationType == AnimationHandler.CATMULLROM_FORWARD)
			{
				key = key < keys.length - 1 ? key : keys.length - 1;
			}
			else
			{
				key = key % keys.length;
			}

			for (; key < keys.length; key++)
			{
				if (keys[key][type] != null)
				{
					return keys[key];
				}
			}

			return data.hierarchy[h].keys[0];
		}

		public virtual dynamic getPrevKeyWith(dynamic type, dynamic h, dynamic key)
		{
			var keys = data.hierarchy[h].keys;

			if (interpolationType == AnimationHandler.CATMULLROM || interpolationType == AnimationHandler.CATMULLROM_FORWARD)
			{
				key = key > 0 ? key : 0;
			}
			else
			{
				key = key >= 0 ? key : key + keys.length;
			}

			for (; key >= 0; key --)
			{
				if (keys[key][type] != null)
				{
					return keys[key];
				}
			}

			return data.hierarchy[h].keys[keys.length - 1];
		}
	}
}
