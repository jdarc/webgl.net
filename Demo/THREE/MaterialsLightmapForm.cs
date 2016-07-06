using System;
using System.Windows.Forms;
using THREE;
using WebGL;

namespace Demo.THREE
{
    public class MaterialsLightmapForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;
        private readonly Clock clock = new Clock();
        private readonly TrackballControls controls;

        public MaterialsLightmapForm()
        {
            const string vertexShader =
                @"
				varying vec3 vWorldPosition;

				void main() {

					vec4 worldPosition = modelMatrix * vec4( position, 1.0 );
					vWorldPosition = worldPosition.xyz;

					gl_Position = projectionMatrix * modelViewMatrix * vec4( position, 1.0 );

				}";

            const string fragmentShader =
                @"
				uniform vec3 topColor;
				uniform vec3 bottomColor;
				uniform float offset;
				uniform float exponent;

				varying vec3 vWorldPosition;

				void main() {

					float h = normalize( vWorldPosition + offset ).y;
					gl_FragColor = vec4( mix( bottomColor, topColor, max( pow( h, exponent ), 0.0 ) ), 1.0 );

				}";

            // CAMERA

            camera = new PerspectiveCamera(40, aspectRatio, 1, 10000)
                     {
                         position = {x = 700, z = -500, y = 180}
                     };

            // SCENE

            scene = new Scene {fog = new Fog(0xfafafa, 1000, 10000)};
            scene.fog.color.setHSV(0.6, 0.125, 1);

            // CONTROLS

            controls = new TrackballControls(camera, this) {target = {z = 150}};

            // LIGHTS

            var directionalLight = new DirectionalLight(0xffffff, 1.475);
            directionalLight.position.set(100, 100, -100);
            scene.add(directionalLight);

            var hemiLight = new HemisphereLight(0xffffff, 0xffffff, 1.25);
            hemiLight.color.setHSV(0.6, 0.45, 1);
            hemiLight.groundColor.setHSV(0.1, 0.45, 0.95);
            hemiLight.position.y = 500;
            scene.add(hemiLight);

            // SKYDOME

            var uniforms = JSObject.create(new
                                         {
                                             topColor = new {type = "c", value = new Color(0x0077ff)},
                                             bottomColor = new {type = "c", value = new Color(0xffffff)},
                                             offset = new {type = "f", value = 400.0},
                                             exponent = new {type = "f", value = 0.6}
                                         });

            uniforms.topColor.value.copy(hemiLight.color);

            scene.fog.color.copy(uniforms.bottomColor.value);

            var skyGeo = new SphereGeometry(4000, 32, 15);
            var skyMat = new ShaderMaterial(JSObject.create(new
                                                          {
                                                              vertexShader,
                                                              fragmentShader,
                                                              uniforms,
                                                              side = global::THREE.THREE.BackSide
                                                          }));

            var sky = new Mesh(skyGeo, skyMat);
            scene.add(sky);

            // RENDERER

            renderer = new WebGLRenderer(new {canvas, antialias = true, alpha = false, clearColor = 0xfafafa, clearAlpha = 1});
            renderer.setSize(ClientSize.Width, ClientSize.Height);

            renderer.setClearColor(scene.fog.color, 1);

            renderer.gammaInput = true;
            renderer.gammaOutput = true;
            renderer.physicallyBasedShading = true;

            // MODEL

            var loader = new JSONLoader();
            Action<object, object> callback = (geometry, materials) => createScene(geometry, materials, 0.0, 0.0, 0.0, 100.0);
            loader.load("objects/lightmap/lightmap.js", callback);
        }

        private void createScene(dynamic geometry, dynamic materials, double x, double y, double z, double s)
        {
            var mesh = new Mesh(geometry, new MeshFaceMaterial(materials));
            mesh.position.set(x, y, z);
            mesh.scale.set(s, s, s);
            scene.add(mesh);
        }

        protected override void onWindowResize(EventArgs e)
        {
            if (camera != null)
            {
                camera.aspect = aspectRatio;
                camera.updateProjectionMatrix();

                renderer.setSize(ClientSize.Width, ClientSize.Height);
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
            var delta = clock.getDelta();
            controls.update(delta);

            renderer.render(scene, camera);
        }
    }
}