using System;
using WebGL;

namespace THREE
{
	public class Matrix4
	{
		private static readonly Vector3 __v1 = new Vector3();
		private static readonly Vector3 __v2 = new Vector3();
		private static readonly Vector3 __v3 = new Vector3();

		private static readonly Matrix4 __m1 = new Matrix4();
		private static readonly Matrix4 __m2 = new Matrix4();

		public Float32Array elements;

		public Matrix4(double n11 = 1, double n12 = 0, double n13 = 0, double n14 = 0,
		               double n21 = 0, double n22 = 1, double n23 = 0, double n24 = 0,
		               double n31 = 0, double n32 = 0, double n33 = 1, double n34 = 0,
		               double n41 = 0, double n42 = 0, double n43 = 0, double n44 = 1)
		{
			elements = new Float32Array(16);

			set(n11, n12, n13, n14,
			    n21, n22, n23, n24,
			    n31, n32, n33, n34,
			    n41, n42, n43, n44);
		}

		public Matrix4 set(double n11, double n12, double n13, double n14,
		                   double n21, double n22, double n23, double n24,
		                   double n31, double n32, double n33, double n34,
		                   double n41, double n42, double n43, double n44)
		{
			elements[0] = (float)n11;
			elements[1] = (float)n21;
			elements[2] = (float)n31;
			elements[3] = (float)n41;
			elements[4] = (float)n12;
			elements[5] = (float)n22;
			elements[6] = (float)n32;
			elements[7] = (float)n42;
			elements[8] = (float)n13;
			elements[9] = (float)n23;
			elements[10] = (float)n33;
			elements[11] = (float)n43;
			elements[12] = (float)n14;
			elements[13] = (float)n24;
			elements[14] = (float)n34;
			elements[15] = (float)n44;

			return this;
		}

		public Matrix4 identity()
		{
			return set(1, 0, 0, 0,
			           0, 1, 0, 0,
			           0, 0, 1, 0,
			           0, 0, 0, 1);
		}

		public Matrix4 copy(Matrix4 m)
		{
			return set(m.elements[0], m.elements[4], m.elements[8], m.elements[12],
			           m.elements[1], m.elements[5], m.elements[9], m.elements[13],
			           m.elements[2], m.elements[6], m.elements[10], m.elements[14],
			           m.elements[3], m.elements[7], m.elements[11], m.elements[15]);
		}

		public Matrix4 setRotationFromEuler(Vector3 v, string order = null)
		{
			var te = elements;

			double x = v.x, y = v.y, z = v.z;
			double a = System.Math.Cos(x), b = System.Math.Sin(x);
			double c = System.Math.Cos(y), d = System.Math.Sin(y);
			double e = System.Math.Cos(z), f = System.Math.Sin(z);

			if (order == null || order == "XYZ")
			{
				var ae = a * e;
				var af = a * f;
				var be = b * e;
				var bf = b * f;

				te[0] = (float)(c * e);
				te[4] = (float)(- c * f);
				te[8] = (float)d;

				te[1] = (float)(af + be * d);
				te[5] = (float)(ae - bf * d);
				te[9] = (float)(- b * c);

				te[2] = (float)(bf - ae * d);
				te[6] = (float)(be + af * d);
				te[10] = (float)(a * c);
			}
			else if (order == "YXZ")
			{
				var ce = c * e;
				var cf = c * f;
				var de = d * e;
				var df = d * f;

				te[0] = (float)(ce + df * b);
				te[4] = (float)(de * b - cf);
				te[8] = (float)(a * d);

				te[1] = (float)(a * f);
				te[5] = (float)(a * e);
				te[9] = (float)(- b);

				te[2] = (float)(cf * b - de);
				te[6] = (float)(df + ce * b);
				te[10] = (float)(a * c);
			}
			else if (order == "ZXY")
			{
				var ce = c * e;
				var cf = c * f;
				var de = d * e;
				var df = d * f;

				te[0] = (float)(ce - df * b);
				te[4] = (float)(- a * f);
				te[8] = (float)(de + cf * b);

				te[1] = (float)(cf + de * b);
				te[5] = (float)(a * e);
				te[9] = (float)(df - ce * b);

				te[2] = (float)(- a * d);
				te[6] = (float)b;
				te[10] = (float)(a * c);
			}
			else if (order == "ZYX")
			{
				var ae = a * e;
				var af = a * f;
				var be = b * e;
				var bf = b * f;

				te[0] = (float)(c * e);
				te[4] = (float)(be * d - af);
				te[8] = (float)(ae * d + bf);

				te[1] = (float)(c * f);
				te[5] = (float)(bf * d + ae);
				te[9] = (float)(af * d - be);

				te[2] = (float)(- d);
				te[6] = (float)(b * c);
				te[10] = (float)(a * c);
			}
			else if (order == "YZX")
			{
				var ac = a * c;
				var ad = a * d;
				var bc = b * c;
				var bd = b * d;

				te[0] = (float)(c * e);
				te[4] = (float)(bd - ac * f);
				te[8] = (float)(bc * f + ad);

				te[1] = (float)f;
				te[5] = (float)(a * e);
				te[9] = (float)(- b * e);

				te[2] = (float)(- d * e);
				te[6] = (float)(ad * f + bc);
				te[10] = (float)(ac - bd * f);
			}
			else if (order == "XZY")
			{
				var ac = a * c;
				var ad = a * d;
				var bc = b * c;
				var bd = b * d;

				te[0] = (float)(c * e);
				te[4] = (float)(- f);
				te[8] = (float)(d * e);

				te[1] = (float)(ac * f + bd);
				te[5] = (float)(a * e);
				te[9] = (float)(ad * f - bc);

				te[2] = (float)(bc * f - ad);
				te[6] = (float)(b * e);
				te[10] = (float)(bd * f + ac);
			}

			return this;
		}

