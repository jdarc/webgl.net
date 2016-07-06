using System;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLVertexAttribPointer : BaseTest
    {
        [Test(Description = "This test checks vertexAttribPointer behaviors in WebGL.")]
        public void ShouldDoMagic()
        {
            wtu.debug("Canvas.getContext");

            var gl = wtu.create3DContext(Canvas);
            if (gl == null)
            {
                wtu.testFailed("context does not exist");
            }
            else
            {
                wtu.testPassed("context exists");

                wtu.debug("");
                wtu.debug("Checking gl.vertexAttribPointer.");

                const uint glFixed = 0x140C;

                gl.vertexAttribPointer(0, 3, gl.FLOAT, false, 0, 12);
                wtu.glErrorShouldBe(gl, gl.INVALID_OPERATION, "vertexAttribPointer should fail if no buffer is bound");

                var vertexObject = gl.createBuffer();
                gl.bindBuffer(gl.ARRAY_BUFFER, vertexObject);
                gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(0), gl.STATIC_DRAW);

                gl.vertexAttribPointer(0, 1, gl.INT, false, 0, 0);
                wtu.glErrorShouldBe(gl, gl.INVALID_ENUM, "vertexAttribPointer should not support INT");
                gl.vertexAttribPointer(0, 1, gl.UNSIGNED_INT, false, 0, 0);
                wtu.glErrorShouldBe(gl, gl.INVALID_ENUM, "vertexAttribPointer should not support UNSIGNED_INT");
                gl.vertexAttribPointer(0, 1, glFixed, false, 0, 0);
                wtu.glErrorShouldBe(gl, gl.INVALID_ENUM, "vertexAttribPointer should not support FIXED");

                Action<WebGLRenderingContext, dynamic, dynamic, dynamic, dynamic, dynamic, dynamic, dynamic> checkVertexAttribPointer =
                    (context, err, reason, size, type, normalize, stride, offset) =>
                    {
                        context.vertexAttribPointer(0, size, type, normalize, stride, offset);
                        wtu.glErrorShouldBe(context, err,
                                            "gl.vertexAttribPointer(0, " + size +
                                            ", gl." + wtu.glEnumToString(gl, type) +
                                            ", " + normalize +
                                            ", " + stride +
                                            ", " + offset +
                                            ") should " + (err == gl.NO_ERROR ? "succeed " : "fail ") + reason);
                    };

                var types = new[]
                            {
                                new {type = gl.BYTE, bytesPerComponent = 1},
                                new {type = gl.UNSIGNED_BYTE, bytesPerComponent = 1},
                                new {type = gl.SHORT, bytesPerComponent = 2},
                                new {type = gl.UNSIGNED_SHORT, bytesPerComponent = 2},
                                new {type = gl.FLOAT, bytesPerComponent = 4}
                            };

                for (var ii = 0; ii < types.Length; ++ii)
                {
                    var info = types[ii];
                    wtu.debug("");
                    for (var size = 1; size <= 4; ++size)
                    {
                        wtu.debug("");
                        wtu.debug("checking: " + wtu.glEnumToString(gl, info.type) + " with size " + size);
                        var bytesPerElement = size * info.bytesPerComponent;
                        var offsetSet = new[]
                                        {
                                            0,
                                            1,
                                            info.bytesPerComponent - 1,
                                            info.bytesPerComponent,
                                            info.bytesPerComponent + 1,
                                            info.bytesPerComponent * 2
                                        };
                        for (var jj = 0; jj < offsetSet.Length; ++jj)
                        {
                            var offset = offsetSet[jj];
                            int stride;
                            for (var kk = 0; kk < offsetSet.Length; ++kk)
                            {
                                stride = offsetSet[kk];
                                var err = gl.NO_ERROR;
                                var reason = "";
                                if (offset % info.bytesPerComponent != 0)
                                {
                                    reason = "because offset is bad";
                                    err = gl.INVALID_OPERATION;
                                }
                                if (stride % info.bytesPerComponent != 0)
                                {
                                    reason = "because stride is bad";
                                    err = gl.INVALID_OPERATION;
                                }
                                checkVertexAttribPointer(gl, err, reason, size, info.type, false, stride, offset);
                            }
                            stride = (int)(Math.Floor(255f / info.bytesPerComponent) * info.bytesPerComponent);

                            if (offset == 0)
                            {
                                checkVertexAttribPointer(gl, gl.NO_ERROR, "at stride limit", size, info.type, false, stride, offset);
                                checkVertexAttribPointer(gl, gl.INVALID_VALUE, "over stride limit", size, info.type, false, stride + info.bytesPerComponent, offset);
                            }
                        }
                    }
                }
            }

            wtu.debug("");
        }
    }
}