using System;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLUniformmatrix4Fv : BaseTest
    {
        private static readonly Script vshader = new Script("vshader", "x-shader/x-vertex")
                                                 {
                                                     text = @"
attribute vec4 vPosition;
uniform mat4 world4;
uniform mat3 world3;
uniform mat2 world2;
void main()
{
  gl_Position = vec4(vPosition.xyz, world3[0].x + world2[0].x) * world4;
}"
                                                 };

        private static readonly Script fshader = new Script("fshader", "x-shader/x-fragment")
                                                 {text = @"
void main()
{
  gl_FragColor = vec4(1.0,0.0,0.0,1.0);
}
"};

        [Test(Description = "This test ensures WebGL implementations handle uniformMatrix in a OpenGL ES 2.0 spec compliant way")]
        public void ShouldDoMagic()
        {
            wtu.debug("");
            wtu.debug("Checking gl.uniformMatrix.");

            var initWebGL = wtu.initWebGL(Canvas, vshader, fshader, new[] {"vPosition"}, new float[] {0, 0, 0, 1}, 1);
            WebGLRenderingContext gl = initWebGL.context;
            WebGLProgram program = initWebGL.program;

            var methods = new Action<WebGLUniformLocation, bool, Float32Array>[5];
            methods[0] = null;
            methods[1] = null;
            methods[2] = gl.uniformMatrix2fv;
            methods[3] = gl.uniformMatrix3fv;
            methods[4] = gl.uniformMatrix4fv;

            for (var ii = 2; ii <= 4; ++ii)
            {
                var loc = gl.getUniformLocation(program, "world" + ii);
                var matLess = new JSArray();
                for (var jj = 0; jj < ii; ++jj)
                {
                    for (var ll = 0; ll < ii; ++ll)
                    {
                        if (jj == ii - 1 && ll == ii - 1)
                        {
                            continue;
                        }
                        matLess[jj * ii + ll] = (jj == ll) ? 1 : 0;
                    }
                }
                var mat = matLess.concat(new JSArray(1f));
                var matMore = mat.concat(new JSArray(1f));

                var name = "uniformMatrix" + ii + "fv";
                methods[ii](loc, false, convert(matLess));
                wtu.glErrorShouldBe(gl, gl.INVALID_VALUE, "should fail with insufficient array size for " + name);
                methods[ii](loc, false, convert(mat));
                wtu.glErrorShouldBe(gl, gl.NO_ERROR, "should succeed with correct array size for " + name);
                methods[ii](loc, false, convert(matMore));
                wtu.glErrorShouldBe(gl, gl.INVALID_VALUE, "should fail with more than 1 array size for " + name);

                mat[ii * ii - 1] = 1;
                methods[ii](loc, false, convert(mat));
                wtu.glErrorShouldBe(gl, gl.NO_ERROR, "can call " + name + "with transpose = false");
                methods[ii](loc, true, convert(mat));
                wtu.glErrorShouldBe(gl, gl.INVALID_VALUE, name + " should return INVALID_VALUE with transpose = true");
            }
        }

        private static Float32Array convert(JSArray array)
        {
            var float32Array = new Float32Array(array.length);
            for (var i = 0; i < array.length; i++)
            {
                float32Array[i] = array[i];
            }
            return float32Array;
        }
    }
}