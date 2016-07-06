using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class MorphTargetsHorseForm : BaseForm
    {
        private const double Duration = 1000.0;
        private const double Radius = 600.0;
        private const double Interpolation = Duration / Keyframes;
        private const int Keyframes = 15;

        private readonly WebGLRenderer _renderer;
        private readonly dynamic _camera;
        private readonly Scene _scene;
        private Mesh _mesh;
        private double _theta;
        private int _lastKeyframe;
        private int _currentKeyframe;

        public MorphTargetsHorseForm()
        {
            _camera = new PerspectiveCamera(50, aspectRatio, 1, 10000);
            _camera.position.y = 300;
            _camera.target = new Vector3(0, 150, 0);

            _scene = new Scene();

            var light = new DirectionalLight(0xefefff, 2);
            light.position.set(1, 1, 1).normalize();
            _scene.add(light);

            light = new DirectionalLight(0xffefef, 2);
            light.position.set(-1, -1, -1).normalize();
            _scene.add(light);

            var loader = new JSONLoader(true);
            loader.load("models/animated/horse.js", (geometry, texture) =>
                                                    {
                                                        var parameters = JSObject.create(new {color = 0x606060, morphTargets = true});
                                                        _mesh = new Mesh(geometry, new MeshLambertMaterial(parameters));
                                                        _mesh.scale.set(1.5, 1.5, 1.5);
                                                        _scene.add(_mesh);
                                                    });

            _renderer = new WebGLRenderer(new {canvas, clearColor = 0xf0f0f0}) {sortObjects = false};
            _renderer.setSize(ClientSize.Width, ClientSize.Height);
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

        protected override void onMouseClick(MouseEventArgs e)
        {
        }

        protected override void onMouseMove(MouseEventArgs e)
        {
        }

        protected override void render()
        {
            _theta += 0.1;

            _camera.position.x = Radius * System.Math.Sin(Math.degToRad(_theta));
            _camera.position.z = Radius * System.Math.Cos(Math.degToRad(_theta));

            _camera.lookAt(_camera.target);

            if (_mesh != null)
            {
                // Alternate morph targets
                var time = JSDate.now() % Duration;

                var keyframe = (int)System.Math.Floor(time / Interpolation);

                if (keyframe != _currentKeyframe)
                {
                    _mesh.morphTargetInfluences[_lastKeyframe] = 0;
                    _mesh.morphTargetInfluences[_currentKeyframe] = 1;
                    _mesh.morphTargetInfluences[keyframe] = 0;

                    _lastKeyframe = _currentKeyframe;
                    _currentKeyframe = keyframe;
                }

                _mesh.morphTargetInfluences[keyframe] = (time % Interpolation) / Interpolation;
                _mesh.morphTargetInfluences[_lastKeyframe] = 1 - _mesh.morphTargetInfluences[keyframe];
            }

            _renderer.render(_scene, _camera);
        }
    }
}