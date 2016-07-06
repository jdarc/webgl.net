using System;
using WebGL;

namespace THREE
{
	public class TubeGeometry : Geometry
	{
		public dynamic path;
		public int segments;
		public double radius;
		public int radiusSegments;
		public bool closed;
		public Object3D debug;
		public JSArray grid;
		public dynamic tangents;
		public dynamic binormals;

		public TubeGeometry(dynamic path, dynamic segments, dynamic radius, dynamic radiusSegments, dynamic closed, dynamic debug)
		{
			this.path = path;
			this.segments = segments || 64;
			this.radius = radius || 1;
			this.radiusSegments = radiusSegments || 8;
			this.closed = closed || false;

			if (debug)
			{
				this.debug = new Object3D();
			}

			grid = new JSArray();

			dynamic scope = this;
			dynamic tangent;
			dynamic normal;
			dynamic binormal;
			dynamic numpoints = this.segments + 1;
			dynamic x;
			dynamic y;
			dynamic z;
			dynamic tx;
			dynamic ty;
			dynamic tz;
			dynamic u;
			dynamic v;
			dynamic cx;
			dynamic cy;
			dynamic pos;
			dynamic pos2 = new Vector3();
			dynamic i;
			dynamic j;
			dynamic ip;
			dynamic jp;
			dynamic a;
			dynamic b;
			dynamic c;
			dynamic d;
			dynamic uva;
			dynamic uvb;
			dynamic uvc;
			dynamic uvd;

			var frames = new FrenetFrames(this.path, this.segments, this.closed);
			var tangents = frames.tangents;
			var normals = frames.normals;
			var binormals = frames.binormals;

			this.tangents = tangents;
			this.normals = normals;
			this.binormals = binormals;

			for (i = 0; i < numpoints; i++)
			{
				grid[i] = new JSArray();

				u = i / (numpoints - 1);

				pos = path.getPointAt(u);

				tangent = tangents[i];
				normal = normals[i];
				binormal = binormals[i];

				if (this.debug != null)
				{
					this.debug.add(new ArrowHelper(tangent, pos, radius, 0x0000ff));
					this.debug.add(new ArrowHelper(normal, pos, radius, 0xff0000));
					this.debug.add(new ArrowHelper(binormal, pos, radius, 0x00ff00));
				}

				for (j = 0; j < this.radiusSegments; j++)
				{
					v = j / this.radiusSegments * 2 * System.Math.PI;

					cx = -this.radius * System.Math.Cos(v);
					cy = this.radius * System.Math.Sin(v);

					pos2.copy(pos);
					pos2.x += cx * normal.x + cy * binormal.x;
					pos2.y += cx * normal.y + cy * binormal.y;
					pos2.z += cx * normal.z + cy * binormal.z;

					grid[i][j] = vert(pos2.x, pos2.y, pos2.z);
				}
			}

			for (i = 0; i < this.segments; i++)
			{
				for (j = 0; j < this.radiusSegments; j++)
				{
					ip = (this.closed) ? (i + 1) % this.segments : i + 1;
					jp = (j + 1) % this.radiusSegments;

					a = grid[i][j];
					b = grid[ip][j];
					c = grid[ip][jp];
					d = grid[i][jp];

					uva = new Vector2(i / this.segments, j / this.radiusSegments);
					uvb = new Vector2((i + 1) / this.segments, j / this.radiusSegments);
					uvc = new Vector2((i + 1) / this.segments, (j + 1) / this.radiusSegments);
					uvd = new Vector2(i / this.segments, (j + 1) / this.radiusSegments);

					faces.push(new Face4(a, b, c, d));
					faceVertexUvs[0].push(new JSArray(uva, uvb, uvc, uvd));
				}
			}

			computeCentroids();
			computeFaceNormals();
			computeVertexNormals();
		}

		private int vert(double x, double y, double z)
		{
			return vertices.push(new Vector3(x, y, z)) - 1;
		}

		public class FrenetFrames
		{
			public dynamic tangents;
			public dynamic normals;
			public dynamic binormals;

			public FrenetFrames(dynamic path, dynamic segments, dynamic closed)
			{
				dynamic normal = new Vector3();
				dynamic tangents = new JSArray();
				dynamic normals = new JSArray();
				dynamic binormals = new JSArray();
				dynamic vec = new Vector3();
				dynamic mat = new Matrix4();
				var numpoints = segments + 1;
				dynamic theta;
				dynamic epsilon = 0.0001;
				dynamic smallest;
				dynamic tx;
				dynamic ty;
				dynamic tz;
				dynamic i;
				dynamic u;
				dynamic v;

				this.tangents = tangents;
				this.normals = normals;
				this.binormals = binormals;

				for (i = 0; i < numpoints; i++)
				{
					u = i / (numpoints - 1);

					tangents[i] = path.getTangentAt(u);
					tangents[i].normalize();
				}

				Action<dynamic> initialNormal1 = (lastBinormal) =>
				{
					normals[0] = new Vector3();
					binormals[0] = new Vector3();
					if (lastBinormal == null)
					{
						lastBinormal = new Vector3(0, 0, 1);
					}
					normals[0].crossVectors(lastBinormal, tangents[0]).normalize();
					binormals[0].crossVectors(tangents[0], normals[0]).normalize();
				};

				Action initialNormal2 = () =>
				{
					var t2 = path.getTangentAt(epsilon);

					normals[0] = new Vector3().subVectors(t2, tangents[0]).normalize();
					binormals[0] = new Vector3().crossVectors(tangents[0], normals[0]);

					normals[0].crossVectors(binormals[0], tangents[0]).normalize();
					binormals[0].crossVectors(tangents[0], normals[0]).normalize();
				};

				Action initialNormal3 = () =>
				{
					normals[0] = new Vector3();
					binormals[0] = new Vector3();
					smallest = double.MaxValue;
					tx = System.Math.Abs(tangents[0].x);
					ty = System.Math.Abs(tangents[0].y);
					tz = System.Math.Abs(tangents[0].z);

					if (tx <= smallest)
					{
						smallest = tx;
						normal.set(1, 0, 0);
					}

					if (ty <= smallest)
					{
						smallest = ty;
						normal.set(0, 1, 0);
					}

					if (tz <= smallest)
					{
						normal.set(0, 0, 1);
					}

					vec.crossVectors(tangents[0], normal).normalize();

					normals[0].crossVectors(tangents[0], vec);
					binormals[0].crossVectors(tangents[0], normals[0]);
				};

				initialNormal3();

				for (i = 1; i < numpoints; i++)
				{
					normals[i] = normals[i - 1].clone();

					binormals[i] = binormals[i - 1].clone();

					vec.crossVectors(tangents[i - 1], tangents[i]);

					if (vec.length() > epsilon)
					{
						vec.normalize();

						theta = System.Math.Acos(tangents[i - 1].dot(tangents[i]));

						normals[i].applyMatrix4(mat.makeRotationAxis(vec, theta));
					}

					binormals[i].crossVectors(tangents[i], normals[i]);
				}

				if (closed)
				{
					theta = System.Math.Acos(normals[0].dot(normals[numpoints - 1]));
					theta /= (numpoints - 1);

					if (tangents[0].dot(vec.crossVectors(normals[0], normals[numpoints - 1])) > 0)
					{
						theta = -theta;
					}

					for (i = 1; i < numpoints; i++)
					{
						normals[i].applyMatrix4(mat.makeRotationAxis(tangents[i], theta * i));
						binormals[i].crossVectors(tangents[i], normals[i]);
					}
				}
			}
		}
	}
}
