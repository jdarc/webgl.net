using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLGetString : BaseTest
    {
        [Test(Description = "This test checks getParameter returns strings in the correct format")]
        public void ShouldDoMagic()
        {
            var gl = WebGLTestUtils.create3DContext(Canvas);
            if (gl == null)
            {
                WebGLTestUtils.testFailed("context does not exist");
            }
            else
            {
                WebGLTestUtils.testPassed("context exists");

                checkPrefix(gl, "WebGL 1.0", "VERSION", gl.VERSION);
                checkPrefix(gl, "WebGL GLSL ES 1.0", "SHADING_LANGUAGE_VERSION", gl.SHADING_LANGUAGE_VERSION);
                WebGLTestUtils.shouldBeNonNull(() => gl.getParameter(gl.VENDOR));
                WebGLTestUtils.shouldBeNonNull(() => gl.getParameter(gl.RENDERER));
                WebGLTestUtils.shouldBe(() => gl.getError(), gl.NO_ERROR);
            }
        }

        private static void checkPrefix(WebGLRenderingContext gl, string expected, string enum_name, uint enum_value)
        {
            var s = gl.getParameter(enum_value);
            if (s != null && s.Length >= expected.Length && s.Substring(0, expected.Length) == expected)
            {
                WebGLTestUtils.testPassed("getParameter(gl." + enum_name + ") correctly started with " + expected);
            }
            else
            {
                WebGLTestUtils.testFailed("getParameter(gl." + enum_name + ") did not start with " + expected);
            }
        }
    }
}