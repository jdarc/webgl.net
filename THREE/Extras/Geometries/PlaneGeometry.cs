using WebGL;

namespace THREE
{
	public class PlaneGeometry : Geometry
	{
		public double width;
		public double height;
		public int widthSegments;
		public int heightSegments;

		public PlaneGeometry(double width, double height, int widthSegments = 1, int heightSegments = 1)
		{
			this.width = width;
			this.height = height;

			this.widthSegments = widthSegments;
			this.heightSegments = heightSegments;

			var width_half = width / 2;
			var height_half = height / 2;

			var gridX = this.widthSegments;
			var gridZ = this.heightSegments;

			var gridX1 = gridX + 1;
			var gridZ1 = gridZ + 1;

			var segment_width = this.width / gridX;
			var segment_height = this.height / gridZ;

			var normal = new Vector3(0, 0, 1);

			for (var iz = 0; iz < gridZ1; iz++)
			{
				for (var ix = 0; ix < gridX1; ix++)
				{
					var x = ix * segment_width - width_half;
					var y = iz * segment_height - height_half;

					vertices.push(new Vector3(x, - y, 0));
				}
			}

			for (var iz = 0; iz < gridZ; iz++)
			{
				for (var ix = 0; ix < gridX; ix++)
				{
					var a = ix + gridX1 * iz;
					var b = ix + gridX1 * (iz + 1);
					var c = (ix + 1) + gridX1 * (iz + 1);
					var d = (ix + 1) + gridX1 * iz;

					var face = new Face4(a, b, c, d);
					face.normal.copy(normal);
					face.vertexNormals.push(normal.clone(), normal.clone(), normal.clone(), normal.clone());

					faces.push(face);
					faceVertexUvs[0].push(new JSArray(
					                      	new Vector2(ix / (double)gridX, 1 - iz / (double)gridZ),
					                      	new Vector2(ix / (double)gridX, 1 - (iz + 1) / (double)gridZ),
					                      	new Vector2((ix + 1) / (double)gridX, 1 - (iz + 1) / (double)gridZ),
					                      	new Vector2((ix + 1) / (double)gridX, 1 - iz / (double)gridZ)
					                      	));
				}
			}

			computeCentroids();
		}
	}
}
