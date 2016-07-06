using System;
using System.Collections.Generic;
using System.Linq;
using WebGL;
using Console = System.Console;

namespace Demo.Benchmark
{
    internal class Renderer
    {
        internal HTMLCanvasElement domElement;
        internal WebGLRenderingContext context;

        internal bool autoClear;
        internal bool autoClearColor;
        internal bool autoClearDepth;
        internal bool autoClearStencil;
        internal bool autoUpdateObjects;
        internal bool autoUpdateScene;
        internal bool gammaInput;
        internal bool gammaOutput;

        private int _programsCounter;
        private GLProgram _currentProgram;
        private int _currentGeometryGroupHash;
        private Camera _currentCamera;
        private int _geometryGroupCounter;
        private int _viewportX;
        private int _viewportY;
        private int _viewportWidth;
        private int _viewportHeight;
        private readonly bool[] _enabledAttributes;
        private readonly Frustum _frustum;
        private readonly Matrix4 _projScreenMatrix;
        private readonly WebGLExtension _glExtensionTextureFloat;
        private readonly WebGLExtension _glExtensionStandardDerivatives;
        private readonly WebGLExtension _glExtensionTextureFilterAnisotropic;
        private readonly WebGLExtension _glExtensionCompressedTextureS3TC;
        private readonly WebGLRenderingContext _gl;
        private readonly HTMLCanvasElement _canvas;
        private readonly string _precision;
        private readonly Color _clearColor;
        private readonly double _clearAlpha;
        private readonly WebGLShaderPrecisionFormat _vertexShaderPrecisionHighpFloat;
        private readonly WebGLShaderPrecisionFormat _vertexShaderPrecisionMediumpFloat;
        private readonly WebGLShaderPrecisionFormat _fragmentShaderPrecisionHighpFloat;
        private readonly WebGLShaderPrecisionFormat _fragmentShaderPrecisionMediumpFloat;

