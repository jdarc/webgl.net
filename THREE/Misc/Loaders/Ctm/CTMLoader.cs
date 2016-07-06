using System;
using System.IO;
using WebGL;

namespace THREE
{
	public class CTMLoader : Loader
	{
		public CTMLoader(bool showStatus) : base(showStatus)
		{
		}

		public void loadParts(dynamic url, Action<dynamic, dynamic> callback, dynamic parameters = null)
		{
			parameters = parameters ?? new JSObject();

			var basePath = parameters.basePath ?? this.extractUrlBase(url);

			string responseText = File.ReadAllText(url);
			var jsonObject = JSON.parse(responseText);

			dynamic materials = new JSArray();
			dynamic geometries = new JSArray();
			dynamic counter = 0;

			Action<dynamic> callbackFinal = geometry =>
			{
				counter += 1;

				geometries.push(geometry);

				if (counter == jsonObject.offsets.length)
				{
					callback(geometries, materials);
				}
			};

			for (var i = 0; i < jsonObject.materials.length; i++)
			{
				materials[i] = createMaterial(jsonObject.materials[i], basePath);
			}

			var partUrl = basePath + jsonObject.data;
			var parametersPart = JSObject.create(new {parameters.useWorker, parameters.useBuffers, jsonObject.offsets});
			this.load(partUrl, callbackFinal, parametersPart);
		}

		public void load(string url, dynamic callback, dynamic parameters = null)
		{
			parameters = parameters ?? new JSObject();

			var offsets = parameters.offsets ?? 0;
			var useBuffers = parameters.useBuffers ?? true;

			var xhr = new XMLHttpRequest();
			dynamic callbackProgress = null;

			var length = 0;

			xhr.onreadystatechange = () =>
			{
				if (xhr.readyState == 4)
				{
					if (xhr.status == 200 || xhr.status == 0)
					{
						var binaryData = xhr.responseText;

						for (var i = 0; i < offsets.length; i++)
						{
							var stream = new CTM.Stream(binaryData) {offset = offsets[i]};
							var ctmFile = new CTM.File(stream);

							if (useBuffers)
							{
								this.createModelBuffers(ctmFile, callback);
							}
							else
							{
								this.createModelClassic(ctmFile, callback);
							}
						}
					}
					else
					{
						JSConsole.error("Couldn't load [" + url + "] [" + xhr.status + "]");
					}
				}
				else if (xhr.readyState == 3)
				{
					if (callbackProgress != null)
					{
						if (length == 0)
						{
							length = Convert.ToInt32(xhr.getResponseHeader("Content-Length"));
						}

						callbackProgress(new {total = length, loaded = xhr.responseText.Length});
					}
				}
				else if (xhr.readyState == 2)
				{
					length = Convert.ToInt32(xhr.getResponseHeader("Content-Length"));
				}
			};

			xhr.open("GET", url, true);
			xhr.send();
		}