		public Matrix4 setRotationFromQuaternion(Quaternion q)
		{
			var te = elements;

			double x = q.x, y = q.y, z = q.z, w = q.w;
			double x2 = x + x, y2 = y + y, z2 = z + z;
			double xx = x * x2, xy = x * y2, xz = x * z2;
			double yy = y * y2, yz = y * z2, zz = z * z2;
			double wx = w * x2, wy = w * y2, wz = w * z2;

			te[0] = (float)(1 - (yy + zz));
			te[4] = (float)(xy - wz);
			te[8] = (float)(xz + wy);

			te[1] = (float)(xy + wz);
			te[5] = (float)(1 - (xx + zz));
			te[9] = (float)(yz - wx);

			te[2] = (float)(xz - wy);
			te[6] = (float)(yz + wx);
			te[10] = (float)(1 - (xx + yy));

			return this;
		}

		public Matrix4 lookAt(Vector3 eye, Vector3 target, Vector3 up)
		{
			var te = elements;

			var x = __v1;
			var y = __v2;
			var z = __v3;

			z.subVectors(eye, target).normalize();

			if (z.length() == 0)
			{
				z.z = 1;
			}

			x.crossVectors(up, z).normalize();

			if (x.length() == 0)
			{
				z.x += 0.0001;
				x.crossVectors(up, z).normalize();
			}

			y.crossVectors(z, x);

			te[0] = (float)x.x;
			te[4] = (float)y.x;
			te[8] = (float)z.x;
			te[1] = (float)x.y;
			te[5] = (float)y.y;
			te[9] = (float)z.y;
			te[2] = (float)x.z;
			te[6] = (float)y.z;
			te[10] = (float)z.z;

			return this;
		}

		public Matrix4 multiply(Matrix4 m, Matrix4 n = null)
		{
			if (n != null)
			{
				JSConsole.warn("DEPRECATED: Matrix4\'s .multiply() now only accepts one argument. Use .multiplyMatrices( a, b ) instead.");
				return multiplyMatrices(m, n);
			}

			return multiplyMatrices(this, m);
		}

		public Matrix4 multiplyMatrices(Matrix4 a, Matrix4 b)
		{
			var ae = a.elements;
			var be = b.elements;
			var te = elements;

			double a11 = ae[0], a12 = ae[4], a13 = ae[8], a14 = ae[12];
			double a21 = ae[1], a22 = ae[5], a23 = ae[9], a24 = ae[13];
			double a31 = ae[2], a32 = ae[6], a33 = ae[10], a34 = ae[14];
			double a41 = ae[3], a42 = ae[7], a43 = ae[11], a44 = ae[15];

			double b11 = be[0], b12 = be[4], b13 = be[8], b14 = be[12];
			double b21 = be[1], b22 = be[5], b23 = be[9], b24 = be[13];
			double b31 = be[2], b32 = be[6], b33 = be[10], b34 = be[14];
			double b41 = be[3], b42 = be[7], b43 = be[11], b44 = be[15];

			te[0] = (float)(a11 * b11 + a12 * b21 + a13 * b31 + a14 * b41);
			te[4] = (float)(a11 * b12 + a12 * b22 + a13 * b32 + a14 * b42);
			te[8] = (float)(a11 * b13 + a12 * b23 + a13 * b33 + a14 * b43);
			te[12] = (float)(a11 * b14 + a12 * b24 + a13 * b34 + a14 * b44);

			te[1] = (float)(a21 * b11 + a22 * b21 + a23 * b31 + a24 * b41);
			te[5] = (float)(a21 * b12 + a22 * b22 + a23 * b32 + a24 * b42);
			te[9] = (float)(a21 * b13 + a22 * b23 + a23 * b33 + a24 * b43);
			te[13] = (float)(a21 * b14 + a22 * b24 + a23 * b34 + a24 * b44);

			te[2] = (float)(a31 * b11 + a32 * b21 + a33 * b31 + a34 * b41);
			te[6] = (float)(a31 * b12 + a32 * b22 + a33 * b32 + a34 * b42);
			te[10] = (float)(a31 * b13 + a32 * b23 + a33 * b33 + a34 * b43);
			te[14] = (float)(a31 * b14 + a32 * b24 + a33 * b34 + a34 * b44);

			te[3] = (float)(a41 * b11 + a42 * b21 + a43 * b31 + a44 * b41);
			te[7] = (float)(a41 * b12 + a42 * b22 + a43 * b32 + a44 * b42);
			te[11] = (float)(a41 * b13 + a42 * b23 + a43 * b33 + a44 * b43);
			te[15] = (float)(a41 * b14 + a42 * b24 + a43 * b34 + a44 * b44);

			return this;
		}

