using System;
using WebGL;

namespace THREE
{
	public class Shape : Path
	{
		public JSArray holes;

		public Shape(JSArray arguments = null) : base(arguments)
		{
			holes = new JSArray();
		}

		public ExtrudeGeometry extrude(dynamic options)
		{
			return new ExtrudeGeometry(this, options);
		}

		public ShapeGeometry makeGeometry(dynamic options)
		{
			return new ShapeGeometry(this, options);
		}

		public JSArray getPointsHoles(double divisions)
		{
			var holesPts = new JSArray();

			for (var i = 0; i < holes.length; i++)
			{
				holesPts[i] = holes[i].getTransformedPoints(divisions, bends);
			}

			return holesPts;
		}

		public JSArray getSpacedPointsHoles(double divisions)
		{
			var holesPts = new JSArray();

			for (var i = 0; i < holes.length; i++)
			{
				holesPts[i] = holes[i].getTransformedSpacedPoints(divisions, bends);
			}

			return holesPts;
		}

		public JSObject extractAllPoints(double divisions)
		{
			return JSObject.create((dynamic)new
			{
				shape = getTransformedPoints(divisions),
				holes = getPointsHoles(divisions)
			});
		}

		public JSObject extractPoints(double divisions)
		{
			if (useSpacedPoints)
			{
				return extractAllSpacedPoints(divisions);
			}

			return extractAllPoints(divisions);
		}

		public JSObject extractAllSpacedPoints(double divisions)
		{
			return JSObject.create((dynamic)new
			{
				shape = getTransformedSpacedPoints(divisions),
				holes = getSpacedPointsHoles(divisions)
			});
		}

		public static class Utils
		{
			public static JSObject removeHoles(JSArray contour, JSArray holes)
			{
				var shape = contour.concat();
				var allpoints = shape.concat();
				var verts = new JSArray();

				for (var h = 0; h < holes.length; h++)
				{
					var hole = holes[h];
					JSArray.prototype.push.apply(allpoints, hole);

					var shortest = double.PositiveInfinity;
					var holeIndex = 0;
					var shapeIndex = 0;
					for (var h2 = 0; h2 < hole.length; h2++)
					{
						var pts1 = hole[h2];
						var dist = new JSArray();

						for (var p = 0; p < shape.length; p++)
						{
							var pts2 = shape[p];
							double d = pts1.distanceToSquared(pts2);
							dist.push(d);

							if (d < shortest)
							{
								shortest = d;
								holeIndex = h2;
								shapeIndex = p;
							}
						}
					}

					var prevShapeVert = (shapeIndex - 1) >= 0 ? shapeIndex - 1 : shape.length - 1;
					var prevHoleVert = (holeIndex - 1) >= 0 ? holeIndex - 1 : hole.length - 1;

					var areaapts = new JSArray(hole[holeIndex], shape[shapeIndex], shape[prevShapeVert]);

					var areaa = FontUtils.Triangulate.area(areaapts);

					var areabpts = new JSArray(hole[holeIndex], hole[prevHoleVert], shape[shapeIndex]);

					var areab = FontUtils.Triangulate.area(areabpts);

					var shapeOffset = 1;
					var holeOffset = -1;

					var oldShapeIndex = shapeIndex;
					var oldHoleIndex = holeIndex;
					shapeIndex += shapeOffset;
					holeIndex += holeOffset;

					if (shapeIndex < 0)
					{
						shapeIndex += shape.length;
					}
					shapeIndex %= shape.length;

					if (holeIndex < 0)
					{
						holeIndex += hole.length;
					}
					holeIndex %= hole.length;

					prevShapeVert = (shapeIndex - 1) >= 0 ? shapeIndex - 1 : shape.length - 1;
					prevHoleVert = (holeIndex - 1) >= 0 ? holeIndex - 1 : hole.length - 1;

					areaapts = new JSArray(hole[holeIndex], shape[shapeIndex], shape[prevShapeVert]);

					var areaa2 = FontUtils.Triangulate.area(areaapts);

					areabpts = new JSArray(hole[holeIndex], hole[prevHoleVert], shape[shapeIndex]);

					var areab2 = FontUtils.Triangulate.area(areabpts);

					if ((areaa + areab) > (areaa2 + areab2))
					{
						shapeIndex = oldShapeIndex;
						holeIndex = oldHoleIndex;

						if (shapeIndex < 0)
						{
							shapeIndex += shape.length;
						}
						shapeIndex %= shape.length;

						if (holeIndex < 0)
						{
							holeIndex += hole.length;
						}
						holeIndex %= hole.length;

						prevShapeVert = (shapeIndex - 1) >= 0 ? shapeIndex - 1 : shape.length - 1;
						prevHoleVert = (holeIndex - 1) >= 0 ? holeIndex - 1 : hole.length - 1;
					}
					else
					{
					}

					var tmpShape1 = shape.slice(0, shapeIndex);
					var tmpShape2 = shape.slice(shapeIndex);
					var tmpHole1 = hole.slice(holeIndex);
					var tmpHole2 = hole.slice(0, holeIndex);

					var trianglea = new JSArray(hole[holeIndex], shape[shapeIndex], shape[prevShapeVert]);

					var triangleb = new JSArray(hole[holeIndex], hole[prevHoleVert], shape[shapeIndex]);

					verts.push(trianglea);
					verts.push(triangleb);

					shape = tmpShape1.concat(tmpHole1).concat(tmpHole2).concat(tmpShape2);
				}

				return JSObject.create(new {shape, isolatedPts = verts, allpoints});
			}

