using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class BufferBindTest : BaseTest
    {
        [Test(Description = "Checks a buffer can only be bound to 1 target.")]
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

                var buf = gl.createBuffer();
                gl.bindBuffer(gl.ARRAY_BUFFER, buf);
                wtu.glErrorShouldBe(gl, gl.NO_ERROR, "should be able to bind buffer.");
                gl.bindBuffer(gl.ARRAY_BUFFER, null);
                wtu.glErrorShouldBe(gl, gl.NO_ERROR, "should be able to unbind buffer.");
                gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, buf);
                wtu.glErrorShouldBe(gl, gl.INVALID_OPERATION, "should get INVALID_OPERATION if attempting to bind buffer to different target");

                buf = gl.createBuffer();
                gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, buf);
                wtu.glErrorShouldBe(gl, gl.NO_ERROR, "should be able to bind buffer.");
                gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, null);
                wtu.glErrorShouldBe(gl, gl.NO_ERROR, "should be able to unbind buffer.");
                gl.bindBuffer(gl.ARRAY_BUFFER, buf);
                wtu.glErrorShouldBe(gl, gl.INVALID_OPERATION, "should get INVALID_OPERATION if attempting to bind buffer to different target");
            }

            wtu.debug("");
        }
    }
}