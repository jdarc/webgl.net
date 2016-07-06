using WebGL;

namespace THREE
{
	public class CylinderGeometry : Geometry
	{
		public CylinderGeometry(dynamic radiusTop = null, dynamic radiusBottom = null, dynamic height = null,
		                        dynamic radiusSegments = null, dynamic heightSegments = null, bool openEnded = false)
		{
			radiusTop = radiusTop ?? 20;
			radiusBottom = radiusBottom ?? 20;
			height = height ?? 100;

			double heightHalf = height / 2;
			double segmentsX = radiusSegments ?? 8;
			double segmentsY = heightSegments ?? 1;

			int x;
			int y;
			var vertices = new JSArray();
			var uvs = new JSArray();

			for (y = 0; y <= segmentsY; y++)
			{
				var verticesRow = new JSArray();
				var uvsRow = new JSArray();

				var v = y / segmentsY;
				var radius = v * (radiusBottom - radiusTop) + radiusTop;

				for (x = 0; x <= segmentsX; x++)
				{
					var u = x / segmentsX;

					var vertex = new Vector3
					{
						x = radius * System.Math.Sin((u * System.Math.PI * 2)),
						y = -v * height + heightHalf,
						z = radius * System.Math.Cos((u * System.Math.PI * 2))
					};

					this.vertices.push(vertex);

					verticesRow.push(this.vertices.length - 1);
					uvsRow.push(new Vector2(u, 1 - v));
				}

				vertices.push(verticesRow);
				uvs.push(uvsRow);
			}

			var tanTheta = (radiusBottom - radiusTop) / height;
			dynamic na;
			dynamic nb;

			for (x = 0; x < segmentsX; x++)
			{
				if (radiusTop != 0)
				{
					na = this.vertices[vertices[0][x]].clone();
					nb = this.vertices[vertices[0][x + 1]].clone();
				}
				else
				{
					na = this.vertices[vertices[1][x]].clone();
					nb = this.vertices[vertices[1][x + 1]].clone();
				}

				na.setY(System.Math.Sqrt(na.x * na.x + na.z * na.z) * tanTheta).normalize();
				nb.setY(System.Math.Sqrt(nb.x * nb.x + nb.z * nb.z) * tanTheta).normalize();

				for (y = 0; y < segmentsY; y++)
				{
					var v1 = vertices[y][x];
					var v2 = vertices[y + 1][x];
					var v3 = vertices[y + 1][x + 1];
					var v4 = vertices[y][x + 1];

					var n1 = na.clone();
					var n2 = na.clone();
					var n3 = nb.clone();
					var n4 = nb.clone();

					var uv1 = uvs[y][x].clone();
					var uv2 = uvs[y + 1][x].clone();
					var uv3 = uvs[y + 1][x + 1].clone();
					var uv4 = uvs[y][x + 1].clone();

					faces.push(new Face4(v1, v2, v3, v4, new JSArray(n1, n2, n3, n4)));
					faceVertexUvs[0].push(new JSArray(uv1, uv2, uv3, uv4));
				}
			}

			if (!openEnded && radiusTop > 0)
			{
				this.vertices.push(new Vector3(0, heightHalf, 0));

				for (x = 0; x < segmentsX; x++)
				{
					var v1 = vertices[0][x];
					var v2 = vertices[0][x + 1];
					var v3 = this.vertices.length - 1;

					var n1 = new Vector3(0, 1, 0);
					var n2 = new Vector3(0, 1, 0);
					var n3 = new Vector3(0, 1, 0);

					var uv1 = uvs[0][x].clone();
					var uv2 = uvs[0][x + 1].clone();
					var uv3 = new Vector2(uv2.x, 0);

					faces.push(new Face3(v1, v2, v3, new JSArray(n1, n2, n3)));
					faceVertexUvs[0].push(new JSArray(uv1, uv2, uv3));
				}
			}

			if (!openEnded && radiusBottom > 0)
			{
				this.vertices.push(new Vector3(0, - heightHalf, 0));

				for (x = 0; x < segmentsX; x++)
				{
					var v1 = vertices[y][x + 1];
					var v2 = vertices[y][x];
					var v3 = this.vertices.length - 1;

					var n1 = new Vector3(0, -1, 0);
					var n2 = new Vector3(0, -1, 0);
					var n3 = new Vector3(0, -1, 0);

					var uv1 = uvs[y][x + 1].clone();
					var uv2 = uvs[y][x].clone();
					var uv3 = new Vector2(uv2.x, 1);

					faces.push(new Face3(v1, v2, v3, new JSArray(n1, n2, n3)));
					faceVertexUvs[0].push(new JSArray(uv1, uv2, uv3));
				}
			}

			computeCentroids();
			computeFaceNormals();
		}
	}
}
