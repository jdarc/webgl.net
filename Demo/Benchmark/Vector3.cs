using System;

namespace Demo.Benchmark
{
    internal class Vector3
    {
        internal double x;
        internal double y;
        internal double z;

        internal Vector3(double x = 0.0, double y = 0.0, double z = 0.0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }

        internal Vector3 set(double x, double y, double z)
        {
            this.x = x;
            this.y = y;
            this.z = z;

            return this;
        }

        internal Vector3 copy(Vector3 v)
        {
            x = v.x;
            y = v.y;
            z = v.z;

            return this;
        }

        internal Vector3 add(Vector3 v)
        {
            x += v.x;
            y += v.y;
            z += v.z;

            return this;
        }

        internal Vector3 subVectors(Vector3 a, Vector3 b)
        {
            x = a.x - b.x;
            y = a.y - b.y;
            z = a.z - b.z;

            return this;
        }

        internal Vector3 multiplyScalar(double s)
        {
            x *= s;
            y *= s;
            z *= s;

            return this;
        }

        internal Vector3 divideScalar(double s)
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

        internal double dot(Vector3 v)
        {
            return x * v.x + y * v.y + z * v.z;
        }

        internal double length()
        {
            return Math.Sqrt(x * x + y * y + z * z);
        }

        internal Vector3 normalize()
        {
            return divideScalar(length());
        }

        internal Vector3 cross(Vector3 v)
        {
            double x = this.x, y = this.y, z = this.z;

            this.x = y * v.z - z * v.y;
            this.y = z * v.x - x * v.z;
            this.z = x * v.y - y * v.x;

            return this;
        }

        internal Vector3 crossVectors(Vector3 a, Vector3 b)
        {
            x = a.y * b.z - a.z * b.y;
            y = a.z * b.x - a.x * b.z;
            z = a.x * b.y - a.y * b.x;

            return this;
        }

        internal double distanceToSquared(Vector3 v)
        {
            var dx = x - v.x;
            var dy = y - v.y;
            var dz = z - v.z;

            return dx * dx + dy * dy + dz * dz;
        }
    }
}