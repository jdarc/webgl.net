using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using THREE;
using WebGL;
using Image = WebGL.Image;
using Math = System.Math;

namespace Demo.Nehe
{
    public partial class Lesson16Form : Form
    {
        private const string per_fragment_lighting_fs =
            @"
precision mediump float;

varying vec2 vTextureCoord;
varying vec3 vTransformedNormal;
varying vec4 vPosition;

uniform vec3 uMaterialAmbientColor;
uniform vec3 uMaterialDiffuseColor;
uniform vec3 uMaterialSpecularColor;
uniform float uMaterialShininess;
uniform vec3 uMaterialEmissiveColor;

uniform bool uShowSpecularHighlights;
uniform bool uUseTextures;

uniform vec3 uAmbientLightingColor;

uniform vec3 uPointLightingLocation;
uniform vec3 uPointLightingDiffuseColor;
uniform vec3 uPointLightingSpecularColor;

uniform sampler2D uSampler;


void main(void) {
    vec3 ambientLightWeighting = uAmbientLightingColor;

    vec3 lightDirection = normalize(uPointLightingLocation - vPosition.xyz);
    vec3 normal = normalize(vTransformedNormal);

    vec3 specularLightWeighting = vec3(0.0, 0.0, 0.0);
    if (uShowSpecularHighlights) {
        vec3 eyeDirection = normalize(-vPosition.xyz);
        vec3 reflectionDirection = reflect(-lightDirection, normal);

        float specularLightBrightness = pow(max(dot(reflectionDirection, eyeDirection), 0.0), uMaterialShininess);
        specularLightWeighting = uPointLightingSpecularColor * specularLightBrightness;
    }

    float diffuseLightBrightness = max(dot(normal, lightDirection), 0.0);
    vec3 diffuseLightWeighting = uPointLightingDiffuseColor * diffuseLightBrightness;

    vec3 materialAmbientColor = uMaterialAmbientColor;
    vec3 materialDiffuseColor = uMaterialDiffuseColor;
    vec3 materialSpecularColor = uMaterialSpecularColor;
    vec3 materialEmissiveColor = uMaterialEmissiveColor;
    float alpha = 1.0;
    if (uUseTextures) {
        vec4 textureColor = texture2D(uSampler, vec2(vTextureCoord.s, vTextureCoord.t));
        materialAmbientColor = materialAmbientColor * textureColor.rgb;
        materialDiffuseColor = materialDiffuseColor * textureColor.rgb;
        materialEmissiveColor = materialEmissiveColor * textureColor.rgb;
        alpha = textureColor.a;
    }
    gl_FragColor = vec4(
        materialAmbientColor * ambientLightWeighting
        + materialDiffuseColor * diffuseLightWeighting
        + materialSpecularColor * specularLightWeighting
        + materialEmissiveColor,
        alpha
    );
}";

        private const string per_fragment_lighting_vs =
            @"
attribute vec3 aVertexPosition;
attribute vec3 aVertexNormal;
attribute vec2 aTextureCoord;

uniform mat4 uMVMatrix;
uniform mat4 uPMatrix;
uniform mat3 uNMatrix;

varying vec2 vTextureCoord;
varying vec3 vTransformedNormal;
varying vec4 vPosition;


void main(void) {
    vPosition = uMVMatrix * vec4(aVertexPosition, 1.0);
    gl_Position = uPMatrix * vPosition;
    vTextureCoord = aTextureCoord;
    vTransformedNormal = uNMatrix * aVertexNormal;
}";

