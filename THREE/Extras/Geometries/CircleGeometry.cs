using WebGL;

namespace THREE
{
	public class CircleGeometry : Geometry
	{
		public CircleGeometry(double radius = 50, int segments = 8, double thetaStart = 0, double thetaLength = System.Math.PI * 2)
		{
			segments = (int)System.Math.Max(3, segments);

			int i;
			var uvs = new JSArray();
			var center = new Vector3();
			var centerUV = new Vector2(0.5, 0.5);

			vertices.push(center);
			uvs.push(centerUV);

			for (i = 0; i <= segments; i++)
			{
				var vertex = new Vector3();

				vertex.x = radius * System.Math.Cos(thetaStart + i / (double)segments * thetaLength);
				vertex.y = radius * System.Math.Sin(thetaStart + i / (double)segments * thetaLength);

				vertices.push(vertex);
				uvs.push(new Vector2((vertex.x / radius + 1) / 2, - (vertex.y / radius + 1) / 2 + 1));
			}

			var n = new Vector3(0, 0, -1);

			for (i = 1; i <= segments; i++)
			{
				var v1 = i;
				var v2 = i + 1;

				faces.push(new Face3(v1, v2, 0, new JSArray(n, n, n)));
				faceVertexUvs[0].push(new JSArray(uvs[i], uvs[i + 1], centerUV));
			}

			computeCentroids();
			computeFaceNormals();

			boundingSphere = new Sphere(new Vector3(), radius);
		}
	}
}
