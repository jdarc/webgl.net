using System;
using WebGL;

namespace THREE
{
	public class BufferGeometry : JSEventDispatcher
	{
		public int id;
		public dynamic attributes;
		public bool dynamic;
		public JSArray offsets;
		public Box3 boundingBox;
		public Sphere boundingSphere;
		public bool hasTangents;
		public JSArray morphTargets;
		public bool verticesNeedUpdate;
		public bool normalsNeedUpdate;
		public bool tangentsNeedUpdate;

		public BufferGeometry()
		{
			id = THREE.GeometryIdCount++;

			attributes = new JSObject();

			dynamic = false;

			offsets = new JSArray();

			boundingBox = null;
			boundingSphere = null;

			hasTangents = false;

			morphTargets = new JSArray();
		}

		public void applyMatrix(Matrix4 matrix)
		{
			dynamic positionArray = null;
			dynamic normalArray = null;

			if (attributes["position"])
			{
				positionArray = attributes["position"].array;
			}
			if (attributes["normal"])
			{
				normalArray = attributes["normal"].array;
			}

			if (positionArray != null)
			{
				matrix.multiplyVector3Array(positionArray);
				verticesNeedUpdate = true;
			}

			if (normalArray != null)
			{
				var normalMatrix = new Matrix3();
				normalMatrix.getInverse(matrix).transpose();

				normalMatrix.multiplyVector3Array(normalArray);

				normalizeNormals();

				normalsNeedUpdate = true;
			}
		}

		public void computeBoundingBox()
		{
			if (boundingBox == null)
			{
				boundingBox = new Box3();
			}

			var positions = attributes["position"].array;

			if (positions)
			{
				var bb = boundingBox;

				if (positions.length >= 3)
				{
					bb.min.x = bb.max.x = positions[0];
					bb.min.y = bb.max.y = positions[1];
					bb.min.z = bb.max.z = positions[2];
				}

				for (int i = 3, il = positions.length; i < il; i += 3)
				{
					var x = positions[i];
					var y = positions[i + 1];
					var z = positions[i + 2];

					// bounding box

					if (x < bb.min.x)
					{
						bb.min.x = x;
					}
					else if (x > bb.max.x)
					{
						bb.max.x = x;
					}

					if (y < bb.min.y)
					{
						bb.min.y = y;
					}
					else if (y > bb.max.y)
					{
						bb.max.y = y;
					}

					if (z < bb.min.z)
					{
						bb.min.z = z;
					}
					else if (z > bb.max.z)
					{
						bb.max.z = z;
					}
				}
			}

			if (positions == null || positions.length == 0)
			{
				boundingBox.min.set(0, 0, 0);
				boundingBox.max.set(0, 0, 0);
			}
		}

		public void computeBoundingSphere()
		{
			if (boundingSphere == null)
			{
				boundingSphere = new Sphere();
			}

			var positions = attributes["position"].array;

			if (eval(positions))
			{
				var maxRadiusSq = 0.0;

				for (int i = 0, il = positions.length; i < il; i += 3)
				{
					var x = positions[i];
					var y = positions[i + 1];
					var z = positions[i + 2];

					var radiusSq = x * x + y * y + z * z;
					if (radiusSq > maxRadiusSq)
					{
						maxRadiusSq = radiusSq;
					}
				}

			    boundingSphere.radius = System.Math.Sqrt(maxRadiusSq);
			}
		}

