using System;
using System.Windows.Forms;
using THREE;
using WebGL;

namespace Demo.THREE
{
    public class LinesDashedForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;
        private readonly JSArray objects = new JSArray();

        public LinesDashedForm()
        {
            camera = new PerspectiveCamera(60, aspectRatio, 1, 200);
            camera.position.z = 150;

            scene = new Scene();

            scene.fog = new Fog(0x111111, 150, 200);

            var subdivisions = 6;
            var recursion = 1;

            var points = hilbert3D(new Vector3(0, 0, 0), 25.0, recursion, 0, 1, 2, 3, 4, 5, 6, 7);

            var spline = new Spline(points);
            var geometrySpline = new Geometry();

            for (var i = 0; i < points.length * subdivisions; i ++)
            {
                var index = i / (double)(points.length * subdivisions);
                var position = spline.getPoint(index);

                geometrySpline.vertices[i] = new Vector3(position.x, position.y, position.z);
            }

            var geometryCube = cube(50);

            geometryCube.computeLineDistances();
            geometrySpline.computeLineDistances();

            var @object = new Line(geometrySpline, new LineDashedMaterial(JSObject.create(new {color = 0xffffff, dashSize = 1, gapSize = 0.5})), global::THREE.THREE.LineStrip);

            objects.push(@object);
            scene.add(@object);

            @object = new Line(geometryCube, new LineDashedMaterial(JSObject.create(new {color = 0xffaa00, dashSize = 3, gapSize = 1, linewidth = 2})), global::THREE.THREE.LinePieces);

            objects.push(@object);
            scene.add(@object);

            renderer = new WebGLRenderer(new {canvas, clearColor = 0x111111, clearAlpha = 1, antialias = true});
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

        protected override void render()
        {
            var time = JSDate.now() * 0.001;

            for (var i = 0; i < objects.length; i ++)
            {
                var obj = objects[i];

                //object.rotation.x = 0.25 * time * ( i%2 == 1 ? 1 : -1);
                obj.rotation.x = 0.25 * time;
                obj.rotation.y = 0.25 * time;
            }

            renderer.render(scene, camera);
        }

        private Geometry cube(double size)
        {
            var h = size * 0.5;

            var geometry = new Geometry();

            geometry.vertices.push(new Vector3(-h, -h, -h));
            geometry.vertices.push(new Vector3(-h, h, -h));

            geometry.vertices.push(new Vector3(-h, h, -h));
            geometry.vertices.push(new Vector3(h, h, -h));

            geometry.vertices.push(new Vector3(h, h, -h));
            geometry.vertices.push(new Vector3(h, -h, -h));

            geometry.vertices.push(new Vector3(h, -h, -h));
            geometry.vertices.push(new Vector3(-h, -h, -h));

            geometry.vertices.push(new Vector3(-h, -h, h));
            geometry.vertices.push(new Vector3(-h, h, h));

            geometry.vertices.push(new Vector3(-h, h, h));
            geometry.vertices.push(new Vector3(h, h, h));

            geometry.vertices.push(new Vector3(h, h, h));
            geometry.vertices.push(new Vector3(h, -h, h));

            geometry.vertices.push(new Vector3(h, -h, h));
            geometry.vertices.push(new Vector3(-h, -h, h));

            geometry.vertices.push(new Vector3(-h, -h, -h));
            geometry.vertices.push(new Vector3(-h, -h, h));

            geometry.vertices.push(new Vector3(-h, h, -h));
            geometry.vertices.push(new Vector3(-h, h, h));

            geometry.vertices.push(new Vector3(h, h, -h));
            geometry.vertices.push(new Vector3(h, h, h));

            geometry.vertices.push(new Vector3(h, -h, -h));
            geometry.vertices.push(new Vector3(h, -h, h));

            return geometry;
        }

        private JSArray hilbert3D(Vector3 center, double side, int iterations, int v0, int v1, int v2, int v3, int v4, int v5, int v6, int v7)
        {
            var half = side / 2.0;

            var vec_s = new JSArray(
                new Vector3(center.x - half, center.y + half, center.z - half),
                new Vector3(center.x - half, center.y + half, center.z + half),
                new Vector3(center.x - half, center.y - half, center.z + half),
                new Vector3(center.x - half, center.y - half, center.z - half),
                new Vector3(center.x + half, center.y - half, center.z - half),
                new Vector3(center.x + half, center.y - half, center.z + half),
                new Vector3(center.x + half, center.y + half, center.z + half),
                new Vector3(center.x + half, center.y + half, center.z - half));

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
    }
}