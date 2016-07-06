using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class TrailsForm : BaseForm
    {
        private WebGLRenderer _renderer;
        private PerspectiveCamera _camera;
        private Scene _scene;
        private int _mouseX;
        private int _mouseY;

        protected override void initialize()
        {
            _camera = new PerspectiveCamera(60, aspectRatio, 1, 10000);
            _camera.position.set(100000, 0, 3200);

            _scene = new Scene();

            var colors = new JSArray(0x000000, 0xff0080, 0x8000ff, 0xffffff);
            var geometry = new Geometry();

            for (var i = 0; i < 2000; i++)
            {
                geometry.vertices.push(new Vector3
                                       {
                                           x = Math.random() * 4000 - 2000,
                                           y = Math.random() * 4000 - 2000,
                                           z = Math.random() * 4000 - 2000
                                       });
                geometry.colors.push(new Color(colors[(int)System.Math.Floor(Math.random() * colors.length)]));
            }

            var material = new ParticleBasicMaterial(JSObject.create((dynamic)new
                                                                            {
                                                                                size = 1,
                                                                                vertexColors = global::THREE.THREE.VertexColors,
                                                                                depthTest = false,
                                                                                opacity = 0.5,
                                                                                sizeAttenuation = false,
                                                                                transparent = true
                                                                            }));

            var mesh = new ParticleSystem(geometry, material);
            _scene.add(mesh);

            _renderer = new WebGLRenderer(new {canvas, preserveDrawingBuffer = true});
            _renderer.setSize(ClientSize.Width, ClientSize.Height);
            _renderer.sortObjects = false;
            _renderer.autoClearColor = false;
        }

        protected override void onWindowResize(EventArgs e)
        {
            if (_camera != null)
            {
                _camera.aspect = aspectRatio;
                _camera.updateProjectionMatrix();
                _renderer.setSize(ClientSize.Width, ClientSize.Height);
            }
        }

        protected override void onMouseMove(MouseEventArgs e)
        {
            _mouseX = (int)((e.X - windowHalf.X) * 10);
            _mouseY = (int)((e.Y - windowHalf.Y) * 10);
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