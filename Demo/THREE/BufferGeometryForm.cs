using System;
using THREE;
using WebGL;
using Math = THREE.Math;

namespace Demo.THREE
{
    public class BufferGeometryForm : BaseForm
    {
        private readonly WebGLRenderer _renderer;
        private readonly PerspectiveCamera _camera;
        private readonly Scene _scene;
        private readonly Mesh _mesh;

        public BufferGeometryForm()
        {
            _camera = new PerspectiveCamera(27, aspectRatio, 1.0, 3500.0);
            _camera.position.z = 2750;

            _scene = new Scene {fog = new Fog(0x050505, 2000.0, 3500.0)};

            _scene.add(new AmbientLight(0x444444));

            var light1 = new DirectionalLight(0xffffff, 0.5);
            light1.position.set(1, 1, 1);
            _scene.add(light1);

            var light2 = new DirectionalLight(0xffffff, 1.5);
            light2.position.set(0, -1, 0);
            _scene.add(light2);

            const int triangles = 160000;

            var geometry = new BufferGeometry
                           {
                               attributes = JSObject.create((dynamic)new
                                                                   {
                                                                       index = new
                                                                               {
                                                                                   itemSize = 1,
                                                                                   array = new Uint16Array(triangles * 3),
                                                                                   numItems = triangles * 3
                                                                               },
                                                                       position = new
                                                                                  {
                                                                                      itemSize = 3,
                                                                                      array = new Float32Array(triangles * 3 * 3),
                                                                                      numItems = triangles * 3 * 3
                                                                                  },
                                                                       normal = new
                                                                                {
                                                                                    itemSize = 3,
                                                                                    array = new Float32Array(triangles * 3 * 3),
                                                                                    numItems = triangles * 3 * 3
                                                                                },
                                                                       color = new
                                                                               {
                                                                                   itemSize = 3,
                                                                                   array = new Float32Array(triangles * 3 * 3),
                                                                                   numItems = triangles * 3 * 3
                                                                               }
                                                                   })
                           };

            const int chunkSize = 21845;

            var indices = geometry.attributes.index.array;

            for (var i = 0; i < indices.length; i ++)
            {
                indices[i] = (ushort)(i % (3 * chunkSize));
            }

            var positions = geometry.attributes.position.array;
            var normals = geometry.attributes.normal.array;
            var colors = geometry.attributes.color.array;

            var color = new Color();

            const double n = 800.0;
            const double n2 = n / 2.0;
            const double d = 12.0;
            const double d2 = d / 2.0;

            var pA = new Vector3();
            var pB = new Vector3();
            var pC = new Vector3();

            var cb = new Vector3();
            var ab = new Vector3();

            for (var i = 0; i < positions.length; i += 9)
            {
                var x = Math.random() * n - n2;
                var y = Math.random() * n - n2;
                var z = Math.random() * n - n2;

                var ax = x + Math.random() * d - d2;
                var ay = y + Math.random() * d - d2;
                var az = z + Math.random() * d - d2;

                var bx = x + Math.random() * d - d2;
                var by = y + Math.random() * d - d2;
                var bz = z + Math.random() * d - d2;

                var cx = x + Math.random() * d - d2;
                var cy = y + Math.random() * d - d2;
                var cz = z + Math.random() * d - d2;

                positions[i] = (float)ax;
                positions[i + 1] = (float)ay;
                positions[i + 2] = (float)az;

                positions[i + 3] = (float)bx;
                positions[i + 4] = (float)by;
                positions[i + 5] = (float)bz;

                positions[i + 6] = (float)cx;
                positions[i + 7] = (float)cy;
                positions[i + 8] = (float)cz;

                // flat face normals

                pA.set(ax, ay, az);
                pB.set(bx, by, bz);
                pC.set(cx, cy, cz);

                cb.subVectors(pC, pB);
                ab.subVectors(pA, pB);
                cb.cross(ab);

                cb.normalize();

                var nx = cb.x;
                var ny = cb.y;
                var nz = cb.z;

                normals[i] = (float)nx;
                normals[i + 1] = (float)ny;
                normals[i + 2] = (float)nz;

                normals[i + 3] = (float)nx;
                normals[i + 4] = (float)ny;
                normals[i + 5] = (float)nz;

                normals[i + 6] = (float)nx;
                normals[i + 7] = (float)ny;
                normals[i + 8] = (float)nz;

                // colors
                var vx = (x / n) + 0.5;
                var vy = (y / n) + 0.5;
                var vz = (z / n) + 0.5;

                color.setRGB(vx, vy, vz);

                colors[i] = (float)color.r;
                colors[i + 1] = (float)color.g;
                colors[i + 2] = (float)color.b;

                colors[i + 3] = (float)color.r;
                colors[i + 4] = (float)color.g;
                colors[i + 5] = (float)color.b;

                colors[i + 6] = (float)color.r;
                colors[i + 7] = (float)color.g;
                colors[i + 8] = (float)color.b;
            }

            geometry.offsets = new JSArray();

            const int offsets = triangles / chunkSize;

            for (var i = 0; i < offsets; i ++)
            {
                var offset = JSObject.create((dynamic)new
                                                    {
                                                        start = i * chunkSize * 3,
                                                        index = i * chunkSize * 3,
                                                        count = System.Math.Min(triangles - (i * chunkSize), chunkSize) * 3
                                                    });

                geometry.offsets.push(offset);
            }

            geometry.computeBoundingSphere();

            var material = new MeshPhongMaterial(JSObject.create(new
                                                               {
                                                                   color = 0xaaaaaa,
                                                                   ambient = 0xaaaaaa,
                                                                   specular = 0xffffff,
                                                                   shininess = 250.0,
                                                                   side = global::THREE.THREE.DoubleSide,
                                                                   vertexColors = global::THREE.THREE.VertexColors
                                                               }));

            _mesh = new Mesh(geometry, material);
            _scene.add(_mesh);

            _renderer = new WebGLRenderer(new {canvas, antialias = false, clearColor = 0x333333, clearAlpha = 1, alpha = false});
            _renderer.setSize(ClientSize.Width, ClientSize.Height);
            _renderer.setClearColor(_scene.fog.color, 1);
            _renderer.gammaInput = true;
            _renderer.gammaOutput = true;
            _renderer.physicallyBasedShading = true;
        }

        protected override void onWindowResize(EventArgs e)
        {
            if (_camera != null)
            {
                _camera.aspect = aspectRatio;
                _camera.updateProjectionMatrix();

                _renderer.setSize(ClientSize.Width, ClientSize.Height);
            }
        }

        protected override void render()
        {
            var time = JSDate.now() * 0.001;

            _mesh.rotation.x = time * 0.25;
            _mesh.rotation.y = time * 0.5;

            _renderer.render(_scene, _camera);
        }
    }
}