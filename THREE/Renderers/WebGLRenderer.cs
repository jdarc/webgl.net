using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using WebGL;

namespace THREE
{
    // ReSharper disable InconsistentNaming
    // ReSharper disable MemberCanBeMadeStatic.Local

    public class WebGLRenderer
    {
        public HTMLCanvasElement domElement;
        public WebGLRenderingContext context;
        public int devicePixelRatio;
        public bool autoClear;
        public bool autoClearColor;
        public bool autoClearDepth;
        public bool autoClearStencil;
        public bool sortObjects;
        public bool autoUpdateObjects;
        public bool autoUpdateScene;
        public bool gammaInput;
        public bool gammaOutput;
        public bool physicallyBasedShading;
        public bool shadowMapEnabled;
        public bool shadowMapAutoUpdate;
        public int shadowMapType;
        public int shadowMapCullFace;
        public bool shadowMapDebug;
        public bool shadowMapCascade;
        public int maxMorphTargets;
        public int maxMorphNormals;
        public bool autoScaleCubemaps;
        public List<Plugin> renderPluginsPre;
        public List<Plugin> renderPluginsPost;
        public Info info;

        private readonly HTMLCanvasElement _canvas;
        private readonly string _precision;
        private readonly bool _alpha;
        private readonly bool _premultipliedAlpha;
        private readonly bool _antialias;
        private readonly bool _stencil;
        private readonly bool _preserveDrawingBuffer;
        private readonly Color _clearColor;
        private double _clearAlpha;
        private JSArray _programs;
        private int _programs_counter;

        private WebGLProgram _currentProgram;
        private WebGLFramebuffer _currentFramebuffer;
        private int _currentMaterialId;
        private int? _currentGeometryGroupHash;
        private Camera _currentCamera;
        private int _geometryGroupCounter;

        private int _usedTextureUnits;

        private bool? _oldDoubleSided;
        private bool? _oldFlipSided;

        private int? _oldBlending;

        private int? _oldBlendEquation;
        private int? _oldBlendSrc;
        private int? _oldBlendDst;

        private bool? _oldDepthTest;
        private bool? _oldDepthWrite;

        private bool? _oldPolygonOffset;
        private double? _oldPolygonOffsetFactor;
        private double? _oldPolygonOffsetUnits;

        private double? _oldLineWidth;

        private int _viewportX;
        private int _viewportY;
        private int _viewportWidth;
        private int _viewportHeight;
        private int _currentWidth;
        private int _currentHeight;

        private readonly bool[] _enabledAttributes;

        private readonly Frustum _frustum;

        private readonly Matrix4 _projScreenMatrix;
        private readonly Matrix4 _projScreenMatrixPS;

        private readonly Vector3 _vector3;

        private readonly Vector3 _direction;

        private bool _lightsNeedUpdate;

        private readonly dynamic _lights;

        private WebGLRenderingContext _gl;

        private WebGLExtension _glExtensionTextureFloat;
        private WebGLExtension _glExtensionStandardDerivatives;
        private WebGLExtension _glExtensionTextureFilterAnisotropic;
        private WebGLExtension _glExtensionCompressedTextureS3TC;

        private readonly int _maxTextures;
        private readonly int _maxVertexTextures;
        private int _maxTextureSize;
        private readonly int _maxCubemapSize;

        private readonly int _maxAnisotropy;

        private readonly bool _supportsVertexTextures;
        private readonly bool _supportsBoneTextures;

        private Uint32Array _compressedTextureFormats;

        private readonly WebGLShaderPrecisionFormat _vertexShaderPrecisionHighpFloat;
        private readonly WebGLShaderPrecisionFormat _vertexShaderPrecisionMediumpFloat;
        private WebGLShaderPrecisionFormat _vertexShaderPrecisionLowpFloat;

        private readonly WebGLShaderPrecisionFormat _fragmentShaderPrecisionHighpFloat;
        private readonly WebGLShaderPrecisionFormat _fragmentShaderPrecisionMediumpFloat;
        private WebGLShaderPrecisionFormat _fragmentShaderPrecisionLowpFloat;

        private WebGLShaderPrecisionFormat _vertexShaderPrecisionHighpInt;
        private WebGLShaderPrecisionFormat _vertexShaderPrecisionMediumpInt;
        private WebGLShaderPrecisionFormat _vertexShaderPrecisionLowpInt;

        private WebGLShaderPrecisionFormat _fragmentShaderPrecisionHighpInt;
        private WebGLShaderPrecisionFormat _fragmentShaderPrecisionMediumpInt;
        private WebGLShaderPrecisionFormat _fragmentShaderPrecisionLowpInt;

        private readonly bool _highpAvailable;
        private readonly bool _mediumpAvailable;

        private readonly ShadowMapPlugin _shadowMapPlugin;

        public WebGLRenderer(dynamic @params = null)
        {
            JSConsole.log(String.Format("THREE.WebGLRenderer: {0}", THREE.REVISION));

            var parameters = JSObject.create(@params);

            _canvas = parameters.canvas ?? new HTMLCanvasElement(null);

            _precision = parameters.precision ?? "highp";

            _alpha = parameters.alpha ?? true;
            _premultipliedAlpha = parameters.premultipliedAlpha ?? true;
            _antialias = parameters.antialias ?? false;
            _stencil = parameters.stencil ?? true;
            _preserveDrawingBuffer = parameters.preserveDrawingBuffer ?? false;

            _clearColor = parameters.clearColor != null ? new Color(parameters.clearColor) : new Color(0x000000);
            _clearAlpha = parameters.clearAlpha ?? 0;

            domElement = _canvas;
            context = null;
            devicePixelRatio = 1;

            autoClear = true;
            autoClearColor = true;
            autoClearDepth = true;
            autoClearStencil = true;

            sortObjects = true;

            autoUpdateObjects = true;
            autoUpdateScene = true;

            gammaInput = false;
            gammaOutput = false;
            physicallyBasedShading = false;

            shadowMapEnabled = false;
            shadowMapAutoUpdate = true;
            shadowMapType = THREE.PCFShadowMap;
            shadowMapCullFace = THREE.CullFaceFront;
            shadowMapDebug = false;
            shadowMapCascade = false;

            maxMorphTargets = 8;
            maxMorphNormals = 4;

            autoScaleCubemaps = true;

            renderPluginsPre = new List<Plugin>();
            renderPluginsPost = new List<Plugin>();

            info = new Info();

            _programs = new JSArray();
            _programs_counter = 0;

            _currentProgram = null;
            _currentFramebuffer = null;
            _currentMaterialId = -1;
            _currentGeometryGroupHash = null;
            _currentCamera = null;
            _geometryGroupCounter = 0;

            _usedTextureUnits = 0;

            _oldDoubleSided = null;
            _oldFlipSided = null;

            _oldBlending = null;

            _oldBlendEquation = -1;
            _oldBlendSrc = -1;
            _oldBlendDst = -1;

            _oldDepthTest = null;
            _oldDepthWrite = null;

            _oldPolygonOffset = null;
            _oldPolygonOffsetFactor = null;
            _oldPolygonOffsetUnits = null;

            _oldLineWidth = null;

            _viewportX = 0;
            _viewportY = 0;
            _viewportWidth = 0;
            _viewportHeight = 0;
            _currentWidth = 0;
            _currentHeight = 0;

            _frustum = new Frustum();

            _projScreenMatrix = new Matrix4();
            _projScreenMatrixPS = new Matrix4();

            _vector3 = new Vector3();

            _direction = new Vector3();

            _lightsNeedUpdate = true;

            _lights = JSObject.create(new
                                      {
                                          ambient = new JSArray(0, 0, 0),
                                          directional = new {length = 0, colors = new JSArray(), positions = new JSArray()},
                                          point = new {length = 0, colors = new JSArray(), positions = new JSArray(), distances = new JSArray()},
                                          spot = new {length = 0, colors = new JSArray(), positions = new JSArray(), distances = new JSArray(), directions = new JSArray(), anglesCos = new JSArray(), exponents = new JSArray()},
                                          hemi = new {length = 0, skyColors = new JSArray(), groundColors = new JSArray(), positions = new JSArray()}
                                      });

            initGL();

            setDefaultGLState();

            context = _gl;

            _enabledAttributes = new bool[context.getParameter(context.MAX_VERTEX_ATTRIBS)];

            _maxTextures = _gl.getParameter(_gl.MAX_TEXTURE_IMAGE_UNITS);
            _maxVertexTextures = _gl.getParameter(_gl.MAX_VERTEX_TEXTURE_IMAGE_UNITS);
            _maxTextureSize = _gl.getParameter(_gl.MAX_TEXTURE_SIZE);
            _maxCubemapSize = _gl.getParameter(_gl.MAX_CUBE_MAP_TEXTURE_SIZE);

            _maxAnisotropy = (int)(_glExtensionTextureFilterAnisotropic != null ? _gl.getParameter(_glExtensionTextureFilterAnisotropic.MAX_TEXTURE_MAX_ANISOTROPY_EXT) : 0);

            _supportsVertexTextures = (_maxVertexTextures > 0);
            _supportsBoneTextures = _supportsVertexTextures && _glExtensionTextureFloat != null;

            _compressedTextureFormats = _glExtensionCompressedTextureS3TC != null ? _gl.getParameter(_gl.COMPRESSED_TEXTURE_FORMATS) : new Uint32Array(0);

            _vertexShaderPrecisionHighpFloat = _gl.getShaderPrecisionFormat(_gl.VERTEX_SHADER, _gl.HIGH_FLOAT);
            _vertexShaderPrecisionMediumpFloat = _gl.getShaderPrecisionFormat(_gl.VERTEX_SHADER, _gl.MEDIUM_FLOAT);
            _vertexShaderPrecisionLowpFloat = _gl.getShaderPrecisionFormat(_gl.VERTEX_SHADER, _gl.LOW_FLOAT);

            _fragmentShaderPrecisionHighpFloat = _gl.getShaderPrecisionFormat(_gl.FRAGMENT_SHADER, _gl.HIGH_FLOAT);
            _fragmentShaderPrecisionMediumpFloat = _gl.getShaderPrecisionFormat(_gl.FRAGMENT_SHADER, _gl.MEDIUM_FLOAT);
            _fragmentShaderPrecisionLowpFloat = _gl.getShaderPrecisionFormat(_gl.FRAGMENT_SHADER, _gl.LOW_FLOAT);

            _vertexShaderPrecisionHighpInt = _gl.getShaderPrecisionFormat(_gl.VERTEX_SHADER, _gl.HIGH_INT);
            _vertexShaderPrecisionMediumpInt = _gl.getShaderPrecisionFormat(_gl.VERTEX_SHADER, _gl.MEDIUM_INT);
            _vertexShaderPrecisionLowpInt = _gl.getShaderPrecisionFormat(_gl.VERTEX_SHADER, _gl.LOW_INT);

            _fragmentShaderPrecisionHighpInt = _gl.getShaderPrecisionFormat(_gl.FRAGMENT_SHADER, _gl.HIGH_INT);
            _fragmentShaderPrecisionMediumpInt = _gl.getShaderPrecisionFormat(_gl.FRAGMENT_SHADER, _gl.MEDIUM_INT);
            _fragmentShaderPrecisionLowpInt = _gl.getShaderPrecisionFormat(_gl.FRAGMENT_SHADER, _gl.LOW_INT);

            _highpAvailable = _vertexShaderPrecisionHighpFloat.precision() > 0 && _fragmentShaderPrecisionHighpFloat.precision() > 0;
            _mediumpAvailable = _vertexShaderPrecisionMediumpFloat.precision() > 0 && _fragmentShaderPrecisionMediumpFloat.precision() > 0;

            if (_precision == "highp" && !_highpAvailable)
            {
                if (_mediumpAvailable)
                {
                    _precision = "mediump";
                    JSConsole.warn("WebGLRenderer: highp not supported, using mediump");
                }
                else
                {
                    _precision = "lowp";
                    JSConsole.warn("WebGLRenderer: highp and mediump not supported, using lowp");
                }
            }

            if (_precision == "mediump" && !_mediumpAvailable)
            {
                _precision = "lowp";
                JSConsole.warn("WebGLRenderer: mediump not supported, using lowp");
            }

            _shadowMapPlugin = new ShadowMapPlugin();
            addPrePlugin(_shadowMapPlugin);

            addPostPlugin(new SpritePlugin());
            addPostPlugin(new LensFlarePlugin());
        }

        public WebGLRenderingContext getContext()
        {
            return _gl;
        }

        public bool supportsVertexTextures()
        {
            return _supportsVertexTextures;
        }

        public bool supportsFloatTextures()
        {
            return _glExtensionTextureFloat != null;
        }

        public bool supportsStandardDerivatives()
        {
            return _glExtensionStandardDerivatives != null;
        }

        public bool supportsCompressedTextureS3TC()
        {
            return _glExtensionCompressedTextureS3TC != null;
        }

        public int getMaxAnisotropy()
        {
            return _maxAnisotropy;
        }

        public string getPrecision()
        {
            return _precision;
        }

        public void setSize(int width, int height)
        {
            _canvas.setWidth(width * devicePixelRatio);
            _canvas.setHeight(height * devicePixelRatio);

            setViewport(0, 0, _canvas.width, _canvas.height);
        }

        public void setViewport(int x, int y, int width, int height)
        {
            _viewportX = x;
            _viewportY = y;

            _viewportWidth = width;
            _viewportHeight = height;

            _gl.viewport(_viewportX, _viewportY, _viewportWidth, _viewportHeight);
        }

        public void setScissor(int x, int y, int width, int height)
        {
            _gl.scissor(x, y, width, height);
        }

        public void enableScissorTest(bool enable)
        {
            if (enable)
            {
                _gl.enable(_gl.SCISSOR_TEST);
            }
            else
            {
                _gl.disable(_gl.SCISSOR_TEST);
            }
        }

        public void setClearColorHex(int hex, double alpha)
        {
            _clearColor.setHex(hex);
            _clearAlpha = alpha;

            _gl.clearColor((float)_clearColor.r, (float)_clearColor.g, (float)_clearColor.b, (float)_clearAlpha);
        }

        public void setClearColor(Color color, double alpha)
        {
            _clearColor.copy(color);
            _clearAlpha = alpha;

            _gl.clearColor((float)_clearColor.r, (float)_clearColor.g, (float)_clearColor.b, (float)_clearAlpha);
        }

        public Color getClearColor()
        {
            return _clearColor;
        }

        public double getClearAlpha()
        {
            return _clearAlpha;
        }

        public void clear(bool color = true, bool depth = true, bool stencil = true)
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

        public void clearTarget(WebGLRenderTarget renderTarget, bool color, bool depth, bool stencil)
        {
            setRenderTarget(renderTarget);
            clear(color, depth, stencil);
        }

        public void addPostPlugin(Plugin plugin)
        {
            plugin.init(this);
            renderPluginsPost.Add(plugin);
        }

        public void addPrePlugin(Plugin plugin)
        {
            plugin.init(this);
            renderPluginsPre.Add(plugin);
        }

        public void updateShadowMap(Scene scene, Camera camera)
        {
            _currentProgram = null;
            _oldBlending = null;
            _oldDepthTest = null;
            _oldDepthWrite = null;
            _currentGeometryGroupHash = -1;
            _currentMaterialId = -1;
            _lightsNeedUpdate = true;
            _oldDoubleSided = null;
            _oldFlipSided = null;

            _shadowMapPlugin.update(scene, camera);
        }

        private void createParticleBuffers(Geometry geometry)
        {
            geometry.__webglVertexBuffer = _gl.createBuffer();
            geometry.__webglColorBuffer = _gl.createBuffer();

            info.memory.geometries++;
        }

        private void createLineBuffers(Geometry geometry)
        {
            geometry.__webglVertexBuffer = _gl.createBuffer();
            geometry.__webglColorBuffer = _gl.createBuffer();
            geometry.__webglLineDistanceBuffer = _gl.createBuffer();

            info.memory.geometries++;
        }

        private void createRibbonBuffers(dynamic geometry)
        {
            geometry.__webglVertexBuffer = _gl.createBuffer();
            geometry.__webglColorBuffer = _gl.createBuffer();
            geometry.__webglNormalBuffer = _gl.createBuffer();

            info.memory.geometries++;
        }

        private void createMeshBuffers(dynamic geometryGroup)
        {
            geometryGroup.__webglVertexBuffer = _gl.createBuffer();
            geometryGroup.__webglNormalBuffer = _gl.createBuffer();
            geometryGroup.__webglTangentBuffer = _gl.createBuffer();
            geometryGroup.__webglColorBuffer = _gl.createBuffer();
            geometryGroup.__webglUVBuffer = _gl.createBuffer();
            geometryGroup.__webglUV2Buffer = _gl.createBuffer();

            geometryGroup.__webglSkinIndicesBuffer = _gl.createBuffer();
            geometryGroup.__webglSkinWeightsBuffer = _gl.createBuffer();

            geometryGroup.__webglFaceBuffer = _gl.createBuffer();
            geometryGroup.__webglLineBuffer = _gl.createBuffer();

            int m, ml;

            if (geometryGroup.numMorphTargets > 0)
            {
                geometryGroup.__webglMorphTargetsBuffers = new JSArray();

                for (m = 0, ml = geometryGroup.numMorphTargets; m < ml; m++)
                {
                    geometryGroup.__webglMorphTargetsBuffers.push(_gl.createBuffer());
                }
            }

            if (geometryGroup.numMorphNormals > 0)
            {
                geometryGroup.__webglMorphNormalsBuffers = new JSArray();

                for (m = 0, ml = geometryGroup.numMorphNormals; m < ml; m++)
                {
                    geometryGroup.__webglMorphNormalsBuffers.push(_gl.createBuffer());
                }
            }

            info.memory.geometries++;
        }

        private readonly Action<JSEvent> onGeometryDispose = @event => { };

        private void onTextureDispose(JSEvent @event)
        {
            var texture = (Texture)@event.target;

            texture.removeEventListener("dispose", onTextureDispose);

            deallocateTexture(this, texture);

            info.memory.textures--;
        }

        private readonly Action<JSEvent> onRenderTargetDispose = @event => { };

        private void onMaterialDispose(JSEvent @event)
        {
            var material = (Material)@event.target;
            material.removeEventListener("dispose", onMaterialDispose);
            deallocateMaterial(this, material);
        }

        private Action<WebGLRenderer, dynamic> deallocateGeometry = (src, geometry) => { };

        private static void deallocateTexture(WebGLRenderer src, dynamic texture)
        {
            if (JSObject.eval(texture.image) && JSObject.eval(texture.image.__webglTextureCube))
            {
                // cube texture
                src._gl.deleteTexture(texture.image.__webglTextureCube);
            }
            else
            {
                // 2D texture
                if (texture.__webglInit)
                {
                    texture.__webglInit = false;
                    src._gl.deleteTexture(texture.__webglTexture);
                }
            }
        }

        private Action<WebGLRenderer, dynamic> deallocateRenderTarget = (src, renderTarget) => { };

        private static void deallocateMaterial(WebGLRenderer src, dynamic material)
        {
            var program = material.program;

            if (program == null)
            {
                return;
            }

            material.program = null;

            // only deallocate GL program if this was the last use of shared program
            // assumed there is only single copy of any program in the _programs list
            // (that's how it's constructed)
            var deleteProgram = false;

            for (int i = 0, il = src._programs.length; i < il; i++)
            {
                var programInfo = src._programs[i];

                if (programInfo.program == program)
                {
                    programInfo.usedTimes --;

                    if (programInfo.usedTimes == 0)
                    {
                        deleteProgram = true;
                    }

                    break;
                }
            }

            if (deleteProgram)
            {
                // avoid using array.splice, this is costlier than creating new array from scratch

                var newPrograms = new JSArray();

                for (int i = 0, il = src._programs.length; i < il; i++)
                {
                    var programInfo = src._programs[i];

                    if (programInfo.program != program)
                    {
                        newPrograms.push(programInfo);
                    }
                }

                src._programs = newPrograms;

                src._gl.deleteProgram(program);

                src.info.memory.programs--;
            }
        }

        private void deleteCustomAttributesBuffers(dynamic geometry)
        {
            if (geometry.__webglCustomAttributesList)
            {
                foreach (var id in geometry.__webglCustomAttributesList)
                {
                    _gl.deleteBuffer(geometry.__webglCustomAttributesList[id].buffer);
                }
            }
        }

        private void initCustomAttributes(Geometry geometry, Object3D obj)
        {
            var nvertices = geometry.vertices.length;

            var material = obj.material;

            if (material != null && material.attributes != null)
            {
                if (geometry.__webglCustomAttributesList == null)
                {
                    geometry.__webglCustomAttributesList = new JSArray();
                }

                foreach (var a in material.attributes)
                {
                    var attribute = material.attributes[a];

                    if (!JSObject.eval(attribute.__webglInitialized) || JSObject.eval(attribute.createUniqueBuffers))
                    {
                        attribute.__webglInitialized = true;

                        var size = 1;

                        if (attribute.type == "v2")
                        {
                            size = 2;
                        }
                        else if (attribute.type == "v3")
                        {
                            size = 3;
                        }
                        else if (attribute.type == "v4")
                        {
                            size = 4;
                        }
                        else if (attribute.type == "c")
                        {
                            size = 3;
                        }

                        attribute.size = size;

                        attribute.array = new Float32Array(nvertices * size);

                        attribute.buffer = _gl.createBuffer();
                        attribute.buffer.belongsToAttribute = a;

                        attribute.needsUpdate = true;
                    }

                    geometry.__webglCustomAttributesList.push(attribute);
                }
            }
        }

        private void initParticleBuffers(Geometry geometry, Object3D obj)
        {
            var nvertices = geometry.vertices.length;

            geometry.__vertexArray = new Float32Array(nvertices * 3);
            geometry.__colorArray = new Float32Array(nvertices * 3);

            geometry.__sortArray = new JSArray();

            geometry.__webglParticleCount = nvertices;

            initCustomAttributes(geometry, obj);
        }

        private void initLineBuffers(Geometry geometry, Object3D obj)
        {
            var nvertices = geometry.vertices.length;

            geometry.__vertexArray = new Float32Array(nvertices * 3);
            geometry.__colorArray = new Float32Array(nvertices * 3);
            geometry.__lineDistanceArray = new Float32Array(nvertices * 1);

            geometry.__webglLineCount = nvertices;

            initCustomAttributes(geometry, obj);
        }

        private void initRibbonBuffers(dynamic geometry, dynamic obj)
        {
            var nvertices = geometry.vertices.length;

            geometry.__vertexArray = new Float32Array(nvertices * 3);
            geometry.__colorArray = new Float32Array(nvertices * 3);
            geometry.__normalArray = new Float32Array(nvertices * 3);

            geometry.__webglVertexCount = nvertices;

            initCustomAttributes(geometry, obj);
        }

        private void initMeshBuffers(dynamic geometryGroup, Object3D obj)
        {
            var geometry = obj.geometry;
            var faces3 = geometryGroup.faces3;
            var faces4 = geometryGroup.faces4;
            var nvertices = faces3.length * 3 + faces4.length * 4;
            var ntris = faces3.length * 1 + faces4.length * 2;
            var nlines = faces3.length * 3 + faces4.length * 4;
            var material = getBufferMaterial(obj, geometryGroup);
            var uvType = bufferGuessUVType(material);
            var normalType = bufferGuessNormalType(material);
            var vertexColorType = bufferGuessVertexColorType(material);

            geometryGroup.__vertexArray = new Float32Array(nvertices * 3);

            if (normalType != THREE.NoShading)
            {
                geometryGroup.__normalArray = new Float32Array(nvertices * 3);
            }

            if (geometry.hasTangents)
            {
                geometryGroup.__tangentArray = new Float32Array(nvertices * 4);
            }

            if (vertexColorType != null)
            {
                geometryGroup.__colorArray = new Float32Array(nvertices * 3);
            }

            if (uvType)
            {
                if (geometry.faceUvs.length > 0 || geometry.faceVertexUvs.length > 0)
                {
                    geometryGroup.__uvArray = new Float32Array(nvertices * 2);
                }

                if (geometry.faceUvs.length > 1 || geometry.faceVertexUvs.length > 1)
                {
                    geometryGroup.__uv2Array = new Float32Array(nvertices * 2);
                }
            }

            if (obj.geometry.skinWeights.length > 0 && obj.geometry.skinIndices.length > 0)
            {
                geometryGroup.__skinIndexArray = new Float32Array(nvertices * 4);
                geometryGroup.__skinWeightArray = new Float32Array(nvertices * 4);
            }

            geometryGroup.__faceArray = new Uint16Array(ntris * 3);
            geometryGroup.__lineArray = new Uint16Array(nlines * 2);

            int m, ml;

            if (geometryGroup.numMorphTargets > 0)
            {
                geometryGroup.__morphTargetsArrays = new JSArray();

                for (m = 0, ml = geometryGroup.numMorphTargets; m < ml; m++)
                {
                    geometryGroup.__morphTargetsArrays.push(new Float32Array(nvertices * 3));
                }
            }

            if (geometryGroup.numMorphNormals > 0)
            {
                geometryGroup.__morphNormalsArrays = new JSArray();

                for (m = 0, ml = geometryGroup.numMorphNormals; m < ml; m++)
                {
                    geometryGroup.__morphNormalsArrays.push(new Float32Array(nvertices * 3));
                }
            }

            geometryGroup.__webglFaceCount = ntris * 3;
            geometryGroup.__webglLineCount = nlines * 2;

            if (material.attributes != null)
            {
                if (geometryGroup.__webglCustomAttributesList == null)
                {
                    geometryGroup.__webglCustomAttributesList = new JSArray();
                }

                foreach (var a in material.attributes)
                {
                    var originalAttribute = material.attributes[a];

                    dynamic attribute = new JSObject();

                    foreach (var property in originalAttribute)
                    {
                        attribute[property] = originalAttribute[property];
                    }

                    if (!JSObject.eval(attribute.__webglInitialized) || attribute.createUniqueBuffers)
                    {
                        attribute.__webglInitialized = true;

                        var size = 1;

                        if (attribute.type == "v2")
                        {
                            size = 2;
                        }
                        else if (attribute.type == "v3")
                        {
                            size = 3;
                        }
                        else if (attribute.type == "v4")
                        {
                            size = 4;
                        }
                        else if (attribute.type == "c")
                        {
                            size = 3;
                        }

                        attribute.size = size;

                        attribute.array = new Float32Array(nvertices * size);

                        attribute.buffer = _gl.createBuffer();
                        attribute.buffer.belongsToAttribute = a;

                        originalAttribute.needsUpdate = true;
                        attribute.__original = originalAttribute;
                    }

                    geometryGroup.__webglCustomAttributesList.push(attribute);
                }
            }

            geometryGroup.__inittedArrays = true;
        }

