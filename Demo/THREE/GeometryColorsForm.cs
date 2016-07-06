using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Windows.Forms;
using THREE;
using WebGL;
using Color = THREE.Color;
using Image = WebGL.Image;

namespace Demo.THREE
{
    public class GeometryColorsForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;
        private float mouseX;
        private float mouseY;

        public GeometryColorsForm()
        {
            camera = new PerspectiveCamera(20, aspectRatio, 1, 10000) {position = {z = 1800}};

            scene = new Scene();

            var light = new DirectionalLight(0xffffff);
            light.position.set(0, 0, 1);
            scene.add(light);

            // shadow
            var b = new Bitmap(128, 128, PixelFormat.Format32bppArgb);
            using (var g = Graphics.FromImage(b))
            {
                g.Clear(System.Drawing.Color.White);
                var bounds = new Rectangle(0, 0, b.Width, b.Height);
                using (var ellipsePath = new GraphicsPath())
                {
                    ellipsePath.AddEllipse(bounds);
                    using (var brush = new PathGradientBrush(ellipsePath))
                    {
                        brush.CenterPoint = new PointF(bounds.Width / 2f, bounds.Height / 2f);
                        brush.CenterColor = System.Drawing.Color.FromArgb(255, 210, 210, 210);
                        brush.SurroundColors = new[] {System.Drawing.Color.FromArgb(255, 255, 255, 255)};
                        brush.FocusScales = new PointF(0, 0);

                        g.FillRectangle(brush, bounds);
                    }
                }
            }

            var canvasImg = new Image(b);
            var shadowTexture = new Texture(canvasImg) {needsUpdate = true};

            var shadowMaterial = new MeshBasicMaterial(JSObject.create(new {map = shadowTexture}));
            var shadowGeo = new PlaneGeometry(300, 300, 1, 1);

            var mesh = new Mesh(shadowGeo, shadowMaterial) {position = {y = - 250}};
            double pi = System.Math.PI;
            mesh.rotation.x = - pi / 2;
            scene.add(mesh);

            mesh = new Mesh(shadowGeo, shadowMaterial) {position = {y = - 250, x = - 400}};
            mesh.rotation.x = - pi / 2;
            scene.add(mesh);

            mesh = new Mesh(shadowGeo, shadowMaterial) {position = {y = - 250, x = 400}};
            mesh.rotation.x = - pi / 2;
            scene.add(mesh);

            var faceIndices = new JSArray("a", "b", "c", "d");

            const double radius = 200;
            var geometry = new IcosahedronGeometry(radius, 1);
            var geometry2 = new IcosahedronGeometry(radius, 1);
            var geometry3 = new IcosahedronGeometry(radius, 1);

            for (var i = 0; i < geometry.faces.length; i++)
            {
                var f = geometry.faces[i];
                var f2 = geometry2.faces[i];
                var f3 = geometry3.faces[i];

                var n = (f is Face3) ? 3 : 4;

                for (var j = 0; j < n; j++)
                {
                    var vertexIndex = f[faceIndices[j]];

                    var p = geometry.vertices[vertexIndex];

                    var color = new Color(0xffffff);
                    color.setHSV((p.y / radius + 1) / 2, 1.0, 1.0);

                    f.vertexColors[j] = color;

                    color = new Color(0xffffff);
                    color.setHSV(0.0, (p.y / radius + 1) / 2, 1.0);

                    f2.vertexColors[j] = color;

                    color = new Color(0xffffff);
                    color.setHSV(0.125 * vertexIndex / geometry.vertices.length, 1.0, 1.0);

                    f3.vertexColors[j] = color;
                }
            }

            var materials = new JSArray(
                new MeshLambertMaterial(JSObject.create(new {color = 0xffffff, shading = global::THREE.THREE.FlatShading, vertexColors = global::THREE.THREE.VertexColors})),
                new MeshBasicMaterial(JSObject.create(new {color = 0x000000, shading = global::THREE.THREE.FlatShading, wireframe = true, transparent = true})));

            var group1 = SceneUtils.createMultiMaterialObject(geometry, materials);
            group1.position.x = -400;
            group1.rotation.x = -1.87;
            scene.add(group1);

            var group2 = SceneUtils.createMultiMaterialObject(geometry2, materials);
            group2.position.x = 400;
            group2.rotation.x = 0;
            scene.add(group2);

            var group3 = SceneUtils.createMultiMaterialObject(geometry3, materials);
            group3.position.x = 0;
            group3.rotation.x = 0;
            scene.add(group3);

            renderer = new WebGLRenderer(new {canvas, antialias = true, clearColor = 0xFFFFFF});
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
            mouseX = (e.X - windowHalf.X);
            mouseY = (e.Y - windowHalf.Y);
        }

        protected override void render()
        {
            camera.position.x += (mouseX - camera.position.x) * 0.05;
            camera.position.y += (-mouseY - camera.position.y) * 0.05;

            camera.lookAt(scene.position);

            renderer.render(scene, camera);
        }
    }
}