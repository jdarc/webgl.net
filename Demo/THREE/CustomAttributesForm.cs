using System;
using THREE;
using WebGL;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class CustomAttributesForm : BaseForm
    {
        private readonly WebGLRenderer _renderer;
        private readonly PerspectiveCamera _camera;
        private readonly Scene _scene;
        private readonly Mesh _sphere;
        private readonly double[] _noise;
        private readonly dynamic _attributes;
        private readonly dynamic _uniforms;

        private const string VertexShader =
            @"
			uniform float amplitude;

			attribute float displacement;

			varying vec3 vNormal;
			varying vec2 vUv;

			void main() {

				vNormal = normal;
				vUv = ( 0.5 + amplitude ) * uv + vec2( amplitude );

				vec3 newPosition = position + amplitude * normal * vec3( displacement );
				gl_Position = projectionMatrix * modelViewMatrix * vec4( newPosition, 1.0 );

			}";

        private const string Fragmentshader =
            @"
			varying vec3 vNormal;
			varying vec2 vUv;

			uniform vec3 color;
			uniform sampler2D texture;

			void main() {

				vec3 light = vec3( 0.5, 0.2, 1.0 );
				light = normalize( light );

				float dProd = dot( vNormal, light ) * 0.5 + 0.5;

				vec4 tcolor = texture2D( texture, vUv );
				vec4 gray = vec4( vec3( tcolor.r * 0.3 + tcolor.g * 0.59 + tcolor.b * 0.11 ), 1.0 );

				gl_FragColor = gray * vec4( vec3( dProd ) * vec3( color ), 1.0 );

			}";

        public CustomAttributesForm()
        {
            _camera = new PerspectiveCamera(30, aspectRatio, 1.0, 10000.0) {position = {z = 300}};

            _scene = new Scene();

            _attributes = JSObject.create((dynamic)new
                                                 {
                                                     displacement = new {type = "f", value = new JSArray()}
                                                 });

            _uniforms = JSObject.create((dynamic)new
                                               {
                                                   amplitude = new {type = "f", value = 1.0},
                                                   color = new {type = "c", value = new Color(0xff2200)},
                                                   texture = new {type = "t", value = ImageUtils.loadTexture("textures/water.jpg")},
                                               });

            _uniforms.texture.value.wrapS = _uniforms.texture.value.wrapT = global::THREE.THREE.RepeatWrapping;

            var shaderMaterial = new ShaderMaterial(JSObject.create((dynamic)new
                                                                           {
                                                                               uniforms = _uniforms,
                                                                               attributes = _attributes,
                                                                               vertexShader = VertexShader,
                                                                               fragmentShader = Fragmentshader
                                                                           }));

            const double radius = 50.0;
            const int segments = 128;
            const int rings = 64;
            var geometry = new SphereGeometry(radius, segments, rings) {dynamic = true};

            _sphere = new Mesh(geometry, shaderMaterial);

            var vertices = _sphere.geometry.vertices;
            var values = _attributes.displacement.value;

            _noise = new double[vertices.length];
            for (var v = 0; v < vertices.length; v++)
            {
                values[v] = 0.0;
                _noise[v] = Math.random() * 5.0;
            }

            _scene.add(_sphere);

            _renderer = new WebGLRenderer(new {canvas, clearColor = 0x050505, clearAlpha = 1});
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

        protected override void render()
        {
            var time = JSDate.now() * 0.01;

            _sphere.rotation.y = _sphere.rotation.z = 0.01 * time;

            _uniforms.amplitude.value = 2.5 * System.Math.Sin(_sphere.rotation.y * 0.125);
            ColorUtils.adjustHSV(_uniforms.color.value, 0.0005, 0.0, 0.0);

            for (var i = 0; i < _attributes.displacement.value.length; i++)
            {
                _attributes.displacement.value[i] = System.Math.Sin(0.1 * i + time);

                _noise[i] += 0.5 * (0.5 - Math.random());
                _noise[i] = Math.clamp(_noise[i], -5.0, 5.0);

                _attributes.displacement.value[i] += _noise[i];
            }

            _attributes.displacement.needsUpdate = true;

            _renderer.render(_scene, _camera);
        }
    }
}