		public Matrix4 multiplyToArray(Matrix4 a, Matrix4 b, double[] r)
		{
			multiplyMatrices(a, b);

			r[0] = elements[0];
			r[1] = elements[1];
			r[2] = elements[2];
			r[3] = elements[3];
			r[4] = elements[4];
			r[5] = elements[5];
			r[6] = elements[6];
			r[7] = elements[7];
			r[8] = elements[8];
			r[9] = elements[9];
			r[10] = elements[10];
			r[11] = elements[11];
			r[12] = elements[12];
			r[13] = elements[13];
			r[14] = elements[14];
			r[15] = elements[15];

			return this;
		}

		public Matrix4 multiplyScalar(double s)
		{
			elements[0] = (float)(elements[0] * s);
			elements[1] = (float)(elements[1] * s);
			elements[2] = (float)(elements[2] * s);
			elements[3] = (float)(elements[3] * s);
			elements[4] = (float)(elements[4] * s);
			elements[5] = (float)(elements[5] * s);
			elements[6] = (float)(elements[6] * s);
			elements[7] = (float)(elements[7] * s);
			elements[8] = (float)(elements[8] * s);
			elements[9] = (float)(elements[9] * s);
			elements[10] = (float)(elements[10] * s);
			elements[11] = (float)(elements[11] * s);
			elements[12] = (float)(elements[12] * s);
			elements[13] = (float)(elements[13] * s);
			elements[14] = (float)(elements[14] * s);
			elements[15] = (float)(elements[15] * s);

			return this;
		}

		public Vector3 multiplyVector3(Vector3 vector)
		{
			JSConsole.warn("DEPRECATED: Matrix4\'s .multiplyVector3() has been removed. Use vector.applyMatrix4( matrix ) or vector.applyProjection( matrix ) instead.");
			return vector.applyProjection(this);
		}

		public Vector3 multiplyVector4(Vector3 vector)
		{
			JSConsole.warn("DEPRECATED: Matrix4\'s .multiplyVector4() has been removed. Use vector.applyMatrix4( matrix ) instead.");
			return vector.applyMatrix4(this);
		}

		public double[] multiplyVector3Array(double[] a)
		{
			var tmp = __v1;

			var il = a.Length;
			for (var i = 0; i < il; i += 3)
			{
				tmp.x = a[i];
				tmp.y = a[i + 1];
				tmp.z = a[i + 2];

				tmp.applyProjection(this);

				a[i] = tmp.x;
				a[i + 1] = tmp.y;
				a[i + 2] = tmp.z;
			}

			return a;
		}

		public Vector3 rotateAxis(Vector3 v)
		{
			var te = elements;
			double vx = v.x, vy = v.y, vz = v.z;

			v.x = vx * te[0] + vy * te[4] + vz * te[8];
			v.y = vx * te[1] + vy * te[5] + vz * te[9];
			v.z = vx * te[2] + vy * te[6] + vz * te[10];

			v.normalize();

			return v;
		}

		public Vector4 crossVector(Vector4 a)
		{
			var te = elements;
			var v = new Vector4();

			v.x = te[0] * a.x + te[4] * a.y + te[8] * a.z + te[12] * a.w;
			v.y = te[1] * a.x + te[5] * a.y + te[9] * a.z + te[13] * a.w;
			v.z = te[2] * a.x + te[6] * a.y + te[10] * a.z + te[14] * a.w;

			v.w = (a.w != 0) ? te[3] * a.x + te[7] * a.y + te[11] * a.z + te[15] * a.w : 1;

			return v;
		}

