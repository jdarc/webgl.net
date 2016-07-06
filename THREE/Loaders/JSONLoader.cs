using System;
using System.IO;
using WebGL;

namespace THREE
{
	public class JSONLoader : Loader
	{
		public bool withCredentials;

		public JSONLoader(bool showStatus = false) : base(showStatus)
		{
		}

		public void load(dynamic url, Action<dynamic, dynamic> callback, dynamic texturePath = null)
		{
			texturePath = texturePath != null && (texturePath is string) ? texturePath : this.extractUrlBase(url);

			onLoadStart();
			this.loadAjaxJSON(this, url, callback, texturePath);
		}

		public void loadAjaxJSON(dynamic context, dynamic url, dynamic callback, dynamic texturePath, dynamic callbackProgress = null)
		{
			string responseText = File.ReadAllText(url);
			var json = JSON.parse(responseText);
			context.createModel(json, callback, texturePath);
		}

		public void createModel(dynamic json, dynamic callback, dynamic texturePath)
		{
			var geometry = new Geometry();
			var scale = (json.scale != null) ? 1.0 / (double)json.scale : 1.0;

			parseModel(scale, json, geometry);
			parseSkin(geometry, json);
			parseMorphing(geometry, json, scale);

			geometry.computeCentroids();
			geometry.computeFaceNormals();

			var materials = this.initMaterials(json.materials, texturePath);

			if (this.needsTangents(materials))
			{
				geometry.computeTangents();
			}

			callback(geometry, materials);
		}

		private int isBitSet(int value, int position)
		{
			return value & (1 << position);
		}

		private void parseModel(double scale, dynamic json, dynamic geometry)
		{
			var faces = json.faces;
			var vertices = json.vertices;
			var normals = json.normals;
			var colors = json.colors;
			var nUvLayers = 0;

			for (var i = 0; i < json.uvs.length; i++)
			{
				if (json.uvs[i].length > 0)
				{
					nUvLayers++;
				}
			}

			for (var i = 0; i < nUvLayers; i++)
			{
				geometry.faceUvs[i] = new JSArray();
				geometry.faceVertexUvs[i] = new JSArray();
			}

			var offset = 0;
			var zLength = vertices.length;

			while (offset < zLength)
			{
				geometry.vertices.push(new Vector3
				{
					x = (double)(vertices[offset++]) * scale,
					y = (double)(vertices[offset++]) * scale,
					z = (double)(vertices[offset++]) * scale
				});
			}

			offset = 0;
			zLength = faces.length;

			while (offset < zLength)
			{
				var type = faces[offset++];

				var isQuad = isBitSet(type, 0);
				var hasMaterial = isBitSet(type, 1);
				var hasFaceUv = isBitSet(type, 2);
				var hasFaceVertexUv = isBitSet(type, 3);
				var hasFaceNormal = isBitSet(type, 4);
				var hasFaceVertexNormal = isBitSet(type, 5);
				var hasFaceColor = isBitSet(type, 6);
				var hasFaceVertexColor = isBitSet(type, 7);

				int nVertices;
				dynamic face;
				if (isQuad != 0)
				{
					face = new Face4();
					face.a = faces[offset++];
					face.b = faces[offset++];
					face.c = faces[offset++];
					face.d = faces[offset++];
					nVertices = 4;
				}
				else
				{
					face = new Face3();
					face.a = faces[offset++];
					face.b = faces[offset++];
					face.c = faces[offset++];
					nVertices = 3;
				}

				if (hasMaterial != 0)
				{
					face.materialIndex = faces[offset++];
				}

				var fi = geometry.faces.length;

				if (hasFaceUv != 0)
				{
					for (var i = 0; i < nUvLayers; i++)
					{
						var uvLayer = json.uvs[i];
						var uvIndex = faces[offset++];
						geometry.faceUvs[i][fi] = new Vector2(uvLayer[uvIndex * 2], uvLayer[uvIndex * 2 + 1]);
					}
				}

				if (hasFaceVertexUv != 0)
				{
					for (var i = 0; i < nUvLayers; i++)
					{
						var uvLayer = json.uvs[i];
						var uvs = new JSArray();
						for (var j = 0; j < nVertices; j++)
						{
							var uvIndex = faces[offset++];
							uvs[j] = new Vector2((double)uvLayer[uvIndex * 2], (double)uvLayer[uvIndex * 2 + 1]);
						}

						geometry.faceVertexUvs[i][fi] = uvs;
					}
				}

				if (hasFaceNormal != 0)
				{
					var normalIndex = faces[offset++] * 3;
					face.normal = new Vector3
					{
						x = (double)normals[normalIndex++],
						y = (double)normals[normalIndex++],
						z = (double)normals[normalIndex]
					};
				}

				if (hasFaceVertexNormal != 0)
				{
					for (var i = 0; i < nVertices; i++)
					{
						var normalIndex = faces[offset++] * 3;
						face.vertexNormals.push(new Vector3
						{
							x = (double)normals[normalIndex++],
							y = (double)normals[normalIndex++],
							z = (double)normals[normalIndex]
						});
					}
				}

				if (hasFaceColor != 0)
				{
					face.color = new Color(colors[faces[offset++]]);
				}

				if (hasFaceVertexColor != 0)
				{
					for (var i = 0; i < nVertices; i++)
					{
						face.vertexColors.push(new Color(colors[faces[offset++]]));
					}
				}

				geometry.faces.push(face);
			}
		}

