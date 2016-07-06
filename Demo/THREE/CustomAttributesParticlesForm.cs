using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class CustomAttributesParticlesForm : BaseForm
    {
        private const string vertexshader =
            @"
			uniform float amplitude;
			attribute float size;
			attribute vec3 customColor;

			varying vec3 vColor;

			void main() {
				vColor = customColor;
				vec4 mvPosition = modelViewMatrix * vec4( position, 1.0 );
				gl_PointSize = size * ( 300.0 / length( mvPosition.xyz ) );
				gl_Position = projectionMatrix * mvPosition;

			}";

        private const string fragmentshader =
            @"
			uniform vec3 color;
			uniform sampler2D texture;

			varying vec3 vColor;

			void main() {

				gl_FragColor = vec4( color * vColor, 1.0 );
				gl_FragColor = gl_FragColor * texture2D( texture, gl_PointCoord );

			}";

        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;
        private dynamic attributes;
        private dynamic uniforms;
        private dynamic sphere;

        public CustomAttributesParticlesForm()
        {
            camera = new PerspectiveCamera(40, aspectRatio, 1, 10000) {position = {z = 300}};

            scene = new Scene();

            attributes = JSObject.create(new
                                       {
                                           size = new {type = "f", value = new JSArray()},
                                           customColor = new {type = "c", value = new JSArray()}
                                       });

            uniforms = JSObject.create(new
                                     {
                                         amplitude = new {type = "f", value = 1.0},
                                         color = new {type = "c", value = new Color(0xffffff)},
                                         texture = new {type = "t", value = ImageUtils.loadTexture("textures/sprites/spark1.png")},
                                     });

            var shaderMaterial = new ShaderMaterial(JSObject.create(new
                                                                  {
                                                                      uniforms,
                                                                      attributes,
                                                                      vertexShader = vertexshader,
                                                                      fragmentShader = fragmentshader,
                                                                      blending = global::THREE.THREE.AdditiveBlending,
                                                                      depthTest = false,
                                                                      transparent = true
                                                                  }));

            var radius = 200.0;
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

                geometry.vertices.push(vertex);
            }

            sphere = new ParticleSystem(geometry, shaderMaterial);
            sphere.dynamic = true;
            //sphere.sortParticles = true;

            var vertices = sphere.geometry.vertices;
            var values_size = attributes.size.value;
            var values_color = attributes.customColor.value;

            for (var v = 0; v < vertices.length; v++)
            {
                values_size[v] = 10.0;
                values_color[v] = new Color(0xffaa00);

                if (vertices[v].x < 0)
                {
                    values_color[v].setHSV(0.5 + 0.1 * (v / (double)vertices.length), 0.7, 0.9);
                }
                else
                {
                    values_color[v].setHSV(0.0 + 0.1 * (v / (double)vertices.length), 0.9, 0.9);
                }
            }

            scene.add(sphere);

            renderer = new WebGLRenderer(new {canvas, clearColor = 0x000000, clearAlpha = 1});
            renderer.setSize(ClientSize.Width, ClientSize.Height);
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
            var time = JSDate.now() * 0.005;

            sphere.rotation.z = 0.01 * time;

            for (var i = 0; i < attributes.size.value.length; i++)
            {
                attributes.size.value[i] = 14 + 13 * System.Math.Sin(0.1 * i + time);
            }

            attributes.size.needsUpdate = true;

            renderer.render(scene, camera);
        }
    }
}