using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class MorphNormalsForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly dynamic camera;
        private readonly Scene scene1;
        private readonly Scene scene2;
        private readonly JSArray morphs = new JSArray();
        private double radius = 600;
        private double theta;
        private Clock clock = new Clock();

        public MorphNormalsForm()
        {
            camera = new PerspectiveCamera(40, 0.5 * aspectRatio, 1, 10000);
            camera.position.y = 300;
            camera.target = new Vector3(0, 150, 0);

            scene1 = new Scene();
            scene2 = new Scene();

            //

            var light = new DirectionalLight(0xffffff, 1.3);
            light.position.set(1, 1, 1);
            scene1.add(light);

            light = new DirectionalLight(0xffffff, 0.1);
            light.position.set(0.25, -1, 0);
            scene1.add(light);

            //

            light = new DirectionalLight(0xffffff, 1.3);
            light.position.set(1, 1, 1);
            scene2.add(light);

            light = new DirectionalLight(0xffffff, 0.1);
            light.position.set(0.25, -1, 0);
            scene2.add(light);

            //

            var loader = new JSONLoader();
            loader.load("models/animated/flamingo.js", (geometry, texture) =>
                                                       {
                                                           morphColorsToFaceColors(geometry);
                                                           geometry.computeMorphNormals();

                                                           var material = new MeshLambertMaterial(JSObject.create(new {color = 0xffffff, morphTargets = true, morphNormals = true, vertexColors = global::THREE.THREE.FaceColors, shading = global::THREE.THREE.FlatShading}));
                                                           var meshAnim = new MorphAnimMesh(geometry, material);

                                                           meshAnim.duration = 5000;

                                                           meshAnim.scale.set(1.5, 1.5, 1.5);
                                                           meshAnim.position.y = 150;

                                                           scene1.add(meshAnim);
                                                           morphs.push(meshAnim);
                                                       });

            loader.load("models/animated/flamingo.js", (geometry, texture) =>
                                                       {
                                                           morphColorsToFaceColors(geometry);
                                                           geometry.computeMorphNormals();

                                                           var material =
                                                               new MeshPhongMaterial(
                                                                   JSObject.create(
                                                                       new {color = 0xffffff, specular = 0xffffff, shininess = 20, morphTargets = true, morphNormals = true, vertexColors = global::THREE.THREE.FaceColors, shading = global::THREE.THREE.SmoothShading}));
                                                           var meshAnim = new MorphAnimMesh(geometry, material);

                                                           meshAnim.duration = 5000;

                                                           meshAnim.scale.set(1.5, 1.5, 1.5);
                                                           meshAnim.position.y = 150;

                                                           scene2.add(meshAnim);
                                                           morphs.push(meshAnim);
                                                       });

            //

            renderer = new WebGLRenderer(new {canvas, antialias = true, clearColor = 0x111111});
            renderer.sortObjects = false;
            renderer.autoClear = false;

            renderer.gammaInput = true;
            renderer.gammaOutput = true;

            renderer.setSize(ClientSize.Width, ClientSize.Height);
        }

        private void morphColorsToFaceColors(dynamic geometry)
        {
            if (geometry.morphColors != null && geometry.morphColors.length > 0)
            {
                var colorMap = geometry.morphColors[0];

                for (var i = 0; i < colorMap.colors.length; i++)
                {
                    geometry.faces[i].color = colorMap.colors[i];
                    ColorUtils.adjustHSV(geometry.faces[i].color, 0, 0.125, 0);
                }
            }
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
            theta += 0.1;

            camera.position.x = radius * System.Math.Sin(Math.degToRad(theta));
            camera.position.z = radius * System.Math.Cos(Math.degToRad(theta));

            camera.lookAt(camera.target);

            var delta = clock.getDelta();

            for (var i = 0; i < morphs.length; i++)
            {
                morphs[i].updateAnimation(1000 * delta);
            }

            renderer.clear();

            renderer.setViewport(0, 0, ClientSize.Width / 2, ClientSize.Height);
            renderer.render(scene1, camera);

            renderer.setViewport(ClientSize.Width / 2, 0, ClientSize.Width / 2, ClientSize.Height);
            renderer.render(scene2, camera);
        }
    }
}