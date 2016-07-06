using System;
using WebGL;

namespace THREE
{
	public class Vector3
	{
		private static readonly Quaternion __q1 = new Quaternion();

		public double x;
		public double y;
		public double z;

		public Vector3(double x = 0.0, double y = 0.0, double z = 0.0)
		{
			this.x = x;
			this.y = y;
			this.z = z;
		}

		public double this[char c]
		{
			get
			{
				switch (c)
				{
					case 'x':
						return x;
					case 'y':
						return y;
					case 'z':
						return z;
					default:
						throw new ArgumentException(c.ToString());
				}
			}
			set
			{
				switch (c)
				{
					case 'x':
						x = value;
						break;
					case 'y':
						y = value;
						break;
					case 'z':
						z = value;
						break;
					default:
						throw new ArgumentException(c.ToString());
				}
			}
		}

		public Vector3 set(double x, double y, double z)
		{
			this.x = x;
			this.y = y;
			this.z = z;

			return this;
		}

		public Vector3 setX(double x)
		{
			this.x = x;

			return this;
		}

		public Vector3 setY(double y)
		{
			this.y = y;

			return this;
		}

		public Vector3 setZ(double z)
		{
			this.z = z;

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
				default:
                    throw new ApplicationException("index is out of range: " + index);
			}
		}

		public Vector3 copy(Vector3 v)
		{
			x = v.x;
			y = v.y;
			z = v.z;

			return this;
		}

		public Vector3 add(Vector3 v, Vector3 w = null)
		{
			if (w != null)
			{
				JSConsole.warn("DEPRECATED: Vector3\'s .add() now only accepts one argument. Use .addVectors( a, b ) instead.");
				return addVectors(v, w);
			}

			x += v.x;
			y += v.y;
			z += v.z;

			return this;
		}

		public Vector3 addScalar(double s)
		{
			x += s;
			y += s;
			z += s;

			return this;
		}

		public Vector3 addVectors(Vector3 a, Vector3 b)
		{
			x = a.x + b.x;
			y = a.y + b.y;
			z = a.z + b.z;

			return this;
		}

		public Vector3 sub(Vector3 v, Vector3 w = null)
		{
			if (w != null)
			{
				JSConsole.warn("DEPRECATED: Vector3\'s .sub() now only accepts one argument. Use .subVectors( a, b ) instead.");
				return subVectors(v, w);
			}

			x -= v.x;
			y -= v.y;
			z -= v.z;

			return this;
		}

		public Vector3 subVectors(Vector3 a, Vector3 b)
		{
			x = a.x - b.x;
			y = a.y - b.y;
			z = a.z - b.z;

			return this;
		}

		public Vector3 multiply(Vector3 v, Vector3 w = null)
		{
			if (w != null)
			{
				JSConsole.warn("DEPRECATED: Vector3\'s .multiply() now only accepts one argument. Use .multiplyVectors( a, b ) instead.");
				return multiplyVectors(v, w);
			}

			x *= v.x;
			y *= v.y;
			z *= v.z;

			return this;
		}

		public Vector3 multiplyScalar(double s)
		{
			x *= s;
			y *= s;
			z *= s;

			return this;
		}

		public Vector3 multiplyVectors(Vector3 a, Vector3 b)
		{
			x = a.x * b.x;
			y = a.y * b.y;
			z = a.z * b.z;

			return this;
		}

		public Vector3 applyMatrix3(Matrix3 m)
		{
			var x = this.x;
			var y = this.y;
			var z = this.z;

			var e = m.elements;

			this.x = e[0] * x + e[3] * y + e[6] * z;
			this.y = e[1] * x + e[4] * y + e[7] * z;
			this.z = e[2] * x + e[5] * y + e[8] * z;

			return this;
		}

		public Vector3 applyMatrix4(Matrix4 m)
		{
			double x = this.x, y = this.y, z = this.z;

			var e = m.elements;

			this.x = e[0] * x + e[4] * y + e[8] * z + e[12];
			this.y = e[1] * x + e[5] * y + e[9] * z + e[13];
			this.z = e[2] * x + e[6] * y + e[10] * z + e[14];

			return this;
		}

		public Vector3 applyProjection(Matrix4 m)
		{
			double x = this.x, y = this.y, z = this.z;

			var e = m.elements;
			var d = 1 / (e[3] * x + e[7] * y + e[11] * z + e[15]);

			this.x = (e[0] * x + e[4] * y + e[8] * z + e[12]) * d;
			this.y = (e[1] * x + e[5] * y + e[9] * z + e[13]) * d;
			this.z = (e[2] * x + e[6] * y + e[10] * z + e[14]) * d;

			return this;
		}

