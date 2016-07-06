using System.Drawing;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class PointSize : BaseTest
    {
        private static readonly Script vshader =
            new Script("vshader", "x-shader/x-vertex")
            {text = @"
attribute vec3 pos;
attribute vec4 colorIn;
uniform float pointSize;
varying vec4 color;

void main()
{
    gl_PointSize = pointSize;
    color = colorIn;
    gl_Position = vec4(pos.xyz, 3.0);
}
"};

        private static readonly Script fshader =
            new Script("fshader", "x-shader/x-fragment") {text = @"
#ifdef GL_ES
precision mediump float;
#endif
varying vec4 color;

void main()
{
    gl_FragColor = color;
}"};

        [Test(Description = "Verify GL_VERTEX_PROGRAM_POINT_SIZE is enabled in WebGL")]
        public void ShouldDoMagic()
        {
            if (runTest())
            {
                wtu.testPassed("");
            }
        }

        private bool runTest()
        {
            var attributes = new WebGLContextAttributes();
            attributes.setAntialias(false);
            var initWebGL = wtu.initWebGL(Canvas, vshader, fshader, new[] {"pos", "colorIn"}, new float[] {0, 0, 0, 1}, 1, attributes);
            WebGLRenderingContext gl = initWebGL.context;
            if (gl == null)
            {
                wtu.testFailed("initWebGL(..) failed");
                return false;
            }
            gl.disable(gl.BLEND);
            gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);

            // The choice of (0.4, 0.4) ensures that the centers of the surrounding
            // pixels are not contained within the point when it is of size 1, but
            // that they definitely are when it is of size 2.
            var vertices = new Float32Array(new[] {0.4f, 0.4f, 0.0f});
            var colors = new Uint8Array(new byte[] {255, 0, 0, 255});

            var colorOffset = vertices.byteLength;

            var vbo = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, vbo);
            gl.bufferData(gl.ARRAY_BUFFER, colorOffset + colors.byteLength, gl.STATIC_DRAW);
            gl.bufferSubData(gl.ARRAY_BUFFER, 0, vertices);
            gl.bufferSubData(gl.ARRAY_BUFFER, colorOffset, colors);

            gl.vertexAttribPointer(0, 3, gl.FLOAT, false, 0, 0);
            gl.enableVertexAttribArray(0);
            gl.vertexAttribPointer(1, 4, gl.UNSIGNED_BYTE, true, 0, colorOffset);
            gl.enableVertexAttribArray(1);

            var locPointSize = gl.getUniformLocation(initWebGL.program, "pointSize");

            wtu.debug("Draw a point of size 1 and verify it does not touch any other pixels.");

            gl.uniform1f(locPointSize, 1.0f);
            gl.drawArrays(gl.POINTS, 0, vertices.length / 3);
            var buf = new Uint8Array(2 * 2 * 4);
            gl.readPixels(0, 0, 2, 2, gl.RGBA, gl.UNSIGNED_BYTE, buf);
            var index = 0;
            for (var y = 0; y < 2; ++y)
            {
                for (var x = 0; x < 2; ++x)
                {
                    var correctColor = new byte[] {0, 0, 0};
                    if (x == 1 && y == 1)
                    {
                        correctColor[0] = 255;
                    }
                    if (buf[index] != correctColor[0] || buf[index + 1] != correctColor[1] || buf[index + 2] != correctColor[2])
                    {
                        wtu.testFailed("Drawing a point of size 1 touched pixels that should not be touched");
                        return false;
                    }
                    index += 4;
                }
            }
            gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);

            wtu.debug("Draw a point of size 2 and verify it fills the appropriate region.");

            var pointSizeRange = gl.getParameter(gl.ALIASED_POINT_SIZE_RANGE);
            if (pointSizeRange[1] < 2.0f)
            {
                return true;
            }

            gl.uniform1f(locPointSize, 2.0f);
            gl.drawArrays(gl.POINTS, 0, vertices.length / 3);
            gl.readPixels(0, 0, 2, 2, gl.RGBA, gl.UNSIGNED_BYTE, buf);
            index = 0;
            for (var y = 0; y < 2; ++y)
            {
                for (var x = 0; x < 2; ++x)
                {
                    var correctColor = new byte[] {255, 0, 0};
                    if (buf[index] != correctColor[0] || buf[index + 1] != correctColor[1] || buf[index + 2] != correctColor[2])
                    {
                        wtu.testFailed("Drawing a point of size 2 failed to fill the appropriate region");
                        return false;
                    }
                    index += 4;
                }
            }

            return true;
        }

        public override Size PreferredSize
        {
            get { return new Size(2, 2); }
        }
    }
}