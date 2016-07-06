using System;
using System.IO;
using WebGL;

namespace Demo.Benchmark
{
    internal class Loader
    {
        internal void load(string url, Action<Geometry, Material[]> callback)
        {
            var json = JSON.parse(File.ReadAllText(url));
            var geometry = new Geometry();
            const double scale = 1.0;

            var faces = json["faces"] as JSArray;
            var vertices = json["vertices"] as JSArray;

            // disregard empty arrays

            var offset = 0;
            var vlen = vertices.length;

            while (offset < vlen)
            {
                var vertex = new Vector3
                             {
                                 x = (double)(vertices[offset++]) * scale,
                                 y = (double)(vertices[offset++]) * scale,
                                 z = (double)(vertices[offset++]) * scale
                             };

                geometry.vertices.Add(vertex);
            }

            offset = 0;
            vlen = faces.length;

            while (offset < vlen)
            {
                var type = faces[offset++];
                var isQuad = type & (1 << 0);
                var hasMaterial = type & (1 << 1);

                Face face;
                if (isQuad != 0)
                {
                    face = new Face(0, 0, 0, 0) {a = faces[offset++], b = faces[offset++], c = faces[offset++], d = faces[offset++]};
                }
                else
                {
                    face = new Face(0, 0, 0) {a = faces[offset++], b = faces[offset++], c = faces[offset++]};
                }

                if (hasMaterial != 0)
                {
                    face.materialIndex = faces[offset++];
                }

                geometry.faces.Add(face);
            }

            geometry.computeCentroids();
            geometry.computeFaceNormals();

            callback(geometry, new[] {new Material()});
        }
    }
}