using WebGL;

namespace THREE
{
	public class Quaternion
	{
		public double x;
		public double y;
		public double z;
		public double w;

		public Quaternion(double x = 0, double y = 0, double z = 0, double w = 1)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;
		}

		public Quaternion set(double x, double y, double z, double w)
		{
			this.x = x;
			this.y = y;
			this.z = z;
			this.w = w;

			return this;
		}

		public Quaternion copy(Quaternion q)
		{
			x = q.x;
			y = q.y;
			z = q.z;
			w = q.w;

			return this;
		}

		public Quaternion setFromRotationMatrix(Matrix4 m)
		{
			var te = m.elements;

			double m11 = te[0], m12 = te[4], m13 = te[8],
			       m21 = te[1], m22 = te[5], m23 = te[9],
			       m31 = te[2], m32 = te[6], m33 = te[10];

			double trace = m11 + m22 + m33, s;

			if (trace > 0)
			{
				s = 0.5 / System.Math.Sqrt(trace + 1.0);

				w = 0.25 / s;
				x = (m32 - m23) * s;
				y = (m13 - m31) * s;
				z = (m21 - m12) * s;
			}
			else if (m11 > m22 && m11 > m33)
			{
				s = 2.0 * System.Math.Sqrt(1.0 + m11 - m22 - m33);

				w = (m32 - m23) / s;
				x = 0.25 * s;
				y = (m12 + m21) / s;
				z = (m13 + m31) / s;
			}
			else if (m22 > m33)
			{
				s = 2.0 * System.Math.Sqrt(1.0 + m22 - m11 - m33);

				w = (m13 - m31) / s;
				x = (m12 + m21) / s;
				y = 0.25 * s;
				z = (m23 + m32) / s;
			}
			else
			{
				s = 2.0 * System.Math.Sqrt(1.0 + m33 - m11 - m22);

				w = (m21 - m12) / s;
				x = (m13 + m31) / s;
				y = (m23 + m32) / s;
				z = 0.25 * s;
			}

			return this;
		}

		public Quaternion setFromAxisAngle(Vector3 axis, double angle)
		{
			double halfAngle = angle / 2.0, s = System.Math.Sin(halfAngle);

			x = axis.x * s;
			y = axis.y * s;
			z = axis.z * s;
			w = System.Math.Cos(halfAngle);

			return this;
		}

		public Quaternion setFromEuler(Vector3 v, string order = null)
		{
			var c1 = System.Math.Cos(v.x / 2);
			var c2 = System.Math.Cos(v.y / 2);
			var c3 = System.Math.Cos(v.z / 2);
			var s1 = System.Math.Sin(v.x / 2);
			var s2 = System.Math.Sin(v.y / 2);
			var s3 = System.Math.Sin(v.z / 2);

			if (order == null || order == "XYZ")
			{
				x = s1 * c2 * c3 + c1 * s2 * s3;
				y = c1 * s2 * c3 - s1 * c2 * s3;
				z = c1 * c2 * s3 + s1 * s2 * c3;
				w = c1 * c2 * c3 - s1 * s2 * s3;
			}
			else if (order == "YXZ")
			{
				x = s1 * c2 * c3 + c1 * s2 * s3;
				y = c1 * s2 * c3 - s1 * c2 * s3;
				z = c1 * c2 * s3 - s1 * s2 * c3;
				w = c1 * c2 * c3 + s1 * s2 * s3;
			}
			else if (order == "ZXY")
			{
				x = s1 * c2 * c3 - c1 * s2 * s3;
				y = c1 * s2 * c3 + s1 * c2 * s3;
				z = c1 * c2 * s3 + s1 * s2 * c3;
				w = c1 * c2 * c3 - s1 * s2 * s3;
			}
			else if (order == "ZYX")
			{
				x = s1 * c2 * c3 - c1 * s2 * s3;
				y = c1 * s2 * c3 + s1 * c2 * s3;
				z = c1 * c2 * s3 - s1 * s2 * c3;
				w = c1 * c2 * c3 + s1 * s2 * s3;
			}
			else if (order == "YZX")
			{
				x = s1 * c2 * c3 + c1 * s2 * s3;
				y = c1 * s2 * c3 + s1 * c2 * s3;
				z = c1 * c2 * s3 - s1 * s2 * c3;
				w = c1 * c2 * c3 - s1 * s2 * s3;
			}
			else if (order == "XZY")
			{
				x = s1 * c2 * c3 - c1 * s2 * s3;
				y = c1 * s2 * c3 - s1 * c2 * s3;
				z = c1 * c2 * s3 + s1 * s2 * c3;
				w = c1 * c2 * c3 + s1 * s2 * s3;
			}

			return this;
		}