        private Material getBufferMaterial(Object3D obj, dynamic geometryGroup)
        {
            var faceMaterials = obj.material as MeshFaceMaterial;
            return faceMaterials != null ? faceMaterials.materials[geometryGroup.materialIndex] : obj.material;
        }

        private bool materialNeedsSmoothNormals(dynamic material)
        {
            return material != null && material.shading == THREE.SmoothShading;
        }

        private int bufferGuessNormalType(dynamic material)
        {
            if ((material is MeshBasicMaterial && !((bool)JSObject.eval(material.envMap))) || material is MeshDepthMaterial)
            {
                return THREE.NoShading;
            }

            return materialNeedsSmoothNormals(material) ? THREE.SmoothShading : THREE.FlatShading;
        }

        private dynamic bufferGuessVertexColorType(dynamic material)
        {
            if (JSObject.eval(material.vertexColors))
            {
                return material.vertexColors;
            }

            return false;
        }

        private bool bufferGuessUVType(dynamic material)
        {
            return material.map != null ||
                   material.lightMap != null ||
                   material.bumpMap != null ||
                   material.normalMap != null ||
                   material.specularMap != null ||
                   material is ShaderMaterial;
        }

        private void initDirectBuffers(dynamic geometry)
        {
            foreach (var a in geometry.attributes)
            {
                var type = a == "index" ? _gl.ELEMENT_ARRAY_BUFFER : _gl.ARRAY_BUFFER;

                var attribute = geometry.attributes[a];

                attribute.buffer = _gl.createBuffer();

                _gl.bindBuffer(type, attribute.buffer);
                _gl.bufferData(type, attribute.array, _gl.STATIC_DRAW);
            }
        }

        private void setParticleBuffers(dynamic geometry, uint hint, dynamic obj)
        {
            var vertices = geometry.vertices;
            var vl = vertices.length;
            var colors = geometry.colors;
            var cl = colors.length;
            var vertexArray = geometry.__vertexArray;
            var colorArray = geometry.__colorArray;
            JSArray sortArray = geometry.__sortArray;
            var dirtyVertices = geometry.verticesNeedUpdate;
            var dirtyElements = geometry.elementsNeedUpdate;
            var dirtyColors = geometry.colorsNeedUpdate;
            var customAttributes = geometry.__webglCustomAttributesList;

            if (obj.sortParticles)
            {
                _projScreenMatrixPS.copy(_projScreenMatrix);
                _projScreenMatrixPS.multiply(obj.matrixWorld);

                for (var v = 0; v < vl; v++)
                {
                    var vertex = vertices[v];

                    _vector3.copy(vertex);
                    _vector3.applyProjection(_projScreenMatrixPS);

                    sortArray[v] = new JSArray(_vector3.z, v);
                }

                sortArray.sort(numericalSort);

                for (var v = 0; v < vl; v++)
                {
                    var vertex = vertices[sortArray[v][1]];

                    var offset = v * 3;

                    vertexArray[offset] = (float)vertex.x;
                    vertexArray[offset + 1] = (float)vertex.y;
                    vertexArray[offset + 2] = (float)vertex.z;
                }

                for (var c = 0; c < cl; c++)
                {
                    var offset = c * 3;

                    var color = colors[sortArray[c][1]];

                    colorArray[offset] = color.r;
                    colorArray[offset + 1] = color.g;
                    colorArray[offset + 2] = color.b;
                }

                if (customAttributes != null)
                {
                    for (int i = 0, il = customAttributes.length; i < il; i++)
                    {
                        var customAttribute = customAttributes[i];

                        if (!(customAttribute.boundTo == null || customAttribute.boundTo == "vertices"))
                        {
                            continue;
                        }

                        var offset = 0;

                        var cal = customAttribute.value.length;

                        dynamic index;
                        if (customAttribute.size == 1)
                        {
                            for (var ca = 0; ca < cal; ca++)
                            {
                                index = sortArray[ca][1];
                                customAttribute.array[ca] = (float)customAttribute.value[index];
                            }
                        }
                        else if (customAttribute.size == 2)
                        {
                            for (var ca = 0; ca < cal; ca++)
                            {
                                index = sortArray[ca][1];

                                var value = customAttribute.value[index];

                                customAttribute.array[offset] = value.x;
                                customAttribute.array[offset + 1] = value.y;

                                offset += 2;
                            }
                        }
                        else if (customAttribute.size == 3)
                        {
                            if (customAttribute.type == "c")
                            {
                                for (var ca = 0; ca < cal; ca++)
                                {
                                    index = sortArray[ca][1];

                                    var value = customAttribute.value[index];

                                    customAttribute.array[offset] = (float)value.r;
                                    customAttribute.array[offset + 1] = (float)value.g;
                                    customAttribute.array[offset + 2] = (float)value.b;

                                    offset += 3;
                                }
                            }
                            else
                            {
                                for (var ca = 0; ca < cal; ca++)
                                {
                                    index = sortArray[ca][1];

                                    var value = customAttribute.value[index];

                                    customAttribute.array[offset] = value.x;
                                    customAttribute.array[offset + 1] = value.y;
                                    customAttribute.array[offset + 2] = value.z;

                                    offset += 3;
                                }
                            }
                        }
                        else if (customAttribute.size == 4)
                        {
                            for (var ca = 0; ca < cal; ca++)
                            {
                                index = sortArray[ca][1];

                                var value = customAttribute.value[index];

                                customAttribute.array[offset] = value.x;
                                customAttribute.array[offset + 1] = value.y;
                                customAttribute.array[offset + 2] = value.z;
                                customAttribute.array[offset + 3] = value.w;

                                offset += 4;
                            }
                        }
                    }
                }
            }
            else
            {
                if (dirtyVertices)
                {
                    for (var v = 0; v < vl; v++)
                    {
                        var vertex = vertices[v];

                        var offset = v * 3;

                        vertexArray[offset] = (float)vertex.x;
                        vertexArray[offset + 1] = (float)vertex.y;
                        vertexArray[offset + 2] = (float)vertex.z;
                    }
                }

                if (dirtyColors)
                {
                    for (var c = 0; c < cl; c++)
                    {
                        var color = colors[c];

                        var offset = c * 3;

                        colorArray[offset] = (float)color.r;
                        colorArray[offset + 1] = (float)color.g;
                        colorArray[offset + 2] = (float)color.b;
                    }
                }

                if (customAttributes != null)
                {
                    for (int i = 0, il = customAttributes.length; i < il; i++)
                    {
                        var customAttribute = customAttributes[i];

                        if (customAttribute.needsUpdate &&
                            (customAttribute.boundTo == null ||
                             customAttribute.boundTo == "vertices"))
                        {
                            var cal = customAttribute.value.length;

                            var offset = 0;

                            if (customAttribute.size == 1)
                            {
                                for (var ca = 0; ca < cal; ca++)
                                {
                                    customAttribute.array[ca] = (float)customAttribute.value[ca];
                                }
                            }
                            else if (customAttribute.size == 2)
                            {
                                for (var ca = 0; ca < cal; ca++)
                                {
                                    var value = customAttribute.value[ca];

                                    customAttribute.array[offset] = value.x;
                                    customAttribute.array[offset + 1] = value.y;

                                    offset += 2;
                                }
                            }
                            else if (customAttribute.size == 3)
                            {
                                if (customAttribute.type == "c")
                                {
                                    for (var ca = 0; ca < cal; ca++)
                                    {
                                        var value = customAttribute.value[ca];

                                        customAttribute.array[offset] = (float)value.r;
                                        customAttribute.array[offset + 1] = (float)value.g;
                                        customAttribute.array[offset + 2] = (float)value.b;

                                        offset += 3;
                                    }
                                }
                                else
                                {
                                    for (var ca = 0; ca < cal; ca++)
                                    {
                                        var value = customAttribute.value[ca];

                                        customAttribute.array[offset] = value.x;
                                        customAttribute.array[offset + 1] = value.y;
                                        customAttribute.array[offset + 2] = value.z;

                                        offset += 3;
                                    }
                                }
                            }
                            else if (customAttribute.size == 4)
                            {
                                for (var ca = 0; ca < cal; ca++)
                                {
                                    var value = customAttribute.value[ca];

                                    customAttribute.array[offset] = value.x;
                                    customAttribute.array[offset + 1] = value.y;
                                    customAttribute.array[offset + 2] = value.z;
                                    customAttribute.array[offset + 3] = value.w;

                                    offset += 4;
                                }
                            }
                        }
                    }
                }
            }

            if (dirtyVertices || obj.sortParticles)
            {
                _gl.bindBuffer(_gl.ARRAY_BUFFER, geometry.__webglVertexBuffer);
                _gl.bufferData(_gl.ARRAY_BUFFER, vertexArray, hint);
            }

            if (dirtyColors || obj.sortParticles)
            {
                _gl.bindBuffer(_gl.ARRAY_BUFFER, geometry.__webglColorBuffer);
                _gl.bufferData(_gl.ARRAY_BUFFER, colorArray, hint);
            }

            if (customAttributes != null)
            {
                for (int i = 0, il = customAttributes.length; i < il; i++)
                {
                    var customAttribute = customAttributes[i];

                    if (customAttribute.needsUpdate || obj.sortParticles)
                    {
                        _gl.bindBuffer(_gl.ARRAY_BUFFER, customAttribute.buffer);
                        _gl.bufferData(_gl.ARRAY_BUFFER, customAttribute.array, hint);
                    }
                }
            }
        }

        private void setLineBuffers(Geometry geometry, uint hint)
        {
            var vertices = geometry.vertices;
            var colors = geometry.colors;
            var lineDistances = geometry.lineDistances;
            var vl = vertices.length;
            var cl = colors.length;
            var dl = lineDistances.length;
            var vertexArray = geometry.__vertexArray;
            var colorArray = geometry.__colorArray;
            var lineDistanceArray = geometry.__lineDistanceArray;
            var dirtyVertices = geometry.verticesNeedUpdate;
            var dirtyColors = geometry.colorsNeedUpdate;
            var dirtyLineDistances = geometry.lineDistancesNeedUpdate;
            var customAttributes = geometry.__webglCustomAttributesList;

            if (dirtyVertices)
            {
                for (var v = 0; v < vl; v++)
                {
                    var vertex = vertices[v];

                    var offset = v * 3;

                    vertexArray[offset] = (float)vertex.x;
                    vertexArray[offset + 1] = (float)vertex.y;
                    vertexArray[offset + 2] = (float)vertex.z;
                }

                _gl.bindBuffer(_gl.ARRAY_BUFFER, geometry.__webglVertexBuffer);
                _gl.bufferData(_gl.ARRAY_BUFFER, vertexArray, hint);
            }

            if (dirtyColors)
            {
                for (var c = 0; c < cl; c++)
                {
                    var color = colors[c];

                    var offset = c * 3;

                    colorArray[offset] = (float)color.r;
                    colorArray[offset + 1] = (float)color.g;
                    colorArray[offset + 2] = (float)color.b;
                }

                _gl.bindBuffer(_gl.ARRAY_BUFFER, geometry.__webglColorBuffer);
                _gl.bufferData(_gl.ARRAY_BUFFER, colorArray, hint);
            }

            if (dirtyLineDistances)
            {
                for (var d = 0; d < dl; d++)
                {
                    lineDistanceArray[d] = (float)lineDistances[d];
                }

                _gl.bindBuffer(_gl.ARRAY_BUFFER, geometry.__webglLineDistanceBuffer);
                _gl.bufferData(_gl.ARRAY_BUFFER, lineDistanceArray, hint);
            }

            if (customAttributes != null)
            {
                for (int i = 0, il = customAttributes.length; i < il; i++)
                {
                    var customAttribute = customAttributes[i];

                    if (customAttribute.needsUpdate &&
                        (customAttribute.boundTo == null ||
                         customAttribute.boundTo == "vertices"))
                    {
                        var offset = 0;

                        var cal = customAttribute.value.length;

                        if (customAttribute.size == 1)
                        {
                            for (var ca = 0; ca < cal; ca++)
                            {
                                customAttribute.array[ca] = customAttribute.value[ca];
                            }
                        }
                        else if (customAttribute.size == 2)
                        {
                            for (var ca = 0; ca < cal; ca++)
                            {
                                var value = customAttribute.value[ca];

                                customAttribute.array[offset] = value.x;
                                customAttribute.array[offset + 1] = value.y;

                                offset += 2;
                            }
                        }
                        else if (customAttribute.size == 3)
                        {
                            if (customAttribute.type == "c")
                            {
                                for (var ca = 0; ca < cal; ca++)
                                {
                                    var value = customAttribute.value[ca];

                                    customAttribute.array[offset] = value.r;
                                    customAttribute.array[offset + 1] = value.g;
                                    customAttribute.array[offset + 2] = value.b;

                                    offset += 3;
                                }
                            }
                            else
                            {
                                for (var ca = 0; ca < cal; ca++)
                                {
                                    var value = customAttribute.value[ca];

                                    customAttribute.array[offset] = value.x;
                                    customAttribute.array[offset + 1] = value.y;
                                    customAttribute.array[offset + 2] = value.z;

                                    offset += 3;
                                }
                            }
                        }
                        else if (customAttribute.size == 4)
                        {
                            for (var ca = 0; ca < cal; ca++)
                            {
                                var value = customAttribute.value[ca];

                                customAttribute.array[offset] = value.x;
                                customAttribute.array[offset + 1] = value.y;
                                customAttribute.array[offset + 2] = value.z;
                                customAttribute.array[offset + 3] = value.w;

                                offset += 4;
                            }
                        }

                        _gl.bindBuffer(_gl.ARRAY_BUFFER, customAttribute.buffer);
                        _gl.bufferData(_gl.ARRAY_BUFFER, customAttribute.array, hint);
                    }
                }
            }
        }

        private void setRibbonBuffers(dynamic geometry, dynamic hint)
        {
            var vertices = geometry.vertices;
            var colors = geometry.colors;
            var normals = geometry.normals;
            var vl = vertices.length;
            var cl = colors.length;
            var nl = normals.length;
            var vertexArray = geometry.__vertexArray;
            var colorArray = geometry.__colorArray;
            var normalArray = geometry.__normalArray;
            var dirtyVertices = geometry.verticesNeedUpdate;
            var dirtyColors = geometry.colorsNeedUpdate;
            var dirtyNormals = geometry.normalsNeedUpdate;
            var customAttributes = geometry.__webglCustomAttributesList;

            if (dirtyVertices)
            {
                for (var v = 0; v < vl; v++)
                {
                    var vertex = vertices[v];

                    var offset = v * 3;

                    vertexArray[offset] = (float)vertex.x;
                    vertexArray[offset + 1] = (float)vertex.y;
                    vertexArray[offset + 2] = (float)vertex.z;
                }

                _gl.bindBuffer(_gl.ARRAY_BUFFER, geometry.__webglVertexBuffer);
                _gl.bufferData(_gl.ARRAY_BUFFER, vertexArray, hint);
            }

            if (dirtyColors)
            {
                for (var c = 0; c < cl; c++)
                {
                    var color = colors[c];

                    var offset = c * 3;

                    colorArray[offset] = color.r;
                    colorArray[offset + 1] = color.g;
                    colorArray[offset + 2] = color.b;
                }

                _gl.bindBuffer(_gl.ARRAY_BUFFER, geometry.__webglColorBuffer);
                _gl.bufferData(_gl.ARRAY_BUFFER, colorArray, hint);
            }

            if (dirtyNormals)
            {
                for (var n = 0; n < nl; n++)
                {
                    var normal = normals[n];

                    var offset = n * 3;

                    normalArray[offset] = normal.x;
                    normalArray[offset + 1] = normal.y;
                    normalArray[offset + 2] = normal.z;
                }

                _gl.bindBuffer(_gl.ARRAY_BUFFER, geometry.__webglNormalBuffer);
                _gl.bufferData(_gl.ARRAY_BUFFER, normalArray, hint);
            }

            if (JSObject.eval(customAttributes))
            {
                for (int i = 0, il = customAttributes.length; i < il; i++)
                {
                    var customAttribute = customAttributes[i];

                    if (customAttribute.needsUpdate &&
                        (customAttribute.boundTo == null ||
                         customAttribute.boundTo == "vertices"))
                    {
                        var offset = 0;

                        var cal = customAttribute.value.length;

                        if (customAttribute.size == 1)
                        {
                            for (var ca = 0; ca < cal; ca++)
                            {
                                customAttribute.array[ca] = customAttribute.value[ca];
                            }
                        }
                        else if (customAttribute.size == 2)
                        {
                            for (var ca = 0; ca < cal; ca++)
                            {
                                var value = customAttribute.value[ca];

                                customAttribute.array[offset] = value.x;
                                customAttribute.array[offset + 1] = value.y;

                                offset += 2;
                            }
                        }
                        else if (customAttribute.size == 3)
                        {
                            if (customAttribute.type == "c")
                            {
                                for (var ca = 0; ca < cal; ca++)
                                {
                                    var value = customAttribute.value[ca];

                                    customAttribute.array[offset] = (float)value.r;
                                    customAttribute.array[offset + 1] = (float)value.g;
                                    customAttribute.array[offset + 2] = (float)value.b;

                                    offset += 3;
                                }
                            }
                            else
                            {
                                for (var ca = 0; ca < cal; ca++)
                                {
                                    var value = customAttribute.value[ca];

                                    customAttribute.array[offset] = (float)value.x;
                                    customAttribute.array[offset + 1] = (float)value.y;
                                    customAttribute.array[offset + 2] = (float)value.z;

                                    offset += 3;
                                }
                            }
                        }
                        else if (customAttribute.size == 4)
                        {
                            for (var ca = 0; ca < cal; ca++)
                            {
                                var value = customAttribute.value[ca];

                                customAttribute.array[offset] = value.x;
                                customAttribute.array[offset + 1] = value.y;
                                customAttribute.array[offset + 2] = value.z;
                                customAttribute.array[offset + 3] = value.w;

                                offset += 4;
                            }
                        }

                        _gl.bindBuffer(_gl.ARRAY_BUFFER, customAttribute.buffer);
                        _gl.bufferData(_gl.ARRAY_BUFFER, customAttribute.array, hint);
                    }
                }
            }
        }

        private void setMeshBuffers(dynamic geometryGroup, Object3D obj, uint hint, bool dispose, dynamic material)
        {
            if (!JSObject.eval(geometryGroup.__inittedArrays))
            {
                return;
            }

            var normalType = bufferGuessNormalType(material);
            var vertexColorType = bufferGuessVertexColorType(material);
            var uvType = bufferGuessUVType(material);
            var needsSmoothNormals = (normalType == THREE.SmoothShading);
            var vertexIndex = 0;
            var offset = 0;
            var offset_uv = 0;
            var offset_uv2 = 0;
            var offset_face = 0;
            var offset_normal = 0;
            var offset_tangent = 0;
            var offset_line = 0;
            var offset_color = 0;
            var offset_skin = 0;
            var offset_morphTarget = 0;
            var offset_custom = 0;
            var offset_customSrc = 0;
            var vertexArray = geometryGroup.__vertexArray;
            var uvArray = geometryGroup.__uvArray;
            var uv2Array = geometryGroup.__uv2Array;
            var normalArray = geometryGroup.__normalArray;
            var tangentArray = geometryGroup.__tangentArray;
            var colorArray = geometryGroup.__colorArray;
            var skinIndexArray = geometryGroup.__skinIndexArray;
            var skinWeightArray = geometryGroup.__skinWeightArray;
            var morphTargetsArrays = geometryGroup.__morphTargetsArrays;
            var morphNormalsArrays = geometryGroup.__morphNormalsArrays;
            var customAttributes = geometryGroup.__webglCustomAttributesList;
            var faceArray = geometryGroup.__faceArray;
            var lineArray = geometryGroup.__lineArray;
            var geometry = obj.geometry;
            var dirtyVertices = geometry.verticesNeedUpdate;
            var dirtyElements = geometry.elementsNeedUpdate;
            var dirtyUvs = geometry.uvsNeedUpdate;
            var dirtyNormals = geometry.normalsNeedUpdate;
            var dirtyTangents = geometry.tangentsNeedUpdate;
            var dirtyColors = geometry.colorsNeedUpdate;
            var dirtyMorphTargets = geometry.morphTargetsNeedUpdate;
            var vertices = geometry.vertices;
            var chunk_faces3 = geometryGroup.faces3;
            var chunk_faces4 = geometryGroup.faces4;
            var obj_faces = geometry.faces;
            var obj_uvs = geometry.faceVertexUvs[0];
            var obj_uvs2 = geometry.faceVertexUvs[1];
            var obj_colors = geometry.colors;
            var obj_skinIndices = geometry.skinIndices;
            var obj_skinWeights = geometry.skinWeights;
            var morphTargets = geometry.morphTargets;
            var morphNormals = geometry.morphNormals;

            if (dirtyVertices)
            {
                for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                {
                    var face = obj_faces[chunk_faces3[f]];

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

                for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                {
                    var face = obj_faces[chunk_faces4[f]];

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
                _gl.bufferData(_gl.ARRAY_BUFFER, vertexArray, hint);
            }

            if (dirtyMorphTargets)
            {
                for (int vk = 0, vkl = morphTargets.length; vk < vkl; vk++)
                {
                    offset_morphTarget = 0;

                    for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                    {
                        var chf = chunk_faces3[f];
                        var face = obj_faces[chf];

                        var v1 = morphTargets[vk].vertices[face.a];
                        var v2 = morphTargets[vk].vertices[face.b];
                        var v3 = morphTargets[vk].vertices[face.c];

                        var vka = morphTargetsArrays[vk];

                        vka[offset_morphTarget] = (float)v1.x;
                        vka[offset_morphTarget + 1] = (float)v1.y;
                        vka[offset_morphTarget + 2] = (float)v1.z;

                        vka[offset_morphTarget + 3] = (float)v2.x;
                        vka[offset_morphTarget + 4] = (float)v2.y;
                        vka[offset_morphTarget + 5] = (float)v2.z;

                        vka[offset_morphTarget + 6] = (float)v3.x;
                        vka[offset_morphTarget + 7] = (float)v3.y;
                        vka[offset_morphTarget + 8] = (float)v3.z;

                        if (material.morphNormals)
                        {
                            Vector3 n1, n2, n3;
                            if (needsSmoothNormals)
                            {
                                var faceVertexNormals = morphNormals[vk].vertexNormals[chf];
                                n1 = faceVertexNormals.a;
                                n2 = faceVertexNormals.b;
                                n3 = faceVertexNormals.c;
                            }
                            else
                            {
                                n1 = morphNormals[vk].faceNormals[chf];
                                n2 = n1;
                                n3 = n1;
                            }

                            var nka = morphNormalsArrays[vk];

                            nka[offset_morphTarget] = (float)n1.x;
                            nka[offset_morphTarget + 1] = (float)n1.y;
                            nka[offset_morphTarget + 2] = (float)n1.z;

                            nka[offset_morphTarget + 3] = (float)n2.x;
                            nka[offset_morphTarget + 4] = (float)n2.y;
                            nka[offset_morphTarget + 5] = (float)n2.z;

                            nka[offset_morphTarget + 6] = (float)n3.x;
                            nka[offset_morphTarget + 7] = (float)n3.y;
                            nka[offset_morphTarget + 8] = (float)n3.z;
                        }

                        offset_morphTarget += 9;
                    }

                    for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                    {
                        var chf = chunk_faces4[f];
                        var face = obj_faces[chf];

                        var v1 = morphTargets[vk].vertices[face.a];
                        var v2 = morphTargets[vk].vertices[face.b];
                        var v3 = morphTargets[vk].vertices[face.c];
                        var v4 = morphTargets[vk].vertices[face.d];

                        var vka = morphTargetsArrays[vk];

                        vka[offset_morphTarget] = (float)v1.x;
                        vka[offset_morphTarget + 1] = (float)v1.y;
                        vka[offset_morphTarget + 2] = (float)v1.z;

                        vka[offset_morphTarget + 3] = (float)v2.x;
                        vka[offset_morphTarget + 4] = (float)v2.y;
                        vka[offset_morphTarget + 5] = (float)v2.z;

                        vka[offset_morphTarget + 6] = (float)v3.x;
                        vka[offset_morphTarget + 7] = (float)v3.y;
                        vka[offset_morphTarget + 8] = (float)v3.z;

                        vka[offset_morphTarget + 9] = (float)v4.x;
                        vka[offset_morphTarget + 10] = (float)v4.y;
                        vka[offset_morphTarget + 11] = (float)v4.z;

                        if (material.morphNormals)
                        {
                            dynamic n1, n2, n3, n4;
                            if (needsSmoothNormals)
                            {
                                var faceVertexNormals = morphNormals[vk].vertexNormals[chf];

                                n1 = faceVertexNormals.a;
                                n2 = faceVertexNormals.b;
                                n3 = faceVertexNormals.c;
                                n4 = faceVertexNormals.d;
                            }
                            else
                            {
                                n1 = morphNormals[vk].faceNormals[chf];
                                n2 = n1;
                                n3 = n1;
                                n4 = n1;
                            }

                            var nka = morphNormalsArrays[vk];

                            nka[offset_morphTarget] = n1.x;
                            nka[offset_morphTarget + 1] = n1.y;
                            nka[offset_morphTarget + 2] = n1.z;

                            nka[offset_morphTarget + 3] = n2.x;
                            nka[offset_morphTarget + 4] = n2.y;
                            nka[offset_morphTarget + 5] = n2.z;

                            nka[offset_morphTarget + 6] = n3.x;
                            nka[offset_morphTarget + 7] = n3.y;
                            nka[offset_morphTarget + 8] = n3.z;

                            nka[offset_morphTarget + 9] = n4.x;
                            nka[offset_morphTarget + 10] = n4.y;
                            nka[offset_morphTarget + 11] = n4.z;
                        }

                        offset_morphTarget += 12;
                    }

                    _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglMorphTargetsBuffers[vk]);
                    _gl.bufferData(_gl.ARRAY_BUFFER, morphTargetsArrays[vk], hint);

                    if (material.morphNormals)
                    {
                        _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglMorphNormalsBuffers[vk]);
                        _gl.bufferData(_gl.ARRAY_BUFFER, morphNormalsArrays[vk], hint);
                    }
                }
            }

            if (obj_skinWeights.length > 0)
            {
                for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                {
                    var face = obj_faces[chunk_faces3[f]];

                    var sw1 = obj_skinWeights[face.a];
                    var sw2 = obj_skinWeights[face.b];
                    var sw3 = obj_skinWeights[face.c];

                    skinWeightArray[offset_skin] = (float)sw1.x;
                    skinWeightArray[offset_skin + 1] = (float)sw1.y;
                    skinWeightArray[offset_skin + 2] = (float)sw1.z;
                    skinWeightArray[offset_skin + 3] = (float)sw1.w;

                    skinWeightArray[offset_skin + 4] = (float)sw2.x;
                    skinWeightArray[offset_skin + 5] = (float)sw2.y;
                    skinWeightArray[offset_skin + 6] = (float)sw2.z;
                    skinWeightArray[offset_skin + 7] = (float)sw2.w;

                    skinWeightArray[offset_skin + 8] = (float)sw3.x;
                    skinWeightArray[offset_skin + 9] = (float)sw3.y;
                    skinWeightArray[offset_skin + 10] = (float)sw3.z;
                    skinWeightArray[offset_skin + 11] = (float)sw3.w;

                    var si1 = obj_skinIndices[face.a];
                    var si2 = obj_skinIndices[face.b];
                    var si3 = obj_skinIndices[face.c];

                    skinIndexArray[offset_skin] = (float)si1.x;
                    skinIndexArray[offset_skin + 1] = (float)si1.y;
                    skinIndexArray[offset_skin + 2] = (float)si1.z;
                    skinIndexArray[offset_skin + 3] = (float)si1.w;

                    skinIndexArray[offset_skin + 4] = (float)si2.x;
                    skinIndexArray[offset_skin + 5] = (float)si2.y;
                    skinIndexArray[offset_skin + 6] = (float)si2.z;
                    skinIndexArray[offset_skin + 7] = (float)si2.w;

                    skinIndexArray[offset_skin + 8] = (float)si3.x;
                    skinIndexArray[offset_skin + 9] = (float)si3.y;
                    skinIndexArray[offset_skin + 10] = (float)si3.z;
                    skinIndexArray[offset_skin + 11] = (float)si3.w;

                    offset_skin += 12;
                }

                for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                {
                    var face = obj_faces[chunk_faces4[f]];

                    var sw1 = obj_skinWeights[face.a];
                    var sw2 = obj_skinWeights[face.b];
                    var sw3 = obj_skinWeights[face.c];
                    var sw4 = obj_skinWeights[face.d];

                    skinWeightArray[offset_skin] = (float)sw1.x;
                    skinWeightArray[offset_skin + 1] = (float)sw1.y;
                    skinWeightArray[offset_skin + 2] = (float)sw1.z;
                    skinWeightArray[offset_skin + 3] = (float)sw1.w;

                    skinWeightArray[offset_skin + 4] = (float)sw2.x;
                    skinWeightArray[offset_skin + 5] = (float)sw2.y;
                    skinWeightArray[offset_skin + 6] = (float)sw2.z;
                    skinWeightArray[offset_skin + 7] = (float)sw2.w;

                    skinWeightArray[offset_skin + 8] = (float)sw3.x;
                    skinWeightArray[offset_skin + 9] = (float)sw3.y;
                    skinWeightArray[offset_skin + 10] = (float)sw3.z;
                    skinWeightArray[offset_skin + 11] = (float)sw3.w;

                    skinWeightArray[offset_skin + 12] = (float)sw4.x;
                    skinWeightArray[offset_skin + 13] = (float)sw4.y;
                    skinWeightArray[offset_skin + 14] = (float)sw4.z;
                    skinWeightArray[offset_skin + 15] = (float)sw4.w;

                    var si1 = obj_skinIndices[face.a];
                    var si2 = obj_skinIndices[face.b];
                    var si3 = obj_skinIndices[face.c];
                    var si4 = obj_skinIndices[face.d];

                    skinIndexArray[offset_skin] = (float)si1.x;
                    skinIndexArray[offset_skin + 1] = (float)si1.y;
                    skinIndexArray[offset_skin + 2] = (float)si1.z;
                    skinIndexArray[offset_skin + 3] = (float)si1.w;

                    skinIndexArray[offset_skin + 4] = (float)si2.x;
                    skinIndexArray[offset_skin + 5] = (float)si2.y;
                    skinIndexArray[offset_skin + 6] = (float)si2.z;
                    skinIndexArray[offset_skin + 7] = (float)si2.w;

                    skinIndexArray[offset_skin + 8] = (float)si3.x;
                    skinIndexArray[offset_skin + 9] = (float)si3.y;
                    skinIndexArray[offset_skin + 10] = (float)si3.z;
                    skinIndexArray[offset_skin + 11] = (float)si3.w;

                    skinIndexArray[offset_skin + 12] = (float)si4.x;
                    skinIndexArray[offset_skin + 13] = (float)si4.y;
                    skinIndexArray[offset_skin + 14] = (float)si4.z;
                    skinIndexArray[offset_skin + 15] = (float)si4.w;

                    offset_skin += 16;
                }

                if (offset_skin > 0)
                {
                    _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglSkinIndicesBuffer);
                    _gl.bufferData(_gl.ARRAY_BUFFER, skinIndexArray, hint);

                    _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglSkinWeightsBuffer);
                    _gl.bufferData(_gl.ARRAY_BUFFER, skinWeightArray, hint);
                }
            }

