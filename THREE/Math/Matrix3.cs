using System;
using WebGL;

namespace THREE
{
	public class Matrix3
	{
		private static readonly Vector3 __v1 = new Vector3();
		public Float32Array elements;

		public Matrix3(double n11 = 1, double n12 = 0, double n13 = 0,
		               double n21 = 0, double n22 = 1, double n23 = 0,
		               double n31 = 0, double n32 = 0, double n33 = 1)
		{
			elements = new Float32Array(9);

			set(n11, n12, n13,
			    n21, n22, n23,
			    n31, n32, n33);
		}

		public Matrix3 set(double n11, double n12, double n13,
		                   double n21, double n22, double n23,
		                   double n31, double n32, double n33)
		{
			elements[0] = (float)n11;
			elements[1] = (float)n21;
			elements[2] = (float)n31;
			elements[3] = (float)n12;
			elements[4] = (float)n22;
			elements[5] = (float)n32;
			elements[6] = (float)n13;
			elements[7] = (float)n23;
			elements[8] = (float)n33;
			return this;
		}

		public Matrix3 identity()
		{
			set(
			    1, 0, 0,
			    0, 1, 0,
			    0, 0, 1
				);

			return this;
		}

		public Matrix3 copy(Matrix3 m)
		{
			var me = m.elements;

			set(
			    me[0], me[3], me[6],
			    me[1], me[4], me[7],
			    me[2], me[5], me[8]
				);

			return this;
		}

		public Vector3 multiplyVector3(Vector3 vector)
		{
			JSConsole.warn("DEPRECATED: Matrix3\'s .multiplyVector3() has been removed. Use vector.applyMatrix3( matrix ) instead.");
			return vector.applyMatrix3(this);
		}

		public double[] multiplyVector3Array(double[] a)
		{
			var tmp = __v1;

			for (var i = 0; i < a.Length; i += 3)
			{
				tmp.x = a[i];
				tmp.y = a[i + 1];
				tmp.z = a[i + 2];

				tmp.applyMatrix3(this);

				a[i] = tmp.x;
				a[i + 1] = tmp.y;
				a[i + 2] = tmp.z;
			}

			return a;
		}

		public Matrix3 multiplyScalar(double s)
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

			return this;
		}

		public double determinant()
		{
			double a = elements[0], b = elements[1], c = elements[2];
			double d = elements[3], e = elements[4], f = elements[5];
			double g = elements[6], h = elements[7], i = elements[8];
			return a * e * i - a * f * h - b * d * i + b * f * g + c * d * h - c * e * g;
		}

		public Matrix3 getInverse(Matrix4 matrix, bool throwOnInvertible = false)
		{
			var me = matrix.elements;
			var te = elements;

			te[0] = me[10] * me[5] - me[6] * me[9];
			te[1] = - me[10] * me[1] + me[2] * me[9];
			te[2] = me[6] * me[1] - me[2] * me[5];
			te[3] = - me[10] * me[4] + me[6] * me[8];
			te[4] = me[10] * me[0] - me[2] * me[8];
			te[5] = - me[6] * me[0] + me[2] * me[4];
			te[6] = me[9] * me[4] - me[5] * me[8];
			te[7] = - me[9] * me[0] + me[1] * me[8];
			te[8] = me[5] * me[0] - me[1] * me[4];

			var det = me[0] * te[0] + me[1] * te[3] + me[2] * te[6];

			if (det == 0)
			{
				const string msg = "Matrix3.getInverse(): can't invert matrix, determinant is 0";

				if (throwOnInvertible)
				{
                    throw new ApplicationException(msg);
				}
				JSConsole.warn(msg);

				identity();

				return this;
			}

			multiplyScalar(1.0 / det);

			return this;
		}

		public Matrix3 transpose()
		{
			var tmp = elements[1];
			elements[1] = elements[3];
			elements[3] = tmp;
			tmp = elements[2];
			elements[2] = elements[6];
			elements[6] = tmp;
			tmp = elements[5];
			elements[5] = elements[7];
			elements[7] = tmp;
			return this;
		}

		public Matrix3 transposeIntoArray(double[] r)
		{
			r[0] = elements[0];
			r[1] = elements[3];
			r[2] = elements[6];
			r[3] = elements[1];
			r[4] = elements[4];
			r[5] = elements[7];
			r[6] = elements[2];
			r[7] = elements[5];
			r[8] = elements[8];
			return this;
		}

		public Matrix3 clone()
		{
			return new Matrix3(elements[0], elements[3], elements[6],
			                   elements[1], elements[4], elements[7],
			                   elements[2], elements[5], elements[8]);
		}
	}
}
