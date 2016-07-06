using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class IsObject : BaseTest
    {
        [Test(Description = "Tests 'is' calls against non-bound and deleted objects")]
        public void ShouldDoMagic()
        {
            WebGLBuffer buffer = null;
            WebGLFramebuffer framebuffer = null;
            WebGLProgram program = null;
            WebGLRenderbuffer renderbuffer = null;
            WebGLShader shader = null;
            WebGLTexture texture = null;

            var gl = wtu.create3DContext(Canvas);

            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => buffer = gl.createBuffer());
            wtu.shouldBeFalse(() => gl.isBuffer(buffer));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.bindBuffer(gl.ARRAY_BUFFER, buffer));
            wtu.shouldBeTrue(() => gl.isBuffer(buffer));
            wtu.debug("");

            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => framebuffer = gl.createFramebuffer());
            wtu.shouldBeFalse(() => gl.isFramebuffer(framebuffer));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.bindFramebuffer(gl.FRAMEBUFFER, framebuffer));
            wtu.shouldBeTrue(() => gl.isFramebuffer(framebuffer));
            wtu.debug("");

            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => renderbuffer = gl.createRenderbuffer());
            wtu.shouldBeFalse(() => gl.isRenderbuffer(renderbuffer));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.bindRenderbuffer(gl.RENDERBUFFER, renderbuffer));
            wtu.shouldBeTrue(() => gl.isRenderbuffer(renderbuffer));
            wtu.debug("");

            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => texture = gl.createTexture());
            wtu.shouldBeFalse(() => gl.isTexture(texture));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.bindTexture(gl.TEXTURE_2D, texture));
            wtu.shouldBeTrue(() => gl.isTexture(texture));
            wtu.debug("");

            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => program = gl.createProgram());
            wtu.shouldBeTrue(() => gl.isProgram(program));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.deleteProgram(program));
            wtu.shouldBeFalse(() => gl.isProgram(program));
            wtu.debug("");

            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => shader = gl.createShader(gl.VERTEX_SHADER));
            wtu.shouldBeTrue(() => gl.isShader(shader));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.deleteShader(shader));
            wtu.shouldBeFalse(() => gl.isShader(shader));
            wtu.debug("");
        }
    }
}