		public void computeVertexNormals()
		{
			if (eval(attributes["position"]))
			{
				var nVertexElements = attributes["position"].array.length;

				if (attributes["normal"] == null)
				{
					attributes["normal"] = create(new
					{
						itemSize = 3,
						array = new Float32Array(nVertexElements),
						numItems = nVertexElements
					});
				}
				else
				{
					// reset existing normals to zero

					for (int i = 0, il = attributes["normal"].array.length; i < il; i ++)
					{
						attributes["normal"].array[i] = 0;
					}
				}

				var positions = attributes["position"].array;
				var normals = attributes["normal"].array;

				double x, y, z;
				Vector3 pA = new Vector3(),
				        pB = new Vector3(),
				        pC = new Vector3(),
				        cb = new Vector3(),
				        ab = new Vector3();

				// indexed elements

				if (attributes["index"])
				{
					var indices = attributes["index"].array;

					for (int j = 0, jl = offsets.length; j < jl; ++ j)
					{
						var start = offsets[j].start;
						var count = offsets[j].count;
						var index = offsets[j].index;

						for (int i = start, il = start + count; i < il; i += 3)
						{
							var vA = index + indices[i];
							var vB = index + indices[i + 1];
							var vC = index + indices[i + 2];

							x = positions[vA * 3];
							y = positions[vA * 3 + 1];
							z = positions[vA * 3 + 2];
							pA.set(x, y, z);

							x = positions[vB * 3];
							y = positions[vB * 3 + 1];
							z = positions[vB * 3 + 2];
							pB.set(x, y, z);

							x = positions[vC * 3];
							y = positions[vC * 3 + 1];
							z = positions[vC * 3 + 2];
							pC.set(x, y, z);

							cb.subVectors(pC, pB);
							ab.subVectors(pA, pB);
							cb.cross(ab);

							normals[vA * 3] += cb.x;
							normals[vA * 3 + 1] += cb.y;
							normals[vA * 3 + 2] += cb.z;

							normals[vB * 3] += cb.x;
							normals[vB * 3 + 1] += cb.y;
							normals[vB * 3 + 2] += cb.z;

							normals[vC * 3] += cb.x;
							normals[vC * 3 + 1] += cb.y;
							normals[vC * 3 + 2] += cb.z;
						}
					}

					// non-indexed elements (unconnected triangle soup)
				}
				else
				{
					for (int i = 0, il = positions.length; i < il; i += 9)
					{
						x = positions[i];
						y = positions[i + 1];
						z = positions[i + 2];
						pA.set(x, y, z);

						x = positions[i + 3];
						y = positions[i + 4];
						z = positions[i + 5];
						pB.set(x, y, z);

						x = positions[i + 6];
						y = positions[i + 7];
						z = positions[i + 8];
						pC.set(x, y, z);

						cb.subVectors(pC, pB);
						ab.subVectors(pA, pB);
						cb.cross(ab);

						normals[i] = cb.x;
						normals[i + 1] = cb.y;
						normals[i + 2] = cb.z;

						normals[i + 3] = cb.x;
						normals[i + 4] = cb.y;
						normals[i + 5] = cb.z;

						normals[i + 6] = cb.x;
						normals[i + 7] = cb.y;
						normals[i + 8] = cb.z;
					}
				}

				normalizeNormals();

				normalsNeedUpdate = true;
			}
		}

		public void normalizeNormals()
		{
			var normals = attributes["normal"].array;

			for (int i = 0, il = normals.length; i < il; i += 3)
			{
				var x = normals[i];
				var y = normals[i + 1];
				var z = normals[i + 2];

				var n = 1.0 / Math.sqrt(x * x + y * y + z * z);

				normals[i] *= n;
				normals[i + 1] *= n;
				normals[i + 2] *= n;
			}
		}

