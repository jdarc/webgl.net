using System;
using WebGL;

namespace THREE
{
	public class Geometry : JSEventDispatcher
	{
		public int id;
		public string name;
		public JSArray vertices;
		public JSArray colors;
		public JSArray normals;
		public JSArray faces;
		public JSArray faceUvs;
		public JSArray faceVertexUvs;
		public JSArray morphTargets;
		public JSArray morphColors;
		public JSArray morphNormals;
		public JSArray skinWeights;
		public JSArray skinIndices;
		public JSArray lineDistances;
		public Box3 boundingBox;
		public Sphere boundingSphere;
		public bool hasTangents;
		public bool dynamic;
		public bool verticesNeedUpdate;
		public bool elementsNeedUpdate;
		public bool uvsNeedUpdate;
		public bool normalsNeedUpdate;
		public bool tangentsNeedUpdate;
		public bool colorsNeedUpdate;
		public bool lineDistancesNeedUpdate;
		public bool buffersNeedUpdate;

		public WebGLBuffer __webglVertexBuffer;
		public WebGLBuffer __webglColorBuffer;
		public WebGLBuffer __webglLineDistanceBuffer;
		public Float32Array __vertexArray;
		public Float32Array __colorArray;
		public Float32Array __lineDistanceArray;
		public JSArray __webglCustomAttributesList;
		public JSArray __sortArray;
		public int __webglParticleCount;
		public int __webglLineCount;

		private JSArray __tmpVertices;

		public Geometry()
		{
			id = THREE.GeometryIdCount++;

			name = "";

			vertices = new JSArray();
			colors = new JSArray();
			normals = new JSArray();

			faces = new JSArray();

			faceUvs = new JSArray(new JSArray());
			faceVertexUvs = new JSArray(new JSArray());

			morphTargets = new JSArray();
			morphColors = new JSArray();
			morphNormals = new JSArray();

			skinWeights = new JSArray();
			skinIndices = new JSArray();

			lineDistances = new JSArray();

			boundingBox = null;
			boundingSphere = null;

			hasTangents = false;

			dynamic = true;

			verticesNeedUpdate = false;
			elementsNeedUpdate = false;
			uvsNeedUpdate = false;
			normalsNeedUpdate = false;
			tangentsNeedUpdate = false;
			colorsNeedUpdate = false;
			lineDistancesNeedUpdate = false;

			buffersNeedUpdate = false;
		}

		public void applyMatrix(Matrix4 matrix)
		{
			var normalMatrix = new Matrix3().getInverse(matrix).transpose();

			for (int i = 0, il = vertices.length; i < il; i++)
			{
				var vertex = vertices[i];
				vertex.applyMatrix4(matrix);
			}

			for (int i = 0, il = faces.length; i < il; i++)
			{
				var face = faces[i];
				face.normal.applyMatrix3(normalMatrix).normalize();

				for (int j = 0, jl = face.vertexNormals.length; j < jl; j++)
				{
					face.vertexNormals[j].applyMatrix3(normalMatrix).normalize();
				}

				face.centroid.applyMatrix4(matrix);
			}
		}

		public void computeCentroids()
		{
			for (var f = 0; f < faces.length; f++)
			{
				var face = faces[f];
				face.centroid.set(0, 0, 0);

				if (face is Face3)
				{
					face.centroid.add(vertices[face.a]);
					face.centroid.add(vertices[face.b]);
					face.centroid.add(vertices[face.c]);
					face.centroid.divideScalar(3);
				}
				else if (face is Face4)
				{
					face.centroid.add(vertices[face.a]);
					face.centroid.add(vertices[face.b]);
					face.centroid.add(vertices[face.c]);
					face.centroid.add(vertices[face.d]);
					face.centroid.divideScalar(4);
				}
			}
		}

		public void computeFaceNormals()
		{
			var cb = new Vector3();
			var ab = new Vector3();

			for (var f = 0; f < faces.length; f++)
			{
				var face = faces[f];

				var vA = vertices[face.a];
				var vB = vertices[face.b];
				var vC = vertices[face.c];

				cb.subVectors(vC, vB);
				ab.subVectors(vA, vB);
				cb.cross(ab);

				cb.normalize();

				face.normal.copy(cb);
			}
		}

