using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class LodForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;
        private int mouseX;
        private int mouseY;

        public LodForm()
        {
            camera = new PerspectiveCamera(45, aspectRatio, 1, 15000);
            camera.position.z = 1000;

            scene = new Scene();
            scene.fog = new Fog(0x000000, 1, 15000);

            dynamic light = new PointLight(0xff2200);
            light.position.set(0, 0, 0);
            scene.add(light);

            light = new DirectionalLight(0xffffff);
            light.position.set(0, 0, 1).normalize();
            scene.add(light);

            var material = new MeshLambertMaterial(JSObject.create(new {color = 0xffffff, wireframe = true}));

            var geometry = new JSArray(
                new JSArray(new SphereGeometry(100, 64, 32), 300),
                new JSArray(new SphereGeometry(100, 32, 16), 1000),
                new JSArray(new SphereGeometry(100, 16, 8), 2000),
                new JSArray(new SphereGeometry(100, 8, 4), 10000));

            for (var j = 0; j < 1000; j ++)
            {
                var lod = new LOD();

                for (var i = 0; i < geometry.length; i ++)
                {
                    var mesh = new Mesh(geometry[i][0], material);
                    mesh.scale.set(1.5, 1.5, 1.5);
                    mesh.updateMatrix();
                    mesh.matrixAutoUpdate = false;
                    lod.addLevel(mesh, geometry[i][1]);
                }

                lod.position.x = 10000 * (0.5 - Math.random());
                lod.position.y = 7500 * (0.5 - Math.random());
                lod.position.z = 10000 * (0.5 - Math.random());
                lod.updateMatrix();
                lod.matrixAutoUpdate = false;
                scene.add(lod);
            }

            renderer = new WebGLRenderer(new {canvas, clearColor = 0x000000, clearAlpha = 1.0});
            renderer.setSize(ClientSize.Width, ClientSize.Height);

            renderer.sortObjects = false;
            renderer.autoUpdateScene = false;
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
            mouseX = (int)((e.X - windowHalf.X) * 10);
            mouseY = (int)((e.Y - windowHalf.Y) * 10);
        }

        protected override void render()
        {
            camera.position.x += (mouseX - camera.position.x) * .005;
            camera.position.y += (- mouseY - camera.position.y) * .01;

            camera.lookAt(scene.position);

            scene.updateMatrixWorld();
            scene.traverse(obj =>
                           {
                               var lodObj = obj as LOD;
                               if (lodObj != null)
                               {
                                   lodObj.update(camera);
                               }
                           });

            renderer.render(scene, camera);
        }
    }
}