namespace Demo.Benchmark
{
    internal class Plane
    {
        internal Vector3 normal;
        internal double constant;

        internal Plane(Vector3 normal = null, double constant = 0)
        {
            this.normal = normal ?? new Vector3(1, 0, 0);
            this.constant = constant;
        }

        internal Plane setComponents(double x, double y, double z, double w)
        {
            normal.set(x, y, z);
            constant = w;

            return this;
        }

        internal Plane normalize()
        {
            var inverseNormalLength = 1.0 / normal.length();
            normal.multiplyScalar(inverseNormalLength);
            constant *= inverseNormalLength;

            return this;
        }

        internal double distanceToPoint(Vector3 point)
        {
            return normal.dot(point) + constant;
        }
    }
}