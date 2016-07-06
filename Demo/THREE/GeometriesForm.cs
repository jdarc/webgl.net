using System;
using THREE;
using WebGL;
using Math = System.Math;

namespace Demo.THREE
{
    public class GeometriesForm : BaseForm
    {
        private readonly WebGLRenderer _renderer;
        private readonly PerspectiveCamera _camera;
        private readonly Scene _scene;

        public GeometriesForm()
        {
            _camera = new PerspectiveCamera(45, aspectRatio, 1, 2000);
            _camera.position.y = 400;

            _scene = new Scene();

            _scene.add(new AmbientLight(0x404040));

            var light = new DirectionalLight(0xffffff);
            light.position.set(0, 1, 0);
            _scene.add(light);

            var map = ImageUtils.loadTexture("textures/ash_uvgrid01.jpg");
            map.wrapS = map.wrapT = global::THREE.THREE.RepeatWrapping;
            map.anisotropy = 16;

            var materials = new JSArray(
                new MeshLambertMaterial(JSObject.create(new {ambient = 0xbbbbbb, map, side = global::THREE.THREE.DoubleSide})),
                new MeshBasicMaterial(JSObject.create(new {color = 0xffffff, wireframe = true, transparent = true, opacity = 0.1, side = global::THREE.THREE.DoubleSide})));

            var obj = SceneUtils.createMultiMaterialObject(new CubeGeometry(100, 100, 100, 4, 4, 4), materials);
            obj.position.set(-200, 0, 400);
            _scene.add(obj);

            obj = SceneUtils.createMultiMaterialObject(new CylinderGeometry(25, 75, 100, 40, 5), materials);
            obj.position.set(0, 0, 400);
            _scene.add(obj);

            obj = SceneUtils.createMultiMaterialObject(new IcosahedronGeometry(75, 1), materials);
            obj.position.set(-200, 0, 200);
            _scene.add(obj);

            obj = SceneUtils.createMultiMaterialObject(new OctahedronGeometry(75, 2), materials);
            obj.position.set(0, 0, 200);
            _scene.add(obj);

            obj = SceneUtils.createMultiMaterialObject(new TetrahedronGeometry(75, 0), materials);
            obj.position.set(200, 0, 200);
            _scene.add(obj);

            obj = SceneUtils.createMultiMaterialObject(new PlaneGeometry(100, 100, 4, 4), materials);
            obj.position.set(-200, 0, 0);
            _scene.add(obj);

            var object2 = SceneUtils.createMultiMaterialObject(new CircleGeometry(50, 10, 0, Math.PI), materials);
            object2.rotation.x = (float)(Math.PI / 2);
            obj.add(object2);

            obj = SceneUtils.createMultiMaterialObject(new SphereGeometry(75, 20, 10), materials);
            obj.position.set(0, 0, 0);
            _scene.add(obj);

            var points = new JSArray();

            for (var i = 0; i < 50; i ++)
            {
                points.push(new Vector3((float)(Math.Sin(i * 0.2) * 15 + 50), 0, (i - 5) * 2));
            }

            obj = SceneUtils.createMultiMaterialObject(new LatheGeometry(points, 20), materials);
            obj.position.set(200, 0, 0);
            _scene.add(obj);

            obj = SceneUtils.createMultiMaterialObject(new TorusGeometry(50, 20, 20, 20), materials);
            obj.position.set(-200, 0, -200);
            _scene.add(obj);

            obj = SceneUtils.createMultiMaterialObject(new TorusKnotGeometry(50, 10, 50, 20), materials);
            obj.position.set(0, 0, -200);
            _scene.add(obj);

            obj = new AxisHelper(50);
            obj.position.set(200, 0, -200);
            _scene.add(obj);

            obj = new ArrowHelper(new Vector3(0, 1, 0), new Vector3(0, 0, 0), 50);
            obj.position.set(200, 0, 400);
            _scene.add(obj);

            _renderer = new WebGLRenderer(new {canvas, antialias = true});
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

        protected override void render()
        {
            var timer = JSDate.now() * 0.0001;

            _camera.position.x = (float)(Math.Cos(timer) * 800);
            _camera.position.z = (float)(Math.Sin(timer) * 800);

            _camera.lookAt(_scene.position);

            for (var i = 0; i < _scene.children.length; i++)
            {
                var obj = _scene.children[i];

                obj.rotation.x = timer * 5.0;
                obj.rotation.y = timer * 2.5;
            }

            _renderer.render(_scene, _camera);
        }
    }
}