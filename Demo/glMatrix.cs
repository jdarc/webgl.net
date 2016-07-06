using System;
using WebGL;

namespace Demo
{
    public static class mat3
    {
        public static Float32Array create(float[] mat = null)
        {
            var dest = new Float32Array(9);

            if (mat != null)
            {
                dest[0] = mat[0];
                dest[1] = mat[1];
                dest[2] = mat[2];
                dest[3] = mat[3];
                dest[4] = mat[4];
                dest[5] = mat[5];
                dest[6] = mat[6];
                dest[7] = mat[7];
                dest[8] = mat[8];
            }

            return dest;
        }

        public static Float32Array transpose(Float32Array mat, Float32Array dest = null)
        {
            // If we are transposing ourselves we can skip a few steps but have to cache some values
            if (dest == null || mat == dest)
            {
                var a01 = mat[1];
                var a02 = mat[2];
                var a12 = mat[5];

                mat[1] = mat[3];
                mat[2] = mat[6];
                mat[3] = a01;
                mat[5] = mat[7];
                mat[6] = a02;
                mat[7] = a12;
                return mat;
            }

            dest[0] = mat[0];
            dest[1] = mat[3];
            dest[2] = mat[6];
            dest[3] = mat[1];
            dest[4] = mat[4];
            dest[5] = mat[7];
            dest[6] = mat[2];
            dest[7] = mat[5];
            dest[8] = mat[8];
            return dest;
        }
    }

    public static class mat4
    {
        public static Float32Array create(float[] mat = null)
        {
            var dest = new Float32Array(16);

            if (mat != null)
            {
                dest[0] = mat[0];
                dest[1] = mat[1];
                dest[2] = mat[2];
                dest[3] = mat[3];
                dest[4] = mat[4];
                dest[5] = mat[5];
                dest[6] = mat[6];
                dest[7] = mat[7];
                dest[8] = mat[8];
                dest[9] = mat[9];
                dest[10] = mat[10];
                dest[11] = mat[11];
                dest[12] = mat[12];
                dest[13] = mat[13];
                dest[14] = mat[14];
                dest[15] = mat[15];
            }

            return dest;
        }

        public static Float32Array perspective(float fovy, float aspect, float near, float far, Float32Array dest)
        {
            var top = (float)(near * Math.Tan(fovy * Math.PI / 360.0));
            var right = top * aspect;
            return frustum(-right, right, -top, top, near, far, dest);
        }

        private static Float32Array frustum(float left, float right, float bottom, float top, float near, float far, Float32Array dest = null)
        {
            if (dest == null)
            {
                dest = create();
            }
            var rl = (right - left);
            var tb = (top - bottom);
            var fn = (far - near);
            dest[0] = (near * 2) / rl;
            dest[1] = 0;
            dest[2] = 0;
            dest[3] = 0;
            dest[4] = 0;
            dest[5] = (near * 2) / tb;
            dest[6] = 0;
            dest[7] = 0;
            dest[8] = (right + left) / rl;
            dest[9] = (top + bottom) / tb;
            dest[10] = -(far + near) / fn;
            dest[11] = -1;
            dest[12] = 0;
            dest[13] = 0;
            dest[14] = -(far * near * 2) / fn;
            dest[15] = 0;
            return dest;
        }

        public static Float32Array identity(Float32Array dest)
        {
            dest[0] = 1;
            dest[1] = 0;
            dest[2] = 0;
            dest[3] = 0;
            dest[4] = 0;
            dest[5] = 1;
            dest[6] = 0;
            dest[7] = 0;
            dest[8] = 0;
            dest[9] = 0;
            dest[10] = 1;
            dest[11] = 0;
            dest[12] = 0;
            dest[13] = 0;
            dest[14] = 0;
            dest[15] = 1;
            return dest;
        }

        public static Float32Array translate(Float32Array mat, float[] vec, Float32Array dest = null)
        {
            float x = vec[0], y = vec[1], z = vec[2];

            if (dest == null || mat == dest)
            {
                mat[12] = mat[0] * x + mat[4] * y + mat[8] * z + mat[12];
                mat[13] = mat[1] * x + mat[5] * y + mat[9] * z + mat[13];
                mat[14] = mat[2] * x + mat[6] * y + mat[10] * z + mat[14];
                mat[15] = mat[3] * x + mat[7] * y + mat[11] * z + mat[15];
                return mat;
            }

            float a00 = mat[0], a01 = mat[1], a02 = mat[2], a03 = mat[3];
            float a10 = mat[4], a11 = mat[5], a12 = mat[6], a13 = mat[7];
            float a20 = mat[8], a21 = mat[9], a22 = mat[10], a23 = mat[11];

            dest[0] = a00;
            dest[1] = a01;
            dest[2] = a02;
            dest[3] = a03;
            dest[4] = a10;
            dest[5] = a11;
            dest[6] = a12;
            dest[7] = a13;
            dest[8] = a20;
            dest[9] = a21;
            dest[10] = a22;
            dest[11] = a23;

            dest[12] = a00 * x + a10 * y + a20 * z + mat[12];
            dest[13] = a01 * x + a11 * y + a21 * z + mat[13];
            dest[14] = a02 * x + a12 * y + a22 * z + mat[14];
            dest[15] = a03 * x + a13 * y + a23 * z + mat[15];
            return dest;
        }

