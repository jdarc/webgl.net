using System.Windows.Forms;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class TextureComplete : BaseTest
    {
        [Test(Description = "")]
        public void ShouldDoMagic()
        {
            wtu.debug("Checks that a texture that is not -texture-complete- does not draw if" +
                      " filtering needs mips");
            wtu.debug("");

            var canvas2d = new HTMLCanvasElement(new Control());
//  var ctx2d = canvas2d.getContext("2d");
//  ctx2d.fillStyle = "rgba(0,192,128,1)";
//  ctx2d.fillRect(0, 0, 16, 16);

            var gl = wtu.create3DContext(Canvas);
            var program = wtu.setupTexturedQuad(gl);

            gl.disable(gl.DEPTH_TEST);
            gl.disable(gl.BLEND);

            var tex = gl.createTexture();
            gl.bindTexture(gl.TEXTURE_2D, tex);
            // 16x16 texture no mips
            gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, canvas2d);

            var loc = gl.getUniformLocation(program, "tex");
            gl.uniform1i(loc, 0);

            wtu.drawQuad(gl);
            wtu.checkCanvas(gl, new[] {0, 0, 0, 255},
                            "texture that is not -texture-complete- when " +
                            "TEXTURE_MIN_FILTER not NEAREST or LINEAR should draw with 0,0,0,255");
        }
    }
}