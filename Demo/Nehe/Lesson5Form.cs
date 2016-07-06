using System;
using System.Drawing;
using System.Windows.Forms;
using WebGL;
using Image = WebGL.Image;

namespace Demo.Nehe
{
    public partial class Lesson5Form : Form
    {
        private const string Fragment =
            @"precision mediump float;

            varying vec2 vTextureCoord;

            uniform sampler2D uSampler;

            void main(void) {
                gl_FragColor = texture2D(uSampler, vec2(vTextureCoord.s, vTextureCoord.t));
            }";

        private const string Vertex =
            @"attribute vec3 aVertexPosition;
            attribute vec2 aTextureCoord;

            uniform mat4 uMVMatrix;
            uniform mat4 uPMatrix;

            varying vec2 vTextureCoord;

            void main(void) {
                gl_Position = uPMatrix * uMVMatrix * vec4(aVertexPosition, 1.0);
                vTextureCoord = aTextureCoord;
            }";

        private WebGLRenderingContext _gl;
        private WebGLProgram _shaderProgram;
        private int _shaderProgramVertexPositionAttribute;
        private WebGLUniformLocation _shaderProgramPMatrixUniform;
        private WebGLUniformLocation _shaderProgramMvMatrixUniform;
        private int _lastTime;
        private float _xRot;
        private float _yRot;
        private float _zRot;
        private readonly Float32Array _mvMatrix = mat4.create();
        private readonly Float32Array _pMatrix = mat4.create();
        private WebGLBuffer _cubeVertexPositionBuffer;
        private int _cubeVertexPositionBufferItemSize;
        private int _cubeVertexPositionBufferNumItems;
        private WebGLBuffer _cubeVertexIndexBuffer;
        private int _cubeVertexIndexBufferNumItems;
        private int _shaderProgramTextureCoordAttribute;
        private WebGLUniformLocation _shaderProgramSamplerUniform;
        private WebGLBuffer _cubeVertexTextureCoordBuffer;
        private int _cubeVertexTextureCoordBufferItemSize;
        private WebGLTexture _neheTexture;
        private int _cubeVertexTextureCoordBufferNumItems;
        private int _cubeVertexIndexBufferItemSize;

        public Lesson5Form()
        {
            InitializeComponent();

            ClientSize = new Size(1440, 900);

            InitGL();
            InitShaders();
            InitBuffers();
            InitTexture();

            _gl.clearColor(0.0f, 0.0f, 0.0f, 1.0f);
            _gl.enable(_gl.DEPTH_TEST);
            _gl.enable(_gl.CULL_FACE);
            _gl.cullFace(_gl.BACK);
        }

        private void InitTexture()
        {
            var image = new Image {src = "textures/crate.jpg"};

            var glExtensionTextureFilterAnisotropic = (WebGLExtension)(_gl.getExtension("EXT_texture_filter_anisotropic") ??
                                                                       _gl.getExtension("MOZ_EXT_texture_filter_anisotropic") ??
                                                                       _gl.getExtension("WEBKIT_EXT_texture_filter_anisotropic"));

            _neheTexture = _gl.createTexture();
            _gl.bindTexture(_gl.TEXTURE_2D, _neheTexture);
            _gl.pixelStorei(_gl.UNPACK_FLIP_Y_WEBGL, 1);
            _gl.texImage2D(_gl.TEXTURE_2D, 0, _gl.RGBA, _gl.RGBA, _gl.UNSIGNED_BYTE, image.imageData);
            _gl.texParameteri(_gl.TEXTURE_2D, _gl.TEXTURE_MAG_FILTER, (int)_gl.LINEAR);
            _gl.texParameteri(_gl.TEXTURE_2D, _gl.TEXTURE_MIN_FILTER, (int)_gl.LINEAR_MIPMAP_LINEAR);
            _gl.generateMipmap(_gl.TEXTURE_2D);
            _gl.texParameterf(_gl.TEXTURE_2D, glExtensionTextureFilterAnisotropic.TEXTURE_MAX_ANISOTROPY_EXT, 16);
            _gl.bindTexture(_gl.TEXTURE_2D, null);
        }

        private void InitGL()
        {
            _gl = (WebGLRenderingContext)new HTMLCanvasElement(canvas).getContext("webgl");
        }

        private void InitShaders()
        {
            var fragmentShader = GetShader(_gl, _gl.createShader(_gl.FRAGMENT_SHADER), Fragment);
            var vertexShader = GetShader(_gl, _gl.createShader(_gl.VERTEX_SHADER), Vertex);

            _shaderProgram = _gl.createProgram();
            _gl.attachShader(_shaderProgram, vertexShader);
            _gl.attachShader(_shaderProgram, fragmentShader);
            _gl.linkProgram(_shaderProgram);

            if (!_gl.getProgramParameter(_shaderProgram, _gl.LINK_STATUS))
            {
                MessageBox.Show(@"Could not initialise shaders");
            }

            _gl.useProgram(_shaderProgram);

            _shaderProgramVertexPositionAttribute = _gl.getAttribLocation(_shaderProgram, "aVertexPosition");
            _gl.enableVertexAttribArray((uint)_shaderProgramVertexPositionAttribute);

            _shaderProgramTextureCoordAttribute = _gl.getAttribLocation(_shaderProgram, "aTextureCoord");
            _gl.enableVertexAttribArray((uint)_shaderProgramTextureCoordAttribute);

            _shaderProgramPMatrixUniform = _gl.getUniformLocation(_shaderProgram, "uPMatrix");
            _shaderProgramMvMatrixUniform = _gl.getUniformLocation(_shaderProgram, "uMVMatrix");
            _shaderProgramSamplerUniform = _gl.getUniformLocation(_shaderProgram, "uSampler");
        }

