using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using WebGL;

namespace Demo.Nehe
{
    public partial class Lesson4Form : Form
    {
        private const string Fragment = @"
#ifdef GL_ES
precision mediump float;
#endif
                                     
varying vec4 vColor;

void main(void) {
    gl_FragColor = vColor;
}";

        private const string Vertex =
            @"
attribute vec3 aVertexPosition;
attribute vec4 aVertexColor;

uniform mat4 uMVMatrix;
uniform mat4 uPMatrix;

varying vec4 vColor;

void main(void) {
    gl_Position = uPMatrix * uMVMatrix * vec4(aVertexPosition, 1.0);
    vColor = aVertexColor;
}";

        private WebGLRenderingContext _gl;
        private WebGLProgram _shaderProgram;
        private int _shaderProgramVertexPositionAttribute;
        private int _shaderProgramVertexColorAttribute;
        private WebGLUniformLocation _shaderProgramPMatrixUniform;
        private WebGLUniformLocation _shaderProgramMvMatrixUniform;
        private WebGLBuffer _pyramidVertexPositionBuffer;
        private int _pyramidVertexPositionBufferItemSize;
        private int _pyramidVertexPositionBufferNumItems;
        private WebGLBuffer _pyramidVertexColorBuffer;
        private int _pyramidVertexColorBufferItemSize;
        private int _pyramidVertexColorBufferNumItems;
        private int _lastTime;
        private double _rPyramid;
        private double _rCube;
        private Float32Array _mvMatrix = mat4.create();
        private readonly Stack<Float32Array> _mvMatrixStack = new Stack<Float32Array>();
        private readonly Float32Array _pMatrix = mat4.create();
        private WebGLBuffer _cubeVertexPositionBuffer;
        private int _cubeVertexPositionBufferItemSize;
        private int _cubeVertexPositionBufferNumItems;
        private WebGLBuffer _cubeVertexColorBuffer;
        private int _cubeVertexColorBufferItemSize;
        private int _cubeVertexColorBufferNumItems;
        private WebGLBuffer _cubeVertexIndexBuffer;
        private int _cubeVertexIndexBufferItemSize;
        private int _cubeVertexIndexBufferNumItems;

        public Lesson4Form()
        {
            InitializeComponent();

            ClientSize = new Size(1440, 900);

            InitGL();
            InitShaders();
            InitBuffers();

            _gl.clearColor(0.0f, 0.0f, 0.0f, 1.0f);
            _gl.enable(_gl.DEPTH_TEST);
            _gl.enable(_gl.CULL_FACE);
            _gl.cullFace(_gl.BACK);
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

            _shaderProgramVertexColorAttribute = _gl.getAttribLocation(_shaderProgram, "aVertexColor");
            _gl.enableVertexAttribArray((uint)_shaderProgramVertexColorAttribute);

            _shaderProgramPMatrixUniform = _gl.getUniformLocation(_shaderProgram, "uPMatrix");
            _shaderProgramMvMatrixUniform = _gl.getUniformLocation(_shaderProgram, "uMVMatrix");

            JSConsole.log(_gl.getProgramInfoLog(_shaderProgram));
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
            _pyramidVertexPositionBuffer = _gl.createBuffer();
            _gl.bindBuffer(_gl.ARRAY_BUFFER, _pyramidVertexPositionBuffer);
            var vertices = new[]
                           {
                               // Front face
                               0.0f, 1.0f, 0.0f,
                               -1.0f, -1.0f, 1.0f,
                               1.0f, -1.0f, 1.0f,
                               // Right face
                               0.0f, 1.0f, 0.0f,
                               1.0f, -1.0f, 1.0f,
                               1.0f, -1.0f, -1.0f,
                               // Back face
                               0.0f, 1.0f, 0.0f,
                               1.0f, -1.0f, -1.0f,
                               -1.0f, -1.0f, -1.0f,
                               // Left face
                               0.0f, 1.0f, 0.0f,
                               -1.0f, -1.0f, -1.0f,
                               -1.0f, -1.0f, 1.0f
                           };

            _gl.bufferData(_gl.ARRAY_BUFFER, new Float32Array(vertices), _gl.STATIC_DRAW);
            _pyramidVertexPositionBufferItemSize = 3;
            _pyramidVertexPositionBufferNumItems = 12;

            _pyramidVertexColorBuffer = _gl.createBuffer();
            _gl.bindBuffer(_gl.ARRAY_BUFFER, _pyramidVertexColorBuffer);
            var colors = new[]
                         {
                             // Front face
                             1.0f, 0.0f, 0.0f, 1.0f,
                             0.0f, 1.0f, 0.0f, 1.0f,
                             0.0f, 0.0f, 1.0f, 1.0f,
                             // Right face
                             1.0f, 0.0f, 0.0f, 1.0f,
                             0.0f, 0.0f, 1.0f, 1.0f,
                             0.0f, 1.0f, 0.0f, 1.0f,
                             // Back face
                             1.0f, 0.0f, 0.0f, 1.0f,
                             0.0f, 1.0f, 0.0f, 1.0f,
                             0.0f, 0.0f, 1.0f, 1.0f,
                             // Left face
                             1.0f, 0.0f, 0.0f, 1.0f,
                             0.0f, 0.0f, 1.0f, 1.0f,
                             0.0f, 1.0f, 0.0f, 1.0f
                         };

            _gl.bufferData(_gl.ARRAY_BUFFER, new Float32Array(colors), _gl.STATIC_DRAW);
            _pyramidVertexColorBufferItemSize = 4;
            _pyramidVertexColorBufferNumItems = 12;

            _cubeVertexPositionBuffer = _gl.createBuffer();
            _gl.bindBuffer(_gl.ARRAY_BUFFER, _cubeVertexPositionBuffer);
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
                           -1.0f, 1.0f, -1.0f
                       };