            if (dirtyColors && JSObject.eval(vertexColorType))
            {
                for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                {
                    var face = obj_faces[chunk_faces3[f]];

                    var vertexColors = face.vertexColors;
                    var faceColor = face.color;

                    dynamic c1, c2, c3;
                    if (vertexColors.length == 3 && vertexColorType == THREE.VertexColors)
                    {
                        c1 = vertexColors[0];
                        c2 = vertexColors[1];
                        c3 = vertexColors[2];
                    }
                    else
                    {
                        c1 = faceColor;
                        c2 = faceColor;
                        c3 = faceColor;
                    }

                    colorArray[offset_color] = (float)c1.r;
                    colorArray[offset_color + 1] = (float)c1.g;
                    colorArray[offset_color + 2] = (float)c1.b;

                    colorArray[offset_color + 3] = (float)c2.r;
                    colorArray[offset_color + 4] = (float)c2.g;
                    colorArray[offset_color + 5] = (float)c2.b;

                    colorArray[offset_color + 6] = (float)c3.r;
                    colorArray[offset_color + 7] = (float)c3.g;
                    colorArray[offset_color + 8] = (float)c3.b;

                    offset_color += 9;
                }

                for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                {
                    var face = obj_faces[chunk_faces4[f]];

                    var vertexColors = face.vertexColors;
                    var faceColor = face.color;
                    dynamic c1, c2, c3, c4;
                    if (vertexColors.length == 4 && vertexColorType == THREE.VertexColors)
                    {
                        c1 = vertexColors[0];
                        c2 = vertexColors[1];
                        c3 = vertexColors[2];
                        c4 = vertexColors[3];
                    }
                    else
                    {
                        c1 = faceColor;
                        c2 = faceColor;
                        c3 = faceColor;
                        c4 = faceColor;
                    }

                    colorArray[offset_color] = c1.r;
                    colorArray[offset_color + 1] = c1.g;
                    colorArray[offset_color + 2] = c1.b;

                    colorArray[offset_color + 3] = c2.r;
                    colorArray[offset_color + 4] = c2.g;
                    colorArray[offset_color + 5] = c2.b;

                    colorArray[offset_color + 6] = c3.r;
                    colorArray[offset_color + 7] = c3.g;
                    colorArray[offset_color + 8] = c3.b;

                    colorArray[offset_color + 9] = c4.r;
                    colorArray[offset_color + 10] = c4.g;
                    colorArray[offset_color + 11] = c4.b;

                    offset_color += 12;
                }

                if (offset_color > 0)
                {
                    _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglColorBuffer);
                    _gl.bufferData(_gl.ARRAY_BUFFER, colorArray, hint);
                }
            }

            if (dirtyTangents && geometry.hasTangents)
            {
                for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                {
                    var face = obj_faces[chunk_faces3[f]];

                    var vertexTangents = face.vertexTangents;

                    var t1 = vertexTangents[0];
                    var t2 = vertexTangents[1];
                    var t3 = vertexTangents[2];

                    tangentArray[offset_tangent] = t1.x;
                    tangentArray[offset_tangent + 1] = t1.y;
                    tangentArray[offset_tangent + 2] = t1.z;
                    tangentArray[offset_tangent + 3] = t1.w;

                    tangentArray[offset_tangent + 4] = t2.x;
                    tangentArray[offset_tangent + 5] = t2.y;
                    tangentArray[offset_tangent + 6] = t2.z;
                    tangentArray[offset_tangent + 7] = t2.w;

                    tangentArray[offset_tangent + 8] = t3.x;
                    tangentArray[offset_tangent + 9] = t3.y;
                    tangentArray[offset_tangent + 10] = t3.z;
                    tangentArray[offset_tangent + 11] = t3.w;

                    offset_tangent += 12;
                }

                for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                {
                    var face = obj_faces[chunk_faces4[f]];

                    var vertexTangents = face.vertexTangents;

                    var t1 = vertexTangents[0];
                    var t2 = vertexTangents[1];
                    var t3 = vertexTangents[2];
                    var t4 = vertexTangents[3];

                    tangentArray[offset_tangent] = t1.x;
                    tangentArray[offset_tangent + 1] = t1.y;
                    tangentArray[offset_tangent + 2] = t1.z;
                    tangentArray[offset_tangent + 3] = t1.w;

                    tangentArray[offset_tangent + 4] = t2.x;
                    tangentArray[offset_tangent + 5] = t2.y;
                    tangentArray[offset_tangent + 6] = t2.z;
                    tangentArray[offset_tangent + 7] = t2.w;

                    tangentArray[offset_tangent + 8] = t3.x;
                    tangentArray[offset_tangent + 9] = t3.y;
                    tangentArray[offset_tangent + 10] = t3.z;
                    tangentArray[offset_tangent + 11] = t3.w;

                    tangentArray[offset_tangent + 12] = t4.x;
                    tangentArray[offset_tangent + 13] = t4.y;
                    tangentArray[offset_tangent + 14] = t4.z;
                    tangentArray[offset_tangent + 15] = t4.w;

                    offset_tangent += 16;
                }

                _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglTangentBuffer);
                _gl.bufferData(_gl.ARRAY_BUFFER, tangentArray, hint);
            }

            if (dirtyNormals && normalType != THREE.NoShading)
            {
                for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                {
                    var face = obj_faces[chunk_faces3[f]];

                    var vertexNormals = face.vertexNormals;
                    var faceNormal = face.normal;

                    if (vertexNormals.length == 3 && needsSmoothNormals)
                    {
                        for (var i = 0; i < 3; i++)
                        {
                            var vn = vertexNormals[i];

                            normalArray[offset_normal] = (float)vn.x;
                            normalArray[offset_normal + 1] = (float)vn.y;
                            normalArray[offset_normal + 2] = (float)vn.z;

                            offset_normal += 3;
                        }
                    }
                    else
                    {
                        for (var i = 0; i < 3; i++)
                        {
                            normalArray[offset_normal] = (float)faceNormal.x;
                            normalArray[offset_normal + 1] = (float)faceNormal.y;
                            normalArray[offset_normal + 2] = (float)faceNormal.z;

                            offset_normal += 3;
                        }
                    }
                }

                for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                {
                    var face = obj_faces[chunk_faces4[f]];

                    var vertexNormals = face.vertexNormals;
                    var faceNormal = face.normal;

                    if (vertexNormals.length == 4 && needsSmoothNormals)
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            var vn = vertexNormals[i];

                            normalArray[offset_normal] = (float)vn.x;
                            normalArray[offset_normal + 1] = (float)vn.y;
                            normalArray[offset_normal + 2] = (float)vn.z;

                            offset_normal += 3;
                        }
                    }
                    else
                    {
                        for (var i = 0; i < 4; i++)
                        {
                            normalArray[offset_normal] = (float)faceNormal.x;
                            normalArray[offset_normal + 1] = (float)faceNormal.y;
                            normalArray[offset_normal + 2] = (float)faceNormal.z;

                            offset_normal += 3;
                        }
                    }
                }

                _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglNormalBuffer);
                _gl.bufferData(_gl.ARRAY_BUFFER, normalArray, hint);
            }

            if (dirtyUvs && obj_uvs != null && uvType)
            {
                for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                {
                    var fi = chunk_faces3[f];

                    var uv = obj_uvs[fi];

                    if (uv == null)
                    {
                        continue;
                    }

                    for (var i = 0; i < 3; i++)
                    {
                        var uvi = uv[i];

                        uvArray[offset_uv] = (float)uvi.x;
                        uvArray[offset_uv + 1] = (float)uvi.y;

                        offset_uv += 2;
                    }
                }

                for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                {
                    var fi = chunk_faces4[f];

                    var uv = obj_uvs[fi];

                    if (uv == null)
                    {
                        continue;
                    }

                    for (var i = 0; i < 4; i++)
                    {
                        var uvi = uv[i];

                        uvArray[offset_uv] = (float)uvi.x;
                        uvArray[offset_uv + 1] = (float)uvi.y;

                        offset_uv += 2;
                    }
                }

                if (offset_uv > 0)
                {
                    _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglUVBuffer);
                    _gl.bufferData(_gl.ARRAY_BUFFER, uvArray, hint);
                }
            }

            if (dirtyUvs && obj_uvs2 != null && uvType)
            {
                for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                {
                    var fi = chunk_faces3[f];

                    var uv2 = obj_uvs2[fi];

                    if (uv2 == null)
                    {
                        continue;
                    }

                    for (var i = 0; i < 3; i++)
                    {
                        var uv2i = uv2[i];

                        uv2Array[offset_uv2] = (float)uv2i.x;
                        uv2Array[offset_uv2 + 1] = (float)uv2i.y;

                        offset_uv2 += 2;
                    }
                }

                for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                {
                    var fi = chunk_faces4[f];

                    var uv2 = obj_uvs2[fi];

                    if (uv2 == null)
                    {
                        continue;
                    }

                    for (var i = 0; i < 4; i++)
                    {
                        var uv2i = uv2[i];

                        uv2Array[offset_uv2] = (float)uv2i.x;
                        uv2Array[offset_uv2 + 1] = (float)uv2i.y;

                        offset_uv2 += 2;
                    }
                }

                if (offset_uv2 > 0)
                {
                    _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglUV2Buffer);
                    _gl.bufferData(_gl.ARRAY_BUFFER, uv2Array, hint);
                }
            }

            if (dirtyElements)
            {
                for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                {
                    faceArray[offset_face] = (ushort)vertexIndex;
                    faceArray[offset_face + 1] = (ushort)(vertexIndex + 1);
                    faceArray[offset_face + 2] = (ushort)(vertexIndex + 2);

                    offset_face += 3;

                    lineArray[offset_line] = (ushort)vertexIndex;
                    lineArray[offset_line + 1] = (ushort)(vertexIndex + 1);

                    lineArray[offset_line + 2] = (ushort)(vertexIndex);
                    lineArray[offset_line + 3] = (ushort)(vertexIndex + 2);

                    lineArray[offset_line + 4] = (ushort)(vertexIndex + 1);
                    lineArray[offset_line + 5] = (ushort)(vertexIndex + 2);

                    offset_line += 6;

                    vertexIndex += 3;
                }

                for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                {
                    faceArray[offset_face] = (ushort)vertexIndex;
                    faceArray[offset_face + 1] = (ushort)(vertexIndex + 1);
                    faceArray[offset_face + 2] = (ushort)(vertexIndex + 3);

                    faceArray[offset_face + 3] = (ushort)(vertexIndex + 1);
                    faceArray[offset_face + 4] = (ushort)(vertexIndex + 2);
                    faceArray[offset_face + 5] = (ushort)(vertexIndex + 3);

                    offset_face += 6;

                    lineArray[offset_line] = (ushort)vertexIndex;
                    lineArray[offset_line + 1] = (ushort)(vertexIndex + 1);

                    lineArray[offset_line + 2] = (ushort)(vertexIndex);
                    lineArray[offset_line + 3] = (ushort)(vertexIndex + 3);

                    lineArray[offset_line + 4] = (ushort)(vertexIndex + 1);
                    lineArray[offset_line + 5] = (ushort)(vertexIndex + 2);

                    lineArray[offset_line + 6] = (ushort)(vertexIndex + 2);
                    lineArray[offset_line + 7] = (ushort)(vertexIndex + 3);

                    offset_line += 8;

                    vertexIndex += 4;
                }

                _gl.bindBuffer(_gl.ELEMENT_ARRAY_BUFFER, geometryGroup.__webglFaceBuffer);
                _gl.bufferData(_gl.ELEMENT_ARRAY_BUFFER, faceArray, hint);

                _gl.bindBuffer(_gl.ELEMENT_ARRAY_BUFFER, geometryGroup.__webglLineBuffer);
                _gl.bufferData(_gl.ELEMENT_ARRAY_BUFFER, lineArray, hint);
            }

            if (customAttributes != null)
            {
                for (int i = 0, il = customAttributes.length; i < il; i++)
                {
                    var customAttribute = customAttributes[i];

                    if (!customAttribute.__original.needsUpdate)
                    {
                        continue;
                    }

                    offset_custom = 0;
                    offset_customSrc = 0;

                    if (customAttribute.size == 1)
                    {
                        if (customAttribute.boundTo == null || customAttribute.boundTo == "vertices")
                        {
                            for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                            {
                                var face = obj_faces[chunk_faces3[f]];

                                customAttribute.array[offset_custom] = (float)customAttribute.value[face.a];
                                customAttribute.array[offset_custom + 1] = (float)customAttribute.value[face.b];
                                customAttribute.array[offset_custom + 2] = (float)customAttribute.value[face.c];

                                offset_custom += 3;
                            }

                            for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                            {
                                var face = obj_faces[chunk_faces4[f]];

                                customAttribute.array[offset_custom] = (float)customAttribute.value[face.a];
                                customAttribute.array[offset_custom + 1] = (float)customAttribute.value[face.b];
                                customAttribute.array[offset_custom + 2] = (float)customAttribute.value[face.c];
                                customAttribute.array[offset_custom + 3] = (float)customAttribute.value[face.d];

                                offset_custom += 4;
                            }
                        }
                        else if (customAttribute.boundTo == "faces")
                        {
                            for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                            {
                                var value = customAttribute.value[chunk_faces3[f]];

                                customAttribute.array[offset_custom] = value;
                                customAttribute.array[offset_custom + 1] = value;
                                customAttribute.array[offset_custom + 2] = value;

                                offset_custom += 3;
                            }

                            for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                            {
                                var value = customAttribute.value[chunk_faces4[f]];

                                customAttribute.array[offset_custom] = value;
                                customAttribute.array[offset_custom + 1] = value;
                                customAttribute.array[offset_custom + 2] = value;
                                customAttribute.array[offset_custom + 3] = value;

                                offset_custom += 4;
                            }
                        }
                    }
                    else if (customAttribute.size == 2)
                    {
                        if (customAttribute.boundTo == null || customAttribute.boundTo == "vertices")
                        {
                            for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                            {
                                var face = obj_faces[chunk_faces3[f]];

                                var v1 = customAttribute.value[face.a];
                                var v2 = customAttribute.value[face.b];
                                var v3 = customAttribute.value[face.c];

                                customAttribute.array[offset_custom] = v1.x;
                                customAttribute.array[offset_custom + 1] = v1.y;

                                customAttribute.array[offset_custom + 2] = v2.x;
                                customAttribute.array[offset_custom + 3] = v2.y;

                                customAttribute.array[offset_custom + 4] = v3.x;
                                customAttribute.array[offset_custom + 5] = v3.y;

                                offset_custom += 6;
                            }

                            for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                            {
                                var face = obj_faces[chunk_faces4[f]];

                                var v1 = customAttribute.value[face.a];
                                var v2 = customAttribute.value[face.b];
                                var v3 = customAttribute.value[face.c];
                                var v4 = customAttribute.value[face.d];

                                customAttribute.array[offset_custom] = v1.x;
                                customAttribute.array[offset_custom + 1] = v1.y;

                                customAttribute.array[offset_custom + 2] = v2.x;
                                customAttribute.array[offset_custom + 3] = v2.y;

                                customAttribute.array[offset_custom + 4] = v3.x;
                                customAttribute.array[offset_custom + 5] = v3.y;

                                customAttribute.array[offset_custom + 6] = v4.x;
                                customAttribute.array[offset_custom + 7] = v4.y;

                                offset_custom += 8;
                            }
                        }
                        else if (customAttribute.boundTo == "faces")
                        {
                            for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                            {
                                var value = customAttribute.value[chunk_faces3[f]];

                                var v1 = value;
                                var v2 = value;
                                var v3 = value;

                                customAttribute.array[offset_custom] = v1.x;
                                customAttribute.array[offset_custom + 1] = v1.y;

                                customAttribute.array[offset_custom + 2] = v2.x;
                                customAttribute.array[offset_custom + 3] = v2.y;

                                customAttribute.array[offset_custom + 4] = v3.x;
                                customAttribute.array[offset_custom + 5] = v3.y;

                                offset_custom += 6;
                            }

                            for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                            {
                                var value = customAttribute.value[chunk_faces4[f]];

                                var v1 = value;
                                var v2 = value;
                                var v3 = value;
                                var v4 = value;

                                customAttribute.array[offset_custom] = v1.x;
                                customAttribute.array[offset_custom + 1] = v1.y;

                                customAttribute.array[offset_custom + 2] = v2.x;
                                customAttribute.array[offset_custom + 3] = v2.y;

                                customAttribute.array[offset_custom + 4] = v3.x;
                                customAttribute.array[offset_custom + 5] = v3.y;

                                customAttribute.array[offset_custom + 6] = v4.x;
                                customAttribute.array[offset_custom + 7] = v4.y;

                                offset_custom += 8;
                            }
                        }
                    }
                    else if (customAttribute.size == 3)
                    {
                        JSArray pp;

                        if (customAttribute.type == "c")
                        {
                            pp = new JSArray("r", "g", "b");
                        }
                        else
                        {
                            pp = new JSArray("x", "y", "z");
                        }

                        if (customAttribute.boundTo == null || customAttribute.boundTo == "vertices")
                        {
                            for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                            {
                                var face = obj_faces[chunk_faces3[f]];

                                var v1 = customAttribute.value[face.a];
                                var v2 = customAttribute.value[face.b];
                                var v3 = customAttribute.value[face.c];

                                customAttribute.array[offset_custom] = v1[pp[0]];
                                customAttribute.array[offset_custom + 1] = v1[pp[1]];
                                customAttribute.array[offset_custom + 2] = v1[pp[2]];

                                customAttribute.array[offset_custom + 3] = v2[pp[0]];
                                customAttribute.array[offset_custom + 4] = v2[pp[1]];
                                customAttribute.array[offset_custom + 5] = v2[pp[2]];

                                customAttribute.array[offset_custom + 6] = v3[pp[0]];
                                customAttribute.array[offset_custom + 7] = v3[pp[1]];
                                customAttribute.array[offset_custom + 8] = v3[pp[2]];

                                offset_custom += 9;
                            }

                            for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                            {
                                var face = obj_faces[chunk_faces4[f]];

                                var v1 = customAttribute.value[face.a];
                                var v2 = customAttribute.value[face.b];
                                var v3 = customAttribute.value[face.c];
                                var v4 = customAttribute.value[face.d];

                                customAttribute.array[offset_custom] = v1[pp[0]];
                                customAttribute.array[offset_custom + 1] = v1[pp[1]];
                                customAttribute.array[offset_custom + 2] = v1[pp[2]];

                                customAttribute.array[offset_custom + 3] = v2[pp[0]];
                                customAttribute.array[offset_custom + 4] = v2[pp[1]];
                                customAttribute.array[offset_custom + 5] = v2[pp[2]];

                                customAttribute.array[offset_custom + 6] = v3[pp[0]];
                                customAttribute.array[offset_custom + 7] = v3[pp[1]];
                                customAttribute.array[offset_custom + 8] = v3[pp[2]];

                                customAttribute.array[offset_custom + 9] = v4[pp[0]];
                                customAttribute.array[offset_custom + 10] = v4[pp[1]];
                                customAttribute.array[offset_custom + 11] = v4[pp[2]];

                                offset_custom += 12;
                            }
                        }
                        else if (customAttribute.boundTo == "faces")
                        {
                            for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                            {
                                var value = customAttribute.value[chunk_faces3[f]];

                                var v1 = value;
                                var v2 = value;
                                var v3 = value;

                                customAttribute.array[offset_custom] = v1[pp[0]];
                                customAttribute.array[offset_custom + 1] = v1[pp[1]];
                                customAttribute.array[offset_custom + 2] = v1[pp[2]];

                                customAttribute.array[offset_custom + 3] = v2[pp[0]];
                                customAttribute.array[offset_custom + 4] = v2[pp[1]];
                                customAttribute.array[offset_custom + 5] = v2[pp[2]];

                                customAttribute.array[offset_custom + 6] = v3[pp[0]];
                                customAttribute.array[offset_custom + 7] = v3[pp[1]];
                                customAttribute.array[offset_custom + 8] = v3[pp[2]];

                                offset_custom += 9;
                            }

                            for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                            {
                                var value = customAttribute.value[chunk_faces4[f]];

                                var v1 = value;
                                var v2 = value;
                                var v3 = value;
                                var v4 = value;

                                customAttribute.array[offset_custom] = v1[pp[0]];
                                customAttribute.array[offset_custom + 1] = v1[pp[1]];
                                customAttribute.array[offset_custom + 2] = v1[pp[2]];

                                customAttribute.array[offset_custom + 3] = v2[pp[0]];
                                customAttribute.array[offset_custom + 4] = v2[pp[1]];
                                customAttribute.array[offset_custom + 5] = v2[pp[2]];

                                customAttribute.array[offset_custom + 6] = v3[pp[0]];
                                customAttribute.array[offset_custom + 7] = v3[pp[1]];
                                customAttribute.array[offset_custom + 8] = v3[pp[2]];

                                customAttribute.array[offset_custom + 9] = v4[pp[0]];
                                customAttribute.array[offset_custom + 10] = v4[pp[1]];
                                customAttribute.array[offset_custom + 11] = v4[pp[2]];

                                offset_custom += 12;
                            }
                        }
                        else if (customAttribute.boundTo == "faceVertices")
                        {
                            for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                            {
                                var value = customAttribute.value[chunk_faces3[f]];

                                var v1 = value[0];
                                var v2 = value[1];
                                var v3 = value[2];

                                customAttribute.array[offset_custom] = v1[pp[0]];
                                customAttribute.array[offset_custom + 1] = v1[pp[1]];
                                customAttribute.array[offset_custom + 2] = v1[pp[2]];

                                customAttribute.array[offset_custom + 3] = v2[pp[0]];
                                customAttribute.array[offset_custom + 4] = v2[pp[1]];
                                customAttribute.array[offset_custom + 5] = v2[pp[2]];

                                customAttribute.array[offset_custom + 6] = v3[pp[0]];
                                customAttribute.array[offset_custom + 7] = v3[pp[1]];
                                customAttribute.array[offset_custom + 8] = v3[pp[2]];

                                offset_custom += 9;
                            }

                            for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                            {
                                var value = customAttribute.value[chunk_faces4[f]];

                                var v1 = value[0];
                                var v2 = value[1];
                                var v3 = value[2];
                                var v4 = value[3];

                                customAttribute.array[offset_custom] = v1[pp[0]];
                                customAttribute.array[offset_custom + 1] = v1[pp[1]];
                                customAttribute.array[offset_custom + 2] = v1[pp[2]];

                                customAttribute.array[offset_custom + 3] = v2[pp[0]];
                                customAttribute.array[offset_custom + 4] = v2[pp[1]];
                                customAttribute.array[offset_custom + 5] = v2[pp[2]];

                                customAttribute.array[offset_custom + 6] = v3[pp[0]];
                                customAttribute.array[offset_custom + 7] = v3[pp[1]];
                                customAttribute.array[offset_custom + 8] = v3[pp[2]];

                                customAttribute.array[offset_custom + 9] = v4[pp[0]];
                                customAttribute.array[offset_custom + 10] = v4[pp[1]];
                                customAttribute.array[offset_custom + 11] = v4[pp[2]];

                                offset_custom += 12;
                            }
                        }
                    }
                    else if (customAttribute.size == 4)
                    {
                        if (customAttribute.boundTo == null || customAttribute.boundTo == "vertices")
                        {
                            for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                            {
                                var face = obj_faces[chunk_faces3[f]];

                                var v1 = customAttribute.value[face.a];
                                var v2 = customAttribute.value[face.b];
                                var v3 = customAttribute.value[face.c];

                                customAttribute.array[offset_custom] = v1.x;
                                customAttribute.array[offset_custom + 1] = v1.y;
                                customAttribute.array[offset_custom + 2] = v1.z;
                                customAttribute.array[offset_custom + 3] = v1.w;

                                customAttribute.array[offset_custom + 4] = v2.x;
                                customAttribute.array[offset_custom + 5] = v2.y;
                                customAttribute.array[offset_custom + 6] = v2.z;
                                customAttribute.array[offset_custom + 7] = v2.w;

                                customAttribute.array[offset_custom + 8] = v3.x;
                                customAttribute.array[offset_custom + 9] = v3.y;
                                customAttribute.array[offset_custom + 10] = v3.z;
                                customAttribute.array[offset_custom + 11] = v3.w;

                                offset_custom += 12;
                            }

                            for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                            {
                                var face = obj_faces[chunk_faces4[f]];

                                var v1 = customAttribute.value[face.a];
                                var v2 = customAttribute.value[face.b];
                                var v3 = customAttribute.value[face.c];
                                var v4 = customAttribute.value[face.d];

                                customAttribute.array[offset_custom] = v1.x;
                                customAttribute.array[offset_custom + 1] = v1.y;
                                customAttribute.array[offset_custom + 2] = v1.z;
                                customAttribute.array[offset_custom + 3] = v1.w;

                                customAttribute.array[offset_custom + 4] = v2.x;
                                customAttribute.array[offset_custom + 5] = v2.y;
                                customAttribute.array[offset_custom + 6] = v2.z;
                                customAttribute.array[offset_custom + 7] = v2.w;

                                customAttribute.array[offset_custom + 8] = v3.x;
                                customAttribute.array[offset_custom + 9] = v3.y;
                                customAttribute.array[offset_custom + 10] = v3.z;
                                customAttribute.array[offset_custom + 11] = v3.w;

                                customAttribute.array[offset_custom + 12] = v4.x;
                                customAttribute.array[offset_custom + 13] = v4.y;
                                customAttribute.array[offset_custom + 14] = v4.z;
                                customAttribute.array[offset_custom + 15] = v4.w;

                                offset_custom += 16;
                            }
                        }
                        else if (customAttribute.boundTo == "faces")
                        {
                            for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                            {
                                var value = customAttribute.value[chunk_faces3[f]];

                                var v1 = value;
                                var v2 = value;
                                var v3 = value;

                                customAttribute.array[offset_custom] = v1.x;
                                customAttribute.array[offset_custom + 1] = v1.y;
                                customAttribute.array[offset_custom + 2] = v1.z;
                                customAttribute.array[offset_custom + 3] = v1.w;

                                customAttribute.array[offset_custom + 4] = v2.x;
                                customAttribute.array[offset_custom + 5] = v2.y;
                                customAttribute.array[offset_custom + 6] = v2.z;
                                customAttribute.array[offset_custom + 7] = v2.w;

                                customAttribute.array[offset_custom + 8] = v3.x;
                                customAttribute.array[offset_custom + 9] = v3.y;
                                customAttribute.array[offset_custom + 10] = v3.z;
                                customAttribute.array[offset_custom + 11] = v3.w;

                                offset_custom += 12;
                            }

                            for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                            {
                                var value = customAttribute.value[chunk_faces4[f]];

                                var v1 = value;
                                var v2 = value;
                                var v3 = value;
                                var v4 = value;

                                customAttribute.array[offset_custom] = v1.x;
                                customAttribute.array[offset_custom + 1] = v1.y;
                                customAttribute.array[offset_custom + 2] = v1.z;
                                customAttribute.array[offset_custom + 3] = v1.w;

                                customAttribute.array[offset_custom + 4] = v2.x;
                                customAttribute.array[offset_custom + 5] = v2.y;
                                customAttribute.array[offset_custom + 6] = v2.z;
                                customAttribute.array[offset_custom + 7] = v2.w;

                                customAttribute.array[offset_custom + 8] = v3.x;
                                customAttribute.array[offset_custom + 9] = v3.y;
                                customAttribute.array[offset_custom + 10] = v3.z;
                                customAttribute.array[offset_custom + 11] = v3.w;

                                customAttribute.array[offset_custom + 12] = v4.x;
                                customAttribute.array[offset_custom + 13] = v4.y;
                                customAttribute.array[offset_custom + 14] = v4.z;
                                customAttribute.array[offset_custom + 15] = v4.w;

                                offset_custom += 16;
                            }
                        }
                        else if (customAttribute.boundTo == "faceVertices")
                        {
                            for (int f = 0, fl = chunk_faces3.length; f < fl; f++)
                            {
                                var value = customAttribute.value[chunk_faces3[f]];

                                var v1 = value[0];
                                var v2 = value[1];
                                var v3 = value[2];

                                customAttribute.array[offset_custom] = (float)v1.x;
                                customAttribute.array[offset_custom + 1] = (float)v1.y;
                                customAttribute.array[offset_custom + 2] = (float)v1.z;
                                customAttribute.array[offset_custom + 3] = (float)v1.w;

                                customAttribute.array[offset_custom + 4] = (float)v2.x;
                                customAttribute.array[offset_custom + 5] = (float)v2.y;
                                customAttribute.array[offset_custom + 6] = (float)v2.z;
                                customAttribute.array[offset_custom + 7] = (float)v2.w;

                                customAttribute.array[offset_custom + 8] = (float)v3.x;
                                customAttribute.array[offset_custom + 9] = (float)v3.y;
                                customAttribute.array[offset_custom + 10] = (float)v3.z;
                                customAttribute.array[offset_custom + 11] = (float)v3.w;

                                offset_custom += 12;
                            }

                            for (int f = 0, fl = chunk_faces4.length; f < fl; f++)
                            {
                                var value = customAttribute.value[chunk_faces4[f]];

                                var v1 = value[0];
                                var v2 = value[1];
                                var v3 = value[2];
                                var v4 = value[3];

                                customAttribute.array[offset_custom] = (float)v1.x;
                                customAttribute.array[offset_custom + 1] = (float)v1.y;
                                customAttribute.array[offset_custom + 2] = (float)v1.z;
                                customAttribute.array[offset_custom + 3] = (float)v1.w;

                                customAttribute.array[offset_custom + 4] = (float)v2.x;
                                customAttribute.array[offset_custom + 5] = (float)v2.y;
                                customAttribute.array[offset_custom + 6] = (float)v2.z;
                                customAttribute.array[offset_custom + 7] = (float)v2.w;

                                customAttribute.array[offset_custom + 8] = (float)v3.x;
                                customAttribute.array[offset_custom + 9] = (float)v3.y;
                                customAttribute.array[offset_custom + 10] = (float)v3.z;
                                customAttribute.array[offset_custom + 11] = (float)v3.w;

                                customAttribute.array[offset_custom + 12] = (float)v4.x;
                                customAttribute.array[offset_custom + 13] = (float)v4.y;
                                customAttribute.array[offset_custom + 14] = (float)v4.z;
                                customAttribute.array[offset_custom + 15] = (float)v4.w;

                                offset_custom += 16;
                            }
                        }
                    }

                    _gl.bindBuffer(_gl.ARRAY_BUFFER, customAttribute.buffer);
                    _gl.bufferData(_gl.ARRAY_BUFFER, customAttribute.array, hint);
                }
            }

            if (dispose)
            {
            }
        }

        private void setDirectBuffers(dynamic geometry, dynamic hint, dynamic dispose)
        {
            var attributes = geometry.attributes;

            var index = attributes["index"];
            var position = attributes["position"];
            var normal = attributes["normal"];
            var uv = attributes["uv"];
            var color = attributes["color"];
            var tangent = attributes["tangent"];

            if (geometry.elementsNeedUpdate && index != null)
            {
                _gl.bindBuffer(_gl.ELEMENT_ARRAY_BUFFER, index.buffer);
                _gl.bufferData(_gl.ELEMENT_ARRAY_BUFFER, index.array, hint);
            }

            if (geometry.verticesNeedUpdate && position != null)
            {
                _gl.bindBuffer(_gl.ARRAY_BUFFER, position.buffer);
                _gl.bufferData(_gl.ARRAY_BUFFER, position.array, hint);
            }

            if (geometry.normalsNeedUpdate && normal != null)
            {
                _gl.bindBuffer(_gl.ARRAY_BUFFER, normal.buffer);
                _gl.bufferData(_gl.ARRAY_BUFFER, normal.array, hint);
            }

            if (geometry.uvsNeedUpdate && uv != null)
            {
                _gl.bindBuffer(_gl.ARRAY_BUFFER, uv.buffer);
                _gl.bufferData(_gl.ARRAY_BUFFER, uv.array, hint);
            }

            if (geometry.colorsNeedUpdate && color != null)
            {
                _gl.bindBuffer(_gl.ARRAY_BUFFER, color.buffer);
                _gl.bufferData(_gl.ARRAY_BUFFER, color.array, hint);
            }

            if (geometry.tangentsNeedUpdate && tangent != null)
            {
                _gl.bindBuffer(_gl.ARRAY_BUFFER, tangent.buffer);
                _gl.bufferData(_gl.ARRAY_BUFFER, tangent.array, hint);
            }

            if (dispose)
            {
                foreach (var i in geometry.attributes)
                {
                }
            }
        }

        public void renderBufferImmediate(dynamic obj, dynamic program, dynamic material)
        {
            if (obj.hasPositions && !obj.__webglVertexBuffer)
            {
                obj.__webglVertexBuffer = _gl.createBuffer();
            }
            if (obj.hasNormals && !obj.__webglNormalBuffer)
            {
                obj.__webglNormalBuffer = _gl.createBuffer();
            }
            if (obj.hasUvs && !obj.__webglUvBuffer)
            {
                obj.__webglUvBuffer = _gl.createBuffer();
            }
            if (obj.hasColors && !obj.__webglColorBuffer)
            {
                obj.__webglColorBuffer = _gl.createBuffer();
            }

            if (obj.hasPositions)
            {
                _gl.bindBuffer(_gl.ARRAY_BUFFER, obj.__webglVertexBuffer);
                _gl.bufferData(_gl.ARRAY_BUFFER, obj.positionArray, _gl.DYNAMIC_DRAW);
                _gl.enableVertexAttribArray(program.attributes.position);
                _gl.vertexAttribPointer(program.attributes.position, 3, _gl.FLOAT, false, 0, 0);
            }

            if (obj.hasNormals)
            {
                _gl.bindBuffer(_gl.ARRAY_BUFFER, obj.__webglNormalBuffer);

                if (material.shading == THREE.FlatShading)
                {
                    dynamic nx;
                    dynamic ny;
                    dynamic nz;
                    dynamic nax;
                    dynamic nbx;
                    dynamic ncx;
                    dynamic nay;
                    dynamic nby;
                    dynamic ncy;
                    dynamic naz;
                    dynamic nbz;
                    dynamic ncz;
                    dynamic normalArray;
                    dynamic i;
                    var il = obj.count * 3;

                    for (i = 0; i < il; i += 9)
                    {
                        normalArray = obj.normalArray;

                        nax = normalArray[i];
                        nay = normalArray[i + 1];
                        naz = normalArray[i + 2];

                        nbx = normalArray[i + 3];
                        nby = normalArray[i + 4];
                        nbz = normalArray[i + 5];

                        ncx = normalArray[i + 6];
                        ncy = normalArray[i + 7];
                        ncz = normalArray[i + 8];

                        nx = (nax + nbx + ncx) / 3;
                        ny = (nay + nby + ncy) / 3;
                        nz = (naz + nbz + ncz) / 3;

                        normalArray[i] = nx;
                        normalArray[i + 1] = ny;
                        normalArray[i + 2] = nz;

                        normalArray[i + 3] = nx;
                        normalArray[i + 4] = ny;
                        normalArray[i + 5] = nz;

                        normalArray[i + 6] = nx;
                        normalArray[i + 7] = ny;
                        normalArray[i + 8] = nz;
                    }
                }

                _gl.bufferData(_gl.ARRAY_BUFFER, obj.normalArray, _gl.DYNAMIC_DRAW);
                _gl.enableVertexAttribArray(program.attributes.normal);
                _gl.vertexAttribPointer(program.attributes.normal, 3, _gl.FLOAT, false, 0, 0);
            }

            if (obj.hasUvs && material.map)
            {
                _gl.bindBuffer(_gl.ARRAY_BUFFER, obj.__webglUvBuffer);
                _gl.bufferData(_gl.ARRAY_BUFFER, obj.uvArray, _gl.DYNAMIC_DRAW);
                _gl.enableVertexAttribArray(program.attributes.uv);
                _gl.vertexAttribPointer(program.attributes.uv, 2, _gl.FLOAT, false, 0, 0);
            }

            if (obj.hasColors && JSObject.eval(material.vertexColors))
            {
                _gl.bindBuffer(_gl.ARRAY_BUFFER, obj.__webglColorBuffer);
                _gl.bufferData(_gl.ARRAY_BUFFER, obj.colorArray, _gl.DYNAMIC_DRAW);
                _gl.enableVertexAttribArray(program.attributes.color);
                _gl.vertexAttribPointer(program.attributes.color, 3, _gl.FLOAT, false, 0, 0);
            }

            _gl.drawArrays(_gl.TRIANGLES, 0, obj.count);

            obj.count = 0;
        }

        public void renderBufferDirect(Camera camera, List<Light> lights, dynamic fog, dynamic material, dynamic geometry, dynamic obj)
        {
            if (material.visible == false)
            {
                return;
            }

            dynamic position = null;

            var program = setProgram(camera, lights, fog, material, obj);

            var attributes = program.attributes;

            var updateBuffers = false;
            var wireframeBit = JSObject.safe<bool>(material.wireframe) ? 1 : 0;
            var geometryHash = (geometry.id * 0xffffff) + (program.id * 2) + wireframeBit;

            if (geometryHash != _currentGeometryGroupHash)
            {
                _currentGeometryGroupHash = geometryHash;
                updateBuffers = true;
            }

            if (updateBuffers)
            {
                disableAttributes();
            }

            if (obj is Mesh)
            {
                var index = geometry.attributes["index"];

                if (index != null)
                {
                    var offsets = geometry.offsets;

                    if (offsets.length > 1)
                    {
                        updateBuffers = true;
                    }

                    for (int i = 0, il = offsets.length; i < il; i++)
                    {
                        var startIndex = offsets[i].index;

                        if (updateBuffers)
                        {
                            position = geometry.attributes["position"];
                            var positionSize = position.itemSize;

                            _gl.bindBuffer(_gl.ARRAY_BUFFER, position.buffer);
                            enableAttribute(attributes.position);
                            _gl.vertexAttribPointer((uint)attributes.position, positionSize, _gl.FLOAT, false, 0, startIndex * positionSize * 4);

                            var normal = geometry.attributes["normal"];

                            if (attributes.normal >= 0 && normal != null)
                            {
                                var normalSize = normal.itemSize;

                                _gl.bindBuffer(_gl.ARRAY_BUFFER, normal.buffer);
                                enableAttribute(attributes.normal);
                                _gl.vertexAttribPointer((uint)attributes.normal, normalSize, _gl.FLOAT, false, 0, startIndex * normalSize * 4);
                            }

                            var uv = geometry.attributes["uv"];

                            if (attributes.uv >= 0 && uv != null)
                            {
                                var uvSize = uv.itemSize;

                                _gl.bindBuffer(_gl.ARRAY_BUFFER, uv.buffer);
                                enableAttribute(attributes.uv);
                                _gl.vertexAttribPointer((uint)attributes.uv, uvSize, _gl.FLOAT, false, 0, startIndex * uvSize * 4);
                            }

                            var color = geometry.attributes["color"];

                            if (attributes.color >= 0 && color != null)
                            {
                                var colorSize = color.itemSize;

                                _gl.bindBuffer(_gl.ARRAY_BUFFER, color.buffer);
                                enableAttribute(attributes.color);
                                _gl.vertexAttribPointer((uint)attributes.color, colorSize, _gl.FLOAT, false, 0, startIndex * colorSize * 4);
                            }

                            var tangent = geometry.attributes["tangent"];

                            if (attributes.tangent >= 0 && tangent)
                            {
                                var tangentSize = tangent.itemSize;

                                _gl.bindBuffer(_gl.ARRAY_BUFFER, tangent.buffer);
                                enableAttribute(attributes.tangent);
                                _gl.vertexAttribPointer(attributes.tangent, tangentSize, _gl.FLOAT, false, 0, startIndex * tangentSize * 4);
                            }

                            _gl.bindBuffer(_gl.ELEMENT_ARRAY_BUFFER, index.buffer);
                        }

                        _gl.drawElements(_gl.TRIANGLES, offsets[i].count, _gl.UNSIGNED_SHORT, offsets[i].start * 2);

                        info.render.calls++;
                        info.render.vertices += offsets[i].count;
                        info.render.faces += offsets[i].count / 3;
                    }
                }
                else
                {
                    if (updateBuffers)
                    {
                        position = geometry.attributes["position"];
                        var positionSize = position.itemSize;

                        _gl.bindBuffer(_gl.ARRAY_BUFFER, position.buffer);
                        enableAttribute(attributes.position);
                        _gl.vertexAttribPointer(attributes.position, positionSize, _gl.FLOAT, false, 0, 0);

                        var normal = geometry.attributes["normal"];

                        if (attributes.normal >= 0 && normal)
                        {
                            var normalSize = normal.itemSize;

                            _gl.bindBuffer(_gl.ARRAY_BUFFER, normal.buffer);
                            enableAttribute(attributes.normal);
                            _gl.vertexAttribPointer(attributes.normal, normalSize, _gl.FLOAT, false, 0, 0);
                        }

                        var uv = geometry.attributes["uv"];

                        if (attributes.uv >= 0 && uv)
                        {
                            var uvSize = uv.itemSize;

                            _gl.bindBuffer(_gl.ARRAY_BUFFER, uv.buffer);
                            enableAttribute(attributes.uv);
                            _gl.vertexAttribPointer(attributes.uv, uvSize, _gl.FLOAT, false, 0, 0);
                        }

                        var color = geometry.attributes["color"];

                        if (attributes.color >= 0 && color)
                        {
                            var colorSize = color.itemSize;

                            _gl.bindBuffer(_gl.ARRAY_BUFFER, color.buffer);
                            enableAttribute(attributes.color);
                            _gl.vertexAttribPointer(attributes.color, colorSize, _gl.FLOAT, false, 0, 0);
                        }

                        var tangent = geometry.attributes["tangent"];

                        if (attributes.tangent >= 0 && tangent)
                        {
                            var tangentSize = tangent.itemSize;

                            _gl.bindBuffer(_gl.ARRAY_BUFFER, tangent.buffer);
                            enableAttribute(attributes.tangent);
                            _gl.vertexAttribPointer(attributes.tangent, tangentSize, _gl.FLOAT, false, 0, 0);
                        }
                    }

                    _gl.drawArrays(_gl.TRIANGLES, 0, position.numItems / 3);

                    info.render.calls++;
                    info.render.vertices += position.numItems / 3;
                    info.render.faces += position.numItems / 3 / 3;
                }
            }
            else if (obj is ParticleSystem)
            {
                if (updateBuffers)
                {
                    position = geometry.attributes["position"];
                    var positionSize = position.itemSize;

                    _gl.bindBuffer(_gl.ARRAY_BUFFER, position.buffer);
                    enableAttribute(attributes.position);
                    _gl.vertexAttribPointer((uint)attributes.position, positionSize, _gl.FLOAT, false, 0, 0);

                    var color = geometry.attributes["color"];

                    if (attributes.color >= 0 && color != null)
                    {
                        var colorSize = color.itemSize;

                        _gl.bindBuffer(_gl.ARRAY_BUFFER, color.buffer);
                        enableAttribute(attributes.color);
                        _gl.vertexAttribPointer((uint)attributes.color, colorSize, _gl.FLOAT, false, 0, 0);
                    }

                    _gl.drawArrays(_gl.POINTS, 0, position.numItems / 3);

                    info.render.calls++;
                    info.render.points += position.numItems / 3;
                }
            }
            else if (obj is Line)
            {
                if (updateBuffers)
                {
                    position = geometry.attributes["position"];
                    var positionSize = position.itemSize;

                    _gl.bindBuffer(_gl.ARRAY_BUFFER, position.buffer);
                    enableAttribute(attributes.position);
                    _gl.vertexAttribPointer((uint)attributes.position, positionSize, _gl.FLOAT, false, 0, 0);

                    var color = geometry.attributes["color"];

                    if (attributes.color >= 0 && color != null)
                    {
                        var colorSize = color.itemSize;

                        _gl.bindBuffer(_gl.ARRAY_BUFFER, color.buffer);
                        enableAttribute(attributes.color);
                        _gl.vertexAttribPointer((uint)attributes.color, colorSize, _gl.FLOAT, false, 0, 0);
                    }

                    _gl.drawArrays(_gl.LINE_STRIP, 0, position.numItems / 3);

                    info.render.calls++;
                    info.render.points += position.numItems;
                }
            }
        }

        public void renderBuffer(Camera camera, List<Light> lights, dynamic fog, dynamic material, dynamic geometryGroup, dynamic obj)
        {
            if (material.visible == false)
            {
                return;
            }

            var program = setProgram(camera, lights, fog, material, obj);
            var attributes = program.attributes;
            var updateBuffers = false;
            var wireframeBit = JSObject.safe<bool>(material.wireframe) ? 1 : 0;
            var geometryGroupHash = (geometryGroup.id * 0xffffff) + (program.id * 2) + wireframeBit;

            if (geometryGroupHash != _currentGeometryGroupHash)
            {
                _currentGeometryGroupHash = geometryGroupHash;
                updateBuffers = true;
            }

            if (updateBuffers)
            {
                disableAttributes();
            }

            if (!JSObject.eval(material.morphTargets) && attributes.position >= 0)
            {
                if (updateBuffers)
                {
                    _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglVertexBuffer);
                    enableAttribute(attributes.position);
                    _gl.vertexAttribPointer((uint)attributes.position, 3, _gl.FLOAT, false, 0, 0);
                }
            }
            else
            {
                if (obj.morphTargetBase != null)
                {
                    setupMorphTargets(material, geometryGroup, obj);
                }
            }

            if (updateBuffers)
            {
                if (geometryGroup.__webglCustomAttributesList != null)
                {
                    int i;
                    int il;
                    for (i = 0, il = geometryGroup.__webglCustomAttributesList.length; i < il; i++)
                    {
                        var attribute = geometryGroup.__webglCustomAttributesList[i];

                        if (attributes[attribute.buffer.belongsToAttribute] >= 0)
                        {
                            _gl.bindBuffer(_gl.ARRAY_BUFFER, attribute.buffer);
                            enableAttribute(attributes[attribute.buffer.belongsToAttribute]);
                            _gl.vertexAttribPointer((uint)attributes[attribute.buffer.belongsToAttribute], attribute.size, _gl.FLOAT, false, 0, 0);
                        }
                    }
                }

                if (attributes.color >= 0)
                {
                    _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglColorBuffer);
                    enableAttribute(attributes.color);
                    _gl.vertexAttribPointer((uint)attributes.color, 3, _gl.FLOAT, false, 0, 0);
                }

                if (attributes.normal >= 0)
                {
                    _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglNormalBuffer);
                    enableAttribute(attributes.normal);
                    _gl.vertexAttribPointer((uint)attributes.normal, 3, _gl.FLOAT, false, 0, 0);
                }

                if (attributes.tangent >= 0)
                {
                    _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglTangentBuffer);
                    enableAttribute(attributes.tangent);
                    _gl.vertexAttribPointer(attributes.tangent, 4, _gl.FLOAT, false, 0, 0);
                }

                if (attributes.uv >= 0)
                {
                    _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglUVBuffer);
                    enableAttribute(attributes.uv);
                    _gl.vertexAttribPointer((uint)attributes.uv, 2, _gl.FLOAT, false, 0, 0);
                }

                if (attributes.uv2 >= 0)
                {
                    _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglUV2Buffer);
                    enableAttribute(attributes.uv2);
                    _gl.vertexAttribPointer((uint)attributes.uv2, 2, _gl.FLOAT, false, 0, 0);
                }

                if (JSObject.safe<bool>(material.skinning) && attributes.skinIndex >= 0 && attributes.skinWeight >= 0)
                {
                    _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglSkinIndicesBuffer);
                    enableAttribute(attributes.skinIndex);
                    _gl.vertexAttribPointer((uint)attributes.skinIndex, 4, _gl.FLOAT, false, 0, 0);

                    _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglSkinWeightsBuffer);
                    enableAttribute(attributes.skinWeight);
                    _gl.vertexAttribPointer((uint)attributes.skinWeight, 4, _gl.FLOAT, false, 0, 0);
                }

                if (attributes.lineDistance >= 0)
                {
                    _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglLineDistanceBuffer);
                    enableAttribute(attributes.lineDistance);
                    _gl.vertexAttribPointer((uint)attributes.lineDistance, 1, _gl.FLOAT, false, 0, 0);
                }
            }

            if (obj is Mesh)
            {
                if (JSObject.safe<bool>(material.wireframe))
                {
                    setLineWidth(material.wireframeLinewidth);

                    if (updateBuffers)
                    {
                        _gl.bindBuffer(_gl.ELEMENT_ARRAY_BUFFER, geometryGroup.__webglLineBuffer);
                    }
                    _gl.drawElements(_gl.LINES, geometryGroup.__webglLineCount, _gl.UNSIGNED_SHORT, 0);
                }
                else
                {
                    if (updateBuffers)
                    {
                        _gl.bindBuffer(_gl.ELEMENT_ARRAY_BUFFER, geometryGroup.__webglFaceBuffer);
                    }
                    _gl.drawElements(_gl.TRIANGLES, geometryGroup.__webglFaceCount, _gl.UNSIGNED_SHORT, 0);
                }

                info.render.calls++;
                info.render.vertices += geometryGroup.__webglFaceCount;
                info.render.faces += geometryGroup.__webglFaceCount / 3;
            }
            else if (obj is Line)
            {
                var primitives = (obj.type == THREE.LineStrip) ? _gl.LINE_STRIP : _gl.LINES;

                setLineWidth(material.linewidth);

                _gl.drawArrays(primitives, 0, geometryGroup.__webglLineCount);

                info.render.calls++;
            }
            else if (obj is ParticleSystem)
            {
                _gl.drawArrays(_gl.POINTS, 0, geometryGroup.__webglParticleCount);

                info.render.calls++;
                info.render.points += geometryGroup.__webglParticleCount;
            }
            else if (obj is Ribbon)
            {
                _gl.drawArrays(_gl.TRIANGLE_STRIP, 0, geometryGroup.__webglVertexCount);

                info.render.calls++;
            }
        }

        private void enableAttribute(int attribute)
        {
            if (!_enabledAttributes[attribute])
            {
                _gl.enableVertexAttribArray((uint)attribute);
                _enabledAttributes[attribute] = true;
            }
        }

        private void disableAttributes()
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

        private void setupMorphTargets(dynamic material, dynamic geometryGroup, dynamic obj)
        {
            var attributes = material.program.attributes;

            if (obj.morphTargetBase != -1 && attributes.position >= 0)
            {
                _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglMorphTargetsBuffers[obj.morphTargetBase]);
                enableAttribute(attributes.position);
                _gl.vertexAttribPointer(attributes.position, 3, _gl.FLOAT, false, 0, 0);
            }
            else if (attributes.position >= 0)
            {
                _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglVertexBuffer);
                enableAttribute(attributes.position);
                _gl.vertexAttribPointer((uint)attributes.position, 3, _gl.FLOAT, false, 0, 0);
            }

            if (obj.morphTargetForcedOrder.length > 0)
            {
                var m = 0;
                var order = obj.morphTargetForcedOrder;
                var influences = obj.morphTargetInfluences;

                while (m < material.numSupportedMorphTargets && m < order.length)
                {
                    if (attributes["morphTarget" + m] >= 0)
                    {
                        _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglMorphTargetsBuffers[order[m]]);
                        enableAttribute(attributes["morphTarget" + m]);
                        _gl.vertexAttribPointer(attributes["morphTarget" + m], 3, _gl.FLOAT, false, 0, 0);
                    }

                    if (attributes["morphNormal" + m] >= 0 && material.morphNormals)
                    {
                        _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglMorphNormalsBuffers[order[m]]);
                        enableAttribute(attributes["morphNormal" + m]);
                        _gl.vertexAttribPointer(attributes["morphNormal" + m], 3, _gl.FLOAT, false, 0, 0);
                    }

                    obj.__webglMorphTargetInfluences[m] = influences[order[m]];

                    m++;
                }
            }
            else
            {
                var activeInfluenceIndices = new JSArray();
                var influences = obj.morphTargetInfluences;
                int i, il = influences.length;

                for (i = 0; i < il; i++)
                {
                    var influence = influences[i];

                    if (influence > 0)
                    {
                        activeInfluenceIndices.push(new JSArray(influence, i));
                    }
                }

                if (activeInfluenceIndices.length > material.numSupportedMorphTargets)
                {
                    activeInfluenceIndices.sort(numericalSort);
                    activeInfluenceIndices.length = material.numSupportedMorphTargets;
                }
                else if (activeInfluenceIndices.length > material.numSupportedMorphNormals)
                {
                    activeInfluenceIndices.sort(numericalSort);
                }
                else if (activeInfluenceIndices.length == 0)
                {
                    activeInfluenceIndices.push(new JSArray(0, 0));
                }

                var m = 0;

                while (m < material.numSupportedMorphTargets)
                {
                    if (activeInfluenceIndices[m] != null)
                    {
                        var influenceIndex = activeInfluenceIndices[m][1];

                        if (attributes["morphTarget" + m] >= 0)
                        {
                            _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglMorphTargetsBuffers[influenceIndex]);
                            enableAttribute(attributes["morphTarget" + m]);
                            _gl.vertexAttribPointer((uint)attributes["morphTarget" + m], 3, _gl.FLOAT, false, 0, 0);
                        }

                        if (attributes["morphNormal" + m] >= 0 && material.morphNormals)
                        {
                            _gl.bindBuffer(_gl.ARRAY_BUFFER, geometryGroup.__webglMorphNormalsBuffers[influenceIndex]);
                            enableAttribute(attributes["morphNormal" + m]);
                            _gl.vertexAttribPointer((uint)attributes["morphNormal" + m], 3, _gl.FLOAT, false, 0, 0);
                        }

                        obj.__webglMorphTargetInfluences[m] = (float)influences[influenceIndex];
                    }
                    else
                    {
                        obj.__webglMorphTargetInfluences[m] = 0;
                    }

                    m++;
                }
            }

            if (material.program.uniforms.morphTargetInfluences != null)
            {
                _gl.uniform1fv(material.program.uniforms.morphTargetInfluences, obj.__webglMorphTargetInfluences);
            }
        }

        private int painterSortStable(dynamic a, dynamic b)
        {
            var az = JSObject.safe<double>(a.z);
            var bz = JSObject.safe<double>(b.z);
            if (!(double.IsNaN(az) || double.IsNaN(bz)) && az != bz)
            {
                var result = (bz - az);
                return result < 0 ? -1 : (result > 0 ? 1 : 0);
            }
            else
            {
                return JSObject.safe<int>(b.id) - JSObject.safe<int>(a.id);
            }
        }

        private int numericalSort(dynamic a, dynamic b)
        {
            var result = b[0] - a[0];
            return result < 0 ? -1 : (result > 0 ? 1 : 0);
        }

        public void render(Scene scene, Camera camera, WebGLRenderTarget renderTarget = null, bool forceClear = false)
        {
            _currentMaterialId = -1;
            _lightsNeedUpdate = true;

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
                initWebGLObjects(scene);
            }

            renderPlugins(renderPluginsPre, scene, camera);

            info.render.calls = 0;
            info.render.vertices = 0;
            info.render.faces = 0;
            info.render.points = 0;

            setRenderTarget(renderTarget);

            if (JSObject.safe<bool>(autoClear) || JSObject.safe<bool>(forceClear))
            {
                clear(autoClearColor, autoClearDepth, autoClearStencil);
            }

            var renderList = scene.__webglObjects;

            for (int i = 0, il = renderList.length; i < il; i++)
            {
                var webglObject = renderList[i];
                var obj = webglObject.obj;

                webglObject.render = false;

                if (obj.visible)
                {
                    if (!(obj is Mesh || obj is ParticleSystem) || !(obj.frustumCulled) || _frustum.intersectsObject(obj))
                    {
                        setupMatrices(obj, camera);

                        unrollBufferMaterial(webglObject);

                        webglObject.render = true;

                        if (sortObjects)
                        {
                            if (obj.renderDepth != null)
                            {
                                webglObject.z = obj.renderDepth;
                            }
                            else
                            {
                                _vector3.copy(obj.matrixWorld.getPosition());
                                _vector3.applyProjection(_projScreenMatrix);

                                webglObject.z = _vector3.z;
                            }

                            webglObject.id = obj.id;
                        }
                    }
                }
            }

