using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLMinUniforms : BaseTest
    {
        private static readonly Script vshader = new Script("vshader", "x-shader/x-vertex")
                                                 {
                                                     text =
                                                         @"
#define NUM_UNIFORMS 128 // See spec
attribute vec4 vPosition;
uniform vec4 uni[NUM_UNIFORMS];
varying vec4 color;
void main()
{
    gl_Position = vPosition;
    vec4 c = vec4(0,0,0,0);
    for (int ii = 0; ii < NUM_UNIFORMS; ++ii) {
      c += uni[ii];
    }
    color = c;
}"
                                                 };

        private static readonly Script fshader = new Script("fshader", "x-shader/x-fragment") {text = @"
#ifdef GL_ES
precision mediump float;
#endif
varying vec4 color;
void main()
{
    gl_FragColor = color;
}"};

        private static readonly Script vshader2 = new Script("vshader2", "x-shader/x-vertex") {text = @"
attribute vec4 vPosition;
void main()
{
    gl_Position = vPosition;
}"};

        private static readonly Script fshader2 = new Script("fshader2", "x-shader/x-fragment")
                                                  {
                                                      text =
                                                          @"
#ifdef GL_ES
precision mediump float;
#endif
#define NUM_UNIFORMS 16 // See spec
uniform vec4 uni[NUM_UNIFORMS];
void main()
{
    vec4 c = vec4(0,0,0,0);
    for (int ii = 0; ii < NUM_UNIFORMS; ++ii) {
       c += uni[ii];
    }
    gl_FragColor = vec4(c.r, c.g, c.b, c.a / 120.0);
}"
                                                  };

        [Test(Description = "")]
        public void ShouldDoMagic()
        {
            var gl = wtu.create3DContext(Canvas);
            wtu.setupTexturedQuad(gl);

            var program = wtu.setupProgram(
                gl,
                new[]
                {
                    wtu.loadShaderFromScript(gl, vshader, gl.VERTEX_SHADER),
                    wtu.loadShaderFromScript(gl, fshader, gl.FRAGMENT_SHADER)
                },
                new[] {"vPosition"}, new[] {0});

            for (var ii = 0; ii < 128; ++ii)
            {
                var loc = gl.getUniformLocation(program, "uni[" + ii + "]");
                gl.uniform4f(loc, 2 / 256f, 2 / 512f, 2 / 1024f, ii / 8128f);
            }

            wtu.drawQuad(gl);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "Should be no errors from setup.");
            wtu.checkCanvasRect(gl, 0, 0, Canvas.width, Canvas.height, new[] {255, 127, 64, 255}, "Should render 255,127,64,32 (+/-1)", 1);

            program = wtu.setupProgram(
                gl,
                new[]
                {
                    wtu.loadShaderFromScript(gl, vshader2, gl.VERTEX_SHADER),
                    wtu.loadShaderFromScript(gl, fshader2, gl.FRAGMENT_SHADER)
                },
                new[] {"vPosition"}, new[] {0});

            for (var ii = 0; ii < 16; ++ii)
            {
                var loc = gl.getUniformLocation(program, "uni[" + ii + "]");
                gl.uniform4f(loc, 16 / 2048f, 16 / 1024f, 16 / 512f, ii);
            }

            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "Should be no errors from setup.");
            wtu.drawQuad(gl);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "Should be no errors from setup.");
            wtu.checkCanvasRect(gl, 0, 0, Canvas.width, Canvas.height, new[] {32, 64, 127, 255}, "Should render 32,64,127,255 (+/-1)", 1);
        }
    }
}