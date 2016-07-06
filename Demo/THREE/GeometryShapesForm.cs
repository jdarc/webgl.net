using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = System.Math;

namespace Demo.THREE
{
    public class GeometryShapesForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;
        private readonly Object3D parent;
        private double targetRotation;
        private double targetRotationOnMouseDown;
        private float mouseXOnMouseDown;
        private bool moving;

        public GeometryShapesForm()
        {
            camera = new PerspectiveCamera(50, aspectRatio, 1, 1000);
            camera.position.set(0, 150, 500);

            scene = new Scene();

            var light = new DirectionalLight(0xFF << 24 | 0xffffff);
            light.position.set(0, 0, 1);
            scene.add(light);

            parent = new Object3D();
            parent.position.y = 50;
            scene.add(parent);

            // California

            var californiaPts = new JSArray();

            californiaPts.push(new Vector2(610, 320));
            californiaPts.push(new Vector2(450, 300));
            californiaPts.push(new Vector2(392, 392));
            californiaPts.push(new Vector2(266, 438));
            californiaPts.push(new Vector2(190, 570));
            californiaPts.push(new Vector2(190, 600));
            californiaPts.push(new Vector2(160, 620));
            californiaPts.push(new Vector2(160, 650));
            californiaPts.push(new Vector2(180, 640));
            californiaPts.push(new Vector2(165, 680));
            californiaPts.push(new Vector2(150, 670));
            californiaPts.push(new Vector2(90, 737));
            californiaPts.push(new Vector2(80, 795));
            californiaPts.push(new Vector2(50, 835));
            californiaPts.push(new Vector2(64, 870));
            californiaPts.push(new Vector2(60, 945));
            californiaPts.push(new Vector2(300, 945));
            californiaPts.push(new Vector2(300, 743));
            californiaPts.push(new Vector2(600, 473));
            californiaPts.push(new Vector2(626, 425));
            californiaPts.push(new Vector2(600, 370));
            californiaPts.push(new Vector2(610, 320));

            var californiaShape = new Shape(californiaPts);

            // Triangle

            var triangleShape = new Shape();
            triangleShape.moveTo(80, 20);
            triangleShape.lineTo(40, 80);
            triangleShape.lineTo(120, 80);
            triangleShape.lineTo(80, 20); // close path

            // Heart
            var x = 0.0;
            var y = 0.0;

            var heartShape = new Shape(); // From http://blog.burlock.org/html5/130-paths

            heartShape.moveTo(x + 25, y + 25);
            heartShape.bezierCurveTo(x + 25, y + 25, x + 20, y, x, y);
            heartShape.bezierCurveTo(x - 30, y, x - 30, y + 35, x - 30, y + 35);
            heartShape.bezierCurveTo(x - 30, y + 55, x - 10, y + 77, x + 25, y + 95);
            heartShape.bezierCurveTo(x + 60, y + 77, x + 80, y + 55, x + 80, y + 35);
            heartShape.bezierCurveTo(x + 80, y + 35, x + 80, y, x + 50, y);
            heartShape.bezierCurveTo(x + 35, y, x + 25, y + 25, x + 25, y + 25);

            // Square

            var sqLength = 80;

            var squareShape = new Shape();
            squareShape.moveTo(0, 0);
            squareShape.lineTo(0, sqLength);
            squareShape.lineTo(sqLength, sqLength);
            squareShape.lineTo(sqLength, 0);
            squareShape.lineTo(0, 0);

            // Rectangle

            double rectLength = 120, rectWidth = 40;

            var rectShape = new Shape();
            rectShape.moveTo(0, 0);
            rectShape.lineTo(0, rectWidth);
            rectShape.lineTo(rectLength, rectWidth);
            rectShape.lineTo(rectLength, 0);
            rectShape.lineTo(0, 0);

            // Rounded rectangle

            var roundedRectShape = new Shape();

            Action<Shape, double, double, double, double, double> roundedRect = (ctx, x1, y1, width, height, radius) =>
                                                                                {
                                                                                    ctx.moveTo(x1, y1 + radius);
                                                                                    ctx.lineTo(x1, y1 + height - radius);
                                                                                    ctx.quadraticCurveTo(x1, y1 + height, x1 + radius, y1 + height);
                                                                                    ctx.lineTo(x1 + width - radius, y1 + height);
                                                                                    ctx.quadraticCurveTo(x1 + width, y1 + height, x1 + width, y1 + height - radius);
                                                                                    ctx.lineTo(x1 + width, y1 + radius);
                                                                                    ctx.quadraticCurveTo(x1 + width, y1, x1 + width - radius, y1);
                                                                                    ctx.lineTo(x1 + radius, y1);
                                                                                    ctx.quadraticCurveTo(x1, y1, x1, y1 + radius);
                                                                                };

            roundedRect(roundedRectShape, 0.0, 0.0, 50.0, 50.0, 20.0);

            // Circle

            const double circleRadius = 40.0;
            var circleShape = new Shape();
            circleShape.moveTo(0, circleRadius);
            circleShape.quadraticCurveTo(circleRadius, circleRadius, circleRadius, 0);
            circleShape.quadraticCurveTo(circleRadius, -circleRadius, 0, -circleRadius);
            circleShape.quadraticCurveTo(-circleRadius, -circleRadius, -circleRadius, 0);
            circleShape.quadraticCurveTo(-circleRadius, circleRadius, 0, circleRadius);

            // Fish

            x = y = 0;

            var fishShape = new Shape();

            fishShape.moveTo(x, y);
            fishShape.quadraticCurveTo(x + 50, y - 80, x + 90, y - 10);
            fishShape.quadraticCurveTo(x + 100, y - 10, x + 115, y - 40);
            fishShape.quadraticCurveTo(x + 115, y, x + 115, y + 40);
            fishShape.quadraticCurveTo(x + 100, y + 10, x + 90, y + 10);
            fishShape.quadraticCurveTo(x + 50, y + 80, x, y);

            // Arc circle

            var arcShape = new Shape();
            arcShape.moveTo(50, 10);
            arcShape.absarc(10, 10, 40, 0, Math.PI * 2, false);

            var holePath = new Path();
            holePath.moveTo(20, 10);
            holePath.absarc(10, 10, 10, 0, Math.PI * 2, true);
            arcShape.holes.push(holePath);

            // Smiley

            var smileyShape = new Shape();
            smileyShape.moveTo(80, 40);
            smileyShape.absarc(40, 40, 40, 0, Math.PI * 2, false);

            var smileyEye1Path = new Path();
            smileyEye1Path.moveTo(35, 20);
            smileyEye1Path.absellipse(25, 20, 10, 10, 0, Math.PI * 2, true);

            smileyShape.holes.push(smileyEye1Path);

            var smileyEye2Path = new Path();
            smileyEye2Path.moveTo(65, 20);
            smileyEye2Path.absarc(55, 20, 10, 0, Math.PI * 2, true);
            smileyShape.holes.push(smileyEye2Path);

            var smileyMouthPath = new Path();

            smileyMouthPath.moveTo(20, 40);
            smileyMouthPath.quadraticCurveTo(40, 60, 60, 40);
            smileyMouthPath.bezierCurveTo(70, 45, 70, 50, 60, 60);
            smileyMouthPath.quadraticCurveTo(40, 80, 20, 60);
            smileyMouthPath.quadraticCurveTo(5, 50, 20, 40);

            smileyShape.holes.push(smileyMouthPath);

            // Spline shape + path extrusion

            var splinepts = new JSArray();
            splinepts.push(new Vector2(350, 100));
            splinepts.push(new Vector2(400, 450));
            splinepts.push(new Vector2(-140, 350));
            splinepts.push(new Vector2(0, 0));

            var splineShape = new Shape();
            splineShape.moveTo(0, 0);
            splineShape.splineThru(splinepts);

            var apath = new SplineCurve3();
            apath.points.push(new Vector3(-50, 150, 10));
            apath.points.push(new Vector3(-20, 180, 20));
            apath.points.push(new Vector3(40, 220, 50));
            apath.points.push(new Vector3(200, 290, 100));

            var extrudeSettings = JSObject.create(new {amount = 20});

            addShape(californiaShape, extrudeSettings, 0xffaa00, -300, -100, 0, 0, 0, 0, 0.25);

            extrudeSettings.bevelEnabled = true;
            extrudeSettings.bevelSegments = 2;
            extrudeSettings.steps = 2;

            addShape(triangleShape, extrudeSettings, 0xffee00, -180, 0, 0, 0, 0, 0, 1);
            addShape(roundedRectShape, extrudeSettings, 0x005500, -150, 150, 0, 0, 0, 0, 1);
            addShape(squareShape, extrudeSettings, 0x0055ff, 150, 100, 0, 0, 0, 0, 1);
            addShape(heartShape, extrudeSettings, 0xff1100, 60, 100, 0, 0, 0, Math.PI, 1);

            addShape(circleShape, extrudeSettings, 0x00ff11, 120, 250, 0, 0, 0, 0, 1);
            addShape(fishShape, extrudeSettings, 0x222222, -60, 200, 0, 0, 0, 0, 1);
            addShape(smileyShape, extrudeSettings, 0xee00ff, -200, 250, 0, 0, 0, Math.PI, 1);

            addShape(arcShape, extrudeSettings, 0xbb4422, 150, 0, 0, 0, 0, 0, 1);

            extrudeSettings.extrudePath = apath;
            extrudeSettings.bevelEnabled = false;
            extrudeSettings.steps = 20;

            addShape(splineShape, extrudeSettings, 0x888888, -50, -100, -50, 0, 0, 0, 0.2);

            renderer = new WebGLRenderer(new {canvas, antialias = true});
            renderer.setSize(ClientSize.Width, ClientSize.Height);
        }

