using WebGL;

namespace THREE
{
	public class SphereGeometry : Geometry
	{
		public double radius;
		public int widthSegments;
		public int heightSegments;

		public SphereGeometry(double radius = 50, int widthSegments = 8, int heightSegments = 6, double phiStart = 0.0,
		                      double phiLength = System.Math.PI * 2.0, double thetaStart = 0.0, double thetaLength = System.Math.PI)
		{
			this.radius = radius;

			this.widthSegments = (int)System.Math.Max(3, System.Math.Floor((double)widthSegments));
			this.heightSegments = (int)System.Math.Max(2, System.Math.Floor((double)heightSegments));

			int x;
			int y;
			var verts = new JSArray();
			var uvs = new JSArray();

			for (y = 0; y <= this.heightSegments; y++)
			{
				var verticesRow = new JSArray();
				var uvsRow = new JSArray();

				for (x = 0; x <= this.widthSegments; x++)
				{
					var u = x / (float)this.widthSegments;
					var v = y / (float)this.heightSegments;

					var vertex = new Vector3
					{
						x = -this.radius * System.Math.Cos(phiStart + u * phiLength) * System.Math.Sin(thetaStart + v * thetaLength),
						y = this.radius * System.Math.Cos(thetaStart + v * thetaLength),
						z = this.radius * System.Math.Sin(phiStart + u * phiLength) * System.Math.Sin(thetaStart + v * thetaLength)
					};

					vertices.push(vertex);

					verticesRow.push(vertices.length - 1);
					uvsRow.push(new Vector2(u, 1 - v));
				}

				verts.push(verticesRow);
				uvs.push(uvsRow);
			}

			for (y = 0; y < this.heightSegments; y++)
			{
				for (x = 0; x < this.widthSegments; x++)
				{
					var v1 = verts[y][x + 1];
					var v2 = verts[y][x];
					var v3 = verts[y + 1][x];
					var v4 = verts[y + 1][x + 1];

					var n1 = vertices[v1].clone().normalize();
					var n2 = vertices[v2].clone().normalize();
					var n3 = vertices[v3].clone().normalize();
					var n4 = vertices[v4].clone().normalize();

					var uv1 = uvs[y][x + 1].clone();
					var uv2 = uvs[y][x].clone();
					var uv3 = uvs[y + 1][x].clone();
					var uv4 = uvs[y + 1][x + 1].clone();

					if (System.Math.Abs(vertices[v1].y) == this.radius)
					{
						faces.push(new Face3(v1, v3, v4, new JSArray(n1, n3, n4)));
						faceVertexUvs[0].push(new JSArray(uv1, uv3, uv4));
					}
					else if (System.Math.Abs(vertices[v3].y) == this.radius)
					{
						faces.push(new Face3(v1, v2, v3, new JSArray(n1, n2, n3)));
						faceVertexUvs[0].push(new JSArray(uv1, uv2, uv3));
					}
					else
					{
						faces.push(new Face4(v1, v2, v3, v4, new JSArray(n1, n2, n3, n4)));
						faceVertexUvs[0].push(new JSArray(uv1, uv2, uv3, uv4));
					}
				}
			}

			computeCentroids();
			computeFaceNormals();

			boundingSphere = new Sphere(new Vector3(), radius);
		}
	}
}