		public void computeVertexNormals(bool areaWeighted = false)
		{
			JSArray tmpVertices;

			if (__tmpVertices == null)
			{
				__tmpVertices = new JSArray(vertices.length);
				tmpVertices = __tmpVertices;

				for (int v = 0, vl = vertices.length; v < vl; v++)
				{
					tmpVertices[v] = new Vector3();
				}

				for (int f = 0, fl = faces.length; f < fl; f++)
				{
					var face = faces[f];

					if (face is Face3)
					{
						face.vertexNormals = new JSArray(new Vector3(), new Vector3(), new Vector3());
					}
					else if (face is Face4)
					{
						face.vertexNormals = new JSArray(new Vector3(), new Vector3(), new Vector3(), new Vector3());
					}
				}
			}
			else
			{
				tmpVertices = __tmpVertices;

				for (int v = 0, vl = vertices.length; v < vl; v++)
				{
					tmpVertices[v].set(0, 0, 0);
				}
			}

			if (eval(areaWeighted))
			{
				var cb = new Vector3();
				var ab = new Vector3();
				var db = new Vector3();
				var dc = new Vector3();
				var bc = new Vector3();

				for (int f = 0, fl = faces.length; f < fl; f++)
				{
					var face = faces[f];

					Vector3 vA;
					Vector3 vB;
					Vector3 vC;
					if (face is Face3)
					{
						vA = vertices[face.a];
						vB = vertices[face.b];
						vC = vertices[face.c];

						cb.subVectors(vC, vB);
						ab.subVectors(vA, vB);
						cb.cross(ab);

						tmpVertices[face.a].add(cb);
						tmpVertices[face.b].add(cb);
						tmpVertices[face.c].add(cb);
					}
					else if (face is Face4)
					{
						vA = vertices[face.a];
						vB = vertices[face.b];
						vC = vertices[face.c];
						Vector3 vD = vertices[face.d];

						db.subVectors(vD, vB);
						ab.subVectors(vA, vB);
						db.cross(ab);

						tmpVertices[face.a].add(db);
						tmpVertices[face.b].add(db);
						tmpVertices[face.d].add(db);

						dc.subVectors(vD, vC);
						bc.subVectors(vB, vC);
						dc.cross(bc);

						tmpVertices[face.b].add(dc);
						tmpVertices[face.c].add(dc);
						tmpVertices[face.d].add(dc);
					}
				}
			}
			else
			{
				for (int f = 0, fl = faces.length; f < fl; f++)
				{
					var face = faces[f];

					if (face is Face3)
					{
						tmpVertices[face.a].add(face.normal);
						tmpVertices[face.b].add(face.normal);
						tmpVertices[face.c].add(face.normal);
					}
					else if (face is Face4)
					{
						tmpVertices[face.a].add(face.normal);
						tmpVertices[face.b].add(face.normal);
						tmpVertices[face.c].add(face.normal);
						tmpVertices[face.d].add(face.normal);
					}
				}
			}

			for (int v = 0, vl = vertices.length; v < vl; v++)
			{
				tmpVertices[v].normalize();
			}

			for (int f = 0, fl = faces.length; f < fl; f++)
			{
				var face = faces[f];

				if (face is Face3)
				{
					face.vertexNormals[0].copy(tmpVertices[face.a]);
					face.vertexNormals[1].copy(tmpVertices[face.b]);
					face.vertexNormals[2].copy(tmpVertices[face.c]);
				}
				else if (face is Face4)
				{
					face.vertexNormals[0].copy(tmpVertices[face.a]);
					face.vertexNormals[1].copy(tmpVertices[face.b]);
					face.vertexNormals[2].copy(tmpVertices[face.c]);
					face.vertexNormals[3].copy(tmpVertices[face.d]);
				}
			}
		}

