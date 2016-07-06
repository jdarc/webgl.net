using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class IndexValidationCopiesIndices : BaseTest
    {
        [Test(Description = "Test that client data is always copied during bufferData and bufferSubData calls, because otherwise the data the GL uses to draw may differ from that checked by the index validation code.")]
        public void ShouldDoMagic()
        {
            var context = wtu.create3DContext(Canvas);
            var program = wtu.loadStandardProgram(context);

            context.useProgram(program);
            var vertexObject = context.createBuffer();
            context.enableVertexAttribArray(0);
            context.bindBuffer(context.ARRAY_BUFFER, vertexObject);

            // 4 vertices -> 2 triangles
            context.bufferData(context.ARRAY_BUFFER, new Float32Array(new float[] {0, 0, 0, 0, 1, 0, 1, 0, 0, 1, 1, 0}), context.STATIC_DRAW);
            context.vertexAttribPointer(0, 3, context.FLOAT, false, 0, 0);

            var indexObject = context.createBuffer();

            context.bindBuffer(context.ELEMENT_ARRAY_BUFFER, indexObject);
            var indices = new Uint16Array(new ushort[] {10000, 0, 1, 2, 3, 10000});
            context.bufferData(context.ELEMENT_ARRAY_BUFFER, indices, context.STATIC_DRAW);
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.drawElements(context.TRIANGLE_STRIP, 4, context.UNSIGNED_SHORT, 2));
            wtu.shouldGenerateGLError(context, context.INVALID_OPERATION, () => context.drawElements(context.TRIANGLE_STRIP, 4, context.UNSIGNED_SHORT, 0));
            wtu.shouldGenerateGLError(context, context.INVALID_OPERATION, () => context.drawElements(context.TRIANGLE_STRIP, 4, context.UNSIGNED_SHORT, 4));
            indices[0] = 2;
            indices[5] = 1;
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.drawElements(context.TRIANGLE_STRIP, 4, context.UNSIGNED_SHORT, 2));
            wtu.shouldGenerateGLError(context, context.INVALID_OPERATION, () => context.drawElements(context.TRIANGLE_STRIP, 4, context.UNSIGNED_SHORT, 0));
            wtu.shouldGenerateGLError(context, context.INVALID_OPERATION, () => context.drawElements(context.TRIANGLE_STRIP, 4, context.UNSIGNED_SHORT, 4));

            wtu.debug("");
        }
    }
}