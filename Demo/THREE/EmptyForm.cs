using System;
using System.Windows.Forms;
using THREE;

namespace Demo.THREE
{
    public class EmptyForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;

        public EmptyForm()
        {
            camera = new PerspectiveCamera(25, aspectRatio, 1, 10000);
            camera.position.set(0, 185, 2500);

            scene = new Scene {fog = new FogExp2(0xffffff, 0.0003)};
            scene.fog.color.setHSV(0.1, 0.10, 1);

            renderer = new WebGLRenderer(new {canvas});
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
        }
    }
}