namespace THREE
{
	public class Box2
	{
		private static readonly Vector2 __v1 = new Vector2();

		public readonly Vector2 min;
		public readonly Vector2 max;

		public Box2(Vector2 min = null, Vector2 max = null)
		{
			this.min = min ?? new Vector2(double.PositiveInfinity, double.PositiveInfinity);
			this.max = max ?? new Vector2(double.NegativeInfinity, double.NegativeInfinity);
		}

		public Box2 set(Vector2 minArg, Vector2 maxArg)
		{
			min.copy(minArg);
			max.copy(maxArg);

			return this;
		}

		public Box2 setFromPoints(dynamic points)
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
				}
			}
			else
			{
				makeEmpty();
			}

			return this;
		}

		public Box2 setFromCenterAndSize(dynamic center, dynamic size)
		{
			var halfSize = __v1.copy(size).multiplyScalar(0.5);
			min.copy(center).sub(halfSize);
			max.copy(center).add(halfSize);

			return this;
		}

		public Box2 copy(dynamic box)
		{
			min.copy(box.min);
			max.copy(box.max);

			return this;
		}

		public Box2 makeEmpty()
		{
			min.x = min.y = double.PositiveInfinity;
			max.x = max.y = double.NegativeInfinity;

			return this;
		}

		public bool empty()
		{
			return (max.x < min.x) || (max.y < min.y);
		}

		public Vector2 center(Vector2 optionalTarget)
		{
			var result = optionalTarget ?? new Vector2();
			return result.addVectors(min, max).multiplyScalar(0.5);
		}

		public Vector2 size(Vector2 optionalTarget)
		{
			var result = optionalTarget ?? new Vector2();
			return result.subVectors(max, min);
		}

		public Box2 expandByPoint(dynamic point)
		{
			min.min(point);
			max.max(point);

			return this;
		}

		public Box2 expandByVector(Vector2 vector)
		{
			min.sub(vector);
			max.add(vector);

			return this;
		}

		public Box2 expandByScalar(dynamic scalar)
		{
			min.addScalar(-scalar);
			max.addScalar(scalar);

			return this;
		}

		public bool containsPoint(dynamic point)
		{
			if (point.x < min.x || point.x > max.x ||
			    point.y < min.y || point.y > max.y)
			{
				return false;
			}

			return true;
		}

		public bool containsBox(dynamic box)
		{
			if ((min.x <= box.min.x) && (box.max.x <= max.x) &&
			    (min.y <= box.min.y) && (box.max.y <= max.y))
			{
				return true;
			}

			return false;
		}

		public Vector2 getParameter(dynamic point)
		{
			return new Vector2(
				(point.x - min.x) / (max.x - min.x),
				(point.y - min.y) / (max.y - min.y)
				);
		}

		public bool isIntersectionBox(dynamic box)
		{
			if (box.max.x < min.x || box.min.x > max.x ||
			    box.max.y < min.y || box.min.y > max.y)
			{
				return false;
			}

			return true;
		}

		public dynamic clampPoint(dynamic point, dynamic optionalTarget)
		{
			var result = optionalTarget || new Vector2();
			return result.copy(point).clamp(min, max);
		}

		public dynamic distanceToPoint(dynamic point)
		{
			var clampedPoint = __v1.copy(point).clamp(min, max);
			return clampedPoint.sub(point).length();
		}

		public Box2 intersect(dynamic box)
		{
			min.max(box.min);
			max.min(box.max);

			return this;
		}

		public Box2 union(dynamic box)
		{
			min.min(box.min);
			max.max(box.max);

			return this;
		}

		public Box2 translate(dynamic offset)
		{
			min.add(offset);
			max.add(offset);

			return this;
		}

		public dynamic equals(dynamic box)
		{
			return box.min.equals(min) && box.max.equals(max);
		}

		public Box2 clone()
		{
			return new Box2().copy(this);
		}
	}
}