        private WebGLRenderingContext gl;
        private WebGLProgram shaderProgram;
        private int shaderProgram_vertexPositionAttribute;
        private int shaderProgram_vertexNormalAttribute;
        private int shaderProgram_textureCoordAttribute;
        private WebGLUniformLocation shaderProgram_pMatrixUniform;
        private WebGLUniformLocation shaderProgram_mvMatrixUniform;
        private WebGLUniformLocation shaderProgram_nMatrixUniform;
        private WebGLUniformLocation shaderProgram_samplerUniform;
        private WebGLUniformLocation shaderProgram_materialAmbientColorUniform;
        private WebGLUniformLocation shaderProgram_materialDiffuseColorUniform;
        private WebGLUniformLocation shaderProgram_materialSpecularColorUniform;
        private WebGLUniformLocation shaderProgram_materialShininessUniform;
        private WebGLUniformLocation shaderProgram_materialEmissiveColorUniform;
        private WebGLUniformLocation shaderProgram_showSpecularHighlightsUniform;
        private WebGLUniformLocation shaderProgram_useTexturesUniform;
        private WebGLUniformLocation shaderProgram_ambientLightingColorUniform;
        private WebGLUniformLocation shaderProgram_pointLightingLocationUniform;
        private WebGLUniformLocation shaderProgram_pointLightingSpecularColorUniform;
        private WebGLUniformLocation shaderProgram_pointLightingDiffuseColorUniform;
        private WebGLTexture moonTexture;
        private WebGLTexture crateTexture;
        private Float32Array mvMatrix = mat4.create();
        private Stack<Float32Array> mvMatrixStack = new Stack<Float32Array>();
        private Float32Array pMatrix = mat4.create();
        private WebGLFramebuffer rttFramebuffer;
        private int rttFramebuffer_width;
        private int rttFramebuffer_height;
        private WebGLTexture rttTexture;
        private WebGLBuffer cubeVertexPositionBuffer;
        private float[] vertices;
        private int cubeVertexPositionBuffer_itemSize;
        private int cubeVertexPositionBuffer_numItems;
        private WebGLBuffer cubeVertexNormalBuffer;
        private int cubeVertexNormalBuffer_itemSize;
        private int cubeVertexNormalBuffer_numItems;
        private WebGLBuffer cubeVertexTextureCoordBuffer;
        private int cubeVertexTextureCoordBuffer_itemSize;
        private int cubeVertexTextureCoordBuffer_numItems;
        private WebGLBuffer cubeVertexIndexBuffer;
        private int cubeVertexIndexBuffer_itemSize;
        private int cubeVertexIndexBuffer_numItems;
        private WebGLBuffer moonVertexNormalBuffer;
        private int moonVertexNormalBuffer_itemSize;
        private int moonVertexNormalBuffer_numItems;
        private WebGLBuffer moonVertexTextureCoordBuffer;
        private int moonVertexTextureCoordBuffer_itemSize;
        private int moonVertexTextureCoordBuffer_numItems;
        private WebGLBuffer moonVertexPositionBuffer;
        private int moonVertexPositionBuffer_itemSize;
        private int moonVertexPositionBuffer_numItems;
        private WebGLBuffer moonVertexIndexBuffer;
        private int moonVertexIndexBuffer_itemSize;
        private int moonVertexIndexBuffer_numItems;
        private WebGLBuffer laptopScreenVertexPositionBuffer;
        private int laptopScreenVertexPositionBuffer_itemSize;
        private int laptopScreenVertexPositionBuffer_numItems;
        private WebGLBuffer laptopScreenVertexNormalBuffer;
        private int laptopScreenVertexNormalBuffer_itemSize;
        private int laptopScreenVertexNormalBuffer_numItems;
        private WebGLBuffer laptopScreenVertexTextureCoordBuffer;
        private int laptopScreenVertexTextureCoordBuffer_itemSize;
        private int laptopScreenVertexTextureCoordBuffer_numItems;
        private WebGLBuffer laptopVertexNormalBuffer;
        private int laptopVertexNormalBuffer_itemSize;
        private int laptopVertexNormalBuffer_numItems;
        private WebGLBuffer laptopVertexTextureCoordBuffer;
        private int laptopVertexTextureCoordBuffer_itemSize;
        private int laptopVertexTextureCoordBuffer_numItems;
        private WebGLBuffer laptopVertexPositionBuffer;
        private int laptopVertexPositionBuffer_itemSize;
        private int laptopVertexPositionBuffer_numItems;
        private WebGLBuffer laptopVertexIndexBuffer;
        private int laptopVertexIndexBuffer_itemSize;
        private int laptopVertexIndexBuffer_numItems;
        private float laptopScreenAspectRatio = 1.66f;
        private float moonAngle = 180;
        private float cubeAngle;
        private float laptopAngle;
        private int gl_viewportWidth;
        private int gl_viewportHeight;
        private int lastTime;

        public Lesson16Form()
        {
            InitializeComponent();

            ClientSize = new Size(1440, 900);

            webGLStart();
        }

        private void initGL(UserControl canvas)
        {
            gl = (WebGLRenderingContext)new HTMLCanvasElement(canvas).getContext("webgl");
            gl_viewportWidth = canvas.Width;
            gl_viewportHeight = canvas.Height;
        }

        private static WebGLShader getShader(WebGLRenderingContext gl, string type, string script)
        {
            if (script == null)
            {
                return null;
            }

            WebGLShader shader;
            if (type == "x-shader/x-fragment")
            {
                shader = gl.createShader(gl.FRAGMENT_SHADER);
            }
            else if (type == "x-shader/x-vertex")
            {
                shader = gl.createShader(gl.VERTEX_SHADER);
            }
            else
            {
                return null;
            }

            gl.shaderSource(shader, script);
            gl.compileShader(shader);

            if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS))
            {
                alert(gl.getShaderInfoLog(shader));
                return null;
            }