        internal Renderer(HTMLCanvasElement canvas)
        {
            _canvas = canvas;

            _precision = "highp";

            _clearColor = new Color();
            _clearAlpha = 0;

            domElement = _canvas;
            context = null;

            autoClear = true;
            autoClearColor = true;
            autoClearDepth = true;
            autoClearStencil = true;

            autoUpdateObjects = true;
            autoUpdateScene = true;

            gammaInput = false;
            gammaOutput = false;

            _programsCounter = 0;

            _currentProgram = null;
            _currentGeometryGroupHash = -1;
            _currentCamera = null;
            _geometryGroupCounter = 0;

            _viewportX = 0;
            _viewportY = 0;
            _viewportWidth = 0;
            _viewportHeight = 0;

            _frustum = new Frustum();

            _projScreenMatrix = new Matrix4();

            var attributes = new WebGLContextAttributes();
            attributes.setAlpha(true);
            attributes.setPremultipliedAlpha(true);
            attributes.setAntialias(false);
            attributes.setStencil(true);
            attributes.setPreserveDrawingBuffer(false);

            if ((_gl = (WebGLRenderingContext)_canvas.getContext("webgl", attributes)) == null)
            {
                throw new ApplicationException("Error creating WebGL context.");
            }
            _glExtensionTextureFloat = (WebGLExtension)_gl.getExtension("OES_texture_float");
            _glExtensionStandardDerivatives = (WebGLExtension)_gl.getExtension("OES_standard_derivatives");

            _glExtensionTextureFilterAnisotropic = (WebGLExtension)(_gl.getExtension("EXT_texture_filter_anisotropic") ??
                                                                    _gl.getExtension("MOZ_EXT_texture_filter_anisotropic") ??
                                                                    _gl.getExtension("WEBKIT_EXT_texture_filter_anisotropic"));

            _glExtensionCompressedTextureS3TC = (WebGLExtension)(_gl.getExtension("WEBGL_compressed_texture_s3tc") ??
                                                                 _gl.getExtension("MOZ_WEBGL_compressed_texture_s3tc") ??
                                                                 _gl.getExtension("WEBKIT_WEBGL_compressed_texture_s3tc"));

            if (_glExtensionTextureFloat == null)
            {
                Console.WriteLine(@"THREE.WebGLRenderer: Float textures not supported.");
            }

            if (_glExtensionStandardDerivatives == null)
            {
                Console.WriteLine(@"THREE.WebGLRenderer: Standard derivatives not supported.");
            }

            if (_glExtensionTextureFilterAnisotropic == null)
            {
                Console.WriteLine(@"THREE.WebGLRenderer: Anisotropic texture filtering not supported.");
            }

            if (_glExtensionCompressedTextureS3TC == null)
            {
                Console.WriteLine(@"THREE.WebGLRenderer: S3TC compressed textures not supported.");
            }

            _gl.clearColor(0, 0, 0, 1);
            _gl.clearDepth(1);
            _gl.clearStencil(0);

            _gl.enable(_gl.DEPTH_TEST);
            _gl.depthFunc(_gl.LEQUAL);

            _gl.frontFace(_gl.CCW);
            _gl.cullFace(_gl.BACK);
            _gl.enable(_gl.CULL_FACE);

            _gl.enable(_gl.BLEND);
            _gl.blendEquation(_gl.FUNC_ADD);
            _gl.blendFunc(_gl.SRC_ALPHA, _gl.ONE_MINUS_SRC_ALPHA);

            _gl.clearColor((float)_clearColor.r, (float)_clearColor.g, (float)_clearColor.b, (float)_clearAlpha);

            context = _gl;

            _enabledAttributes = new bool[context.getParameter(context.MAX_VERTEX_ATTRIBS)];

            _gl.getParameter(_gl.MAX_TEXTURE_IMAGE_UNITS);
            _gl.getParameter(_gl.MAX_VERTEX_TEXTURE_IMAGE_UNITS);
            _gl.getParameter(_gl.MAX_CUBE_MAP_TEXTURE_SIZE);

            _vertexShaderPrecisionHighpFloat = _gl.getShaderPrecisionFormat(_gl.VERTEX_SHADER, _gl.HIGH_FLOAT);
            _vertexShaderPrecisionMediumpFloat = _gl.getShaderPrecisionFormat(_gl.VERTEX_SHADER, _gl.MEDIUM_FLOAT);

            _fragmentShaderPrecisionHighpFloat = _gl.getShaderPrecisionFormat(_gl.FRAGMENT_SHADER, _gl.HIGH_FLOAT);
            _fragmentShaderPrecisionMediumpFloat = _gl.getShaderPrecisionFormat(_gl.FRAGMENT_SHADER, _gl.MEDIUM_FLOAT);

            var highpAvailable = _vertexShaderPrecisionHighpFloat.precision() > 0 && _fragmentShaderPrecisionHighpFloat.precision() > 0;
            var mediumpAvailable = _vertexShaderPrecisionMediumpFloat.precision() > 0 && _fragmentShaderPrecisionMediumpFloat.precision() > 0;

            if (_precision == "highp" && ! highpAvailable)
            {
                if (mediumpAvailable)
                {
                    _precision = "mediump";
                    Console.WriteLine(@"WebGLRenderer: highp not supported, using mediump");
                }
                else
                {
                    _precision = "lowp";
                    Console.WriteLine(@"WebGLRenderer: highp and mediump not supported, using lowp");
                }
            }

            if (_precision == "mediump" && ! mediumpAvailable)
            {
                _precision = "lowp";
                Console.WriteLine(@"WebGLRenderer: mediump not supported, using lowp");
            }
        }

