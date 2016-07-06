using WebGL;

namespace THREE
{
	public class TorusGeometry : Geometry
	{
		public double radius;
		public double tube;
		public int radialSegments;
		public int tubularSegments;
		public double arc;

		public TorusGeometry(double radius, double tube, int radialSegments, int tubularSegments, double arc = System.Math.PI * 2)
		{
			this.radius = radius;
			this.tube = tube;
			this.radialSegments = radialSegments;
			this.tubularSegments = tubularSegments;
			this.arc = arc;

			var center = new Vector3();
			var uvs = new JSArray();
			var normals = new JSArray();

			for (var j = 0; j <= this.radialSegments; j++)
			{
				for (var i = 0; i <= this.tubularSegments; i++)
				{
					var u = i / (double)this.tubularSegments * this.arc;
					var v = j / (double)this.radialSegments * System.Math.PI * 2;

					center.x = this.radius * System.Math.Cos(u);
					center.y = this.radius * System.Math.Sin(u);

					var vertex = new Vector3();
					vertex.x = (this.radius + this.tube * System.Math.Cos(v)) * System.Math.Cos(u);
					vertex.y = (this.radius + this.tube * System.Math.Cos(v)) * System.Math.Sin(u);
					vertex.z = this.tube * System.Math.Sin(v);

					vertices.push(vertex);

					uvs.push(new Vector2(i / (double)this.tubularSegments, j / (double)this.radialSegments));
					normals.push(vertex.clone().sub(center).normalize());
				}
			}

			for (var j = 1; j <= this.radialSegments; j++)
			{
				for (var i = 1; i <= this.tubularSegments; i++)
				{
					var a = (this.tubularSegments + 1) * j + i - 1;
					var b = (this.tubularSegments + 1) * (j - 1) + i - 1;
					var c = (this.tubularSegments + 1) * (j - 1) + i;
					var d = (this.tubularSegments + 1) * j + i;

					var face = new Face4(a, b, c, d, new JSArray(normals[a], normals[b], normals[c], normals[d]));
					face.normal.add(normals[a]);
					face.normal.add(normals[b]);
					face.normal.add(normals[c]);
					face.normal.add(normals[d]);
					face.normal.normalize();

					faces.push(face);

					faceVertexUvs[0].push(new JSArray(uvs[a].clone(), uvs[b].clone(), uvs[c].clone(), uvs[d].clone()));
				}
			}

			computeCentroids();
		}
	}
}
