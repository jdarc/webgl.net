using System;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class BufferPreserveTest : BaseTest
    {
        [Test(Description = "This test ensures WebGL implementations correctly clear the drawing buffer on composite if preserveDrawingBuffer is false.")]
        public void ShouldDoMagic()
        {
            var iter = 0;
            WebGLRenderingContext gl = null;

            Func<dynamic, dynamic, dynamic, bool> checkPixel =
                (x, y, c) =>
                {
                    var buf = new Uint8Array(4);
                    gl.readPixels(x, y, 1, 1, gl.RGBA, gl.UNSIGNED_BYTE, buf);

                    return buf[0] == c[0] &&
                           buf[1] == c[1] &&
                           buf[2] == c[2] &&
                           buf[3] == c[3];
                };

            Action timer = null;
            timer =
                () =>
                {
                    if (iter == 0)
                    {
                        // some random hacky stuff to make sure that we get a compositing step
                        CanvasControl.Location.Offset(0, -10);
                        CanvasControl.Location.Offset(0, 10);
                        iter++;
                    }
                    else if (iter == 1)
                    {
                        // scissor was set earlier
                        gl.clearColor(0, 0, 1, 1);
                        gl.clear(gl.COLOR_BUFFER_BIT);

                        wtu.checkCanvasRect(gl, 0, 10, 10, 10, new[] {0, 0, 255, 255}, "cleared corner should be blue, stencil should be preserved");
                        wtu.checkCanvasRect(gl, 0, 0, 10, 10, new[] {0, 0, 0, 0}, "remainder of buffer should be cleared");

                        wtu.finishTest();
                    }
                };

            wtu.debug("");

            gl = wtu.create3DContext(Canvas);
            if (gl == null)
            {
                wtu.finishTest();
                return;
            }

            wtu.shouldBeTrue(() => gl.getContextAttributes().preserveDrawingBuffer() == false);

            gl.clearColor(1, 0, 0, 1);
            gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT | gl.STENCIL_BUFFER_BIT);

            // enable scissor here, before compositing, to make sure it's correctly
            // ignored and restored
            gl.scissor(0, 10, 10, 10);
            gl.enable(gl.SCISSOR_TEST);

            timer();
            timer();
        }
    }
}