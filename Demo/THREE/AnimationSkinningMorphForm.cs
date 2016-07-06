using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = System.Math;

namespace Demo.THREE
{
    public class AnimationSkinningMorphForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;
        private int mouseX;
        private int mouseY;
        private Mesh mesh;
        private Animation animation;
        private readonly Clock clock = new Clock();

        public AnimationSkinningMorphForm()
        {
            const int FLOOR = -250;
            camera = new PerspectiveCamera(30, aspectRatio, 1, 10000);
            camera.position.z = 2200;

            scene = new Scene();

            scene.fog = new Fog(0xffffff, 2000, 10000);
            scene.fog.color.setHSV(0.6, 0, 1);

            scene.add(camera);

            var groundMaterial = new MeshPhongMaterial(JSObject.create(new {emissive = 0xbbbbbb}));
            var planeGeometry = new PlaneGeometry(16000, 16000);

            var ground = new Mesh(planeGeometry, groundMaterial);
            ground.position.set(0, FLOOR, 0);
            ground.rotation.x = -Math.PI / 2;
            scene.add(ground);

            ground.receiveShadow = true;

            // LIGHTS

            var ambient = new AmbientLight(0x222222);
            scene.add(ambient);

            var light = new DirectionalLight(0xffffff, 1.6);
            light.position.set(0, 140, 500);
            light.position.multiplyScalar(1.1);
            light.color.setHSV(0.6, 0.075, 1);
            scene.add(light);

            light.castShadow = true;

            light.shadowMapWidth = 2048;
            light.shadowMapHeight = 2048;

            var d = 390.0;

            light.shadowCameraLeft = -d * 2;
            light.shadowCameraRight = d * 2;
            light.shadowCameraTop = d * 1.5;
            light.shadowCameraBottom = -d;

            light.shadowCameraFar = 3500;
            //light.shadowCameraVisible = true;

            //

            light = new DirectionalLight(0xffffff, 1);
            light.position.set(0, -1, 0);
            light.color.setHSV(0.25, 0.85, 0.5);
            scene.add(light);

            // RENDERER

            renderer = new WebGLRenderer(new {canvas, antialias = true});
            renderer.setSize(ClientSize.Width, ClientSize.Height);

            renderer.setClearColor(scene.fog.color, 1);

            renderer.gammaInput = true;
            renderer.gammaOutput = true;
            renderer.physicallyBasedShading = true;

            renderer.shadowMapEnabled = true;

            var loader = new JSONLoader();
            loader.load("models/skinned/knight.js", (geometry, materials) => createScene(geometry, materials, 0, FLOOR, -300, 60));
        }

        private void createScene(dynamic geometry, dynamic materials, dynamic x, dynamic y, dynamic z, dynamic s)
        {
            ensureLoop(geometry.animation);

            geometry.computeBoundingBox();
            var bb = geometry.boundingBox;

            AnimationHandler.add(geometry.animation);

            for (var i = 0; i < materials.length; i ++)
            {
                var m = materials[i];
                m.skinning = true;
                m.morphTargets = true;

                m.specular.setHSV(0.0, 0.0, 0.1);

                m.color.setHSV(0.6, 0.0, 0.6);
                m.ambient.copy(m.color);

                m.wrapAround = true;
            }

            mesh = new SkinnedMesh(geometry, new MeshFaceMaterial(materials));
            mesh.position.set(x, y - bb.min.y * s, z);
            mesh.scale.set(s, s, s);
            scene.add(mesh);

            mesh.castShadow = true;
            mesh.receiveShadow = true;

            animation = new Animation(mesh, geometry.animation.name);
            animation.interpolationType = AnimationHandler.LINEAR;

            animation.play();
        }

        private void ensureLoop(dynamic anim)
        {
            for (var i = 0; i < anim.hierarchy.length; i ++)
            {
                var bone = anim.hierarchy[i];

                var first = bone["keys"][0];
                var last = bone["keys"][bone["keys"].length - 1];

                last.pos = first.pos;
                last.rot = first.rot;
                last.scl = first.scl;
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
            var delta = 0.75 * clock.getDelta();

            camera.position.x += (mouseX - camera.position.x) * .05;
            camera.position.y = global::THREE.Math.clamp(camera.position.y + (-mouseY - camera.position.y) * .05, 0.0, 1000.0);

            camera.lookAt(scene.position);

            // update skinning
            AnimationHandler.update(delta);

            // update morphs
            if (mesh != null)
            {
                var time = JSDate.now() * 0.001;

                // mouth
                mesh.morphTargetInfluences[1] = (1.0 + Math.Sin(4.0 * time)) / 2.0;

                // frown ?
                mesh.morphTargetInfluences[2] = (1.0 + Math.Sin(2.0 * time)) / 2.0;

                // eyes
                mesh.morphTargetInfluences[3] = (1.0 + Math.Cos(4.0 * time)) / 2.0;
            }

            renderer.render(scene, camera);
        }
    }
}