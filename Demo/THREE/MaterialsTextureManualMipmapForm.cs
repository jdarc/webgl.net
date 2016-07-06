using System;
using System.Drawing;
using System.Windows.Forms;
using THREE;
using WebGL;
using Color = System.Drawing.Color;
using Image = WebGL.Image;
using Math = System.Math;

namespace Demo.THREE
{
    public class MaterialsTextureManualMipmapForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene1;
        private readonly Scene scene2;
        private int mouseX;
        private int mouseY;

        public MaterialsTextureManualMipmapForm()
        {
            camera = new PerspectiveCamera(35, aspectRatio, 1, 5000);
            camera.position.z = 1500;

            scene1 = new Scene();
            scene1.fog = new Fog(0x000000, 1500, 4000);

            scene2 = new Scene();
            scene2.fog = scene1.fog;

            // GROUND

            var mipcan = mipmap(128, 0xff0000);
            var textureCanvas1 = new Texture(mipcan, global::THREE.THREE.UVMapping, global::THREE.THREE.RepeatWrapping, global::THREE.THREE.RepeatWrapping);
            textureCanvas1.repeat.set(1000, 1000);

            textureCanvas1.mipmaps[0] = mipcan;
            textureCanvas1.mipmaps[1] = mipmap(64, 0x00ff00);
            textureCanvas1.mipmaps[2] = mipmap(32, 0x0000ff);
            textureCanvas1.mipmaps[3] = mipmap(16, 0x440000);
            textureCanvas1.mipmaps[4] = mipmap(8, 0x004400);
            textureCanvas1.mipmaps[5] = mipmap(4, 0x000044);
            textureCanvas1.mipmaps[6] = mipmap(2, 0x004444);
            textureCanvas1.mipmaps[7] = mipmap(1, 0x440044);
            textureCanvas1.needsUpdate = true;

            var materialCanvas1 = new MeshBasicMaterial(JSObject.create(new {map = textureCanvas1}));

            var textureCanvas2 = textureCanvas1.clone(null);
            textureCanvas2.magFilter = global::THREE.THREE.NearestFilter;
            textureCanvas2.minFilter = global::THREE.THREE.NearestMipMapNearestFilter;
            textureCanvas2.needsUpdate = true;
            var materialCanvas2 = new MeshBasicMaterial(JSObject.create(new {color = 0xffccaa, map = textureCanvas2}));

            var geometry = new PlaneGeometry(100, 100);

            var meshCanvas1 = new Mesh(geometry, materialCanvas1);
            meshCanvas1.rotation.x = -Math.PI / 2;
            meshCanvas1.scale.set(1000, 1000, 1000);

            var meshCanvas2 = new Mesh(geometry, materialCanvas2);
            meshCanvas2.rotation.x = -Math.PI / 2;
            meshCanvas2.scale.set(1000, 1000, 1000);

            var texturePainting1 = ImageUtils.loadTexture("textures/758px-Canestra_di_frutta_(Caravaggio).jpg", global::THREE.THREE.UVMapping, x =>
                                                                                                                                       {
                                                                                                                                           var texturePainting2 = new Texture();
                                                                                                                                           var materialPainting = new MeshBasicMaterial(JSObject.create(new {color = 0xffffff, map = x}));
                                                                                                                                           var materialPainting2 = new MeshBasicMaterial(JSObject.create(new {color = 0xffccaa, map = texturePainting2}));

                                                                                                                                           texturePainting2.minFilter = texturePainting2.magFilter = global::THREE.THREE.NearestFilter;

                                                                                                                                           var image = x.image;

                                                                                                                                           texturePainting2.image = image;
                                                                                                                                           texturePainting2.needsUpdate = true;

                                                                                                                                           scene1.add(meshCanvas1);
                                                                                                                                           scene2.add(meshCanvas2);

                                                                                                                                           geometry = new PlaneGeometry(100, 100);
                                                                                                                                           var mesh = new Mesh(geometry, materialPainting);
                                                                                                                                           var mesh2 = new Mesh(geometry, materialPainting2);

                                                                                                                                           addPainting(scene1, mesh, image, geometry, meshCanvas1, meshCanvas2);
                                                                                                                                           addPainting(scene2, mesh2, image, geometry, meshCanvas1, meshCanvas2);

                                                                                                                                           texturePainting2.minFilter = texturePainting2.magFilter = global::THREE.THREE.NearestFilter;
                                                                                                                                       });

            texturePainting1.minFilter = texturePainting1.magFilter = global::THREE.THREE.LinearFilter;

            renderer = new WebGLRenderer(new {canvas, antialias = true});
            renderer.setSize(ClientSize.Width, ClientSize.Height);
            renderer.setClearColor(scene1.fog.color, 1);
            renderer.autoClear = false;
        }

