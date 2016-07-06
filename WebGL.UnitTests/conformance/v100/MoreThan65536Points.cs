using System;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class MoreThan65536Points : BaseTest
    {
        private static readonly Script vs = new Script("vs", "text/something-not-javascript")
                                            {text = @"
        attribute vec4 vPosition;
attribute vec4 vColor;
varying vec4 color;
void main() {
    gl_Position = vPosition;
    color = vColor;
}"};

        private static readonly Script fs = new Script("fs", "text/something-not-javascript")
                                            {text = @"
#if defined(GL_ES)
precision mediump float;
#endif
varying vec4 color;
void main() {
    gl_FragColor = color;
}"};

        [Test(Description = "")]
        public void ShouldDoMagic()
        {
            var initWebGL = wtu.initWebGL(Canvas, vs, fs, new[] {"vPosition", "vColor"}, new float[] {0, 0, 0, 1}, 1);
            WebGLRenderingContext gl = initWebGL.context;
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "after initWebGL");
            var bufferObjects = wtu.setupUnitQuad(gl, 0, 1);
            gl.bindBuffer(gl.ARRAY_BUFFER, bufferObjects[0]);
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(new float[]
                                                            {
                                                                -1, 1, 0, 1, 1, 0, -1, -1, 0, 1, -1, 0,
                                                                -1, 1, 0, 1, 1, 0, -1, -1, 0, 1, -1, 0
                                                            }), gl.STATIC_DRAW);
            gl.bindBuffer(gl.ARRAY_BUFFER, bufferObjects[1]);
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
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "after program setup");
            gl.enable(gl.BLEND);
            gl.disable(gl.DEPTH_TEST);

            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "after creating texture");
            var numQuads = Math.Floor((double)(65536 / 6)) + 2;
            wtu.debug("numQuads: " + numQuads);
            wtu.debug("numPoints: " + numQuads * 6);
            var indexBuf = new ArrayBuffer((int)(numQuads * 6));
            var indices = new Uint8Array(indexBuf);
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
            var indexBuffer = gl.createBuffer();
            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, indexBuffer);
            gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, indices, gl.STATIC_DRAW);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "after setting up indices");
            gl.drawElements(gl.TRIANGLES, (int)(numQuads * 6), gl.UNSIGNED_BYTE, 0);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR, "after drawing");
            wtu.checkCanvas(gl, new[] {0, 255, 0, 255}, "Should be green.");
        }
    }
}