using System;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLScissorTest : BaseTest
    {
        [Test(Description = "Check if glScissor setting works.")]
        public void ShouldDoMagic()
        {
            wtu.debug("");
            wtu.debug("Canvas.getContext");

            var gl = wtu.create3DContext(Canvas);
            if (gl == null)
            {
                wtu.testFailed("context does not exist");
            }
            else
            {
                wtu.testPassed("context exists");

                wtu.debug("");

                gl.clearColor(0, 0, 0, 0);
                gl.clear(gl.COLOR_BUFFER_BIT);

                // clear a portion of our FBO
                gl.enable(gl.SCISSOR_TEST);
                gl.scissor(0, 0, 1, 1);
                gl.clearColor(0, 1, 0, 1);
                gl.clear(gl.COLOR_BUFFER_BIT);

                var bb = new Uint8Array(2 * 2 * 4);
                gl.readPixels(0, 0, 2, 2, gl.RGBA, gl.UNSIGNED_BYTE, bb);

                Action<dynamic, int, int, byte[]> checkPixel =
                    (b, x, y, color) =>
                    {
                        var offset = (y * 2 + x) * 4;
                        var match = true;
                        for (var c = 0; c < 4; ++c)
                        {
                            if (b[offset + c] != color[c] * 255)
                            {
                                match = false;
                                break;
                            }
                        }
                        wtu.assertMsg(() => match, "pixel at " + x + ", " + y + " is expected value");
                    };

                checkPixel(bb, 0, 0, new byte[] {0, 1, 0, 1});
                checkPixel(bb, 1, 0, new byte[] {0, 0, 0, 0});
                checkPixel(bb, 0, 1, new byte[] {0, 0, 0, 0});
                checkPixel(bb, 1, 1, new byte[] {0, 0, 0, 0});
            }

            wtu.debug("");
        }
    }
}