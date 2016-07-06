using WebGL;

namespace THREE
{
	public class Sphere
	{
		public Vector3 center;
		public double radius;

		public Sphere(Vector3 center = null, double radius = 0)
		{
			this.center = center ?? new Vector3();
			this.radius = radius;
		}

		public Sphere set(Vector3 center, double radius)
		{
			this.center.copy(center);
			this.radius = radius;
			return this;
		}

		public Sphere setFromCenterAndPoints(Vector3 center, JSArray points)
		{
			double maxRadiusSq = 0;
			var il = points.length;
			for (var i = 0; i < il; i++)
			{
				var radiusSq = center.distanceToSquared(points[i] as Vector3);
				maxRadiusSq = System.Math.Max(maxRadiusSq, radiusSq);
			}

			this.center = center;
			radius = System.Math.Sqrt(maxRadiusSq);

			return this;
		}

		public Sphere copy(Sphere sphere)
		{
			center.copy(sphere.center);
			radius = sphere.radius;

			return this;
		}

		public bool empty()
		{
			return (radius <= 0);
		}

		public bool containsPoint(Vector3 point)
		{
			return (point.distanceToSquared(center) <= (radius * radius));
		}

		public double distanceToPoint(Vector3 point)
		{
			return (point.distanceTo(center) - radius);
		}

		public bool intersectsSphere(Sphere sphere)
		{
			var radiusSum = radius + sphere.radius;

			return sphere.center.distanceToSquared(center) <= (radiusSum * radiusSum);
		}

		public Vector3 clampPoint(Vector3 point, Vector3 optionalTarget = null)
		{
			var deltaLengthSq = center.distanceToSquared(point);

			var result = optionalTarget ?? new Vector3();
			result.copy(point);

			if (deltaLengthSq > (radius * radius))
			{
				result.sub(center).normalize();
				result.multiplyScalar(radius).add(center);
			}

			return result;
		}

		public Box3 getBoundingBox(Box3 optionalTarget = null)
		{
			var box = optionalTarget ?? new Box3();

			box.set(center, center);
			box.expandByScalar(radius);

			return box;
		}

		public Sphere transform(Matrix4 matrix)
		{
			center.applyMatrix4(matrix);
			radius = radius * matrix.getMaxScaleOnAxis();

			return this;
		}

		public Sphere translate(Vector3 offset)
		{
			center.add(offset);

			return this;
		}

		public bool equals(Sphere sphere)
		{
			return sphere.center.equals(center) && (sphere.radius == radius);
		}

		public Sphere clone()
		{
			return new Sphere().copy(this);
		}
	}
}
