using System;
using THREE;
using WebGL;

namespace Demo.THREE
{
    public class LoaderCtmMaterialsForm : BaseForm
    {
        private readonly Scene _scene;
        private readonly PerspectiveCamera _camera;
        private readonly WebGLRenderer _renderer;
        private readonly Mesh _mesh;
        private readonly TrackballControls _controls;
        private readonly Texture _textureCube;
        private readonly PerspectiveCamera _cameraCube;
        private readonly Scene _sceneCube;

        public LoaderCtmMaterialsForm()
        {
            // CAMERA

            _camera = new PerspectiveCamera(25, ClientSize.Width / (double)ClientSize.Height, 1, 10000);
            _camera.position.set(185, 40, 170);

            _controls = new TrackballControls(_camera, this) {dynamicDampingFactor = 0.25};

            // SCENE

            _scene = new Scene();

            // SKYBOX

            _sceneCube = new Scene();
            _cameraCube = new PerspectiveCamera(25, ClientSize.Width / (double)ClientSize.Height, 1, 10000);

            _sceneCube.add(_cameraCube);

            const string r = "textures/cube/pisa/";
            var urls = new JSArray(
                r + "px.png", r + "nx.png",
                r + "py.png", r + "ny.png",
                r + "pz.png", r + "nz.png"
                );

            _textureCube = ImageUtils.loadTextureCube(urls);

            var shader = global::THREE.THREE.ShaderLib["cube"];
            shader.uniforms["tCube"].value = _textureCube;

            var material = new ShaderMaterial(JSObject.create((dynamic)new
                                                                     {
                                                                         shader.fragmentShader,
                                                                         shader.vertexShader,
                                                                         shader.uniforms,
                                                                         depthWrite = false,
                                                                         side = global::THREE.THREE.BackSide
                                                                     }));

            _mesh = new Mesh(new CubeGeometry(100, 100, 100), material);
            _sceneCube.add(_mesh);

            // LIGHTS

            var light = new PointLight(0xffffff, 1);
            light.position.set(2, 5, 1);
            light.position.multiplyScalar(30);
            _scene.add(light);

            ColorUtils.adjustHSV(light.color, 0, -0.75, 0);

            light = new PointLight(0xffffff, 0.75);
            light.position.set(-12, 4.6, 2.4);
            light.position.multiplyScalar(30);
            _scene.add(light);

            ColorUtils.adjustHSV(light.color, 0, -0.5, 0);

            _scene.add(new AmbientLight(0x050505));

            // RENDERER

            _renderer = new WebGLRenderer(new {canvas, antialias = true, clearColor = 0xffffff, clearAlpha = 1});
            _renderer.setSize(ClientSize.Width, ClientSize.Height);
            _renderer.autoClear = false;

            //

            _renderer.gammaInput = true;
            _renderer.gammaOutput = true;
            _renderer.physicallyBasedShading = true;

            // STATS

            //		stats = new Stats();
            //		container.appendChild( stats.domElement );

            // EVENTS

            //		window.addEventListener( 'resize', onWindowResize, false );
            //		window.addEventListener( 'mousemove', onDocumentMouseMove, false );

            // LOADER

            // new way via CTMLoader and separate parts
            var loaderCTM = new CTMLoader(true);
            loaderCTM.loadParts("models/ctm/camaro/camaro.js", (geometries, materials) =>
                                                               {
                                                                   hackMaterials(materials);
                                                                   for (var i = 0; i < geometries.length; i++)
                                                                   {
                                                                       var mesh1 = new Mesh(geometries[i], materials[i])
                                                                                   {
                                                                                       position = new Vector3(-105, -78, -40),
                                                                                       scale = new Vector3(30, 30, 30)
                                                                                   };
                                                                       _scene.add(mesh1);
                                                                   }
                                                               });
        }

        private void hackMaterials(dynamic materials)
        {
            for (var i = 0; i < materials.length; i++)
            {
                var m = materials[i];
                if (m.name.IndexOf("Body") != -1)
                {
                    var mm = new MeshPhongMaterial(JSObject.create(new {m.map})) {envMap = _textureCube, combine = global::THREE.THREE.MixOperation, reflectivity = 0.75};

                    materials[i] = mm;
                }
                else if (m.name.IndexOf("mirror") != -1)
                {
                    var mm = new MeshPhongMaterial(JSObject.create(new {m.map})) {envMap = _textureCube, combine = global::THREE.THREE.MultiplyOperation};

                    materials[i] = mm;
                }
                else if (m.name.IndexOf("glass") != -1)
                {
                    var mm = new MeshPhongMaterial(JSObject.create(new {m.map})) {envMap = _textureCube};

                    mm.color.copy(m.color);
                    mm.combine = global::THREE.THREE.MixOperation;
                    mm.reflectivity = 0.25;
                    mm.opacity = m.opacity;
                    mm.transparent = true;

                    materials[i] = mm;
                }
                else if (m.name.IndexOf("Material.001") != -1)
                {
                    var mm = new MeshPhongMaterial(JSObject.create(new {m.map})) {shininess = 30};

                    mm.color.setHex(0x404040);
                    mm.metal = true;

                    materials[i] = mm;
                }

                materials[i].side = global::THREE.THREE.DoubleSide;
            }
        }

        protected override void onWindowResize(EventArgs eventArgs)
        {
            if (_renderer != null)
            {
                _renderer.setSize(ClientSize.Width, ClientSize.Height);

                _camera.aspect = aspectRatio;
                _camera.updateProjectionMatrix();

                _cameraCube.aspect = aspectRatio;
                _cameraCube.updateProjectionMatrix();
            }
        }

        protected override void render()
        {
            _controls.update();

            _cameraCube.rotation.copy(_camera.rotation);

            _renderer.clear();
            _renderer.render(_sceneCube, _cameraCube);
            _renderer.render(_scene, _camera);
        }
    }
}