        public static Float32Array rotate(Float32Array mat, float angle, float[] axis, Float32Array dest = null)
        {
            float x = axis[0], y = axis[1], z = axis[2];
            var len = (float)Math.Sqrt(x * x + y * y + z * z);
            if (len == 0)
            {
                return null;
            }
            if (len != 1)
            {
                len = 1 / len;
                x *= len;
                y *= len;
                z *= len;
            }

            var s = (float)Math.Sin(angle);
            var c = (float)Math.Cos(angle);
            var t = 1 - c;

            // Cache the matrix values (makes for huge speed increases!)
            float a00 = mat[0], a01 = mat[1], a02 = mat[2], a03 = mat[3];
            float a10 = mat[4], a11 = mat[5], a12 = mat[6], a13 = mat[7];
            float a20 = mat[8], a21 = mat[9], a22 = mat[10], a23 = mat[11];

            // Construct the elements of the rotation matrix
            float b00 = x * x * t + c, b01 = y * x * t + z * s, b02 = z * x * t - y * s;
            float b10 = x * y * t - z * s, b11 = y * y * t + c, b12 = z * y * t + x * s;
            float b20 = x * z * t + y * s, b21 = y * z * t - x * s, b22 = z * z * t + c;

            if (dest == null)
            {
                dest = mat;
            }
            else if (mat != dest)
            {
                // If the source and destination differ, copy the unchanged last row
                dest[12] = mat[12];
                dest[13] = mat[13];
                dest[14] = mat[14];
                dest[15] = mat[15];
            }

            // Perform rotation-specific matrix multiplication
            dest[0] = a00 * b00 + a10 * b01 + a20 * b02;
            dest[1] = a01 * b00 + a11 * b01 + a21 * b02;
            dest[2] = a02 * b00 + a12 * b01 + a22 * b02;
            dest[3] = a03 * b00 + a13 * b01 + a23 * b02;

            dest[4] = a00 * b10 + a10 * b11 + a20 * b12;
            dest[5] = a01 * b10 + a11 * b11 + a21 * b12;
            dest[6] = a02 * b10 + a12 * b11 + a22 * b12;
            dest[7] = a03 * b10 + a13 * b11 + a23 * b12;

            dest[8] = a00 * b20 + a10 * b21 + a20 * b22;
            dest[9] = a01 * b20 + a11 * b21 + a21 * b22;
            dest[10] = a02 * b20 + a12 * b21 + a22 * b22;
            dest[11] = a03 * b20 + a13 * b21 + a23 * b22;
            return dest;
        }

        public static Float32Array set(Float32Array mat, Float32Array dest)
        {
            dest[0] = mat[0];
            dest[1] = mat[1];
            dest[2] = mat[2];
            dest[3] = mat[3];
            dest[4] = mat[4];
            dest[5] = mat[5];
            dest[6] = mat[6];
            dest[7] = mat[7];
            dest[8] = mat[8];
            dest[9] = mat[9];
            dest[10] = mat[10];
            dest[11] = mat[11];
            dest[12] = mat[12];
            dest[13] = mat[13];
            dest[14] = mat[14];
            dest[15] = mat[15];
            return dest;
        }

        public static Float32Array toInverseMat3(Float32Array mat, Float32Array dest)
        {
            // Cache the matrix values (makes for huge speed increases!)
            var a00 = mat[0];
            var a01 = mat[1];
            var a02 = mat[2];
            var a10 = mat[4];
            var a11 = mat[5];
            var a12 = mat[6];
            var a20 = mat[8];
            var a21 = mat[9];
            var a22 = mat[10];

            var b01 = a22 * a11 - a12 * a21;
            var b11 = -a22 * a10 + a12 * a20;
            var b21 = a21 * a10 - a11 * a20;

            var d = a00 * b01 + a01 * b11 + a02 * b21;
            if (d == 0)
            {
                return null;
            }
            var id = 1 / d;

            if (dest == null)
            {
                dest = mat3.create();
            }

            dest[0] = b01 * id;
            dest[1] = (-a22 * a01 + a02 * a21) * id;
            dest[2] = (a12 * a01 - a02 * a11) * id;
            dest[3] = b11 * id;
            dest[4] = (a22 * a00 - a02 * a20) * id;
            dest[5] = (-a12 * a00 + a02 * a10) * id;
            dest[6] = b21 * id;
            dest[7] = (-a21 * a00 + a01 * a20) * id;
            dest[8] = (a11 * a00 - a01 * a10) * id;

            return dest;
        }
    }
}