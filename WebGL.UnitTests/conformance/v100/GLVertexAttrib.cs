using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLVertexAttrib : BaseTest
    {
        [Test(Description = "This test ensures WebGL implementations vertexAttrib can be set and read.")]
        public void ShouldDoMagic()
        {
            wtu.debug("");
            wtu.debug("Canvas.getContext");

            var gl = wtu.create3DContext(Canvas);
            if (gl == null)
            {
                wtu.testFailed("context does not exist");
            }
            else
            {
                wtu.testPassed("context exists");

                wtu.debug("");
                wtu.debug("Checking gl.vertexAttrib.");

                var numVertexAttribs = gl.getParameter(gl.MAX_VERTEX_ATTRIBS);
                for (uint i = 0; i < numVertexAttribs; ++i)
                {
                    var ii = i;
                    gl.vertexAttrib1fv(ii, new Float32Array(new float[] {1}));
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[0], 1f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[1], 0f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[2], 0f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[3], 1f);

                    gl.vertexAttrib2fv(ii, new Float32Array(new float[] {1, 2}));
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[0], 1f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[1], 2f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[2], 0f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[3], 1f);

                    gl.vertexAttrib3fv(ii, new Float32Array(new float[] {1, 2, 3}));
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[0], 1f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[1], 2f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[2], 3f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[3], 1f);

                    gl.vertexAttrib4fv(ii, new Float32Array(new float[] {1, 2, 3, 4}));
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[0], 1f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[1], 2f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[2], 3f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[3], 4f);

                    gl.vertexAttrib1f(ii, 5);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[0], 5f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[1], 0f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[2], 0f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[3], 1f);

                    gl.vertexAttrib2f(ii, 6, 7);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[0], 6f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[1], 7f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[2], 0f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[3], 1f);

                    gl.vertexAttrib3f(ii, 7, 8, 9);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[0], 7f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[1], 8f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[2], 9f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[3], 1f);

                    gl.vertexAttrib4f(ii, 6, 7, 8, 9);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[0], 6f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[1], 7f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[2], 8f);
                    wtu.shouldBe(() => gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[3], 9f);
                }
                wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            }

            wtu.debug("");
        }
    }
}