		public Vector3 applyQuaternion(Quaternion q)
		{
			var x = this.x;
			var y = this.y;
			var z = this.z;

			var qx = q.x;
			var qy = q.y;
			var qz = q.z;
			var qw = q.w;

			var ix = qw * x + qy * z - qz * y;
			var iy = qw * y + qz * x - qx * z;
			var iz = qw * z + qx * y - qy * x;
			var iw = -qx * x - qy * y - qz * z;

			this.x = ix * qw + iw * -qx + iy * -qz - iz * -qy;
			this.y = iy * qw + iw * -qy + iz * -qx - ix * -qz;
			this.z = iz * qw + iw * -qz + ix * -qy - iy * -qx;

			return this;
		}

		public Vector3 applyEuler(Vector3 v, string eulerOrder)
		{
			var quaternion = __q1.setFromEuler(v, eulerOrder);

			applyQuaternion(quaternion);

			return this;
		}

		public Vector3 applyAxisAngle(Vector3 axis, double angle)
		{
			var quaternion = __q1.setFromAxisAngle(axis, angle);

			applyQuaternion(quaternion);

			return this;
		}

		public Vector3 divide(Vector3 v)
		{
			x /= v.x;
			y /= v.y;
			z /= v.z;

			return this;
		}

		public Vector3 divideScalar(double s)
		{
			if (s != 0)
			{
				x /= s;
				y /= s;
				z /= s;
			}
			else
			{
				x = 0;
				y = 0;
				z = 0;
			}

			return this;
		}

		public Vector3 min(Vector3 v)
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

