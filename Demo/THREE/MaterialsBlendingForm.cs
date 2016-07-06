using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows.Forms;
using THREE;
using WebGL;
using Color = System.Drawing.Color;
using Image = WebGL.Image;

namespace Demo.THREE
{
    public class MaterialsBlendingForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;

        public MaterialsBlendingForm()
        {
            // CAMERA

            camera = new PerspectiveCamera(70, aspectRatio, 1, 1000);
            camera.position.z = 600;

            // SCENE

            scene = new Scene();

            // BACKGROUND

            var img = new Bitmap(128, 128, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(img))
            {
                g.Clear(Color.FromArgb(0xFF << 24 | 0xdddddd));
                g.FillRectangle(new SolidBrush(Color.FromArgb(0xFF << 24 | 0x555555)), 0, 0, 64, 64);
                g.FillRectangle(new SolidBrush(Color.FromArgb(0xFF << 24 | 0x999999)), 32, 32, 32, 32);
                g.FillRectangle(new SolidBrush(Color.FromArgb(0xFF << 24 | 0x555555)), 64, 64, 64, 64);
                g.FillRectangle(new SolidBrush(Color.FromArgb(0xFF << 24 | 0x777777)), 96, 96, 32, 32);
            }

            var mapBg = new Texture(new Image(img));
            mapBg.wrapS = mapBg.wrapT = global::THREE.THREE.RepeatWrapping;
            mapBg.repeat.set(128, 64);
            mapBg.needsUpdate = true;

            var materialBg = new MeshBasicMaterial(JSObject.create(new {map = mapBg}));

            var meshBg = new Mesh(new PlaneGeometry(4000, 2000), materialBg);
            meshBg.position.set(0, 0, -1);
            scene.add(meshBg);

            // OBJECTS

            var blendings = new JSArray("NoBlending", "NormalBlending", "AdditiveBlending", "SubtractiveBlending", "MultiplyBlending", "AdditiveAlphaBlending");

            var map0 = ImageUtils.loadTexture("textures/ash_uvgrid01.jpg");
            var map1 = ImageUtils.loadTexture("textures/sprite0.jpg");
            var map2 = ImageUtils.loadTexture("textures/sprite0.png");
            var map3 = ImageUtils.loadTexture("textures/lensflare/lensflare0.png");
            var map4 = ImageUtils.loadTexture("textures/lensflare/lensflare0_alpha.png");

            var geo1 = new PlaneGeometry(100, 100);
            var geo2 = new PlaneGeometry(100, 25);

            Action<Texture, int> addImageRow = (map, y) =>
                                               {
                                                   for (var i = 0; i < blendings.length; i ++)
                                                   {
                                                       var blending = blendings[i];

                                                       var material = new MeshBasicMaterial(JSObject.create(new {map}));

                                                       material.transparent = true;
                                                       material.blending = global::THREE.THREE.get(blending) ?? -1;

                                                       var x = (i - blendings.length / 2) * 110;
                                                       var z = 0;

                                                       var mesh = new Mesh(geo1, material);
                                                       mesh.position.set(x, y, z);
                                                       scene.add(mesh);

                                                       mesh = new Mesh(geo2, generateLabelMaterial(blending.Replace("Blending", "")));
                                                       mesh.position.set(x, y - 75, z);
                                                       scene.add(mesh);
                                                   }
                                               };

            addImageRow(map0, 300);
            addImageRow(map1, 150);
            addImageRow(map2, 0);
            addImageRow(map3, -150);
            addImageRow(map4, -300);

            // RENDERER

            renderer = new WebGLRenderer(new {canvas, clearColor = 0x000000, clearAlpha = 1});
            renderer.setSize(ClientSize.Width, ClientSize.Height);
        }

        private Material generateLabelMaterial(string text)
        {
            var x = new Bitmap(128, 32);
            using (var g = Graphics.FromImage(x))
            {
                g.Clear(Color.FromArgb(242, 0, 0, 0));
                g.DrawString(text, new Font("Arial", 12, FontStyle.Bold, GraphicsUnit.Point), Brushes.White, 10, 8);
            }
            var map = new Texture(new Image(x));
            map.needsUpdate = true;

            var material = new MeshBasicMaterial(JSObject.create(new {map, transparent = true}));
            return material;
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
            renderer.render(scene, camera);
        }
    }
}