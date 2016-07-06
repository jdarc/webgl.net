using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class BufferDataArrayBuffer : BaseTest
    {
        [Test(Description = "Test bufferData/bufferSubData with ArrayBuffer input")]
        public void ShouldDoMagic()
        {
            wtu.debug("Regression test for <a href='https://bugs.webkit.org/show_bug.cgi?id=41884'>https://bugs.webkit.org/show_bug.cgi?id=41884</a> : <code>Implement bufferData and bufferSubData with ArrayBuffer as input</code>");

            var gl = wtu.create3DContext(Canvas);
            wtu.shouldBeNonNull(() => gl);

            var array = new ArrayBuffer(128);
            wtu.shouldBeNonNull(() => array);

            var buf = gl.createBuffer();
            wtu.shouldBeNonNull(() => buf);

            gl.bufferData(gl.ARRAY_BUFFER, array, gl.STATIC_DRAW);
            wtu.glErrorShouldBe(gl, gl.INVALID_OPERATION);

            gl.bindBuffer(gl.ARRAY_BUFFER, buf);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);

            gl.bufferData(gl.ARRAY_BUFFER, -10, gl.STATIC_DRAW);
            wtu.glErrorShouldBe(gl, gl.INVALID_VALUE);

            // This should not crash, but the selection of the overload is ambiguous per Web IDL.
            gl.bufferData(gl.ARRAY_BUFFER, (ArrayBuffer)null, gl.STATIC_DRAW);
            gl.getError();

            gl.bufferData(gl.ARRAY_BUFFER, array, gl.STATIC_DRAW);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);

            array = new ArrayBuffer(64);

            gl.bufferSubData(gl.ARRAY_BUFFER, -10, array);
            wtu.glErrorShouldBe(gl, gl.INVALID_VALUE);

            gl.bufferSubData(gl.ARRAY_BUFFER, -10, new Float32Array(8));
            wtu.glErrorShouldBe(gl, gl.INVALID_VALUE);

            gl.bufferSubData(gl.ARRAY_BUFFER, 10, array);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);

            gl.bufferSubData(gl.ARRAY_BUFFER, 10, (ArrayBufferView)null);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
        }
    }
}