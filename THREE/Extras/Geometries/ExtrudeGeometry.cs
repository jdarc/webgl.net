using System;
using WebGL;

namespace THREE
{
	public class ExtrudeGeometry : Geometry
	{
		public static readonly Vector2 __v1 = new Vector2();
		public static readonly Vector2 __v2 = new Vector2();
		public static readonly Vector2 __v3 = new Vector2();
		public static readonly Vector2 __v4 = new Vector2();
		public static readonly Vector2 __v5 = new Vector2();
		public static readonly Vector2 __v6 = new Vector2();

		public dynamic shapebb;

		public ExtrudeGeometry(dynamic shapes = null, dynamic options = null)
		{
			if (shapes != null)
			{
				shapes = shapes is JSArray ? shapes : new JSArray(shapes);

				shapebb = shapes[shapes.length - 1].getBoundingBox();

				this.addShapeList(shapes, options);

				computeCentroids();
				computeFaceNormals();
			}
		}

		public void addShapeList(dynamic shapes, dynamic options = null)
		{
			var sl = shapes.length;

			for (var s = 0; s < sl; s++)
			{
				this.addShape(shapes[s], options);
			}
		}

		private readonly Func<dynamic, dynamic, double, dynamic> scalePt2 = (pt, vec, size) =>
		{
			if (vec != null)
			{
				return vec.clone().multiplyScalar(size).add(pt);
			}
			JSConsole.log("die");
			return null;
		};

		private static readonly Func<dynamic, dynamic, dynamic, dynamic> getBevelVec1 = (pt_i, pt_j, pt_k) =>
		{
			var anglea = System.Math.Atan2((double)(pt_j.y - pt_i.y), (double)(pt_j.x - pt_i.x));
			var angleb = System.Math.Atan2((double)(pt_k.y - pt_i.y), (double)(pt_k.x - pt_i.x));

			if (anglea > angleb)
			{
				angleb += System.Math.PI * 2;
			}

			var anglec = (anglea + angleb) / 2;

			var x = -System.Math.Cos(anglec);
			var y = -System.Math.Sin(anglec);

			var vec = new Vector2(x, y);

			return vec;
		};

		private readonly Func<dynamic, dynamic, dynamic, dynamic> getBevelVec2 = (pt_i, pt_j, pt_k) =>
		{
			var a = __v1;
			var b = __v2;
			var v_hat = __v3;
			var w_hat = __v4;
			var p = __v5;
			var q = __v6;

			a.set(pt_i.x - pt_j.x, pt_i.y - pt_j.y);
			b.set(pt_i.x - pt_k.x, pt_i.y - pt_k.y);

			var v = a.normalize();
			var w = b.normalize();

			v_hat.set(-v.y, v.x);
			w_hat.set(w.y, -w.x);

			p.copy(pt_i).add(v_hat);
			q.copy(pt_i).add(w_hat);

			if (p.equals(q))
			{
				return w_hat.clone();
			}

			p.copy(pt_j).add(v_hat);
			q.copy(pt_k).add(w_hat);

			var v_dot_w_hat = v.dot(w_hat);
			var q_sub_p_dot_w_hat = q.sub(p).dot(w_hat);

			if (v_dot_w_hat == 0)
			{
				JSConsole.log("Either infinite or no solutions!");

				if (q_sub_p_dot_w_hat == 0)
				{
					JSConsole.log("Its finite solutions.");
				}
				else
				{
					JSConsole.log("Too bad, no solutions.");
				}
			}

			var s = q_sub_p_dot_w_hat / v_dot_w_hat;

			if (s < 0)
			{
				return getBevelVec1(pt_i, pt_j, pt_k);
			}

			var intersection = v.multiplyScalar(s).add(p);

			return intersection.sub(pt_i).clone();
		};