        internal void render(Scene scene, Camera camera)
        {
            if (autoUpdateScene)
            {
                scene.updateMatrixWorld();
            }

            if (camera.parent == null)
            {
                camera.updateMatrixWorld();
            }

            camera.matrixWorldInverse.getInverse(camera.matrixWorld);

            _projScreenMatrix.multiplyMatrices(camera.projectionMatrix, camera.matrixWorldInverse);
            _frustum.setFromMatrix(_projScreenMatrix);

            if (autoUpdateObjects)
            {
                if (scene.__webglObjects == null)
                {
                    scene.__webglObjects = new List<WebGLRenderObject>();
                }

                while (scene.__objectsAdded.Count > 0)
                {
                    addObject(scene.__objectsAdded[0], scene);
                    scene.__objectsAdded.RemoveAt(0);
                }

                var webglObjects = scene.__webglObjects;
                foreach (var t in webglObjects)
                {
                    var geometry = t.obj.geometry;

                    var length = geometry.geometryGroupsList.Count;
                    for (var i = 0; i < length; i++)
                    {
                        var geometryGroup = geometry.geometryGroupsList[i];

                        if (geometry.verticesNeedUpdate || geometry.elementsNeedUpdate || geometry.normalsNeedUpdate)
                        {
                            if (geometryGroup.__inittedArrays)
                            {
                                var vertexIndex = 0;
                                var offset = 0;
                                var offsetFace = 0;
                                var offsetNormal = 0;
                                var vertexArray = geometryGroup.__vertexArray;
                                var normalArray = geometryGroup.__normalArray;
                                var faceArray = geometryGroup.__faceArray;
                                var dirtyVertices = t.obj.geometry.verticesNeedUpdate;
                                var dirtyElements = t.obj.geometry.elementsNeedUpdate;
                                var dirtyNormals = t.obj.geometry.normalsNeedUpdate;
                                var vertices = t.obj.geometry.vertices;
                                var chunkFaces3 = geometryGroup.faces3;
                                var chunkFaces4 = geometryGroup.faces4;
                                var objFaces = t.obj.geometry.faces;

                                if (dirtyVertices)
                                {
                                    foreach (var t1 in chunkFaces3)
                                    {
                                        var face = objFaces[t1];

                                        var v1 = vertices[face.a];
                                        var v2 = vertices[face.b];
                                        var v3 = vertices[face.c];

                                        vertexArray[offset] = (float)v1.x;
                                        vertexArray[offset + 1] = (float)v1.y;
                                        vertexArray[offset + 2] = (float)v1.z;

                                        vertexArray[offset + 3] = (float)v2.x;
                                        vertexArray[offset + 4] = (float)v2.y;
                                        vertexArray[offset + 5] = (float)v2.z;

                                        vertexArray[offset + 6] = (float)v3.x;
                                        vertexArray[offset + 7] = (float)v3.y;
                                        vertexArray[offset + 8] = (float)v3.z;

                                        offset += 9;
                                    }

                                    foreach (var t2 in chunkFaces4)
                                    {
                                        var face = objFaces[t2];

                                        var v1 = vertices[face.a];
                                        var v2 = vertices[face.b];
                                        var v3 = vertices[face.c];
                                        var v4 = vertices[face.d];

                                        vertexArray[offset] = (float)v1.x;
                                        vertexArray[offset + 1] = (float)v1.y;
                                        vertexArray[offset + 2] = (float)v1.z;

                                        vertexArray[offset + 3] = (float)v2.x;
                                        vertexArray[offset + 4] = (float)v2.y;
                                        vertexArray[offset + 5] = (float)v2.z;

                                        vertexArray[offset + 6] = (float)v3.x;
                                        vertexArray[offset + 7] = (float)v3.y;
                                        vertexArray[offset + 8] = (float)v3.z;

                                        vertexArray[offset + 9] = (float)v4.x;
                                        vertexArray[offset + 10] = (float)v4.y;
                                        vertexArray[offset + 11] = (float)v4.z;

                                        offset += 12;
                                    }

                                    _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglVertexBuffer);
                                    _gl.bufferData(_gl.ARRAY_BUFFER, vertexArray, _gl.DYNAMIC_DRAW);
                                }

                                if (dirtyNormals)
                                {
                                    foreach (var t3 in chunkFaces3)
                                    {
                                        var face = objFaces[t3];

                                        var vertexNormals = face.vertexNormals;
                                        var faceNormal = face.normal;

                                        if (vertexNormals.Count == 3)
                                        {
                                            for (var i1 = 0; i1 < 3; i1++)
                                            {
                                                var vn = vertexNormals[i1];

                                                normalArray[offsetNormal] = (float)vn.x;
                                                normalArray[offsetNormal + 1] = (float)vn.y;
                                                normalArray[offsetNormal + 2] = (float)vn.z;

                                                offsetNormal += 3;
                                            }
                                        }
                                        else
                                        {
                                            for (var i2 = 0; i2 < 3; i2++)
                                            {
                                                normalArray[offsetNormal] = (float)faceNormal.x;
                                                normalArray[offsetNormal + 1] = (float)faceNormal.y;
                                                normalArray[offsetNormal + 2] = (float)faceNormal.z;

                                                offsetNormal += 3;
                                            }
                                        }
                                    }

                                    foreach (var t4 in chunkFaces4)
                                    {
                                        var face = objFaces[t4];

                                        var vertexNormals = face.vertexNormals;
                                        var faceNormal = face.normal;

                                        if (vertexNormals.Count == 4)
                                        {
                                            for (var i3 = 0; i3 < 4; i3++)
                                            {
                                                var vn = vertexNormals[i3];

                                                normalArray[offsetNormal] = (float)vn.x;
                                                normalArray[offsetNormal + 1] = (float)vn.y;
                                                normalArray[offsetNormal + 2] = (float)vn.z;

                                                offsetNormal += 3;
                                            }
                                        }
                                        else
                                        {
                                            for (var i4 = 0; i4 < 4; i4++)
                                            {
                                                normalArray[offsetNormal] = (float)faceNormal.x;
                                                normalArray[offsetNormal + 1] = (float)faceNormal.y;
                                                normalArray[offsetNormal + 2] = (float)faceNormal.z;

                                                offsetNormal += 3;
                                            }
                                        }
                                    }

                                    _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglNormalBuffer);
                                    _gl.bufferData(_gl.ARRAY_BUFFER, normalArray, _gl.DYNAMIC_DRAW);
                                }

                                if (dirtyElements)
                                {
                                    foreach (var ignore in chunkFaces3)
                                    {
                                        faceArray[offsetFace] = (ushort)vertexIndex;
                                        faceArray[offsetFace + 1] = (ushort)(vertexIndex + 1);
                                        faceArray[offsetFace + 2] = (ushort)(vertexIndex + 2);

                                        offsetFace += 3;

                                        vertexIndex += 3;
                                    }

                                    foreach (var ignore in chunkFaces4)
                                    {
                                        faceArray[offsetFace] = (ushort)vertexIndex;
                                        faceArray[offsetFace + 1] = (ushort)(vertexIndex + 1);
                                        faceArray[offsetFace + 2] = (ushort)(vertexIndex + 3);

                                        faceArray[offsetFace + 3] = (ushort)(vertexIndex + 1);
                                        faceArray[offsetFace + 4] = (ushort)(vertexIndex + 2);
                                        faceArray[offsetFace + 5] = (ushort)(vertexIndex + 3);

                                        offsetFace += 6;

                                        vertexIndex += 4;
                                    }

                                    _gl.bindBuffer(_gl.ELEMENT_ARRAY_BUFFER, geometryGroup.__webglFaceBuffer);
                                    _gl.bufferData(_gl.ELEMENT_ARRAY_BUFFER, faceArray, _gl.DYNAMIC_DRAW);
                                }
                            }
                        }
                    }

                    geometry.verticesNeedUpdate = false;
                    geometry.elementsNeedUpdate = false;
                    geometry.normalsNeedUpdate = false;
                }
            }

