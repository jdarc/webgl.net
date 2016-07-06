using WebGL;

namespace THREE
{
	public class Box3
	{
		private static readonly Vector3 __v0 = new Vector3();
		private static readonly Vector3 __v1 = new Vector3();
		private static readonly Vector3 __v2 = new Vector3();
		private static readonly Vector3 __v3 = new Vector3();
		private static readonly Vector3 __v4 = new Vector3();
		private static readonly Vector3 __v5 = new Vector3();
		private static readonly Vector3 __v6 = new Vector3();
		private static readonly Vector3 __v7 = new Vector3();

		public readonly Vector3 min;
		public readonly Vector3 max;

		public Box3(Vector3 min = null, Vector3 max = null)
		{
			this.min = min ?? new Vector3(double.PositiveInfinity, double.PositiveInfinity, double.PositiveInfinity);
			this.max = max ?? new Vector3(double.NegativeInfinity, double.NegativeInfinity, double.NegativeInfinity);
		}

		public Box3 set(Vector3 minArg, Vector3 maxArg)
		{
			minArg.copy(minArg);
			maxArg.copy(maxArg);

			return this;
		}

		public Box3 setFromPoints(JSArray points)
		{
			if (points.length > 0)
			{
				var point = points[0];

				min.copy(point);
				max.copy(point);

				for (var i = 1; i < points.length; i++)
				{
					point = points[i];

					if (point.x < min.x)
					{
						min.x = point.x;
					}
					else if (point.x > max.x)
					{
						max.x = point.x;
					}

					if (point.y < min.y)
					{
						min.y = point.y;
					}
					else if (point.y > max.y)
					{
						max.y = point.y;
					}

					if (point.z < min.z)
					{
						min.z = point.z;
					}
					else if (point.z > max.z)
					{
						max.z = point.z;
					}
				}
			}
			else
			{
				makeEmpty();
			}

			return this;
		}

		public Box3 setFromCenterAndSize(Vector3 center, Vector3 size)
		{
			var halfSize = __v1.copy(size).multiplyScalar(0.5);

			min.copy(center).sub(halfSize);
			max.copy(center).add(halfSize);

			return this;
		}

		public Box3 copy(Box3 box)
		{
			min.copy(box.min);
			max.copy(box.max);

			return this;
		}

		public Box3 makeEmpty()
		{
			min.x = min.y = min.z = double.PositiveInfinity;
			max.x = max.y = max.z = double.NegativeInfinity;

			return this;
		}

		public bool empty()
		{
			return (max.x < min.x) || (max.y < min.y) || (max.z < min.z);
		}

		public Vector3 center(Vector3 optionalTarget = null)
		{
			var result = optionalTarget ?? new Vector3();
			return result.addVectors(min, max).multiplyScalar(0.5);
		}

		public Vector3 size(Vector3 optionalTarget = null)
		{
			var result = optionalTarget ?? new Vector3();
			return result.subVectors(max, min);
		}

		public Box3 expandByPoint(Vector3 point)
		{
			min.min(point);
			max.max(point);

			return this;
		}

		public Box3 expandByVector(Vector3 vector)
		{
			min.sub(vector);
			max.add(vector);

			return this;
		}

		public Box3 expandByScalar(double scalar)
		{
			min.addScalar(-scalar);
			max.addScalar(scalar);

			return this;
		}

		public bool containsPoint(Vector3 point)
		{
			if (point.x < min.x || point.x > max.x ||
			    point.y < min.y || point.y > max.y ||
			    point.z < min.z || point.z > max.z)
			{
				return false;
			}

			return true;
		}

		public bool containsBox(Box3 box)
		{
			if ((min.x <= box.min.x) && (box.max.x <= max.x) &&
			    (min.y <= box.min.y) && (box.max.y <= max.y) &&
			    (min.z <= box.min.z) && (box.max.z <= max.z))
			{
				return true;
			}

			return false;
		}

		public Vector3 getParameter(Vector3 point)
		{
			return new Vector3(
				(point.x - min.x) / (max.x - min.x),
				(point.y - min.y) / (max.y - min.y),
				(point.z - min.z) / (max.z - min.z)
				);
		}

		public bool isIntersectionBox(Box3 box)
		{
			if (box.max.x < min.x || box.min.x > max.x ||
			    box.max.y < min.y || box.min.y > max.y ||
			    box.max.z < min.z || box.min.z > max.z)
			{
				return false;
			}

			return true;
		}

		public Vector3 clampPoint(Vector3 point, Vector3 optionalTarget = null)
		{
			return new Vector3().copy(point).clamp(min, max);
		}

		public double distanceToPoint(Vector3 point)
		{
			var clampedPoint = __v1.copy(point).clamp(min, max);
			return clampedPoint.sub(point).length();
		}

		public Box3 getBoundingSphere(dynamic optionalTarget = null)
		{
			var result = optionalTarget ?? new Sphere();

			result.center = center();
			result.radius = size(__v0).length() * 0.5;

			return result;
		}

		public Box3 intersect(Box3 box)
		{
			min.max(box.min);
			max.min(box.max);

			return this;
		}

		public Box3 union(Box3 box)
		{
			min.min(box.min);
			max.max(box.max);

			return this;
		}

		public Box3 transform(Matrix4 matrix)
		{
			var newPoints = new JSArray(__v0.set(min.x, min.y, min.z).applyMatrix4(matrix),
			                          __v0.set(min.x, min.y, min.z).applyMatrix4(matrix),
			                          __v1.set(min.x, min.y, max.z).applyMatrix4(matrix),
			                          __v2.set(min.x, max.y, min.z).applyMatrix4(matrix),
			                          __v3.set(min.x, max.y, max.z).applyMatrix4(matrix),
			                          __v4.set(max.x, min.y, min.z).applyMatrix4(matrix),
			                          __v5.set(max.x, min.y, max.z).applyMatrix4(matrix),
			                          __v6.set(max.x, max.y, min.z).applyMatrix4(matrix),
			                          __v7.set(max.x, max.y, max.z).applyMatrix4(matrix)
				);

			makeEmpty();
			setFromPoints(newPoints);

			return this;
		}

		public Box3 translate(Vector3 offset)
		{
			min.add(offset);
			max.add(offset);

			return this;
		}

		public bool equals(Box3 box)
		{
			return box.min.equals(min) && box.max.equals(max);
		}

		public Box3 clone()
		{
			return new Box3().copy(this);
		}
	}
}
