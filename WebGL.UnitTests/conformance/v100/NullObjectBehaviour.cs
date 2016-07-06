using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class NullObjectBehaviour : BaseTest
    {
        [Test(Description = "Tests calling WebGL APIs without providing the necessary objects")]
        public void ShouldDoMagic()
        {
            var context = wtu.create3DContext(Canvas);
            var program = wtu.loadStandardProgram(context);
            var shader = wtu.loadStandardVertexShader(context);

            wtu.assertMsg(() => program != null, "Program Compiled");
            wtu.assertMsg(() => shader != null, "Shader Compiled");
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.compileShader(null));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.linkProgram(null));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.attachShader(null, null));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.attachShader(program, null));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.attachShader(null, shader));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.detachShader(program, null));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.detachShader(null, shader));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.shaderSource(null, null));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.shaderSource(null, "foo"));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.bindAttribLocation(null, 0, "foo"));
            wtu.shouldThrow(() =>
                            {
                                context.bindBuffer(context.ARRAY_BUFFER, null);
                                return null;
                            });
            wtu.shouldThrow(() =>
                            {
                                context.bindFramebuffer(context.FRAMEBUFFER, null);
                                return null;
                            });
            wtu.shouldThrow(() =>
                            {
                                context.bindRenderbuffer(context.RENDERBUFFER, null);
                                return null;
                            });
            wtu.shouldThrow(() =>
                            {
                                context.bindTexture(context.TEXTURE_2D, null);
                                return null;
                            });
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.bindBuffer(context.ARRAY_BUFFER, null));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.bindFramebuffer(context.FRAMEBUFFER, null));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.bindRenderbuffer(context.RENDERBUFFER, null));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.bindTexture(context.TEXTURE_2D, null));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.bindBuffer(context.ARRAY_BUFFER, null));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.bindFramebuffer(context.FRAMEBUFFER, null));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.bindRenderbuffer(context.RENDERBUFFER, null));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.bindTexture(context.TEXTURE_2D, null));
            wtu.shouldGenerateGLError(context, context.INVALID_OPERATION, () => context.framebufferRenderbuffer(context.FRAMEBUFFER, context.DEPTH_ATTACHMENT, context.RENDERBUFFER, null));
            wtu.shouldGenerateGLError(context, context.INVALID_OPERATION, () => context.framebufferTexture2D(context.FRAMEBUFFER, context.COLOR_ATTACHMENT0, context.TEXTURE_2D, null, 0));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.getProgramParameter(null, 0u));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.getProgramInfoLog(null));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.getShaderParameter(null, 0u));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.getShaderInfoLog(null));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.getShaderSource(null));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.getUniform(null, null));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.getUniformLocation(null, "foo"));

            wtu.debug("");
            wtu.debug("check with bindings");
            context.bindBuffer(context.ARRAY_BUFFER, context.createBuffer());
            context.bindTexture(context.TEXTURE_2D, context.createTexture());
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.bufferData(context.ARRAY_BUFFER, 1, context.STATIC_DRAW));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.getBufferParameter(context.ARRAY_BUFFER, context.BUFFER_SIZE));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.texImage2D(context.TEXTURE_2D, 0, context.RGBA, 1, 1, 0, context.RGBA, context.UNSIGNED_BYTE, new Uint8Array(new byte[] {0, 0, 0, 0})));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.texParameteri(context.TEXTURE_2D, context.TEXTURE_MIN_FILTER, (int)context.NEAREST));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.getTexParameter(context.TEXTURE_2D, context.TEXTURE_MIN_FILTER));

            wtu.debug("");
            wtu.debug("check without bindings");
            context.bindBuffer(context.ARRAY_BUFFER, null);
            context.bindTexture(context.TEXTURE_2D, null);
            wtu.shouldGenerateGLError(context, context.INVALID_OPERATION, () => context.bufferData(context.ARRAY_BUFFER, 1, context.STATIC_DRAW));
            wtu.shouldGenerateGLError(context, context.INVALID_OPERATION, () => context.getBufferParameter(context.ARRAY_BUFFER, context.BUFFER_SIZE));
            wtu.shouldGenerateGLError(context, context.INVALID_OPERATION, () => context.texImage2D(context.TEXTURE_2D, 0, context.RGBA, 1, 1, 0, context.RGBA, context.UNSIGNED_BYTE, new Uint8Array(new byte[] {0, 0, 0, 0})));
            wtu.shouldGenerateGLError(context, context.INVALID_OPERATION, () => context.texParameteri(context.TEXTURE_2D, context.TEXTURE_MIN_FILTER, (int)context.NEAREST));
            wtu.shouldGenerateGLError(context, context.INVALID_OPERATION, () => context.getTexParameter(context.TEXTURE_2D, context.TEXTURE_MIN_FILTER));
        }
    }
}