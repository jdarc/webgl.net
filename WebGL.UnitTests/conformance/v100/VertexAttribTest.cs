using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class VertexAttribTest : BaseTest
    {
        [Test(Description = "This test ensures WebGL implementations vertexAttrib can be set and read.")]
        public void VertexAttribCanBeSetAndRead()
        {
            var gl = Context;
            var numVertexAttribs = gl.getParameter(gl.MAX_VERTEX_ATTRIBS);
            for (uint ii = 0; ii < numVertexAttribs; ++ii)
            {
                gl.vertexAttrib1fv(ii, 1);
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[0], Is.EqualTo(1));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[1], Is.EqualTo(0));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[2], Is.EqualTo(0));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[3], Is.EqualTo(1));

                gl.vertexAttrib2fv(ii, 1, 2);
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[0], Is.EqualTo(1));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[1], Is.EqualTo(2));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[2], Is.EqualTo(0));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[3], Is.EqualTo(1));

                gl.vertexAttrib3fv(ii, 1, 2, 3);
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[0], Is.EqualTo(1));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[1], Is.EqualTo(2));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[2], Is.EqualTo(3));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[3], Is.EqualTo(1));

                gl.vertexAttrib4fv(ii, 1, 2, 3, 4);
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[0], Is.EqualTo(1));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[1], Is.EqualTo(2));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[2], Is.EqualTo(3));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[3], Is.EqualTo(4));

                gl.vertexAttrib1f(ii, 5);
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[0], Is.EqualTo(5));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[1], Is.EqualTo(0));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[2], Is.EqualTo(0));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[3], Is.EqualTo(1));

                gl.vertexAttrib2f(ii, 6, 7);
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[0], Is.EqualTo(6));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[1], Is.EqualTo(7));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[2], Is.EqualTo(0));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[3], Is.EqualTo(1));

                gl.vertexAttrib3f(ii, 7, 8, 9);
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[0], Is.EqualTo(7));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[1], Is.EqualTo(8));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[2], Is.EqualTo(9));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[3], Is.EqualTo(1));

                gl.vertexAttrib4f(ii, 6, 7, 8, 9);
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[0], Is.EqualTo(6));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[1], Is.EqualTo(7));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[2], Is.EqualTo(8));
                Assert.That(gl.getVertexAttrib(ii, gl.CURRENT_VERTEX_ATTRIB)[3], Is.EqualTo(9));
            }

            WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR);
        }
    }
}