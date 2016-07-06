using System;
using System.Text.RegularExpressions;
using WebGL;

namespace THREE
{
	public class MorphAnimMesh : Mesh
	{
		public double duration;
		public bool mirroredLoop;
		public double time;
		public int lastKeyframe;
		public int currentKeyframe;
		public int direction;
		public bool directionBackwards;
		public int startKeyframe;
		public int endKeyframe;
		public int length;

		public MorphAnimMesh(Geometry geometry, Material material) : base(geometry, material)
		{
			duration = 1000;
			mirroredLoop = false;
			time = 0;

			lastKeyframe = 0;
			currentKeyframe = 0;

			direction = 1;
			directionBackwards = false;

			this.setFrameRange(0, this.geometry.morphTargets.length - 1);
		}

		public virtual void setFrameRange(int start, int end)
		{
			startKeyframe = start;
			endKeyframe = end;

			length = endKeyframe - startKeyframe + 1;
		}

		public virtual void setDirectionForward()
		{
			direction = 1;
			directionBackwards = false;
		}

		public virtual void setDirectionBackward()
		{
			direction = -1;
			directionBackwards = true;
		}

		public virtual void parseAnimations()
		{
			if (! eval(geometry.animations))
			{
				geometry.animations = new Object();
			}

			var firstAnimation = false;
			var animations = geometry.animations;

			var pattern = new Regex(@"/([a-z]+)(\d+)/");

			var morphTargets = geometry.morphTargets;
			for (int i = 0, il = morphTargets.length; i < il; i ++)
			{
				var morph = morphTargets[i];
				var parts = morph.name.match(pattern);

				if (parts && parts.length > 1)
				{
					var label = parts[1];

					if (! animations[label])
					{
						animations[label] = create(new {start = double.PositiveInfinity, end = double.NegativeInfinity});
					}

					var animation = animations[label];

					if (i < animation.start)
					{
						animation.start = i;
					}
					if (i > animation.end)
					{
						animation.end = i;
					}

					if (! firstAnimation)
					{
						firstAnimation = label;
					}
				}
			}

			geometry.firstAnimation = firstAnimation;
		}

		public virtual void setAnimationLabel(string label, double start, double end)
		{
			if (! eval(geometry.animations))
			{
				geometry.animations = new Object();
			}

			geometry.animations[label] = create(new {start, end});
		}

		public virtual void playAnimation(string label, double fps)
		{
			var animation = geometry.animations[label];

			if (eval(animation))
			{
				this.setFrameRange(animation.start, animation.end);
				duration = 1000.0 * ((animation.end - animation.start) / fps);
				time = 0.0;
			}
			else
			{
				JSConsole.warn("animation[" + label + "] undefined");
			}
		}

		public virtual void updateAnimation(dynamic delta)
		{
			var frameTime = duration / length;

			time += direction * delta;

			if (mirroredLoop)
			{
				if (time > duration || time < 0)
				{
					direction *= -1;

					if (time > duration)
					{
						time = duration;
						directionBackwards = true;
					}

					if (time < 0)
					{
						time = 0;
						directionBackwards = false;
					}
				}
			}
			else
			{
				time = time % duration;

				if (time < 0)
				{
					time += duration;
				}
			}

			var keyframe = (int)(startKeyframe + Math.clamp(System.Math.Floor(time / frameTime), 0, length - 1));

			if (keyframe != currentKeyframe)
			{
				morphTargetInfluences[lastKeyframe] = 0;
				morphTargetInfluences[currentKeyframe] = 1;

				morphTargetInfluences[keyframe] = 0;

				lastKeyframe = currentKeyframe;
				currentKeyframe = keyframe;
			}

			var mix = (time % frameTime) / frameTime;

			if (directionBackwards)
			{
				mix = 1 - mix;
			}

			morphTargetInfluences[currentKeyframe] = mix;
			morphTargetInfluences[lastKeyframe] = 1 - mix;
		}

		public virtual MorphAnimMesh clone(MorphAnimMesh obj)
		{
			if (obj == null)
			{
				obj = new MorphAnimMesh(geometry, material);
			}

			obj.duration = duration;
			obj.mirroredLoop = mirroredLoop;
			obj.time = time;

			obj.lastKeyframe = lastKeyframe;
			obj.currentKeyframe = currentKeyframe;

			obj.direction = direction;
			obj.directionBackwards = directionBackwards;

			return base.clone(obj) as MorphAnimMesh;
		}
	}
}
