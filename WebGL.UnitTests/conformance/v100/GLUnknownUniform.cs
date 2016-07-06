using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLUnknownUniform : BaseTest
    {
        private Script vshader = new Script("vshader", "x-shader/x-vertex")
                                 {text = @"
attribute vec4 vPosition;
void main()
{
    gl_Position = vPosition;
}
"};

        private Script fshader = new Script("fshader", "x-shader/x-fragment")
                                 {text = @"
void main()
{
    gl_FragColor = vec4(1.0,0.0,0.0,1.0);
}
"};

        [Test(Description = "Tests that unknown uniforms don't cause errors.")]
        public void ShouldDoMagic()
        {
            wtu.debug("");
            wtu.debug("Canvas.getContext");

            var initWebGL = wtu.initWebGL(Canvas, vshader, fshader, new[] {"vPosition"}, new float[] {0, 0, 0, 1}, 1);
            WebGLRenderingContext gl = initWebGL.context;
            var program = initWebGL.program;
            if (gl == null)
            {
                wtu.testFailed("context does not exist");
            }
            else
            {
                wtu.testPassed("context exists");

                wtu.debug("");

                // Get the location of an unknown uniform.
                var loc = gl.getUniformLocation(program, "someUnknownUniform");
                wtu.assertMsg(() => loc == null, "location of unknown uniform should be null");
                wtu.glErrorShouldBe(gl, gl.NO_ERROR, "there should be no error from getting an unknown uniform");
                gl.uniform1f(loc, 1);
                wtu.glErrorShouldBe(gl, gl.NO_ERROR, "there should be no error from trying to set an unknown uniform");
            }

            wtu.debug("");
        }
    }
}