		public double determinant()
		{
			var te = elements;

			double n11 = te[0], n12 = te[4], n13 = te[8], n14 = te[12];
			double n21 = te[1], n22 = te[5], n23 = te[9], n24 = te[13];
			double n31 = te[2], n32 = te[6], n33 = te[10], n34 = te[14];
			double n41 = te[3], n42 = te[7], n43 = te[11], n44 = te[15];

			return (
			       	n41 * (
			       	      	+n14 * n23 * n32
			       	      	- n13 * n24 * n32
			       	      	- n14 * n22 * n33
			       	      	+ n12 * n24 * n33
			       	      	+ n13 * n22 * n34
			       	      	- n12 * n23 * n34
			       	      ) +
			       	n42 * (
			       	      	+n11 * n23 * n34
			       	      	- n11 * n24 * n33
			       	      	+ n14 * n21 * n33
			       	      	- n13 * n21 * n34
			       	      	+ n13 * n24 * n31
			       	      	- n14 * n23 * n31
			       	      ) +
			       	n43 * (
			       	      	+n11 * n24 * n32
			       	      	- n11 * n22 * n34
			       	      	- n14 * n21 * n32
			       	      	+ n12 * n21 * n34
			       	      	+ n14 * n22 * n31
			       	      	- n12 * n24 * n31
			       	      ) +
			       	n44 * (
			       	      	-n13 * n22 * n31
			       	      	- n11 * n23 * n32
			       	      	+ n11 * n22 * n33
			       	      	+ n13 * n21 * n32
			       	      	- n12 * n21 * n33
			       	      	+ n12 * n23 * n31
			       	      )
			       );
		}

		public Matrix4 transpose()
		{
			var te = elements;

			var tmp = te[1];
			te[1] = te[4];
			te[4] = tmp;
			tmp = te[2];
			te[2] = te[8];
			te[8] = tmp;
			tmp = te[6];
			te[6] = te[9];
			te[9] = tmp;

			tmp = te[3];
			te[3] = te[12];
			te[12] = tmp;
			tmp = te[7];
			te[7] = te[13];
			te[13] = tmp;
			tmp = te[11];
			te[11] = te[14];
			te[14] = tmp;

			return this;
		}

		public double[] flattenToArray(double[] flat)
		{
			flat[0] = elements[0];
			flat[1] = elements[1];
			flat[2] = elements[2];
			flat[3] = elements[3];
			flat[4] = elements[4];
			flat[5] = elements[5];
			flat[6] = elements[6];
			flat[7] = elements[7];
			flat[8] = elements[8];
			flat[9] = elements[9];
			flat[10] = elements[10];
			flat[11] = elements[11];
			flat[12] = elements[12];
			flat[13] = elements[13];
			flat[14] = elements[14];
			flat[15] = elements[15];

			return flat;
		}

		public dynamic flattenToArrayOffset(dynamic flat, int offset)
		{
			flat[offset] = elements[0];
			flat[offset + 1] = elements[1];
			flat[offset + 2] = elements[2];
			flat[offset + 3] = elements[3];

			flat[offset + 4] = elements[4];
			flat[offset + 5] = elements[5];
			flat[offset + 6] = elements[6];
			flat[offset + 7] = elements[7];

			flat[offset + 8] = elements[8];
			flat[offset + 9] = elements[9];
			flat[offset + 10] = elements[10];
			flat[offset + 11] = elements[11];

			flat[offset + 12] = elements[12];
			flat[offset + 13] = elements[13];
			flat[offset + 14] = elements[14];
			flat[offset + 15] = elements[15];

			return flat;
		}

		public Vector3 getPosition()
		{
			return __v1.set(elements[12], elements[13], elements[14]);
		}

		public Matrix4 setPosition(Vector3 v)
		{
			elements[12] = (float)v.x;
			elements[13] = (float)v.y;
			elements[14] = (float)v.z;

			return this;
		}

		public Vector3 getColumnX()
		{
			return __v1.set(elements[0], elements[1], elements[2]);
		}

		public Vector3 getColumnY()
		{
			return __v1.set(elements[4], elements[5], elements[6]);
		}

		public Vector3 getColumnZ()
		{
			return __v1.set(elements[8], elements[9], elements[10]);
		}

