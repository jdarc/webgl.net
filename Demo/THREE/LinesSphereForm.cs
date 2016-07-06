using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class LinesSphereForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;
        private int mouseX;
        private int mouseY;

        public LinesSphereForm()
        {
            camera = new PerspectiveCamera(80, aspectRatio, 1, 3000);
            camera.position.z = 1000;

            scene = new Scene();

            var r = 450;

            var parameters = new JSArray(
                new JSArray(0.25, 0xff7700, 1, 2),
                new JSArray(0.5, 0xff9900, 1, 1),
                new JSArray(0.75, 0xffaa00, 0.75, 1),
                new JSArray(1, 0xffaa00, 0.5, 1),
                new JSArray(1.25, 0x000833, 0.8, 1),
                new JSArray(3.0, 0xaaaaaa, 0.75, 2),
                new JSArray(3.5, 0xffffff, 0.5, 1),
                new JSArray(4.5, 0xffffff, 0.25, 1),
                new JSArray(5.5, 0xffffff, 0.125, 1));

            var geometry = new Geometry();

            for (var i = 0; i < 1500; i ++)
            {
                var vertex1 = new Vector3();
                vertex1.x = Math.random() * 2 - 1;
                vertex1.y = Math.random() * 2 - 1;
                vertex1.z = Math.random() * 2 - 1;
                vertex1.normalize();
                vertex1.multiplyScalar(r);

                var vertex2 = vertex1.clone();
                vertex2.multiplyScalar(Math.random() * 0.09 + 1);

                geometry.vertices.push(vertex1);
                geometry.vertices.push(vertex2);
            }

            for (var i = 0; i < parameters.length; ++ i)
            {
                var p = parameters[i];

                var material = new LineBasicMaterial(JSObject.create(new {color = p[1], opacity = p[2], linewidth = p[3]}));

                dynamic line = new Line(geometry, material, global::THREE.THREE.LinePieces);
                line.scale.x = line.scale.y = line.scale.z = p[0];
                line.originalScale = p[0];
                line.rotation.y = Math.random() * System.Math.PI;
                line.updateMatrix();
                scene.add(line);
            }

            renderer = new WebGLRenderer(new {canvas, antialias = true});
            renderer.setSize(ClientSize.Width, ClientSize.Height);
        }

        protected override void onWindowResize(EventArgs e)
        {
            if (camera != null)
            {
                camera.aspect = aspectRatio;
                camera.updateProjectionMatrix();

                renderer.setSize(ClientSize.Width, ClientSize.Height);
            }
        }

        protected override void onMouseClick(MouseEventArgs e)
        {
        }

        protected override void onMouseMove(MouseEventArgs e)
        {
            mouseX = (int)(e.X - windowHalf.X);
            mouseY = (int)(e.Y - windowHalf.Y);
        }

        protected override void render()
        {
            camera.position.y += (- mouseY + 200 - camera.position.y) * .05;
            camera.lookAt(scene.position);

            renderer.render(scene, camera);

            var time = JSDate.now() * 0.0001;

            for (var i = 0; i < scene.children.length; i ++)
            {
                var obj = scene.children[i];

                if (obj is Line)
                {
                    obj.rotation.y = time * (i < 4 ? (i + 1) : - (i + 1));

                    if (i < 5)
                    {
                        obj.scale.x = obj.scale.y = obj.scale.z = obj.originalScale * (i / 5 + 1) * (1 + 0.5 * System.Math.Sin(7 * time));
                    }
                }
            }
        }
    }
}