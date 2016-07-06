using System;
using WebGL;

namespace THREE
{
	public class Face3
	{
		public int a;
		public int b;
		public int c;
		public Vector3 normal;
		public JSArray vertexNormals;
		public Color color;
		public JSArray vertexColors;
		public JSArray vertexTangents;
		public int materialIndex;
		public Vector3 centroid;

		public dynamic __originalFaceNormal;
		public dynamic __originalVertexNormals;

		public Face3(int a = 0, int b = 0, int c = 0, dynamic normal = null, dynamic color = null, int materialIndex = 0)
		{
			this.a = a;
			this.b = b;
			this.c = c;

			this.normal = normal is Vector3 ? normal : new Vector3();
			vertexNormals = normal is JSArray ? normal : new JSArray();

			this.color = color is Color ? color : new Color();
			vertexColors = color is JSArray ? color : new JSArray();

			vertexTangents = new JSArray();

			this.materialIndex = materialIndex;

			centroid = new Vector3();
		}

		public int this[string name]
		{
			get
			{
				switch (name)
				{
					case "a":
						return a;
					case "b":
						return b;
					case "c":
						return c;
					default:
						throw new ApplicationException("Argument out of range");
				}
			}
		}

		public dynamic clone()
		{
			var face = new Face3(a, b, c);

			face.normal.copy(normal);
			face.color.copy(color);
			face.centroid.copy(centroid);

			face.materialIndex = materialIndex;

			int i, il;
			for (i = 0, il = vertexNormals.length; i < il; i++)
			{
				face.vertexNormals[i] = vertexNormals[i].clone();
			}
			for (i = 0, il = vertexColors.length; i < il; i++)
			{
				face.vertexColors[i] = vertexColors[i].clone();
			}
			for (i = 0, il = vertexTangents.length; i < il; i++)
			{
				face.vertexTangents[i] = vertexTangents[i].clone();
			}

			return face;
		}
	}
}
