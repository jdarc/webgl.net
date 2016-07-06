namespace Demo.Benchmark
{
    internal class Frustum
    {
        internal readonly Plane[] planes = new[] {new Plane(), new Plane(), new Plane(), new Plane(), new Plane(), new Plane()};

        internal Frustum setFromMatrix(Matrix4 m)
        {
            double me0 = m.elements[0], me1 = m.elements[1], me2 = m.elements[2], me3 = m.elements[3];
            double me4 = m.elements[4], me5 = m.elements[5], me6 = m.elements[6], me7 = m.elements[7];
            double me8 = m.elements[8], me9 = m.elements[9], me10 = m.elements[10], me11 = m.elements[11];
            double me12 = m.elements[12], me13 = m.elements[13], me14 = m.elements[14], me15 = m.elements[15];

            planes[0].setComponents(me3 - me0, me7 - me4, me11 - me8, me15 - me12).normalize();
            planes[1].setComponents(me3 + me0, me7 + me4, me11 + me8, me15 + me12).normalize();
            planes[2].setComponents(me3 + me1, me7 + me5, me11 + me9, me15 + me13).normalize();
            planes[3].setComponents(me3 - me1, me7 - me5, me11 - me9, me15 - me13).normalize();
            planes[4].setComponents(me3 - me2, me7 - me6, me11 - me10, me15 - me14).normalize();
            planes[5].setComponents(me3 + me2, me7 + me6, me11 + me10, me15 + me14).normalize();

            return this;
        }

        internal bool intersectsObject(Object3D obj)
        {
            var center = obj.matrixWorld.getPosition();
            var negRadius = -obj.geometry.boundingSphere.radius * obj.matrixWorld.getMaxScaleOnAxis();
            for (var i = 0; i < 6; i++)
            {
                if (planes[i].distanceToPoint(center) < negRadius)
                {
                    return false;
                }
            }
            return true;
        }
    }
}