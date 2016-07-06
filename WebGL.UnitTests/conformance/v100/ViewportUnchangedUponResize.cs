using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class ViewportUnchangedUponResize : BaseTest
    {
        private readonly Script _vshader = new Script("vshader", "x-shader/x-vertex")
                                           {
                                               text = @"
attribute vec3 g_Position;

void main()
{
    gl_Position = vec4(g_Position.x, g_Position.y, g_Position.z, 1.0);
}
"
                                           };

        private readonly Script _fshader = new Script("fshader", "x-shader/x-fragment")
                                           {
                                               text = @"
void main()
{
    gl_FragColor = vec4(1.0, 0.0, 0.0, 1.0);
}
"
                                           };

        [Test(Description = "Verifies that GL viewport does not change when canvas is resized")]
        public void ShouldDoMagic()
        {
            Canvas.setWidth(4);
            Canvas.setHeight(4);
            var result = WebGLTestUtils.initWebGL(Canvas, _vshader, _fshader, new[] {"g_Position"}, new[] {0f, 0f, 1f, 1f}, 1);
            WebGLRenderingContext gl = result.context;
            WebGLProgram program = result.program;

            var vertices = new Float32Array(new[]
                                            {
                                                1.0f, 1.0f, 0.0f,
                                                -1.0f, 1.0f, 0.0f,
                                                -1.0f, -1.0f, 0.0f,
                                                1.0f, 1.0f, 0.0f,
                                                -1.0f, -1.0f, 0.0f,
                                                1.0f, -1.0f, 0.0f
                                            });

            var vbo = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, vbo);
            gl.bufferData(gl.ARRAY_BUFFER, vertices, gl.STATIC_DRAW);

            gl.enableVertexAttribArray(0);
            gl.vertexAttribPointer(0, 3, gl.FLOAT, false, 0, 0);

            // Clear and set up
            gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);
            gl.useProgram(program);
            // Draw the triangle pair to the frame buffer
            gl.drawArrays(gl.TRIANGLES, 0, 6);

            // Ensure that the frame buffer is red at the sampled pixel
            var buf = new Uint8Array(4);
            gl.readPixels(2, 2, 1, 1, gl.RGBA, gl.UNSIGNED_BYTE, buf);
            var passed = true;
            if (buf[0] != 255 || buf[1] != 0 || buf[2] != 0 || buf[3] != 255)
            {
                WebGLTestUtils.testFailed("Pixel at (2, 2) should have been (255, 0, 0, 255), " + "was (" + buf[0] + ", " + buf[1] + ", " + buf[2] + ", " + buf[3] + ")");
                passed = false;
            }

            if (passed)
            {
                // Now resize the canvas
                WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR, "No GL errors before resizing the canvas");
                Canvas.setWidth(8);
                Canvas.setHeight(8);
                var err = gl.getError();
                // Some implementations might lost the context when resizing
                if (err == gl.CONTEXT_LOST_WEBGL)
                {
                    WebGLTestUtils.testPassed("canvas lost context on resize");
                }
                else
                {
                    WebGLTestUtils.shouldBe(() => err, gl.NO_ERROR);
                    // Do another render
                    gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);
                    gl.drawArrays(gl.TRIANGLES, 0, 6);

                    // This time, because we did not change the viewport, it should
                    // still be (0, 0, 4, 4), so only the lower-left quadrant should
                    // have been filled.
                    buf = new Uint8Array(4);
                    gl.readPixels(6, 6, 1, 1, gl.RGBA, gl.UNSIGNED_BYTE, buf);
                    if (buf[0] != 0 || buf[1] != 0 || buf[2] != 255 || buf[3] != 255)
                    {
                        WebGLTestUtils.testFailed("Pixel at (6, 6) should have been (0, 0, 255, 255), " + "was (" + buf[0] + ", " + buf[1] + ", " + buf[2] + ", " + buf[3] + ")");
                        passed = false;
                    }
                    if (passed)
                    {
                        WebGLTestUtils.testPassed("Viewport correctly did not change size during canvas resize");
                    }
                }
            }
        }
    }
}