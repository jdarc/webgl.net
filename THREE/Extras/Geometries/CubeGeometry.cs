using WebGL;

namespace THREE
{
	public class CubeGeometry : Geometry
	{
		public readonly double width;
		public readonly double height;
		public readonly double depth;
		public readonly int widthSegments;
		public readonly int heightSegments;
		public readonly int depthSegments;

		public CubeGeometry(double width, double height, double depth,
		                    int widthSegments = 1, int heightSegments = 1, int depthSegments = 1)
		{
			this.width = width;
			this.height = height;
			this.depth = depth;

			this.widthSegments = widthSegments;
			this.heightSegments = heightSegments;
			this.depthSegments = depthSegments;

			var width_half = this.width / 2;
			var height_half = this.height / 2;
			var depth_half = this.depth / 2;

			buildPlane('z', 'y', - 1, - 1, this.depth, this.height, width_half, 0);
			buildPlane('z', 'y', 1, - 1, this.depth, this.height, - width_half, 1);
			buildPlane('x', 'z', 1, 1, this.width, this.depth, height_half, 2);
			buildPlane('x', 'z', 1, - 1, this.width, this.depth, - height_half, 3);
			buildPlane('x', 'y', 1, - 1, this.width, this.height, depth_half, 4);
			buildPlane('x', 'y', - 1, - 1, this.width, this.height, - depth_half, 5);

			computeCentroids();
			mergeVertices();
		}

		private void buildPlane(char u, char v, double udir, double vdir, double width, double height, double depth, int materialIndex)
		{
			var w = 'x';
			int ix;
			int iy;
			var gridX = widthSegments;
			var gridY = heightSegments;
			var width_half = width / 2;
			var height_half = height / 2;
			var offset = vertices.length;

			if ((u == 'x' && v == 'y') || (u == 'y' && v == 'x'))
			{
				w = 'z';
			}
			else if ((u == 'x' && v == 'z') || (u == 'z' && v == 'x'))
			{
				w = 'y';
				gridY = depthSegments;
			}
			else if ((u == 'z' && v == 'y') || (u == 'y' && v == 'z'))
			{
				w = 'x';
				gridX = depthSegments;
			}

			var gridX1 = gridX + 1;
			var gridY1 = gridY + 1;
			var segment_width = width / gridX;
			var segment_height = height / gridY;
			var normal = new Vector3();

			normal[w] = depth > 0 ? 1 : - 1;

			for (iy = 0; iy < gridY1; iy++)
			{
				for (ix = 0; ix < gridX1; ix++)
				{
					var vector = new Vector3();
					vector[u] = (ix * segment_width - width_half) * udir;
					vector[v] = (iy * segment_height - height_half) * vdir;
					vector[w] = depth;

					vertices.push(vector);
				}
			}

			for (iy = 0; iy < gridY; iy++)
			{
				for (ix = 0; ix < gridX; ix++)
				{
					var a = ix + gridX1 * iy;
					var b = ix + gridX1 * (iy + 1);
					var c = (ix + 1) + gridX1 * (iy + 1);
					var d = (ix + 1) + gridX1 * iy;

					var face = new Face4(a + offset, b + offset, c + offset, d + offset);
					face.normal.copy(normal);
					face.vertexNormals.push(normal.clone(), normal.clone(), normal.clone(), normal.clone());
					face.materialIndex = materialIndex;

					faces.push(face);
					faceVertexUvs[0].push(new JSArray(
					                      	new Vector2(ix / (double)gridX, 1 - iy / (double)gridY),
					                      	new Vector2(ix / (double)gridX, 1 - (iy + 1) / (double)gridY),
					                      	new Vector2((ix + 1) / (double)gridX, 1 - (iy + 1) / (double)gridY),
					                      	new Vector2((ix + 1) / (double)gridX, 1 - iy / (double)gridY)));
				}
			}
		}
	}
}
