using WebGL;

namespace THREE
{
	public static class GeometryUtils
	{
		// Merge two geometries or geometry and geometry from object (using object's transform)
		public static void merge(Geometry geometry1, dynamic object2)
		{
			dynamic matrix = null;
			dynamic normalMatrix = null;
			dynamic vertexOffset = geometry1.vertices.length;
			var geometry2 = object2 is Mesh ? object2.geometry : object2;
			dynamic vertices1 = geometry1.vertices;
			var vertices2 = geometry2.vertices;
			dynamic faces1 = geometry1.faces;
			var faces2 = geometry2.faces;
			var uvs1 = geometry1.faceVertexUvs[0];
			var uvs2 = geometry2.faceVertexUvs[0];

			if (object2 is Mesh)
			{
				if (object2.matrixAutoUpdate)
				{
					object2.updateMatrix();
				}

				matrix = object2.matrix;

				normalMatrix = new Matrix3();
				normalMatrix.getInverse(matrix);
				normalMatrix.transpose();
			}

			// vertices
			for (int i = 0, il = vertices2.length; i < il; i ++)
			{
				var vertex = vertices2[i];

				var vertexCopy = vertex.clone();

				if (matrix != null)
				{
					vertexCopy.applyMatrix4(matrix);
				}

				vertices1.push(vertexCopy);
			}

			// faces
			for (int i = 0, il = faces2.length; i < il; i ++)
			{
				var face = faces2[i];
				var faceVertexNormals = face.vertexNormals;
				var faceVertexColors = face.vertexColors;

				dynamic faceCopy = null;
				if (face is Face3)
				{
					faceCopy = new Face3(face.a + vertexOffset, face.b + vertexOffset, face.c + vertexOffset);
				}
				else if (face is Face4)
				{
					faceCopy = new Face4(face.a + vertexOffset, face.b + vertexOffset, face.c + vertexOffset, face.d + vertexOffset);
				}

				if (faceCopy != null)
				{
					Vector3 normal;
					faceCopy.normal.copy(face.normal);

					if (normalMatrix != null)
					{
						faceCopy.normal.applyMatrix3(normalMatrix).normalize();
					}

					for (int j = 0, jl = faceVertexNormals.length; j < jl; j ++)
					{
						normal = faceVertexNormals[j].clone();

						if (normalMatrix != null)
						{
							normal.applyMatrix3(normalMatrix).normalize();
						}

						faceCopy.vertexNormals.push(normal);
					}

					Color color;
					faceCopy.color.copy(face.color);

					for (int j = 0, jl = faceVertexColors.length; j < jl; j ++)
					{
						color = faceVertexColors[j];
						faceCopy.vertexColors.push(color.clone());
					}

					faceCopy.materialIndex = face.materialIndex;

					faceCopy.centroid.copy(face.centroid);

					if (matrix != null)
					{
						faceCopy.centroid.applyMatrix4(matrix);
					}

					faces1.push(faceCopy);
				}
			}

			// uvs
			for (int i = 0, il = uvs2.length; i < il; i ++)
			{
				var uv = uvs2[i];
				var uvCopy = new JSArray();

				for (int j = 0, jl = uv.length; j < jl; j ++)
				{
					uvCopy.push(new Vector2(uv[j].x, uv[j].y));
				}

				uvs1.push(uvCopy);
			}
		}

		public static Vector3 center(Geometry geometry)
		{
			geometry.computeBoundingBox();

			var bb = geometry.boundingBox;

			var offset = new Vector3();

			offset.addVectors(bb.min, bb.max);
			offset.multiplyScalar(-0.5);

			geometry.applyMatrix(new Matrix4().makeTranslation(offset.x, offset.y, offset.z));
			geometry.computeBoundingBox();

			return offset;
		}

		public static void triangulateQuads(Geometry geometry)
		{
			var faces = new JSArray();
			var faceUvs = new JSArray();
			var faceVertexUvs = new JSArray();

			for (int i = 0, il = geometry.faceUvs.length; i < il; i++)
			{
				faceUvs[i] = new JSArray();
			}

			for (int i = 0, il = geometry.faceVertexUvs.length; i < il; i++)
			{
				faceVertexUvs[i] = new JSArray();
			}

			for (int i = 0, il = geometry.faces.length; i < il; i++)
			{
				var face = geometry.faces[i];

				if (face is Face4)
				{
					var a = face.a;
					var b = face.b;
					var c = face.c;
					var d = face.d;

					var triA = new Face3();
					var triB = new Face3();

					triA.color.copy(face.color);
					triB.color.copy(face.color);

					triA.materialIndex = face.materialIndex;
					triB.materialIndex = face.materialIndex;

					triA.a = a;
					triA.b = b;
					triA.c = d;

					triB.a = b;
					triB.b = c;
					triB.c = d;

					if (face.vertexColors.length == 4)
					{
						triA.vertexColors[0] = face.vertexColors[0].clone();
						triA.vertexColors[1] = face.vertexColors[1].clone();
						triA.vertexColors[2] = face.vertexColors[3].clone();

						triB.vertexColors[0] = face.vertexColors[1].clone();
						triB.vertexColors[1] = face.vertexColors[2].clone();
						triB.vertexColors[2] = face.vertexColors[3].clone();
					}

					faces.push(triA, triB);

					for (int j = 0, jl = geometry.faceVertexUvs.length; j < jl; j++)
					{
						if (geometry.faceVertexUvs[j].length > 0)
						{
							var uvs = geometry.faceVertexUvs[j][i];

							var uvA = uvs[0];
							var uvB = uvs[1];
							var uvC = uvs[2];
							var uvD = uvs[3];

							var uvsTriA = new JSArray(uvA.clone(), uvB.clone(), uvD.clone());
							var uvsTriB = new JSArray(uvB.clone(), uvC.clone(), uvD.clone());

							faceVertexUvs[j].push(uvsTriA, uvsTriB);
						}
					}

					for (int j = 0, jl = geometry.faceUvs.length; j < jl; j++)
					{
						if (geometry.faceUvs[j].length > 0)
						{
							var faceUv = geometry.faceUvs[j][i];

							faceUvs[j].push(faceUv, faceUv);
						}
					}
				}
				else
				{
					faces.push(face);

					for (int j = 0, jl = geometry.faceUvs.length; j < jl; j++)
					{
						faceUvs[j].push(geometry.faceUvs[j][i]);
					}

					for (int j = 0, jl = geometry.faceVertexUvs.length; j < jl; j++)
					{
						faceVertexUvs[j].push(geometry.faceVertexUvs[j][i]);
					}
				}
			}

			geometry.faces = faces;
			geometry.faceUvs = faceUvs;
			geometry.faceVertexUvs = faceVertexUvs;

			geometry.computeCentroids();
			geometry.computeFaceNormals();
			geometry.computeVertexNormals();

			if (geometry.hasTangents)
			{
				geometry.computeTangents();
			}
		}
	}
}