		private void parseSkin(dynamic geometry, dynamic json)
		{
			if (json.skinWeights != null)
			{
				var l = json.skinWeights.length;
				for (var i = 0; i < l; i += 2)
				{
					var x = (double)json.skinWeights[i];
					var y = (double)json.skinWeights[i + 1];
					geometry.skinWeights.push(new Vector4(x, y, 0.0, 0.0));
				}
			}

			if (json.skinIndices != null)
			{
				var l = json.skinIndices.length;
				for (var i = 0; i < l; i += 2)
				{
					var a = (double)json.skinIndices[i];
					var b = (double)json.skinIndices[i + 1];
					geometry.skinIndices.push(new Vector4(a, b, 0.0, 0.0));
				}
			}

			geometry.bones = json.bones;
			geometry.animation = json.animation;
		}

		public void parseMorphing(dynamic geometry, dynamic json, double scale)
		{
			if (json.morphTargets != null)
			{
				for (int i = 0, l = json.morphTargets.length; i < l; i++)
				{
					geometry.morphTargets[i] = new JSObject();
					geometry.morphTargets[i].name = json.morphTargets[i].name;
					geometry.morphTargets[i].vertices = new JSArray();

					var dstVertices = geometry.morphTargets[i].vertices;
					var srcVertices = json.morphTargets[i].vertices;

					for (int v = 0, vl = srcVertices.length; v < vl; v += 3)
					{
						var vertex = new Vector3
						{
							x = (double)srcVertices[v] * scale,
							y = (double)srcVertices[v + 1] * scale,
							z = (double)srcVertices[v + 2] * scale
						};

						dstVertices.push(vertex);
					}
				}
			}

			if (json.morphColors != null)
			{
				for (int i = 0, l = json.morphColors.length; i < l; i++)
				{
					geometry.morphColors[i] = new JSObject();
					geometry.morphColors[i].name = json.morphColors[i].name;
					geometry.morphColors[i].colors = new JSArray();

					var dstColors = geometry.morphColors[i].colors;
					var srcColors = json.morphColors[i].colors;

					for (int c = 0, cl = srcColors.length; c < cl; c += 3)
					{
						var color = new Color();
						color.setRGB((double)srcColors[c], (double)srcColors[c + 1], (double)srcColors[c + 2]);
						dstColors.push(color);
					}
				}
			}
		}
	}
}