        private static WebGLShader GetShader(WebGLRenderingContext gl, WebGLShader shader, string str)
        {
            gl.shaderSource(shader, str);
            gl.compileShader(shader);
            if (!gl.getShaderParameter(shader, gl.COMPILE_STATUS))
            {
                MessageBox.Show(gl.getShaderInfoLog(shader));
                return null;
            }
            return shader;
        }

        private void InitBuffers()
        {
            _cubeVertexPositionBuffer = _gl.createBuffer();
            _gl.bindBuffer(_gl.ARRAY_BUFFER, _cubeVertexPositionBuffer);
            var vertices = new[]
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

            _gl.bufferData(_gl.ARRAY_BUFFER, new Float32Array(vertices), _gl.STATIC_DRAW);
            _cubeVertexPositionBufferItemSize = 3;
            _cubeVertexPositionBufferNumItems = 24;

            _cubeVertexTextureCoordBuffer = _gl.createBuffer();
            _gl.bindBuffer(_gl.ARRAY_BUFFER, _cubeVertexTextureCoordBuffer);
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
                                    0.0f, 1.0f,
                                };

            _gl.bufferData(_gl.ARRAY_BUFFER, new Float32Array(textureCoords), _gl.STATIC_DRAW);
            _cubeVertexTextureCoordBufferItemSize = 2;
            _cubeVertexTextureCoordBufferNumItems = 24;

            _cubeVertexIndexBuffer = _gl.createBuffer();
            _gl.bindBuffer(_gl.ELEMENT_ARRAY_BUFFER, _cubeVertexIndexBuffer);
            var cubeVertexIndices = new UInt16[]
                                    {
                                        0, 1, 2, 0, 2, 3, // Front face
                                        4, 5, 6, 4, 6, 7, // Back face
                                        8, 9, 10, 8, 10, 11, // Top face
                                        12, 13, 14, 12, 14, 15, // Bottom face
                                        16, 17, 18, 16, 18, 19, // Right face
                                        20, 21, 22, 20, 22, 23 // Left face
                                    };
            _gl.bufferData(_gl.ELEMENT_ARRAY_BUFFER, new Uint16Array(cubeVertexIndices), _gl.STATIC_DRAW);
            _cubeVertexIndexBufferItemSize = 1;
            _cubeVertexIndexBufferNumItems = 36;
        }

        private void DrawScene()
        {
            _gl.swapBuffers();

            _gl.viewport(0, 0, ClientSize.Width, ClientSize.Height);
            _gl.clear(_gl.COLOR_BUFFER_BIT | _gl.DEPTH_BUFFER_BIT);

            mat4.perspective(45, ClientSize.Width / (float)ClientSize.Height, 0.1f, 100.0f, _pMatrix);

            mat4.identity(_mvMatrix);

            mat4.translate(_mvMatrix, new float[] {0, 0, -5});

            mat4.rotate(_mvMatrix, DegToRad(_xRot), new[] {1f, 0f, 0f});
            mat4.rotate(_mvMatrix, DegToRad(_yRot), new[] {0f, 1f, 0f});
            mat4.rotate(_mvMatrix, DegToRad(_zRot), new[] {0f, 0f, 1f});

            _gl.bindBuffer(_gl.ARRAY_BUFFER, _cubeVertexPositionBuffer);
            _gl.vertexAttribPointer((uint)_shaderProgramVertexPositionAttribute, _cubeVertexPositionBufferItemSize, _gl.FLOAT, false, 0, 0);

            _gl.bindBuffer(_gl.ARRAY_BUFFER, _cubeVertexTextureCoordBuffer);
            _gl.vertexAttribPointer((uint)_shaderProgramTextureCoordAttribute, _cubeVertexTextureCoordBufferItemSize, _gl.FLOAT, false, 0, 0);

            _gl.activeTexture(_gl.TEXTURE0);
            _gl.bindTexture(_gl.TEXTURE_2D, _neheTexture);
            _gl.uniform1i(_shaderProgramSamplerUniform, 0);

            _gl.uniformMatrix4fv(_shaderProgramPMatrixUniform, false, _pMatrix);
            _gl.uniformMatrix4fv(_shaderProgramMvMatrixUniform, false, _mvMatrix);

            _gl.bindBuffer(_gl.ELEMENT_ARRAY_BUFFER, _cubeVertexIndexBuffer);
            _gl.drawElements(_gl.TRIANGLES, _cubeVertexIndexBufferNumItems, _gl.UNSIGNED_SHORT, 0);
        }

        private static float DegToRad(double degrees)
        {
            return (float)(degrees * Math.PI / 180);
        }

        private void Animate()
        {
            var timeNow = Environment.TickCount;
            if (_lastTime != 0)
            {
                var elapsed = timeNow - _lastTime;

                _xRot += (10 * elapsed) / 1000.0f;
                _yRot += (10 * elapsed) / 1000.0f;
                _zRot += (10 * elapsed) / 1000.0f;
            }
            _lastTime = timeNow;
        }

        private void TimerTick(object sender, EventArgs e)
        {
            DrawScene();
            Animate();
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