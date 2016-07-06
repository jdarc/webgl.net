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
    public class MaterialsTextureFiltersForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;
        private readonly Scene scene2;
        private int mouseX;
        private int mouseY;

        public MaterialsTextureFiltersForm()
        {
            camera = new PerspectiveCamera(35, aspectRatio, 1, 5000) {position = {z = 1500}};

            scene = new Scene {fog = new Fog(0x000000, 1500, 4000)};

            scene2 = new Scene {fog = scene.fog};

            // GROUND
            var img = new Bitmap(128, 128);
            using (var g = Graphics.FromImage(img))
            {
                g.FillRectangle(new SolidBrush(Color.FromArgb(0x44, 0x44, 0x44)), 0, 0, 128, 128);
                g.FillRectangle(Brushes.White, 0, 0, 64, 64);
                g.FillRectangle(Brushes.White, 64, 64, 64, 64);
            }

            var imageCanvas = new Image(img);

            var textureCanvas = new Texture(imageCanvas, global::THREE.THREE.UVMapping, global::THREE.THREE.RepeatWrapping, global::THREE.THREE.RepeatWrapping);
            var materialCanvas = new MeshBasicMaterial(JSObject.create(new {map = textureCanvas}));

            textureCanvas.needsUpdate = true;
            textureCanvas.repeat.set(1000, 1000);

            var textureCanvas2 = new Texture(imageCanvas, global::THREE.THREE.UVMapping, global::THREE.THREE.RepeatWrapping, global::THREE.THREE.RepeatWrapping, global::THREE.THREE.NearestFilter, global::THREE.THREE.NearestFilter);
            var materialCanvas2 = new MeshBasicMaterial(JSObject.create(new {color = 0xffccaa, map = textureCanvas2}));

            textureCanvas2.needsUpdate = true;
            textureCanvas2.repeat.set(1000, 1000);

            var geometry = new PlaneGeometry(100, 100);

            var meshCanvas = new Mesh(geometry, materialCanvas);
            meshCanvas.rotation.x = - Math.PI / 2;
            meshCanvas.scale.set(1000, 1000, 1000);

            var meshCanvas2 = new Mesh(geometry, materialCanvas2);
            meshCanvas2.rotation.x = - Math.PI / 2;
            meshCanvas2.scale.set(1000, 1000, 1000);

            // PAINTING
            var texturePainting = ImageUtils.loadTexture("textures/758px-Canestra_di_frutta_(Caravaggio).jpg", global::THREE.THREE.UVMapping, x =>
                                                                                                                                      {
                                                                                                                                          var texturePainting2 = new Texture();
                                                                                                                                          var materialPainting = new MeshBasicMaterial(JSObject.create(new {color = 0xffffff, map = x}));
                                                                                                                                          var materialPainting2 = new MeshBasicMaterial(JSObject.create(new {color = 0xffccaa, map = texturePainting2}));

                                                                                                                                          texturePainting2.minFilter = texturePainting2.magFilter = global::THREE.THREE.NearestFilter;

                                                                                                                                          var image = x.image;

                                                                                                                                          texturePainting2.image = image;
                                                                                                                                          texturePainting2.needsUpdate = true;

                                                                                                                                          scene.add(meshCanvas);
                                                                                                                                          scene2.add(meshCanvas2);

                                                                                                                                          geometry = new PlaneGeometry(100, 100);
                                                                                                                                          var mesh = new Mesh(geometry, materialPainting);
                                                                                                                                          var mesh2 = new Mesh(geometry, materialPainting2);

                                                                                                                                          addPainting(scene, mesh, image, geometry, meshCanvas, meshCanvas2);
                                                                                                                                          addPainting(scene2, mesh2, image, geometry, meshCanvas, meshCanvas2);
                                                                                                                                      });
            texturePainting.minFilter = texturePainting.magFilter = global::THREE.THREE.LinearFilter;

            renderer = new WebGLRenderer(new {canvas, antialias = true});
            renderer.setSize(ClientSize.Width, ClientSize.Height);
            renderer.setClearColor(scene.fog.color, 1);
            renderer.autoClear = false;
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

            camera.lookAt(scene.position);

            renderer.enableScissorTest(false);
            renderer.clear();
            renderer.enableScissorTest(true);

            //renderer.setViewport( 0, 0, SCREEN_WIDTH/2, SCREEN_HEIGHT );
            renderer.setScissor(0, 0, ClientSize.Width / 2 - 2, ClientSize.Height);
            renderer.render(scene, camera);

            //renderer.setViewport( SCREEN_WIDTH/2, 0, SCREEN_WIDTH/2, SCREEN_HEIGHT );
            renderer.setScissor(ClientSize.Width / 2, 0, ClientSize.Width / 2 - 2, ClientSize.Height);
            renderer.render(scene2, camera);
        }
    }
}