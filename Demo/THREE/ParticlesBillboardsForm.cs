using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class ParticlesBillboardsForm : BaseForm
    {
        private readonly WebGLRenderer _renderer;
        private readonly PerspectiveCamera _camera;
        private readonly Scene _scene;
        private readonly Geometry _geometry;
        private readonly Texture _sprite;
        private readonly ParticleBasicMaterial _material;
        private readonly ParticleSystem _particles;
        private int _mouseX;
        private int _mouseY;

        public ParticlesBillboardsForm()
        {
            _camera = new PerspectiveCamera(55, aspectRatio, 2, 2000) {position = {z = 1000}};

            _scene = new Scene {fog = new FogExp2(0x000000, 0.001)};

            _geometry = new Geometry();

            _sprite = ImageUtils.loadTexture("textures/sprites/disc.png");

            for (var i = 0; i < 10000; i ++)
            {
                var vertex = new Vector3
                             {
                                 x = 2000 * Math.random() - 1000,
                                 y = 2000 * Math.random() - 1000,
                                 z = 2000 * Math.random() - 1000
                             };

                _geometry.vertices.push(vertex);
            }

            _material = new ParticleBasicMaterial(JSObject.create(new {size = 35, sizeAttenuation = false, map = _sprite, transparent = true}));
            _material.color.setHSV(1.0, 0.2, 0.8);

            _particles = new ParticleSystem(_geometry, _material) {sortParticles = true};
            _scene.add(_particles);

            _renderer = new WebGLRenderer(new {canvas, clearAlpha = 1});
            _renderer.setSize(ClientSize.Width, ClientSize.Height);
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

        protected override void onMouseMove(MouseEventArgs e)
        {
            _mouseX = (int)(e.X - windowHalf.X);
            _mouseY = (int)(e.Y - windowHalf.Y);
        }

        protected override void render()
        {
            var time = JSDate.now() * 0.00005;

            _camera.position.x += (_mouseX - _camera.position.x) * 0.05;
            _camera.position.y += (-_mouseY - _camera.position.y) * 0.05;

            _camera.lookAt(_scene.position);

            var h = (360 * (1.0 + time) % 360) / 360;
            _material.color.setHSV(h, 0.75, 0.8);

            _renderer.render(_scene, _camera);
        }
    }
}