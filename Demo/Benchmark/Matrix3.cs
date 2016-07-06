using WebGL;

namespace Demo.Benchmark
{
    internal class Matrix3
    {
        internal Float32Array elements;

        internal Matrix3(double n11 = 1, double n12 = 0, double n13 = 0,
                         double n21 = 0, double n22 = 1, double n23 = 0,
                         double n31 = 0, double n32 = 0, double n33 = 1)
        {
            elements = new Float32Array(9);

            set(n11, n12, n13,
                n21, n22, n23,
                n31, n32, n33);
        }

        internal Matrix3 set(double n11, double n12, double n13,
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

        internal Matrix3 multiplyScalar(double s)
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

        internal Matrix3 getInverse(Matrix4 matrix, bool throwOnInvertible = false)
        {
            elements[0] = matrix.elements[10] * matrix.elements[5] - matrix.elements[6] * matrix.elements[9];
            elements[1] = - matrix.elements[10] * matrix.elements[1] + matrix.elements[2] * matrix.elements[9];
            elements[2] = matrix.elements[6] * matrix.elements[1] - matrix.elements[2] * matrix.elements[5];
            elements[3] = - matrix.elements[10] * matrix.elements[4] + matrix.elements[6] * matrix.elements[8];
            elements[4] = matrix.elements[10] * matrix.elements[0] - matrix.elements[2] * matrix.elements[8];
            elements[5] = - matrix.elements[6] * matrix.elements[0] + matrix.elements[2] * matrix.elements[4];
            elements[6] = matrix.elements[9] * matrix.elements[4] - matrix.elements[5] * matrix.elements[8];
            elements[7] = - matrix.elements[9] * matrix.elements[0] + matrix.elements[1] * matrix.elements[8];
            elements[8] = matrix.elements[5] * matrix.elements[0] - matrix.elements[1] * matrix.elements[4];

            multiplyScalar(1.0 / (matrix.elements[0] * elements[0] + matrix.elements[1] * elements[3] + matrix.elements[2] * elements[6]));

            return this;
        }

        internal Matrix3 transpose()
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
    }
}