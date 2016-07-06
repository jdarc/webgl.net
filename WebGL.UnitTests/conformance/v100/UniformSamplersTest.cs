using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class UniformSamplersTest : BaseTest
    {
        [Test(Description = "")]
        public void ShouldDoMagic()
        {
            JSConsole.debug("Tests that only Uniform1i and Uniform1iv can be used to set");
            JSConsole.debug("sampler uniforms.");
            JSConsole.debug("");

            var canvas = Canvas; //document.getElementById("example");
            var gl = WebGLTestUtils.create3DContext(canvas);
            var program = WebGLTestUtils.setupTexturedQuad(gl);

            var textureLoc = gl.getUniformLocation(program, "tex");

            gl.uniform1i(textureLoc, 1);
            WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR, "uniform1i can set a sampler uniform");
            gl.uniform1iv(textureLoc, new Int32Array(new[] {1}));
            WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR, "uniform1iv can set a sampler uniform");
            gl.uniform1f(textureLoc, 1);
            WebGLTestUtils.glErrorShouldBe(gl, gl.INVALID_OPERATION, "uniform1f returns INVALID_OPERATION if attempting to set a sampler uniform");
            gl.uniform1fv(textureLoc, new Float32Array(new float[] {1}));
            WebGLTestUtils.glErrorShouldBe(gl, gl.INVALID_OPERATION, "uniform1fv returns INVALID_OPERATION if attempting to set a sampler uniform");
        }
    }
}