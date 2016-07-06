using System;
using System.Drawing;
using System.Windows.Forms;
using THREE;
using WebGL;
using Color = System.Drawing.Color;
using Image = WebGL.Image;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class TestMemoryForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;

        public TestMemoryForm()
        {
            camera = new PerspectiveCamera(60, aspectRatio, 1, 10000);
            camera.position.z = 200;

            scene = new Scene();

            renderer = new WebGLRenderer(new {canvas, clearColor = 0xFFFFFF});
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

        public Image createImage()
        {
            using (var b = new Bitmap(256, 256))
            {
                using (var g = Graphics.FromImage(b))
                {
                    var fromArgb = Color.FromArgb(0xFF, (int)(Math.random() * 0xFF), (int)(Math.random() * 0xFF), (int)(Math.random() * 0xFF));
                    using (Brush brush = new SolidBrush(fromArgb))
                    {
                        g.FillRectangle(brush, 0, 0, 256, 256);
                    }
                }
                return new Image(b);
            }
        }

        protected override void render()
        {
            var geometry = new SphereGeometry(50, (int)(Math.random() * 64), (int)(Math.random() * 32));

            var texture = new Texture(createImage());
            texture.needsUpdate = true;

            var material = new MeshBasicMaterial(JSObject.create(new {map = texture, wireframe = true}));

            var mesh = new Mesh(geometry, material);

            scene.add(mesh);

            renderer.render(scene, camera);

            scene.remove(mesh);

            // clean up
            geometry.dispose();
            material.dispose();
            texture.dispose();
        }
    }
}