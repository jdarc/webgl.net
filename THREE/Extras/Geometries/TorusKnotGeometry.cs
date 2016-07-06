using System;
using WebGL;

namespace THREE
{
	public class TorusKnotGeometry : Geometry
	{
		public double radius;
		public double tube;
		public int radialSegments;
		public int tubularSegments;
		public double p;
		public double q;
		public double heightScale;
		public JSArray grid;

		private readonly Func<double, double, double, double, double, double, Vector3> getPos = (u, v, in_q, in_p, radius, heightScale) =>
		{
			var cu = System.Math.Cos(u);
			var cv = System.Math.Cos(v);
			var su = System.Math.Sin(u);
			var quOverP = in_q / in_p * u;
			var cs = System.Math.Cos(quOverP);

			var tx = radius * (2 + cs) * 0.5 * cu;
			var ty = radius * (2 + cs) * su * 0.5;
			var tz = heightScale * radius * System.Math.Sin(quOverP) * 0.5;

			return new Vector3(tx, ty, tz);
		};

		public TorusKnotGeometry(double radius = 100.0, double tube = 40.0, int radialSegments = 64,
		                         int tubularSegments = 8, double p = 2.0, double q = 3.0, double heightScale = 1.0)
		{
			Func<double, double, double, int> vert = (x, y, z) => vertices.push(new Vector3(x, y, z)) - 1;

			this.radius = radius;
			this.tube = tube;
			this.radialSegments = radialSegments;
			this.tubularSegments = tubularSegments;
			this.p = p;
			this.q = q;
			this.heightScale = heightScale;
			grid = new JSArray(this.radialSegments);

			var tang = new Vector3();
			var n = new Vector3();
			var bitan = new Vector3();

			for (var i = 0; i < this.radialSegments;++ i)
			{
				grid[i] = new JSArray(this.tubularSegments);

				for (var j = 0; j < this.tubularSegments;++ j)
				{
					var u = i / (double)this.radialSegments * 2.0 * this.p * System.Math.PI;
					var v = j / (double)this.tubularSegments * 2.0 * System.Math.PI;
					var p1 = getPos(u, v, this.q, this.p, this.radius, this.heightScale);
					var p2 = getPos(u + 0.01, v, this.q, this.p, this.radius, this.heightScale);

					tang.subVectors(p2, p1);
					n.addVectors(p2, p1);

					bitan.crossVectors(tang, n);
					n.crossVectors(bitan, tang);
					bitan.normalize();
					n.normalize();

					var cx = - this.tube * System.Math.Cos(v);
					var cy = this.tube * System.Math.Sin(v);

					p1.x += cx * n.x + cy * bitan.x;
					p1.y += cx * n.y + cy * bitan.y;
					p1.z += cx * n.z + cy * bitan.z;

					grid[i][j] = vert(p1.x, p1.y, p1.z);
				}
			}

			for (var i = 0; i < this.radialSegments;++ i)
			{
				for (var j = 0; j < this.tubularSegments;++ j)
				{
					var ip = (i + 1) % this.radialSegments;
					var jp = (j + 1) % this.tubularSegments;

					var a = grid[i][j];
					var b = grid[ip][j];
					var c = grid[ip][jp];
					var d = grid[i][jp];

					var uva = new Vector2(i / (double)this.radialSegments, j / (double)this.tubularSegments);
					var uvb = new Vector2((i + 1) / (double)this.radialSegments, j / (double)this.tubularSegments);
					var uvc = new Vector2((i + 1) / (double)this.radialSegments, (j + 1) / (double)this.tubularSegments);
					var uvd = new Vector2(i / (double)this.radialSegments, (j + 1) / (double)this.tubularSegments);

					faces.push(new Face4(a, b, c, d));
					faceVertexUvs[0].push(new JSArray(uva, uvb, uvc, uvd));
				}
			}

			computeCentroids();
			computeFaceNormals();
			computeVertexNormals();
		}
	}
}
