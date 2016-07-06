using System;
using WebGL;

namespace THREE
{
	public class Vector2
	{
		public double x;
		public double y;

		public Vector2(double x = 0, double y = 0)
		{
			this.x = x;
			this.y = y;
		}

		public Vector2 set(double x, double y)
		{
			this.x = x;
			this.y = y;

			return this;
		}

		public Vector2 setX(double x)
		{
			this.x = x;

			return this;
		}

		public Vector2 setY(double y)
		{
			this.y = y;

			return this;
		}

		public void setComponent(int index, double value)
		{
			switch (index)
			{
				case 0:
					x = value;
					break;
				case 1:
					y = value;
					break;
				default:
                    throw new ApplicationException("index is out of range: " + index);
			}
		}

		public double getComponent(int index)
		{
			switch (index)
			{
				case 0:
					return x;
				case 1:
					return y;
				default:
                    throw new ApplicationException("index is out of range: " + index);
			}
		}

		public Vector2 copy(Vector2 v)
		{
			x = v.x;
			y = v.y;

			return this;
		}

		public Vector2 add(Vector2 v, Vector2 w = null)
		{
			if (w != null)
			{
				JSConsole.warn("DEPRECATED: Vector2\'s .add() now only accepts one argument. Use .addVectors( a, b ) instead.");
				return addVectors(v, w);
			}

			x += v.x;
			y += v.y;

			return this;
		}

		public Vector2 addVectors(Vector2 a, Vector2 b)
		{
			x = a.x + b.x;
			y = a.y + b.y;

			return this;
		}

		public Vector2 addScalar(double s)
		{
			x += s;
			y += s;

			return this;
		}

		public Vector2 sub(Vector2 v, Vector2 w = null)
		{
			if (w != null)
			{
				JSConsole.warn("DEPRECATED: Vector2\'s .sub() now only accepts one argument. Use .subVectors( a, b ) instead.");
				return subVectors(v, w);
			}

			x -= v.x;
			y -= v.y;

			return this;
		}

		public Vector2 subVectors(Vector2 a, Vector2 b)
		{
			x = a.x - b.x;
			y = a.y - b.y;

			return this;
		}

		public Vector2 multiplyScalar(double s)
		{
			x *= s;
			y *= s;

			return this;
		}

		public Vector2 divideScalar(double s)
		{
			if (s != 0)
			{
				x /= s;
				y /= s;
			}
			else
			{
				set(0, 0);
			}

			return this;
		}

		public Vector2 min(Vector2 v)
		{
			if (x > v.x)
			{
				x = v.x;
			}

			if (y > v.y)
			{
				y = v.y;
			}

			return this;
		}

		public Vector2 max(Vector2 v)
		{
			if (x < v.x)
			{
				x = v.x;
			}

			if (y < v.y)
			{
				y = v.y;
			}

			return this;
		}

		public Vector2 clamp(Vector2 min, Vector2 max)
		{
			if (x < min.x)
			{
				x = min.x;
			}
			else if (x > max.x)
			{
				x = max.x;
			}

			if (y < min.y)
			{
				y = min.y;
			}
			else if (y > max.y)
			{
				y = max.y;
			}

			return this;
		}

		public Vector2 negate()
		{
			return multiplyScalar(- 1);
		}

		public double dot(Vector2 v)
		{
			return x * v.x + y * v.y;
		}

		public double lengthSq()
		{
			return x * x + y * y;
		}

		public double length()
		{
			return System.Math.Sqrt(x * x + y * y);
		}

		public Vector2 normalize()
		{
			return divideScalar(length());
		}

		public double distanceTo(Vector2 v)
		{
			return System.Math.Sqrt(distanceToSquared(v));
		}

		public double distanceToSquared(Vector2 v)
		{
			var dx = x - v.x;
			var dy = y - v.y;
			return dx * dx + dy * dy;
		}

		public Vector2 setLength(double l)
		{
			var oldLength = length();

			if (oldLength != 0 && l != oldLength)
			{
				multiplyScalar(l / oldLength);
			}

			return this;
		}

		public Vector2 lerp(Vector2 v, double alpha)
		{
			x += (v.x - x) * alpha;
			y += (v.y - y) * alpha;

			return this;
		}

		public bool equals(Vector2 v)
		{
			return ((v.x == x) && (v.y == y));
		}

		public Vector2 clone()
		{
			return new Vector2(x, y);
		}
	}
}
