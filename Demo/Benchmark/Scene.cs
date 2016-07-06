using System.Collections.Generic;

namespace Demo.Benchmark
{
    internal class Scene : Object3D
    {
        internal List<Object3D> __objects;
        internal List<Object3D> __objectsAdded;
        internal List<Object3D> __objectsRemoved;
        internal List<WebGLRenderObject> __webglObjects;

        internal Scene()
        {
            matrixAutoUpdate = false;

            __objects = new List<Object3D>();
            __objectsAdded = new List<Object3D>();
            __objectsRemoved = new List<Object3D>();
        }

        internal void addObject(Object3D obj)
        {
            if (!(obj is Camera))
            {
                if (__objects.IndexOf(obj) == -1)
                {
                    __objects.Add(obj);
                    __objectsAdded.Add(obj);

                    // check if previously removed
                    var i = __objectsRemoved.IndexOf(obj);
                    if (i != -1)
                    {
                        __objectsRemoved.RemoveAt(i);
                    }
                }
            }

            foreach (var t in obj.children)
            {
                addObject(t);
            }
        }
    }
}