		public Matrix4 getInverse(Matrix4 m, bool throwOnInvertible = false)
		{
			var te = elements;
			var me = m.elements;

			double n11 = me[0], n12 = me[4], n13 = me[8], n14 = me[12];
			double n21 = me[1], n22 = me[5], n23 = me[9], n24 = me[13];
			double n31 = me[2], n32 = me[6], n33 = me[10], n34 = me[14];
			double n41 = me[3], n42 = me[7], n43 = me[11], n44 = me[15];

			te[0] = (float)(n23 * n34 * n42 - n24 * n33 * n42 + n24 * n32 * n43 - n22 * n34 * n43 - n23 * n32 * n44 + n22 * n33 * n44);
			te[4] = (float)(n14 * n33 * n42 - n13 * n34 * n42 - n14 * n32 * n43 + n12 * n34 * n43 + n13 * n32 * n44 - n12 * n33 * n44);
			te[8] = (float)(n13 * n24 * n42 - n14 * n23 * n42 + n14 * n22 * n43 - n12 * n24 * n43 - n13 * n22 * n44 + n12 * n23 * n44);
			te[12] = (float)(n14 * n23 * n32 - n13 * n24 * n32 - n14 * n22 * n33 + n12 * n24 * n33 + n13 * n22 * n34 - n12 * n23 * n34);
			te[1] = (float)(n24 * n33 * n41 - n23 * n34 * n41 - n24 * n31 * n43 + n21 * n34 * n43 + n23 * n31 * n44 - n21 * n33 * n44);
			te[5] = (float)(n13 * n34 * n41 - n14 * n33 * n41 + n14 * n31 * n43 - n11 * n34 * n43 - n13 * n31 * n44 + n11 * n33 * n44);
			te[9] = (float)(n14 * n23 * n41 - n13 * n24 * n41 - n14 * n21 * n43 + n11 * n24 * n43 + n13 * n21 * n44 - n11 * n23 * n44);
			te[13] = (float)(n13 * n24 * n31 - n14 * n23 * n31 + n14 * n21 * n33 - n11 * n24 * n33 - n13 * n21 * n34 + n11 * n23 * n34);
			te[2] = (float)(n22 * n34 * n41 - n24 * n32 * n41 + n24 * n31 * n42 - n21 * n34 * n42 - n22 * n31 * n44 + n21 * n32 * n44);
			te[6] = (float)(n14 * n32 * n41 - n12 * n34 * n41 - n14 * n31 * n42 + n11 * n34 * n42 + n12 * n31 * n44 - n11 * n32 * n44);
			te[10] = (float)(n12 * n24 * n41 - n14 * n22 * n41 + n14 * n21 * n42 - n11 * n24 * n42 - n12 * n21 * n44 + n11 * n22 * n44);
			te[14] = (float)(n14 * n22 * n31 - n12 * n24 * n31 - n14 * n21 * n32 + n11 * n24 * n32 + n12 * n21 * n34 - n11 * n22 * n34);
			te[3] = (float)(n23 * n32 * n41 - n22 * n33 * n41 - n23 * n31 * n42 + n21 * n33 * n42 + n22 * n31 * n43 - n21 * n32 * n43);
			te[7] = (float)(n12 * n33 * n41 - n13 * n32 * n41 + n13 * n31 * n42 - n11 * n33 * n42 - n12 * n31 * n43 + n11 * n32 * n43);
			te[11] = (float)(n13 * n22 * n41 - n12 * n23 * n41 - n13 * n21 * n42 + n11 * n23 * n42 + n12 * n21 * n43 - n11 * n22 * n43);
			te[15] = (float)(n12 * n23 * n31 - n13 * n22 * n31 + n13 * n21 * n32 - n11 * n23 * n32 - n12 * n21 * n33 + n11 * n22 * n33);

			var det = me[0] * te[0] + me[1] * te[4] + me[2] * te[8] + me[3] * te[12];

			if (det == 0)
			{
				const string msg = "Matrix4.getInverse(): can't invert matrix, determinant is 0";

				if (throwOnInvertible)
				{
                    throw new ApplicationException(msg);
				}
				JSConsole.warn(msg);

				identity();

				return this;
			}

			multiplyScalar(1 / det);

			return this;
		}

		public Matrix4 compose(Vector3 translation, Quaternion rotation, Vector3 scale)
		{
			var te = elements;
			var mRotation = __m1;
			var mScale = __m2;

			mRotation.identity();
			mRotation.setRotationFromQuaternion(rotation);

			mScale.makeScale(scale.x, scale.y, scale.z);

			multiplyMatrices(mRotation, mScale);

			te[12] = (float)translation.x;
			te[13] = (float)translation.y;
			te[14] = (float)translation.z;

			return this;
		}