		public void computeMorphNormals()
		{
			dynamic vertexNormals;
			for (var f = 0; f < faces.length; f++)
			{
				var face = faces[f];

				if (!eval(face.__originalFaceNormal))
				{
					face.__originalFaceNormal = face.normal.clone();
				}
				else
				{
					face.__originalFaceNormal.copy(face.normal);
				}

				if (!eval(face.__originalVertexNormals))
				{
					face.__originalVertexNormals = new JSArray();
				}

				for (var i = 0; i < (int)face.vertexNormals.length; i++)
				{
					if (!eval(face.__originalVertexNormals[i]))
					{
						face.__originalVertexNormals[i] = face.vertexNormals[i].clone();
					}
					else
					{
						face.__originalVertexNormals[i].copy(face.vertexNormals[i]);
					}
				}
			}

			var tmpGeo = new Geometry {faces = faces};

			for (int i = 0, il = morphTargets.length; i < il; i++)
			{
				if (! eval(morphNormals[i]))
				{
					morphNormals[i] = new JSObject();
					morphNormals[i].faceNormals = new JSArray();
					morphNormals[i].vertexNormals = new JSArray();

					var dstNormalsFace = morphNormals[i].faceNormals;
					var dstNormalsVertex = morphNormals[i].vertexNormals;

					for (int f = 0, fl = faces.length; f < fl; f++)
					{
						var face = faces[f];

						var faceNormal = new Vector3();

						if (face is Face3)
						{
							vertexNormals = create(new {a = new Vector3(), b = new Vector3(), c = new Vector3()});
						}
						else
						{
							vertexNormals = create(new {a = new Vector3(), b = new Vector3(), c = new Vector3(), d = new Vector3()});
						}

						dstNormalsFace.push(faceNormal);
						dstNormalsVertex.push(vertexNormals);
					}
				}

				var monormals = morphNormals[i];

				tmpGeo.vertices = morphTargets[i].vertices;

				tmpGeo.computeFaceNormals();
				tmpGeo.computeVertexNormals();

				for (var f = 0; f < faces.length; f++)
				{
					var face = faces[f];

					var faceNormal = monormals.faceNormals[f];
					vertexNormals = monormals.vertexNormals[f];

					faceNormal.copy(face.normal);

					if (face is Face3)
					{
						vertexNormals.a.copy(face.vertexNormals[0]);
						vertexNormals.b.copy(face.vertexNormals[1]);
						vertexNormals.c.copy(face.vertexNormals[2]);
					}
					else
					{
						vertexNormals.a.copy(face.vertexNormals[0]);
						vertexNormals.b.copy(face.vertexNormals[1]);
						vertexNormals.c.copy(face.vertexNormals[2]);
						vertexNormals.d.copy(face.vertexNormals[3]);
					}
				}
			}

			for (int f = 0, fl = faces.length; f < fl; f++)
			{
				var face = faces[f];

				face.normal = face.__originalFaceNormal;
				face.vertexNormals = face.__originalVertexNormals;
			}
		}

