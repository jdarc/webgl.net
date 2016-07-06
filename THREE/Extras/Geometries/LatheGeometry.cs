using WebGL;

namespace THREE
{
	public class LatheGeometry : Geometry
	{
		public LatheGeometry(JSArray points, int segments = 12, double phiStart = 0, double phiLength = 2 * System.Math.PI)
		{
			var inversePointLength = 1.0 / (points.length - 1);
			var inverseSegments = 1.0 / segments;

			for (var i = 0; i <= segments; i++)
			{
				var phi = phiStart + i * inverseSegments * phiLength;

				var c = System.Math.Cos(phi);
				var s = System.Math.Sin(phi);

				for (var j = 0; j < points.length; j++)
				{
					var pt = points[j];

					var vertex = new Vector3();
					vertex.x = c * pt.x - s * pt.y;
					vertex.y = s * pt.x + c * pt.y;
					vertex.z = pt.z;

					vertices.push(vertex);
				}
			}

			var np = points.length;

			for (var i = 0; i < segments; i++)
			{
				var jl = points.length - 1;
				for (var j = 0; j < jl; j++)
				{
					var baze = j + np * i;
					var a = baze;
					var b = baze + np;
					var c = baze + 1 + np;
					var d = baze + 1;

					faces.push(new Face4(a, b, c, d));

					var u0 = i * inverseSegments;
					var v0 = j * inversePointLength;
					var u1 = u0 + inverseSegments;
					var v1 = v0 + inversePointLength;

					faceVertexUvs[0].push(new JSArray(
					                      	new Vector2(u0, v0),
					                      	new Vector2(u1, v0),
					                      	new Vector2(u1, v1),
					                      	new Vector2(u0, v1)
					                      	));
				}
			}

			mergeVertices();
			computeCentroids();
			computeFaceNormals();
			computeVertexNormals();
		}
	}
}
