using System;
using System.Drawing;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class Triangle : BaseTest
    {
        private static readonly Script vshader = new Script("vshader", "x-shader/x-vertex") {text = @"
        attribute vec4 vPosition;
        void main()
        {
            gl_Position = vPosition;
        }"};

        private static readonly Script fshader = new Script("fshader", "x-shader/x-fragment") {text = @"
        void main()
        {
            gl_FragColor = vec4(1.0,0.0,0.0,1.0);
        }"};

        [Test(Description = "")]
        public void ShouldDoMagic()
        {
            Action<dynamic, dynamic, dynamic, dynamic> fail =
                (x, y, buffer, shouldBe) =>
                {
                    var i = (y * 50 + x) * 4;
                    var reason = "pixel at (" + x + "," + y + ") is (" + buffer[i] + "," + buffer[i + 1] + "," + buffer[i + 2] + "," + buffer[i + 3] + "), should be " + shouldBe;
                    wtu.testFailed(reason);
                };

            Action pass = () => wtu.testPassed("drawing is correct");

            WebGLRenderingContext gl = wtu.initWebGL(Canvas, vshader, fshader, new[] {"vPosition"}, new float[] {0, 0, 0, 1}, 1).context;

            var vertexObject = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, vertexObject);
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(new[] {0f, 0.5f, 0f, -0.5f, -0.5f, 0f, 0.5f, -0.5f, 0f}), gl.STATIC_DRAW);
            gl.enableVertexAttribArray(0);
            gl.vertexAttribPointer(0, 3, gl.FLOAT, false, 0, 0);

            gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);
            gl.drawArrays(gl.TRIANGLES, 0, 3);

            var buf = new Uint8Array(50 * 50 * 4);
            gl.readPixels(0, 0, 50, 50, gl.RGBA, gl.UNSIGNED_BYTE, buf);

            // Test several locations
            // First line should be all black
            for (var i = 0; i < 50; ++i)
            {
                if (buf[i * 4] != 0 || buf[i * 4 + 1] != 0 || buf[i * 4 + 2] != 0 || buf[i * 4 + 3] != 255)
                {
                    fail(i, 0, buf, "(0,0,0,255)");
                    return;
                }
            }

            // Line 15 should be red for at least 10 red pixels starting 20 pixels in
            var offset = (15 * 50 + 20) * 4;
            for (var i = 0; i < 10; ++i)
            {
                if (buf[offset + i * 4] != 255 || buf[offset + i * 4 + 1] != 0 || buf[offset + i * 4 + 2] != 0 || buf[offset + i * 4 + 3] != 255)
                {
                    fail(20 + i, 15, buf, "(255,0,0,255)");
                    return;
                }
            }
            // Last line should be all black
            offset = (49 * 50) * 4;
            for (var i = 0; i < 50; ++i)
            {
                if (buf[offset + i * 4] != 0 || buf[offset + i * 4 + 1] != 0 || buf[offset + i * 4 + 2] != 0 || buf[offset + i * 4 + 3] != 255)
                {
                    fail(i, 49, buf, "(0,0,0,255)");
                    return;
                }
            }

            pass();
        }

        public override Size PreferredSize
        {
            get { return new Size(50, 50); }
        }
    }
}