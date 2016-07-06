using System;
using System.Drawing;
using System.Windows.Forms;
using THREE;
using WebGL;
using Color = System.Drawing.Color;
using Math = System.Math;

namespace Demo.THREE
{
    public class MaterialsTextureAnisotropyForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private int mouseX;
        private int mouseY;
        private readonly Scene scene1;
        private readonly Scene scene2;

        public MaterialsTextureAnisotropyForm()
        {
            var valLeft = new Label
                          {
                              TextAlign = ContentAlignment.MiddleCenter,
                              Location = new Point(8, 8),
                              BackColor = Color.Black,
                              ForeColor = Color.Orange,
                              Size = new Size(128, 32),
                              Font = new Font(FontFamily.GenericSansSerif, 16)
                          };
            Controls.Add(valLeft);

            var valRight = new Label
                           {
                               TextAlign = ContentAlignment.MiddleCenter,
                               Location = new Point(ClientSize.Width - 136, 8),
                               BackColor = Color.Black,
                               ForeColor = Color.Orange,
                               Size = new Size(128, 32),
                               Font = new Font(FontFamily.GenericSansSerif, 16),
                               Anchor = AnchorStyles.Right | AnchorStyles.Top
                           };
            Controls.Add(valRight);

            renderer = new WebGLRenderer(new {canvas, antialias = true});

            camera = new PerspectiveCamera(35, aspectRatio, 1, 25000);
            camera.position.z = 1500;

            scene1 = new Scene();
            scene2 = new Scene();

            scene1.fog = new Fog(0xffffff, 1, 25000);
            scene1.fog.color.setHSV(0.6, 0.05, 1);
            scene2.fog = scene1.fog;

            scene1.add(new AmbientLight(0xeef0ff));
            scene2.add(new AmbientLight(0xeef0ff));

            var light1 = new DirectionalLight(0xffffff, 2);
            light1.position.set(1, 1, 1);
            scene1.add(light1);

            var light2 = new DirectionalLight(0xffffff, 2);
            light2.position = light1.position;
            scene2.add(light2);

            // GROUND

            var maxAnisotropy = renderer.getMaxAnisotropy();

            var texture1 = ImageUtils.loadTexture("textures/crate.gif");
            var material1 = new MeshPhongMaterial(JSObject.create(new {color = 0xffffff, map = texture1}));

            texture1.anisotropy = maxAnisotropy;
            texture1.wrapS = texture1.wrapT = global::THREE.THREE.RepeatWrapping;
            texture1.repeat.set(512, 512);

            var texture2 = ImageUtils.loadTexture("textures/crate.gif");
            var material2 = new MeshPhongMaterial(JSObject.create(new {color = 0xffffff, map = texture2}));

            texture2.anisotropy = 1;
            texture2.wrapS = texture2.wrapT = global::THREE.THREE.RepeatWrapping;
            texture2.repeat.set(512, 512);

            if (maxAnisotropy > 0)
            {
                valLeft.Text = texture1.anisotropy.ToString();
                valRight.Text = texture2.anisotropy.ToString();
            }
            else
            {
                valLeft.Text = @"not supported";
                valRight.Text = @"not supported";
            }

            var geometry = new PlaneGeometry(100, 100);

            var mesh1 = new Mesh(geometry, material1);
            mesh1.rotation.x = - Math.PI / 2;
            mesh1.scale.set(1000, 1000, 1000);

            var mesh2 = new Mesh(geometry, material2);
            mesh2.rotation.x = - Math.PI / 2;
            mesh2.scale.set(1000, 1000, 1000);

            scene1.add(mesh1);
            scene2.add(mesh2);

            // RENDERER

            renderer.setSize(ClientSize.Width, ClientSize.Height);
            renderer.setClearColor(scene1.fog.color, 1);
            renderer.autoClear = false;
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
            camera.position.x += (mouseX - camera.position.x) * .05;
            camera.position.y = global::THREE.Math.clamp(camera.position.y + (-(mouseY - 200) - camera.position.y) * .05, 50, 1000);

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