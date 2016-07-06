using System;
using THREE;

namespace Demo.THREE
{
    public class GeometryCubeForm : BaseForm
    {
        private readonly WebGLRenderer _renderer;
        private readonly PerspectiveCamera _camera;
        private readonly Scene _scene;
        private readonly Mesh _mesh;

        public GeometryCubeForm()
        {
            _renderer = new WebGLRenderer(new {canvas});
            _renderer.setSize(ClientSize.Width, ClientSize.Height);

            _camera = new PerspectiveCamera(70, aspectRatio, 1, 1000);
            _camera.position.z = 400;

            _scene = new Scene();

            var geometry = new CubeGeometry(200, 200, 200);

            var texture = ImageUtils.loadTexture("textures/crate.gif");
            texture.anisotropy = _renderer.getMaxAnisotropy();

            var material = new MeshBasicMaterial {map = texture};

            _mesh = new Mesh(geometry, material);
            _scene.add(_mesh);
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
            _mesh.rotation.x += 0.005f;
            _mesh.rotation.y += 0.01f;
            _renderer.render(_scene, _camera);
        }
    }
}