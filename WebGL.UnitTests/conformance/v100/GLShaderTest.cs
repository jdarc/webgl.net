using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLShaderTest : BaseTest
    {
        [Test(Description = "This test checks a few things about WebGL Shaders.")]
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
                wtu.debug("Checking shaders.");

                // Create the shader object
                var shader = gl.createShader(desktopGL["GEOMETRY_SHADER_ARB"]);
                wtu.assertMsg(() => shader == null, "should not be able to create GEOMETRY shader");
            }

            wtu.debug("");
        }
    }
}