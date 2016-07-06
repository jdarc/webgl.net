using System.Windows.Forms;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class TextureNPOT : BaseTest
    {
        private static readonly Script vshader = new Script("vshader", "x-shader/x-vertex")
                                                 {
                                                     text = @"attribute vec4 vPosition;
attribute vec2 texCoord0;
varying vec2 texCoord;
void main()
{
    gl_Position = vPosition;
    texCoord = texCoord0;
}"
                                                 };

        private static readonly Script fshader = new Script("fshader", "x-shader/x-fragment")
                                                 {
                                                     text = @"#ifdef GL_ES
precision mediump float;
#endif
uniform samplerCube tex;
varying vec2 texCoord;
void main()
{
    gl_FragColor = textureCube(tex, normalize(vec3(texCoord, 1)));
}"
                                                 };

        [Test(Description = "")]
        public void ShouldDoMagic()
        {
            var gl = wtu.create3DContext(Canvas);
            var program = wtu.setupTexturedQuad(gl);

            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "Should be no errors from setup.");

            var tex = gl.createTexture();

            // Check that an NPOT texture not on level 0 generates INVALID_VALUE
            wtu.fillTexture(gl, tex, 5, 3, new byte[] {0, 192, 128, 255}, 1);
            wtu.glErrorShouldBe(gl, gl.INVALID_VALUE, "gl.texImage2D with NPOT texture with level > 0 should return INVALID_VALUE");

            // Check that an NPOT texture on level 0 succeeds
            wtu.fillTexture(gl, tex, 5, 3, new byte[] {0, 192, 128, 255});
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "gl.texImage2D with NPOT texture at level 0 should succeed");

            // Check that generateMipmap fails on NPOT
            gl.generateMipmap(gl.TEXTURE_2D);
            wtu.glErrorShouldBe(gl, gl.INVALID_OPERATION, "gl.generateMipmap with NPOT texture should return INVALID_OPERATION");

            var loc = gl.getUniformLocation(program, "tex");
            gl.uniform1i(loc, 0);

            // Check that nothing is drawn if filtering is not correct for NPOT
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, (int)gl.NEAREST);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, (int)gl.NEAREST);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, (int)gl.REPEAT);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, (int)gl.REPEAT);

            wtu.drawQuad(gl);
            wtu.checkCanvas(gl, new[] {0, 0, 0, 255}, "NPOT texture with TEXTURE_WRAP set to REPEAT should draw with 0,0,0,255");
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "Should be no errors from setup.");

            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, (int)gl.CLAMP_TO_EDGE);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, (int)gl.CLAMP_TO_EDGE);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, (int)gl.NEAREST_MIPMAP_LINEAR);

            wtu.drawQuad(gl);
            wtu.checkCanvas(gl, new[] {0, 0, 0, 255}, "NPOT texture with TEXTURE_MIN_FILTER not NEAREST or LINEAR should draw with 0,0,0,255");
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "Should be no errors from setup.");

            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, (int)gl.LINEAR);

            wtu.drawQuad(gl);
            wtu.checkCanvas(gl, new[] {0, 192, 128, 255}, "NPOT texture with TEXTURE_MIN_FILTER set to LINEAR should draw.");

            gl.copyTexImage2D(gl.TEXTURE_2D, 1, gl.RGBA, 0, 0, 5, 3, 0);
            wtu.glErrorShouldBe(gl, gl.INVALID_VALUE,
                                "copyTexImage2D with NPOT texture with level > 0 should return INVALID_VALUE.");

            // Check that generateMipmap for an POT texture succeeds
            wtu.fillTexture(gl, tex, 4, 4, new byte[] {0, 192, 128, 255});
            gl.generateMipmap(gl.TEXTURE_2D);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "gl.texImage2D and gl.generateMipmap with POT texture at level 0 should succeed");

            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, (int)gl.LINEAR_MIPMAP_LINEAR);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, (int)gl.LINEAR);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, (int)gl.REPEAT);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, (int)gl.REPEAT);

            wtu.drawQuad(gl);
            wtu.checkCanvas(gl, new[] {0, 192, 128, 255}, "POT texture with TEXTURE_MIN_FILTER set to LINEAR_MIPMAP_LINEAR should draw.");
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "Should be no errors from setup.");

            wtu.debug("");
            wtu.debug("check using cubemap");
            program = wtu.setupProgram(
                gl,
                new[]
                {
                    wtu.loadShaderFromScript(gl, vshader, gl.VERTEX_SHADER),
                    wtu.loadShaderFromScript(gl, fshader, gl.FRAGMENT_SHADER)
                },
                new[] {"vPosition", "texCoord0"}, new[] {0, 1});
            tex = gl.createTexture();

            // Check that an NPOT texture not on level 0 generates INVALID_VALUE
            fillCubeTexture(gl, tex, 5, 3, new byte[] {0, 192, 128, 255}, 1);
            wtu.glErrorShouldBe(gl, gl.INVALID_VALUE, "gl.texImage2D with NPOT texture with level > 0 should return INVALID_VALUE");

            // Check that an NPOT texture on level 0 succeeds
            fillCubeTexture(gl, tex, 5, 5, new byte[] {0, 192, 128, 255});
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "gl.texImage2D with NPOT texture at level 0 should succeed");

            // Check that generateMipmap fails on NPOT
            gl.generateMipmap(gl.TEXTURE_CUBE_MAP);
            wtu.glErrorShouldBe(gl, gl.INVALID_OPERATION, "gl.generateMipmap with NPOT texture should return INVALID_OPERATION");

            loc = gl.getUniformLocation(program, "tex");
            gl.uniform1i(loc, 0);

            // Check that nothing is drawn if filtering is not correct for NPOT
            gl.texParameteri(gl.TEXTURE_CUBE_MAP, gl.TEXTURE_MIN_FILTER, (int)gl.NEAREST);
            gl.texParameteri(gl.TEXTURE_CUBE_MAP, gl.TEXTURE_MAG_FILTER, (int)gl.NEAREST);
            gl.texParameteri(gl.TEXTURE_CUBE_MAP, gl.TEXTURE_WRAP_S, (int)gl.REPEAT);
            gl.texParameteri(gl.TEXTURE_CUBE_MAP, gl.TEXTURE_WRAP_T, (int)gl.REPEAT);

            wtu.drawQuad(gl);
            wtu.checkCanvas(gl, new[] {0, 0, 0, 255}, "NPOT cubemap with TEXTURE_WRAP set to REPEAT should draw with 0,0,0,255");
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "Should be no errors from setup.");

            gl.texParameteri(gl.TEXTURE_CUBE_MAP, gl.TEXTURE_WRAP_S, (int)gl.CLAMP_TO_EDGE);
            gl.texParameteri(gl.TEXTURE_CUBE_MAP, gl.TEXTURE_WRAP_T, (int)gl.CLAMP_TO_EDGE);
            gl.texParameteri(gl.TEXTURE_CUBE_MAP, gl.TEXTURE_MIN_FILTER, (int)gl.NEAREST_MIPMAP_LINEAR);

            wtu.drawQuad(gl);
            wtu.checkCanvas(gl, new[] {0, 0, 0, 255}, "NPOT cubemap with TEXTURE_MIN_FILTER not NEAREST or LINEAR should draw with 0,0,0,255");
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "Should be no errors from setup.");

            gl.texParameteri(gl.TEXTURE_CUBE_MAP, gl.TEXTURE_MIN_FILTER, (int)gl.LINEAR);

            wtu.drawQuad(gl);
            wtu.checkCanvas(gl, new[] {0, 192, 128, 255}, "NPOT cubemap with TEXTURE_MIN_FILTER set to LINEAR should draw.");

            // Check that an POT texture on level 0 succeeds
            fillCubeTexture(gl, tex, 4, 4, new byte[] {0, 192, 128, 255});
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "gl.texImage2D with POT texture at level 0 should succeed");

            gl.texParameteri(gl.TEXTURE_CUBE_MAP, gl.TEXTURE_MIN_FILTER, (int)gl.LINEAR_MIPMAP_LINEAR);
            gl.texParameteri(gl.TEXTURE_CUBE_MAP, gl.TEXTURE_MAG_FILTER, (int)gl.LINEAR);
            gl.texParameteri(gl.TEXTURE_CUBE_MAP, gl.TEXTURE_WRAP_S, (int)gl.REPEAT);
            gl.texParameteri(gl.TEXTURE_CUBE_MAP, gl.TEXTURE_WRAP_T, (int)gl.REPEAT);

            wtu.drawQuad(gl);
            wtu.checkCanvas(gl, new[] {0, 0, 0, 255}, "POT cubemap with TEXTURE_MIN_FILTER set to LINEAR_MIPMAP_LINEAR but no mips draw with 0,0,0,255");

            // Check that generateMipmap succeeds on POT
            gl.generateMipmap(gl.TEXTURE_CUBE_MAP);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "gl.generateMipmap with POT texture should return succeed");

            wtu.drawQuad(gl);
            wtu.checkCanvas(gl, new[] {0, 192, 128, 255}, "POT cubemap with TEXTURE_MIN_FILTER set to LINEAR_MIPMAP_LINEAR should draw.");
        }

        private static void fillCubeTexture(WebGLRenderingContext gl, WebGLTexture tex, int width, int height, byte[] color, int opt_level = 0)
        {
            var canvas = new HTMLCanvasElement(new Control());
            canvas.setWidth(width);
            canvas.setHeight(height);
            //            var ctx2d = canvas.getContext("2d");
            //            ctx2d.fillStyle = "rgba(" + color[0] + "," + color[1] + "," + color[2] + "," + color[3] + ")";
            //            ctx2d.fillRect(0, 0, width, height);
            gl.bindTexture(gl.TEXTURE_CUBE_MAP, tex);
            var targets = new[]
                          {
                              gl.TEXTURE_CUBE_MAP_POSITIVE_X,
                              gl.TEXTURE_CUBE_MAP_NEGATIVE_X,
                              gl.TEXTURE_CUBE_MAP_POSITIVE_Y,
                              gl.TEXTURE_CUBE_MAP_NEGATIVE_Y,
                              gl.TEXTURE_CUBE_MAP_POSITIVE_Z,
                              gl.TEXTURE_CUBE_MAP_NEGATIVE_Z
                          };
            for (var tt = 0; tt < targets.Length; ++tt)
            {
                gl.texImage2D(targets[tt], opt_level, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, canvas);
            }
        }
    }
}