//			if (sortObjects)
//			{
//				renderList.sort(painterSortStable);
//			}

            renderList = scene.__webglObjectsImmediate;

            for (int i = 0, il = renderList.length; i < il; i++)
            {
                var webglObject = renderList[i];
                var obj = webglObject.obj;

                if (obj.visible)
                {
                    setupMatrices(obj, camera);

                    unrollImmediateBufferMaterial(webglObject);
                }
            }

            if (scene.overrideMaterial != null)
            {
                var material = scene.overrideMaterial;

                setBlending(material.blending, material.blendEquation, material.blendSrc, material.blendDst);
                setDepthTest(material.depthTest);
                setDepthWrite(material.depthWrite);
                setPolygonOffset(material.polygonOffset, material.polygonOffsetFactor, material.polygonOffsetUnits);

                renderObjects(scene.__webglObjects, false, "", camera, scene.__lights, scene.fog, true, material);
                renderObjectsImmediate(scene.__webglObjectsImmediate, "", camera, scene.__lights, scene.fog, false, material);
            }
            else
            {
                Material material = null;

                setBlending(THREE.NoBlending);

                renderObjects(scene.__webglObjects, true, "opaque", camera, scene.__lights, scene.fog, false, material);
                renderObjectsImmediate(scene.__webglObjectsImmediate, "opaque", camera, scene.__lights, scene.fog, false, material);

                renderObjects(scene.__webglObjects, false, "transparent", camera, scene.__lights, scene.fog, true, material);
                renderObjectsImmediate(scene.__webglObjectsImmediate, "transparent", camera, scene.__lights, scene.fog, true, material);
            }

            renderPlugins(renderPluginsPost, scene, camera);

            if (renderTarget != null && renderTarget.generateMipmaps && renderTarget.minFilter != THREE.NearestFilter && renderTarget.minFilter != THREE.LinearFilter)
            {
                updateRenderTargetMipmap(renderTarget);
            }

            setDepthTest(true);
            setDepthWrite(true);
        }

        private void renderPlugins(List<Plugin> plugins, Scene scene, Camera camera)
        {
            if (plugins.Count == 0)
            {
                return;
            }

            for (var i = 0; i < plugins.Count; i++)
            {
                _currentProgram = null;
                _currentCamera = null;

                _oldBlending = null;
                _oldDepthTest = null;
                _oldDepthWrite = null;
                _oldDoubleSided = null;
                _oldFlipSided = null;
                _currentGeometryGroupHash = -1;
                _currentMaterialId = -1;

                _lightsNeedUpdate = true;

                plugins[i].render(scene, camera, _currentWidth, _currentHeight);

                _currentProgram = null;
                _currentCamera = null;

                _oldBlending = -1;
                _oldDepthTest = null;
                _oldDepthWrite = null;
                _oldDoubleSided = null;
                _oldFlipSided = null;
                _currentGeometryGroupHash = -1;
                _currentMaterialId = -1;

                _lightsNeedUpdate = true;
            }
        }

        private void renderObjects(dynamic renderList, bool reverse, String materialType, Camera camera, List<Light> lights, dynamic fog, bool useBlending, Material overrideMaterial)
        {
            int start;
            int end;
            int delta;

            if (reverse)
            {
                start = renderList.length - 1;
                end = -1;
                delta = -1;
            }
            else
            {
                start = 0;
                end = renderList.length;
                delta = 1;
            }

            for (var i = start; i != end; i += delta)
            {
                var webglObject = renderList[i];

                if (webglObject.render)
                {
                    var obj = webglObject.obj;
                    var buffer = webglObject.buffer;

                    Material material;
                    if (overrideMaterial != null)
                    {
                        material = overrideMaterial;
                    }
                    else
                    {
                        material = webglObject[materialType];

                        if (!JSObject.eval(material))
                        {
                            continue;
                        }

                        if (useBlending)
                        {
                            setBlending(material.blending, material.blendEquation, material.blendSrc, material.blendDst);
                        }

                        setDepthTest(material.depthTest);
                        setDepthWrite(material.depthWrite);
                        setPolygonOffset(material.polygonOffset, material.polygonOffsetFactor, material.polygonOffsetUnits);
                    }

                    setMaterialFaces(material);

                    if (buffer is BufferGeometry)
                    {
                        renderBufferDirect(camera, lights, fog, material, buffer, obj);
                    }
                    else
                    {
                        this.renderBuffer(camera, lights, fog, material, buffer, obj);
                    }
                }
            }
        }

        private void renderObjectsImmediate(dynamic renderList, String materialType, Camera camera, List<Light> lights, dynamic fog, bool useBlending, Material overrideMaterial)
        {
            for (int i = 0, il = renderList.length; i < il; i++)
            {
                var webglObject = renderList[i];
                var obj = webglObject.obj;

                if (obj.visible)
                {
                    Material material;
                    if (overrideMaterial != null)
                    {
                        material = overrideMaterial;
                    }
                    else
                    {
                        material = webglObject[materialType];

                        if ((!(JSObject.eval(material))))
                        {
                            continue;
                        }

                        if (useBlending)
                        {
                            setBlending(material.blending, material.blendEquation, material.blendSrc, material.blendDst);
                        }

                        setDepthTest(material.depthTest);
                        setDepthWrite(material.depthWrite);
                        setPolygonOffset(material.polygonOffset, material.polygonOffsetFactor, material.polygonOffsetUnits);
                    }

                    this.renderImmediateObject(camera, lights, fog, material, obj);
                }
            }
        }

        public void renderImmediateObject(Camera camera, List<Light> lights, dynamic fog, Material material, dynamic obj)
        {
            var program = setProgram(camera, lights, fog, material, obj);

            _currentGeometryGroupHash = -1;

            setMaterialFaces(material);

            if (obj.immediateRenderCallback)
            {
                obj.immediateRenderCallback(program, _gl, _frustum);
            }
            else
            {
            }
        }

        private void unrollImmediateBufferMaterial(dynamic globject)
        {
            var obj = globject.obj;
            var material = obj.material;

            if (material.transparent)
            {
                globject.transparent = material;
                globject.opaque = null;
            }
            else
            {
                globject.opaque = material;
                globject.transparent = null;
            }
        }

        private void unrollBufferMaterial(dynamic globject)
        {
            var obj = globject.obj;
            var buffer = globject.buffer;

            var meshMaterial = obj.material;

            if (meshMaterial is MeshFaceMaterial)
            {
                var materialIndex = buffer.materialIndex;

                var material = meshMaterial.materials[materialIndex];

                if (material.transparent)
                {
                    globject.transparent = material;
                    globject.opaque = null;
                }
                else
                {
                    globject.opaque = material;
                    globject.transparent = null;
                }
            }
            else
            {
                var material = meshMaterial;

                if (material != null)
                {
                    if (material.transparent)
                    {
                        globject.transparent = material;
                        globject.opaque = null;
                    }
                    else
                    {
                        globject.opaque = material;
                        globject.transparent = null;
                    }
                }
            }
        }

        private void sortFacesByMaterial(dynamic geometry, Material material)
        {
            var hash_map = new JSObject();

            var numMorphTargets = geometry.morphTargets.length;
            var numMorphNormals = geometry.morphNormals.length;

            var usesFaceMaterial = material is MeshFaceMaterial;

            geometry.geometryGroups = new JSObject();

            for (int f = 0, fl = geometry.faces.length; f < fl; f++)
            {
                var face = geometry.faces[f];
                var materialIndex = usesFaceMaterial ? face.materialIndex : 0;

                if (hash_map[materialIndex] == null)
                {
                    hash_map[materialIndex] = JSObject.create(new {hash = materialIndex, counter = 0});
                }

                var groupHash = hash_map[materialIndex].hash + '_' + hash_map[materialIndex].counter;

                if (geometry.geometryGroups[groupHash] == null)
                {
                    geometry.geometryGroups[groupHash] = JSObject.create(new
                                                                         {
                                                                             faces3 = new JSArray(),
                                                                             faces4 = new JSArray(),
                                                                             materialIndex,
                                                                             vertices = 0,
                                                                             numMorphTargets,
                                                                             numMorphNormals
                                                                         });
                }

                var vertices = face is Face3 ? 3 : 4;

                if (geometry.geometryGroups[groupHash].vertices + vertices > 65535)
                {
                    hash_map[materialIndex].counter += 1;
                    groupHash = hash_map[materialIndex].hash + '_' + hash_map[materialIndex].counter;

                    if (geometry.geometryGroups[groupHash] == null)
                    {
                        geometry.geometryGroups[groupHash] = JSObject.create(new
                                                                             {
                                                                                 faces3 = new JSArray(),
                                                                                 faces4 = new JSArray(),
                                                                                 materialIndex,
                                                                                 vertices = 0,
                                                                                 numMorphTargets,
                                                                                 numMorphNormals
                                                                             });
                    }
                }

                if (face is Face3)
                {
                    geometry.geometryGroups[groupHash].faces3.push(f);
                }
                else
                {
                    geometry.geometryGroups[groupHash].faces4.push(f);
                }

                geometry.geometryGroups[groupHash].vertices += vertices;
            }

            geometry.geometryGroupsList = new JSArray();

            foreach (var g in geometry.geometryGroups)
            {
                geometry.geometryGroups[g].id = _geometryGroupCounter++;

                geometry.geometryGroupsList.push(geometry.geometryGroups[g]);
            }
        }

        public void initWebGLObjects(Scene scene)
        {
            if (!JSObject.eval(scene.__webglObjects))
            {
                scene.__webglObjects = new JSArray();
                scene.__webglObjectsImmediate = new JSArray();
                scene.__webglSprites = new JSArray();
                scene.__webglFlares = new JSArray();
            }

            while (scene.__objectsAdded.Count > 0)
            {
                addObject(scene.__objectsAdded[0], scene);
                scene.__objectsAdded.RemoveAt(0);
            }

            while (scene.__objectsRemoved.Count > 0)
            {
                removeObject(scene.__objectsRemoved[0], scene);
                scene.__objectsRemoved.RemoveAt(0);
            }

            var o = 0;
            int ol;
            for (ol = scene.__webglObjects.length; o < ol; o++)
            {
                updateObject(scene.__webglObjects[o].obj);
            }
        }

        private void addObject(Object3D obj, Scene scene)
        {
            if (!(JSObject.eval(obj.__webglInit)))
            {
                obj.__webglInit = true;

                obj._modelViewMatrix = new Matrix4();
                obj._normalMatrix = new Matrix3();

                if (obj.geometry != null && JSObject.safe<bool>(obj.geometry.__webglInit))
                {
                    obj.geometry.__webglInit = true;
                    obj.geometry.addEventListener("dispose", onGeometryDispose);
                }

                if (obj is Mesh)
                {
                    var geometry = obj.geometry;
                    var material = obj.material;

                    if (geometry is Geometry)
                    {
                        if (geometry.geometryGroups == null)
                        {
                            sortFacesByMaterial(geometry, material);
                        }

                        foreach (var g in geometry.geometryGroups)
                        {
                            var geometryGroup = geometry.geometryGroups[g];

                            if (!JSObject.eval(geometryGroup.__webglVertexBuffer))
                            {
                                createMeshBuffers(geometryGroup);
                                initMeshBuffers(geometryGroup, obj);

                                geometry.verticesNeedUpdate = true;
                                geometry.morphTargetsNeedUpdate = true;
                                geometry.elementsNeedUpdate = true;
                                geometry.uvsNeedUpdate = true;
                                geometry.normalsNeedUpdate = true;
                                geometry.tangentsNeedUpdate = true;
                                geometry.colorsNeedUpdate = true;
                            }
                        }
                    }
                    else if (geometry is BufferGeometry)
                    {
                        initDirectBuffers(geometry);
                    }
                }
                else if (obj is Ribbon)
                {
                    var geometry = obj.geometry;

                    if (geometry != null && !JSObject.eval(geometry.__webglVertexBuffer))
                    {
                        createRibbonBuffers(geometry);
                        initRibbonBuffers(geometry, obj);

                        geometry.verticesNeedUpdate = true;
                        geometry.colorsNeedUpdate = true;
                        geometry.normalsNeedUpdate = true;
                    }
                }
                else if (obj is Line)
                {
                    var geometry = obj.geometry;

                    if (geometry != null && !JSObject.eval(geometry.__webglVertexBuffer))
                    {
                        if (geometry is Geometry)
                        {
                            createLineBuffers(geometry);
                            initLineBuffers(geometry, obj);

                            geometry.verticesNeedUpdate = true;
                            geometry.colorsNeedUpdate = true;
                            geometry.lineDistancesNeedUpdate = true;
                        }
                        else if (geometry is BufferGeometry)
                        {
                            initDirectBuffers(geometry);
                        }
                    }
                }
                else if (obj is ParticleSystem)
                {
                    var geometry = obj.geometry;

                    if (geometry != null && !JSObject.eval(geometry.__webglVertexBuffer))
                    {
                        if (geometry is Geometry)
                        {
                            createParticleBuffers(geometry);
                            initParticleBuffers(geometry, obj);

                            geometry.verticesNeedUpdate = true;
                            geometry.colorsNeedUpdate = true;
                        }
                        else if (geometry is BufferGeometry)
                        {
                            initDirectBuffers(geometry);
                        }
                    }
                }
            }

            if (!(JSObject.eval(obj.__webglActive)))
            {
                if (obj is Mesh)
                {
                    var geometry = obj.geometry;

                    if (geometry is BufferGeometry)
                    {
                        addBuffer(scene.__webglObjects, geometry, obj);
                    }
                    else if (geometry is Geometry)
                    {
                        foreach (var g in geometry.geometryGroups)
                        {
                            var geometryGroup = geometry.geometryGroups[g];

                            addBuffer(scene.__webglObjects, geometryGroup, obj);
                        }
                    }
                }
                else if (obj is Ribbon || obj is Line || obj is ParticleSystem)
                {
                    addBuffer(scene.__webglObjects, obj.geometry, obj);
                }
                else if (obj is ImmediateRenderObject || JSObject.safe<bool>(obj.immediateRenderCallback))
                {
                    addBufferImmediate(scene.__webglObjectsImmediate, obj);
                }
                else if (obj is Sprite)
                {
                    scene.__webglSprites.push(obj);
                }
                else if (obj is LensFlare)
                {
                    scene.__webglFlares.push(obj);
                }

                obj.__webglActive = true;
            }
        }

        private void addBuffer(JSArray objlist, dynamic buffer, Object3D obj)
        {
            dynamic exo = new JSObject();
            exo.buffer = buffer;
            exo.obj = obj;
            exo.opaque = null;
            exo.transparent = null;
            objlist.push(exo);
        }

        private void addBufferImmediate(dynamic objlist, dynamic obj)
        {
            objlist.push(JSObject.create(new
                                         {
                                             obj,
                                             opaque = (object)null,
                                             transparent = (object)null
                                         }));
        }

        private void updateObject(Object3D obj)
        {
            var geometry = obj.geometry;

            dynamic material = null;
            if (obj is Mesh)
            {
                if (geometry is BufferGeometry)
                {
                    if (JSObject.safe<bool>(geometry.verticesNeedUpdate) ||
                        JSObject.safe<bool>(geometry.elementsNeedUpdate) ||
                        JSObject.safe<bool>(geometry.uvsNeedUpdate) ||
                        JSObject.safe<bool>(geometry.normalsNeedUpdate) ||
                        JSObject.safe<bool>(geometry.colorsNeedUpdate) ||
                        JSObject.safe<bool>(geometry.tangentsNeedUpdate))
                    {
                        setDirectBuffers(geometry, _gl.DYNAMIC_DRAW, !geometry.dynamic);
                    }

                    geometry.verticesNeedUpdate = false;
                    geometry.elementsNeedUpdate = false;
                    geometry.uvsNeedUpdate = false;
                    geometry.normalsNeedUpdate = false;
                    geometry.colorsNeedUpdate = false;
                    geometry.tangentsNeedUpdate = false;
                }
                else
                {
                    for (int i = 0, il = geometry.geometryGroupsList.length; i < il; i++)
                    {
                        var geometryGroup = geometry.geometryGroupsList[i];

                        material = getBufferMaterial(obj, geometryGroup);

                        if (geometry.buffersNeedUpdate)
                        {
                            initMeshBuffers(geometryGroup, obj);
                        }

                        var customAttributesDirty = material.attributes != null && areCustomAttributesDirty(material);

                        if (geometry.verticesNeedUpdate || geometry.morphTargetsNeedUpdate || geometry.elementsNeedUpdate ||
                            geometry.uvsNeedUpdate || geometry.normalsNeedUpdate ||
                            geometry.colorsNeedUpdate || geometry.tangentsNeedUpdate || customAttributesDirty)
                        {
                            setMeshBuffers(geometryGroup, obj, _gl.DYNAMIC_DRAW, !geometry.dynamic, material);
                        }
                    }

                    geometry.verticesNeedUpdate = false;
                    geometry.morphTargetsNeedUpdate = false;
                    geometry.elementsNeedUpdate = false;
                    geometry.uvsNeedUpdate = false;
                    geometry.normalsNeedUpdate = false;
                    geometry.colorsNeedUpdate = false;
                    geometry.tangentsNeedUpdate = false;

                    geometry.buffersNeedUpdate = false;

                    if (material != null && material.attributes != null)
                    {
                        clearCustomAttributes(material);
                    }
                }
            }
            else
            {
                bool customAttributesDirty;
                if (obj is Ribbon)
                {
                    material = getBufferMaterial(obj, geometry);

                    customAttributesDirty = material.attributes != null && areCustomAttributesDirty(material);

                    if (geometry.verticesNeedUpdate || geometry.colorsNeedUpdate || geometry.normalsNeedUpdate || customAttributesDirty)
                    {
                        setRibbonBuffers(geometry, _gl.DYNAMIC_DRAW);
                    }

                    geometry.verticesNeedUpdate = false;
                    geometry.colorsNeedUpdate = false;
                    geometry.normalsNeedUpdate = false;

                    if (material.attributes != null)
                    {
                        clearCustomAttributes(material);
                    }
                }
                else if (obj is Line)
                {
                    if (geometry is BufferGeometry)
                    {
                        if (JSObject.safe<bool>(geometry.verticesNeedUpdate) || JSObject.safe<bool>(geometry.colorsNeedUpdate))
                        {
                            setDirectBuffers(geometry, _gl.DYNAMIC_DRAW, !geometry.dynamic);
                        }

                        geometry.verticesNeedUpdate = false;
                        geometry.colorsNeedUpdate = false;
                    }
                    else
                    {
                        material = getBufferMaterial(obj, geometry);

                        customAttributesDirty = material.attributes != null && areCustomAttributesDirty(material);

                        if (geometry.verticesNeedUpdate || geometry.colorsNeedUpdate || geometry.lineDistancesNeedUpdate || customAttributesDirty)
                        {
                            setLineBuffers(geometry, _gl.DYNAMIC_DRAW);
                        }

                        geometry.verticesNeedUpdate = false;
                        geometry.colorsNeedUpdate = false;
                        geometry.lineDistancesNeedUpdate = false;

                        if (material.attributes != null)
                        {
                            clearCustomAttributes(material);
                        }
                    }
                }
                else if (obj is ParticleSystem)
                {
                    if (geometry is BufferGeometry)
                    {
                        if (JSObject.safe<bool>(geometry.verticesNeedUpdate) || JSObject.safe<bool>(geometry.colorsNeedUpdate))
                        {
                            setDirectBuffers(geometry, _gl.DYNAMIC_DRAW, !geometry.dynamic);
                        }

                        geometry.verticesNeedUpdate = false;
                        geometry.colorsNeedUpdate = false;
                    }
                    else
                    {
                        material = getBufferMaterial(obj, geometry);

                        customAttributesDirty = material.attributes != null && areCustomAttributesDirty(material);

                        if (geometry.verticesNeedUpdate || geometry.colorsNeedUpdate || ((ParticleSystem)obj).sortParticles || customAttributesDirty)
                        {
                            setParticleBuffers(geometry, _gl.DYNAMIC_DRAW, obj);
                        }

                        geometry.verticesNeedUpdate = false;
                        geometry.colorsNeedUpdate = false;

                        if (material.attributes != null)
                        {
                            clearCustomAttributes(material);
                        }
                    }
                }
            }
        }

        private bool areCustomAttributesDirty(dynamic material)
        {
            return ((IEnumerable)material.attributes).Cast<dynamic>().Any(a => material.attributes[a].needsUpdate);
        }

        private void clearCustomAttributes(dynamic material)
        {
            foreach (var a in material.attributes)
            {
                material.attributes[a].needsUpdate = false;
            }
        }

        private void removeObject(dynamic obj, Scene scene)
        {
            if (obj is Mesh || obj is ParticleSystem || obj is Ribbon || obj is Line)
            {
                removeInstances(scene.__webglObjects, obj);
            }
            else if (obj is Sprite)
            {
                removeInstancesDirect(scene.__webglSprites, obj);
            }
            else if (obj is LensFlare)
            {
                removeInstancesDirect(scene.__webglFlares, obj);
            }
            else if (obj is ImmediateRenderObject || obj.immediateRenderCallback)
            {
                removeInstances(scene.__webglObjectsImmediate, obj);
            }

            obj.__webglActive = false;
        }

        private void removeInstances(dynamic objlist, dynamic obj)
        {
            for (var o = objlist.length - 1; o >= 0; o--)
            {
                if (objlist[o].obj == obj)
                {
                    objlist.splice(o, 1);
                }
            }
        }

        private void removeInstancesDirect(dynamic objlist, dynamic obj)
        {
            for (var o = objlist.length - 1; o >= 0; o--)
            {
                if (objlist[o] == obj)
                {
                    objlist.splice(o, 1);
                }
            }
        }

        public void initMaterial(dynamic material, List<Light> lights, dynamic fog, dynamic obj)
        {
            ((Material)material).addEventListener("dispose", onMaterialDispose);

            string shaderID = null;

            if (material is MeshDepthMaterial)
            {
                shaderID = "depth";
            }
            else if (material is MeshNormalMaterial)
            {
                shaderID = "normal";
            }
            else if (material is MeshBasicMaterial)
            {
                shaderID = "basic";
            }
            else if (material is MeshLambertMaterial)
            {
                shaderID = "lambert";
            }
            else if (material is MeshPhongMaterial)
            {
                shaderID = "phong";
            }
            else if (material is LineBasicMaterial)
            {
                shaderID = "basic";
            }
            else if (material is LineDashedMaterial)
            {
                shaderID = "dashed";
            }
            else if (material is ParticleBasicMaterial)
            {
                shaderID = "particle_basic";
            }

            if (shaderID != null)
            {
                setMaterialShaders(material, THREE.ShaderLib[shaderID]);
            }

            var maxLightCount = allocateLights(lights);
            var maxShadows = allocateShadows(lights);
            var maxBones = allocateBones(obj);

            dynamic parameters = new JSObject();
            parameters.map = !!JSObject.eval(material.map);
            parameters.envMap = !!JSObject.eval(material.envMap);
            parameters.lightMap = !!JSObject.eval(material.lightMap);
            parameters.bumpMap = !!JSObject.eval(material.bumpMap);
            parameters.normalMap = !!JSObject.eval(material.normalMap);
            parameters.specularMap = !!JSObject.eval(material.specularMap);
            parameters.vertexColors = material.vertexColors;
            parameters.fog = fog;
            parameters.useFog = material.fog;
            parameters.fogExp = fog as FogExp2;
            parameters.sizeAttenuation = material.sizeAttenuation;
            parameters.skinning = material.skinning;
            parameters.maxBones = maxBones;
            parameters.useVertexTexture = _supportsBoneTextures && obj != null && JSObject.safe<bool>(obj.useVertexTexture);
            parameters.boneTextureWidth = obj != null ? JSObject.safe<int>(obj.boneTextureWidth) : 0;
            parameters.boneTextureHeight = obj != null ? JSObject.safe<int>(obj.boneTextureHeight) : 0;
            parameters.morphTargets = material.morphTargets;
            parameters.morphNormals = material.morphNormals;
            parameters.maxMorphTargets = maxMorphTargets;
            parameters.maxMorphNormals = maxMorphNormals;
            parameters.maxDirLights = maxLightCount.directional;
            parameters.maxPointLights = maxLightCount.point;
            parameters.maxSpotLights = maxLightCount.spot;
            parameters.maxHemiLights = maxLightCount.hemi;
            parameters.maxShadows = maxShadows;
            parameters.shadowMapEnabled = shadowMapEnabled && obj != null && obj.receiveShadow;
            parameters.shadowMapType = shadowMapType;
            parameters.shadowMapDebug = shadowMapDebug;
            parameters.shadowMapCascade = shadowMapCascade;
            parameters.alphaTest = material.alphaTest;
            parameters.metal = material.metal;
            parameters.perPixel = material.perPixel;
            parameters.wrapAround = material.wrapAround;
            parameters.doubleSided = material.side == THREE.DoubleSide;
            parameters.flipSided = material.side == THREE.BackSide;

            material.program = buildProgram(shaderID, material.fragmentShader, material.vertexShader, material.uniforms, material.attributes, material.defines, parameters);

            var attributes = material.program.attributes;

            if (JSObject.safe<bool>(material.morphTargets))
            {
                material.numSupportedMorphTargets = 0;

                for (var i = 0; i < maxMorphTargets; i++)
                {
                    if (attributes[string.Format("morphTarget{0}", i)] >= 0)
                    {
                        material.numSupportedMorphTargets++;
                    }
                }
            }

            if (material.morphNormals != null)
            {
                material.numSupportedMorphNormals = 0;

                for (var i = 0; i < maxMorphNormals; i++)
                {
                    if (attributes[string.Format("morphNormal{0}", i)] >= 0)
                    {
                        material.numSupportedMorphNormals++;
                    }
                }
            }

            material.uniformsList = new JSArray();
            foreach (var u in material.uniforms)
            {
                material.uniformsList.push(new JSArray(material.uniforms[u], u));
            }
        }

        private void setMaterialShaders(dynamic material, JSObject shaders)
        {
            material.uniforms = WebGLShaders.UniformsUtils.clone(shaders["uniforms"]);
            material.vertexShader = shaders["vertexShader"];
            material.fragmentShader = shaders["fragmentShader"];
        }

        private dynamic setProgram(Camera camera, List<Light> lights, dynamic fog, dynamic material, dynamic obj)
        {
            _usedTextureUnits = 0;

            if (material.needsUpdate)
            {
                if (material.program != null)
                {
                    deallocateMaterial(this, material);
                }

                this.initMaterial(material, lights, fog, obj);
                material.needsUpdate = false;
            }

            if (JSObject.safe<bool>(material.morphTargets))
            {
                if (!JSObject.eval(obj.__webglMorphTargetInfluences))
                {
                    obj.__webglMorphTargetInfluences = new Float32Array(maxMorphTargets);
                }
            }

            var refreshMaterial = false;

            var program = material.program;
            var p_uniforms = program.uniforms;
            var m_uniforms = material.uniforms;

            if (program != _currentProgram)
            {
                _gl.useProgram(program);
                _currentProgram = program;

                refreshMaterial = true;
            }

            if (material.id != _currentMaterialId)
            {
                _currentMaterialId = material.id;
                refreshMaterial = true;
            }

            if (refreshMaterial || camera != _currentCamera)
            {
                _gl.uniformMatrix4fv(p_uniforms.projectionMatrix, false, camera.projectionMatrix.elements);
                _currentCamera = camera;
            }

            if (JSObject.safe<bool>(material.skinning))
            {
                if (_supportsBoneTextures && obj.useVertexTexture)
                {
                    if (p_uniforms.boneTexture != null)
                    {
                        var textureUnit = getTextureUnit();

                        _gl.uniform1i(p_uniforms.boneTexture, textureUnit);
                        this.setTexture(obj.boneTexture, textureUnit);
                    }
                }
                else
                {
                    if (p_uniforms.boneGlobalMatrices != null)
                    {
                        _gl.uniformMatrix4fv(p_uniforms.boneGlobalMatrices, false, obj.boneMatrices);
                    }
                }
            }

            if (refreshMaterial)
            {
                if (fog != null && material.fog)
                {
                    refreshUniformsFog(m_uniforms, fog);
                }

                if (material is MeshPhongMaterial || material is MeshLambertMaterial || JSObject.safe<bool>(material.lights))
                {
                    if (_lightsNeedUpdate)
                    {
                        setupLights(lights);
                        _lightsNeedUpdate = false;
                    }

                    refreshUniformsLights(m_uniforms, _lights);
                }

                if (material is MeshBasicMaterial || material is MeshLambertMaterial || material is MeshPhongMaterial)
                {
                    refreshUniformsCommon(m_uniforms, material);
                }

                if (material is LineBasicMaterial)
                {
                    refreshUniformsLine(m_uniforms, material);
                }
                else if (material is LineDashedMaterial)
                {
                    refreshUniformsLine(m_uniforms, material);
                    refreshUniformsDash(m_uniforms, material);
                }
                else if (material is ParticleBasicMaterial)
                {
                    refreshUniformsParticle(m_uniforms, material);
                }
                else if (material is MeshPhongMaterial)
                {
                    refreshUniformsPhong(m_uniforms, material);
                }
                else if (material is MeshLambertMaterial)
                {
                    refreshUniformsLambert(m_uniforms, material);
                }
                else if (material is MeshDepthMaterial)
                {
                    var perspectiveCamera = (PerspectiveCamera)camera;
                    m_uniforms.mNear.value = perspectiveCamera.near;
                    m_uniforms.mFar.value = perspectiveCamera.far;
                    m_uniforms.opacity.value = material.opacity;
                }
                else if (material is MeshNormalMaterial)
                {
                    m_uniforms.opacity.value = material.opacity;
                }

                if (obj.receiveShadow && !JSObject.eval(material._shadowPass))
                {
                    refreshUniformsShadow(m_uniforms, lights);
                }

                loadUniformsGeneric(program, material.uniformsList);

                if (material is ShaderMaterial || material is MeshPhongMaterial || material.envMap != null)
                {
                    if (p_uniforms.cameraPosition != null)
                    {
                        var position = camera.matrixWorld.getPosition();
                        _gl.uniform3f(p_uniforms.cameraPosition, (float)position.x, (float)position.y, (float)position.z);
                    }
                }

                if (material is MeshPhongMaterial || material is MeshLambertMaterial || material is ShaderMaterial || JSObject.safe<bool>(material.skinning))
                {
                    if (p_uniforms.viewMatrix != null)
                    {
                        _gl.uniformMatrix4fv(p_uniforms.viewMatrix, false, camera.matrixWorldInverse.elements);
                    }
                }
            }

            loadUniformsMatrices(p_uniforms, obj);

            if (p_uniforms.modelMatrix != null)
            {
                _gl.uniformMatrix4fv(p_uniforms.modelMatrix, false, obj.matrixWorld.elements);
            }

            return program;
        }

        private void refreshUniformsCommon(dynamic uniforms, dynamic material)
        {
            uniforms.opacity.value = material.opacity;

            if (gammaInput)
            {
                uniforms.diffuse.value.copyGammaToLinear(material.color);
            }
            else
            {
                uniforms.diffuse.value = material.color;
            }

            uniforms.map.value = material.map;
            uniforms.lightMap.value = material.lightMap;
            uniforms.specularMap.value = material.specularMap;

            if (material.bumpMap != null)
            {
                uniforms.bumpMap.value = material.bumpMap;
                uniforms.bumpScale.value = material.bumpScale;
            }

            if (material.normalMap != null)
            {
                uniforms.normalMap.value = material.normalMap;
                uniforms.normalScale.value.copy(material.normalScale);
            }

            dynamic uvScaleMap = null;

            if (material.map != null)
            {
                uvScaleMap = material.map;
            }
            else if (material.specularMap != null)
            {
                uvScaleMap = material.specularMap;
            }
            else if (material.normalMap != null)
            {
                uvScaleMap = material.normalMap;
            }
            else if (material.bumpMap != null)
            {
                uvScaleMap = material.bumpMap;
            }

            if (uvScaleMap != null)
            {
                var offset = uvScaleMap.offset;
                var repeat = uvScaleMap.repeat;

                uniforms.offsetRepeat.value.set(offset.x, offset.y, repeat.x, repeat.y);
            }

            uniforms.envMap.value = material.envMap;
            uniforms.flipEnvMap.value = (material.envMap is WebGLRenderTargetCube) ? 1 : -1;

            uniforms.reflectivity.value = material.reflectivity;

            uniforms.refractionRatio.value = material.refractionRatio;
            uniforms.combine.value = material.combine;
            uniforms.useRefract.value = material.envMap != null && material.envMap.mapping == THREE.CubeRefractionMapping;
        }

        private void refreshUniformsLine(dynamic uniforms, dynamic material)
        {
            uniforms.diffuse.value = material.color;
            uniforms.opacity.value = material.opacity;
        }

        private void refreshUniformsDash(dynamic uniforms, dynamic material)
        {
            uniforms.dashSize.value = material.dashSize;
            uniforms.totalSize.value = material.dashSize + material.gapSize;
            uniforms.scale.value = material.scale;
        }

        private void refreshUniformsParticle(dynamic uniforms, dynamic material)
        {
            uniforms.psColor.value = material.color;
            uniforms.opacity.value = material.opacity;
            uniforms.size.value = material.size;
            uniforms.scale.value = _canvas.height / 2.0;

            uniforms.map.value = material.map;
        }

        private void refreshUniformsFog(dynamic uniforms, dynamic fog)
        {
            uniforms.fogColor.value = fog.color;

            if (fog is Fog)
            {
                uniforms.fogNear.value = fog.near;
                uniforms.fogFar.value = fog.far;
            }
            else if (fog is FogExp2)
            {
                uniforms.fogDensity.value = fog.density;
            }
        }

        private void refreshUniformsPhong(dynamic uniforms, dynamic material)
        {
            uniforms.shininess.value = material.shininess;

            if (gammaInput)
            {
                uniforms.ambient.value.copyGammaToLinear(material.ambient);
                uniforms.emissive.value.copyGammaToLinear(material.emissive);
                uniforms.specular.value.copyGammaToLinear(material.specular);
            }
            else
            {
                uniforms.ambient.value = material.ambient;
                uniforms.emissive.value = material.emissive;
                uniforms.specular.value = material.specular;
            }

            if (material.wrapAround)
            {
                uniforms.wrapRGB.value.copy(material.wrapRGB);
            }
        }

        private void refreshUniformsLambert(dynamic uniforms, dynamic material)
        {
            if (gammaInput)
            {
                uniforms.ambient.value.copyGammaToLinear(material.ambient);
                uniforms.emissive.value.copyGammaToLinear(material.emissive);
            }
            else
            {
                uniforms.ambient.value = material.ambient;
                uniforms.emissive.value = material.emissive;
            }

            if (material.wrapAround)
            {
                uniforms.wrapRGB.value.copy(material.wrapRGB);
            }
        }

        private void refreshUniformsLights(dynamic uniforms, dynamic lights)
        {
            uniforms.ambientLightColor.value = lights.ambient;

            uniforms.directionalLightColor.value = lights.directional.colors;
            uniforms.directionalLightDirection.value = lights.directional.positions;

            uniforms.pointLightColor.value = lights.point.colors;
            uniforms.pointLightPosition.value = lights.point.positions;
            uniforms.pointLightDistance.value = lights.point.distances;

            uniforms.spotLightColor.value = lights.spot.colors;
            uniforms.spotLightPosition.value = lights.spot.positions;
            uniforms.spotLightDistance.value = lights.spot.distances;
            uniforms.spotLightDirection.value = lights.spot.directions;
            uniforms.spotLightAngleCos.value = lights.spot.anglesCos;
            uniforms.spotLightExponent.value = lights.spot.exponents;

            uniforms.hemisphereLightSkyColor.value = lights.hemi.skyColors;
            uniforms.hemisphereLightGroundColor.value = lights.hemi.groundColors;
            uniforms.hemisphereLightDirection.value = lights.hemi.positions;
        }

        private void refreshUniformsShadow(dynamic uniforms, List<Light> lights)
        {
            if (uniforms.shadowMatrix != null)
            {
                var j = 0;

                for (var i = 0; i < lights.Count; i++)
                {
                    dynamic light = lights[i];

                    if (light.castShadow)
                    {
                        if (light is SpotLight || (light is DirectionalLight && !light.shadowCascade))
                        {
                            uniforms.shadowMap.value[j] = light.shadowMap;
                            uniforms.shadowMapSize.value[j] = light.shadowMapSize;

                            uniforms.shadowMatrix.value[j] = light.shadowMatrix;

                            uniforms.shadowDarkness.value[j] = light.shadowDarkness;
                            uniforms.shadowBias.value[j] = light.shadowBias;

                            j++;
                        }
                    }
                }
            }
        }

        private void loadUniformsMatrices(dynamic uniforms, dynamic obj)
        {
            _gl.uniformMatrix4fv(uniforms.modelViewMatrix, false, obj._modelViewMatrix.elements);

            if (uniforms.normalMatrix != null)
            {
                _gl.uniformMatrix3fv(uniforms.normalMatrix, false, obj._normalMatrix.elements);
            }
        }

        private int getTextureUnit()
        {
            var textureUnit = _usedTextureUnits;

            if (textureUnit >= _maxTextures)
            {
                JSConsole.warn(string.Format("WebGLRenderer: trying to use {0} texture units while this GPU supports only {1}", textureUnit, _maxTextures));
            }

            _usedTextureUnits += 1;

            return textureUnit;
        }

        private void loadUniformsGeneric(dynamic program, dynamic uniforms)
        {
            for (int j = 0, jl = uniforms.length; j < jl; j++)
            {
                var location = program.uniforms[uniforms[j][1]];
                if (!JSObject.eval(location))
                {
                    continue;
                }

                var uniform = uniforms[j][0];

                var type = uniform.type;
                var value = uniform.value;

                if (type == "i")
                {
                    _gl.uniform1i(location, Convert.ToInt32(value));
                }
                else if (type == "f")
                {
                    _gl.uniform1f(location, (float)value);
                }
                else if (type == "v2")
                {
                    _gl.uniform2f(location, value.x, value.y);
                }
                else if (type == "v3")
                {
                    _gl.uniform3f(location, (float)value.x, (float)value.y, (float)value.z);
                }
                else if (type == "v4")
                {
                    _gl.uniform4f(location, (float)value.x, (float)value.y, (float)value.z, (float)value.w);
                }
                else if (type == "c")
                {
                    _gl.uniform3f(location, (float)value.r, (float)value.g, (float)value.b);
                }
                else if (type == "iv1")
                {
                    _gl.uniform1iv(location, value);
                }
                else if (type == "iv")
                {
                    _gl.uniform3iv(location, value);
                }
                else if (type == "fv1")
                {
                    _gl.uniform1fv(location, toFloatArray(value));
                }
                else if (type == "fv")
                {
                    _gl.uniform3fv(location, toFloatArray(value));
                }
                else
                {
                    int offset;
                    if (type == "v2v")
                    {
                        if (uniform._array == null)
                        {
                            uniform._array = new Float32Array(2 * value.length);
                        }

                        for (int i = 0, il = value.length; i < il; i++)
                        {
                            offset = i * 2;

                            uniform._array[offset] = (float)value[i].x;
                            uniform._array[offset + 1] = (float)value[i].y;
                        }

                        _gl.uniform2fv(location, uniform._array);
                    }
                    else if (type == "v3v")
                    {
                        if (uniform._array == null)
                        {
                            uniform._array = new Float32Array(3 * value.length);
                        }

                        for (int i = 0, il = value.length; i < il; i++)
                        {
                            offset = i * 3;

                            uniform._array[offset] = value[i].x;
                            uniform._array[offset + 1] = value[i].y;
                            uniform._array[offset + 2] = value[i].z;
                        }

                        _gl.uniform3fv(location, uniform._array);
                    }
                    else if (type == "v4v")
                    {
                        if (uniform._array == null)
                        {
                            uniform._array = new Float32Array(4 * value.length);
                        }

                        for (int i = 0, il = value.length; i < il; i++)
                        {
                            offset = i * 4;

                            uniform._array[offset] = value[i].x;
                            uniform._array[offset + 1] = value[i].y;
                            uniform._array[offset + 2] = value[i].z;
                            uniform._array[offset + 3] = value[i].w;
                        }

                        _gl.uniform4fv(location, uniform._array);
                    }
                    else if (type == "m4")
                    {
                        if (uniform._array == null)
                        {
                            uniform._array = new Float32Array(16);
                        }

                        value.flattenToArray(uniform._array);
                        _gl.uniformMatrix4fv(location, false, uniform._array);
                    }
                    else if (type == "m4v")
                    {
                        if (uniform._array == null)
                        {
                            uniform._array = new Float32Array(16 * value.length);
                        }

                        for (int i = 0, il = value.length; i < il; i++)
                        {
                            value[i].flattenToArrayOffset(uniform._array, i * 16);
                        }

                        _gl.uniformMatrix4fv(location, false, uniform._array);
                    }
                    else
                    {
                        int textureUnit;
                        dynamic texture;
                        if (type == "t")
                        {
                            texture = value;
                            textureUnit = getTextureUnit();

                            _gl.uniform1i(location, textureUnit);

                            if (!JSObject.eval(texture))
                            {
                                continue;
                            }

                            if (texture.image is JSArray && texture.image.length == 6)
                            {
                                setCubeTexture(texture, textureUnit);
                            }
                            else if (texture is WebGLRenderTargetCube)
                            {
                                setCubeTextureDynamic(texture, textureUnit);
                            }
                            else
                            {
                                this.setTexture(texture, textureUnit);
                            }
                        }
                        else if (type == "tv")
                        {
                            if (uniform._array == null)
                            {
                                uniform._array = new JSArray();
                            }

                            for (int i = 0, il = uniform.value.length; i < il; i++)
                            {
                                uniform._array[i] = getTextureUnit();
                            }

                            _gl.uniform1iv(location, toInt32Array(uniform._array));

                            for (int i = 0, il = uniform.value.length; i < il; i++)
                            {
                                texture = uniform.value[i];
                                textureUnit = uniform._array[i];

                                if (!JSObject.eval(texture))
                                {
                                    continue;
                                }

                                this.setTexture(texture, textureUnit);
                            }
                        }
                    }
                }
            }
        }

        private void setupMatrices(Object3D obj, Camera camera)
        {
            obj._modelViewMatrix.multiplyMatrices(camera.matrixWorldInverse, obj.matrixWorld);

            obj._normalMatrix.getInverse(obj._modelViewMatrix);
            obj._normalMatrix.transpose();
        }

        private void setColorGamma(JSArray array, int offset, Color color, double intensitySq)
        {
            array[offset] = color.r * color.r * intensitySq;
            array[offset + 1] = color.g * color.g * intensitySq;
            array[offset + 2] = color.b * color.b * intensitySq;
        }

        private void setColorLinear(JSArray array, int offset, Color color, double intensity)
        {
            array[offset] = color.r * intensity;
            array[offset + 1] = color.g * intensity;
            array[offset + 2] = color.b * intensity;
        }

        private void setupLights(List<Light> lights)
        {
            var r = 0.0;
            var g = 0.0;
            var b = 0.0;
            var zlights = _lights;
            var dirColors = zlights.directional.colors;
            var dirPositions = zlights.directional.positions;
            var pointColors = zlights.point.colors;
            var pointPositions = zlights.point.positions;
            var pointDistances = zlights.point.distances;
            var spotColors = zlights.spot.colors;
            var spotPositions = zlights.spot.positions;
            var spotDistances = zlights.spot.distances;
            var spotDirections = zlights.spot.directions;
            var spotAnglesCos = zlights.spot.anglesCos;
            var spotExponents = zlights.spot.exponents;
            var hemiSkyColors = zlights.hemi.skyColors;
            var hemiGroundColors = zlights.hemi.groundColors;
            var hemiPositions = zlights.hemi.positions;
            var dirLength = 0;
            var pointLength = 0;
            var spotLength = 0;
            var hemiLength = 0;
            var dirCount = 0;
            var pointCount = 0;
            var spotCount = 0;
            var hemiCount = 0;
            var dirOffset = 0;
            var pointOffset = 0;
            var spotOffset = 0;
            var hemiOffset = 0;

            for (int l = 0, ll = lights.Count; l < ll; l++)
            {
                dynamic light = lights[l];

                if (light.onlyShadow)
                {
                    continue;
                }

                var color = light.color;
                var intensity = light.intensity;
                var distance = light.distance;

                if (light is AmbientLight)
                {
                    if (!light.visible)
                    {
                        continue;
                    }

                    if (gammaInput)
                    {
                        r += color.r * color.r;
                        g += color.g * color.g;
                        b += color.b * color.b;
                    }
                    else
                    {
                        r += color.r;
                        g += color.g;
                        b += color.b;
                    }
                }
                else if (light is DirectionalLight)
                {
                    dirCount += 1;

                    if (!light.visible)
                    {
                        continue;
                    }

                    _direction.copy(light.matrixWorld.getPosition());
                    _direction.sub(light.target.matrixWorld.getPosition());
                    _direction.normalize();

                    if (_direction.x == 0 && _direction.y == 0 && _direction.z == 0)
                    {
                        continue;
                    }

                    dirOffset = dirLength * 3;

                    dirPositions[dirOffset] = _direction.x;
                    dirPositions[dirOffset + 1] = _direction.y;
                    dirPositions[dirOffset + 2] = _direction.z;

                    if (gammaInput)
                    {
                        setColorGamma(dirColors, dirOffset, color, intensity * intensity);
                    }
                    else
                    {
                        setColorLinear(dirColors, dirOffset, color, intensity);
                    }

                    dirLength += 1;
                }
                else if (light is PointLight)
                {
                    pointCount += 1;

                    if (!light.visible)
                    {
                        continue;
                    }

                    pointOffset = pointLength * 3;

                    if (gammaInput)
                    {
                        setColorGamma(pointColors, pointOffset, color, intensity * intensity);
                    }
                    else
                    {
                        setColorLinear(pointColors, pointOffset, color, intensity);
                    }

                    var position = light.matrixWorld.getPosition();

                    pointPositions[pointOffset] = position.x;
                    pointPositions[pointOffset + 1] = position.y;
                    pointPositions[pointOffset + 2] = position.z;

                    pointDistances[pointLength] = distance;

                    pointLength += 1;
                }
                else if (light is SpotLight)
                {
                    spotCount += 1;

                    if (!light.visible)
                    {
                        continue;
                    }

                    spotOffset = spotLength * 3;

                    if (gammaInput)
                    {
                        setColorGamma(spotColors, spotOffset, color, intensity * intensity);
                    }
                    else
                    {
                        setColorLinear(spotColors, spotOffset, color, intensity);
                    }

                    var position = light.matrixWorld.getPosition();

                    spotPositions[spotOffset] = position.x;
                    spotPositions[spotOffset + 1] = position.y;
                    spotPositions[spotOffset + 2] = position.z;

                    spotDistances[spotLength] = distance;

                    _direction.copy(position);
                    _direction.sub(light.target.matrixWorld.getPosition());
                    _direction.normalize();

                    spotDirections[spotOffset] = _direction.x;
                    spotDirections[spotOffset + 1] = _direction.y;
                    spotDirections[spotOffset + 2] = _direction.z;

                    spotAnglesCos[spotLength] = System.Math.Cos((double)light.angle);
                    spotExponents[spotLength] = light.exponent;

                    spotLength += 1;
                }
                else if (light is HemisphereLight)
                {
                    hemiCount += 1;

                    if (!light.visible)
                    {
                        continue;
                    }

                    _direction.copy(light.matrixWorld.getPosition());
                    _direction.normalize();

                    if (_direction.x == 0 && _direction.y == 0 && _direction.z == 0)
                    {
                        continue;
                    }

                    hemiOffset = hemiLength * 3;

                    hemiPositions[hemiOffset] = _direction.x;
                    hemiPositions[hemiOffset + 1] = _direction.y;
                    hemiPositions[hemiOffset + 2] = _direction.z;

                    var skyColor = light.color;
                    var groundColor = light.groundColor;

                    if (gammaInput)
                    {
                        var intensitySq = intensity * intensity;

                        setColorGamma(hemiSkyColors, hemiOffset, skyColor, intensitySq);
                        setColorGamma(hemiGroundColors, hemiOffset, groundColor, intensitySq);
                    }
                    else
                    {
                        setColorLinear(hemiSkyColors, hemiOffset, skyColor, intensity);
                        setColorLinear(hemiGroundColors, hemiOffset, groundColor, intensity);
                    }

                    hemiLength += 1;
                }
            }

            for (int l = dirLength * 3, ll = (int)System.Math.Max(dirColors.length, dirCount * 3); l < ll; l++)
            {
                dirColors[l] = 0.0;
            }
            for (int l = pointLength * 3, ll = (int)System.Math.Max(pointColors.length, pointCount * 3); l < ll; l++)
            {
                pointColors[l] = 0.0;
            }
            for (int l = spotLength * 3, ll = (int)System.Math.Max(spotColors.length, spotCount * 3); l < ll; l++)
            {
                spotColors[l] = 0.0;
            }
            for (int l = hemiLength * 3, ll = (int)System.Math.Max(hemiSkyColors.length, hemiCount * 3); l < ll; l++)
            {
                hemiSkyColors[l] = 0.0;
            }
            for (int l = hemiLength * 3, ll = (int)System.Math.Max(hemiGroundColors.length, hemiCount * 3); l < ll; l++)
            {
                hemiGroundColors[l] = 0.0;
            }

            zlights.directional.length = dirLength;
            zlights.point.length = pointLength;
            zlights.spot.length = spotLength;
            zlights.hemi.length = hemiLength;

            zlights.ambient[0] = r;
            zlights.ambient[1] = g;
            zlights.ambient[2] = b;
        }

        public void setFaceCulling(dynamic cullFace, dynamic frontFaceDirection)
        {
            if (cullFace == THREE.CullFaceNone)
            {
                _gl.disable(_gl.CULL_FACE);
            }
            else
            {
                if (frontFaceDirection == THREE.FrontFaceDirectionCW)
                {
                    _gl.frontFace(_gl.CW);
                }
                else
                {
                    _gl.frontFace(_gl.CCW);
                }

                if (cullFace == THREE.CullFaceBack)
                {
                    _gl.cullFace(_gl.BACK);
                }
                else if (cullFace == THREE.CullFaceFront)
                {
                    _gl.cullFace(_gl.FRONT);
                }
                else
                {
                    _gl.cullFace(_gl.FRONT_AND_BACK);
                }

                _gl.enable(_gl.CULL_FACE);
            }
        }

        public void setMaterialFaces(Material material)
        {
            var doubleSided = material.side == THREE.DoubleSide;
            var flipSided = material.side == THREE.BackSide;

            if (_oldDoubleSided != doubleSided)
            {
                if (doubleSided)
                {
                    _gl.disable(_gl.CULL_FACE);
                }
                else
                {
                    _gl.enable(_gl.CULL_FACE);
                }

                _oldDoubleSided = doubleSided;
            }

            if (_oldFlipSided != flipSided)
            {
                _gl.frontFace(flipSided ? _gl.CW : _gl.CCW);

                _oldFlipSided = flipSided;
            }
        }

        public void setDepthTest(bool depthTest)
        {
            if (_oldDepthTest != depthTest)
            {
                if (depthTest)
                {
                    _gl.enable(_gl.DEPTH_TEST);
                }
                else
                {
                    _gl.disable(_gl.DEPTH_TEST);
                }

                _oldDepthTest = depthTest;
            }
        }

        public void setDepthWrite(bool depthWrite)
        {
            if (_oldDepthWrite != depthWrite)
            {
                _gl.depthMask(depthWrite);
                _oldDepthWrite = depthWrite;
            }
        }

        private void setLineWidth(double width)
        {
            if (width != _oldLineWidth)
            {
                _gl.lineWidth((float)width);
                _oldLineWidth = width;
            }
        }

        private void setPolygonOffset(bool polygonoffset, double factor, double units)
        {
            if (_oldPolygonOffset != polygonoffset)
            {
                if (polygonoffset)
                {
                    _gl.enable(_gl.POLYGON_OFFSET_FILL);
                }
                else
                {
                    _gl.disable(_gl.POLYGON_OFFSET_FILL);
                }

                _oldPolygonOffset = polygonoffset;
            }

            if (polygonoffset && (_oldPolygonOffsetFactor != factor || _oldPolygonOffsetUnits != units))
            {
                _gl.polygonOffset((float)factor, (float)units);

                _oldPolygonOffsetFactor = factor;
                _oldPolygonOffsetUnits = units;
            }
        }

        public void setBlending(int blending, int? blendEquation = null, int? blendSrc = null, int? blendDst = null)
        {
            if (blending != _oldBlending)
            {
                if (blending == THREE.NoBlending)
                {
                    _gl.disable(_gl.BLEND);
                }
                else if (blending == THREE.AdditiveBlending)
                {
                    _gl.enable(_gl.BLEND);
                    _gl.blendEquation(_gl.FUNC_ADD);
                    _gl.blendFunc(_gl.SRC_ALPHA, _gl.ONE);
                }
                else if (blending == THREE.SubtractiveBlending)
                {
                    _gl.enable(_gl.BLEND);
                    _gl.blendEquation(_gl.FUNC_ADD);
                    _gl.blendFunc(_gl.ZERO, _gl.ONE_MINUS_SRC_COLOR);
                }
                else if (blending == THREE.MultiplyBlending)
                {
                    _gl.enable(_gl.BLEND);
                    _gl.blendEquation(_gl.FUNC_ADD);
                    _gl.blendFunc(_gl.ZERO, _gl.SRC_COLOR);
                }
                else if (blending == THREE.CustomBlending)
                {
                    _gl.enable(_gl.BLEND);
                }
                else
                {
                    _gl.enable(_gl.BLEND);
                    _gl.blendEquationSeparate(_gl.FUNC_ADD, _gl.FUNC_ADD);
                    _gl.blendFuncSeparate(_gl.SRC_ALPHA, _gl.ONE_MINUS_SRC_ALPHA, _gl.ONE, _gl.ONE_MINUS_SRC_ALPHA);
                }

                _oldBlending = blending;
            }

            if (blending == THREE.CustomBlending)
            {
                if (blendEquation != _oldBlendEquation)
                {
                    _gl.blendEquation(paramThreeToGL(blendEquation.Value));

                    _oldBlendEquation = blendEquation;
                }

                if (blendSrc != _oldBlendSrc || blendDst != _oldBlendDst)
                {
                    _gl.blendFunc(paramThreeToGL(blendSrc.Value), paramThreeToGL(blendDst.Value));

                    _oldBlendSrc = blendSrc;
                    _oldBlendDst = blendDst;
                }
            }
            else
            {
                _oldBlendEquation = null;
                _oldBlendSrc = null;
                _oldBlendDst = null;
            }
        }

        private string generateDefines(dynamic defines)
        {
            var chunks = new JSArray();

            if (defines != null)
            {
                foreach (var d in defines)
                {
                    var value = defines[d];
                    if (value == false)
                    {
                        continue;
                    }
                    chunks.push("#define " + d + " " + value);
                }
            }

            return chunks.join("\n");
        }

        private dynamic buildProgram(string shaderID, string fragmentShader, string vertexShader, JSObject uniforms, JSObject attributes, JSObject defines, dynamic parameters)
        {
            var chunks = new JSArray();

            if (shaderID != null)
            {
                chunks.push(shaderID);
            }
            else
            {
                chunks.push(fragmentShader);
                chunks.push(vertexShader);
            }

            if (defines != null)
            {
                foreach (var d in defines)
                {
                    chunks.push(d);
                    chunks.push(defines[d]);
                }
            }

            foreach (var p in parameters)
            {
                chunks.push(p);
                chunks.push(parameters[p]);
            }

            var code = chunks.join();

            for (int p = 0, pl = _programs.length; p < pl; p++)
            {
                var programInfo = _programs[p];

                if (programInfo.code == code)
                {
                    programInfo.usedTimes++;

                    return programInfo.program;
                }
            }

            var shadowMapTypeDefine = "SHADOWMAP_TYPE_BASIC";

            if (parameters.shadowMapType == THREE.PCFShadowMap)
            {
                shadowMapTypeDefine = "SHADOWMAP_TYPE_PCF";
            }
            else if (parameters.shadowMapType == THREE.PCFSoftShadowMap)
            {
                shadowMapTypeDefine = "SHADOWMAP_TYPE_PCF_SOFT";
            }

            var customDefines = generateDefines(defines);

            dynamic program = _gl.createProgram();

            var prefix_vertex = new JSArray();
            prefix_vertex.push("precision " + _precision + " float;");
            prefix_vertex.push(customDefines);
            prefix_vertex.push(_supportsVertexTextures ? "#define VERTEX_TEXTURES" : "");
            prefix_vertex.push(gammaInput ? "#define GAMMA_INPUT" : "");
            prefix_vertex.push(gammaOutput ? "#define GAMMA_OUTPUT" : "");
            prefix_vertex.push(physicallyBasedShading ? "#define PHYSICALLY_BASED_SHADING" : "");
            prefix_vertex.push("#define MAX_DIR_LIGHTS " + parameters.maxDirLights);
            prefix_vertex.push("#define MAX_POINT_LIGHTS " + parameters.maxPointLights);
            prefix_vertex.push("#define MAX_SPOT_LIGHTS " + parameters.maxSpotLights);
            prefix_vertex.push("#define MAX_HEMI_LIGHTS " + parameters.maxHemiLights);
            prefix_vertex.push("#define MAX_SHADOWS " + parameters.maxShadows);
            prefix_vertex.push("#define MAX_BONES " + parameters.maxBones);
            prefix_vertex.push(parameters.map ? "#define USE_MAP" : "");
            prefix_vertex.push(parameters.envMap ? "#define USE_ENVMAP" : "");
            prefix_vertex.push(parameters.lightMap ? "#define USE_LIGHTMAP" : "");
            prefix_vertex.push(parameters.bumpMap ? "#define USE_BUMPMAP" : "");
            prefix_vertex.push(parameters.normalMap ? "#define USE_NORMALMAP" : "");
            prefix_vertex.push(parameters.specularMap ? "#define USE_SPECULARMAP" : "");
            prefix_vertex.push(JSObject.eval(parameters.vertexColors) ? "#define USE_COLOR" : "");
            prefix_vertex.push(JSObject.safe<bool>(parameters.skinning) ? "#define USE_SKINNING" : "");
            prefix_vertex.push(JSObject.safe<bool>(parameters.useVertexTexture) ? "#define BONE_TEXTURE" : "");
            prefix_vertex.push(JSObject.safe<int>(parameters.boneTextureWidth) > 0 ? "#define N_BONE_PIXEL_X " + parameters.boneTextureWidth.ToString("N" + 1) : "");
            prefix_vertex.push(JSObject.safe<int>(parameters.boneTextureHeight) > 0 ? "#define N_BONE_PIXEL_Y " + parameters.boneTextureHeight.ToString("N" + 1) : "");
            prefix_vertex.push(JSObject.safe<bool>(parameters.morphTargets) ? "#define USE_MORPHTARGETS" : "");
            prefix_vertex.push(JSObject.safe<bool>(parameters.morphNormals) ? "#define USE_MORPHNORMALS" : "");
            prefix_vertex.push(JSObject.safe<bool>(parameters.perPixel) ? "#define PHONG_PER_PIXEL" : "");
            prefix_vertex.push(JSObject.safe<bool>(parameters.wrapAround) ? "#define WRAP_AROUND" : "");
            prefix_vertex.push(JSObject.safe<bool>(parameters.doubleSided) ? "#define DOUBLE_SIDED" : "");
            prefix_vertex.push(JSObject.safe<bool>(parameters.flipSided) ? "#define FLIP_SIDED" : "");
            prefix_vertex.push(JSObject.safe<bool>(parameters.shadowMapEnabled) ? "#define USE_SHADOWMAP" : "");
            prefix_vertex.push(JSObject.safe<bool>(parameters.shadowMapEnabled) ? "#define " + shadowMapTypeDefine : "");
            prefix_vertex.push(JSObject.safe<bool>(parameters.shadowMapDebug) ? "#define SHADOWMAP_DEBUG" : "");
            prefix_vertex.push(JSObject.safe<bool>(parameters.shadowMapCascade) ? "#define SHADOWMAP_CASCADE" : "");
            prefix_vertex.push(JSObject.safe<bool>(parameters.sizeAttenuation) ? "#define USE_SIZEATTENUATION" : "");
            prefix_vertex.push("uniform mat4 modelMatrix;");
            prefix_vertex.push("uniform mat4 modelViewMatrix;");
            prefix_vertex.push("uniform mat4 projectionMatrix;");
            prefix_vertex.push("uniform mat4 viewMatrix;");
            prefix_vertex.push("uniform mat3 normalMatrix;");
            prefix_vertex.push("uniform vec3 cameraPosition;");
            prefix_vertex.push("attribute vec3 position;");
            prefix_vertex.push("attribute vec3 normal;");
            prefix_vertex.push("attribute vec2 uv;");
            prefix_vertex.push("attribute vec2 uv2;");
            prefix_vertex.push("#ifdef USE_COLOR");
            prefix_vertex.push("attribute vec3 color;");
            prefix_vertex.push("#endif");
            prefix_vertex.push("#ifdef USE_MORPHTARGETS");
            prefix_vertex.push("attribute vec3 morphTarget0;");
            prefix_vertex.push("attribute vec3 morphTarget1;");
            prefix_vertex.push("attribute vec3 morphTarget2;");
            prefix_vertex.push("attribute vec3 morphTarget3;");
            prefix_vertex.push("#ifdef USE_MORPHNORMALS");
            prefix_vertex.push("attribute vec3 morphNormal0;");
            prefix_vertex.push("attribute vec3 morphNormal1;");
            prefix_vertex.push("attribute vec3 morphNormal2;");
            prefix_vertex.push("attribute vec3 morphNormal3;");
            prefix_vertex.push("#else");
            prefix_vertex.push("attribute vec3 morphTarget4;");
            prefix_vertex.push("attribute vec3 morphTarget5;");
            prefix_vertex.push("attribute vec3 morphTarget6;");
            prefix_vertex.push("attribute vec3 morphTarget7;");
            prefix_vertex.push("#endif");
            prefix_vertex.push("#endif");
            prefix_vertex.push("#ifdef USE_SKINNING");
            prefix_vertex.push("attribute vec4 skinIndex;");
            prefix_vertex.push("attribute vec4 skinWeight;");
            prefix_vertex.push("#endif");
            prefix_vertex.push("");

            var prefix_fragment = new JSArray();
            prefix_fragment.push("precision " + _precision + " float;");
            prefix_fragment.push((parameters.bumpMap || parameters.normalMap) ? "#extension GL_OES_standard_derivatives : enable" : "");
            prefix_fragment.push(customDefines);
            prefix_fragment.push("#define MAX_DIR_LIGHTS " + parameters.maxDirLights);
            prefix_fragment.push("#define MAX_POINT_LIGHTS " + parameters.maxPointLights);
            prefix_fragment.push("#define MAX_SPOT_LIGHTS " + parameters.maxSpotLights);
            prefix_fragment.push("#define MAX_HEMI_LIGHTS " + parameters.maxHemiLights);
            prefix_fragment.push("#define MAX_SHADOWS " + parameters.maxShadows);
            prefix_fragment.push(JSObject.safe<double>(parameters.alphaTest) != 0 ? "#define ALPHATEST " + parameters.alphaTest : "");
            prefix_fragment.push(gammaInput ? "#define GAMMA_INPUT" : "");
            prefix_fragment.push(gammaOutput ? "#define GAMMA_OUTPUT" : "");
            prefix_fragment.push(physicallyBasedShading ? "#define PHYSICALLY_BASED_SHADING" : "");
            prefix_fragment.push((JSObject.safe<bool>(parameters.useFog) && parameters.fog != null) ? "#define USE_FOG" : "");
            prefix_fragment.push((JSObject.safe<bool>(parameters.useFog) && parameters.fogExp != null) ? "#define FOG_EXP2" : "");
            prefix_fragment.push(parameters.map ? "#define USE_MAP" : "");
            prefix_fragment.push(parameters.envMap ? "#define USE_ENVMAP" : "");
            prefix_fragment.push(parameters.lightMap ? "#define USE_LIGHTMAP" : "");
            prefix_fragment.push(parameters.bumpMap ? "#define USE_BUMPMAP" : "");
            prefix_fragment.push(parameters.normalMap ? "#define USE_NORMALMAP" : "");
            prefix_fragment.push(parameters.specularMap ? "#define USE_SPECULARMAP" : "");
            prefix_fragment.push(JSObject.eval(parameters.vertexColors) ? "#define USE_COLOR" : "");
            prefix_fragment.push(JSObject.safe<bool>(parameters.metal) ? "#define METAL" : "");
            prefix_fragment.push(JSObject.safe<bool>(parameters.perPixel) ? "#define PHONG_PER_PIXEL" : "");
            prefix_fragment.push(JSObject.safe<bool>(parameters.wrapAround) ? "#define WRAP_AROUND" : "");
            prefix_fragment.push(JSObject.safe<bool>(parameters.doubleSided) ? "#define DOUBLE_SIDED" : "");
            prefix_fragment.push(JSObject.safe<bool>(parameters.flipSided) ? "#define FLIP_SIDED" : "");
            prefix_fragment.push(JSObject.safe<bool>(parameters.shadowMapEnabled) ? "#define USE_SHADOWMAP" : "");
            prefix_fragment.push(JSObject.safe<bool>(parameters.shadowMapEnabled) ? "#define " + shadowMapTypeDefine : "");
            prefix_fragment.push(JSObject.safe<bool>(parameters.shadowMapDebug) ? "#define SHADOWMAP_DEBUG" : "");
            prefix_fragment.push(JSObject.safe<bool>(parameters.shadowMapCascade) ? "#define SHADOWMAP_CASCADE" : "");
            prefix_fragment.push("uniform mat4 viewMatrix;");
            prefix_fragment.push("uniform vec3 cameraPosition;");
            prefix_fragment.push("");

            var glFragmentShader = getShader("fragment", prefix_fragment.join("\n") + fragmentShader);
            var glVertexShader = getShader("vertex", prefix_vertex.join("\n") + vertexShader);

            _gl.attachShader(program, glVertexShader);
            _gl.attachShader(program, glFragmentShader);

            _gl.linkProgram(program);

            if (!_gl.getProgramParameter(program, _gl.LINK_STATUS))
            {
                JSConsole.error("Could not initialise shader\n" + "VALIDATE_STATUS: " + _gl.getProgramParameter(program, _gl.VALIDATE_STATUS) + ", gl error [" + _gl.getError() + "]");
            }

            _gl.deleteShader(glFragmentShader);
            _gl.deleteShader(glVertexShader);

            program.uniforms = new JSObject();
            program.attributes = new JSObject();

            var identifiers = new JSArray("viewMatrix", "modelViewMatrix", "projectionMatrix", "normalMatrix", "modelMatrix", "cameraPosition",
                                          "morphTargetInfluences");

            if (parameters.useVertexTexture)
            {
                identifiers.push("boneTexture");
            }
            else
            {
                identifiers.push("boneGlobalMatrices");
            }

            foreach (var u in uniforms)
            {
                identifiers.push(u);
            }

            cacheUniformLocations(program, identifiers);

            identifiers = new JSArray("position", "normal", "uv", "uv2", "tangent", "color", "skinIndex", "skinWeight", "lineDistance");

            for (var i = 0; i < parameters.maxMorphTargets; i++)
            {
                identifiers.push("morphTarget" + i);
            }

            for (var i = 0; i < parameters.maxMorphNormals; i++)
            {
                identifiers.push("morphNormal" + i);
            }

            if (attributes != null)
            {
                foreach (var a in attributes)
                {
                    identifiers.push(a);
                }
            }

            cacheAttributeLocations(program, identifiers);

            program.id = _programs_counter++;

            _programs.push(JSObject.create(new {program, code, usedTimes = 1}));

            info.memory.programs = _programs.length;

            return program;
        }

        private void cacheUniformLocations(dynamic program, JSArray identifiers)
        {
            for (var i = 0; i < identifiers.length; i++)
            {
                var id = identifiers[i];
                program.uniforms[id] = _gl.getUniformLocation(program, id);
            }
        }

        private void cacheAttributeLocations(dynamic program, JSArray identifiers)
        {
            for (var i = 0; i < identifiers.length; i++)
            {
                var id = identifiers[i];
                program.attributes[id] = _gl.getAttribLocation(program, id);
            }
        }

        private string addLineNumbers(string str)
        {
            var chunks = str.Split('\n');
            for (var i = 0; i < chunks.Length; i++)
            {
                chunks[i] = (i + 1) + ": " + chunks[i];
            }
            return String.Join("\n", chunks);
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

            _gl.shaderSource(shader, str);
            _gl.compileShader(shader);

            if (!_gl.getShaderParameter(shader, _gl.COMPILE_STATUS))
            {
                JSConsole.error(_gl.getShaderInfoLog(shader));
                JSConsole.error(addLineNumbers(str));
                return null;
            }

            return shader;
        }

        private bool isPowerOfTwo(int value)
        {
            return (value & (value - 1)) == 0;
        }

        private void setTextureParameters(uint textureType, dynamic texture, bool isImagePowerOfTwo)
        {
            if (isImagePowerOfTwo)
            {
                _gl.texParameteri(textureType, _gl.TEXTURE_WRAP_S, (int)paramThreeToGL(texture.wrapS));
                _gl.texParameteri(textureType, _gl.TEXTURE_WRAP_T, (int)paramThreeToGL(texture.wrapT));

                _gl.texParameteri(textureType, _gl.TEXTURE_MAG_FILTER, (int)paramThreeToGL(texture.magFilter));
                _gl.texParameteri(textureType, _gl.TEXTURE_MIN_FILTER, (int)paramThreeToGL(texture.minFilter));
            }
            else
            {
                _gl.texParameteri(textureType, _gl.TEXTURE_WRAP_S, (int)_gl.CLAMP_TO_EDGE);
                _gl.texParameteri(textureType, _gl.TEXTURE_WRAP_T, (int)_gl.CLAMP_TO_EDGE);

                _gl.texParameteri(textureType, _gl.TEXTURE_MAG_FILTER, (int)filterFallback(texture.magFilter));
                _gl.texParameteri(textureType, _gl.TEXTURE_MIN_FILTER, (int)filterFallback(texture.minFilter));
            }

            if (_glExtensionTextureFilterAnisotropic != null && texture.type != THREE.FloatType)
            {
                if (texture.anisotropy > 1 || texture.__oldAnisotropy > 0)
                {
                    _gl.texParameterf(textureType, _glExtensionTextureFilterAnisotropic.TEXTURE_MAX_ANISOTROPY_EXT, System.Math.Min(texture.anisotropy, _maxAnisotropy));
                    texture.__oldAnisotropy = texture.anisotropy;
                }
            }
        }

        public void setTexture(dynamic texture, int slot)
        {
            if (JSObject.safe<bool>(texture.needsUpdate))
            {
                if (!texture.__webglInit)
                {
                    texture.__webglInit = true;

                    ((Texture)texture).addEventListener("dispose", onTextureDispose);

                    texture.__webglTexture = _gl.createTexture();

                    info.memory.textures++;
                }

                _gl.activeTexture((uint)(_gl.TEXTURE0 + slot));
                _gl.bindTexture(_gl.TEXTURE_2D, texture.__webglTexture);

                _gl.pixelStorei(_gl.UNPACK_FLIP_Y_WEBGL, Convert.ToInt32(texture.flipY));
                _gl.pixelStorei(_gl.UNPACK_PREMULTIPLY_ALPHA_WEBGL, Convert.ToInt32(texture.premultiplyAlpha));
                _gl.pixelStorei(_gl.UNPACK_ALIGNMENT, (int)texture.unpackAlignment);

                var image = texture.image;
                var isImagePowerOfTwo = isPowerOfTwo(image.width) && isPowerOfTwo(image.height);
                var glFormat = paramThreeToGL(texture.format);
                var glType = paramThreeToGL(texture.type);

                setTextureParameters(_gl.TEXTURE_2D, texture, isImagePowerOfTwo);

                var mipmaps = texture.mipmaps;

                if (texture is DataTexture)
                {
                    if (mipmaps.length > 0 && isImagePowerOfTwo)
                    {
                        for (int i = 0, il = mipmaps.length; i < il; i++)
                        {
                            var mipmap = mipmaps[i];
                            _gl.texImage2D(_gl.TEXTURE_2D, i, glFormat, mipmap.width, mipmap.height, 0, glFormat, glType, mipmap.data);
                        }

                        texture.generateMipmaps = false;
                    }
                    else
                    {
                        _gl.texImage2D(_gl.TEXTURE_2D, 0, glFormat, image.width, image.height, 0, glFormat, glType, image.data);
                    }
                }
                else if (texture is CompressedTexture)
                {
                    for (var i = 0; i < mipmaps.length; i++)
                    {
                        _gl.compressedTexImage2D(_gl.TEXTURE_2D, i, glFormat, mipmaps[i].width, mipmaps[i].height, 0, mipmaps[i].data);
                    }
                }
                else
                {
                    if (mipmaps.length > 0 && isImagePowerOfTwo)
                    {
                        for (int i = 0, il = mipmaps.length; i < il; i++)
                        {
                            _gl.texImage2D(_gl.TEXTURE_2D, i, glFormat, glFormat, glType, mipmaps[i].imageData);
                        }

                        texture.generateMipmaps = false;
                    }
                    else
                    {
                        _gl.texImage2D(_gl.TEXTURE_2D, 0, glFormat, glFormat, glType, texture.image.imageData);
                    }
                }

                if (texture.generateMipmaps && isImagePowerOfTwo)
                {
                    _gl.generateMipmap(_gl.TEXTURE_2D);
                }

                texture.needsUpdate = false;

                if (texture.onUpdate != null)
                {
                    texture.onUpdate();
                }
            }
            else
            {
                _gl.activeTexture((uint)(_gl.TEXTURE0 + slot));
                _gl.bindTexture(_gl.TEXTURE_2D, texture.__webglTexture);
            }
        }

        private Image clampToMaxSize(Image image, int maxSize)
        {
            if (image.width <= maxSize && image.height <= maxSize)
            {
                return image;
            }
            throw new NotImplementedException();
        }

        private void setCubeTexture(Texture texture, int slot)
        {
            if (texture.image.length == 6)
            {
                if (texture.needsUpdate)
                {
                    if (!JSObject.eval(texture.image.__webglTextureCube))
                    {
                        texture.image.__webglTextureCube = _gl.createTexture();

                        info.memory.textures++;
                    }

                    _gl.activeTexture((uint)(_gl.TEXTURE0 + slot));
                    _gl.bindTexture(_gl.TEXTURE_CUBE_MAP, texture.image.__webglTextureCube);

                    _gl.pixelStorei(_gl.UNPACK_FLIP_Y_WEBGL, Convert.ToInt32(texture.flipY));

                    var isCompressed = texture is CompressedTexture;

                    var cubeImage = new JSArray();

                    for (var i = 0; i < 6; i++)
                    {
                        if (autoScaleCubemaps && !isCompressed)
                        {
                            cubeImage[i] = clampToMaxSize(texture.image[i], _maxCubemapSize);
                        }
                        else
                        {
                            cubeImage[i] = texture.image[i];
                        }
                    }

                    var image = cubeImage[0];
                    var isImagePowerOfTwo = isPowerOfTwo(image.width) && isPowerOfTwo(image.height);
                    var glFormat = paramThreeToGL(texture.format);
                    var glType = paramThreeToGL(texture.type);

                    setTextureParameters(_gl.TEXTURE_CUBE_MAP, texture, isImagePowerOfTwo);

                    for (var i = 0; i < 6; i++)
                    {
                        if (isCompressed)
                        {
                            var mipmaps = cubeImage[i].mipmaps;

                            for (var j = 0; j < mipmaps.length; j++)
                            {
                                _gl.compressedTexImage2D((uint)(_gl.TEXTURE_CUBE_MAP_POSITIVE_X + i), j, glFormat, mipmaps[j].width, mipmaps[j].height, 0, mipmaps[j].data);
                            }
                        }
                        else
                        {
                            _gl.texImage2D((uint)(_gl.TEXTURE_CUBE_MAP_POSITIVE_X + i), 0, glFormat, glFormat, glType, cubeImage[i].imageData);
                        }
                    }

                    if (texture.generateMipmaps && isImagePowerOfTwo)
                    {
                        _gl.generateMipmap(_gl.TEXTURE_CUBE_MAP);
                    }

                    texture.needsUpdate = false;

                    if (texture.onUpdate != null)
                    {
                        texture.onUpdate();
                    }
                }
                else
                {
                    _gl.activeTexture((uint)(_gl.TEXTURE0 + slot));
                    _gl.bindTexture(_gl.TEXTURE_CUBE_MAP, texture.image.__webglTextureCube);
                }
            }
        }

        private void setCubeTextureDynamic(dynamic texture, int slot)
        {
            _gl.activeTexture((uint)(_gl.TEXTURE0 + slot));
            _gl.bindTexture(_gl.TEXTURE_CUBE_MAP, texture.__webglTexture);
        }

        private void setupFrameBuffer(dynamic framebuffer, dynamic renderTarget, dynamic textureTarget)
        {
            _gl.bindFramebuffer(_gl.FRAMEBUFFER, framebuffer);
            _gl.framebufferTexture2D(_gl.FRAMEBUFFER, _gl.COLOR_ATTACHMENT0, textureTarget, renderTarget.__webglTexture, 0);
        }

        private void setupRenderBuffer(dynamic renderbuffer, dynamic renderTarget)
        {
            _gl.bindRenderbuffer(_gl.RENDERBUFFER, renderbuffer);

            if (renderTarget.depthBuffer && !renderTarget.stencilBuffer)
            {
                _gl.renderbufferStorage(_gl.RENDERBUFFER, _gl.DEPTH_COMPONENT16, renderTarget.width, renderTarget.height);
                _gl.framebufferRenderbuffer(_gl.FRAMEBUFFER, _gl.DEPTH_ATTACHMENT, _gl.RENDERBUFFER, renderbuffer);
            }
            else if (renderTarget.depthBuffer && renderTarget.stencilBuffer)
            {
                _gl.renderbufferStorage(_gl.RENDERBUFFER, _gl.DEPTH_STENCIL, renderTarget.width, renderTarget.height);
                _gl.framebufferRenderbuffer(_gl.FRAMEBUFFER, _gl.DEPTH_STENCIL_ATTACHMENT, _gl.RENDERBUFFER, renderbuffer);
            }
            else
            {
                _gl.renderbufferStorage(_gl.RENDERBUFFER, _gl.RGBA4, renderTarget.width, renderTarget.height);
            }
        }

        public void setRenderTarget(WebGLRenderTarget renderTarget)
        {
            var isCube = (renderTarget is WebGLRenderTargetCube);

            if (renderTarget != null && !JSObject.eval(renderTarget.__webglFramebuffer))
            {
                renderTarget.addEventListener("dispose", onRenderTargetDispose);

                renderTarget.__webglTexture = _gl.createTexture();

                info.memory.textures++;

                var isTargetPowerOfTwo = isPowerOfTwo(renderTarget.width) && isPowerOfTwo(renderTarget.height);
                var glFormat = paramThreeToGL(renderTarget.format);
                var glType = paramThreeToGL(renderTarget.type);

                if (isCube)
                {
                    renderTarget.__webglFramebuffer = new JSArray();
                    renderTarget.__webglRenderbuffer = new JSArray();

                    _gl.bindTexture(_gl.TEXTURE_CUBE_MAP, renderTarget.__webglTexture);
                    setTextureParameters(_gl.TEXTURE_CUBE_MAP, renderTarget, isTargetPowerOfTwo);

                    for (var i = 0; i < 6; i++)
                    {
                        renderTarget.__webglFramebuffer[i] = _gl.createFramebuffer();
                        renderTarget.__webglRenderbuffer[i] = _gl.createRenderbuffer();

                        _gl.texImage2D((uint)(_gl.TEXTURE_CUBE_MAP_POSITIVE_X + i), 0, glFormat, renderTarget.width, renderTarget.height, 0, glFormat, glType, null);

                        setupFrameBuffer(renderTarget.__webglFramebuffer[i], renderTarget, _gl.TEXTURE_CUBE_MAP_POSITIVE_X + i);
                        setupRenderBuffer(renderTarget.__webglRenderbuffer[i], renderTarget);
                    }

                    if (isTargetPowerOfTwo)
                    {
                        _gl.generateMipmap(_gl.TEXTURE_CUBE_MAP);
                    }
                }
                else
                {
                    renderTarget.__webglFramebuffer = _gl.createFramebuffer();

                    if (renderTarget.shareDepthFrom != null)
                    {
                        renderTarget.__webglRenderbuffer = renderTarget.shareDepthFrom.__webglRenderbuffer;
                    }
                    else
                    {
                        renderTarget.__webglRenderbuffer = _gl.createRenderbuffer();
                    }

                    _gl.bindTexture(_gl.TEXTURE_2D, renderTarget.__webglTexture);
                    setTextureParameters(_gl.TEXTURE_2D, renderTarget, isTargetPowerOfTwo);

                    _gl.texImage2D(_gl.TEXTURE_2D, 0, glFormat, renderTarget.width, renderTarget.height, 0, glFormat, glType, null);

                    setupFrameBuffer(renderTarget.__webglFramebuffer, renderTarget, _gl.TEXTURE_2D);

                    if (renderTarget.shareDepthFrom != null)
                    {
                        if (renderTarget.depthBuffer && !renderTarget.stencilBuffer)
                        {
                            _gl.framebufferRenderbuffer(_gl.FRAMEBUFFER, _gl.DEPTH_ATTACHMENT, _gl.RENDERBUFFER, renderTarget.__webglRenderbuffer);
                        }
                        else if (renderTarget.depthBuffer && renderTarget.stencilBuffer)
                        {
                            _gl.framebufferRenderbuffer(_gl.FRAMEBUFFER, _gl.DEPTH_STENCIL_ATTACHMENT, _gl.RENDERBUFFER, renderTarget.__webglRenderbuffer);
                        }
                    }
                    else
                    {
                        setupRenderBuffer(renderTarget.__webglRenderbuffer, renderTarget);
                    }

                    if (isTargetPowerOfTwo)
                    {
                        _gl.generateMipmap(_gl.TEXTURE_2D);
                    }
                }

                if (isCube)
                {
                    _gl.bindTexture(_gl.TEXTURE_CUBE_MAP, null);
                }
                else
                {
                    _gl.bindTexture(_gl.TEXTURE_2D, null);
                }

                _gl.bindRenderbuffer(_gl.RENDERBUFFER, null);
                _gl.bindFramebuffer(_gl.FRAMEBUFFER, null);
            }

            dynamic framebuffer;
            int width;
            int height;
            int vx;
            int vy;

            if (renderTarget != null)
            {
                if (isCube)
                {
                    framebuffer = renderTarget.__webglFramebuffer[renderTarget.activeCubeFace];
                }
                else
                {
                    framebuffer = renderTarget.__webglFramebuffer;
                }

                width = renderTarget.width;
                height = renderTarget.height;

                vx = 0;
                vy = 0;
            }
            else
            {
                framebuffer = null;

                width = _viewportWidth;
                height = _viewportHeight;

                vx = _viewportX;
                vy = _viewportY;
            }

            if (framebuffer != _currentFramebuffer)
            {
                _gl.bindFramebuffer(_gl.FRAMEBUFFER, framebuffer);
                _gl.viewport(vx, vy, width, height);

                _currentFramebuffer = framebuffer;
            }

            _currentWidth = width;
            _currentHeight = height;
        }

        private void updateRenderTargetMipmap(WebGLRenderTarget renderTarget)
        {
            if (renderTarget is WebGLRenderTargetCube)
            {
                _gl.bindTexture(_gl.TEXTURE_CUBE_MAP, renderTarget.__webglTexture);
                _gl.generateMipmap(_gl.TEXTURE_CUBE_MAP);
                _gl.bindTexture(_gl.TEXTURE_CUBE_MAP, null);
            }
            else
            {
                _gl.bindTexture(_gl.TEXTURE_2D, renderTarget.__webglTexture);
                _gl.generateMipmap(_gl.TEXTURE_2D);
                _gl.bindTexture(_gl.TEXTURE_2D, null);
            }
        }

        private uint filterFallback(int f)
        {
            if (f == THREE.NearestFilter || f == THREE.NearestMipMapNearestFilter || f == THREE.NearestMipMapLinearFilter)
            {
                return _gl.NEAREST;
            }

            return _gl.LINEAR;
        }

        private uint paramThreeToGL(int p)
        {
            if (p == THREE.RepeatWrapping)
            {
                return _gl.REPEAT;
            }
            if (p == THREE.ClampToEdgeWrapping)
            {
                return _gl.CLAMP_TO_EDGE;
            }
            if (p == THREE.MirroredRepeatWrapping)
            {
                return _gl.MIRRORED_REPEAT;
            }

            if (p == THREE.NearestFilter)
            {
                return _gl.NEAREST;
            }
            if (p == THREE.NearestMipMapNearestFilter)
            {
                return _gl.NEAREST_MIPMAP_NEAREST;
            }
            if (p == THREE.NearestMipMapLinearFilter)
            {
                return _gl.NEAREST_MIPMAP_LINEAR;
            }

            if (p == THREE.LinearFilter)
            {
                return _gl.LINEAR;
            }
            if (p == THREE.LinearMipMapNearestFilter)
            {
                return _gl.LINEAR_MIPMAP_NEAREST;
            }
            if (p == THREE.LinearMipMapLinearFilter)
            {
                return _gl.LINEAR_MIPMAP_LINEAR;
            }

            if (p == THREE.UnsignedByteType)
            {
                return _gl.UNSIGNED_BYTE;
            }
            if (p == THREE.UnsignedShort4444Type)
            {
                return _gl.UNSIGNED_SHORT_4_4_4_4;
            }
            if (p == THREE.UnsignedShort5551Type)
            {
                return _gl.UNSIGNED_SHORT_5_5_5_1;
            }
            if (p == THREE.UnsignedShort565Type)
            {
                return _gl.UNSIGNED_SHORT_5_6_5;
            }

            if (p == THREE.ByteType)
            {
                return _gl.BYTE;
            }
            if (p == THREE.ShortType)
            {
                return _gl.SHORT;
            }
            if (p == THREE.UnsignedShortType)
            {
                return _gl.UNSIGNED_SHORT;
            }
            if (p == THREE.IntType)
            {
                return _gl.INT;
            }
            if (p == THREE.UnsignedIntType)
            {
                return _gl.UNSIGNED_INT;
            }
            if (p == THREE.FloatType)
            {
                return _gl.FLOAT;
            }

            if (p == THREE.AlphaFormat)
            {
                return _gl.ALPHA;
            }
            if (p == THREE.RGBFormat)
            {
                return _gl.RGB;
            }
            if (p == THREE.RGBAFormat)
            {
                return _gl.RGBA;
            }
            if (p == THREE.LuminanceFormat)
            {
                return _gl.LUMINANCE;
            }
            if (p == THREE.LuminanceAlphaFormat)
            {
                return _gl.LUMINANCE_ALPHA;
            }

            if (p == THREE.AddEquation)
            {
                return _gl.FUNC_ADD;
            }
            if (p == THREE.SubtractEquation)
            {
                return _gl.FUNC_SUBTRACT;
            }
            if (p == THREE.ReverseSubtractEquation)
            {
                return _gl.FUNC_REVERSE_SUBTRACT;
            }

            if (p == THREE.ZeroFactor)
            {
                return _gl.ZERO;
            }
            if (p == THREE.OneFactor)
            {
                return _gl.ONE;
            }
            if (p == THREE.SrcColorFactor)
            {
                return _gl.SRC_COLOR;
            }
            if (p == THREE.OneMinusSrcColorFactor)
            {
                return _gl.ONE_MINUS_SRC_COLOR;
            }
            if (p == THREE.SrcAlphaFactor)
            {
                return _gl.SRC_ALPHA;
            }
            if (p == THREE.OneMinusSrcAlphaFactor)
            {
                return _gl.ONE_MINUS_SRC_ALPHA;
            }
            if (p == THREE.DstAlphaFactor)
            {
                return _gl.DST_ALPHA;
            }
            if (p == THREE.OneMinusDstAlphaFactor)
            {
                return _gl.ONE_MINUS_DST_ALPHA;
            }

            if (p == THREE.DstColorFactor)
            {
                return _gl.DST_COLOR;
            }
            if (p == THREE.OneMinusDstColorFactor)
            {
                return _gl.ONE_MINUS_DST_COLOR;
            }
            if (p == THREE.SrcAlphaSaturateFactor)
            {
                return _gl.SRC_ALPHA_SATURATE;
            }

            if (_glExtensionCompressedTextureS3TC != null)
            {
                if (p == THREE.RGB_S3TC_DXT1_Format)
                {
                    return _glExtensionCompressedTextureS3TC.COMPRESSED_RGB_S3TC_DXT1_EXT;
                }
                if (p == THREE.RGBA_S3TC_DXT1_Format)
                {
                    return _glExtensionCompressedTextureS3TC.COMPRESSED_RGBA_S3TC_DXT1_EXT;
                }
                if (p == THREE.RGBA_S3TC_DXT3_Format)
                {
                    return _glExtensionCompressedTextureS3TC.COMPRESSED_RGBA_S3TC_DXT3_EXT;
                }
                if (p == THREE.RGBA_S3TC_DXT5_Format)
                {
                    return _glExtensionCompressedTextureS3TC.COMPRESSED_RGBA_S3TC_DXT5_EXT;
                }
            }

            return 0;
        }

        private int allocateBones(Object3D @object)
        {
            var obj = @object as SkinnedMesh;

            if (_supportsBoneTextures && obj != null && obj.useVertexTexture)
            {
                return 1024;
            }

            var nVertexUniforms = _gl.getParameter(_gl.MAX_VERTEX_UNIFORM_VECTORS);
            var nVertexMatrices = System.Math.Floor((double)(nVertexUniforms - 20.0) / 4.0);
            var maxBones = nVertexMatrices;

            if (obj != null)
            {
                maxBones = System.Math.Min(obj.bones.length, maxBones);

                if (maxBones < obj.bones.length)
                {
                    JSConsole.warn(string.Format("WebGLRenderer: too many bones - {0}, this GPU supports just {1} (try OpenGL instead of ANGLE)", obj.bones.length, maxBones));
                }
            }

            return (int)maxBones;
        }

        private AllocatedLights allocateLights(IEnumerable<Light> lights)
        {
            var totals = new AllocatedLights();

            foreach (var light in lights.Where(light => !light.onlyShadow))
            {
                if (light is DirectionalLight)
                {
                    totals.directional++;
                }
                if (light is PointLight)
                {
                    totals.point++;
                }
                if (light is SpotLight)
                {
                    totals.spot++;
                }
                if (light is HemisphereLight)
                {
                    totals.hemi++;
                }
            }

            return totals;
        }

        private int allocateShadows(IEnumerable<Light> lights)
        {
            var maxShadows = 0;

            foreach (var light in lights.Where(light => JSObject.safe<bool>(light.castShadow)))
            {
                if (light is SpotLight)
                {
                    maxShadows++;
                }

                var directional = light as DirectionalLight;
                if (directional != null && !directional.shadowCascade)
                {
                    maxShadows++;
                }
            }

            return maxShadows;
        }

        private void initGL()
        {
            var attributes = new WebGLContextAttributes();
            attributes.setAlpha(_alpha);
            attributes.setPremultipliedAlpha(_premultipliedAlpha);
            attributes.setAntialias(_antialias);
            attributes.setStencil(_stencil);
            attributes.setPreserveDrawingBuffer(_preserveDrawingBuffer);

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
                JSConsole.log("THREE.WebGLRenderer: Float textures not supported.");
            }

            if (_glExtensionStandardDerivatives == null)
            {
                JSConsole.log("THREE.WebGLRenderer: Standard derivatives not supported.");
            }

            if (_glExtensionTextureFilterAnisotropic == null)
            {
                JSConsole.log("THREE.WebGLRenderer: Anisotropic texture filtering not supported.");
            }

            if (_glExtensionCompressedTextureS3TC == null)
            {
                JSConsole.log("THREE.WebGLRenderer: S3TC compressed textures not supported.");
            }
        }

        private void setDefaultGLState()
        {
            _gl.clearColor(0.0f, 0.0f, 0.0f, 1.0f);
            _gl.clearDepth(1.0f);
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
        }

        private struct AllocatedLights
        {
            public int directional;
            public int point;
            public int spot;
            public int hemi;
        }

        public class Info
        {
            public class Memory
            {
                public int programs;
                public int geometries;
                public int textures;
            }

            public class Render
            {
                public int calls;
                public int vertices;
                public int faces;
                public int points;
            }

            public Memory memory = new Memory();
            public Render render = new Render();
        }

        private static float[] toFloatArray(JSArray src, float[] dest = null)
        {
            dest = dest ?? new float[src.length];
            for (var index = 0; index < src.length; index++)
            {
                dest[index] = Convert.ToSingle(src[index]);
            }
            return dest;
        }

        private static int[] toInt32Array(JSArray src, int[] dest = null)
        {
            dest = dest ?? new int[src.length];
            for (var index = 0; index < src.length; index++)
            {
                dest[index] = Convert.ToInt32(src[index]);
            }
            return dest;
        }
    }

    // ReSharper restore MemberCanBeMadeStatic.Local
    // ReSharper restore InconsistentNaming
}