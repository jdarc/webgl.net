using System.Collections.Generic;
using WebGL;

namespace THREE
{
	public class PolyhedronGeometry : Geometry
	{
		private class PolyhedronVertex
		{
			public readonly Vector3 vector;
			public int index;
			public Vector2 uv;

			public PolyhedronVertex(Vector3 vector)
			{
				this.vector = vector;
			}
		}

		protected static void call(Geometry geometry, JSArray vertices, JSArray faces, double radius = 1, int detail = 0)
		{
			var midpoints = new JSArray();
			var p = geometry.vertices;

			var map = new Dictionary<Vector3, PolyhedronVertex>();

			for (var i = 0; i < vertices.length; i++)
			{
				var vertex = prepare(new Vector3(vertices[i][0], vertices[i][1], vertices[i][2]), geometry);
				map.Add(vertex.vector, vertex);
			}

			for (var i = 0; i < faces.length; i++)
			{
				make(map[p[faces[i][0]]], map[p[faces[i][1]]], map[p[faces[i][2]]], detail, geometry, midpoints);
			}

			geometry.mergeVertices();

			for (var i = 0; i < geometry.vertices.length; i++)
			{
				geometry.vertices[i].multiplyScalar(radius);
			}

			geometry.computeCentroids();
			geometry.boundingSphere = new Sphere(new Vector3(), radius);
		}

		private static void make(PolyhedronVertex v1, PolyhedronVertex v2, PolyhedronVertex v3, int detail, Geometry geometry, JSArray midpoints)
		{
			if (detail < 1)
			{
				var face = new Face3(v1.index, v2.index, v3.index, new JSArray(v1.vector.clone(), v2.vector.clone(), v3.vector.clone()));
				face.centroid.add(v1.vector).add(v2.vector).add(v3.vector).divideScalar(3);
				face.normal = face.centroid.clone().normalize();
				geometry.faces.push(face);

				var azi = azimuth(face.centroid);
				var uvs = new JSArray(correctUV(v1.uv, v1.vector, azi), correctUV(v2.uv, v2.vector, azi), correctUV(v3.uv, v3.vector, azi));
				geometry.faceVertexUvs[0].push(uvs);
			}
			else
			{
				detail -= 1;

				make(v1, midpoint(v1, v2, midpoints, geometry), midpoint(v1, v3, midpoints, geometry), detail, geometry, midpoints);
				make(midpoint(v1, v2, midpoints, geometry), v2, midpoint(v2, v3, midpoints, geometry), detail, geometry, midpoints);
				make(midpoint(v1, v3, midpoints, geometry), midpoint(v2, v3, midpoints, geometry), v3, detail, geometry, midpoints);
				make(midpoint(v1, v2, midpoints, geometry), midpoint(v2, v3, midpoints, geometry), midpoint(v1, v3, midpoints, geometry), detail, geometry, midpoints);
			}
		}

		private static PolyhedronVertex prepare(Vector3 vector, Geometry geometry)
		{
			var vertex = new PolyhedronVertex(vector.normalize().clone());
			vertex.index = geometry.vertices.push(vertex.vector) - 1;

			var u = azimuth(vector) / 2.0 / System.Math.PI + 0.5;
			var v = inclination(vector) / System.Math.PI + 0.5;
			vertex.uv = new Vector2(u, 1.0 - v);

			return vertex;
		}

		private static PolyhedronVertex midpoint(PolyhedronVertex v1, PolyhedronVertex v2, JSArray midpoints, Geometry geometry)
		{
			if (midpoints[v1.index] == null)
			{
				midpoints[v1.index] = new JSArray();
			}

			if (midpoints[v2.index] == null)
			{
				midpoints[v2.index] = new JSArray();
			}

			var mid = midpoints[v1.index][v2.index];

			if (mid == null)
			{
				midpoints[v1.index][v2.index] =
					midpoints[v2.index][v1.index] =
					mid =
					prepare(new Vector3().addVectors(v1.vector, v2.vector).divideScalar(2), geometry);
			}

			return mid;
		}

		private static double azimuth(Vector3 vector)
		{
			return System.Math.Atan2(vector.z, -vector.x);
		}

		private static double inclination(Vector3 vector)
		{
			return System.Math.Atan2(-vector.y, System.Math.Sqrt(vector.x * vector.x + vector.z * vector.z));
		}

		private static Vector2 correctUV(Vector2 uv, Vector3 vector, double azimuth)
		{
			if ((azimuth < 0) && (uv.x == 1))
			{
				uv = new Vector2(uv.x - 1, uv.y);
			}
			if ((vector.x == 0) && (vector.z == 0))
			{
				uv = new Vector2(azimuth / 2.0 / System.Math.PI + 0.5, uv.y);
			}
			return uv;
		}
	}
}
