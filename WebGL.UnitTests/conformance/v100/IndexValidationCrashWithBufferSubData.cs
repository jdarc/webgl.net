using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class IndexValidationCrashWithBufferSubData : BaseTest
    {
        [Test(Description = "Verifies that the index validation code which is within bufferSubData does not crash.")]
        public void ShouldDoMagic()
        {
            var gl = wtu.create3DContext(Canvas);

            var elementBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, elementBuffer);
            gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, 256, gl.STATIC_DRAW);
            var data = new Uint8Array(127);
            gl.bufferSubData(gl.ELEMENT_ARRAY_BUFFER, 63, data);
            wtu.testPassed("bufferSubData, when buffer object was initialized with null, did not crash");
        }
    }
}