using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class ArrayBufferViewCrash : BaseTest
    {
        [Test(Description = "Verify that constructing a typed array view with no arguments and fetching its length does not crash")]
        public void ShouldDoMagic()
        {
            var len = new Uint32Array(0).length;
            wtu.testPassed("new Uint32Array().length did not crash");
        }
    }
}