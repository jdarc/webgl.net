using System;

namespace Demo.Benchmark
{
    internal class Camera : Object3D
    {
        internal readonly Matrix4 matrixWorldInverse;
        internal readonly Matrix4 projectionMatrix;
        internal readonly Matrix4 projectionMatrixInverse;
        internal double fov;
        internal double aspect;
        internal double near;
        internal double far;

        internal Camera(double fov = 50.0, double aspect = 1.0, double near = 0.1, double far = 2000.0)
        {
            matrixWorldInverse = new Matrix4();
            projectionMatrix = new Matrix4();
            projectionMatrixInverse = new Matrix4();

            this.fov = fov;
            this.aspect = aspect;
            this.near = near;
            this.far = far;

            updateProjectionMatrix();
        }

        internal void updateProjectionMatrix()
        {
            projectionMatrix.makePerspective(fov, aspect, near, far);
        }

        internal override void lookAt(Vector3 vector)
        {
            matrix.lookAt(position, vector, up);

            if (rotationAutoUpdate)
            {
                rotation.y = Math.Asin(Math.Min(Math.Max(matrix.elements[8], -1.0), 1.0));

                if (Math.Abs(matrix.elements[8]) < 0.99999)
                {
                    rotation.x = Math.Atan2(-matrix.elements[9], matrix.elements[10]);
                    rotation.z = Math.Atan2(-matrix.elements[4], matrix.elements[0]);
                }
                else
                {
                    rotation.x = Math.Atan2(matrix.elements[6], matrix.elements[5]);
                    rotation.z = 0;
                }
            }
        }
    }
}