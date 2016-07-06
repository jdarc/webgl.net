using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLGetShaderSource : BaseTest
    {
        private static readonly Script Vshader = new Script("vshader", "x-shader/x-vertex")
                                                 {
                                                     text = @"abc//defＮＯＴＡＳＣＩＩ"
                                                 };

        [Test(Description = "Tests that the source that goes into a shader is what comes out.")]
        public void ShouldDoMagic()
        {
            var gl = WebGLTestUtils.create3DContext(Canvas);
            var original = Vshader.text;
            var shader = gl.createShader(gl.VERTEX_SHADER);
            gl.shaderSource(shader, original);
            var source = gl.getShaderSource(shader);
            WebGLTestUtils.shouldBe(() => source, original);
            WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR, "Should be no errors.");
        }
    }
}