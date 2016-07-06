using System;
using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLEnumTests : BaseTest
    {
        [Test(Description = "This test ensures various WebGL functions fail when passed non OpenGL ES 2.0 enums.")]
        public void ShouldDoMagic()
        {
            var gl = WebGLTestUtils.create3DContext(Canvas);
            if (gl == null)
            {
                WebGLTestUtils.testFailed("context does not exist");
            }
            else
            {
                WebGLTestUtils.testPassed("context exists");

                var buffer = new ArrayBuffer(2);
                var buf = new Uint16Array(buffer);
                var tex = gl.createTexture();
                gl.bindBuffer(gl.ARRAY_BUFFER, gl.createBuffer());
                WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR);

                var tests = new Action[]
                            {
                                () => gl.bindTexture(desktopGL["TEXTURE_3D"], tex),
                                () => gl.blendEquation(desktopGL["MIN"]),
                                () => gl.blendEquation(desktopGL["MAX"]),
                                () => gl.blendEquationSeparate(desktopGL["MIN"], gl.FUNC_ADD),
                                () => gl.blendEquationSeparate(desktopGL["MAX"], gl.FUNC_ADD),
                                () => gl.blendEquationSeparate(gl.FUNC_ADD, desktopGL["MIN"]),
                                () => gl.blendEquationSeparate(gl.FUNC_ADD, desktopGL["MAX"]),
                                () => gl.bufferData(gl.ARRAY_BUFFER, 3, desktopGL["STATIC_READ"]),
                                () => gl.disable(desktopGL["CLIP_PLANE0"]),
                                () => gl.disable(desktopGL["POINT_SPRITE"]),
                                () => gl.getBufferParameter(gl.ARRAY_BUFFER, desktopGL["PIXEL_PACK_BUFFER"]),
                                () => gl.hint(desktopGL["PERSPECTIVE_CORRECTION_HINT"], gl.FASTEST),
                                () => gl.isEnabled(desktopGL["CLIP_PLANE0"]),
                                () => gl.isEnabled(desktopGL["POINT_SPRITE"]),
                                () => gl.pixelStorei(desktopGL["PACK_SWAP_BYTES"], 1),
                            };

                for (var ii = 0; ii < tests.Length; ++ii)
                {
                    tests[ii]();
                    WebGLTestUtils.glErrorShouldBe(gl, gl.INVALID_ENUM, tests[ii] + " should return INVALID_ENUM.");
                }

                gl.bindTexture(gl.TEXTURE_2D, tex);
                WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR);

                tests = new Action[]
                        {
                            () => gl.getTexParameter(gl.TEXTURE_2D, desktopGL["GENERATE_MIPMAP"]),
                            () => gl.texParameteri(desktopGL["TEXTURE_3D"], gl.TEXTURE_MAG_FILTER, (int)gl.NEAREST),
                            () => gl.texParameteri(gl.TEXTURE_2D, desktopGL["GENERATE_MIPMAP"], 1)
                        };
                for (var ii = 0; ii < tests.Length; ++ii)
                {
                    tests[ii]();
                    WebGLTestUtils.glErrorShouldBe(gl, gl.INVALID_ENUM, tests[ii] + " should return INVALID_ENUM.");
                }
            }
        }
    }
}