using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = System.Math;

namespace Demo.THREE
{
    public class AnimationSkinningForm : BaseForm
    {
        private const int dstep = -10;
        private readonly Clock _clock = new Clock();
        private readonly WebGLRenderer _renderer;
        private readonly PerspectiveCamera _camera;
        private readonly Scene _scene;
        private readonly JSArray _offset = new JSArray();
        private readonly DirectionalLight _light;
        private readonly Object3D _floor;
        private JSArray _buffalos;
        private JSArray _animations;
        private bool _playback;
        private int _mouseX;
        private int _mouseY;
        private int _dz;

        public AnimationSkinningForm()
        {
            _camera = new PerspectiveCamera(25, aspectRatio, 1, 10000);
            _camera.position.set(0, 185, 2500);

            _scene = new Scene();
            _scene.fog = new FogExp2(0xffffff, 0.0003);
            _scene.fog.color.setHSV(0.1, 0.10, 1);

            _light = new DirectionalLight(0xffffff, 1.5);
            _light.position.set(0, 1, 1).normalize();
            _scene.add(_light);

            var planeSimple = new PlaneGeometry(200, 300);
            var planeTesselated = new PlaneGeometry(100, 300, 25, 40);
            var matWire = new MeshBasicMaterial(JSObject.create(new {color = 0x110000, wireframe = true, wireframeLinewidth = 2}));
            var matSolid = new MeshBasicMaterial(JSObject.create(new {color = 0x330000}));
            matSolid.color.setHSV(0.1, 0.75, 1);

            _floor = new Mesh(planeSimple, matSolid);
            _floor.position.y = -10;
            _floor.rotation.x = - Math.PI / 2;
            _floor.scale.set(25, 25, 25);
            _scene.add(_floor);

            _floor = new Mesh(planeTesselated, matWire);
            _floor.rotation.x = - Math.PI / 2;
            _floor.scale.set(25, 25, 25);
            _scene.add(_floor);

            _renderer = new WebGLRenderer(new {canvas, clearColor = 0xffffff, clearAlpha = 1, antialias = true});
            _renderer.setSize(ClientSize.Width, ClientSize.Height);
            _renderer.setClearColor(_scene.fog.color, 1);
            _renderer.sortObjects = false;

            var loader = new JSONLoader();
            loader.load("objects/buffalo/buffalo.js", createScene);
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
            startAnimation();
        }

        protected override void onMouseMove(MouseEventArgs e)
        {
            _mouseX = (int)(e.X - windowHalf.X);
            _mouseY = (int)(e.Y - windowHalf.Y);
        }

        private void createScene(dynamic geometry, dynamic materials)
        {
            _buffalos = new JSArray();
            _animations = new JSArray();

            SkinnedMesh buffalo;
            const int gridx = 25;
            const int gridz = 15;
            const int sepx = 150;
            const int sepz = 300;

            var material = new MeshFaceMaterial(materials);

            var originalMaterial = materials[0];

            originalMaterial.skinning = true;
            originalMaterial.transparent = true;
            originalMaterial.alphaTest = 0.75;

            AnimationHandler.add(geometry.animation);

            for (var x = 0; x < gridx; x++)
            {
                for (var z = 0; z < gridz; z++)
                {
                    buffalo = new SkinnedMesh(geometry, material, false);

                    buffalo.position.x = -(gridx - 1) * sepx * 0.5 + x * sepx + global::THREE.Math.random() * 0.5 * sepx;
                    buffalo.position.z = -(gridz - 1) * sepz * 0.5 + z * sepz + global::THREE.Math.random() * 0.5 * sepz - 500;

                    buffalo.position.y = buffalo.geometry.boundingSphere.radius * 0.5;
                    buffalo.rotation.y = 0.2 - global::THREE.Math.random() * 0.4;

                    _scene.add(buffalo);

                    _buffalos.push(buffalo);

                    var animation = new Animation(buffalo, "take_001");
                    _animations.push(animation);

                    _offset.push(global::THREE.Math.random());
                }
            }
        }

        protected override void render()
        {
            var delta = _clock.getDelta();

            AnimationHandler.update(delta);

            _camera.position.x += (_mouseX - _camera.position.x) * 0.05;
            _camera.lookAt(_scene.position);

            if (_buffalos != null && _playback)
            {
                var elapsed = _clock.getElapsedTime();

                _camera.position.z += 2.0 * Math.Sin(elapsed);

                for (var i = 0; i < _buffalos.length; i++)
                {
                    _buffalos[i].position.z += 2.0 * Math.Sin(elapsed + _offset[i]);
                }
            }

            _floor.position.z += _dz;
            if (_floor.position.z < -500)
            {
                _floor.position.z = 0;
            }

            _renderer.render(_scene, _camera);
        }

        private void startAnimation()
        {
            for (var i = 0; i < _animations.length; i++)
            {
                _animations[i].offset = 0.05 * global::THREE.Math.random();
                _animations[i].play();
            }

            _dz = dstep;
            _playback = true;
        }
    }
}