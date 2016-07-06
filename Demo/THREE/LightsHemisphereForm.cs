using System;
using System.Windows.Forms;
using THREE;
using WebGL;

namespace Demo.THREE
{
    public class LightsHemisphereForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;
        private readonly DirectionalLight dirLight;
        private readonly HemisphereLight hemiLight;
        private readonly JSArray morphs = new JSArray();
        private readonly Clock clock = new Clock();

        public LightsHemisphereForm()
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

            camera = new PerspectiveCamera(30, aspectRatio, 1, 5000);
            camera.position.set(0, 0, 250);

            scene = new Scene();

            scene.fog = new Fog(0xffffff, 1, 5000);
            scene.fog.color.setHSV(0.6, 0, 1);

            // LIGHTS

            hemiLight = new HemisphereLight(0xffffff, 0xffffff, 0.6);
            hemiLight.color.setHSV(0.6, 0.75, 1);
            hemiLight.groundColor.setHSV(0.095, 0.5, 1);
            hemiLight.position.set(0, 500, 0);
            scene.add(hemiLight);

            //

            dirLight = new DirectionalLight(0xffffff, 1);
            dirLight.color.setHSV(0.1, 0.1, 1);
            dirLight.position.set(-1, 1.75, 1);
            dirLight.position.multiplyScalar(50);
            scene.add(dirLight);

            dirLight.castShadow = true;

            dirLight.shadowMapWidth = 2048;
            dirLight.shadowMapHeight = 2048;

            var d = 50;

            dirLight.shadowCameraLeft = -d;
            dirLight.shadowCameraRight = d;
            dirLight.shadowCameraTop = d;
            dirLight.shadowCameraBottom = -d;

            dirLight.shadowCameraFar = 3500;
            dirLight.shadowBias = -0.0001;
            dirLight.shadowDarkness = 0.35;
            //dirLight.shadowCameraVisible = true;

            // GROUND

            var groundGeo = new PlaneGeometry(10000, 10000);
            var groundMat = new MeshPhongMaterial(JSObject.create(new {ambient = 0xffffff, color = 0xffffff, specular = 0x050505}));
            groundMat.color.setHSV(0.095, 0.5, 1);

            var ground = new Mesh(groundGeo, groundMat);
            ground.rotation.x = -System.Math.PI / 2;
            ground.position.y = -33;
            scene.add(ground);

            ground.receiveShadow = true;

            // SKYDOME

            var uniforms = JSObject.create((dynamic)new
                                                  {
                                                      topColor = new {type = "c", value = new Color(0x0077ff)},
                                                      bottomColor = new {type = "c", value = new Color(0xffffff)},
                                                      offset = new {type = "f", value = 33.0},
                                                      exponent = new {type = "f", value = 0.6}
                                                  });
            uniforms.topColor.value.copy(hemiLight.color);

            scene.fog.color.copy(uniforms.bottomColor.value);

            var skyGeo = new SphereGeometry(4000, 32, 15);
            var skyMat = new ShaderMaterial(JSObject.create(new {vertexShader, fragmentShader, uniforms, side = global::THREE.THREE.BackSide}));

            var sky = new Mesh(skyGeo, skyMat);
            scene.add(sky);

            // MODEL

            var loader = new JSONLoader();

            loader.load("models/animated/flamingo.js", (geometry, igonre) =>
                                                       {
                                                           morphColorsToFaceColors(geometry);
                                                           geometry.computeMorphNormals();

                                                           var material =
                                                               new MeshPhongMaterial(
                                                                   JSObject.create(new {color = 0xffffff, specular = 0xffffff, shininess = 20, morphTargets = true, morphNormals = true, vertexColors = global::THREE.THREE.FaceColors, shading = global::THREE.THREE.FlatShading}));
                                                           var meshAnim = new MorphAnimMesh(geometry, material);

                                                           meshAnim.duration = 1000;

                                                           var s = 0.35;
                                                           meshAnim.scale.set(s, s, s);
                                                           meshAnim.position.y = 15;
                                                           meshAnim.rotation.y = -1;

                                                           meshAnim.castShadow = true;
                                                           meshAnim.receiveShadow = true;

                                                           scene.add(meshAnim);
                                                           morphs.push(meshAnim);
                                                       });

            // RENDERER

            renderer = new WebGLRenderer(new {canvas, antialias = true});
            renderer.setSize(ClientSize.Width, ClientSize.Height);

            renderer.setClearColor(scene.fog.color, 1);

            renderer.gammaInput = true;
            renderer.gammaOutput = true;
            renderer.physicallyBasedShading = true;

            renderer.shadowMapEnabled = true;
            renderer.shadowMapCullFace = global::THREE.THREE.CullFaceBack;
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

        protected override void onKeyDown(KeyEventArgs keyEventArgs)
        {
            switch (keyEventArgs.KeyCode)
            {
                case Keys.H: /*h*/

                    hemiLight.visible = !hemiLight.visible;
                    break;

                case Keys.D: /*d*/

                    dirLight.visible = !dirLight.visible;
                    break;
            }
        }

        protected override void render()
        {
            var delta = clock.getDelta();

            for (var i = 0; i < morphs.length; i++)
            {
                morphs[i].updateAnimation(1000.0 * delta);
            }

            renderer.render(scene, camera);
        }

        private void morphColorsToFaceColors(Geometry geometry)
        {
            if (geometry.morphColors != null && geometry.morphColors.length > 0)
            {
                var colorMap = geometry.morphColors[0];

                for (var i = 0; i < colorMap.colors.length; i ++)
                {
                    geometry.faces[i].color = colorMap.colors[i];
                    ColorUtils.adjustHSV(geometry.faces[i].color, 0, -0.1, 0);
                }
            }
        }
    }
}