            if (autoClear)
            {
                clear(autoClearColor, autoClearDepth, autoClearStencil);
            }

            var renderList = scene.__webglObjects;
            foreach (var webglObject in renderList)
            {
                var obj = webglObject.obj;

                webglObject.render = false;
                webglObject.material = webglObject.obj.material;
                if (obj.visible && (! (obj.frustumCulled) || _frustum.intersectsObject(obj)))
                {
                    obj._modelViewMatrix.multiplyMatrices(camera.matrixWorldInverse, obj.matrixWorld);

                    obj._normalMatrix.getInverse(obj._modelViewMatrix);
                    obj._normalMatrix.transpose();
                    webglObject.render = true;
                }
            }

            _gl.disable(_gl.BLEND);

            foreach (var renderObject in scene.__webglObjects.Where(renderObject => renderObject.render).Where(renderObject => renderObject.material != null))
            {
                _gl.enable(_gl.DEPTH_TEST);
                _gl.depthMask(true);
                _gl.disable(_gl.POLYGON_OFFSET_FILL);

                _gl.enable(_gl.CULL_FACE);
                _gl.frontFace(_gl.CCW);

                var program = setProgram(camera, renderObject.material, renderObject.obj);
                var attributes = program.attributes;
                var updateBuffers = false;
                var geometryGroupHash = (renderObject.buffer.id * 0xffffff) + (program.id * 2);

                if (geometryGroupHash != _currentGeometryGroupHash)
                {
                    _currentGeometryGroupHash = geometryGroupHash;
                    updateBuffers = true;
                }

                if (updateBuffers)
                {
                    for (var i = 0; i < _enabledAttributes.Length; i++)
                    {
                        if (_enabledAttributes[i])
                        {
                            _gl.disableVertexAttribArray((uint)i);
                            _enabledAttributes[i] = false;
                        }
                    }
                }

                if (attributes.position >= 0 && updateBuffers)
                {
                    _gl.bindBuffer(_gl.ARRAY_BUFFER, renderObject.buffer.__webglVertexBuffer);
                    enableAttribute(attributes.position);
                    _gl.vertexAttribPointer((uint)attributes.position, 3, _gl.FLOAT, false, 0, 0);
                }

                if (updateBuffers && attributes.normal >= 0)
                {
                    _gl.bindBuffer(_gl.ARRAY_BUFFER, renderObject.buffer.__webglNormalBuffer);
                    enableAttribute(attributes.normal);
                    _gl.vertexAttribPointer((uint)attributes.normal, 3, _gl.FLOAT, false, 0, 0);
                }

                if (renderObject.obj is Mesh)
                {
                    if (updateBuffers)
                    {
                        _gl.bindBuffer(_gl.ELEMENT_ARRAY_BUFFER, renderObject.buffer.__webglFaceBuffer);
                    }
                    _gl.drawElements(_gl.TRIANGLES, renderObject.buffer.__webglFaceCount, _gl.UNSIGNED_SHORT, 0);
                }
            }

