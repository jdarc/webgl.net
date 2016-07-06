using System;
using System.Drawing;
using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLBindAttribLocationTest : BaseTest
    {
        private readonly Script vshader = new Script("vshader", "text/something-not-javascript") {text = @"
attribute vec4 vPosition;
attribute vec4 vColor;
varying vec4 color;
void main()
{
  gl_Position = vPosition;
  color = vColor;
}"};

        private readonly Script fshader = new Script("fshader", "text/something-not-javascript") {text = @"
#ifdef GL_ES
precision highp float;
#endif
varying vec4 color;
void main()
{
  gl_FragColor = color;
}"};

        [Test(Description = "This test ensures WebGL implementations don't allow names that start with 'gl_' when calling bindAttribLocation.")]
        public void ShouldDoMagic()
        {
            var gl = WebGLTestUtils.create3DContext(Canvas);
            WebGLTestUtils.shouldBeNonNull(() => gl);

            Action<int, int, Uint8Array, string> fail = (x, y, buf, shouldBe) =>
                                                        {
                                                            var i = (y * 50 + x) * 4;
                                                            var reason = "pixel at (" + x + "," + y + ") is (" + buf[i] + "," + buf[i + 1] + "," + buf[i + 2] + "," + buf[i + 3] + "), should be " + shouldBe;
                                                            WebGLTestUtils.testFailed(reason);
                                                        };

            Action pass = () => WebGLTestUtils.testPassed("drawing is correct");

            Func<uint, Script, WebGLShader> loadShader = (shaderType, shaderId) =>
                                                         {
                                                             // Get the shader source.
                                                             var shaderSource = shaderId.text;

                                                             // Create the shader object
                                                             var shader = gl.createShader(shaderType);
                                                             if (shader == null)
                                                             {
                                                                 WebGLTestUtils.debug("*** Error: unable to create shader '" + shaderId + "'");
                                                                 return null;
                                                             }

                                                             // Load the shader source
                                                             gl.shaderSource(shader, shaderSource);

                                                             // Compile the shader
                                                             gl.compileShader(shader);

                                                             // Check the compile status
                                                             var compiled = gl.getShaderParameter(shader, gl.COMPILE_STATUS);
                                                             if (!compiled)
                                                             {
                                                                 // Something went wrong during compilation; get the error
                                                                 var error = gl.getShaderInfoLog(shader);
                                                                 WebGLTestUtils.debug("*** Error compiling shader '" + shader + "':" + error);
                                                                 gl.deleteShader(shader);
                                                                 return null;
                                                             }
                                                             return shader;
                                                         };

            var program = gl.createProgram();
            gl.bindAttribLocation(program, 0, "gl_foo");
            WebGLTestUtils.glErrorShouldBe(gl, gl.INVALID_OPERATION, "bindAttribLocation should return INVALID_OPERATION if name starts with 'gl_'");
            gl.bindAttribLocation(program, 0, "gl_TexCoord0");
            WebGLTestUtils.glErrorShouldBe(gl, gl.INVALID_OPERATION, "bindAttribLocation should return INVALID_OPERATION if name starts with 'gl_'");

            var vs = loadShader(gl.VERTEX_SHADER, vshader);
            var fs = loadShader(gl.FRAGMENT_SHADER, fshader);
            gl.attachShader(program, vs);
            gl.attachShader(program, fs);

            var positions = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, positions);
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(new[] {0f, 0.5f, 0f, -0.5f, -0.5f, 0f, 0.5f, -0.5f, 0f}), gl.STATIC_DRAW);

            var colors = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, colors);
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(new float[] {0, 1, 0, 1, 0, 1, 0, 1, 0, 1, 0, 1}), gl.STATIC_DRAW);

            Action<uint, uint> setBindLocations = (colorLocation, positionLocation) =>
                                                  {
                                                      gl.bindAttribLocation(program, positionLocation, "vPosition");
                                                      gl.bindAttribLocation(program, colorLocation, "vColor");
                                                      gl.linkProgram(program);
                                                      gl.useProgram(program);
                                                      Func<bool> linked = () => (gl.getProgramParameter(program, gl.LINK_STATUS) != false);
                                                      WebGLTestUtils.assertMsg(linked, "program linked successfully");

                                                      WebGLTestUtils.debug("vPosition:" + gl.getAttribLocation(program, "vPosition"));
                                                      WebGLTestUtils.debug("vColor   :" + gl.getAttribLocation(program, "vColor"));
                                                      WebGLTestUtils.assertMsg(() => gl.getAttribLocation(program, "vPosition") == positionLocation, "location of vPosition should be " + positionLocation);
                                                      WebGLTestUtils.assertMsg(() => gl.getAttribLocation(program, "vColor") == colorLocation, "location of vColor should be " + colorLocation);

                                                      var ploc = gl.getAttribLocation(program, "vPosition");
                                                      var cloc = gl.getAttribLocation(program, "vColor");
                                                      gl.bindBuffer(gl.ARRAY_BUFFER, positions);
                                                      gl.enableVertexAttribArray(positionLocation);
                                                      gl.vertexAttribPointer(positionLocation, 3, gl.FLOAT, false, 0, 0);
                                                      gl.bindBuffer(gl.ARRAY_BUFFER, colors);
                                                      gl.enableVertexAttribArray(colorLocation);
                                                      gl.vertexAttribPointer(colorLocation, 4, gl.FLOAT, false, 0, 0);
                                                  };

            Action<uint, uint, int, int, int, int> checkDraw = (colorLocation, positionLocation, r, g, b, a) =>
                                                               {
                                                                   gl.clearColor(0, 0, 0, 1);
                                                                   gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);
                                                                   gl.drawArrays(gl.TRIANGLES, 0, 3);

                                                                   var width = 50;
                                                                   var height = 50;
                                                                   var buf = new Uint8Array(width * height * 4);
                                                                   gl.readPixels(0, 0, width, height, gl.RGBA, gl.UNSIGNED_BYTE, buf);

                                                                   Func<int, int, int, int, int, int, bool> checkPixel = (x, y, r1, g1, b1, a1) =>
                                                                                                                         {
                                                                                                                             var offset1 = (y * width + x) * 4;
                                                                                                                             if (buf[offset1 + 0] != r1 ||
                                                                                                                                 buf[offset1 + 1] != g1 ||
                                                                                                                                 buf[offset1 + 2] != b1 ||
                                                                                                                                 buf[offset1 + 3] != a1)
                                                                                                                             {
                                                                                                                                 fail(x, y, buf, "(" + r1 + "," + g1 + "," + b1 + "," + a1 + ")");
                                                                                                                                 return false;
                                                                                                                             }
                                                                                                                             return true;
                                                                                                                         };

                                                                   // Test several locations
                                                                   // First line should be all black
                                                                   var success = true;
                                                                   for (var i = 0; i < 50; ++i)
                                                                   {
                                                                       success = success && checkPixel(i, 0, 0, 0, 0, 255);
                                                                   }

                                                                   // Line 15 should be red for at least 10 rgba pixels starting 20 pixels in
                                                                   var offset = (15 * 50 + 20) * 4;
                                                                   for (var i = 0; i < 10; ++i)
                                                                   {
                                                                       success = success && checkPixel(20 + i, 15, r, g, b, a);
                                                                   }

                                                                   // Last line should be all black
                                                                   for (var i = 0; i < 50; ++i)
                                                                   {
                                                                       success = success && checkPixel(i, 49, 0, 0, 0, 255);
                                                                   }

                                                                   if (success)
                                                                   {
                                                                       pass();
                                                                   }

                                                                   gl.disableVertexAttribArray(positionLocation);
                                                                   gl.disableVertexAttribArray(colorLocation);
                                                               };

            setBindLocations(2, 3);
            checkDraw(2, 3, 0, 255, 0, 255);

            setBindLocations(0, 3);
            gl.disableVertexAttribArray(0);
            gl.vertexAttrib4f(0, 1, 0, 0, 1);
            checkDraw(0, 3, 255, 0, 0, 255);

            WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR);
        }

        public override Size PreferredSize
        {
            get { return new Size(50, 50); }
        }
    }
}