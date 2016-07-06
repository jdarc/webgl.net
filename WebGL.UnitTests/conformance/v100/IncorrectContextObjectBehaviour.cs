using System.Windows.Forms;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class IncorrectContextObjectBehaviour : BaseTest
    {
        [Test(Description = "Tests calling WebGL APIs with objects from other contexts")]
        public void ShouldDoMagic()
        {
            var contextA = wtu.create3DContext(Canvas);
            var contextB = wtu.create3DContext(new HTMLCanvasElement(new Control()));
            var programA = wtu.loadStandardProgram(contextA);
            var programB = wtu.loadStandardProgram(contextB);
            var shaderA = wtu.loadStandardVertexShader(contextA);
            var shaderB = wtu.loadStandardVertexShader(contextB);
            var textureA = contextA.createTexture();
            var textureB = contextB.createTexture();
            var frameBufferA = contextA.createFramebuffer();
            var frameBufferB = contextB.createFramebuffer();
            var renderBufferA = contextA.createRenderbuffer();
            var renderBufferB = contextB.createRenderbuffer();
            var locationA = contextA.getUniformLocation(programA, "u_modelViewProjMatrix");
            var locationB = contextB.getUniformLocation(programB, "u_modelViewProjMatrix");

            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.compileShader(shaderB));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.linkProgram(programB));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.attachShader(programA, shaderB));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.attachShader(programB, shaderA));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.attachShader(programB, shaderB));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.detachShader(programA, shaderB));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.detachShader(programB, shaderA));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.detachShader(programB, shaderB));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.shaderSource(shaderB, "foo"));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.bindAttribLocation(programB, 0, "foo"));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.bindFramebuffer(contextA.FRAMEBUFFER, frameBufferB));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.bindRenderbuffer(contextA.RENDERBUFFER, renderBufferB));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.bindTexture(contextA.TEXTURE_2D, textureB));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.framebufferRenderbuffer(contextA.FRAMEBUFFER, contextA.DEPTH_ATTACHMENT, contextA.RENDERBUFFER, renderBufferB));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.framebufferTexture2D(contextA.FRAMEBUFFER, contextA.COLOR_ATTACHMENT0, contextA.TEXTURE_2D, textureB, 0));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.getProgramParameter(programB, 0));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.getProgramInfoLog(programB));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.getShaderParameter(shaderB, 0));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.getShaderInfoLog(shaderB));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.getShaderSource(shaderB));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.getUniform(programB, locationA));
            wtu.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.getUniformLocation(programB, "u_modelViewProjMatrix"));
        }
    }
}