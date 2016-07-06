using System;
using System.Windows.Forms;
using THREE;
using WebGL;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class SpritesForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;
        private Object3D group;
        private double time;
        private Texture mapC;

        public SpritesForm()
        {
            camera = new PerspectiveCamera(60, aspectRatio, 1, 2100) {position = {z = 1500}};

            scene = new Scene {fog = new Fog(0x000000, 1500, 2100)};

            // create sprites

            const int amount = 200;
            const int radius = 500;

            // add 2d-sprites

            ImageUtils.loadTexture("textures/sprite0.png", null, x => createHUDSprites(x));
            var mapB = ImageUtils.loadTexture("textures/sprite1.png");
            mapC = ImageUtils.loadTexture("textures/sprite2.png");

            group = new Object3D();

            var materialC = new SpriteMaterial(JSObject.create(new {map = mapC, useScreenCoordinates = false, color = 0xffffff, fog = true}));
            var materialB = new SpriteMaterial(JSObject.create(new {map = mapB, useScreenCoordinates = false, color = 0xffffff, fog = true}));

            for (var a = 0; a < amount; a++)
            {
                var x = Math.random() - 0.5;
                var y = Math.random() - 0.5;
                var z = Math.random() - 0.5;

                SpriteMaterial material;
                if (z < 0)
                {
                    material = materialB.clone();
                }
                else
                {
                    material = materialC.clone();
                    material.color.setHSV(0.5 * Math.random(), 0.8, 0.9);
                    material.uvScale.set(2, 2);
                    material.uvOffset.set(-0.5, -0.5);
                }

                var sprite = new Sprite(material);

                sprite.position.set(x, y, z);
                sprite.position.normalize();
                sprite.position.multiplyScalar(radius);

                group.add(sprite);
            }

            scene.add(group);

            // renderer

            renderer = new WebGLRenderer(new {canvas});
            renderer.setClearColorHex(0x000000, 1.0);
            renderer.setSize(ClientSize.Width, ClientSize.Height);
        }

        private void createHUDSprites(dynamic texture)
        {
            var scaleX = texture.image.width;
            var scaleY = texture.image.height;

            var materialA1 = new SpriteMaterial(JSObject.create(new {map = texture, alignment = global::THREE.THREE.SpriteAlignment.topLeft, opacity = 0.25}));
            var materialA2 = new SpriteMaterial(JSObject.create(new {map = texture, alignment = global::THREE.THREE.SpriteAlignment.topLeft, opacity = 0.5}));
            var materialA3 = new SpriteMaterial(JSObject.create(new {map = texture, alignment = global::THREE.THREE.SpriteAlignment.topLeft, opacity = 1.0}));

            var sprite = new Sprite(materialA1);
            sprite.position.set(100, 100, 0);
            sprite.scale.set(scaleX, scaleY, 1);
            scene.add(sprite);

            sprite = new Sprite(materialA2);
            sprite.position.set(150, 150, 2);
            sprite.scale.set(scaleX, scaleY, 1);
            scene.add(sprite);

            sprite = new Sprite(materialA3);
            sprite.position.set(200, 200, 3);
            sprite.scale.set(scaleX, scaleY, 1);
            scene.add(sprite);
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
            for (var c = 0; c < group.children.length; c++)
            {
                var sprite = group.children[c];
                var material = sprite.material;
                var scale = Math.sin(time + sprite.position.x * 0.01) * 0.3 + 1.0;

                var imageWidth = 1.0;
                var imageHeight = 1.0;

                if (JSObject.eval(material.map) && JSObject.eval(material.map.image) && JSObject.eval(material.map.image.width))
                {
                    imageWidth = material.map.image.width;
                    imageHeight = material.map.image.height;
                }

                sprite.rotation += 0.1 * (c / (double)group.children.length);
                sprite.scale.set(scale * imageWidth, scale * imageHeight, 1.0);

                if (material.map != mapC)
                {
                    material.opacity = Math.sin(time + sprite.position.x * 0.01) * 0.4 + 0.6;
                }
            }

            group.rotation.x = time * 0.5;
            group.rotation.y = time * 0.75;
            group.rotation.z = time * 1.0;

            time += 0.02;

            renderer.render(scene, camera);
        }
    }
}