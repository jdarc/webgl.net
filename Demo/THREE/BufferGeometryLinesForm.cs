using System;
using THREE;
using WebGL;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class BufferGeometryLinesForm : BaseForm
    {
        private readonly Scene _scene;
        private readonly Line _mesh;
        private readonly PerspectiveCamera _camera;
        private readonly WebGLRenderer _renderer;

        public BufferGeometryLinesForm()
        {
            _camera = new PerspectiveCamera(27, aspectRatio, 1.0, 4000.0) {position = {z = 2750}};

            _scene = new Scene();

            const int segments = 10000;

            var geometry = new BufferGeometry();
            var material = new LineBasicMaterial(JSObject.create(new {vertexColors = true}));

            geometry.attributes = JSObject.create((dynamic)new
                                                         {
                                                             position = new
                                                                        {
                                                                            itemSize = 3,
                                                                            array = new Float32Array(segments * 3),
                                                                            numItems = segments * 3
                                                                        },
                                                             color = new
                                                                     {
                                                                         itemSize = 3,
                                                                         array = new Float32Array(segments * 3),
                                                                         numItems = segments * 3
                                                                     }
                                                         });

            var positions = geometry.attributes.position.array;
            var colors = geometry.attributes.color.array;

            const int r = 800;

            for (var i = 0; i < segments; i ++)
            {
                var x = Math.random() * r - r / 2.0;
                var y = Math.random() * r - r / 2.0;
                var z = Math.random() * r - r / 2.0;

                // positions

                positions[i * 3] = (float)x;
                positions[i * 3 + 1] = (float)y;
                positions[i * 3 + 2] = (float)z;

                // colors

                colors[i * 3] = (float)((x / r) + 0.5);
                colors[i * 3 + 1] = (float)((y / r) + 0.5);
                colors[i * 3 + 2] = (float)((z / r) + 0.5);
            }

            geometry.computeBoundingSphere();

            _mesh = new Line(geometry, material);
            _scene.add(_mesh);

            //

            _renderer = new WebGLRenderer(new {canvas, antialias = false, clearColor = 0x000000, clearAlpha = 0, alpha = true});
            _renderer.setSize(ClientSize.Width, ClientSize.Height);

            _renderer.gammaInput = true;
            _renderer.gammaOutput = true;
            _renderer.physicallyBasedShading = true;
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

            _mesh.rotation.x = time * 0.25;
            _mesh.rotation.y = time * 0.5;

            _renderer.render(_scene, _camera);
        }
    }
}