using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLMinTextures : BaseTest
    {
        private static readonly Script vshader = new Script("vshader", "x-shader/x-vertex")
                                                 {text = @"
attribute vec4 vPosition;
void main()
{
    gl_Position = vPosition;
}"};

        private static readonly Script fshader = new Script("fshader", "x-shader/x-fragment")
                                                 {
                                                     text =
                                                         @"
#define NUM_TEXTURES 8 // See spec
#ifdef GL_ES
precision mediump float;
#endif
uniform sampler2D uni[8];
void main()
{
    vec4 c = vec4(0,0,0,0);
    for (int ii = 0; ii < NUM_TEXTURES; ++ii) {
      c += texture2D(uni[ii], vec2(0.5, 0.5));
    }
    gl_FragColor = c;
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

            for (var ii = 0; ii < 8; ++ii)
            {
                var loc = gl.getUniformLocation(program, "uni[" + ii + "]");
                gl.activeTexture((uint)(gl.TEXTURE0 + ii));
                var tex = gl.createTexture();
                wtu.fillTexture(gl, tex, 1, 1, new byte[] {32, 16, 8, (byte)(ii * 9)}, 0);
                gl.uniform1i(loc, ii);
            }

            wtu.drawQuad(gl);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "Should be no errors from setup.");
            wtu.checkCanvas(gl, new[] {255, 128, 64, 252}, "Should render using all texture units");
        }
    }
}