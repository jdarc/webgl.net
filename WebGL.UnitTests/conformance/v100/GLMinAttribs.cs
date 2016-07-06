using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLMinAttribs : BaseTest
    {
        private readonly Script _vshader = new Script("vshader", "x-shader/x-vertex")
                                           {
                                               text =
                                                   @"
attribute vec4 vPosition;
attribute vec4 v0;
attribute vec4 v1;
attribute vec4 v2;
attribute vec4 v3;
attribute vec4 v4;
attribute vec4 v5;
attribute vec4 v6;
varying vec4 color;
void main()
{
    gl_Position = vPosition;
    color = v0 + v1 + v2 + v3 + v4 + v5 + v6;
}
"
                                           };

        private readonly Script _fshader = new Script("fshader", "x-shader/x-fragment")
                                           {
                                               text = @"
#ifdef GL_ES
precision mediump float;
#endif
varying vec4 color;
void main()
{
    gl_FragColor = color;
}
"
                                           };

        [Test(Description = "")]
        public void ShouldDoMagic()
        {
            var gl = WebGLTestUtils.create3DContext(Canvas);

            WebGLTestUtils.setupTexturedQuad(gl);

            var program = WebGLTestUtils.setupProgram(
                gl,
                new[]
                {
                    WebGLTestUtils.loadShaderFromScript(gl, _vshader, gl.VERTEX_SHADER),
                    WebGLTestUtils.loadShaderFromScript(gl, _fshader, gl.FRAGMENT_SHADER)
                },
                new[] {"vPosition", "v0", "v1", "v2", "v3", "v4", "v5", "v6"},
                new[] {0, 1, 2, 3, 4, 5, 6, 7});

            for (var ii = 0; ii < 7; ++ii)
            {
                var v = (ii + 1) / 28f;
                var vertexObject = gl.createBuffer();
                gl.bindBuffer(gl.ARRAY_BUFFER, vertexObject);
                gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(new[]
                                                                {
                                                                    v, v / 2f, v / 4f, v / 8f,
                                                                    v, v / 2f, v / 4f, v / 8f,
                                                                    v, v / 2f, v / 4f, v / 8f,
                                                                    v, v / 2f, v / 4f, v / 8f,
                                                                    v, v / 2f, v / 4f, v / 8f,
                                                                    v, v / 2f, v / 4f, v / 8f
                                                                }), gl.STATIC_DRAW);
                gl.enableVertexAttribArray((uint)(ii + 1));
                gl.vertexAttribPointer((uint)(ii + 1), 4, gl.FLOAT, false, 0, 0);
            }

            WebGLTestUtils.drawQuad(gl);
            WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR, "Should be no errors from setup.");
            WebGLTestUtils.checkCanvasRect(gl, 0, 0, Canvas.width, Canvas.height, new[] {255, 127, 64, 32}, "Should render 255,127,64,32 (+/-1)", 1);
        }
    }
}