			public static dynamic triangulateShape(JSArray contour, JSArray holes)
			{
				var shapeWithoutHoles = removeHoles(contour, holes);

				JSArray shape = shapeWithoutHoles["shape"];
				JSArray allpoints = shapeWithoutHoles["allpoints"];
				JSArray isolatedPts = shapeWithoutHoles["isolatedPts"];

				var triangles = FontUtils.Triangulate.process(shape, false);

				dynamic f;
				dynamic face;
				string key;
				dynamic index;
				dynamic allPointsMap = new JSObject();
				dynamic isolatedPointsMap = new JSObject();

				for (int i = 0, il = allpoints.length; i < il; i++)
				{
					key = allpoints[i].x + ":" + allpoints[i].y;

					if (allPointsMap[key] != null)
					{
						JSConsole.log(String.Format("Duplicate point", key));
					}

					allPointsMap[key] = i;
				}

				for (int i = 0, il = triangles.length; i < il; i++)
				{
					face = triangles[i];

					for (f = 0; f < 3; f++)
					{
						key = face[f].x + ":" + face[f].y;

						index = allPointsMap[key];

						if (index != null)
						{
							face[f] = index;
						}
					}
				}

				for (int i = 0, il = isolatedPts.length; i < il; i++)
				{
					face = isolatedPts[i];

					for (f = 0; f < 3; f++)
					{
						key = face[f].x + ":" + face[f].y;

						index = allPointsMap[key];

						if (index != null)
						{
							face[f] = index;
						}
					}
				}

				return triangles.concat(isolatedPts);
			}

			public static bool isClockWise(dynamic pts)
			{
				return FontUtils.Triangulate.area(pts) < 0.0;
			}

			public static double b2p0(double t, double p)
			{
				var k = 1 - t;
				return k * k * p;
			}

			public static double b2p1(double t, double p)
			{
				return 2 * (1 - t) * t * p;
			}

			public static double b2p2(double t, double p)
			{
				return t * t * p;
			}

			public static double b2(double t, double p0, double p1, double p2)
			{
				return b2p0(t, p0) + b2p1(t, p1) + b2p2(t, p2);
			}

			public static double b3p0(double t, double p)
			{
				var k = 1 - t;
				return k * k * k * p;
			}

			public static double b3p1(double t, double p)
			{
				var k = 1 - t;
				return 3 * k * k * t * p;
			}

			public static double b3p2(double t, double p)
			{
				var k = 1 - t;
				return 3 * k * t * t * p;
			}

			public static double b3p3(double t, double p)
			{
				return t * t * t * p;
			}

			public static double b3(double t, double p0, double p1, double p2, double p3)
			{
				return b3p0(t, p0) + b3p1(t, p1) + b3p2(t, p2) + b3p3(t, p3);
			}
		}
	}
}