        private void addShape(Shape shape, dynamic extrudeSettings, int color, dynamic x, dynamic y, dynamic z, dynamic rx, dynamic ry, dynamic rz, dynamic s)
        {
            var points = shape.createPointsGeometry();
            var spacedPoints = shape.createSpacedPointsGeometry(100.0);

            // flat shape

            dynamic geometry = new ShapeGeometry(shape);

            var mesh = SceneUtils.createMultiMaterialObject(geometry, new JSArray(new MeshLambertMaterial(JSObject.create(new {color})),
                                                                                new MeshBasicMaterial(JSObject.create(new {color = 0x000000, wireframe = true, transparent = true}))));
            mesh.position.set(x, y, z - 125);
            mesh.rotation.set(rx, ry, rz);
            mesh.scale.set(s, s, s);
            parent.add(mesh);

            // 3d shape

            geometry = new ExtrudeGeometry(shape, extrudeSettings);

            mesh = SceneUtils.createMultiMaterialObject(geometry, new JSArray(new MeshLambertMaterial(JSObject.create(new {color})),
                                                                            new MeshBasicMaterial(JSObject.create(new {color = 0x000000, wireframe = true, transparent = true}))));
            mesh.position.set(x, y, z - 75);
            mesh.rotation.set(rx, ry, rz);
            mesh.scale.set(s, s, s);
            parent.add(mesh);

            // solid line

            dynamic line = new Line(points, new LineBasicMaterial(JSObject.create(new {color, linewidth = 2.0})));
            line.position.set(x, y, z + 25);
            line.rotation.set(rx, ry, rz);
            line.scale.set(s, s, s);
            parent.add(line);

            // transparent line from real points

            line = new Line(points, new LineBasicMaterial(JSObject.create(new {color, opacity = 0.5})));
            line.position.set(x, y, z + 75);
            line.rotation.set(rx, ry, rz);
            line.scale.set(s, s, s);
            parent.add(line);

            // vertices from real points

            var pgeo = points.clone();
            var particles = new ParticleSystem(pgeo, new ParticleBasicMaterial(JSObject.create((dynamic)new
                                                                                                      {
                                                                                                          color,
                                                                                                          size = 2.0,
                                                                                                          opacity = 0.75
                                                                                                      })));
            particles.position.set(x, y, z + 75);
            particles.rotation.set(rx, ry, rz);
            particles.scale.set(s, s, s);
            parent.add(particles);

            // transparent line from equidistance sampled points

            line = new Line(spacedPoints, new LineBasicMaterial(JSObject.create((dynamic)new
                                                                                       {
                                                                                           color,
                                                                                           opacity = 0.2
                                                                                       })));
            line.position.set(x, y, z + 125);
            line.rotation.set(rx, ry, rz);
            line.scale.set(s, s, s);
            parent.add(line);

            // equidistance sampled points

            pgeo = spacedPoints.clone();
            var particles2 = new ParticleSystem(pgeo, new ParticleBasicMaterial(JSObject.create((dynamic)new
                                                                                                       {
                                                                                                           color,
                                                                                                           size = 2.0,
                                                                                                           opacity = 0.5
                                                                                                       })));
            particles2.position.set(x, y, z + 125);
            particles2.rotation.set(rx, ry, rz);
            particles2.scale.set(s, s, s);
            parent.add(particles2);
        }

        protected override void onMouseUp(MouseEventArgs mouseEventArgs)
        {
            moving = false;
        }

        protected override void onMouseDown(MouseEventArgs mouseEventArgs)
        {
            moving = true;

            mouseXOnMouseDown = mouseEventArgs.X - windowHalf.X;
            targetRotationOnMouseDown = targetRotation;
        }

        protected override void onMouseMove(MouseEventArgs mouseEventArgs)
        {
            if (moving)
            {
                var mouseX = mouseEventArgs.X - windowHalf.X;
                targetRotation = targetRotationOnMouseDown + (mouseX - mouseXOnMouseDown) * 0.02;
            }
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

        protected override void render()
        {
            parent.rotation.y += (targetRotation - parent.rotation.y) * 0.05;
            renderer.render(scene, camera);
        }
    }
}