using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class CustomAttributesRibbonsForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;
        private JSArray materials = new JSArray();

        public CustomAttributesRibbonsForm()
        {
            camera = new PerspectiveCamera(70, aspectRatio, 1, 3000) {position = {z = 1200}};

            scene = new Scene {fog = new FogExp2(0x000000, 0.0016)};

            const string vertexShader =
                @"
			uniform float ratio;

			attribute vec3 position2;
			attribute vec3 customColor;

			varying vec3 vColor;

			void main() {

				vColor = customColor;

				vec3 newPosition = mix( position, position2, ratio );
				gl_Position = projectionMatrix * modelViewMatrix * vec4( newPosition, 1.0 );

			}";

            const string fragmentShader = @"
			uniform vec3 color;
			varying vec3 vColor;

			void main() {

				gl_FragColor = vec4( color * vColor, 1.0 );

			}";

            var attributes = JSObject.create(new
                                           {
                                               customColor = new {type = "c", boundTo = "vertices", value = new JSArray()},
                                               position2 = new {type = "v3", boundTo = "vertices", value = new JSArray()}
                                           });

            var uniforms = JSObject.create(new
                                         {
                                             ratio = new {type = "f", value = 1.0},
                                             color = new {type = "c", value = new Color(0xffffff)}
                                         });

            var material = new ShaderMaterial(JSObject.create(new {uniforms, attributes, vertexShader, fragmentShader, side = global::THREE.THREE.DoubleSide}));

            var position2 = attributes.position2.value;
            var colors = attributes.customColor.value;

            //

            var geometry = new Geometry();

            const int n = 200;

            for (var i = -n; i < n; i ++)
            {
                var i2 = i + n;

                var x1 = 10.0 * i;
                double pi = System.Math.PI;
                var y1 = - 50.0 + (i2 % 2) * 100 - System.Math.Cos(4 * pi * i / n) * 50;
                const double z1 = 0.0;

                var x2 = x1;
                var y2 = y1 + System.Math.Cos(4 * pi * i / n) * 100;
                const double z2 = z1;

                var h = i2 % 2 > 0 ? 1 : 0.15;
                if (i2 % 4 <= 2)
                {
                    h -= 0.15;
                }

                var color = new Color(0xffffff);
                color.setHSV(0.1 * Math.random(), 0.15, h);

                position2[geometry.vertices.length] = new Vector3(x2, y2, z2);
                colors[geometry.vertices.length] = color;

                geometry.vertices.push(new Vector3(x1, y1, z1));
            }

            var ribbon = new Ribbon(geometry, material);
            scene.add(ribbon);

            materials.push(ribbon.material);

            ribbon = new Ribbon(geometry, material.clone()) {position = {y = 250, x = 250}};
            scene.add(ribbon);

            ribbon.material.uniforms.color.value.setHSV(0, 0.75, 1);
            materials.push(ribbon.material);

            ribbon = new Ribbon(geometry, material.clone()) {position = {y = -250, x = 250}};
            scene.add(ribbon);

            ribbon.material.uniforms.color.value.setHSV(0.1, 0.75, 1);
            materials.push(ribbon.material);

            renderer = new WebGLRenderer(new {canvas, antialias = true});
            renderer.setSize(ClientSize.Width, ClientSize.Height);
            renderer.setClearColor(scene.fog.color, 1);
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
            var time = JSDate.now() * 0.0025;

            camera.lookAt(scene.position);

            for (var i = 0; i < materials.length; i++)
            {
                var uniforms = materials[i].uniforms;
                uniforms.ratio.value = 0.5 * (System.Math.Sin(time) + 1);
            }

            renderer.render(scene, camera);
        }
    }
}