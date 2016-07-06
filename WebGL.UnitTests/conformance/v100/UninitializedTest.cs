using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class UninitializedTest : BaseTest
    {
        private const int Width = 512;
        private const int Height = 512;

        [Test(Description = "")]
        public void ShouldDoMagic()
        {
            JSConsole.debug("Tests to check user code cannot access uninitialized data from GL resources.");
            var gl = WebGLTestUtils.create3DContext(Canvas);
            if (gl == null)
            {
                WebGLTestUtils.testFailed("Context created.");
                return;
            }
            WebGLTestUtils.testPassed("Context created.");

            JSConsole.debug("Reading an uninitialized texture (texImage2D) should succeed with all bytes set to 0.");
            {
                var tex = SetupTexture(gl, Width, Height);
                gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, Width, Height, 0, gl.RGBA, gl.UNSIGNED_BYTE, null);
                CheckNonZeroPixels(gl, tex, Width, Height, 0, 0, 0, 0, 0, 0, 0, 0);
                gl.deleteTexture(tex);
                gl.finish();
                WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR);
            }

            JSConsole.debug("Reading an uninitialized portion of a texture (copyTexImage2D) should succeed with all bytes set to 0.");
            {
                var tex = SetupTexture(gl, Width, Height);
                var fbo = gl.createFramebuffer();
                gl.bindFramebuffer(gl.FRAMEBUFFER, fbo);
                var rbo = gl.createRenderbuffer();
                gl.bindRenderbuffer(gl.RENDERBUFFER, rbo);
                const int fboWidth = 16;
                const int fboHeight = 16;
                gl.renderbufferStorage(gl.RENDERBUFFER, gl.RGBA4, fboWidth, fboHeight);
                gl.framebufferRenderbuffer(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.RENDERBUFFER, rbo);
                WebGLTestUtils.shouldBe(() => gl.checkFramebufferStatus(gl.FRAMEBUFFER), gl.FRAMEBUFFER_COMPLETE);
                gl.clearColor(1.0f, 0.0f, 0.0f, 1.0f);
                gl.clear(gl.COLOR_BUFFER_BIT);
                WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR);
                gl.copyTexImage2D(gl.TEXTURE_2D, 0, gl.RGBA, 0, 0, Width, Height, 0);
                CheckNonZeroPixels(gl, tex, Width, Height, 0, 0, fboWidth, fboHeight, 255, 0, 0, 255);
                gl.deleteTexture(tex);
                gl.finish();
                WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR);
            }

            JSConsole.debug("Reading an uninitialized portion of a texture (copyTexImage2D with negative x and y) should succeed with all bytes set to 0.");
            {
                var tex = SetupTexture(gl, Width, Height);
                var fbo = gl.createFramebuffer();
                gl.bindFramebuffer(gl.FRAMEBUFFER, fbo);
                var rbo = gl.createRenderbuffer();
                gl.bindRenderbuffer(gl.RENDERBUFFER, rbo);
                const int fboWidth = 16;
                const int fboHeight = 16;
                gl.renderbufferStorage(gl.RENDERBUFFER, gl.RGBA4, fboWidth, fboHeight);
                gl.framebufferRenderbuffer(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.RENDERBUFFER, rbo);
                WebGLTestUtils.shouldBe(() => gl.checkFramebufferStatus(gl.FRAMEBUFFER), gl.FRAMEBUFFER_COMPLETE);
                gl.clearColor(1.0f, 0.0f, 0.0f, 1.0f);
                gl.clear(gl.COLOR_BUFFER_BIT);
                WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR);
                const int x = -8;
                const int y = -8;
                gl.copyTexImage2D(gl.TEXTURE_2D, 0, gl.RGBA, x, y, Width, Height, 0);
                CheckNonZeroPixels(gl, tex, Width, Height, -x, -y, fboWidth, fboHeight, 255, 0, 0, 255);
                gl.deleteTexture(tex);
                gl.finish();
                WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR);
            }

            JSConsole.debug("Reading an uninitialized portion of a texture (copyTexImage2D from WebGL internal fbo) should succeed with all bytes set to 0.");
            {
                var tex = SetupTexture(gl, Width, Height);
                gl.bindFramebuffer(gl.FRAMEBUFFER, null);
                gl.clearColor(0.0f, 1.0f, 0.0f, 0.0f);
                gl.clear(gl.COLOR_BUFFER_BIT);
                WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR);
                gl.copyTexImage2D(gl.TEXTURE_2D, 0, gl.RGBA, 0, 0, Width, Height, 0);
                CheckNonZeroPixels(gl, tex, Width, Height, 0, 0, Canvas.width, Canvas.height, 0, 255, 0, 0);
                gl.deleteTexture(tex);
                gl.finish();
                WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR);
            }
        }

        private static WebGLTexture SetupTexture(WebGLRenderingContext gl, int texWidth, int texHeight)
        {
            var texture = gl.createTexture();
            gl.bindTexture(gl.TEXTURE_2D, texture);
            gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, texWidth, texHeight, 0, gl.RGBA, gl.UNSIGNED_BYTE, null);

            // this can be quite undeterministic so to improve odds of seeing uninitialized data write bits
            // into tex then delete texture then re-create one with same characteristics (driver will likely reuse mem)
            // with this trick on r59046 WebKit/OSX I get FAIL 100% of the time instead of ~15% of the time.

            var badData = new Uint8Array(texWidth * texHeight * 4);
            for (var i = 0; i < badData.length; ++i)
            {
                badData[i] = (byte)(i % 255);
            }

            gl.texSubImage2D(gl.TEXTURE_2D, 0, 0, 0, texWidth, texHeight, gl.RGBA, gl.UNSIGNED_BYTE, badData);
            gl.finish(); // make sure it has been uploaded

            gl.deleteTexture(texture);
            gl.finish(); // make sure it has been deleted

            texture = gl.createTexture();
            gl.bindTexture(gl.TEXTURE_2D, texture);
            return texture;
        }

        private static void CheckNonZeroPixels(WebGLRenderingContext gl, WebGLTexture texture, int texWidth, int texHeight, int skipX, int skipY, int skipWidth, int skipHeight, int skipR, int skipG, int skipB, int skipA)
        {
            gl.bindTexture(gl.TEXTURE_2D, null);
            var fb = gl.createFramebuffer();
            gl.bindFramebuffer(gl.FRAMEBUFFER, fb);
            gl.framebufferTexture2D(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.TEXTURE_2D, texture, 0);
            WebGLTestUtils.shouldBe(() => gl.checkFramebufferStatus(gl.FRAMEBUFFER), gl.FRAMEBUFFER_COMPLETE);

            var data = new Uint8Array(texWidth * texHeight * 4);
            gl.readPixels(0, 0, texWidth, texHeight, gl.RGBA, gl.UNSIGNED_BYTE, data);

            var k = 0;
            for (var y = 0; y < texHeight; ++y)
            {
                for (var x = 0; x < texWidth; ++x)
                {
                    var index = (y * texWidth + x) * 4;
                    if (x >= skipX && x < skipX + skipWidth && y >= skipY && y < skipY + skipHeight)
                    {
                        if (data[index] != skipR || data[index + 1] != skipG || data[index + 2] != skipB || data[index + 3] != skipA)
                        {
                            WebGLTestUtils.testFailed("non-zero pixel values are wrong");
                            return;
                        }
                    }
                    else
                    {
                        for (var i = 0; i < 4; ++i)
                        {
                            if (data[index + i] != 0)
                            {
                                k++;
                            }
                        }
                    }
                }
            }
            if (k != 0)
            {
                WebGLTestUtils.testFailed("Found " + k + " non-zero bytes");
            }
            else
            {
                WebGLTestUtils.testPassed("All data initialized");
            }
        }
    }
}