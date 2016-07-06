using System;
using System.Windows.Forms;
using THREE;
using WebGL;

namespace Demo.THREE
{
    public class MaterialsTextureCompressedForm : BaseForm
    {
        private readonly WebGLRenderer renderer;
        private readonly PerspectiveCamera camera;
        private readonly Scene scene;
        private readonly JSArray meshes = new JSArray();

        public MaterialsTextureCompressedForm()
        {
            camera = new PerspectiveCamera(50, aspectRatio, 1, 2000);
            camera.position.z = 1000;

            scene = new Scene();

            var geometry = new CubeGeometry(200, 200, 200);

            /*
				This is how compressed textures are supposed to be used:

				DXT1 - RGB - opaque textures
				DXT3 - RGBA - transparent textures with sharp alpha transitions
				DXT5 - RGBA - transparent textures with full alpha range
				*/

            var map1 = ImageUtils.loadCompressedTexture("textures/compressed/disturb_dxt1_nomip.dds");
            map1.minFilter = map1.magFilter = global::THREE.THREE.LinearFilter;
            map1.anisotropy = 4;

            var map2 = ImageUtils.loadCompressedTexture("textures/compressed/disturb_dxt1_mip.dds");
            map2.anisotropy = 4;

            var map3 = ImageUtils.loadCompressedTexture("textures/compressed/hepatica_dxt3_mip.dds");
            map3.anisotropy = 4;

            var map4 = ImageUtils.loadCompressedTexture("textures/compressed/explosion_dxt5_mip.dds");
            map4.anisotropy = 4;

            var cubemap = ImageUtils.loadCompressedTextureCube("textures/compressed/Mountains.dds", global::THREE.THREE.CubeReflectionMapping, cmap => { cmap.magFilter = cmap.minFilter = global::THREE.THREE.LinearFilter; });

            var material1 = new MeshBasicMaterial(JSObject.create(new {map = map1, envMap = cubemap}));
            var material2 = new MeshBasicMaterial(JSObject.create(new {map = map2}));
            var material3 = new MeshBasicMaterial(JSObject.create(new {map = map3, alphaTest = 0.5, side = global::THREE.THREE.DoubleSide}));
            var material4 = new MeshBasicMaterial(JSObject.create(new {map = map4, side = global::THREE.THREE.DoubleSide, blending = global::THREE.THREE.AdditiveBlending, depthTest = false, transparent = true}));

            var mesh = new Mesh(new TorusGeometry(100, 50, 32, 16), material1);
            mesh.position.x = -200;
            mesh.position.y = -200;
            scene.add(mesh);
            meshes.push(mesh);

            mesh = new Mesh(geometry, material2);
            mesh.position.x = 200;
            mesh.position.y = -200;
            scene.add(mesh);
            meshes.push(mesh);

            mesh = new Mesh(geometry, material3);
            mesh.position.x = 200;
            mesh.position.y = 200;
            scene.add(mesh);
            meshes.push(mesh);

            mesh = new Mesh(geometry, material4);
            mesh.position.x = -200;
            mesh.position.y = 200;
            scene.add(mesh);
            meshes.push(mesh);

            renderer = new WebGLRenderer(new {canvas, antialias = true});
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

            for (var i = 0; i < meshes.length; i++)
            {
                var mesh = meshes[i];
                mesh.rotation.x = time;
                mesh.rotation.y = time;
            }

            renderer.render(scene, camera);
        }
    }
}