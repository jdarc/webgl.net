using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = System.Math;

namespace Demo.THREE
{
    public class CameraForm : BaseForm
    {
        private readonly Scene _scene;
        private readonly dynamic _camera;
        private readonly PerspectiveCamera _cameraPerspective;
        private readonly CameraHelper _cameraPerspectiveHelper;
        private readonly OrthographicCamera _cameraOrtho;
        private readonly CameraHelper _cameraOrthoHelper;
        private Camera _activeCamera;
        private CameraHelper _activeHelper;
        private readonly Object3D _cameraRig;
        private readonly Mesh _mesh;
        private readonly WebGLRenderer _renderer;

        public CameraForm()
        {
            _scene = new Scene();

            _camera = new PerspectiveCamera(50, 0.5 * aspectRatio, 1, 10000);
            _camera.position.z = 2500;

            _cameraPerspective = new PerspectiveCamera(50, 0.5 * aspectRatio, 150, 1000);

            _cameraPerspectiveHelper = new CameraHelper(_cameraPerspective);
            _scene.add(_cameraPerspectiveHelper);

            _cameraOrtho = new OrthographicCamera(0.5 * ClientSize.Width / -2, 0.5 * ClientSize.Width / 2.0, ClientSize.Height / 2.0, ClientSize.Height / -2.0, 150, 1000);

            _cameraOrthoHelper = new CameraHelper(_cameraOrtho);
            _scene.add(_cameraOrthoHelper);

            _activeCamera = _cameraPerspective;
            _activeHelper = _cameraPerspectiveHelper;

            // counteract different front orientation of cameras vs rig

            _cameraOrtho.rotation.y = Math.PI;
            _cameraPerspective.rotation.y = Math.PI;

            _cameraRig = new Object3D();

            _cameraRig.add(_cameraPerspective);
            _cameraRig.add(_cameraOrtho);

            _scene.add(_cameraRig);

            _mesh = new Mesh(new SphereGeometry(100, 16, 8), new MeshBasicMaterial(JSObject.create(new {color = 0xffffff, wireframe = true})));
            _scene.add(_mesh);

            var mesh2 = new Mesh(new SphereGeometry(50, 16, 8), new MeshBasicMaterial(JSObject.create(new {color = 0x00ff00, wireframe = true})));
            mesh2.position.y = 150;
            _mesh.add(mesh2);

            var mesh3 = new Mesh(new SphereGeometry(5, 16, 8), new MeshBasicMaterial(JSObject.create(new {color = 0x0000ff, wireframe = true})));
            mesh3.position.z = 150;
            _cameraRig.add(mesh3);

            var geometry = new Geometry();

            for (var i = 0; i < 10000; i ++)
            {
                var vertex = new Vector3();
                vertex.x = global::THREE.Math.randFloatSpread(2000);
                vertex.y = global::THREE.Math.randFloatSpread(2000);
                vertex.z = global::THREE.Math.randFloatSpread(2000);

                geometry.vertices.push(vertex);
            }

            var particles = new ParticleSystem(geometry, new ParticleBasicMaterial(JSObject.create(new {color = 0x888888})));
            _scene.add(particles);

            _renderer = new WebGLRenderer(new {canvas, antialias = true, clearColor = 0x000000, clearAlpha = 1});
            _renderer.setSize(ClientSize.Width, ClientSize.Height);

            _renderer.autoClear = false;
        }

        protected override void onKeyDown(KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.O: /*O*/

                    _activeCamera = _cameraOrtho;
                    _activeHelper = _cameraOrthoHelper;

                    break;

                case Keys.P: /*P*/

                    _activeCamera = _cameraPerspective;
                    _activeHelper = _cameraPerspectiveHelper;

                    break;
            }
        }

        protected override void onWindowResize(EventArgs eventArgs)
        {
            if (_camera != null)
            {
                _renderer.setSize(ClientSize.Width, ClientSize.Height);

                _camera.aspect = 0.5 * aspectRatio;
                _camera.updateProjectionMatrix();

                _cameraPerspective.aspect = 0.5 * aspectRatio;
                _cameraPerspective.updateProjectionMatrix();

                _cameraOrtho.left = -0.5 * ClientSize.Width / 2.0;
                _cameraOrtho.right = 0.5 * ClientSize.Width / 2.0;
                _cameraOrtho.top = ClientSize.Height / 2.0;
                _cameraOrtho.bottom = -ClientSize.Height / 2.0;
                _cameraOrtho.updateProjectionMatrix();
            }
        }

        protected override void render()
        {
            var r = JSDate.now() * 0.0005;

            _mesh.position.x = 700 * Math.Cos(r);
            _mesh.position.z = 700 * Math.Sin(r);
            _mesh.position.y = 700 * Math.Sin(r);

            _mesh.children[0].position.x = 70 * Math.Cos(2 * r);
            _mesh.children[0].position.z = 70 * Math.Sin(r);

            if (_activeCamera == _cameraPerspective)
            {
                _cameraPerspective.fov = 35 + 30 * Math.Sin(0.5 * r);
                _cameraPerspective.far = _mesh.position.length();
                _cameraPerspective.updateProjectionMatrix();

                _cameraPerspectiveHelper.update();
                _cameraPerspectiveHelper.visible = true;

                _cameraOrthoHelper.visible = false;
            }
            else
            {
                _cameraOrtho.far = _mesh.position.length();
                _cameraOrtho.updateProjectionMatrix();

                _cameraOrthoHelper.update();
                _cameraOrthoHelper.visible = true;

                _cameraPerspectiveHelper.visible = false;
            }

            _cameraRig.lookAt(_mesh.position);

            _renderer.clear();

            _activeHelper.visible = false;

            _renderer.setViewport(0, 0, ClientSize.Width / 2, ClientSize.Height);
            _renderer.render(_scene, _activeCamera);

            _activeHelper.visible = true;

            _renderer.setViewport(ClientSize.Width / 2, 0, ClientSize.Width / 2, ClientSize.Height);
            _renderer.render(_scene, _camera);
        }
    }
}