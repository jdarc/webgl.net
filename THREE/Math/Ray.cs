namespace THREE
{
	public class Ray
	{
		private static readonly Vector3 __v1 = new Vector3();
		private static readonly Vector3 __v2 = new Vector3();
		public Vector3 origin;
		public Vector3 direction;

		public Ray(Vector3 origin = null, Vector3 direction = null)
		{
			this.origin = origin ?? new Vector3();
			this.direction = direction ?? new Vector3();
		}

		public Ray set(Vector3 origin, Vector3 direction)
		{
			this.origin.copy(origin);
			this.direction.copy(direction);

			return this;
		}

		public Ray copy(Ray ray)
		{
			origin.copy(ray.origin);
			direction.copy(ray.direction);

			return this;
		}

		public Vector3 at(double t, Vector3 optionalTarget = null)
		{
			var result = optionalTarget ?? new Vector3();

			return result.copy(direction).multiplyScalar(t).add(origin);
		}

		public Ray recast(double t)
		{
			origin.copy(at(t, __v1));

			return this;
		}

		public Vector3 closestPointToPoint(Vector3 point, Vector3 optionalTarget = null)
		{
			var result = optionalTarget ?? new Vector3();
			result.subVectors(point, origin);
			var directionDistance = result.dot(direction);

			return result.copy(direction).multiplyScalar(directionDistance).add(origin);
		}

		public double distanceToPoint(Vector3 point)
		{
			var directionDistance = __v1.subVectors(point, origin).dot(direction);
			__v1.copy(direction).multiplyScalar(directionDistance).add(origin);

			return __v1.distanceTo(point);
		}

		public bool isIntersectionSphere(Sphere sphere)
		{
			return (distanceToPoint(sphere.center) <= sphere.radius);
		}

		public bool isIntersectionPlane(Plane plane)
		{
			var denominator = plane.normal.dot(direction);
			if (denominator != 0)
			{
				return true;
			}

			if (plane.distanceToPoint(origin) == 0)
			{
				return true;
			}

			return false;
		}

		public dynamic distanceToPlane(Plane plane)
		{
			var denominator = plane.normal.dot(direction);
			if (denominator == 0)
			{
				if (plane.distanceToPoint(origin) == 0)
				{
					return 0;
				}

				return null;
			}

			var t = - (origin.dot(plane.normal) + plane.constant) / denominator;

			return t;
		}

		public dynamic intersectPlane(Plane plane, Vector3 optionalTarget = null)
		{
			var t = distanceToPlane(plane);

			if (t == null)
			{
				return null;
			}

			return this.at(t, optionalTarget);
		}

		public Ray transform(Matrix4 matrix4)
		{
			direction.add(origin).applyMatrix4(matrix4);
			origin.applyMatrix4(matrix4);
			direction.sub(origin);

			return this;
		}

		public bool equals(Ray ray)
		{
			return ray.origin.equals(origin) && ray.direction.equals(direction);
		}

		public Ray clone()
		{
			return new Ray().copy(this);
		}
	}
}
