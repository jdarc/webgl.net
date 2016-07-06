using System;
using THREE;
using WebGL;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class BufferGeometryParticlesForm : BaseForm
    {
        private readonly Scene _scene;
        private readonly PerspectiveCamera _camera;
        private readonly ParticleSystem _particleSystem;
        private readonly WebGLRenderer _renderer;

        public BufferGeometryParticlesForm()
        {
            _camera = new PerspectiveCamera(27, aspectRatio, 5, 3500) {position = {z = 2750}};

            _scene = new Scene {fog = new Fog(0x050505, 2000.0, 3500.0)};

            const int particles = 500000;

            var geometry = new BufferGeometry
                           {
                               attributes = JSObject.create((dynamic)new
                                                                   {
                                                                       position = new
                                                                                  {
                                                                                      itemSize = 3,
                                                                                      array = new Float32Array(particles * 3),
                                                                                      numItems = particles * 3
                                                                                  },
                                                                       color = new
                                                                               {
                                                                                   itemSize = 3,
                                                                                   array = new Float32Array(particles * 3),
                                                                                   numItems = particles * 3
                                                                               }
                                                                   })
                           };

            var positions = geometry.attributes.position.array;
            var colors = geometry.attributes.color.array;

            var color = new Color();

            const double n = 1000.0;
            const double n2 = n / 2.0;

            for (var i = 0; i < positions.length; i += 3)
            {
                // positions

                var x = Math.random() * n - n2;
                var y = Math.random() * n - n2;
                var z = Math.random() * n - n2;

                positions[i] = (float)x;
                positions[i + 1] = (float)y;
                positions[i + 2] = (float)z;

                // colors

                var vx = (x / n) + 0.5;
                var vy = (y / n) + 0.5;
                var vz = (z / n) + 0.5;

                //color.setHSV( 0.5 + 0.5 * vx, 0.25 + 0.75 * vy, 0.25 + 0.75 * vz );
                color.setRGB(vx, vy, vz);

                colors[i] = (float)color.r;
                colors[i + 1] = (float)color.g;
                colors[i + 2] = (float)color.b;
            }

            geometry.computeBoundingSphere();

            //

            var material = new ParticleBasicMaterial(JSObject.create(new {size = 15.0, vertexColors = true}));

            _particleSystem = new ParticleSystem(geometry, material);
            _scene.add(_particleSystem);

            //

            _renderer = new WebGLRenderer(new {canvas, antialias = false, clearColor = 0x050505, clearAlpha = 1, alpha = false});

            _renderer.setSize(ClientSize.Width, ClientSize.Height);
            _renderer.setClearColor(_scene.fog.color, 1.0);
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
            var time = JSDate.now() * 0.001;

            _particleSystem.rotation.x = time * 0.25;
            _particleSystem.rotation.y = time * 0.5;

            _renderer.render(_scene, _camera);
        }
    }
}