		public Quaternion inverse()
		{
			conjugate().normalize();

			return this;
		}

		public Quaternion conjugate()
		{
			x *= -1;
			y *= -1;
			z *= -1;

			return this;
		}

		public double lengthSq()
		{
			return x * x + y * y + z * z + w * w;
		}

		public double length()
		{
			return System.Math.Sqrt(x * x + y * y + z * z + w * w);
		}

		public Quaternion normalize()
		{
			var l = length();

			if (l == 0)
			{
				x = 0;
				y = 0;
				z = 0;
				w = 1;
			}
			else
			{
				l = 1 / l;

				x = x * l;
				y = y * l;
				z = z * l;
				w = w * l;
			}

			return this;
		}

		public Quaternion multiply(Quaternion q, Quaternion p = null)
		{
			if (p != null)
			{
				JSConsole.warn("DEPRECATED: Quaternion\'s .multiply() now only accepts one argument. Use .multiplyQuaternions( a, b ) instead.");
				return multiplyQuaternions(q, p);
			}

			return multiplyQuaternions(this, q);
		}

		public Quaternion multiplyQuaternions(Quaternion a, Quaternion b)
		{
			double qax = a.x, qay = a.y, qaz = a.z, qaw = a.w;
			double qbx = b.x, qby = b.y, qbz = b.z, qbw = b.w;

			x = qax * qbw + qaw * qbx + qay * qbz - qaz * qby;
			y = qay * qbw + qaw * qby + qaz * qbx - qax * qbz;
			z = qaz * qbw + qaw * qbz + qax * qby - qay * qbx;
			w = qaw * qbw - qax * qbx - qay * qby - qaz * qbz;

			return this;
		}

		public Vector3 multiplyVector3(Vector3 vector)
		{
			JSConsole.warn("DEPRECATED: Quaternion\'s .multiplyVector3() has been removed. Use is now vector.applyQuaternion( quaternion ) instead.");
			return vector.applyQuaternion(this);
		}

		public Quaternion slerp(Quaternion qb, double t)
		{
			double x = this.x, y = this.y, z = this.z, w = this.w;

			var cosHalfTheta = w * qb.w + x * qb.x + y * qb.y + z * qb.z;

			if (cosHalfTheta < 0)
			{
				this.w = -qb.w;
				this.x = -qb.x;
				this.y = -qb.y;
				this.z = -qb.z;

				cosHalfTheta = -cosHalfTheta;
			}
			else
			{
				copy(qb);
			}

			if (cosHalfTheta >= 1.0)
			{
				this.w = w;
				this.x = x;
				this.y = y;
				this.z = z;

				return this;
			}

			var halfTheta = System.Math.Acos(cosHalfTheta);
			var sinHalfTheta = System.Math.Sqrt(1.0 - cosHalfTheta * cosHalfTheta);

			if (System.Math.Abs(sinHalfTheta) < 0.001)
			{
				this.w = 0.5 * (w + this.w);
				this.x = 0.5 * (x + this.x);
				this.y = 0.5 * (y + this.y);
				this.z = 0.5 * (z + this.z);

				return this;
			}

			double ratioA = System.Math.Sin((1 - t) * halfTheta) / sinHalfTheta,
			       ratioB = System.Math.Sin(t * halfTheta) / sinHalfTheta;

			this.w = (w * ratioA + this.w * ratioB);
			this.x = (x * ratioA + this.x * ratioB);
			this.y = (y * ratioA + this.y * ratioB);
			this.z = (z * ratioA + this.z * ratioB);

			return this;
		}

		public bool equals(Quaternion v)
		{
			return ((v.x == x) && (v.y == y) && (v.z == z) && (v.w == w));
		}

		public Quaternion clone()
		{
			return new Quaternion(x, y, z, w);
		}

		public static Quaternion slerp(Quaternion qa, Quaternion qb, Quaternion qm, double t)
		{
			return qm.copy(qa).slerp(qb, t);
		}
	}
}