		public void addShape(dynamic shape, dynamic options = null)
		{
			Action<double, double, double> v = (x, y, z) => this.vertices.push(new Vector3(x, y, z));

			Func<dynamic, dynamic, dynamic, dynamic> getBevelVec = (pt_i, pt_j, pt_k) => getBevelVec2(pt_i, pt_j, pt_k);

			var amount = options.amount != null ? options.amount : 100;

			var bevelThickness = options.bevelThickness ?? 6;
			var bevelSize = options.bevelSize ?? bevelThickness - 2;
			var bevelSegments = options.bevelSegments ?? 3;

			var bevelEnabled = options.bevelEnabled ?? true;

			var curveSegments = options.curveSegments ?? 12;

			var steps = options.steps ?? 1;

			var extrudePath = options.extrudePath;
			dynamic extrudePts = null;
			var extrudeByPath = false;

			var material = options.material;
			var extrudeMaterial = options.extrudeMaterial;

			var uvgen = options.UVGenerator ?? new WorldUVGenerator();

			var shapebb = this.shapebb;

			dynamic splineTube = null;
			dynamic binormal = null;
			dynamic normal = null;
			dynamic position2 = null;
			if (extrudePath != null)
			{
				extrudePts = extrudePath.getSpacedPoints(steps);

				extrudeByPath = true;
				bevelEnabled = false;

				splineTube = options.frames ?? new TubeGeometry.FrenetFrames(extrudePath, steps, false);

				binormal = new Vector3();
				normal = new Vector3();
				position2 = new Vector3();
			}

			if (! bevelEnabled)
			{
				bevelSegments = 0;
				bevelThickness = 0;
				bevelSize = 0;
			}

			dynamic ahole;
			int h;
			int hl;
			var bevelPoints = new JSArray();

			var shapesOffset = this.vertices.length;

			var shapePoints = shape.extractPoints(curveSegments);

			var vertices = shapePoints.shape;
			var holes = shapePoints.holes;

			bool reverse = !Shape.Utils.isClockWise(vertices);

			if (reverse)
			{
				vertices = vertices.reverse();

				for (h = 0, hl = holes.length; h < hl; h++)
				{
					ahole = holes[h];

					if (Shape.Utils.isClockWise(ahole))
					{
						holes[h] = ahole.reverse();
					}
				}

				reverse = false;
			}

			var faces = Shape.Utils.triangulateShape(vertices, holes);

			/* Vertices */

			JSArray contour = vertices;

			for (h = 0, hl = holes.length; h < hl; h++)
			{
				ahole = holes[h];

				vertices = vertices.concat(ahole);
			}

			var vlen = vertices.length;
			dynamic face;
			var flen = faces.length;
			dynamic cont;
			var clen = contour.length;

			var RAD_TO_DEGREES = 180 / System.Math.PI;

			dynamic contourMovements = new JSArray();

			for (int i = 0, il = contour.length, j = il - 1, k = i + 1; i < il; i++, j++, k++)
			{
				if (j == il)
				{
					j = 0;
				}
				if (k == il)
				{
					k = 0;
				}

				var pt_i = contour[i];
				var pt_j = contour[j];
				var pt_k = contour[k];

				contourMovements[i] = getBevelVec(contour[i], contour[j], contour[k]);
			}

			dynamic holesMovements = new JSArray();
			dynamic oneHoleMovements;
			var verticesMovements = contourMovements.concat();

			for (h = 0, hl = holes.length; h < hl; h++)
			{
				ahole = holes[h];

				oneHoleMovements = new JSArray();

				for (int i = 0, il = ahole.length, j = il - 1, k = i + 1; i < il; i++, j++, k++)
				{
					if (j == il)
					{
						j = 0;
					}
					if (k == il)
					{
						k = 0;
					}

					oneHoleMovements[i] = getBevelVec(ahole[i], ahole[j], ahole[k]);
				}

				holesMovements.push(oneHoleMovements);
				verticesMovements = verticesMovements.concat(oneHoleMovements);
			}

			dynamic bs;
			for (var b = 0; b < bevelSegments; b++)
			{
				var t = b / bevelSegments;
				var z = bevelThickness * (1 - t);

				bs = bevelSize * (System.Math.Sin(t * System.Math.PI / 2));

				for (int i = 0, il = contour.length; i < il; i++)
				{
					var vert = scalePt2(contour[i], contourMovements[i], bs);

					v(vert.x, vert.y, - z);
				}

				for (h = 0, hl = holes.length; h < hl; h++)
				{
					ahole = holes[h];
					oneHoleMovements = holesMovements[h];

					for (int i = 0, il = ahole.length; i < il; i++)
					{
						var vert = scalePt2(ahole[i], oneHoleMovements[i], bs);

						v(vert.x, vert.y, -z);
					}
				}
			}

			bs = bevelSize;

			for (var i = 0; i < vlen; i++)
			{
				var vert = bevelEnabled ? scalePt2(vertices[i], verticesMovements[i], bs) : vertices[i];

				if (!extrudeByPath)
				{
					v(vert.x, vert.y, 0);
				}
				else
				{
					normal.copy(splineTube.normals[0]).multiplyScalar(vert.x);
					binormal.copy(splineTube.binormals[0]).multiplyScalar(vert.y);

					position2.copy(extrudePts[0]).add(normal).add(binormal);

					v(position2.x, position2.y, position2.z);
				}
			}

			for (var s = 1; s <= steps; s++)
			{
				for (var i = 0; i < vlen; i++)
				{
					var vert = bevelEnabled ? scalePt2(vertices[i], verticesMovements[i], bs) : vertices[i];

					if (!extrudeByPath)
					{
						v(vert.x, vert.y, amount / steps * s);
					}
					else
					{
						normal.copy(splineTube.normals[s]).multiplyScalar(vert.x);
						binormal.copy(splineTube.binormals[s]).multiplyScalar(vert.y);

						position2.copy(extrudePts[s]).add(normal).add(binormal);

						v(position2.x, position2.y, position2.z);
					}
				}
			}

			for (int b = bevelSegments - 1; b >= 0; b --)
			{
				var t = b / bevelSegments;
				var z = bevelThickness * (1 - t);

				bs = bevelSize * System.Math.Sin(t * System.Math.PI / 2);

				for (int i = 0, il = contour.length; i < il; i++)
				{
					var vert = scalePt2(contour[i], contourMovements[i], bs);
					v(vert.x, vert.y, amount + z);
				}

				for (h = 0, hl = holes.length; h < hl; h++)
				{
					ahole = holes[h];
					oneHoleMovements = holesMovements[h];

					for (int i = 0, il = ahole.length; i < il; i++)
					{
						var vert = scalePt2(ahole[i], oneHoleMovements[i], bs);

						if (!extrudeByPath)
						{
							v(vert.x, vert.y, amount + z);
						}
						else
						{
							v(vert.x, vert.y + extrudePts[steps - 1].y, extrudePts[steps - 1].x + z);
						}
					}
				}
			}

			/* Faces */

			Action<dynamic, dynamic, dynamic, dynamic> f3 = (a, b, c, isBottom) =>
			{
				a += shapesOffset;
				b += shapesOffset;
				c += shapesOffset;

				this.faces.push(new Face3(a, b, c, null, null, material ?? 0));

				var uvs = isBottom ? uvgen.generateBottomUV(this, shape, options, a, b, c) : uvgen.generateTopUV(this, shape, options, a, b, c);

				faceVertexUvs[0].push(uvs);
			};

			Action<dynamic, dynamic, dynamic, dynamic, dynamic, dynamic, dynamic, dynamic, dynamic> f4 = (a, b, c, d, wallContour, stepIndex, stepsLength, contourIndex1, contourIndex2) =>
			{
				a += shapesOffset;
				b += shapesOffset;
				c += shapesOffset;
				d += shapesOffset;

				this.faces.push(new Face4(a, b, c, d, null, null, extrudeMaterial ?? 0));

				var uvs = uvgen.generateSideWallUV(this, shape, wallContour, options, a, b, c, d, stepIndex, stepsLength, contourIndex1, contourIndex2);
				faceVertexUvs[0].push(uvs);
			};

			Action buildLidFaces = () =>
			{
				if (bevelEnabled)
				{
					var layer = 0;
					var offset = vlen * layer;

					for (var i = 0; i < flen; i++)
					{
						face = faces[i];
						f3(face[2] + offset, face[1] + offset, face[0] + offset, true);
					}

					layer = steps + bevelSegments * 2;
					offset = vlen * layer;

					for (var i = 0; i < flen; i++)
					{
						face = faces[i];
						f3(face[0] + offset, face[1] + offset, face[2] + offset, false);
					}
				}
				else
				{
					for (var i = 0; i < flen; i++)
					{
						face = faces[i];
						f3(face[2], face[1], face[0], true);
					}

					for (var i = 0; i < flen; i++)
					{
						face = faces[i];
						f3(face[0] + vlen * steps, face[1] + vlen * steps, face[2] + vlen * steps, false);
					}
				}
			};

			Action<dynamic, dynamic> sidewalls = (cntour, layeroffset) =>
			{
				var i = cntour.length;

				while (--i >= 0)
				{
					var j = i;
					var k = i - 1;
					if (k < 0)
					{
						k = cntour.length - 1;
					}
					int s;
					var sl = steps + bevelSegments * 2;

					for (s = 0; s < sl; s++)
					{
						var slen1 = vlen * s;
						var slen2 = vlen * (s + 1);

						var a = layeroffset + j + slen1;
						var b = layeroffset + k + slen1;
						var c = layeroffset + k + slen2;
						var d = layeroffset + j + slen2;

						f4(a, b, c, d, cntour, s, sl, j, k);
					}
				}
			};

			Action buildSideFaces = () =>
			{
				var layeroffset = 0;
				sidewalls(contour, layeroffset);
				layeroffset += contour.length;

				for (h = 0, hl = holes.length; h < hl; h++)
				{
					ahole = holes[h];
					sidewalls(ahole, layeroffset);

					layeroffset += ahole.length;
				}
			};

			buildLidFaces();

			buildSideFaces();
		}