		private class Model : BufferGeometry
		{
			public Model(CTM.File file)
			{
				dynamic scope = this;

				var reorderVertices = true;

				scope.materials = new JSArray();

				var vertexIndexArray = file.body.indices;
				var vertexPositionArray = file.body.vertices;
				var vertexNormalArray = file.body.normals;

				Float32Array vertexUvArray = null;
				dynamic vertexColorArray = null;

				if (file.body.uvMaps != null && file.body.uvMaps.length > 0)
				{
					vertexUvArray = file.body.uvMaps[0].uv;
				}

				if (file.body.attrMaps != null && file.body.attrMaps.length > 0 && file.body.attrMaps[0].name == "Color")
				{
					vertexColorArray = file.body.attrMaps[0].attr;
				}

				int i;
				if (reorderVertices)
				{
					var newFaces = new Uint32Array(vertexIndexArray.length);
					var newVertices = new Float32Array(vertexPositionArray.length);

					Float32Array newNormals = null;
					dynamic newUvs = null;
					dynamic newColors = null;

					if (vertexNormalArray != null)
					{
						newNormals = new Float32Array(vertexNormalArray.length);
					}
					if (vertexUvArray != null)
					{
						newUvs = new Float32Array(vertexUvArray.length);
					}
					if (vertexColorArray != null)
					{
						newColors = new Float32Array(vertexColorArray.length);
					}

					var indexMap = new JSArray();
					var vertexCounter = 0;

					Action<int> handleVertex = (v) =>
					{
						if (indexMap[v] == null)
						{
							indexMap[v] = vertexCounter;

							var sx = v * 3;
							var sy = v * 3 + 1;
							var sz = v * 3 + 2;
							var dx = vertexCounter * 3;
							var dy = vertexCounter * 3 + 1;
							var dz = vertexCounter * 3 + 2;

							newVertices[dx] = vertexPositionArray[sx];
							newVertices[dy] = vertexPositionArray[sy];
							newVertices[dz] = vertexPositionArray[sz];

							if (vertexNormalArray != null)
							{
								newNormals[dx] = vertexNormalArray[sx];
								newNormals[dy] = vertexNormalArray[sy];
								newNormals[dz] = vertexNormalArray[sz];
							}

							if (vertexUvArray != null)
							{
								newUvs[vertexCounter * 2] = vertexUvArray[v * 2];
								newUvs[vertexCounter * 2 + 1] = vertexUvArray[v * 2 + 1];
							}

							if (vertexColorArray != null)
							{
								newColors[vertexCounter * 4] = vertexColorArray[v * 4];
								newColors[vertexCounter * 4 + 1] = vertexColorArray[v * 4 + 1];
								newColors[vertexCounter * 4 + 2] = vertexColorArray[v * 4 + 2];
								newColors[vertexCounter * 4 + 3] = vertexColorArray[v * 4 + 3];
							}

							vertexCounter += 1;
						}
					};

					for (i = 0; i < vertexIndexArray.length; i += 3)
					{
						var a = vertexIndexArray[i];
						var b = vertexIndexArray[i + 1];
						var c = vertexIndexArray[i + 2];

						handleVertex((int)a);
						handleVertex((int)b);
						handleVertex((int)c);

						newFaces[i] = (uint)indexMap[(int)a];
						newFaces[i + 1] = (uint)indexMap[(int)b];
						newFaces[i + 2] = (uint)indexMap[(int)c];
					}

					vertexIndexArray = newFaces;
					vertexPositionArray = newVertices;

					if (vertexNormalArray != null)
					{
						vertexNormalArray = newNormals;
					}
					if (vertexUvArray != null)
					{
						vertexUvArray = newUvs;
					}
					if (vertexColorArray != null)
					{
						vertexColorArray = newColors;
					}
				}

				scope.offsets = new JSArray();

				var indices = vertexIndexArray;

				var start = 0;
				var min = vertexPositionArray.length;
				var max = 0;
				var minPrev = min;

				for (i = 0; i < indices.length;)
				{
					for (var j = 0; j < 3;++ j)
					{
						var idx = (int)indices[i++];

						if (idx < min)
						{
							min = idx;
						}
						if (idx > max)
						{
							max = idx;
						}
					}

					if (max - min > 65535)
					{
						i -= 3;

						for (var k = start; k < i;++ k)
						{
							indices[k] -= (uint)minPrev;
						}

						scope.offsets.push(JSObject.create(new {start, count = i - start, index = minPrev}));

						start = i;
						min = vertexPositionArray.length;
						max = 0;
					}

					minPrev = min;
				}

				for (var k = start; k < i;++ k)
				{
					indices[k] -= (uint)minPrev;
				}

				scope.offsets.push(JSObject.create(new {start, count = i - start, index = minPrev}));

				var vertexIndexArray16 = new Uint16Array(vertexIndexArray);

				attributes.index = JSObject.create(new {itemSize = 1, array = vertexIndexArray16, numItems = vertexIndexArray16.length});
				attributes.position = JSObject.create(new {itemSize = 3, array = vertexPositionArray, numItems = vertexPositionArray.length});

				if (vertexNormalArray != null)
				{
					attributes.normal = JSObject.create(new {itemSize = 3, array = vertexNormalArray, numItems = vertexNormalArray.length});
				}

				if (vertexUvArray != null)
				{
					attributes.uv = JSObject.create(new {itemSize = 2, array = vertexUvArray, numItems = vertexUvArray.length});
				}

				if (vertexColorArray != null)
				{
					attributes.color = JSObject.create(new {itemSize = 4, array = vertexColorArray, numItems = vertexColorArray.length});
				}
			}
		}

		public void createModelBuffers(CTM.File file, dynamic callback)
		{
			dynamic geometry = new Model(file);

			if (geometry.attributes["normal"] == null)
			{
				geometry.computeVertexNormals();
			}

			callback(geometry);
		}

		public void createModelClassic(dynamic file, dynamic callback)
		{
			throw new NotImplementedException();
		}
	}
}