        // todo: this should be a canvas not an image
        private Image mipmap(int size, int color)
        {
            var img = new Bitmap(size, size);
            using (var g = Graphics.FromImage(img))
            {
                color = 0xff << 24 | color;
                g.FillRectangle(new SolidBrush(Color.FromArgb(0x44, 0x44, 0x44)), 0, 0, size, size);
                g.FillRectangle(new SolidBrush(Color.FromArgb(color)), 0, 0, size / 2, size / 2);
                g.FillRectangle(new SolidBrush(Color.FromArgb(color)), size / 2, size / 2, size / 2, size / 2);
            }

            return new Image(img);
        }

        private void addPainting(dynamic zscene, dynamic zmesh, dynamic image, dynamic geometry, dynamic meshCanvas, dynamic meshCanvas2)
        {
            zmesh.scale.x = image.width / 100.0;
            zmesh.scale.y = image.height / 100.0;

            zscene.add(zmesh);

            var meshFrame = new Mesh(geometry, new MeshBasicMaterial(JSObject.create((dynamic)new
                                                                                            {
                                                                                                color = 0x000000,
                                                                                                polygonOffset = true,
                                                                                                polygonOffsetFactor = 1,
                                                                                                polygonOffsetUnits = 5
                                                                                            })));
            meshFrame.scale.x = 1.1 * image.width / 100.0;
            meshFrame.scale.y = 1.1 * image.height / 100.0;

            zscene.add(meshFrame);

            var meshShadow = new Mesh(geometry, new MeshBasicMaterial(JSObject.create((dynamic)new
                                                                                             {
                                                                                                 color = 0x000000,
                                                                                                 opacity = 0.75,
                                                                                                 transparent = true
                                                                                             })));
            meshShadow.position.y = - 1.1 * image.height / 2.0;
            meshShadow.position.z = - 1.1 * image.height / 2.0;
            meshShadow.rotation.x = - Math.PI / 2;
            meshShadow.scale.x = 1.1 * image.width / 100.0;
            meshShadow.scale.y = 1.1 * image.height / 100.0;
            zscene.add(meshShadow);

            var floorHeight = - 1.117 * image.height / 2.0;
            meshCanvas.position.y = meshCanvas2.position.y = floorHeight;
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
            mouseX = (int)(e.X - windowHalf.X);
            mouseY = (int)(e.Y - windowHalf.Y);
        }

        protected override void render()
        {
            camera.position.x += (mouseX - camera.position.x) * .05;
            camera.position.y += (-(mouseY - 200) - camera.position.y) * .05;

            camera.lookAt(scene1.position);

            renderer.enableScissorTest(false);
            renderer.clear();
            renderer.enableScissorTest(true);

            renderer.setScissor(0, 0, ClientSize.Width / 2 - 2, ClientSize.Height);
            renderer.render(scene1, camera);

            renderer.setScissor(ClientSize.Width / 2, 0, ClientSize.Width / 2 - 2, ClientSize.Height);
            renderer.render(scene2, camera);
        }
    }
}