namespace THREE
{
	public class Plane
	{
		private static readonly Vector3 __vZero = new Vector3(0, 0, 0);
		private static readonly Vector3 __v1 = new Vector3();
		private static readonly Vector3 __v2 = new Vector3();
		public Vector3 normal;
		public double constant;

		public Plane(Vector3 normal = null, double constant = 0)
		{
			this.normal = normal ?? new Vector3(1, 0, 0);
			this.constant = constant;
		}

		public Plane set(Vector3 normal, double constant)
		{
			this.normal.copy(normal);
			this.constant = constant;

			return this;
		}

		public Plane setComponents(double x, double y, double z, double w)
		{
			normal.set(x, y, z);
			constant = w;

			return this;
		}

		public Plane setFromNormalAndCoplanarPoint(Vector3 normal, Vector3 point)
		{
			this.normal.copy(normal);
			constant = - point.dot(this.normal);

			return this;
		}

		public Plane setFromCoplanarPoints(Vector3 a, Vector3 b, Vector3 c)
		{
			var normal = __v1.subVectors(c, b).cross(__v2.subVectors(a, b)).normalize();

			setFromNormalAndCoplanarPoint(normal, a);

			return this;
		}

		public Plane copy(Plane plane)
		{
			normal.copy(plane.normal);
			constant = plane.constant;

			return this;
		}

		public Plane normalize()
		{
			var inverseNormalLength = 1.0 / normal.length();
			normal.multiplyScalar(inverseNormalLength);
			constant *= inverseNormalLength;

			return this;
		}

		public Plane negate()
		{
			constant *= -1;
			normal.negate();

			return this;
		}

		public double distanceToPoint(Vector3 point)
		{
			return normal.dot(point) + constant;
		}

		public double distanceToSphere(Sphere sphere)
		{
			return distanceToPoint(sphere.center) - sphere.radius;
		}

		public Vector3 projectPoint(Vector3 point, Vector3 optionalTarget = null)
		{
			return orthoPoint(point, optionalTarget).sub(point).negate();
		}

		public Vector3 orthoPoint(Vector3 point, Vector3 optionalTarget = null)
		{
			var perpendicularMagnitude = distanceToPoint(point);

			var result = optionalTarget ?? new Vector3();
			return result.copy(normal).multiplyScalar(perpendicularMagnitude);
		}

		public bool isIntersectionLine(Vector3 startPoint, Vector3 endPoint)
		{
			var startSign = distanceToPoint(startPoint);
			var endSign = distanceToPoint(endPoint);

			return (startSign < 0 && endSign > 0) || (endSign < 0 && startSign > 0);
		}

		public Vector3 intersectLine(Vector3 startPoint, Vector3 endPoint, Vector3 optionalTarget = null)
		{
			var result = optionalTarget ?? new Vector3();

			var direction = __v1.subVectors(endPoint, startPoint);

			var denominator = normal.dot(direction);

			if (denominator == 0)
			{
				if (distanceToPoint(startPoint) == 0)
				{
					return result.copy(startPoint);
				}

				return null;
			}

			var t = - (startPoint.dot(normal) + constant) / denominator;

			if (t < 0 || t > 1)
			{
				return null;
			}

			return result.copy(direction).multiplyScalar(t).add(startPoint);
		}

		public Vector3 coplanarPoint(Vector3 optionalTarget = null)
		{
			var result = optionalTarget ?? new Vector3();
			return result.copy(normal).multiplyScalar(- constant);
		}

		public Plane transform(Matrix4 matrix, Matrix3 optionalNormalMatrix = null)
		{
			optionalNormalMatrix = optionalNormalMatrix ?? new Matrix3().getInverse(matrix).transpose();
			var newNormal = __v1.copy(normal).applyMatrix3(optionalNormalMatrix);

			var newCoplanarPoint = coplanarPoint(__v2);
			newCoplanarPoint.applyMatrix4(matrix);

			setFromNormalAndCoplanarPoint(newNormal, newCoplanarPoint);

			return this;
		}

		public Plane translate(Vector3 offset)
		{
			constant = constant - offset.dot(normal);

			return this;
		}

		public bool equals(Plane plane)
		{
			return plane.normal.equals(normal) && (plane.constant == constant);
		}

		public Plane clone()
		{
			return new Plane().copy(this);
		}
	}
}
