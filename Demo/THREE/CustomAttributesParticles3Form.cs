using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class CustomAttributesParticles3Form : BaseForm
    {
        private const string Vertexshader =
            @"
			attribute float size;
			attribute vec4 ca;

			varying vec4 vColor;

			void main() {

				vColor = ca;

				vec4 mvPosition = modelViewMatrix * vec4( position, 1.0 );

				gl_PointSize = size * ( 150.0 / length( mvPosition.xyz ) );

				gl_Position = projectionMatrix * mvPosition;

			}";

        private const string Fragmentshader =
            @"
			uniform vec3 color;
			uniform sampler2D texture;

			varying vec4 vColor;

			void main() {

				vec4 outColor = texture2D( texture, gl_PointCoord );

				if ( outColor.a < 0.5 ) discard;

				gl_FragColor = outColor * vec4( color * vColor.xyz, 1.0 );

				float depth = gl_FragCoord.z / gl_FragCoord.w;
				const vec3 fogColor = vec3( 0.0 );

				float fogFactor = smoothstep( 200.0, 600.0, depth );
				gl_FragColor = mix( gl_FragColor, vec4( fogColor, gl_FragColor.w ), fogFactor );

			}";

        private readonly WebGLRenderer _renderer;
        private readonly PerspectiveCamera _camera;
        private readonly Scene _scene;
        private dynamic _obj;
        private dynamic _attributes;
        private dynamic _uniforms;
        private int _vc1;

        public CustomAttributesParticles3Form()
        {
            _camera = new PerspectiveCamera(40, aspectRatio, 1, 1000) {position = {z = 500}};

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
                                          texture = new {type = "t", value = ImageUtils.loadTexture("textures/sprites/ball.png")},
                                      });

            _uniforms.texture.value.wrapS = _uniforms.texture.value.wrapT = global::THREE.THREE.RepeatWrapping;

            var shaderMaterial = new ShaderMaterial(JSObject.create(new
                                                                  {
                                                                      uniforms = _uniforms,
                                                                      attributes = _attributes,
                                                                      vertexShader = Vertexshader,
                                                                      fragmentShader = Fragmentshader
                                                                  }));

            double radius = 100, inner = 0.6 * radius;
            var geometry = new Geometry();

            for (var i = 0; i < 100000; i++)
            {
                var vertex = new Vector3
                             {
                                 x = Math.random() * 2 - 1,
                                 y = Math.random() * 2 - 1,
                                 z = Math.random() * 2 - 1
                             };
                vertex.multiplyScalar(radius);

                if ((vertex.x > inner || vertex.x < -inner) || (vertex.y > inner || vertex.y < -inner) || (vertex.z > inner || vertex.z < -inner))
                {
                    geometry.vertices.push(vertex);
                }
            }

            _vc1 = geometry.vertices.length;

            var dummyMaterial = new MeshFaceMaterial();

            radius = 200;
            var geometry2 = new CubeGeometry(radius, 0.1 * radius, 0.1 * radius, 50, 5, 5);

            Action<Geometry, double, double, double, double> addGeo = (geo, x, y, z, ry) =>
                                                                      {
                                                                          var m = new Mesh(geo, dummyMaterial);
                                                                          m.position.set(x, y, z);
                                                                          m.rotation.y = ry;

                                                                          GeometryUtils.merge(geometry, m);
                                                                      };

            // side 1

            addGeo(geometry2, 0, 110, 110, 0);
            addGeo(geometry2, 0, 110, -110, 0);
            addGeo(geometry2, 0, -110, 110, 0);
            addGeo(geometry2, 0, -110, -110, 0);

            // side 2

            double pi = System.Math.PI;
            addGeo(geometry2, 110, 110, 0, pi / 2);
            addGeo(geometry2, 110, -110, 0, pi / 2);
            addGeo(geometry2, -110, 110, 0, pi / 2);
            addGeo(geometry2, -110, -110, 0, pi / 2);

            // corner edges

            var geometry3 = new CubeGeometry(0.1 * radius, radius * 1.2, 0.1 * radius, 5, 60, 5);

            addGeo(geometry3, 110, 0, 110, 0);
            addGeo(geometry3, 110, 0, -110, 0);
            addGeo(geometry3, -110, 0, 110, 0);
            addGeo(geometry3, -110, 0, -110, 0);

            // particle system

            _obj = new ParticleSystem(geometry, shaderMaterial);
            _obj.dynamic = true;

            // custom attributes

            var vertices = _obj.geometry.vertices;

            var valuesSize = _attributes.size.value;
            var valuesColor = _attributes.ca.value;

            for (var v = 0; v < vertices.length; v++)
            {
                valuesSize[v] = 10;
                valuesColor[v] = new Color(0xffffff);

                if (v < _vc1)
                {
                    valuesColor[v].setHSV(0.5 + 0.2 * (v / (double)_vc1), 0.99, 1.0);
                }
                else
                {
                    valuesSize[v] = 55;
                    valuesColor[v].setHSV(0.1, 0.99, 1.0);
                }
            }

            //console.log( vertices.length );

            _scene.add(_obj);

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
            var time = JSDate.now() * 0.01;

            _obj.rotation.y = _obj.rotation.z = 0.02 * time;

            for (var i = 0; i < _attributes.size.value.length; i++)
            {
                if (i < _vc1)
                {
                    _attributes.size.value[i] = System.Math.Max(0, 26 + 32 * System.Math.Sin(0.1 * i + 0.6 * time));
                }
            }

            _attributes.size.needsUpdate = true;

            _renderer.render(_scene, _camera);
        }
    }
}