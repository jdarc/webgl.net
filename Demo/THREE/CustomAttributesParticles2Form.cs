using System;
using System.Windows.Forms;
using THREE;
using WebGL;

namespace Demo.THREE
{
    public class CustomAttributesParticles2Form : BaseForm
    {
        private const string Vertexshader =
            @"
			attribute float size;
			attribute vec3 ca;

			varying vec3 vColor;

			void main() {

				vColor = ca;

				vec4 mvPosition = modelViewMatrix * vec4( position, 1.0 );

				//gl_PointSize = size;
				gl_PointSize = size * ( 300.0 / length( mvPosition.xyz ) );

				gl_Position = projectionMatrix * mvPosition;

			}";

        private const string Fragmentshader =
            @"
			uniform vec3 color;
			uniform sampler2D texture;

			varying vec3 vColor;

			void main() {

				gl_FragColor = vec4( color * vColor, 1.0 );
				gl_FragColor = gl_FragColor * texture2D( texture, gl_PointCoord );

			}";

        private readonly WebGLRenderer _renderer;
        private readonly PerspectiveCamera _camera;
        private readonly Scene _scene;
        private dynamic _attributes;
        private dynamic _uniforms;
        private dynamic _sphere;
        private int _vc1;

        public CustomAttributesParticles2Form()
        {
            _camera = new PerspectiveCamera(45, aspectRatio, 1, 10000) {position = {z = 300}};

            _scene = new Scene();

            _attributes = JSObject.create(new
                                        {
                                            size = new {type = "f", value = new JSArray()},
                                            ca = new {type = "c", value = new JSArray()}
                                        });

            _uniforms = JSObject.create(new
                                      {
                                          amplitude = new {type = "f", value = 1.0},
                                          color = new {type = "c", value = new Color(0xffffff)},
                                          texture = new {type = "t", value = ImageUtils.loadTexture("textures/sprites/disc.png")},
                                      });

            _uniforms.texture.value.wrapS = _uniforms.texture.value.wrapT = global::THREE.THREE.RepeatWrapping;

            var shaderMaterial = new ShaderMaterial(JSObject.create(new
                                                                  {
                                                                      uniforms = _uniforms,
                                                                      attributes = _attributes,
                                                                      vertexShader = Vertexshader,
                                                                      fragmentShader = Fragmentshader,
                                                                      transparent = true
                                                                  }));

            const double radius = 100.0;
            const int segments = 68;
            const int rings = 38;
            var geometry = new SphereGeometry(radius, segments, rings);

            _vc1 = geometry.vertices.length;

            var geometry2 = new CubeGeometry(0.8 * radius, 0.8 * radius, 0.8 * radius, 10, 10, 10);

            GeometryUtils.merge(geometry, geometry2);

            _sphere = new ParticleSystem(geometry, shaderMaterial);

            _sphere.dynamic = true;
            _sphere.sortParticles = true;

            var vertices = _sphere.geometry.vertices;
            var valuesSize = _attributes.size.value;
            var valuesColor = _attributes.ca.value;

            for (var v = 0; v < vertices.length; v++)
            {
                valuesSize[v] = 10;
                valuesColor[v] = new Color(0xffffff);

                if (v < _vc1)
                {
                    valuesColor[v].setHSV(0.01 + 0.1 * (v / (double)_vc1), 0.99, (vertices[v].y + radius) / (2 * radius));
                }
                else
                {
                    valuesSize[v] = 40;
                    valuesColor[v].setHSV(0.6, 0.75, 0.5 + vertices[v].y / (0.8 * radius));
                }
            }

            _scene.add(_sphere);

            _renderer = new WebGLRenderer(new {canvas, clearColor = 0x000000, clearAlpha = 1});
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
            var time = JSDate.now() * 0.005;

            _sphere.rotation.y = 0.02 * time;
            _sphere.rotation.z = 0.02 * time;

            for (var i = 0; i < _attributes.size.value.length; i++)
            {
                if (i < _vc1)
                {
                    _attributes.size.value[i] = 16 + 12 * System.Math.Sin(0.1 * i + time);
                }
            }

            _attributes.size.needsUpdate = true;

            _renderer.render(_scene, _camera);
        }
    }
}