			return this;
		}

		public Vector3 max(Vector3 v)
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

			return this;
		}

		public Vector3 clamp(Vector3 min, Vector3 max)
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

			return this;
		}

		public Vector3 negate()
		{
			return multiplyScalar(- 1);
		}

		public double dot(Vector3 v)
		{
			return x * v.x + y * v.y + z * v.z;
		}

		public double lengthSq()
		{
			return x * x + y * y + z * z;
		}

		public double length()
		{
			return System.Math.Sqrt(x * x + y * y + z * z);
		}

		public double lengthManhattan()
		{
			return System.Math.Abs(x) + System.Math.Abs(y) + System.Math.Abs(z);
		}

		public Vector3 normalize()
		{
			return divideScalar(length());
		}

		public Vector3 setLength(double l)
		{
			var oldLength = length();

			if (oldLength != 0 && l != oldLength)
			{
				multiplyScalar(l / oldLength);
			}

			return this;
		}

		public Vector3 lerp(Vector3 v, double alpha)
		{
			x += (v.x - x) * alpha;
			y += (v.y - y) * alpha;
			z += (v.z - z) * alpha;

			return this;
		}

		public Vector3 cross(Vector3 v, Vector3 w = null)
		{
			if (w != null)
			{
				JSConsole.warn("DEPRECATED: Vector3\'s .cross() now only accepts one argument. Use .crossVectors( a, b ) instead.");
				return crossVectors(v, w);
			}

			double x = this.x, y = this.y, z = this.z;

			this.x = y * v.z - z * v.y;
			this.y = z * v.x - x * v.z;
			this.z = x * v.y - y * v.x;

			return this;
		}

		public Vector3 crossVectors(Vector3 a, Vector3 b)
		{
			x = a.y * b.z - a.z * b.y;
			y = a.z * b.x - a.x * b.z;
			z = a.x * b.y - a.y * b.x;

			return this;
		}

		public double angleTo(Vector3 v)
		{
			return System.Math.Acos(dot(v) / length() / v.length());
		}

		public double distanceTo(Vector3 v)
		{
			return System.Math.Sqrt(distanceToSquared(v));
		}

		public double distanceToSquared(Vector3 v)
		{
			var dx = x - v.x;
			var dy = y - v.y;
			var dz = z - v.z;

			return dx * dx + dy * dy + dz * dz;
		}

		public Vector3 getPositionFromMatrix(Matrix4 m)
		{
			x = m.elements[12];
			y = m.elements[13];
			z = m.elements[14];

			return this;
		}

		public Vector3 setEulerFromRotationMatrix(Matrix4 m, string order)
		{
			Func<double, double> clamp = d => System.Math.Min(System.Math.Max(d, -1.0), 1.0);

			var te = m.elements;
			double m11 = te[0], m12 = te[4], m13 = te[8];
			double m21 = te[1], m22 = te[5], m23 = te[9];
			double m31 = te[2], m32 = te[6], m33 = te[10];

			if (order == null || order == "XYZ")
			{
				y = System.Math.Asin(clamp(m13));

				if (System.Math.Abs(m13) < 0.99999)
				{
					x = System.Math.Atan2(- m23, m33);
					z = System.Math.Atan2(- m12, m11);
				}
				else
				{
					x = System.Math.Atan2(m32, m22);
					z = 0;
				}
			}
			else if (order == "YXZ")
			{
				x = System.Math.Asin(- clamp(m23));

				if (System.Math.Abs(m23) < 0.99999)
				{
					y = System.Math.Atan2(m13, m33);
					z = System.Math.Atan2(m21, m22);
				}
				else
				{
					y = System.Math.Atan2(- m31, m11);
					z = 0;
				}
			}
			else if (order == "ZXY")
			{
				x = System.Math.Asin(clamp(m32));

				if (System.Math.Abs(m32) < 0.99999)
				{
					y = System.Math.Atan2(- m31, m33);
					z = System.Math.Atan2(- m12, m22);
				}
				else
				{
					y = 0;
					z = System.Math.Atan2(m21, m11);
				}
			}
			else if (order == "ZYX")
			{
				y = System.Math.Asin(- clamp(m31));

				if (System.Math.Abs(m31) < 0.99999)
				{
					x = System.Math.Atan2(m32, m33);
					z = System.Math.Atan2(m21, m11);
				}
				else
				{
					x = 0;
					z = System.Math.Atan2(- m12, m22);
				}
			}
			else if (order == "YZX")
			{
				z = System.Math.Asin(clamp(m21));

				if (System.Math.Abs(m21) < 0.99999)
				{
					x = System.Math.Atan2(- m23, m22);
					y = System.Math.Atan2(- m31, m11);
				}
				else
				{
					x = 0;
					y = System.Math.Atan2(m13, m33);
				}
			}
			else if (order == "XZY")
			{
				z = System.Math.Asin(- clamp(m12));

				if (System.Math.Abs(m12) < 0.99999)
				{
					x = System.Math.Atan2(m32, m22);
					y = System.Math.Atan2(m13, m11);
				}
				else
				{
					x = System.Math.Atan2(- m23, m33);
					y = 0;
				}
			}

			return this;
		}

		public Vector3 setEulerFromQuaternion(Quaternion q, string order)
		{
			Func<double, double> clamp = d => System.Math.Min(System.Math.Max(d, -1.0), 1.0);

			var sqx = q.x * q.x;
			var sqy = q.y * q.y;
			var sqz = q.z * q.z;
			var sqw = q.w * q.w;

			if (order == null || order == "XYZ")
			{
				x = System.Math.Atan2(2 * (q.x * q.w - q.y * q.z), (sqw - sqx - sqy + sqz));
				y = System.Math.Asin(clamp(2 * (q.x * q.z + q.y * q.w)));
				z = System.Math.Atan2(2 * (q.z * q.w - q.x * q.y), (sqw + sqx - sqy - sqz));
			}
			else if (order == "YXZ")
			{
				x = System.Math.Asin(clamp(2 * (q.x * q.w - q.y * q.z)));
				y = System.Math.Atan2(2 * (q.x * q.z + q.y * q.w), (sqw - sqx - sqy + sqz));
				z = System.Math.Atan2(2 * (q.x * q.y + q.z * q.w), (sqw - sqx + sqy - sqz));
			}
			else if (order == "ZXY")
			{
				x = System.Math.Asin(clamp(2 * (q.x * q.w + q.y * q.z)));
				y = System.Math.Atan2(2 * (q.y * q.w - q.z * q.x), (sqw - sqx - sqy + sqz));
				z = System.Math.Atan2(2 * (q.z * q.w - q.x * q.y), (sqw - sqx + sqy - sqz));
			}
			else if (order == "ZYX")
			{
				x = System.Math.Atan2(2 * (q.x * q.w + q.z * q.y), (sqw - sqx - sqy + sqz));
				y = System.Math.Asin(clamp(2 * (q.y * q.w - q.x * q.z)));
				z = System.Math.Atan2(2 * (q.x * q.y + q.z * q.w), (sqw + sqx - sqy - sqz));
			}
			else if (order == "YZX")
			{
				x = System.Math.Atan2(2 * (q.x * q.w - q.z * q.y), (sqw - sqx + sqy - sqz));
				y = System.Math.Atan2(2 * (q.y * q.w - q.x * q.z), (sqw + sqx - sqy - sqz));
				z = System.Math.Asin(clamp(2 * (q.x * q.y + q.z * q.w)));
			}
			else if (order == "XZY")
			{
				x = System.Math.Atan2(2 * (q.x * q.w + q.y * q.z), (sqw - sqx + sqy - sqz));
				y = System.Math.Atan2(2 * (q.x * q.z + q.y * q.w), (sqw + sqx - sqy - sqz));
				z = System.Math.Asin(clamp(2 * (q.z * q.w - q.x * q.y)));
			}

			return this;
		}

		public Vector3 getScaleFromMatrix(Matrix4 m)
		{
			var sx = set(m.elements[0], m.elements[1], m.elements[2]).length();
			var sy = set(m.elements[4], m.elements[5], m.elements[6]).length();
			var sz = set(m.elements[8], m.elements[9], m.elements[10]).length();

			x = sx;
			y = sy;
			z = sz;

			return this;
		}

		public bool equals(Vector3 v)
		{
			return ((v.x == x) && (v.y == y) && (v.z == z));
		}

		public Vector3 clone()
		{
			return new Vector3(x, y, z);
		}
	}
}
