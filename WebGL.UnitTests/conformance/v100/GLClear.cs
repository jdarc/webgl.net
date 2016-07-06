using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLClear : BaseTest
    {
        [Test(Description = "Test clear.")]
        public void ShouldDoMagic()
        {
            var gl = WebGLTestUtils.create3DContext(Canvas);
            var program = WebGLTestUtils.setupTexturedQuad(gl);

            WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR, "Should be no errors from setup.");
            WebGLTestUtils.checkCanvas(gl, new[] {0, 0, 0, 0}, "should be 0,0,0,0");

            gl.clearColor(1, 1, 1, 1);
            gl.clear(gl.COLOR_BUFFER_BIT);
            WebGLTestUtils.checkCanvas(gl, new[] {255, 255, 255, 255}, "should be 255,255,255,255");

            gl.clearColor(0, 0, 0, 0);
            gl.clear(gl.COLOR_BUFFER_BIT);
            WebGLTestUtils.checkCanvas(gl, new[] {0, 0, 0, 0}, "should be 0,0,0,0");

            gl.colorMask(false, false, false, true);
            gl.clearColor(1, 1, 1, 1);
            gl.clear(gl.COLOR_BUFFER_BIT);
            WebGLTestUtils.checkCanvas(gl, new[] {0, 0, 0, 255}, "should be 0,0,0,255");

            var tex = gl.createTexture();
            gl.bindTexture(gl.TEXTURE_2D, tex);
            gl.texImage2D(
                gl.TEXTURE_2D, 0, gl.RGBA, 1, 1, 0, gl.RGBA, gl.UNSIGNED_BYTE,
                new Uint8Array(new byte[] {128, 128, 128, 192}));

            gl.disable(gl.DEPTH_TEST);
            gl.disable(gl.BLEND);
            gl.colorMask(true, true, true, true);
            gl.drawArrays(gl.TRIANGLES, 0, 6);
            WebGLTestUtils.checkCanvas(gl, new[] {128, 128, 128, 192}, "should be 128,128,128,192");

            gl.colorMask(false, false, false, true);
            gl.clearColor(1, 1, 1, 1);
            gl.clear(gl.COLOR_BUFFER_BIT);
            WebGLTestUtils.checkCanvas(gl, new[] {128, 128, 128, 255}, "should be 128,128,128,255");
        }
    }
}