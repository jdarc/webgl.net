using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class PerformanceStaticForm : BaseForm
    {
        private readonly Scene _scene;
        private readonly PerspectiveCamera _camera;
        private readonly WebGLRenderer _renderer;
        private int _mouseX;
        private int _mouseY;

        public PerformanceStaticForm()
        {
            _camera = new PerspectiveCamera(60, aspectRatio, 1, 10000) {position = {z = 3200}};

            _scene = new Scene();

            var material = new MeshNormalMaterial(JSObject.create(new {shading = global::THREE.THREE.SmoothShading}));

            var loader = new JSONLoader();
            loader.load("objects/Suzanne.js", (geometry, ignored) =>
                                              {
                                                  geometry.computeVertexNormals();

                                                  for (var i = 0; i < 7700; i ++)
                                                  {
                                                      var mesh = new Mesh(geometry, material)
                                                                 {
                                                                     position =
                                                                         {
                                                                             x = Math.random() * 10000 - 5000,
                                                                             y = Math.random() * 10000 - 5000,
                                                                             z = Math.random() * 10000 - 5000
                                                                         }
                                                                 };

                                                      mesh.rotation.x = Math.random() * 2 * System.Math.PI;
                                                      mesh.rotation.y = Math.random() * 2 * System.Math.PI;
                                                      mesh.scale.x = mesh.scale.y = mesh.scale.z = Math.random() * 50 + 100;
                                                      mesh.matrixAutoUpdate = false;
                                                      mesh.updateMatrix();

                                                      _scene.add(mesh);
                                                  }
                                              });

            _renderer = new WebGLRenderer(new {canvas});
            _renderer.setSize(ClientSize.Width, ClientSize.Height);
            _renderer.sortObjects = false;
        }

        protected override void onMouseMove(MouseEventArgs e)
        {
            _mouseX = (int)((e.X - windowHalf.X) * 10);
            _mouseY = (int)((e.Y - windowHalf.Y) * 10);
        }

        protected override void onWindowResize(EventArgs eventArgs)
        {
            if (_camera != null)
            {
                _camera.aspect = aspectRatio;
                _camera.updateProjectionMatrix();

                _renderer.setSize(ClientSize.Width, ClientSize.Height);
            }
        }

        protected override void render()
        {
            _camera.position.x += (_mouseX - _camera.position.x) * .05;
            _camera.position.y += (-_mouseY - _camera.position.y) * .05;

            _camera.lookAt(_scene.position);

            _renderer.render(_scene, _camera);
        }
    }
}