using System;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLVertexAttribZeroIssues : BaseTest
    {
        private static readonly Script vshader = new Script("vshader", "x-shader/x-vertex")
                                                 {text = @"
attribute vec4 vPosition;
void main()
{
    gl_Position = vPosition;
}"};

        private static readonly Script fshader = new Script("fshader", "x-shader/x-fragment") {text = @"
void main()
{
    gl_FragColor = vec4(0.0,0.0,0.0,0.0);
}"};

        [Test(Description = "Test some of the issues of the difference between attrib 0 on OpenGL vs WebGL")]
        public void ShouldDoMagic()
        {
            wtu.debug("");
            var gl = wtu.create3DContext(Canvas);

            Func<int, int, WebGLProgram> setup =
                (numVerts, attribIndex) =>
                {
                    var program = wtu.setupProgram(
                        gl, new[]
                            {
                                wtu.loadShaderFromScript(gl, vshader, gl.VERTEX_SHADER),
                                wtu.loadShaderFromScript(gl, fshader, gl.FRAGMENT_SHADER)
                            },
                        new[] {"vPosition"}, new[] {attribIndex});
                    // draw with something on attrib zero with a small number of vertices
                    var vertexObject = gl.createBuffer();
                    var g_program = program;
                    var g_attribLocation = attribIndex;
                    wtu.shouldBe(() => g_attribLocation, gl.getAttribLocation(g_program, "vPosition"));
                    gl.bindBuffer(gl.ARRAY_BUFFER, vertexObject);
                    gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(numVerts * 3), gl.STATIC_DRAW);
                    gl.vertexAttribPointer((uint)attribIndex, 3, gl.FLOAT, false, 0, 0);
                    var indices = new Uint16Array(numVerts);
                    for (var ii = 0; ii < numVerts; ++ii)
                    {
                        indices[ii] = ii;
                    }
                    var indexBuffer = gl.createBuffer();
                    gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, indexBuffer);
                    gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, indices, gl.STATIC_DRAW);
                    return program;
                };

            var p1 = setup(3, 0);
            var p2 = setup(60000, 3);

            for (var ii = 0; ii < 5; ++ii)
            {
                gl.useProgram(p1);
                gl.enableVertexAttribArray(0);
                gl.drawElements(gl.TRIANGLES, 3, gl.UNSIGNED_SHORT, 0);
                wtu.glErrorShouldBe(gl, gl.NO_ERROR, "drawing using attrib 0 with 3 verts");

                gl.useProgram(p2);
                gl.enableVertexAttribArray(3);
                gl.drawArrays(gl.LINES, 0, 60000);
                wtu.glErrorShouldBe(gl, gl.NO_ERROR, "drawing using attrib 3 with 60000 verts");
            }

            wtu.checkCanvas(gl, new[] {0, 0, 0, 0}, "canvas should be 0, 0, 0, 0");
        }
    }
}