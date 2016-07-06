using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = System.Math;

namespace Demo.THREE
{
    public class PerformanceDoubleSidedForm : BaseForm
    {
        private readonly Scene _scene;
        private readonly PerspectiveCamera _camera;
        private readonly WebGLRenderer _renderer;
        private int _mouseX;
        private int _mouseY;

        public PerformanceDoubleSidedForm()
        {
            _camera = new PerspectiveCamera(50, aspectRatio, 1, 20000) {position = {z = 3200}};

            _scene = new Scene();

            var light = new PointLight(0x0011ff, 1, 5500);
            light.position.set(4000, 0, 0);
            _scene.add(light);

            light = new PointLight(0xff1100, 1, 5500);
            light.position.set(-4000, 0, 0);
            _scene.add(light);

            light = new PointLight(0xffaa00, 2, 3000);
            light.position.set(0, 0, 0);
            _scene.add(light);

            const string path = "textures/cube/SwedishRoyalCastle/";
            const string format = ".jpg";
            var urls = new JSArray(
                path + "px" + format, path + "nx" + format,
                path + "py" + format, path + "ny" + format,
                path + "pz" + format, path + "nz" + format
                );

            var reflectionCube = ImageUtils.loadTextureCube(urls);
            reflectionCube.format = global::THREE.THREE.RGBFormat;

            dynamic material = new MeshPhongMaterial(JSObject.create((dynamic)new
                                                                            {
                                                                                specular = 0xffffff,
                                                                                shininess = 100,
                                                                                envMap = reflectionCube,
                                                                                combine = global::THREE.THREE.MixOperation,
                                                                                reflectivity = 0.1,
                                                                                side = global::THREE.THREE.DoubleSide
                                                                            }));
            material.wrapAround = true;
            material.wrapRGB.set(0.5, 0.5, 0.5);

            var geometry = new SphereGeometry(1, 32, 16, 0, Math.PI);

            for (var i = 0; i < 5000; i ++)
            {
                var mesh = new Mesh(geometry, material)
                           {
                               position =
                                   {
                                       x = global::THREE.Math.random() * 10000 - 5000,
                                       y = global::THREE.Math.random() * 10000 - 5000,
                                       z = global::THREE.Math.random() * 10000 - 5000
                                   }
                           };

                mesh.rotation.x = global::THREE.Math.random() * 2 * Math.PI;
                mesh.rotation.y = global::THREE.Math.random() * 2 * Math.PI;
                mesh.scale.x = mesh.scale.y = mesh.scale.z = global::THREE.Math.random() * 50 + 100;

                mesh.matrixAutoUpdate = false;
                mesh.updateMatrix();

                _scene.add(mesh);
            }

            _renderer = new WebGLRenderer(new {canvas, clearColor = 0x050505, clearAlpha = 1, antialias = true});
            _renderer.setSize(ClientSize.Width, ClientSize.Height);

            _renderer.gammaInput = true;
            _renderer.gammaOutput = true;
            _renderer.physicallyBasedShading = true;
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
                _renderer.setSize(ClientSize.Width, ClientSize.Height);

                _camera.aspect = aspectRatio;
                _camera.updateProjectionMatrix();
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