		public dynamic decompose(Vector3 translation = null, Quaternion rotation = null, Vector3 scale = null)
		{
			var te = elements;

			var x = __v1;
			var y = __v2;
			var z = __v3;

			x.set(te[0], te[1], te[2]);
			y.set(te[4], te[5], te[6]);
			z.set(te[8], te[9], te[10]);

			translation = translation ?? new Vector3();
			rotation = rotation ?? new Quaternion();
			scale = scale ?? new Vector3();

			scale.x = x.length();
			scale.y = y.length();
			scale.z = z.length();

			translation.x = te[12];
			translation.y = te[13];
			translation.z = te[14];

			var matrix = __m1;

			matrix.copy(this);

			matrix.elements[0] = (float)(matrix.elements[0] / scale.x);
			matrix.elements[1] = (float)(matrix.elements[1] / scale.x);
			matrix.elements[2] = (float)(matrix.elements[2] / scale.x);
			matrix.elements[4] = (float)(matrix.elements[4] / scale.y);
			matrix.elements[5] = (float)(matrix.elements[5] / scale.y);
			matrix.elements[6] = (float)(matrix.elements[6] / scale.y);
			matrix.elements[8] = (float)(matrix.elements[8] / scale.z);
			matrix.elements[9] = (float)(matrix.elements[9] / scale.z);
			matrix.elements[10] = (float)(matrix.elements[10] / scale.z);

			rotation.setFromRotationMatrix(matrix);

			return new {translation, rotation, scale};
		}

		public Matrix4 extractPosition(Matrix4 m)
		{
			var te = elements;
			var me = m.elements;

			te[12] = me[12];
			te[13] = me[13];
			te[14] = me[14];

			return this;
		}

		public Matrix4 extractRotation(Matrix4 m)
		{
			var te = elements;
			var me = m.elements;

			var vector = __v1;

			var scaleX = 1.0 / vector.set(me[0], me[1], me[2]).length();
			var scaleY = 1.0 / vector.set(me[4], me[5], me[6]).length();
			var scaleZ = 1.0 / vector.set(me[8], me[9], me[10]).length();

			te[0] = (float)(me[0] * scaleX);
			te[1] = (float)(me[1] * scaleX);
			te[2] = (float)(me[2] * scaleX);

			te[4] = (float)(me[4] * scaleY);
			te[5] = (float)(me[5] * scaleY);
			te[6] = (float)(me[6] * scaleY);

			te[8] = (float)(me[8] * scaleZ);
			te[9] = (float)(me[9] * scaleZ);
			te[10] = (float)(me[10] * scaleZ);

			return this;
		}

		public Matrix4 translate(Vector3 v)
		{
			var te = elements;
			double x = v.x, y = v.y, z = v.z;

			te[12] = (float)(te[0] * x + te[4] * y + te[8] * z + te[12]);
			te[13] = (float)(te[1] * x + te[5] * y + te[9] * z + te[13]);
			te[14] = (float)(te[2] * x + te[6] * y + te[10] * z + te[14]);
			te[15] = (float)(te[3] * x + te[7] * y + te[11] * z + te[15]);

			return this;
		}

		public Matrix4 rotateX(double angle)
		{
			var te = elements;
			var m12 = te[4];
			var m22 = te[5];
			var m32 = te[6];
			var m42 = te[7];
			var m13 = te[8];
			var m23 = te[9];
			var m33 = te[10];
			var m43 = te[11];
			var c = System.Math.Cos(angle);
			var s = System.Math.Sin(angle);

			te[4] = (float)(c * m12 + s * m13);
			te[5] = (float)(c * m22 + s * m23);
			te[6] = (float)(c * m32 + s * m33);
			te[7] = (float)(c * m42 + s * m43);

			te[8] = (float)(c * m13 - s * m12);
			te[9] = (float)(c * m23 - s * m22);
			te[10] = (float)(c * m33 - s * m32);
			te[11] = (float)(c * m43 - s * m42);

			return this;
		}

		public Matrix4 rotateY(double angle)
		{
			var te = elements;
			var m11 = te[0];
			var m21 = te[1];
			var m31 = te[2];
			var m41 = te[3];
			var m13 = te[8];
			var m23 = te[9];
			var m33 = te[10];
			var m43 = te[11];
			var c = System.Math.Cos(angle);
			var s = System.Math.Sin(angle);

			te[0] = (float)(c * m11 - s * m13);
			te[1] = (float)(c * m21 - s * m23);
			te[2] = (float)(c * m31 - s * m33);
			te[3] = (float)(c * m41 - s * m43);

			te[8] = (float)(c * m13 + s * m11);
			te[9] = (float)(c * m23 + s * m21);
			te[10] = (float)(c * m33 + s * m31);
			te[11] = (float)(c * m43 + s * m41);

			return this;
		}

