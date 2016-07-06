using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class IndexValidationWithResizedBuffer : BaseTest
    {
        private static readonly Script vs = new Script("vs", "x-shader/x-vertex")
                                            {text = @"
attribute vec4 vPosition;
attribute vec4 vColor;
varying vec4 color;
void main() {
    gl_Position = vPosition;
    color = vColor;
}"};

        private static readonly Script fs = new Script("fs", "x-shader/x-fragment")
                                            {text = @"
#if defined(GL_ES)
precision mediump float;
#endif
varying vec4 color;
void main() {
  gl_FragColor = color;
}
"};

        [Test(Description = "Test that updating the size of a vertex buffer is properly noticed by the WebGL implementation.")]
        public void ShouldDoMagic()
        {
            var initWebGL = wtu.initWebGL(Canvas, vs, fs, new[] {"vPosition", "vColor"}, new float[] {0, 0, 0, 1}, 1);
            WebGLRenderingContext gl = initWebGL.context;
            WebGLProgram program = initWebGL.program;

            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "after initialization");

            gl.useProgram(program);
            var vertexObject = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, vertexObject);
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(new float[] {-1, 1, 0, 1, 1, 0, -1, -1, 0, -1, -1, 0, 1, 1, 0, 1, -1, 0}), gl.STATIC_DRAW);
            gl.enableVertexAttribArray(0);
            gl.vertexAttribPointer(0, 3, gl.FLOAT, false, 0, 0);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "after vertex setup");

            var texCoordObject = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, texCoordObject);
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(new float[] {0, 0, 1, 0, 0, 1, 0, 1, 1, 0, 1, 1}), gl.STATIC_DRAW);
            gl.enableVertexAttribArray(1);
            gl.vertexAttribPointer(1, 2, gl.FLOAT, false, 0, 0);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "after texture coord setup");

            // Now resize these buffers because we want to change what we're drawing.
            gl.bindBuffer(gl.ARRAY_BUFFER, vertexObject);
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(new float[] {-1, 1, 0, 1, 1, 0, -1, -1, 0, 1, -1, 0, -1, 1, 0, 1, 1, 0, -1, -1, 0, 1, -1, 0}), gl.STATIC_DRAW);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "after vertex redefinition");
            gl.bindBuffer(gl.ARRAY_BUFFER, texCoordObject);
            gl.bufferData(gl.ARRAY_BUFFER, new Uint8Array(new byte[]
                                                          {
                                                              255, 0, 0, 255,
                                                              255, 0, 0, 255,
                                                              255, 0, 0, 255,
                                                              255, 0, 0, 255,
                                                              0, 255, 0, 255,
                                                              0, 255, 0, 255,
                                                              0, 255, 0, 255,
                                                              0, 255, 0, 255
                                                          }), gl.STATIC_DRAW);
            gl.vertexAttribPointer(1, 4, gl.UNSIGNED_BYTE, false, 0, 0);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "after texture coordinate / color redefinition");

            var numQuads = 2;
            var indices = new Uint8Array(numQuads * 6);
            for (var ii = 0; ii < numQuads; ++ii)
            {
                var offset = ii * 6;
                var quad = (ii == (numQuads - 1)) ? 4 : 0;
                indices[offset + 0] = quad + 0;
                indices[offset + 1] = quad + 1;
                indices[offset + 2] = quad + 2;
                indices[offset + 3] = quad + 2;
                indices[offset + 4] = quad + 1;
                indices[offset + 5] = quad + 3;
            }
            var indexObject = gl.createBuffer();
            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, indexObject);
            gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, indices, gl.STATIC_DRAW);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "after setting up indices");
            gl.drawElements(gl.TRIANGLES, numQuads * 6, gl.UNSIGNED_BYTE, 0);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "after drawing");

            wtu.debug("");
        }
    }
}