            return shader;
        }

        private void initShaders()
        {
            var fragmentShader = getShader(gl, "x-shader/x-fragment", per_fragment_lighting_fs);
            var vertexShader = getShader(gl, "x-shader/x-vertex", per_fragment_lighting_vs);

            shaderProgram = gl.createProgram();
            gl.attachShader(shaderProgram, vertexShader);
            gl.attachShader(shaderProgram, fragmentShader);
            gl.linkProgram(shaderProgram);

            if (!gl.getProgramParameter(shaderProgram, gl.LINK_STATUS))
            {
                alert("Could not initialise shaders");
            }

            gl.useProgram(shaderProgram);

            shaderProgram_vertexPositionAttribute = gl.getAttribLocation(shaderProgram, "aVertexPosition");
            gl.enableVertexAttribArray((uint)shaderProgram_vertexPositionAttribute);

            shaderProgram_vertexNormalAttribute = gl.getAttribLocation(shaderProgram, "aVertexNormal");
            gl.enableVertexAttribArray((uint)shaderProgram_vertexNormalAttribute);

            shaderProgram_textureCoordAttribute = gl.getAttribLocation(shaderProgram, "aTextureCoord");
            gl.enableVertexAttribArray((uint)shaderProgram_textureCoordAttribute);

            shaderProgram_pMatrixUniform = gl.getUniformLocation(shaderProgram, "uPMatrix");
            shaderProgram_mvMatrixUniform = gl.getUniformLocation(shaderProgram, "uMVMatrix");
            shaderProgram_nMatrixUniform = gl.getUniformLocation(shaderProgram, "uNMatrix");
            shaderProgram_samplerUniform = gl.getUniformLocation(shaderProgram, "uSampler");

            shaderProgram_materialAmbientColorUniform = gl.getUniformLocation(shaderProgram, "uMaterialAmbientColor");
            shaderProgram_materialDiffuseColorUniform = gl.getUniformLocation(shaderProgram, "uMaterialDiffuseColor");
            shaderProgram_materialSpecularColorUniform = gl.getUniformLocation(shaderProgram, "uMaterialSpecularColor");
            shaderProgram_materialShininessUniform = gl.getUniformLocation(shaderProgram, "uMaterialShininess");
            shaderProgram_materialEmissiveColorUniform = gl.getUniformLocation(shaderProgram, "uMaterialEmissiveColor");
            shaderProgram_showSpecularHighlightsUniform = gl.getUniformLocation(shaderProgram, "uShowSpecularHighlights");
            shaderProgram_useTexturesUniform = gl.getUniformLocation(shaderProgram, "uUseTextures");
            shaderProgram_ambientLightingColorUniform = gl.getUniformLocation(shaderProgram, "uAmbientLightingColor");
            shaderProgram_pointLightingLocationUniform = gl.getUniformLocation(shaderProgram, "uPointLightingLocation");
            shaderProgram_pointLightingSpecularColorUniform = gl.getUniformLocation(shaderProgram, "uPointLightingSpecularColor");
            shaderProgram_pointLightingDiffuseColorUniform = gl.getUniformLocation(shaderProgram, "uPointLightingDiffuseColor");
        }

        private void handleLoadedTexture(WebGLTexture texture, Image image)
        {
            gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, 1);
            gl.bindTexture(gl.TEXTURE_2D, texture);
            gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, image.imageData);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, (int)gl.LINEAR);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, (int)gl.LINEAR_MIPMAP_NEAREST);
            gl.generateMipmap(gl.TEXTURE_2D);

            gl.bindTexture(gl.TEXTURE_2D, null);
        }

        private void initTextures()
        {
            moonTexture = gl.createTexture();
            handleLoadedTexture(moonTexture, new Image {src = "textures/moon.gif"});

            crateTexture = gl.createTexture();
            handleLoadedTexture(crateTexture, new Image {src = "textures/crate.gif"});
        }

        private void mvPushMatrix()
        {
            var copy = mat4.create();
            mat4.set(mvMatrix, copy);
            mvMatrixStack.Push(copy);
        }

        private void mvPopMatrix()
        {
            if (mvMatrixStack.Count == 0)
            {
                throw new ApplicationException("Invalid popMatrix!");
            }
            mvMatrix = mvMatrixStack.Pop();
        }

        private void setMatrixUniforms()
        {
            gl.uniformMatrix4fv(shaderProgram_pMatrixUniform, false, pMatrix);
            gl.uniformMatrix4fv(shaderProgram_mvMatrixUniform, false, mvMatrix);

            var normalMatrix = mat3.create();
            mat4.toInverseMat3(mvMatrix, normalMatrix);
            mat3.transpose(normalMatrix);
            gl.uniformMatrix3fv(shaderProgram_nMatrixUniform, false, normalMatrix);
        }

        private static float degToRad(double degrees)
        {
            return (float)(degrees * Math.PI / 180);
        }

        private void initTextureFramebuffer()
        {
            rttFramebuffer = gl.createFramebuffer();
            gl.bindFramebuffer(gl.FRAMEBUFFER, rttFramebuffer);
            rttFramebuffer_width = 512;
            rttFramebuffer_height = 512;

            rttTexture = gl.createTexture();
            gl.bindTexture(gl.TEXTURE_2D, rttTexture);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, (int)gl.LINEAR);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, (int)gl.LINEAR_MIPMAP_NEAREST);
