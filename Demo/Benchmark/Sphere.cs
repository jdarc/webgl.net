using System;
using System.Collections.Generic;

namespace Demo.Benchmark
{
    internal class Sphere
    {
        internal Vector3 center;
        internal double radius;

        internal Sphere(Vector3 center = null, double radius = 0)
        {
            this.center = center ?? new Vector3();
            this.radius = radius;
        }

        internal Sphere setFromCenterAndPoints(Vector3 center, List<Vector3> points)
        {
            double maxRadiusSq = 0;
            var il = points.Count;
            for (var i = 0; i < il; i ++)
            {
                var radiusSq = center.distanceToSquared(points[i]);
                maxRadiusSq = Math.Max(maxRadiusSq, radiusSq);
            }

            this.center = center;
            radius = Math.Sqrt(maxRadiusSq);

            return this;
        }
    }
}