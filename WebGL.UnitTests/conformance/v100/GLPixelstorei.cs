using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLPixelstorei : BaseTest
    {
        private readonly Script vshader = new Script("vshader", "x-shader/x-vertex")
                                          {text = @"
attribute vec4 vPosition;
void main() {
  gl_Position = vPosition;
}
"};

        private readonly Script fshader = new Script("fshader", "x-shader/x-fragment")
                                          {text = @"
void main() {
  gl_FragColor = vec4(1.0,0.0,0.0,1.0);
}
"};

        [Test(Description = "This test checks that drawImage and readPixels are not effected by gl.Pixelstorei(gl.PACK_ALIGNMENT) and visa versa")]
        public void ShouldDoMagic()
        {
            wtu.debug("There should be 5 red triangles on 5 black squares above");
            wtu.debug("");

            wtu.debug(string.Empty);
            wtu.debug("");

            WebGLRenderingContext gl = wtu.initWebGL(Canvas, vshader, fshader, new[] {"vPosition"}, new float[] {0, 0, 0, 1}, 1).context;

            var vertexObject = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, vertexObject);
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(new[] {0, 0.5f, 0, -0.5f, -0.5f, 0, 0.5f, -0.5f, 0f}), gl.STATIC_DRAW);
            gl.enableVertexAttribArray(0);
            gl.vertexAttribPointer(0, 3, gl.FLOAT, false, 0, 0);

            gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);
            gl.drawArrays(gl.TRIANGLES, 0, 3);

            var table = new[] {1, 2, 4, 8};
            for (var ii = 0; ii < table.Length; ++ii)
            {
                gl.pixelStorei(gl.PACK_ALIGNMENT, table[ii]);
//                ctx2d = document.getElementById("2d0" + ii).getContext("2d");
//                ctx2d.globalCompositeOperation = "copy";
//                ctx2d.drawImage(canvas3d, 0, 0);
                checkColors(gl);
                wtu.assertMsg(gl.getParameter(gl.PACK_ALIGNMENT) == table[ii], "PACK_ALIGNMENT is " + table[ii]);
            }
        }

        private static void fail(int x, int y, string name, dynamic buf, string shouldBe)
        {
            var i = (y * 50 + x) * 4;
            var reason = "pixel in " + name + " at (" + x + "," + y + ") is (" + buf[i] + "," + buf[i + 1] + "," + buf[i + 2] + "," + buf[i + 3] + "), should be " + shouldBe;
            wtu.testFailed(reason);
        }

        private static void pass(string name)
        {
            wtu.testPassed("drawing is correct in " + name);
        }

        private static void checkColors(WebGLRenderingContext gl)
        {
            var buf = new Uint8Array(50 * 50 * 4);
            gl.readPixels(0, 0, 50, 50, gl.RGBA, gl.UNSIGNED_BYTE, buf);
            checkData(buf, "3d context");
//            var imgData = ctx2d.getImageData(0, 0, 50, 50);
//            checkData(imgData.data, "2d context");
        }

        private static void checkData(dynamic buf, string name)
        {
            // Test several locations
            // First line should be all black
            for (var i = 0; i < 50; ++i)
            {
                if (buf[i * 4] != 0 || buf[i * 4 + 1] != 0 || buf[i * 4 + 2] != 0 || buf[i * 4 + 3] != 255)
                {
                    fail(i, 0, name, buf, "(0,0,0,255)");
                    return;
                }
            }

            // Line 25 should be red for at least 6 red pixels starting 22 pixels in
            var offset = (25 * 50 + 22) * 4;
            for (var i = 0; i < 6; ++i)
            {
                if (buf[offset + i * 4] != 255 || buf[offset + i * 4 + 1] != 0 || buf[offset + i * 4 + 2] != 0 || buf[offset + i * 4 + 3] != 255)
                {
                    fail(22 + i, 25, name, buf, "(255,0,0,255)");
                    return;
                }
            }

            // Last line should be all black
            offset = (49 * 50) * 4;
            for (var i = 0; i < 50; ++i)
            {
                if (buf[offset + i * 4] != 0 || buf[offset + i * 4 + 1] != 0 || buf[offset + i * 4 + 2] != 0 || buf[offset + i * 4 + 3] != 255)
                {
                    fail(i, 49, name, buf, "(0,0,0,255)");
                    return;
                }
            }

            pass(name);
        }
    }
}