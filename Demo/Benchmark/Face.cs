using System.Collections.Generic;

namespace Demo.Benchmark
{
    internal class Face
    {
        internal int a;
        internal int b;
        internal int c;
        internal int d;
        internal Vector3 normal;
        internal List<Vector3> vertexNormals;
        internal Color color;
        internal List<Color> vertexColors;
        internal int materialIndex;
        internal Vector3 centroid;
        internal int vertexCount;

        internal Face(int a, int b, int c, dynamic normal = null, dynamic color = null, int materialIndex = 0)
        {
            this.a = a;
            this.b = b;
            this.c = c;

            this.normal = normal is Vector3 ? normal : new Vector3();
            vertexNormals = normal is List<Vector3> ? normal : new List<Vector3>();

            this.color = color is Color ? color : new Color();
            vertexColors = color is List<Color> ? color : new List<Color>();

            this.materialIndex = materialIndex;

            centroid = new Vector3();
            vertexCount = 3;
        }

        internal Face(int a, int b, int c, int d, dynamic normal = null, dynamic color = null, int materialIndex = 0)
        {
            this.a = a;
            this.b = b;
            this.c = c;
            this.d = d;

            this.normal = normal is Vector3 ? normal : new Vector3();
            vertexNormals = normal is List<Vector3> ? normal : new List<Vector3>();

            this.color = color is Color ? color : new Color();
            vertexColors = color is List<Color> ? color : new List<Color>();

            this.materialIndex = materialIndex;

            centroid = new Vector3();

            vertexCount = 4;
        }
    }
}