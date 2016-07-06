using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class ArrayBufferCrash : BaseTest
    {
        [Test(Description = "Test ArrayBuffer.byteLength")]
        public void ShouldDoMagic()
        {
            var v = new ArrayBuffer(0).byteLength;
            wtu.testPassed("new ArrayBuffer().byteLength did not crash");
        }
    }
}