		public class WorldUVGenerator
		{
			public JSArray generateTopUV(dynamic geometry, dynamic extrudedShape, dynamic extrudeOptions, dynamic indexA, dynamic indexB, dynamic indexC)
			{
				var ax = geometry.vertices[indexA].x;
				var ay = geometry.vertices[indexA].y;
				var bx = geometry.vertices[indexB].x;
				var by = geometry.vertices[indexB].y;
				var cx = geometry.vertices[indexC].x;
				var cy = geometry.vertices[indexC].y;

				return new JSArray(new Vector2(ax, ay), new Vector2(bx, by), new Vector2(cx, cy));
			}

			public JSArray generateBottomUV(dynamic geometry, dynamic extrudedShape, dynamic extrudeOptions, dynamic indexA, dynamic indexB, dynamic indexC)
			{
				return generateTopUV(geometry, extrudedShape, extrudeOptions, indexA, indexB, indexC);
			}

			public JSArray generateSideWallUV(dynamic geometry, dynamic extrudedShape, dynamic wallContour,
			                                dynamic extrudeOptions, dynamic indexA, dynamic indexB,
			                                dynamic indexC, dynamic indexD, dynamic stepIndex,
			                                dynamic stepsLength, dynamic contourIndex1, dynamic contourIndex2)
			{
				var ax = geometry.vertices[indexA].x;
				var ay = geometry.vertices[indexA].y;
				var az = geometry.vertices[indexA].z;
				var bx = geometry.vertices[indexB].x;
				var by = geometry.vertices[indexB].y;
				var bz = geometry.vertices[indexB].z;
				var cx = geometry.vertices[indexC].x;
				var cy = geometry.vertices[indexC].y;
				var cz = geometry.vertices[indexC].z;
				var dx = geometry.vertices[indexD].x;
				var dy = geometry.vertices[indexD].y;
				var dz = geometry.vertices[indexD].z;

				if (System.Math.Abs(ay - by) < 0.01)
				{
					return new JSArray(new Vector2(ax, 1 - az),
					                 new Vector2(bx, 1 - bz),
					                 new Vector2(cx, 1 - cz),
					                 new Vector2(dx, 1 - dz));
				}
				else
				{
					return new JSArray(new Vector2(ay, 1 - az),
					                 new Vector2(by, 1 - bz),
					                 new Vector2(cy, 1 - cz),
					                 new Vector2(dy, 1 - dz));
				}
			}
		}
	}
}
