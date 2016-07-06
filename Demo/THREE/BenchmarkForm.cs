using System;
using System.Windows.Forms;
using Demo.Benchmark;
using WebGL;

namespace Demo.THREE
{
    public class BenchmarkForm : BaseForm
    {
        private Random _random = new Random(Environment.TickCount);
        private readonly Scene _scene;
        private readonly Camera _camera;
        private readonly Renderer _renderer;
        private int _mouseX;
        private int _mouseY;
        private JSArray _objects;

        internal BenchmarkForm()
        {
            _camera = new Camera(60, aspectRatio, 1, 10000) {position = {z = 3200}};

            _scene = new Scene();

            _objects = new JSArray();

            var material = new Material();

            var loader = new Loader();
            loader.load("objects/Suzanne.js", (geometry, ignored) =>
                                              {
                                                  geometry.computeVertexNormals();

                                                  for (var i = 0; i < 5000; i++)
                                                  {
                                                      double pi = System.Math.PI;
                                                      var mesh = new Mesh(geometry, material)
                                                                 {
                                                                     position =
                                                                         {
                                                                             x = _random.NextDouble() * 10000 - 5000,
                                                                             y = _random.NextDouble() * 10000 - 5000,
                                                                             z = _random.NextDouble() * 10000 - 5000
                                                                         },
                                                                     rotation =
                                                                         {
                                                                             x = _random.NextDouble() * 2 * pi,
                                                                             y = _random.NextDouble() * 2 * pi
                                                                         }
                                                                 };

                                                      mesh.scale.x = mesh.scale.y = mesh.scale.z = _random.NextDouble() * 50 + 100;

                                                      _objects.push(mesh);

                                                      _scene.add(mesh);
                                                  }
                                              });

            _renderer = new Renderer(canvas);
            _renderer.setSize(ClientSize.Width, ClientSize.Height);
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

            for (int i = 0, il = _objects.length; i < il; i++)
            {
                _objects[i].rotation.x += 0.01;
                _objects[i].rotation.y += 0.02;
            }

            _renderer.render(_scene, _camera);
        }
    }
}