using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLUniformBool : BaseTest
    {
        private static readonly Script vshader = new Script("vshader", "x-shader/x-vertex")
                                                 {text = @"
        attribute vec4 vPosition;
        void main()
        {
            gl_Position = vPosition;
        }
"};

        private static readonly Script fshader = new Script("fshader", "x-shader/x-fragment")
                                                 {text = @"
        uniform bool color;
        void main()
        {
            gl_FragColor = vec4(float(color),0.0,0.0,1.0);
        }
"};

        [Test(Description = "This test ensures WebGL implementations handle bool uniforms in a OpenGL ES 2.0 spec compliant way")]
        public void ShouldDoMagic()
        {
            wtu.debug("");
            wtu.debug("NOTE: Some OpenGL drivers do not handle this correctly");
            wtu.debug("");
            wtu.debug("Checking gl.uniform1f with bool.");

            var initWebGL = wtu.initWebGL(Canvas, vshader, fshader, new[] {"vPosition"}, new float[] {0, 0, 0, 1}, 1);
            WebGLRenderingContext gl = initWebGL.context;
            var loc = gl.getUniformLocation(initWebGL.program, "color");
            gl.uniform1f(loc, 1);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "should be able to set bool with gl.uniform1f");

            wtu.debug("");
        }
    }
}