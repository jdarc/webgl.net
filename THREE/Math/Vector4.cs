using System;
using WebGL;

namespace THREE
{
	public class Vector4
	{
		public double x;
		public double y;
		public double z;
		public double w;

		public Vector4(double x = 0, double y = 0, double z = 0, double w = 1)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public Vector4 set(double x, double y, double z, double w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;

			return this;
		}

		public Vector4 setX(double x)
		{
			this.x = x;

			return this;
		}

		public Vector4 setY(double y)
		{
			this.y = y;

			return this;
		}

		public Vector4 setZ(double z)
		{
			this.z = z;

			return this;
		}

		public Vector4 setW(double w)
		{
			this.w = w;

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
				case 2:
					z = value;
					break;
				case 3:
					w = value;
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
				case 2:
					return z;
				case 3:
					return w;
				default:
                    throw new ApplicationException("index is out of range: " + index);
			}
		}

		public Vector4 copy(Vector4 v)
		{
			x = v.x;
			y = v.y;
			z = v.z;
			w = v.w;

			return this;
		}

		public Vector4 add(Vector4 v, Vector4 w = null)
		{
			if (w != null)
			{
				JSConsole.warn("DEPRECATED: Vector4\'s .add() now only accepts one argument. Use .addVectors( a, b ) instead.");
				return addVectors(v, w);
			}

			x += v.x;
			y += v.y;
			z += v.z;
			this.w += v.w;

			return this;
		}

		public Vector4 addScalar(double s)
		{
			x += s;
			y += s;
			z += s;
			w += s;

			return this;
		}

		public Vector4 addVectors(Vector4 a, Vector4 b)
		{
			x = a.x + b.x;
			y = a.y + b.y;
			z = a.z + b.z;
			w = a.w + b.w;

			return this;
		}

		public Vector4 sub(Vector4 v, Vector4 w = null)
		{
			if (w != null)
			{
				JSConsole.warn("DEPRECATED: Vector4\'s .sub() now only accepts one argument. Use .subVectors( a, b ) instead.");
				return subVectors(v, w);
			}

			x -= v.x;
			y -= v.y;
			z -= v.z;
			this.w -= v.w;

			return this;
		}

		public Vector4 subVectors(Vector4 a, Vector4 b)
		{
			x = a.x - b.x;
			y = a.y - b.y;
			z = a.z - b.z;
			w = a.w - b.w;

			return this;
		}

		public Vector4 multiplyScalar(double s)
		{
			x *= s;
			y *= s;
			z *= s;
			w *= s;

			return this;
		}

		public Vector4 applyMatrix4(Matrix4 m)
		{
			var x = this.x;
			var y = this.y;
			var z = this.z;
			var w = this.w;

			var e = m.elements;

			this.x = e[0] * x + e[4] * y + e[8] * z + e[12] * w;
			this.y = e[1] * x + e[5] * y + e[9] * z + e[13] * w;
			this.z = e[2] * x + e[6] * y + e[10] * z + e[14] * w;
			this.w = e[3] * x + e[7] * y + e[11] * z + e[15] * w;

			return this;
		}

		public Vector4 divideScalar(double s)
		{
			if (s != 0)
			{
				x /= s;
				y /= s;
				z /= s;
				w /= s;
			}
			else
			{
				x = 0;
				y = 0;
				z = 0;
				w = 1;
			}

			return this;
		}

		public Vector4 min(Vector4 v)
		{
			if (x > v.x)
			{
				x = v.x;
			}

			if (y > v.y)
			{
				y = v.y;
			}

			if (z > v.z)
			{
				z = v.z;
			}

			if (w > v.w)
			{
				w = v.w;
			}

			return this;
		}

		public Vector4 max(Vector4 v)
		{
			if (x < v.x)
			{
				x = v.x;
			}

			if (y < v.y)
			{
				y = v.y;
			}

			if (z < v.z)
			{
				z = v.z;
			}

			if (w < v.w)
			{
				w = v.w;
			}

			return this;
		}

		public Vector4 clamp(Vector4 min, Vector4 max)
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

			if (z < min.z)
			{
				z = min.z;
			}
			else if (z > max.z)
			{
				z = max.z;
			}

			if (w < min.w)
			{
				w = min.w;
			}
			else if (w > max.w)
			{
				w = max.w;
			}

