using System;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class IndexValidation : BaseTest
    {
        [Test(Description = "Tests that index validation verifies the correct number of indices")]
        public void ShouldDoMagic()
        {
            var gl = wtu.create3DContext(Canvas);
            var program = wtu.loadStandardProgram(gl);

            // 3 vertices => 1 triangle, interleaved data
            var dataComplete = new Float32Array(new float[]
                                                {
                                                    0, 0, 0, 1,
                                                    0, 0, 1,
                                                    1, 0, 0, 1,
                                                    0, 0, 1,
                                                    1, 1, 1, 1,
                                                    0, 0, 1
                                                });
            var dataIncomplete = new Float32Array(new float[]
                                                  {
                                                      0, 0, 0, 1,
                                                      0, 0, 1,
                                                      1, 0, 0, 1,
                                                      0, 0, 1,
                                                      1, 1, 1, 1
                                                  });
            var indices = new Uint16Array(new ushort[] {0, 1, 2});

            wtu.debug("Testing with valid indices");

            var bufferComplete = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, bufferComplete);
            gl.bufferData(gl.ARRAY_BUFFER, dataComplete, gl.STATIC_DRAW);
            var elements = gl.createBuffer();
            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, elements);
            gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, indices, gl.STATIC_DRAW);
            gl.useProgram(program);
            var vertexLoc = gl.getAttribLocation(program, "a_vertex");
            var normalLoc = gl.getAttribLocation(program, "a_normal");
            gl.vertexAttribPointer((uint)vertexLoc, 4, gl.FLOAT, false, 7 * sizeInBytes(gl, gl.FLOAT), 0);
            gl.enableVertexAttribArray((uint)vertexLoc);
            gl.vertexAttribPointer((uint)normalLoc, 3, gl.FLOAT, false, 7 * sizeInBytes(gl, gl.FLOAT), 4 * sizeInBytes(gl, gl.FLOAT));
            gl.enableVertexAttribArray((uint)normalLoc);
            wtu.shouldBe(() => gl.checkFramebufferStatus(gl.FRAMEBUFFER), gl.FRAMEBUFFER_COMPLETE);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            wtu.shouldBeUndefined(() =>
                                  {
                                      gl.drawElements(gl.TRIANGLES, 3, gl.UNSIGNED_SHORT, 0);
                                      return null;
                                  });
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);

            wtu.debug("Testing with out-of-range indices");

            var bufferIncomplete = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, bufferIncomplete);
            gl.bufferData(gl.ARRAY_BUFFER, dataIncomplete, gl.STATIC_DRAW);
            gl.vertexAttribPointer((uint)vertexLoc, 4, gl.FLOAT, false, 7 * sizeInBytes(gl, gl.FLOAT), 0);
            gl.enableVertexAttribArray((uint)vertexLoc);
            gl.disableVertexAttribArray((uint)normalLoc);
            wtu.debug("Enable vertices, valid");
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            wtu.shouldBeUndefined(() =>
                                  {
                                      gl.drawElements(gl.TRIANGLES, 3, gl.UNSIGNED_SHORT, 0);
                                      return null;
                                  });
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            wtu.debug("Enable normals, out-of-range");
            gl.vertexAttribPointer((uint)normalLoc, 3, gl.FLOAT, false, 7 * sizeInBytes(gl, gl.FLOAT), 4 * sizeInBytes(gl, gl.FLOAT));
            gl.enableVertexAttribArray((uint)normalLoc);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            wtu.shouldBeUndefined(() =>
                                  {
                                      gl.drawElements(gl.TRIANGLES, 3, gl.UNSIGNED_SHORT, 0);
                                      return null;
                                  });
            wtu.glErrorShouldBe(gl, gl.INVALID_OPERATION);

            wtu.debug("Test with enabled attribute that does not belong to current program");

            gl.disableVertexAttribArray((uint)normalLoc);
            var extraLoc = Math.Max(vertexLoc, (double)normalLoc) + 1;
            gl.enableVertexAttribArray((uint)extraLoc);
            wtu.debug("Enable an extra attribute with null");
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            wtu.shouldBeUndefined(() =>
                                  {
                                      gl.drawElements(gl.TRIANGLES, 3, gl.UNSIGNED_SHORT, 0);
                                      return null;
                                  });
            wtu.glErrorShouldBe(gl, gl.INVALID_OPERATION);
            wtu.debug("Enable an extra attribute with insufficient data buffer");
            gl.vertexAttribPointer((uint)extraLoc, 3, gl.FLOAT, false, 7 * sizeInBytes(gl, gl.FLOAT), 4 * sizeInBytes(gl, gl.FLOAT));
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            wtu.shouldBeUndefined(() =>
                                  {
                                      gl.drawElements(gl.TRIANGLES, 3, gl.UNSIGNED_SHORT, 0);
                                      return null;
                                  });
            gl.vertexAttribPointer((uint)normalLoc, 3, gl.FLOAT, false, 7 * sizeInBytes(gl, gl.FLOAT), -2000000000 * sizeInBytes(gl, gl.FLOAT));
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            wtu.shouldBeUndefined(() =>
                                  {
                                      gl.drawElements(gl.TRIANGLES, 3, gl.UNSIGNED_SHORT, 0);
                                      return null;
                                  });
        }

        public static int sizeInBytes(WebGLRenderingContext gl, uint type)
        {
            if (type == gl.BYTE || type == gl.UNSIGNED_BYTE)
            {
                return 1;
            }
            if (type == gl.SHORT || type == gl.UNSIGNED_SHORT)
            {
                return 2;
            }
            if (type == gl.INT || type == gl.UNSIGNED_INT || type == gl.FLOAT)
            {
                return 4;
            }
            throw new Exception("unknown type");
        }
    }
}