using System;
using System.Collections.Generic;

namespace Demo.Benchmark
{
    internal class Object3D
    {
        internal Object3D parent;
        internal List<Object3D> children;
        internal Vector3 up;
        internal Vector3 position;
        internal Vector3 rotation;
        internal Vector3 scale;
        internal bool rotationAutoUpdate;
        internal Matrix4 matrix;
        internal Matrix4 matrixWorld;
        internal Matrix4 matrixRotationWorld;
        internal bool matrixAutoUpdate;
        internal bool matrixWorldNeedsUpdate;
        internal bool visible;
        internal bool frustumCulled;
        internal Geometry geometry;
        internal Material material;
        internal Matrix4 _modelViewMatrix;
        internal Matrix3 _normalMatrix;
        internal bool __webglInit;
        internal bool __webglActive;

        internal Object3D()
        {
            parent = null;
            children = new List<Object3D>();

            up = new Vector3(0, 1, 0);

            position = new Vector3();
            rotation = new Vector3();
            scale = new Vector3(1, 1, 1);

            rotationAutoUpdate = true;

            matrix = new Matrix4();
            matrixWorld = new Matrix4();
            matrixRotationWorld = new Matrix4();

            matrixAutoUpdate = true;
            matrixWorldNeedsUpdate = true;

            visible = true;

            frustumCulled = true;
        }

        internal virtual void lookAt(Vector3 vector)
        {
            matrix.lookAt(vector, position, up);
        }

        internal void add(Object3D obj)
        {
            if (obj == this)
            {
                Console.WriteLine(@"Object3D.add: An obj can't be added as a child of itself.");
                return;
            }
            if (obj.parent != null)
            {
                obj.parent.remove();
            }

            obj.parent = this;
            children.Add(obj);

            // add to scene

            var scene = this;

            while (scene.parent != null)
            {
                scene = scene.parent;
            }

            if (scene is Scene)
            {
                ((Scene)scene).addObject(obj);
            }
        }

        private void remove()
        {
            throw new NotImplementedException();
        }

        internal virtual void updateMatrix()
        {
            matrix.setPosition(position);

            matrix.setRotationFromEuler(rotation);

            if (scale.x != 1 || scale.y != 1 || scale.z != 1)
            {
                matrix.scale(scale);
            }

            matrixWorldNeedsUpdate = true;
        }

        internal void updateMatrixWorld(bool force = false)
        {
            if (matrixAutoUpdate)
            {
                updateMatrix();
            }

            if (matrixWorldNeedsUpdate || force)
            {
                if (parent == null)
                {
                    matrixWorld.copy(matrix);
                }
                else
                {
                    matrixWorld.multiplyMatrices(parent.matrixWorld, matrix);
                }

                matrixWorldNeedsUpdate = false;

                force = true;
            }

            // update children
            foreach (var t in children)
            {
                t.updateMatrixWorld(force);
            }
        }
    }
}