            _gl.bufferData(_gl.ARRAY_BUFFER, new Float32Array(vertices), _gl.STATIC_DRAW);
            _cubeVertexPositionBufferItemSize = 3;
            _cubeVertexPositionBufferNumItems = 24;

            _cubeVertexColorBuffer = _gl.createBuffer();
            _gl.bindBuffer(_gl.ARRAY_BUFFER, _cubeVertexColorBuffer);
            var cubcolors = new List<float[]>
                            {
                                new[] {1.0f, 0.0f, 0.0f, 1.0f},
                                // Front face
                                new[] {1.0f, 1.0f, 0.0f, 1.0f},
                                // Back face
                                new[] {0.0f, 1.0f, 0.0f, 1.0f},
                                // Top face
                                new[] {1.0f, 0.5f, 0.5f, 1.0f},
                                // Bottom face
                                new[] {1.0f, 0.0f, 1.0f, 1.0f},
                                // Right face
                                new[] {0.0f, 0.0f, 1.0f, 1.0f}
                            }; // Left face

            var unpackedColors = new float[96];
            var off = 0;
            foreach (var i in cubcolors)
            {
                for (var j = 0; j < 4; j++)
                {
                    unpackedColors[off++] = i[0];
                    unpackedColors[off++] = i[1];
                    unpackedColors[off++] = i[2];
                    unpackedColors[off++] = i[3];
                }
            }

            _gl.bufferData(_gl.ARRAY_BUFFER, new Float32Array(unpackedColors), _gl.STATIC_DRAW);
            _cubeVertexColorBufferItemSize = 4;
            _cubeVertexColorBufferNumItems = 24;

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

            mat4.translate(_mvMatrix, new[] {-1.5f, 0.0f, -8.0f});

            MvPushMatrix();

            mat4.rotate(_mvMatrix, DegToRad(_rPyramid), new float[] {0, 1, 0});

            _gl.bindBuffer(_gl.ARRAY_BUFFER, _pyramidVertexPositionBuffer);
            _gl.vertexAttribPointer((uint)_shaderProgramVertexPositionAttribute, _pyramidVertexPositionBufferItemSize, _gl.FLOAT, false, 0, 0);

            _gl.bindBuffer(_gl.ARRAY_BUFFER, _pyramidVertexColorBuffer);
            _gl.vertexAttribPointer((uint)_shaderProgramVertexColorAttribute, _pyramidVertexColorBufferItemSize, _gl.FLOAT, false, 0, 0);

            SetMatrixUniforms();
            _gl.drawArrays(_gl.TRIANGLES, 0, _pyramidVertexPositionBufferNumItems);

            MvPopMatrix();

            mat4.translate(_mvMatrix, new[] {3.0f, 0.0f, 0.0f});

            MvPushMatrix();
            mat4.rotate(_mvMatrix, DegToRad(_rCube), new float[] {1, 1, 1});

            _gl.bindBuffer(_gl.ARRAY_BUFFER, _cubeVertexPositionBuffer);
            _gl.vertexAttribPointer((uint)_shaderProgramVertexPositionAttribute, _cubeVertexPositionBufferItemSize, _gl.FLOAT, false, 0, 0);

            _gl.bindBuffer(_gl.ARRAY_BUFFER, _cubeVertexColorBuffer);
            _gl.vertexAttribPointer((uint)_shaderProgramVertexColorAttribute, _cubeVertexColorBufferItemSize, _gl.FLOAT, false, 0, 0);

            _gl.bindBuffer(_gl.ELEMENT_ARRAY_BUFFER, _cubeVertexIndexBuffer);
            SetMatrixUniforms();
            _gl.drawElements(_gl.TRIANGLES, _cubeVertexIndexBufferNumItems, _gl.UNSIGNED_SHORT, 0);

            MvPopMatrix();
        }

        private void MvPopMatrix()
        {
            if (_mvMatrixStack.Count == 0)
            {
                throw new ApplicationException("Invalid popMatrix!");
            }
            _mvMatrix = _mvMatrixStack.Pop();
        }

        private void MvPushMatrix()
        {
            var copy = mat4.create();
            mat4.set(_mvMatrix, copy);
            _mvMatrixStack.Push(copy);
        }

        private static float DegToRad(double degrees)
        {
            return (float)(degrees * Math.PI / 180);
        }

        private void SetMatrixUniforms()
        {
            _gl.uniformMatrix4fv(_shaderProgramPMatrixUniform, false, _pMatrix);
            _gl.uniformMatrix4fv(_shaderProgramMvMatrixUniform, false, _mvMatrix);
        }

        private void Animate()
        {
            var timeNow = Environment.TickCount;
            if (_lastTime != 0)
            {
                var elapsed = timeNow - _lastTime;

                _rPyramid += (90 * elapsed) / 1000.0;
                _rCube -= (75 * elapsed) / 1000.0;
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