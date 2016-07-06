using System.Collections.Generic;
using System.Linq;

namespace Demo.Benchmark
{
    internal class Geometry
    {
        internal List<Vector3> vertices;
        internal List<Vector3> normals;
        internal List<Face> faces;

        internal Sphere boundingSphere;

        internal bool verticesNeedUpdate;
        internal bool normalsNeedUpdate;
        internal bool elementsNeedUpdate;

        internal Dictionary<int, GeometryGroup> geometryGroups;
        internal List<GeometryGroup> geometryGroupsList;

        internal Geometry()
        {
            vertices = new List<Vector3>();
            normals = new List<Vector3>();
            faces = new List<Face>();

            boundingSphere = null;

            verticesNeedUpdate = false;
            elementsNeedUpdate = false;
            normalsNeedUpdate = false;
        }

        internal void computeCentroids()
        {
            foreach (var face in faces)
            {
                face.centroid.set(0, 0, 0);

                face.centroid.add(vertices[face.a]);
                face.centroid.add(vertices[face.b]);
                face.centroid.add(vertices[face.c]);
                if (face.vertexCount == 4)
                {
                    face.centroid.add(vertices[face.d]);
                }
                face.centroid.divideScalar(face.vertexCount);
            }
        }

        internal void computeFaceNormals()
        {
            var cb = new Vector3();
            var ab = new Vector3();

            foreach (var face in faces)
            {
                var vA = vertices[face.a];
                var vB = vertices[face.b];
                var vC = vertices[face.c];
                cb.subVectors(vC, vB);
                ab.subVectors(vA, vB);
                cb.cross(ab).normalize();
                face.normal.copy(cb);
            }
        }

        internal void computeVertexNormals(bool areaWeighted = false)
        {
            foreach (var face in faces)
            {
                face.vertexNormals = new List<Vector3> {new Vector3(), new Vector3(), new Vector3()};
                if (face.vertexCount == 4)
                {
                    face.vertexNormals.Add(new Vector3());
                }
            }

            var tmpVertices = new List<Vector3>(vertices.Count);
            tmpVertices.AddRange(vertices.Select(t => new Vector3()));
            foreach (var face in faces)
            {
                tmpVertices[face.a].add(face.normal);
                tmpVertices[face.b].add(face.normal);
                tmpVertices[face.c].add(face.normal);
                if (face.vertexCount == 4)
                {
                    tmpVertices[face.d].add(face.normal);
                }
            }

            for (var v = 0; v < vertices.Count; v++)
            {
                tmpVertices[v].normalize();
            }

            foreach (var face in faces)
            {
                face.vertexNormals[0].copy(tmpVertices[face.a]);
                face.vertexNormals[1].copy(tmpVertices[face.b]);
                face.vertexNormals[2].copy(tmpVertices[face.c]);
                if (face.vertexCount == 4)
                {
                    face.vertexNormals[3].copy(tmpVertices[face.d]);
                }
            }
        }

        internal void computeBoundingSphere()
        {
            boundingSphere = boundingSphere ?? new Sphere();
            boundingSphere.setFromCenterAndPoints(boundingSphere.center, vertices);
        }
    }
}