		public void computeTangents()
		{
			JSArray uv = null;
			var tan1 = new JSArray();
			var tan2 = new JSArray();
			var sdir = new Vector3();
			var tdir = new Vector3();
			var tmp = new Vector3();
			var tmp2 = new Vector3();
			var n = new Vector3();

			for (int v = 0, vl = vertices.length; v < vl; v ++)
			{
				tan1[v] = new Vector3();
				tan2[v] = new Vector3();
			}

			Action<Geometry, int, int, int, int, int, int> handleTriangle = (context, a, b, c, ua, ub, uc) =>
			{
				var vA = context.vertices[a];
				var vB = context.vertices[b];
				var vC = context.vertices[c];

				var uvA = uv[ua];
				var uvB = uv[ub];
				var uvC = uv[uc];

				var x1 = vB.x - vA.x;
				var x2 = vC.x - vA.x;
				var y1 = vB.y - vA.y;
				var y2 = vC.y - vA.y;
				var z1 = vB.z - vA.z;
				var z2 = vC.z - vA.z;

				var s1 = uvB.x - uvA.x;
				var s2 = uvC.x - uvA.x;
				var t1 = uvB.y - uvA.y;
				var t2 = uvC.y - uvA.y;

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

			for (int f = 0, fl = faces.length; f < fl; f ++)
			{
				var face = faces[f];
				uv = faceVertexUvs[0][f]; // use UV layer 0 for tangents

				if (face is Face3)
				{
					handleTriangle(this, face.a, face.b, face.c, 0, 1, 2);
				}
				else if (face is Face4)
				{
					handleTriangle(this, face.a, face.b, face.d, 0, 1, 3);
					handleTriangle(this, face.b, face.c, face.d, 1, 2, 3);
				}
			}

			var faceIndex = new JSArray('a', 'b', 'c', 'd');

			for (int f = 0, fl = faces.length; f < fl; f ++)
			{
				var face = faces[f];

				for (int i = 0; i < face.vertexNormals.length; i++)
				{
					n.copy(face.vertexNormals[i]);

					var vertexIndex = face[faceIndex[i]];

					var t = tan1[vertexIndex];

					// Gram-Schmidt orthogonalize

					tmp.copy(t);
					tmp.sub(n.multiplyScalar(n.dot(t))).normalize();

					// Calculate handedness

					tmp2.crossVectors(face.vertexNormals[i], t);
					var test = tmp2.dot(tan2[vertexIndex]);
					var w = (test < 0.0) ? -1.0 : 1.0;

					face.vertexTangents[i] = new Vector4(tmp.x, tmp.y, tmp.z, w);
				}
			}

			hasTangents = true;
		}

		public void computeLineDistances()
		{
			var d = 0.0;

			for (var i = 0; i < vertices.length; i++)
			{
				if (i > 0)
				{
					d += vertices[i].distanceTo(vertices[i - 1]);
				}

				lineDistances[i] = d;
			}
		}

		public void computeBoundingBox()
		{
			if (boundingBox == null)
			{
				boundingBox = new Box3();
			}

			boundingBox.setFromPoints(vertices);
		}

		public void computeBoundingSphere()
		{
			if (boundingSphere == null)
			{
				boundingSphere = new Sphere();
			}

			boundingSphere.setFromCenterAndPoints(boundingSphere.center, vertices);
		}

		public int mergeVertices()
		{
			var verticesMap = new JSObject();
			JSArray unique = new JSArray(), changes = new JSArray();

			const double precisionPoints = 4.0;
			var precision = System.Math.Pow(10, precisionPoints);

			__tmpVertices = null;

			for (int i = 0, il = vertices.length; i < il; i++)
			{
				var v = vertices[i];
				var key = string.Format("{0}_{1}_{2}", Math.round(v.x * precision), Math.round(v.y * precision), Math.round(v.z * precision));

				if (verticesMap[key] == null)
				{
					verticesMap[key] = i;
					unique.push(vertices[i]);
					changes[i] = unique.length - 1;
				}
				else
				{
					changes[i] = changes[verticesMap[key]];
				}
			}

			var faceIndicesToRemove = new JSArray();

			for (int i = 0, il = faces.length; i < il; i++)
			{
				var face = faces[i];

				if (face is Face3)
				{
					face.a = changes[face.a];
					face.b = changes[face.b];
					face.c = changes[face.c];

					var indices = new JSArray(face.a, face.b, face.c);

					for (var n = 0; n < 3; n++)
					{
						if (indices[n] == indices[(n + 1) % 3])
						{
							faceIndicesToRemove.push(i);
							break;
						}
					}
				}
				else if (face is Face4)
				{
					face.a = changes[face.a];
					face.b = changes[face.b];
					face.c = changes[face.c];
					face.d = changes[face.d];

					var indices = new JSArray(face.a, face.b, face.c, face.d);

					var dupIndex = -1;

					for (var n = 0; n < 4; n++)
					{
						if (indices[n] == indices[(n + 1) % 4])
						{
							if (dupIndex >= 0)
							{
								faceIndicesToRemove.push(i);
							}

							dupIndex = n;
						}
					}

					if (dupIndex >= 0)
					{
						indices.splice(dupIndex, 1);

						var newFace = new Face3(indices[0], indices[1], indices[2], face.normal, face.color, face.materialIndex);

						for (int j = 0, jl = faceVertexUvs.length; j < jl; j++)
						{
							var u = faceVertexUvs[j][i];

							if (eval(u))
							{
								u.splice(dupIndex, 1);
							}
						}

						if (eval(face.vertexNormals) && face.vertexNormals.length > 0)
						{
							newFace.vertexNormals = face.vertexNormals;
							newFace.vertexNormals.splice(dupIndex, 1);
						}

						if (eval(face.vertexColors) && face.vertexColors.length > 0)
						{
							newFace.vertexColors = face.vertexColors;
							newFace.vertexColors.splice(dupIndex, 1);
						}

						faces[i] = newFace;
					}
				}
			}

			for (var i = faceIndicesToRemove.length - 1; i >= 0; i --)
			{
				faces.splice(i, 1);

				for (int j = 0, jl = faceVertexUvs.length; j < jl; j++)
				{
					faceVertexUvs[j].splice(i, 1);
				}
			}

			var diff = vertices.length - unique.length;
			vertices = unique;
			return diff;
		}

		public Geometry clone()
		{
			var geometry = new Geometry();

			for (var i = 0; i < vertices.length; i++)
			{
				geometry.vertices.push(vertices[i].clone());
			}

			for (var i = 0; i < faces.length; i++)
			{
				geometry.faces.push(faces[i].clone());
			}

			var uvs = faceVertexUvs[0];

			for (int i = 0, il = uvs.length; i < il; i++)
			{
				JSArray uv = uvs[i], uvCopy = new JSArray();
				for (int j = 0, jl = uv.length; j < jl; j++)
				{
					uvCopy.push(new Vector2(uv[j].x, uv[j].y));
				}
				geometry.faceVertexUvs[0].push(uvCopy);
			}

			return geometry;
		}

		public void dispose()
		{
			dispatchEvent(new JSEvent(this, "dispose"));
		}
	}
}
