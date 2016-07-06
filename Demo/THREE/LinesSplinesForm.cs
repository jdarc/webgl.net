using System;
using System.Windows.Forms;
using THREE;
using WebGL;

namespace Demo.THREE
{
    public class LinesSplinesForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;
        private int mouseX;
        private int mouseY;

        public LinesSplinesForm()
        {
            camera = new PerspectiveCamera(33, aspectRatio, 1, 10000);
            camera.position.z = 700;

            scene = new Scene();

            renderer = new WebGLRenderer(new {canvas, antialias = true});
            renderer.setSize(ClientSize.Width, ClientSize.Height);

            var geometry = new Geometry();
            var geometry2 = new Geometry();
            var geometry3 = new Geometry();

            var points = hilbert3D(new Vector3(0, 0, 0), 200.0, 1, 0, 1, 2, 3, 4, 5, 6, 7);
            var colors = new JSArray();
            var colors2 = new JSArray();
            var colors3 = new JSArray();

            double n_sub = 6;

            var spline = new Spline(points);

            for (var i = 0; i < points.length * n_sub; i ++)
            {
                var index = i / (points.length * n_sub);
                var position = spline.getPoint(index);

                geometry.vertices[i] = new Vector3(position.x, position.y, position.z);

                colors[i] = new Color(0xffffff);
                colors[i].setHSV(0.6, (200 + position.x) / 400, 1.0);

                colors2[i] = new Color(0xffffff);
                colors2[i].setHSV(0.9, (200 + position.y) / 400, 1.0);

                colors3[i] = new Color(0xffffff);
                colors3[i].setHSV(i / (points.length * n_sub), 1.0, 1.0);
            }

            geometry2.vertices = geometry3.vertices = geometry.vertices;

            geometry.colors = colors;
            geometry2.colors = colors2;
            geometry3.colors = colors3;

            // lines

            var material = new LineBasicMaterial(JSObject.create(new {color = 0xffffff, opacity = 1, linewidth = 3, vertexColors = global::THREE.THREE.VertexColors}));

            var scale = 0.3;
            var d = 225;
            var parameters =
                new JSArray(
                    new JSArray(material, scale * 1.5, new JSArray(-d, 0, 0), geometry),
                    new JSArray(material, scale * 1.5, new JSArray(0, 0, 0), geometry2),
                    new JSArray(material, scale * 1.5, new JSArray(d, 0, 0), geometry3)
                    );

            for (var i = 0; i < parameters.length; ++ i)
            {
                var p = parameters[i];
                var line = new Line(p[3], p[0]);
                line.scale.x = line.scale.y = line.scale.z = p[1];
                line.position.x = p[2][0];
                line.position.y = p[2][1];
                line.position.z = p[2][2];
                scene.add(line);
            }
        }

        private JSArray hilbert3D(Vector3 center, double side, int iterations, int v0, int v1, int v2, int v3, int v4, int v5, int v6, int v7)
        {
            var half = side / 2.0;
            var vec_s = new[]
                        {
                            new Vector3(center.x - half, center.y + half, center.z - half),
                            new Vector3(center.x - half, center.y + half, center.z + half),
                            new Vector3(center.x - half, center.y - half, center.z + half),
                            new Vector3(center.x - half, center.y - half, center.z - half),
                            new Vector3(center.x + half, center.y - half, center.z - half),
                            new Vector3(center.x + half, center.y - half, center.z + half),
                            new Vector3(center.x + half, center.y + half, center.z + half),
                            new Vector3(center.x + half, center.y + half, center.z - half)
                        };

            var vec = new JSArray(vec_s[v0], vec_s[v1], vec_s[v2], vec_s[v3], vec_s[v4], vec_s[v5], vec_s[v6], vec_s[v7]);

            if (--iterations >= 0)
            {
                var tmp = new JSArray();

                JSArray.prototype.push.apply(tmp, hilbert3D(vec[0], half, iterations, v0, v3, v4, v7, v6, v5, v2, v1));
                JSArray.prototype.push.apply(tmp, hilbert3D(vec[1], half, iterations, v0, v7, v6, v1, v2, v5, v4, v3));
                JSArray.prototype.push.apply(tmp, hilbert3D(vec[2], half, iterations, v0, v7, v6, v1, v2, v5, v4, v3));
                JSArray.prototype.push.apply(tmp, hilbert3D(vec[3], half, iterations, v2, v3, v0, v1, v6, v7, v4, v5));
                JSArray.prototype.push.apply(tmp, hilbert3D(vec[4], half, iterations, v2, v3, v0, v1, v6, v7, v4, v5));
                JSArray.prototype.push.apply(tmp, hilbert3D(vec[5], half, iterations, v4, v3, v2, v5, v6, v1, v0, v7));
                JSArray.prototype.push.apply(tmp, hilbert3D(vec[6], half, iterations, v4, v3, v2, v5, v6, v1, v0, v7));
                JSArray.prototype.push.apply(tmp, hilbert3D(vec[7], half, iterations, v6, v5, v2, v1, v0, v3, v4, v7));

                return tmp;
            }

            return vec;
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
            camera.position.y += (- mouseY + 200 - camera.position.y) * .05;

            camera.lookAt(scene.position);

            var time = JSDate.now() * 0.0005;

            for (var i = 0; i < scene.children.length; i ++)
            {
                var obj = scene.children[i];

                if (obj is Line)
                {
                    obj.rotation.y = time * (i % 2 != 0 ? 1 : -1);
                }
            }

            renderer.render(scene, camera);
        }
    }
}