            _gl.enable(_gl.DEPTH_TEST);
            _gl.depthMask(true);
        }

        internal void setSize(int width, int height)
        {
            _canvas.setWidth(width);
            _canvas.setHeight(height);

            _viewportX = 0;
            _viewportY = 0;

            _viewportWidth = _canvas.width;
            _viewportHeight = _canvas.height;

            _gl.viewport(_viewportX, _viewportY, _viewportWidth, _viewportHeight);
        }

        private void addObject(Object3D obj, Scene scene)
        {
            Geometry geometry;
            GeometryGroup geometryGroup;

            if (!(obj.__webglInit))
            {
                obj.__webglInit = true;

                obj._modelViewMatrix = new Matrix4();
                obj._normalMatrix = new Matrix3();

                if (obj is Mesh)
                {
                    geometry = obj.geometry;
                    if (geometry.geometryGroups == null)
                    {
                        var hashMapCounter = 0;
                        geometry.geometryGroups = new Dictionary<int, GeometryGroup>();

                        for (var f = 0; f < geometry.faces.Count; f++)
                        {
                            var face = geometry.faces[f];

                            var groupHash = 0 + '_' + hashMapCounter;

                            if (!geometry.geometryGroups.ContainsKey(groupHash))
                            {
                                geometry.geometryGroups[groupHash] = new GeometryGroup
                                                                     {
                                                                         faces3 = new List<int>(),
                                                                         faces4 = new List<int>(),
                                                                         materialIndex = 0,
                                                                         vertices = 0
                                                                     };
                            }

                            var vertices = face.vertexCount;

                            if (geometry.geometryGroups[groupHash].vertices + vertices > 65535)
                            {
                                hashMapCounter += 1;
                                groupHash = 95 + hashMapCounter;

                                if (geometry.geometryGroups[groupHash] == null)
                                {
                                    geometry.geometryGroups[groupHash] = new GeometryGroup
                                                                         {
                                                                             faces3 = new List<int>(),
                                                                             faces4 = new List<int>(),
                                                                             materialIndex = 0,
                                                                             vertices = 0
                                                                         };
                                }
                            }

                            if (face.vertexCount == 3)
                            {
                                geometry.geometryGroups[groupHash].faces3.Add(f);
                            }
                            else
                            {
                                geometry.geometryGroups[groupHash].faces4.Add(f);
                            }

                            geometry.geometryGroups[groupHash].vertices += vertices;
                        }

                        geometry.geometryGroupsList = new List<GeometryGroup>();

                        foreach (var g in geometry.geometryGroups.Keys)
                        {
                            geometry.geometryGroups[g].id = _geometryGroupCounter++;
                            geometry.geometryGroupsList.Add(geometry.geometryGroups[g]);
                        }
                    }
                    foreach (var g in geometry.geometryGroups.Keys)
                    {
                        geometryGroup = geometry.geometryGroups[g];

                        if (geometryGroup.__webglVertexBuffer == null)
                        {
                            geometryGroup.__webglVertexBuffer = _gl.createBuffer();
                            geometryGroup.__webglNormalBuffer = _gl.createBuffer();
                            geometryGroup.__webglFaceBuffer = _gl.createBuffer();

                            var faces3 = geometryGroup.faces3;
                            var faces4 = geometryGroup.faces4;
                            var nvertices = faces3.Count * 3 + faces4.Count * 4;
                            var ntris = faces3.Count * 1 + faces4.Count * 2;

                            geometryGroup.__vertexArray = new Float32Array(nvertices * 3);
                            geometryGroup.__normalArray = new Float32Array(nvertices * 3);
                            geometryGroup.__faceArray = new Uint16Array(ntris * 3);

                            geometryGroup.__webglFaceCount = ntris * 3;

                            geometryGroup.__inittedArrays = true;

                            geometry.verticesNeedUpdate = true;
                            geometry.elementsNeedUpdate = true;
                            geometry.normalsNeedUpdate = true;
                        }
                    }
                }
            }

            if (!obj.__webglActive)
            {
                if (obj is Mesh)
                {
                    geometry = obj.geometry;
                    if (geometry != null)
                    {
                        foreach (var g in geometry.geometryGroups.Keys)
                        {
                            geometryGroup = geometry.geometryGroups[g];
                            scene.__webglObjects.Add(new WebGLRenderObject {buffer = geometryGroup, obj = obj});
                        }
                    }
                }

                obj.__webglActive = true;
            }
        }

        private void clear(bool color = true, bool depth = true, bool stencil = true)
        {
            uint bits = 0;

            if (color)
            {
                bits |= _gl.COLOR_BUFFER_BIT;
            }
            if (depth)
            {
                bits |= _gl.DEPTH_BUFFER_BIT;
            }
            if (stencil)
            {
                bits |= _gl.STENCIL_BUFFER_BIT;
            }

            _gl.clear(bits);
        }

        private WebGLShader getShader(string type, string str)
        {
            WebGLShader shader = null;

            if (type == "fragment")
            {
                shader = _gl.createShader(_gl.FRAGMENT_SHADER);
            }
            else if (type == "vertex")
            {
                shader = _gl.createShader(_gl.VERTEX_SHADER);
            }

            if (shader == null)
            {
                return null;
            }

            _gl.shaderSource(shader, str);
            _gl.compileShader(shader);

            if (!_gl.getShaderParameter(shader, _gl.COMPILE_STATUS))
            {
                Console.WriteLine(_gl.getShaderInfoLog(shader));
                return null;
            }

            return shader;
        }

        private GLProgram setProgram(Camera camera, Material material, Object3D obj)
        {
            if (material.needsUpdate)
            {
                material.program = buildProgram();
                material.needsUpdate = false;
            }

            var refreshMaterial = false;

            if (material.program != _currentProgram)
            {
                _gl.useProgram(material.program.program);
                _currentProgram = material.program;

                refreshMaterial = true;
            }

            if (refreshMaterial || camera != _currentCamera)
            {
                _gl.uniformMatrix4fv(material.program.uniforms.projectionMatrix, false, camera.projectionMatrix.elements);

                _currentCamera = camera;
            }

            _gl.uniformMatrix4fv(material.program.uniforms.modelViewMatrix, false, obj._modelViewMatrix.elements);

            if (material.program.uniforms.normalMatrix != null)
            {
                _gl.uniformMatrix3fv(material.program.uniforms.normalMatrix, false, obj._normalMatrix.elements);
            }

            if (material.program.uniforms.modelMatrix != null)
            {
                _gl.uniformMatrix4fv(material.program.uniforms.modelMatrix, false, obj.matrixWorld.elements);
            }

            return material.program;
        }

        private GLProgram buildProgram()
        {
            var program = new GLProgram(_gl.createProgram());

            var glFragmentShader = getShader("fragment", "precision highp float; \n" +
                                                         "uniform float opacity; \n" +
                                                         "varying vec3 vNormal; \n" +
                                                         "void main() {	\n" +
                                                         "gl_FragColor = vec4( 0.5 * normalize( vNormal ) + 0.5, opacity ); \n" +
                                                         "} \n");

            var glVertexShader = getShader("vertex", "precision highp float; \n" +
                                                     "uniform mat4 modelMatrix; \n" +
                                                     "uniform mat4 modelViewMatrix; \n" +
                                                     "uniform mat4 projectionMatrix; \n" +
                                                     "uniform mat4 viewMatrix; \n" +
                                                     "uniform mat3 normalMatrix; \n" +
                                                     "uniform vec3 cameraPosition; \n" +
                                                     "attribute vec3 position; \n" +
                                                     "attribute vec3 normal; \n" +
                                                     "attribute vec2 uv; \n" +
                                                     "attribute vec2 uv2; \n" +
                                                     "varying vec3 vNormal;	\n" +
                                                     "void main() {	\n" +
                                                     "vec4 mvPosition = modelViewMatrix * vec4( position, 1.0 ); \n" +
                                                     "vNormal = normalize( normalMatrix * normal ); \n" +
                                                     "gl_Position = projectionMatrix * mvPosition; \n" +
                                                     "} \n");

            if (_gl.isShader(glVertexShader))
            {
                Console.WriteLine(@"Vertex shader created successfully!");
            }

            if (_gl.isShader(glFragmentShader))
            {
                Console.WriteLine(@"Fragment shader created successfully!");
            }

            _gl.attachShader(program.program, glVertexShader);
            _gl.attachShader(program.program, glFragmentShader);

            _gl.linkProgram(program.program);

            if (!_gl.getProgramParameter(program.program, _gl.LINK_STATUS))
            {
                Console.WriteLine("Could not initialise shader\n" + "VALIDATE_STATUS: " + _gl.getProgramParameter(program.program, _gl.VALIDATE_STATUS) + ", gl error [" + _gl.getError() + "]");
            }

            _gl.deleteShader(glFragmentShader);
            _gl.deleteShader(glVertexShader);

            _gl.validateProgram(program.program);
            if (_gl.isProgram(program.program))
            {
                Console.WriteLine(@"Program created successfully!");
            }

            program.uniforms = new GLProgram.CachedUniforms();
            program.attributes = new GLProgram.CachedAttributes();

            program.uniforms.viewMatrix = _gl.getUniformLocation(program.program, "viewMatrix");
            program.uniforms.modelViewMatrix = _gl.getUniformLocation(program.program, "modelViewMatrix");
            program.uniforms.projectionMatrix = _gl.getUniformLocation(program.program, "projectionMatrix");
            program.uniforms.normalMatrix = _gl.getUniformLocation(program.program, "normalMatrix");
            program.uniforms.modelMatrix = _gl.getUniformLocation(program.program, "modelMatrix");
            program.uniforms.cameraPosition = _gl.getUniformLocation(program.program, "cameraPosition");

            program.attributes.position = _gl.getAttribLocation(program.program, "position");
            program.attributes.normal = _gl.getAttribLocation(program.program, "normal");
            program.attributes.uv = _gl.getAttribLocation(program.program, "uv");
            program.attributes.uv2 = _gl.getAttribLocation(program.program, "uv2");
            program.attributes.color = _gl.getAttribLocation(program.program, "color");

            program.id = _programsCounter++;

            return program;
        }

        private void enableAttribute(int attribute)
        {
            if (!_enabledAttributes[attribute])
            {
                _gl.enableVertexAttribArray((uint)attribute);
                _enabledAttributes[attribute] = true;
            }
        }
    }
}