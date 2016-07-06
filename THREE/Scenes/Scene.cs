using System.Collections.Generic;
using WebGL;

namespace THREE
{
	public class Scene : Object3D
	{
		public dynamic fog;
		public Material overrideMaterial;
		public List<Object3D> __objects;
		public List<Light> __lights;
		public List<Object3D> __objectsAdded;
		public List<Object3D> __objectsRemoved;
		public JSArray __webglObjectsImmediate;
		public JSArray __webglObjects;
		public JSArray __webglSprites;
		public JSArray __webglFlares;

		public Scene()
		{
			fog = null;
			overrideMaterial = null;

			matrixAutoUpdate = false;

			__objects = new List<Object3D>();
			__lights = new List<Light>();

			__objectsAdded = new List<Object3D>();
			__objectsRemoved = new List<Object3D>();
		}

		public void __addObject(dynamic obj)
		{
			if (obj is Light)
			{
				if (__lights.IndexOf(obj) == - 1)
				{
					__lights.Add((Light)obj);
				}

				if (obj.target != null && obj.target.parent == null)
				{
					add(obj.target);
				}
			}
			else if (!(obj is Camera || obj is Bone))
			{
				if (__objects.IndexOf(obj) == - 1)
				{
					__objects.Add((Object3D)obj);
					__objectsAdded.Add((Object3D)obj);

					var i = __objectsRemoved.IndexOf(obj);

					if (i != -1)
					{
						__objectsRemoved.RemoveAt(i);
					}
				}
			}

			for (var c = 0; c < obj.children.length; c++)
			{
				this.__addObject(obj.children[c]);
			}
		}

		public void __removeObject(Object3D obj)
		{
			if (obj is Light)
			{
				var i = __lights.IndexOf(obj as Light);

				if (i != -1)
				{
					__lights.RemoveAt(i);
				}
			}
			else if (!(obj is Camera))
			{
				var i = __objects.IndexOf(obj);

				if (i != -1)
				{
					__objects.RemoveAt(i);
					__objectsRemoved.Add(obj);

					var ai = __objectsAdded.IndexOf(obj);

					if (ai != -1)
					{
						__objectsAdded.RemoveAt(ai);
					}
				}
			}

			for (var c = 0; c < obj.children.length; c++)
			{
				this.__removeObject(obj.children[c]);
			}
		}
	}
}
