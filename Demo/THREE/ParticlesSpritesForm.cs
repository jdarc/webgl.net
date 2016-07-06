using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class ParticlesSpritesForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;
        private float mouseX;
        private float mouseY;
        private Geometry geometry;
        private JSArray materials = new JSArray();
        private JSArray parameters;

        public ParticlesSpritesForm()
        {
            camera = new PerspectiveCamera(75, aspectRatio, 1, 2000) {position = {z = 1000}};

            scene = new Scene {fog = new FogExp2(0x000000, 0.0008)};

            geometry = new Geometry();

            var sprite1 = ImageUtils.loadTexture("textures/sprites/snowflake1.png");
            var sprite2 = ImageUtils.loadTexture("textures/sprites/snowflake2.png");
            var sprite3 = ImageUtils.loadTexture("textures/sprites/snowflake3.png");
            var sprite4 = ImageUtils.loadTexture("textures/sprites/snowflake4.png");
            var sprite5 = ImageUtils.loadTexture("textures/sprites/snowflake5.png");

            for (var i = 0; i < 10000; i ++)
            {
                var vertex = new Vector3
                             {
                                 x = Math.random() * 2000 - 1000,
                                 y = Math.random() * 2000 - 1000,
                                 z = Math.random() * 2000 - 1000
                             };

                geometry.vertices.push(vertex);
            }

            parameters = new JSArray(new JSArray(new JSArray(1.0, 0.2, 1.0), sprite2, 20),
                                   new JSArray(new JSArray(0.95, 0.1, 1), sprite3, 15),
                                   new JSArray(new JSArray(0.90, 0.05, 1), sprite1, 10),
                                   new JSArray(new JSArray(0.85, 0, 0.8), sprite5, 8),
                                   new JSArray(new JSArray(0.80, 0, 0.7), sprite4, 5));

            for (var i = 0; i < parameters.length; i ++)
            {
                var color = parameters[i][0];
                var sprite = parameters[i][1];
                var size = parameters[i][2];

                materials[i] = new ParticleBasicMaterial(JSObject.create(new {size, map = sprite, blending = global::THREE.THREE.AdditiveBlending, depthTest = false, transparent = true}));
                materials[i].color.setHSV(color[0], color[1], color[2]);

                var particles = new ParticleSystem(geometry, materials[i]);

                particles.rotation.x = Math.random() * 6;
                particles.rotation.y = Math.random() * 6;
                particles.rotation.z = Math.random() * 6;

                scene.add(particles);
            }

            renderer = new WebGLRenderer(new {canvas, clearAlpha = 1});
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
            mouseX = e.X - windowHalf.X;
            mouseY = e.Y - windowHalf.Y;
        }

        protected override void render()
        {
            var time = JSDate.now() * 0.00005;

            camera.position.x += (mouseX - camera.position.x) * 0.05;
            camera.position.y += (- mouseY - camera.position.y) * 0.05;

            camera.lookAt(scene.position);

            for (var i = 0; i < scene.children.length; i ++)
            {
                var obj = scene.children[i];

                if (obj is ParticleSystem)
                {
                    obj.rotation.y = time * (i < 4 ? i + 1 : - (i + 1));
                }
            }

            for (var i = 0; i < materials.length; i ++)
            {
                var color = parameters[i][0];

                var h = (360 * (color[0] + time) % 360) / 360;
                materials[i].color.setHSV(h, color[1], color[2]);
            }

            renderer.render(scene, camera);
        }
    }
}