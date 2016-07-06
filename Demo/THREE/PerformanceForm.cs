using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class PerformanceForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;
        private float mouseX;
        private float mouseY;
        private JSArray objects;

        public PerformanceForm()
        {
            camera = new PerspectiveCamera(60, aspectRatio, 1, 10000);
            camera.position.z = 3200;

            scene = new Scene();

            objects = new JSArray();

            var material = new MeshNormalMaterial(JSObject.create(new {shading = global::THREE.THREE.SmoothShading}));

            var loader = new JSONLoader();
            loader.load("objects/Suzanne.js", (geometry, ignored) =>
                                              {
                                                  geometry.computeVertexNormals();

                                                  for (var i = 0; i < 5000; i ++)
                                                  {
                                                      var mesh = new Mesh(geometry, material);

                                                      mesh.position.x = Math.random() * 8000 - 4000;
                                                      mesh.position.y = Math.random() * 8000 - 4000;
                                                      mesh.position.z = Math.random() * 8000 - 4000;
                                                      double pi = System.Math.PI;
                                                      mesh.rotation.x = Math.random() * 2 * pi;
                                                      mesh.rotation.y = Math.random() * 2 * pi;
                                                      mesh.scale.x = mesh.scale.y = mesh.scale.z = Math.random() * 50 + 100;

                                                      objects.push(mesh);

                                                      scene.add(mesh);
                                                  }
                                              });

            renderer = new WebGLRenderer(new {canvas, clearColor = 0xffffff});
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
            mouseX = (e.X - windowHalf.X) * 10;
            mouseY = (e.Y - windowHalf.Y) * 10;
        }

        protected override void render()
        {
            camera.position.x += (mouseX - camera.position.x) * .05;
            camera.position.y += (-mouseY - camera.position.y) * .05;
            camera.lookAt(scene.position);

            for (int i = 0, il = objects.length; i < il; i++)
            {
                objects[i].rotation.x += 0.01;
                objects[i].rotation.y += 0.02;
            }

            renderer.render(scene, camera);
        }
    }
}