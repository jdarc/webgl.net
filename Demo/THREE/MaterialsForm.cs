using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class MaterialsForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;
        private readonly JSArray objects;
        private readonly dynamic particleLight;
        private readonly dynamic pointLight;
        private readonly JSArray materials;

        public MaterialsForm()
        {
            camera = new PerspectiveCamera(45, aspectRatio, 1, 2000);
            camera.position.set(0, 200, 800);

            scene = new Scene();

            // Grid

            var lineMaterial = new LineBasicMaterial(JSObject.create(new {color = 0x303030}));
            var geometry = new Geometry();
            const int floor = -75;
            const int step = 25;

            for (var i = 0; i <= 40; i ++)
            {
                geometry.vertices.push(new Vector3(- 500, floor, i * step - 500));
                geometry.vertices.push(new Vector3(500, floor, i * step - 500));

                geometry.vertices.push(new Vector3(i * step - 500, floor, -500));
                geometry.vertices.push(new Vector3(i * step - 500, floor, 500));
            }

            var line = new Line(geometry, lineMaterial, global::THREE.THREE.LinePieces);
            scene.add(line);

            // Materials

            var texture = new Texture(generateTexture()) {needsUpdate = true};

            materials = new JSArray();
            materials.push(new MeshLambertMaterial(JSObject.create(new {map = texture, transparent = true})));
            materials.push(new MeshLambertMaterial(JSObject.create(new {color = 0xdddddd, shading = global::THREE.THREE.FlatShading})));
            materials.push(new MeshPhongMaterial(JSObject.create(new {ambient = 0x030303, color = 0xdddddd, specular = 0x009900, shininess = 30, shading = global::THREE.THREE.FlatShading})));
            materials.push(new MeshNormalMaterial());
            materials.push(new MeshBasicMaterial(JSObject.create(new {color = 0xffaa00, transparent = true, blending = global::THREE.THREE.AdditiveBlending})));

            materials.push(new MeshLambertMaterial(JSObject.create(new {color = 0xdddddd, shading = global::THREE.THREE.SmoothShading})));
            materials.push(new MeshPhongMaterial(JSObject.create(new {ambient = 0x030303, color = 0xdddddd, specular = 0x009900, shininess = 30, shading = global::THREE.THREE.SmoothShading, map = texture, transparent = true})));
            materials.push(new MeshNormalMaterial(JSObject.create(new {shading = global::THREE.THREE.SmoothShading})));
            materials.push(new MeshBasicMaterial(JSObject.create(new {color = 0xffaa00, wireframe = true})));

            materials.push(new MeshDepthMaterial());

            materials.push(new MeshLambertMaterial(JSObject.create(new {color = 0x666666, emissive = 0xff0000, ambient = 0x000000, shading = global::THREE.THREE.SmoothShading})));
            materials.push(new MeshPhongMaterial(JSObject.create(new {color = 0x000000, specular = 0x666666, emissive = 0xff0000, ambient = 0x000000, shininess = 10, shading = global::THREE.THREE.SmoothShading, opacity = 0.9, transparent = true})));

            materials.push(new MeshBasicMaterial(JSObject.create(new {map = texture, transparent = true})));

            // Spheres geometry

            var geometrySmooth = new SphereGeometry(70, 32, 16);
            var geometryFlat = new SphereGeometry(70, 32, 16);
            dynamic geometryPieces = new SphereGeometry(70, 32, 16); // Extra geometry to be broken down for MeshFaceMaterial

            for (var i = 0; i < geometryPieces.faces.length; i ++)
            {
                var face = geometryPieces.faces[i];
                face.materialIndex = (int)System.Math.Floor(Math.random() * materials.length);
            }

            geometryPieces.materials = materials;

            materials.push(new MeshFaceMaterial(materials));

            objects = new JSArray();

            for (var i = 0; i < materials.length; i ++)
            {
                var material = materials[i];

                geometry = material is MeshFaceMaterial
                               ? geometryPieces
                               : (material.shading == global::THREE.THREE.FlatShading ? geometryFlat : geometrySmooth);

                var sphere = new Mesh(geometry, material) {position = {x = (i % 4) * 200 - 400, z = System.Math.Floor(i / 4.0) * 200 - 200}};

                sphere.rotation.x = Math.random() * 200 - 100;
                sphere.rotation.y = Math.random() * 200 - 100;
                sphere.rotation.z = Math.random() * 200 - 100;

                objects.push(sphere);

                scene.add(sphere);
            }

            particleLight = new Mesh(new SphereGeometry(4, 8, 8), new MeshBasicMaterial(JSObject.create(new {color = 0xffffff})));
            scene.add(particleLight);

            // Lights

            scene.add(new AmbientLight(0x111111));

            var directionalLight = new DirectionalLight( /*Math.random() * */ 0xffffff, 0.125) {position = {x = Math.random() - 0.5, y = Math.random() - 0.5, z = Math.random() - 0.5}};

            directionalLight.position.normalize();

            scene.add(directionalLight);

            pointLight = new PointLight(0xffffff, 1);
            scene.add(pointLight);

            renderer = new WebGLRenderer(new {canvas, antialias = true});
            renderer.setSize(ClientSize.Width, ClientSize.Height);
        }

        private Image generateTexture()
        {
            var data = new byte[256 * 256 * 4];
            var y = 0;
            for (int i = 0, j = 0; i < data.Length; i += 4, j++)
            {
                var x = j % 256;
                y = x == 0 ? y + 1 : y;

                data[i] = 255;
                data[i + 1] = 255;
                data[i + 2] = 255;
                data[i + 3] = (byte)System.Math.Floor((double)(x ^ y));
            }

            return new Image(data, 256, 256);
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
            var timer = 0.0001 * JSDate.now();

            camera.position.x = System.Math.Cos(timer) * 1000;
            camera.position.z = System.Math.Sin(timer) * 1000;

            camera.lookAt(scene.position);

            for (var i = 0; i < objects.length; i ++)
            {
                var obj = objects[i];

                obj.rotation.x += 0.01;
                obj.rotation.y += 0.005;
            }

            materials[materials.length - 3].emissive.setHSV(0.54, 1, 0.7 * (0.5 + 0.5 * System.Math.Sin(35 * timer)));
            materials[materials.length - 4].emissive.setHSV(0.04, 1, 0.7 * (0.5 + 0.5 * System.Math.Cos(35 * timer)));

            particleLight.position.x = System.Math.Sin(timer * 7) * 300;
            particleLight.position.y = System.Math.Cos(timer * 5) * 400;
            particleLight.position.z = System.Math.Cos(timer * 3) * 300;

            pointLight.position.x = particleLight.position.x;
            pointLight.position.y = particleLight.position.y;
            pointLight.position.z = particleLight.position.z;

            renderer.render(scene, camera);
        }
    }
}