		public Matrix4 rotateZ(double angle)
		{
			var te = elements;
			var m11 = te[0];
			var m21 = te[1];
			var m31 = te[2];
			var m41 = te[3];
			var m12 = te[4];
			var m22 = te[5];
			var m32 = te[6];
			var m42 = te[7];
			var c = System.Math.Cos(angle);
			var s = System.Math.Sin(angle);

			te[0] = (float)(c * m11 + s * m12);
			te[1] = (float)(c * m21 + s * m22);
			te[2] = (float)(c * m31 + s * m32);
			te[3] = (float)(c * m41 + s * m42);

			te[4] = (float)(c * m12 - s * m11);
			te[5] = (float)(c * m22 - s * m21);
			te[6] = (float)(c * m32 - s * m31);
			te[7] = (float)(c * m42 - s * m41);

			return this;
		}

		public Matrix4 rotateByAxis(Vector3 axis, double angle)
		{
			var te = elements;

			if (axis.x == 1 && axis.y == 0 && axis.z == 0)
			{
				return rotateX(angle);
			}
			if (axis.x == 0 && axis.y == 1 && axis.z == 0)
			{
				return rotateY(angle);
			}
			if (axis.x == 0 && axis.y == 0 && axis.z == 1)
			{
				return rotateZ(angle);
			}

			double x = axis.x, y = axis.y, z = axis.z;
			var n = System.Math.Sqrt(x * x + y * y + z * z);

			x /= n;
			y /= n;
			z /= n;

			double xx = x * x, yy = y * y, zz = z * z;
			var c = System.Math.Cos(angle);
			var s = System.Math.Sin(angle);
			var oneMinusCosine = 1 - c;
			var xy = x * y * oneMinusCosine;
			var xz = x * z * oneMinusCosine;
			var yz = y * z * oneMinusCosine;
			var xs = x * s;
			var ys = y * s;
			var zs = z * s;

			var r11 = xx + (1 - xx) * c;
			var r21 = xy + zs;
			var r31 = xz - ys;
			var r12 = xy - zs;
			var r22 = yy + (1 - yy) * c;
			var r32 = yz + xs;
			var r13 = xz + ys;
			var r23 = yz - xs;
			var r33 = zz + (1 - zz) * c;

			double m11 = te[0], m21 = te[1], m31 = te[2], m41 = te[3];
			double m12 = te[4], m22 = te[5], m32 = te[6], m42 = te[7];
			double m13 = te[8], m23 = te[9], m33 = te[10], m43 = te[11];

			te[0] = (float)(r11 * m11 + r21 * m12 + r31 * m13);
			te[1] = (float)(r11 * m21 + r21 * m22 + r31 * m23);
			te[2] = (float)(r11 * m31 + r21 * m32 + r31 * m33);
			te[3] = (float)(r11 * m41 + r21 * m42 + r31 * m43);

			te[4] = (float)(r12 * m11 + r22 * m12 + r32 * m13);
			te[5] = (float)(r12 * m21 + r22 * m22 + r32 * m23);
			te[6] = (float)(r12 * m31 + r22 * m32 + r32 * m33);
			te[7] = (float)(r12 * m41 + r22 * m42 + r32 * m43);

			te[8] = (float)(r13 * m11 + r23 * m12 + r33 * m13);
			te[9] = (float)(r13 * m21 + r23 * m22 + r33 * m23);
			te[10] = (float)(r13 * m31 + r23 * m32 + r33 * m33);
			te[11] = (float)(r13 * m41 + r23 * m42 + r33 * m43);

			return this;
		}

		public Matrix4 scale(Vector3 v)
		{
			elements[0] = (float)(elements[0] * v.x);
			elements[1] = (float)(elements[1] * v.x);
			elements[2] = (float)(elements[2] * v.x);
			elements[3] = (float)(elements[3] * v.x);
			elements[4] = (float)(elements[4] * v.y);
			elements[5] = (float)(elements[5] * v.y);
			elements[6] = (float)(elements[6] * v.y);
			elements[7] = (float)(elements[7] * v.y);
			elements[8] = (float)(elements[8] * v.z);
			elements[9] = (float)(elements[9] * v.z);
			elements[10] = (float)(elements[10] * v.z);
			elements[11] = (float)(elements[11] * v.z);
			return this;
		}

