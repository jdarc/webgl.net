namespace Demo.Benchmark
{
    internal class Mesh : Object3D
    {
        internal Mesh(Geometry geometry, Material material)
        {
            this.geometry = geometry;
            this.material = material;

            if (this.geometry != null && this.geometry.boundingSphere == null)
            {
                this.geometry.computeBoundingSphere();
            }
        }
    }
}