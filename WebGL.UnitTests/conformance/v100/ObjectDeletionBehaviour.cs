using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class ObjectDeletionBehaviour : BaseTest
    {
        [Test(Description = "Tests deletion behavior for texture, renderbuffer, shader, and program")]
        public void ShouldDoMagic()
        {
            var gl = wtu.create3DContext(Canvas);

            wtu.debug("");
            wtu.debug("shader and program deletion");

            var vertexShader = wtu.loadStandardVertexShader(gl);
            wtu.assertMsg(() => vertexShader != null, "vertex shader loaded");
            var fragmentShader = wtu.loadStandardFragmentShader(gl);
            wtu.assertMsg(() => fragmentShader != null, "fragment shader loaded");

            var program = gl.createProgram();
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.attachShader(program, vertexShader));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.attachShader(program, fragmentShader));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.linkProgram(program));
            wtu.shouldBeTrue(() => gl.getProgramParameter(program, gl.LINK_STATUS));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.useProgram(program));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.deleteShader(vertexShader));
            wtu.shouldBeTrue(() => gl.isShader(vertexShader));
            wtu.shouldBeTrue(() => gl.getShaderParameter(vertexShader, gl.DELETE_STATUS));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.detachShader(program, vertexShader));
            wtu.shouldBeFalse(() => gl.isShader(vertexShader));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.deleteShader(fragmentShader));
            wtu.shouldBeTrue(() => gl.isShader(fragmentShader));
            wtu.shouldBeTrue(() => gl.getShaderParameter(fragmentShader, gl.DELETE_STATUS));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.deleteProgram(program));
            wtu.shouldBeTrue(() => gl.isProgram(program));
            wtu.shouldBeTrue(() => gl.getProgramParameter(program, gl.DELETE_STATUS));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.useProgram(null));
            wtu.shouldBeFalse(() => gl.isProgram(program));
            wtu.shouldBeFalse(() => gl.isShader(fragmentShader));

            wtu.debug("");
            wtu.debug("texture deletion");

            var fbo = gl.createFramebuffer();
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.bindFramebuffer(gl.FRAMEBUFFER, fbo));

            var tex = gl.createTexture();
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.bindTexture(gl.TEXTURE_2D, tex));
            wtu.shouldBe(() => gl.getParameter(gl.TEXTURE_BINDING_2D), tex);
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.framebufferTexture2D(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.TEXTURE_2D, tex, 0));
            wtu.shouldBe(() => gl.getFramebufferAttachmentParameter(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.FRAMEBUFFER_ATTACHMENT_OBJECT_NAME), tex);
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.deleteTexture(tex));
            // Deleting a texture bound to the currently-bound fbo is the same as
            // detaching the textue from fbo first, then delete the texture.
            wtu.shouldBeNull(() => gl.getFramebufferAttachmentParameter(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.FRAMEBUFFER_ATTACHMENT_OBJECT_NAME));
            wtu.shouldBeFalse(() => gl.isTexture(tex));
            wtu.shouldBeNull(() => gl.getParameter(gl.TEXTURE_BINDING_2D));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.bindTexture(gl.TEXTURE_2D, tex));
            wtu.shouldBeNull(() => gl.getParameter(gl.TEXTURE_BINDING_2D));

            var texCubeMap = gl.createTexture();
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.bindTexture(gl.TEXTURE_CUBE_MAP, texCubeMap));
            wtu.shouldBe(() => gl.getParameter(gl.TEXTURE_BINDING_CUBE_MAP), texCubeMap);
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.deleteTexture(texCubeMap));
            wtu.shouldBeFalse(() => gl.isTexture(texCubeMap));
            wtu.shouldBeNull(() => gl.getParameter(gl.TEXTURE_BINDING_CUBE_MAP));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.bindTexture(gl.TEXTURE_CUBE_MAP, texCubeMap));
            wtu.shouldBeNull(() => gl.getParameter(gl.TEXTURE_BINDING_CUBE_MAP));

            wtu.debug("");
            wtu.debug("renderbuffer deletion");

            var rbo = gl.createRenderbuffer();
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.bindRenderbuffer(gl.RENDERBUFFER, rbo));
            wtu.shouldBe(() => gl.getParameter(gl.RENDERBUFFER_BINDING), rbo);
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.framebufferRenderbuffer(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.RENDERBUFFER, rbo));
            wtu.shouldBe(() => gl.getFramebufferAttachmentParameter(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.FRAMEBUFFER_ATTACHMENT_OBJECT_NAME), rbo);
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.deleteRenderbuffer(rbo));
            // Deleting a renderbuffer bound to the currently-bound fbo is the same as
            // detaching the renderbuffer from fbo first, then delete the renderbuffer.
            wtu.shouldBeNull(() => gl.getFramebufferAttachmentParameter(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.FRAMEBUFFER_ATTACHMENT_OBJECT_NAME));
            wtu.shouldBeFalse(() => gl.isRenderbuffer(rbo));
            wtu.shouldBeNull(() => gl.getParameter(gl.RENDERBUFFER_BINDING));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.bindRenderbuffer(gl.RENDERBUFFER, rbo));
            wtu.shouldBeNull(() => gl.getParameter(gl.RENDERBUFFER_BINDING));

            wtu.debug("");
            wtu.debug("buffer deletion");

            var buffer = gl.createBuffer();
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.bindBuffer(gl.ARRAY_BUFFER, buffer));
            wtu.shouldBe(() => gl.getParameter(gl.ARRAY_BUFFER_BINDING), buffer);
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.deleteBuffer(buffer));
            wtu.shouldBeFalse(() => gl.isBuffer(buffer));
            wtu.shouldBeNull(() => gl.getParameter(gl.ARRAY_BUFFER_BINDING));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.bindBuffer(gl.ARRAY_BUFFER, buffer));
            wtu.shouldBeNull(() => gl.getParameter(gl.ARRAY_BUFFER_BINDING));

            var bufferElement = gl.createBuffer();
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, bufferElement));
            wtu.shouldBe(() => gl.getParameter(gl.ELEMENT_ARRAY_BUFFER_BINDING), bufferElement);
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.deleteBuffer(bufferElement));
            wtu.shouldBeFalse(() => gl.isBuffer(bufferElement));
            wtu.shouldBeNull(() => gl.getParameter(gl.ELEMENT_ARRAY_BUFFER_BINDING));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, bufferElement));
            wtu.shouldBeNull(() => gl.getParameter(gl.ELEMENT_ARRAY_BUFFER_BINDING));

            wtu.debug("");
            wtu.debug("framebuffer deletion");

            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.bindFramebuffer(gl.FRAMEBUFFER, fbo));
            wtu.shouldBe(() => gl.getParameter(gl.FRAMEBUFFER_BINDING), fbo);
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.deleteFramebuffer(fbo));
            wtu.shouldBeFalse(() => gl.isFramebuffer(fbo));
            wtu.shouldBeNull(() => gl.getParameter(gl.FRAMEBUFFER_BINDING));
            wtu.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.bindFramebuffer(gl.FRAMEBUFFER, fbo));
            wtu.shouldBeNull(() => gl.getParameter(gl.FRAMEBUFFER_BINDING));
        }
    }
}