//            gl.generateMipmap(gl.TEXTURE_2D);

            gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, rttFramebuffer_width, rttFramebuffer_height, 0, gl.RGBA, gl.UNSIGNED_BYTE, null);

            var renderbuffer = gl.createRenderbuffer();
            gl.bindRenderbuffer(gl.RENDERBUFFER, renderbuffer);
            gl.renderbufferStorage(gl.RENDERBUFFER, gl.DEPTH_COMPONENT16, rttFramebuffer_width, rttFramebuffer_height);

            gl.framebufferTexture2D(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.TEXTURE_2D, rttTexture, 0);
            gl.framebufferRenderbuffer(gl.FRAMEBUFFER, gl.DEPTH_ATTACHMENT, gl.RENDERBUFFER, renderbuffer);

            gl.bindTexture(gl.TEXTURE_2D, null);
            gl.bindRenderbuffer(gl.RENDERBUFFER, null);
            gl.bindFramebuffer(gl.FRAMEBUFFER, null);
        }

        private void initBuffers()
        {
            cubeVertexPositionBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, cubeVertexPositionBuffer);
            vertices = new[]
                       {
                           // Front face
                           -1.0f, -1.0f, 1.0f,
                           1.0f, -1.0f, 1.0f,
                           1.0f, 1.0f, 1.0f,
                           -1.0f, 1.0f, 1.0f,
                           // Back face
                           -1.0f, -1.0f, -1.0f,
                           -1.0f, 1.0f, -1.0f,
                           1.0f, 1.0f, -1.0f,
                           1.0f, -1.0f, -1.0f,
                           // Top face
                           -1.0f, 1.0f, -1.0f,
                           -1.0f, 1.0f, 1.0f,
                           1.0f, 1.0f, 1.0f,
                           1.0f, 1.0f, -1.0f,
                           // Bottom face
                           -1.0f, -1.0f, -1.0f,
                           1.0f, -1.0f, -1.0f,
                           1.0f, -1.0f, 1.0f,
                           -1.0f, -1.0f, 1.0f,
                           // Right face
                           1.0f, -1.0f, -1.0f,
                           1.0f, 1.0f, -1.0f,
                           1.0f, 1.0f, 1.0f,
                           1.0f, -1.0f, 1.0f,
                           // Left face
                           -1.0f, -1.0f, -1.0f,
                           -1.0f, -1.0f, 1.0f,
                           -1.0f, 1.0f, 1.0f,
                           -1.0f, 1.0f, -1.0f,
                       };

            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertices), gl.STATIC_DRAW);
            cubeVertexPositionBuffer_itemSize = 3;
            cubeVertexPositionBuffer_numItems = 24;

            cubeVertexNormalBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, cubeVertexNormalBuffer);
            var vertexNormals = new[]
                                {
                                    // Front face
                                    0.0f, 0.0f, 1.0f,
                                    0.0f, 0.0f, 1.0f,
                                    0.0f, 0.0f, 1.0f,
                                    0.0f, 0.0f, 1.0f,
                                    // Back face
                                    0.0f, 0.0f, -1.0f,
                                    0.0f, 0.0f, -1.0f,
                                    0.0f, 0.0f, -1.0f,
                                    0.0f, 0.0f, -1.0f,
                                    // Top face
                                    0.0f, 1.0f, 0.0f,
                                    0.0f, 1.0f, 0.0f,
                                    0.0f, 1.0f, 0.0f,
                                    0.0f, 1.0f, 0.0f,
                                    // Bottom face
                                    0.0f, -1.0f, 0.0f,
                                    0.0f, -1.0f, 0.0f,
                                    0.0f, -1.0f, 0.0f,
                                    0.0f, -1.0f, 0.0f,
                                    // Right face
                                    1.0f, 0.0f, 0.0f,
                                    1.0f, 0.0f, 0.0f,
                                    1.0f, 0.0f, 0.0f,
                                    1.0f, 0.0f, 0.0f,
                                    // Left face
                                    -1.0f, 0.0f, 0.0f,
                                    -1.0f, 0.0f, 0.0f,
                                    -1.0f, 0.0f, 0.0f,
                                    -1.0f, 0.0f, 0.0f,
                                };

            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertexNormals), gl.STATIC_DRAW);
            cubeVertexNormalBuffer_itemSize = 3;
            cubeVertexNormalBuffer_numItems = 24;

            cubeVertexTextureCoordBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, cubeVertexTextureCoordBuffer);
            var textureCoords = new[]
                                {
                                    // Front face
                                    0.0f, 0.0f,
                                    1.0f, 0.0f,
                                    1.0f, 1.0f,
                                    0.0f, 1.0f,
                                    // Back face
                                    1.0f, 0.0f,
                                    1.0f, 1.0f,
                                    0.0f, 1.0f,
                                    0.0f, 0.0f,
                                    // Top face
                                    0.0f, 1.0f,
                                    0.0f, 0.0f,
                                    1.0f, 0.0f,
                                    1.0f, 1.0f,
                                    // Bottom face
                                    1.0f, 1.0f,
                                    0.0f, 1.0f,
                                    0.0f, 0.0f,
                                    1.0f, 0.0f,
                                    // Right face
                                    1.0f, 0.0f,
                                    1.0f, 1.0f,
                                    0.0f, 1.0f,
                                    0.0f, 0.0f,
                                    // Left face
                                    0.0f, 0.0f,
                                    1.0f, 0.0f,
                                    1.0f, 1.0f,
                                    0.0f, 1.0f
                                };
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(textureCoords), gl.STATIC_DRAW);
            cubeVertexTextureCoordBuffer_itemSize = 2;
            cubeVertexTextureCoordBuffer_numItems = 24;

            cubeVertexIndexBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, cubeVertexIndexBuffer);
            var cubeVertexIndices = new ushort[]
                                    {
                                        0, 1, 2, 0, 2, 3,
                                        4, 5, 6, 4, 6, 7,
                                        8, 9, 10, 8, 10, 11,
                                        12, 13, 14, 12, 14, 15,
                                        16, 17, 18, 16, 18, 19,
                                        20, 21, 22, 20, 22, 23
                                    };
            gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, new Uint16Array(cubeVertexIndices), gl.STATIC_DRAW);
            cubeVertexIndexBuffer_itemSize = 1;
            cubeVertexIndexBuffer_numItems = 36;

            const int latitudeBands = 30;
            const int longitudeBands = 30;
            const float radius = 1f;

            var vertexPositionData = new List<float>();
            var normalData = new List<float>();
            var textureCoordData = new List<float>();
            for (var latNumber = 0; latNumber <= latitudeBands; latNumber++)
            {
                var theta = latNumber * Math.PI / latitudeBands;
                var sinTheta = Math.Sin(theta);
                var cosTheta = Math.Cos(theta);

                for (var longNumber = 0; longNumber <= longitudeBands; longNumber++)
                {
                    var phi = longNumber * 2 * Math.PI / longitudeBands;
                    var sinPhi = Math.Sin(phi);
                    var cosPhi = Math.Cos(phi);

                    var x = cosPhi * sinTheta;
                    var y = cosTheta;
                    var z = sinPhi * sinTheta;
                    var u = 1f - (longNumber / (float)longitudeBands);
                    var v = 1f - (latNumber / (float)latitudeBands);

                    normalData.Add((float)x);
                    normalData.Add((float)y);
                    normalData.Add((float)z);
                    textureCoordData.Add(u);
                    textureCoordData.Add(v);
                    vertexPositionData.Add((float)(radius * x));
                    vertexPositionData.Add((float)(radius * y));
                    vertexPositionData.Add((float)(radius * z));
                }
            }

            var indexData = new List<ushort>();
            for (var latNumber = 0; latNumber < latitudeBands; latNumber++)
            {
                for (var longNumber = 0; longNumber < longitudeBands; longNumber++)
                {
                    var first = (latNumber * (longitudeBands + 1)) + longNumber;
                    var second = first + longitudeBands + 1;
                    indexData.Add((ushort)first);
                    indexData.Add((ushort)second);
                    indexData.Add((ushort)(first + 1));

                    indexData.Add((ushort)second);
                    indexData.Add((ushort)(second + 1));
                    indexData.Add((ushort)(first + 1));
                }
            }

            moonVertexNormalBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, moonVertexNormalBuffer);
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(normalData.ToArray()), gl.STATIC_DRAW);
            moonVertexNormalBuffer_itemSize = 3;
            moonVertexNormalBuffer_numItems = normalData.Count / 3;

            moonVertexTextureCoordBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, moonVertexTextureCoordBuffer);
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(textureCoordData.ToArray()), gl.STATIC_DRAW);
            moonVertexTextureCoordBuffer_itemSize = 2;
            moonVertexTextureCoordBuffer_numItems = textureCoordData.Count / 2;

            moonVertexPositionBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, moonVertexPositionBuffer);
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertexPositionData.ToArray()), gl.STATIC_DRAW);
            moonVertexPositionBuffer_itemSize = 3;
            moonVertexPositionBuffer_numItems = vertexPositionData.Count / 3;

            moonVertexIndexBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, moonVertexIndexBuffer);
            gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, new Uint16Array(indexData.ToArray()), gl.STREAM_DRAW);
            moonVertexIndexBuffer_itemSize = 1;
            moonVertexIndexBuffer_numItems = indexData.Count;

            laptopScreenVertexPositionBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, laptopScreenVertexPositionBuffer);
            vertices = new[]
                       {
                           0.580687f, 0.659f, 0.813106f,
                           -0.580687f, 0.659f, 0.813107f,
                           0.580687f, 0.472f, 0.113121f,
                           -0.580687f, 0.472f, 0.113121f,
                       };
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertices), gl.STATIC_DRAW);
            laptopScreenVertexPositionBuffer_itemSize = 3;
            laptopScreenVertexPositionBuffer_numItems = 4;

            laptopScreenVertexNormalBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, laptopScreenVertexNormalBuffer);
            vertexNormals = new[]
                            {
                                0.000000f, -0.965926f, 0.258819f,
                                0.000000f, -0.965926f, 0.258819f,
                                0.000000f, -0.965926f, 0.258819f,
                                0.000000f, -0.965926f, 0.258819f,
                            };
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(vertexNormals), gl.STATIC_DRAW);
            laptopScreenVertexNormalBuffer_itemSize = 3;
            laptopScreenVertexNormalBuffer_numItems = 4;

            laptopScreenVertexTextureCoordBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, laptopScreenVertexTextureCoordBuffer);
            textureCoords = new[]
                            {
                                1.0f, 1.0f,
                                0.0f, 1.0f,
                                1.0f, 0.0f,
                                0.0f, 0.0f,
                            };
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(textureCoords), gl.STATIC_DRAW);
            laptopScreenVertexTextureCoordBuffer_itemSize = 2;
            laptopScreenVertexTextureCoordBuffer_numItems = 4;
        }

        private void handleLoadedLaptop(dynamic laptopData)
        {
            laptopVertexNormalBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, laptopVertexNormalBuffer);
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(toFloat32Array(laptopData.vertexNormals)), gl.STATIC_DRAW);
            laptopVertexNormalBuffer_itemSize = 3;
            laptopVertexNormalBuffer_numItems = laptopData.vertexNormals.length / 3;

            laptopVertexTextureCoordBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, laptopVertexTextureCoordBuffer);
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(toFloat32Array(laptopData.vertexTextureCoords)), gl.STATIC_DRAW);
            laptopVertexTextureCoordBuffer_itemSize = 2;
            laptopVertexTextureCoordBuffer_numItems = laptopData.vertexTextureCoords.length / 2;

            laptopVertexPositionBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, laptopVertexPositionBuffer);
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(toFloat32Array(laptopData.vertexPositions)), gl.STATIC_DRAW);
            laptopVertexPositionBuffer_itemSize = 3;
            laptopVertexPositionBuffer_numItems = laptopData.vertexPositions.length / 3;

            laptopVertexIndexBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, laptopVertexIndexBuffer);
            gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, new Uint16Array(toUint16Array(laptopData.indices)), gl.STREAM_DRAW);
            laptopVertexIndexBuffer_itemSize = 1;
            laptopVertexIndexBuffer_numItems = laptopData.indices.length;
        }

        private void loadLaptop()
        {
            var request = new XMLHttpRequest();
            request.open("GET", "models/macbook.json");
            request.onreadystatechange = () =>
                                         {
                                             if (request.readyState == 4)
                                             {
                                                 handleLoadedLaptop(JSON.parse(request.responseText));
                                             }
                                         };
            request.send();
        }

        private void drawSceneOnLaptopScreen()
        {
            gl.viewport(0, 0, rttFramebuffer_width, rttFramebuffer_height);
            gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);

            mat4.perspective(45, laptopScreenAspectRatio, 0.1f, 100.0f, pMatrix);

            gl.uniform1i(shaderProgram_showSpecularHighlightsUniform, Convert.ToInt32(false));
            gl.uniform3f(shaderProgram_ambientLightingColorUniform, 0.2f, 0.2f, 0.2f);
            gl.uniform3f(shaderProgram_pointLightingLocationUniform, 0, 0, -5);
            gl.uniform3f(shaderProgram_pointLightingDiffuseColorUniform, 0.8f, 0.8f, 0.8f);

            gl.uniform1i(shaderProgram_showSpecularHighlightsUniform, Convert.ToInt32(false));
            gl.uniform1i(shaderProgram_useTexturesUniform, Convert.ToInt32(true));

            gl.uniform3f(shaderProgram_materialAmbientColorUniform, 1.0f, 1.0f, 1.0f);
            gl.uniform3f(shaderProgram_materialDiffuseColorUniform, 1.0f, 1.0f, 1.0f);
            gl.uniform3f(shaderProgram_materialSpecularColorUniform, 0.0f, 0.0f, 0.0f);
            gl.uniform1f(shaderProgram_materialShininessUniform, 0);
            gl.uniform3f(shaderProgram_materialEmissiveColorUniform, 0.0f, 0.0f, 0.0f);

            mat4.identity(mvMatrix);

            mat4.translate(mvMatrix, new float[] {0, 0, -5});
            mat4.rotate(mvMatrix, degToRad(30), new float[] {1, 0, 0});

            mvPushMatrix();
            mat4.rotate(mvMatrix, degToRad(moonAngle), new float[] {0, 1, 0});
            mat4.translate(mvMatrix, new float[] {2, 0, 0});
            gl.activeTexture(gl.TEXTURE0);
            gl.bindTexture(gl.TEXTURE_2D, moonTexture);
            gl.uniform1i(shaderProgram_samplerUniform, 0);

            gl.bindBuffer(gl.ARRAY_BUFFER, moonVertexPositionBuffer);
            gl.vertexAttribPointer((uint)shaderProgram_vertexPositionAttribute, moonVertexPositionBuffer_itemSize, gl.FLOAT, false, 0, 0);

            gl.bindBuffer(gl.ARRAY_BUFFER, moonVertexTextureCoordBuffer);
            gl.vertexAttribPointer((uint)shaderProgram_textureCoordAttribute, moonVertexTextureCoordBuffer_itemSize, gl.FLOAT, false, 0, 0);

            gl.bindBuffer(gl.ARRAY_BUFFER, moonVertexNormalBuffer);
            gl.vertexAttribPointer((uint)shaderProgram_vertexNormalAttribute, moonVertexNormalBuffer_itemSize, gl.FLOAT, false, 0, 0);

            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, moonVertexIndexBuffer);
            setMatrixUniforms();
            gl.drawElements(gl.TRIANGLES, moonVertexIndexBuffer_numItems, gl.UNSIGNED_SHORT, 0);
            mvPopMatrix();

            mvPushMatrix();
            mat4.rotate(mvMatrix, degToRad(cubeAngle), new float[] {0, 1, 0});
            mat4.translate(mvMatrix, new[] {1.25f, 0, 0});
            gl.bindBuffer(gl.ARRAY_BUFFER, cubeVertexPositionBuffer);
            gl.vertexAttribPointer((uint)shaderProgram_vertexPositionAttribute, cubeVertexPositionBuffer_itemSize, gl.FLOAT, false, 0, 0);

            gl.bindBuffer(gl.ARRAY_BUFFER, cubeVertexNormalBuffer);
            gl.vertexAttribPointer((uint)shaderProgram_vertexNormalAttribute, cubeVertexNormalBuffer_itemSize, gl.FLOAT, false, 0, 0);

            gl.bindBuffer(gl.ARRAY_BUFFER, cubeVertexTextureCoordBuffer);
            gl.vertexAttribPointer((uint)shaderProgram_textureCoordAttribute, cubeVertexTextureCoordBuffer_itemSize, gl.FLOAT, false, 0, 0);

            gl.activeTexture(gl.TEXTURE0);
            gl.bindTexture(gl.TEXTURE_2D, crateTexture);
            gl.uniform1i(shaderProgram_samplerUniform, 0);

            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, cubeVertexIndexBuffer);
            setMatrixUniforms();
            gl.drawElements(gl.TRIANGLES, cubeVertexIndexBuffer_numItems, gl.UNSIGNED_SHORT, 0);
            mvPopMatrix();

            gl.bindTexture(gl.TEXTURE_2D, rttTexture);
            gl.generateMipmap(gl.TEXTURE_2D);
            gl.bindTexture(gl.TEXTURE_2D, null);
        }

        private void drawScene()
        {
            gl.bindFramebuffer(gl.FRAMEBUFFER, rttFramebuffer);
            drawSceneOnLaptopScreen();

            gl.bindFramebuffer(gl.FRAMEBUFFER, null);

            gl.viewport(0, 0, gl_viewportWidth, gl_viewportHeight);
            gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);

            mat4.perspective(45, gl_viewportWidth / (float)gl_viewportHeight, 0.1f, 100.0f, pMatrix);

            mat4.identity(mvMatrix);

            mvPushMatrix();

            mat4.translate(mvMatrix, new[] {0, -0.4f, -2.2f});
            mat4.rotate(mvMatrix, degToRad(laptopAngle), new float[] {0, 1, 0});
            mat4.rotate(mvMatrix, degToRad(-90), new float[] {1, 0, 0});

            gl.uniform1i(shaderProgram_showSpecularHighlightsUniform, Convert.ToInt32(true));
            gl.uniform3f(shaderProgram_pointLightingLocationUniform, -1, 2, -1);

            gl.uniform3f(shaderProgram_ambientLightingColorUniform, 0.2f, 0.2f, 0.2f);
            gl.uniform3f(shaderProgram_pointLightingDiffuseColorUniform, 0.8f, 0.8f, 0.8f);
            gl.uniform3f(shaderProgram_pointLightingSpecularColorUniform, 0.8f, 0.8f, 0.8f);

            // The laptop body is quite shiny and has no texture.  It reflects lots of specular light
            gl.uniform3f(shaderProgram_materialAmbientColorUniform, 1.0f, 1.0f, 1.0f);
            gl.uniform3f(shaderProgram_materialDiffuseColorUniform, 1.0f, 1.0f, 1.0f);
            gl.uniform3f(shaderProgram_materialSpecularColorUniform, 1.5f, 1.5f, 1.5f);
            gl.uniform1f(shaderProgram_materialShininessUniform, 5);
            gl.uniform3f(shaderProgram_materialEmissiveColorUniform, 0.0f, 0.0f, 0.0f);
            gl.uniform1i(shaderProgram_useTexturesUniform, Convert.ToInt32(false));

            if (laptopVertexPositionBuffer != null)
            {
                gl.bindBuffer(gl.ARRAY_BUFFER, laptopVertexPositionBuffer);
                gl.vertexAttribPointer((uint)shaderProgram_vertexPositionAttribute, laptopVertexPositionBuffer_itemSize, gl.FLOAT, false, 0, 0);

                gl.bindBuffer(gl.ARRAY_BUFFER, laptopVertexTextureCoordBuffer);
                gl.vertexAttribPointer((uint)shaderProgram_textureCoordAttribute, laptopVertexTextureCoordBuffer_itemSize, gl.FLOAT, false, 0, 0);

                gl.bindBuffer(gl.ARRAY_BUFFER, laptopVertexNormalBuffer);
                gl.vertexAttribPointer((uint)shaderProgram_vertexNormalAttribute, laptopVertexNormalBuffer_itemSize, gl.FLOAT, false, 0, 0);

                gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, laptopVertexIndexBuffer);
                setMatrixUniforms();
                gl.drawElements(gl.TRIANGLES, laptopVertexIndexBuffer_numItems, gl.UNSIGNED_SHORT, 0);
            }

            gl.uniform3f(shaderProgram_materialAmbientColorUniform, 0.0f, 0.0f, 0.0f);
            gl.uniform3f(shaderProgram_materialDiffuseColorUniform, 0.0f, 0.0f, 0.0f);
            gl.uniform3f(shaderProgram_materialSpecularColorUniform, 0.5f, 0.5f, 0.5f);
            gl.uniform1f(shaderProgram_materialShininessUniform, 20);
            gl.uniform3f(shaderProgram_materialEmissiveColorUniform, 1.5f, 1.5f, 1.5f);
            gl.uniform1i(shaderProgram_useTexturesUniform, Convert.ToInt32(true));

            gl.bindBuffer(gl.ARRAY_BUFFER, laptopScreenVertexPositionBuffer);
            gl.vertexAttribPointer((uint)shaderProgram_vertexPositionAttribute, laptopScreenVertexPositionBuffer_itemSize, gl.FLOAT, false, 0, 0);

            gl.bindBuffer(gl.ARRAY_BUFFER, laptopScreenVertexNormalBuffer);
            gl.vertexAttribPointer((uint)shaderProgram_vertexNormalAttribute, laptopScreenVertexNormalBuffer_itemSize, gl.FLOAT, false, 0, 0);

            gl.bindBuffer(gl.ARRAY_BUFFER, laptopScreenVertexTextureCoordBuffer);
            gl.vertexAttribPointer((uint)shaderProgram_textureCoordAttribute, laptopScreenVertexTextureCoordBuffer_itemSize, gl.FLOAT, false, 0, 0);

            gl.activeTexture(gl.TEXTURE0);
            gl.bindTexture(gl.TEXTURE_2D, rttTexture);
            gl.uniform1i(shaderProgram_samplerUniform, 0);

            setMatrixUniforms();
            gl.drawArrays(gl.TRIANGLE_STRIP, 0, laptopScreenVertexPositionBuffer_numItems);

            mvPopMatrix();
        }

        private void animate()
        {
            var timeNow = Environment.TickCount;
            if (lastTime != 0)
            {
                var elapsed = timeNow - lastTime;

                moonAngle += 0.05f * elapsed;
                cubeAngle += 0.05f * elapsed;

                laptopAngle -= 0.005f * elapsed;
            }
            lastTime = timeNow;
        }

        private void tick()
        {
            gl.swapBuffers();
            drawScene();
            animate();
        }

        private void webGLStart()
        {
            initGL(canvas);
            initTextureFramebuffer();
            initShaders();
            initBuffers();
            initTextures();
            loadLaptop();

            gl.clearColor(0.0f, 0.0f, 0.0f, 1.0f);
            gl.enable(gl.DEPTH_TEST);
        }

        private static void alert(string msg)
        {
            MessageBox.Show(msg);
        }

        private Uint16Array toUint16Array(JSArray array)
        {
            var result = new Uint16Array(array.length);
            for (var i = 0; i < array.length; i++)
            {
                result[i] = (ushort)array[i];
            }
            return result;
        }

        private Float32Array toFloat32Array(JSArray array)
        {
            var result = new Float32Array(array.length);
            for (var i = 0; i < array.length; i++)
            {
                result[i] = (float)array[i];
            }
            return result;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            tick();
        }

        private void MainFormActivated(object sender, EventArgs e)
        {
            timer.Enabled = true;
        }

        private void MainFormDeactivate(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }
    }
}