		public double getMaxScaleOnAxis()
		{
			var scaleXSq = elements[0] * elements[0] + elements[1] * elements[1] + elements[2] * elements[2];
			var scaleYSq = elements[4] * elements[4] + elements[5] * elements[5] + elements[6] * elements[6];
			var scaleZSq = elements[8] * elements[8] + elements[9] * elements[9] + elements[10] * elements[10];
			return System.Math.Sqrt(System.Math.Max(scaleXSq, System.Math.Max(scaleYSq, scaleZSq)));
		}

		public Matrix4 makeTranslation(double x, double y, double z)
		{
			set(
			    1, 0, 0, x,
			    0, 1, 0, y,
			    0, 0, 1, z,
			    0, 0, 0, 1
				);

			return this;
		}

		public Matrix4 makeRotationX(double theta)
		{
			double c = System.Math.Cos(theta), s = System.Math.Sin(theta);

			set(
			    1, 0, 0, 0,
			    0, c, -s, 0,
			    0, s, c, 0,
			    0, 0, 0, 1
				);

			return this;
		}

		public Matrix4 makeRotationY(double theta)
		{
			double c = System.Math.Cos(theta), s = System.Math.Sin(theta);

			set(
			    c, 0, s, 0,
			    0, 1, 0, 0,
			    -s, 0, c, 0,
			    0, 0, 0, 1
				);

			return this;
		}

		public Matrix4 makeRotationZ(double theta)
		{
			double c = System.Math.Cos(theta), s = System.Math.Sin(theta);

			set(
			    c, -s, 0, 0,
			    s, c, 0, 0,
			    0, 0, 1, 0,
			    0, 0, 0, 1
				);

			return this;
		}

		public Matrix4 makeRotationAxis(Vector3 axis, double angle)
		{
			var c = System.Math.Cos(angle);
			var s = System.Math.Sin(angle);
			var t = 1 - c;
			double x = axis.x, y = axis.y, z = axis.z;
			double tx = t * x, ty = t * y;

			set(
			    tx * x + c, tx * y - s * z, tx * z + s * y, 0,
			    tx * y + s * z, ty * y + c, ty * z - s * x, 0,
			    tx * z - s * y, ty * z + s * x, t * z * z + c, 0,
			    0, 0, 0, 1
				);

			return this;
		}

		public Matrix4 makeScale(double x, double y, double z)
		{
			set(
			    x, 0, 0, 0,
			    0, y, 0, 0,
			    0, 0, z, 0,
			    0, 0, 0, 1
				);

			return this;
		}

		public Matrix4 makeFrustum(double left, double right, double bottom, double top, double near, double far)
		{
			var te = elements;
			var x = 2 * near / (right - left);
			var y = 2 * near / (top - bottom);

			var a = (right + left) / (right - left);
			var b = (top + bottom) / (top - bottom);
			var c = - (far + near) / (far - near);
			var d = - 2 * far * near / (far - near);

			te[0] = (float)x;
			te[4] = 0;
			te[8] = (float)a;
			te[12] = 0;
			te[1] = 0;
			te[5] = (float)y;
			te[9] = (float)b;
			te[13] = 0;
			te[2] = 0;
			te[6] = 0;
			te[10] = (float)c;
			te[14] = (float)d;
			te[3] = 0;
			te[7] = 0;
			te[11] = - 1;
			te[15] = 0;

			return this;
		}

		public Matrix4 makePerspective(double fov, double aspect, double near, double far)
		{
			var ymax = near * System.Math.Tan(Math.degToRad(fov * 0.5));
			var ymin = - ymax;
			var xmin = ymin * aspect;
			var xmax = ymax * aspect;

			return makeFrustum(xmin, xmax, ymin, ymax, near, far);
		}

		public Matrix4 makeOrthographic(double left, double right, double top, double bottom, double near, double far)
		{
			var te = elements;
			var w = right - left;
			var h = top - bottom;
			var p = far - near;

			var x = (right + left) / w;
			var y = (top + bottom) / h;
			var z = (far + near) / p;

			te[0] = (float)(2 / w);
			te[4] = 0;
			te[8] = 0;
			te[12] = (float)(-x);
			te[1] = 0;
			te[5] = (float)(2 / h);
			te[9] = 0;
			te[13] = (float)(-y);
			te[2] = 0;
			te[6] = 0;
			te[10] = (float)(-2 / p);
			te[14] = (float)(-z);
			te[3] = 0;
			te[7] = 0;
			te[11] = 0;
			te[15] = 1;

			return this;
		}

		public Matrix4 clone()
		{
			var te = elements;

			return new Matrix4(
				te[0], te[4], te[8], te[12],
				te[1], te[5], te[9], te[13],
				te[2], te[6], te[10], te[14],
				te[3], te[7], te[11], te[15]
				);
		}
	}
}