		public void computeTangents()
		{
			// based on http://www.terathon.com/code/tangent.html
			// (per vertex tangents)

			if (attributes["index"] == null ||
			    attributes["position"] == null ||
			    attributes["normal"] == null ||
			    attributes["uv"] == null)
			{
				JSConsole.warn("Missing required attributes (index, position, normal or uv) in BufferGeometry.computeTangents()");
				return;
			}

			var indices = attributes["index"].array;
			var positions = attributes["position"].array;
			var normals = attributes["normal"].array;
			var uvs = attributes["uv"].array;

			var nVertices = positions.length / 3;

			if (attributes["tangent"] == null)
			{
				var nTangentElements = 4 * nVertices;

				attributes["tangent"] = create(new
				{
					itemSize = 4,
					array = new Float32Array(nTangentElements),
					numItems = nTangentElements
				});
			}

			var tangents = attributes["tangent"].array;

			JSArray tan1 = new JSArray(), tan2 = new JSArray();

			for (var k = 0; k < nVertices; k ++)
			{
				tan1[k] = new Vector3();
				tan2[k] = new Vector3();
			}

			Vector3 sdir = new Vector3(), tdir = new Vector3();

			Action<int, int, int> handleTriangle = (a, b, c) =>
			{
				var xA = positions[a * 3];
				var yA = positions[a * 3 + 1];
				var zA = positions[a * 3 + 2];

				var xB = positions[b * 3];
				var yB = positions[b * 3 + 1];
				var zB = positions[b * 3 + 2];

				var xC = positions[c * 3];
				var yC = positions[c * 3 + 1];
				var zC = positions[c * 3 + 2];

				var uA = uvs[a * 2];
				var vA = uvs[a * 2 + 1];

				var uB = uvs[b * 2];
				var vB = uvs[b * 2 + 1];

				var uC = uvs[c * 2];
				var vC = uvs[c * 2 + 1];

				var x1 = xB - xA;
				var x2 = xC - xA;

				var y1 = yB - yA;
				var y2 = yC - yA;

				var z1 = zB - zA;
				var z2 = zC - zA;

				var s1 = uB - uA;
				var s2 = uC - uA;

				var t1 = vB - vA;
				var t2 = vC - vA;

				var r = 1.0 / (s1 * t2 - s2 * t1);

				sdir.set((t2 * x1 - t1 * x2) * r,
				         (t2 * y1 - t1 * y2) * r,
				         (t2 * z1 - t1 * z2) * r);

				tdir.set((s1 * x2 - s2 * x1) * r,
				         (s1 * y2 - s2 * y1) * r,
				         (s1 * z2 - s2 * z1) * r);

				tan1[a].add(sdir);
				tan1[b].add(sdir);
				tan1[c].add(sdir);

				tan2[a].add(tdir);
				tan2[b].add(tdir);
				tan2[c].add(tdir);
			};

			int iA, iB, iC;

			for (int j = 0, jl = offsets.length; j < jl; ++ j)
			{
				var start = offsets[j].start;
				var count = offsets[j].count;
				var index = offsets[j].index;

				for (int i = start, il = start + count; i < il; i += 3)
				{
					iA = index + indices[i];
					iB = index + indices[i + 1];
					iC = index + indices[i + 2];

					handleTriangle(iA, iB, iC);
				}
			}

			Vector3 tmp = new Vector3(), tmp2 = new Vector3();
			Vector3 n = new Vector3(), n2 = new Vector3();

			Action<int> handleVertex = v =>
			{
				n.x = normals[v * 3];
				n.y = normals[v * 3 + 1];
				n.z = normals[v * 3 + 2];

				n2.copy(n);

				var t = tan1[v];

				// Gram-Schmidt orthogonalize

				tmp.copy(t);
				tmp.sub(n.multiplyScalar(n.dot(t))).normalize();

				// Calculate handedness

				tmp2.crossVectors(n2, t);
				var test = tmp2.dot(tan2[v]);
				var w = (test < 0.0) ? -1.0 : 1.0;

				tangents[v * 4] = tmp.x;
				tangents[v * 4 + 1] = tmp.y;
				tangents[v * 4 + 2] = tmp.z;
				tangents[v * 4 + 3] = w;
			};

			for (int j = 0, jl = offsets.length; j < jl; ++ j)
			{
				var start = offsets[j].start;
				var count = offsets[j].count;
				var index = offsets[j].index;

				for (int i = start, il = start + count; i < il; i += 3)
				{
					iA = index + indices[i];
					iB = index + indices[i + 1];
					iC = index + indices[i + 2];

					handleVertex(iA);
					handleVertex(iB);
					handleVertex(iC);
				}
			}

			hasTangents = true;
			tangentsNeedUpdate = true;
		}

		public void dispose()
		{
			dispatchEvent(new JSEvent(this, "dispose"));
		}
	}
}
