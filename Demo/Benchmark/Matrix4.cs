using WebGL;
using Math = System.Math;

namespace Demo.Benchmark
{
    internal class Matrix4
    {
        private static readonly Vector3 __v1 = new Vector3();
        private static readonly Vector3 __v2 = new Vector3();
        private static readonly Vector3 __v3 = new Vector3();

        internal Float32Array elements;

        internal Matrix4(double n11 = 1, double n12 = 0, double n13 = 0, double n14 = 0,
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

        internal Matrix4 set(double n11, double n12, double n13, double n14,
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

        internal Matrix4 copy(Matrix4 m)
        {
            return set(m.elements[0], m.elements[4], m.elements[8], m.elements[12],
                       m.elements[1], m.elements[5], m.elements[9], m.elements[13],
                       m.elements[2], m.elements[6], m.elements[10], m.elements[14],
                       m.elements[3], m.elements[7], m.elements[11], m.elements[15]);
        }

        internal Matrix4 setRotationFromEuler(Vector3 v)
        {
            double x = v.x, y = v.y, z = v.z;
            double a = Math.Cos(x), b = Math.Sin(x);
            double c = Math.Cos(y), d = Math.Sin(y);
            double e = Math.Cos(z), f = Math.Sin(z);

            var ae = a * e;
            var af = a * f;
            var be = b * e;
            var bf = b * f;

            elements[0] = (float)(c * e);
            elements[4] = (float)(-c * f);
            elements[8] = (float)d;

            elements[1] = (float)(af + be * d);
            elements[5] = (float)(ae - bf * d);
            elements[9] = (float)(-b * c);

            elements[2] = (float)(bf - ae * d);
            elements[6] = (float)(be + af * d);
            elements[10] = (float)(a * c);

            return this;
        }

        internal Matrix4 lookAt(Vector3 eye, Vector3 target, Vector3 up)
        {
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

            elements[0] = (float)x.x;
            elements[4] = (float)y.x;
            elements[8] = (float)z.x;
            elements[1] = (float)x.y;
            elements[5] = (float)y.y;
            elements[9] = (float)z.y;
            elements[2] = (float)x.z;
            elements[6] = (float)y.z;
            elements[10] = (float)z.z;

            return this;
        }

        internal Matrix4 multiplyMatrices(Matrix4 a, Matrix4 b)
        {
            double a11 = a.elements[0], a12 = a.elements[4], a13 = a.elements[8], a14 = a.elements[12];
            double a21 = a.elements[1], a22 = a.elements[5], a23 = a.elements[9], a24 = a.elements[13];
            double a31 = a.elements[2], a32 = a.elements[6], a33 = a.elements[10], a34 = a.elements[14];
            double a41 = a.elements[3], a42 = a.elements[7], a43 = a.elements[11], a44 = a.elements[15];

            double b11 = b.elements[0], b12 = b.elements[4], b13 = b.elements[8], b14 = b.elements[12];
            double b21 = b.elements[1], b22 = b.elements[5], b23 = b.elements[9], b24 = b.elements[13];
            double b31 = b.elements[2], b32 = b.elements[6], b33 = b.elements[10], b34 = b.elements[14];
            double b41 = b.elements[3], b42 = b.elements[7], b43 = b.elements[11], b44 = b.elements[15];

            elements[0] = (float)(a11 * b11 + a12 * b21 + a13 * b31 + a14 * b41);
            elements[4] = (float)(a11 * b12 + a12 * b22 + a13 * b32 + a14 * b42);
            elements[8] = (float)(a11 * b13 + a12 * b23 + a13 * b33 + a14 * b43);
            elements[12] = (float)(a11 * b14 + a12 * b24 + a13 * b34 + a14 * b44);

            elements[1] = (float)(a21 * b11 + a22 * b21 + a23 * b31 + a24 * b41);
            elements[5] = (float)(a21 * b12 + a22 * b22 + a23 * b32 + a24 * b42);
            elements[9] = (float)(a21 * b13 + a22 * b23 + a23 * b33 + a24 * b43);
            elements[13] = (float)(a21 * b14 + a22 * b24 + a23 * b34 + a24 * b44);

            elements[2] = (float)(a31 * b11 + a32 * b21 + a33 * b31 + a34 * b41);
            elements[6] = (float)(a31 * b12 + a32 * b22 + a33 * b32 + a34 * b42);
            elements[10] = (float)(a31 * b13 + a32 * b23 + a33 * b33 + a34 * b43);
            elements[14] = (float)(a31 * b14 + a32 * b24 + a33 * b34 + a34 * b44);

            elements[3] = (float)(a41 * b11 + a42 * b21 + a43 * b31 + a44 * b41);
            elements[7] = (float)(a41 * b12 + a42 * b22 + a43 * b32 + a44 * b42);
            elements[11] = (float)(a41 * b13 + a42 * b23 + a43 * b33 + a44 * b43);
            elements[15] = (float)(a41 * b14 + a42 * b24 + a43 * b34 + a44 * b44);

            return this;
        }

        internal Matrix4 multiplyScalar(double s)
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

        internal Vector3 getPosition()
        {
            return __v1.set(elements[12], elements[13], elements[14]);
        }

        internal Matrix4 setPosition(Vector3 v)
        {
            elements[12] = (float)v.x;
            elements[13] = (float)v.y;
            elements[14] = (float)v.z;

            return this;
        }

        internal Matrix4 getInverse(Matrix4 m, bool throwOnInvertible = false)
        {
            double n11 = m.elements[0], n12 = m.elements[4], n13 = m.elements[8], n14 = m.elements[12];
            double n21 = m.elements[1], n22 = m.elements[5], n23 = m.elements[9], n24 = m.elements[13];
            double n31 = m.elements[2], n32 = m.elements[6], n33 = m.elements[10], n34 = m.elements[14];
            double n41 = m.elements[3], n42 = m.elements[7], n43 = m.elements[11], n44 = m.elements[15];

            elements[0] = (float)(n23 * n34 * n42 - n24 * n33 * n42 + n24 * n32 * n43 - n22 * n34 * n43 - n23 * n32 * n44 + n22 * n33 * n44);
            elements[4] = (float)(n14 * n33 * n42 - n13 * n34 * n42 - n14 * n32 * n43 + n12 * n34 * n43 + n13 * n32 * n44 - n12 * n33 * n44);
            elements[8] = (float)(n13 * n24 * n42 - n14 * n23 * n42 + n14 * n22 * n43 - n12 * n24 * n43 - n13 * n22 * n44 + n12 * n23 * n44);
            elements[12] = (float)(n14 * n23 * n32 - n13 * n24 * n32 - n14 * n22 * n33 + n12 * n24 * n33 + n13 * n22 * n34 - n12 * n23 * n34);
            elements[1] = (float)(n24 * n33 * n41 - n23 * n34 * n41 - n24 * n31 * n43 + n21 * n34 * n43 + n23 * n31 * n44 - n21 * n33 * n44);
            elements[5] = (float)(n13 * n34 * n41 - n14 * n33 * n41 + n14 * n31 * n43 - n11 * n34 * n43 - n13 * n31 * n44 + n11 * n33 * n44);
            elements[9] = (float)(n14 * n23 * n41 - n13 * n24 * n41 - n14 * n21 * n43 + n11 * n24 * n43 + n13 * n21 * n44 - n11 * n23 * n44);
            elements[13] = (float)(n13 * n24 * n31 - n14 * n23 * n31 + n14 * n21 * n33 - n11 * n24 * n33 - n13 * n21 * n34 + n11 * n23 * n34);
            elements[2] = (float)(n22 * n34 * n41 - n24 * n32 * n41 + n24 * n31 * n42 - n21 * n34 * n42 - n22 * n31 * n44 + n21 * n32 * n44);
            elements[6] = (float)(n14 * n32 * n41 - n12 * n34 * n41 - n14 * n31 * n42 + n11 * n34 * n42 + n12 * n31 * n44 - n11 * n32 * n44);
            elements[10] = (float)(n12 * n24 * n41 - n14 * n22 * n41 + n14 * n21 * n42 - n11 * n24 * n42 - n12 * n21 * n44 + n11 * n22 * n44);
            elements[14] = (float)(n14 * n22 * n31 - n12 * n24 * n31 - n14 * n21 * n32 + n11 * n24 * n32 + n12 * n21 * n34 - n11 * n22 * n34);
            elements[3] = (float)(n23 * n32 * n41 - n22 * n33 * n41 - n23 * n31 * n42 + n21 * n33 * n42 + n22 * n31 * n43 - n21 * n32 * n43);
            elements[7] = (float)(n12 * n33 * n41 - n13 * n32 * n41 + n13 * n31 * n42 - n11 * n33 * n42 - n12 * n31 * n43 + n11 * n32 * n43);
            elements[11] = (float)(n13 * n22 * n41 - n12 * n23 * n41 - n13 * n21 * n42 + n11 * n23 * n42 + n12 * n21 * n43 - n11 * n22 * n43);
            elements[15] = (float)(n12 * n23 * n31 - n13 * n22 * n31 + n13 * n21 * n32 - n11 * n23 * n32 - n12 * n21 * n33 + n11 * n22 * n33);

            multiplyScalar(1.0 / (m.elements[0] * elements[0] + m.elements[1] * elements[4] + m.elements[2] * elements[8] + m.elements[3] * elements[12]));

            return this;
        }

        internal Matrix4 scale(Vector3 v)
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

        internal double getMaxScaleOnAxis()
        {
            var scaleXSq = elements[0] * elements[0] + elements[1] * elements[1] + elements[2] * elements[2];
            var scaleYSq = elements[4] * elements[4] + elements[5] * elements[5] + elements[6] * elements[6];
            var scaleZSq = elements[8] * elements[8] + elements[9] * elements[9] + elements[10] * elements[10];
            return Math.Sqrt(Math.Max(scaleXSq, Math.Max(scaleYSq, scaleZSq)));
        }

        internal Matrix4 makeFrustum(double left, double right, double bottom, double top, double near, double far)
        {
            var x = 2 * near / (right - left);
            var y = 2 * near / (top - bottom);

            var a = (right + left) / (right - left);
            var b = (top + bottom) / (top - bottom);
            var c = - (far + near) / (far - near);
            var d = - 2 * far * near / (far - near);

            elements[0] = (float)x;
            elements[4] = 0;
            elements[8] = (float)a;
            elements[12] = 0;
            elements[1] = 0;
            elements[5] = (float)y;
            elements[9] = (float)b;
            elements[13] = 0;
            elements[2] = 0;
            elements[6] = 0;
            elements[10] = (float)c;
            elements[14] = (float)d;
            elements[3] = 0;
            elements[7] = 0;
            elements[11] = - 1;
            elements[15] = 0;

            return this;
        }

        internal Matrix4 makePerspective(double fov, double aspect, double near, double far)
        {
            var ymax = near * Math.Tan(fov * 0.5 * (Math.PI / 180.0));
            var ymin = - ymax;
            var xmin = ymin * aspect;
            var xmax = ymax * aspect;

            return makeFrustum(xmin, xmax, ymin, ymax, near, far);
        }
    }
}