			return this;
		}

		public Vector4 negate()
		{
			return multiplyScalar(-1);
		}

		public double dot(Vector4 v)
		{
			return x * v.x + y * v.y + z * v.z + w * v.w;
		}

		public double lengthSq()
		{
			return x * x + y * y + z * z + w * w;
		}

		public double length()
		{
			return System.Math.Sqrt(x * x + y * y + z * z + w * w);
		}

		public double lengthManhattan()
		{
			return System.Math.Abs(x) + System.Math.Abs(y) + System.Math.Abs(z) + System.Math.Abs(w);
		}

		public Vector4 normalize()
		{
			return divideScalar(length());
		}

		public Vector4 setLength(double l)
		{
			var oldLength = length();

			if (oldLength != 0 && l != oldLength)
			{
				multiplyScalar(l / oldLength);
			}

			return this;
		}

		public Vector4 lerp(Vector4 v, double alpha)
		{
			x += (v.x - x) * alpha;
			y += (v.y - y) * alpha;
			z += (v.z - z) * alpha;
			w += (v.w - w) * alpha;

			return this;
		}

		public bool equals(Vector4 v)
		{
			return ((v.x == x) && (v.y == y) && (v.z == z) && (v.w == w));
		}

		public Vector4 clone()
		{
			return new Vector4(x, y, z, w);
		}

		public Vector4 setAxisAngleFromQuaternion(Quaternion q)
		{
			w = 2 * System.Math.Acos(q.w);

			var s = System.Math.Sqrt(1 - q.w * q.w);

			if (s < 0.0001)
			{
				x = 1;
				y = 0;
				z = 0;
			}
			else
			{
				x = q.x / s;
				y = q.y / s;
				z = q.z / s;
			}

			return this;
		}

		public Vector4 setAxisAngleFromRotationMatrix(Matrix4 m)
		{
			double x, y, z;
			const double epsilon = 0.01,
			             epsilon2 = 0.1;

			var te = m.elements;

			double m11 = te[0], m12 = te[4], m13 = te[8],
			       m21 = te[1], m22 = te[5], m23 = te[9],
			       m31 = te[2], m32 = te[6], m33 = te[10];

			if ((System.Math.Abs(m12 - m21) < epsilon)
			    && (System.Math.Abs(m13 - m31) < epsilon)
			    && (System.Math.Abs(m23 - m32) < epsilon))
			{
				if ((System.Math.Abs(m12 + m21) < epsilon2)
				    && (System.Math.Abs(m13 + m31) < epsilon2)
				    && (System.Math.Abs(m23 + m32) < epsilon2)
				    && (System.Math.Abs(m11 + m22 + m33 - 3) < epsilon2))
				{
					set(1, 0, 0, 0);

					return this;
				}

				const double angle = System.Math.PI;

				var xx = (m11 + 1) / 2;
				var yy = (m22 + 1) / 2;
				var zz = (m33 + 1) / 2;
				var xy = (m12 + m21) / 4;
				var xz = (m13 + m31) / 4;
				var yz = (m23 + m32) / 4;

				if ((xx > yy) && (xx > zz))
				{
					if (xx < epsilon)
					{
						x = 0;
						y = 0.707106781;
						z = 0.707106781;
					}
					else
					{
						x = System.Math.Sqrt(xx);
						y = xy / x;
						z = xz / x;
					}
				}
				else if (yy > zz)
				{
					if (yy < epsilon)
					{
						x = 0.707106781;
						y = 0;
						z = 0.707106781;
					}
					else
					{
						y = System.Math.Sqrt(yy);
						x = xy / y;
						z = yz / y;
					}
				}
				else
				{
					if (zz < epsilon)
					{
						x = 0.707106781;
						y = 0.707106781;
						z = 0;
					}
					else
					{
						z = System.Math.Sqrt(zz);
						x = xz / z;
						y = yz / z;
					}
				}

				set(x, y, z, angle);

				return this;
			}

			var s = System.Math.Sqrt((m32 - m23) * (m32 - m23)
			                         + (m13 - m31) * (m13 - m31)
			                         + (m21 - m12) * (m21 - m12));

			if (System.Math.Abs(s) < 0.001)
			{
				s = 1;
			}

			this.x = (m32 - m23) / s;
			this.y = (m13 - m31) / s;
			this.z = (m21 - m12) / s;
			w = System.Math.Acos((m11 + m22 + m33 - 1) / 2);

			return this;
		}
	}
}
