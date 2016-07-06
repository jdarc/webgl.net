using System;
using WebGL;

namespace THREE
{
	public static class FontUtils
	{
		public static class Triangulate
		{
			private const double EPSILON = 0.0000000001;

			public static bool snip(JSArray contour, int u, int v, int w, int n, JSArray verts)
			{
				double ax = contour[verts[u]].x;
				double ay = contour[verts[u]].y;

				double bx = contour[verts[v]].x;
				double by = contour[verts[v]].y;

				double cx = contour[verts[w]].x;
				double cy = contour[verts[w]].y;

				if (EPSILON > (((bx - ax) * (cy - ay)) - ((by - ay) * (cx - ax))))
				{
					return false;
				}

				var aX = cx - bx;
				var aY = cy - by;
				var bX = ax - cx;
				var bY = ay - cy;
				var cX = bx - ax;
				var cY = by - ay;

				for (var p = 0; p < n; p++)
				{
					if ((p == u) || (p == v) || (p == w))
					{
						continue;
					}

					double px = contour[verts[p]].x;
					double py = contour[verts[p]].y;

					var apx = px - ax;
					var apy = py - ay;
					var bpx = px - bx;
					var bpy = py - by;
					var cpx = px - cx;
					var cpy = py - cy;

					if ((aX * bpy - aY * bpx >= 0.0) && (bX * cpy - bY * cpx >= 0.0) && (cX * apy - cY * apx >= 0.0))
					{
						return false;
					}
				}

				return true;
			}

			public static double area(JSArray contour)
			{
				var n = contour.length;
				var a = 0.0;

				for (int p = n - 1, q = 0; q < n; p = q++)
				{
					a += contour[p].x * contour[q].y - contour[q].x * contour[p].y;
				}

				return a * 0.5;
			}

			public static JSArray process(JSArray contour, bool indices)
			{
				var n = contour.length;

				if (n < 3)
				{
					return null;
				}

				var result = new JSArray();
				var verts = new JSArray();
				var vertIndices = new JSArray();

				/* we want a counter-clockwise polygon in verts */
				if (area(contour) > 0.0)
				{
					for (var v = 0; v < n; v++)
					{
						verts[v] = v;
					}
				}
				else
				{
					for (var v = 0; v < n; v++)
					{
						verts[v] = (n - 1) - v;
					}
				}

				var nv = n;

				/*  remove nv - 2 vertices, creating 1 triangle every time */

				var count = 2 * nv; /* error detection */

				for (var v = nv - 1; nv > 2;)
				{
					if ((count--) <= 0)
					{
						JSConsole.log("Warning, unable to triangulate polygon!");
						return indices ? vertIndices : result;
					}

					var u = v;
					if (nv <= u)
					{
						u = 0; /* previous */
					}
					v = u + 1;
					if (nv <= v)
					{
						v = 0; /* new v    */
					}
					var w = v + 1;
					if (nv <= w)
					{
						w = 0; /* next     */
					}

					if (snip(contour, u, v, w, nv, verts))
					{
						/* true names of the vertices */

						var a = verts[u];
						var b = verts[v];
						var c = verts[w];

						/* output Triangle */

						result.push(new JSArray(contour[a], contour[b], contour[c]));

						vertIndices.push(new JSArray(verts[u], verts[v], verts[w]));

						/* remove v from the remaining polygon */

						for (int s = v, t = v + 1; t < nv; s++, t++)
						{
							verts[s] = verts[t];
						}

						nv--;

						/* reset error detection counter */
						count = 2 * nv;
					}
				}

				return indices ? vertIndices : result;
			}
		}

		public static dynamic generateShapes(string text, JSObject parameters = null)
		{
			throw new NotImplementedException();
		}
	}
}
