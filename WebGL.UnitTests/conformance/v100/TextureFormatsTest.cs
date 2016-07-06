using System;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class TextureFormatsTest : BaseTest
    {
        [Test(Description = "This test ensures WebGL implementations allow the OpenGL ES 2.0 texture formats and do not allow DesktopGL texture formats.")]
        public void ShouldDoMagic()
        {
            wtu.debug("");
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
                wtu.debug("Checking texture formats.");

                Action<dynamic, dynamic, dynamic> createTexture = (internalFormat, format, opt_border) =>
                                                                  {
                                                                      var border = opt_border ?? 0;
                                                                      var tex = gl.createTexture();
                                                                      gl.bindTexture(gl.TEXTURE_2D, tex);
                                                                      gl.texImage2D(gl.TEXTURE_2D,
                                                                                    0, // level
                                                                                    internalFormat, // internalFormat
                                                                                    16, // width
                                                                                    16, // height
                                                                                    border, // border
                                                                                    format, // format
                                                                                    gl.UNSIGNED_BYTE, // type
                                                                                    null); // data
                                                                  };

                Action<dynamic, dynamic> testValidFormat = (internalFormat, formatName) =>
                                                           {
                                                               createTexture(internalFormat, internalFormat, null);
                                                               wtu.glErrorShouldBe(gl, gl.NO_ERROR, "was able to create texture of " + formatName);
                                                           };

                Action<dynamic, dynamic> testInvalidFormat = (internalFormat, formatName) =>
                                                             {
                                                                 createTexture(internalFormat, internalFormat, null);
                                                                 var err = gl.getError();
                                                                 if (err == gl.NO_ERROR)
                                                                 {
                                                                     wtu.testFailed("should NOT be able to create texture of type " + formatName);
                                                                 }
                                                                 else if (err == gl.INVALID_OPERATION)
                                                                 {
                                                                     wtu.testFailed("should return gl.INVALID_ENUM for type " + formatName);
                                                                 }
                                                                 else if (err == gl.INVALID_ENUM)
                                                                 {
                                                                     wtu.testPassed("not able to create invalid format: " + formatName);
                                                                 }
                                                             };

                var invalidEnums = new[]
                                   {
                                       "1",
                                       "2",
                                       "3",
                                       "4",
                                       "RGB4",
                                       "RGB5",
                                       "RGB8",
                                       "RGB10",
                                       "RGB12",
                                       "RGB16",
                                       "RGBA2",
                                       "RGBA4",
                                       "RGB5_A1",
                                       "RGBA8",
                                       "RGB10_A2",
                                       "RGBA12",
                                       "RGBA16",
                                       "BGR",
                                       "BGRA",
                                       "ALPHA4_EXT",
                                       "ALPHA8_EXT",
                                       "ALPHA12_EXT",
                                       "ALPHA16_EXT",
                                       "COMPRESSED_ALPHA",
                                       "COMPRESSED_LUMINANCE",
                                       "COMPRESSED_LUMINANCE_ALPHA",
                                       "COMPRESSED_INTENSITY",
                                       "COMPRESSED_RGB",
                                       "COMPRESSED_RGBA",
                                       "DEPTH_COMPONENT16",
                                       "DEPTH_COMPONENT24",
                                       "DEPTH_COMPONENT32",
                                       "LUMINANCE4_EXT",
                                       "LUMINANCE8_EXT",
                                       "LUMINANCE12_EXT",
                                       "LUMINANCE16_EXT",
                                       "LUMINANCE4_ALPHA4_EXT",
                                       "LUMINANCE6_ALPHA2_EXT",
                                       "LUMINANCE8_ALPHA8_EXT",
                                       "LUMINANCE12_ALPHA4_EXT",
                                       "LUMINANCE12_ALPHA12_EXT",
                                       "LUMINANCE16_ALPHA16_EXT",
                                       "INTENSITY_EXT",
                                       "INTENSITY4_EXT",
                                       "INTENSITY8_EXT",
                                       "INTENSITY12_EXT",
                                       "INTENSITY16_EXT",
                                       "RGB4_EXT",
                                       "RGB5_EXT",
                                       "RGB8_EXT",
                                       "RGB10_EXT",
                                       "RGB12_EXT",
                                       "RGB16_EXT",
                                       "RGBA2_EXT",
                                       "RGBA4_EXT",
                                       "RGB5_A1_EXT",
                                       "RGBA8_EXT",
                                       "RGB10_A2_EXT",
                                       "RGBA12_EXT",
                                       "RGBA16_EXT",
                                       "SLUMINANCE_EXT",
                                       "SLUMINANCE8_EXT",
                                       "SLUMINANCE_ALPHA_EXT",
                                       "SLUMINANCE8_ALPHA8_EXT",
                                       "SRGB_EXT",
                                       "SRGB8_EXT",
                                       "SRGB_ALPHA_EXT",
                                       "SRGB8_ALPHA8"
                                   };

                for (var ii = 0; ii < invalidEnums.Length; ++ii)
                {
                    var formatName = invalidEnums[ii];
                    if (!desktopGL.ContainsKey(formatName))
                    {
                        wtu.debug("bad format" + formatName);
                    }
                    else
                    {
                        testInvalidFormat(desktopGL[formatName], "GL_" + formatName);
                    }
                }

                var validEnums = new[]
                                 {
                                     gl.ALPHA,
                                     gl.RGB,
                                     gl.RGBA,
                                     gl.LUMINANCE,
                                     gl.LUMINANCE_ALPHA
                                 };

                for (var ii = 0; ii < validEnums.Length; ++ii)
                {
                    var formatName = validEnums[ii];
                    testValidFormat(formatName, "gl." + formatName);
                }

                wtu.debug("");
                wtu.debug("checking non 0 border parameter to gl.TexImage2D");
                createTexture(gl.RGBA, gl.RGBA, 1);
                wtu.glErrorShouldBe(gl, gl.INVALID_VALUE,
                                    "non 0 border to gl.TexImage2D should return INVALID_VALUE");

                Action checkTypes = () =>
                                    {
                                        gl = wtu.create3DContext(Canvas);
                                        var program = wtu.setupTexturedQuad(gl);

                                        gl.disable(gl.DEPTH_TEST);
                                        gl.disable(gl.BLEND);

                                        var tex = gl.createTexture();
                                        gl.bindTexture(gl.TEXTURE_2D, tex);
                                        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, (int)gl.CLAMP_TO_EDGE);
                                        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, (int)gl.CLAMP_TO_EDGE);
                                        gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, (int)gl.LINEAR);

                                        var loc = gl.getUniformLocation(program, "tex");
                                        gl.uniform1i(loc, 0);

                                        Action<dynamic, dynamic, dynamic, dynamic, dynamic, dynamic, dynamic> checkType =
                                            (r, g, b, a, type, format, buf) =>
                                            {
                                                var typeName = type;
                                                wtu.debug("");
                                                wtu.debug("checking gl.texImage2D with type: " + typeName);
                                                gl.texImage2D(gl.TEXTURE_2D,
                                                              0, // level
                                                              format, // internalFormat
                                                              2, // width
                                                              2, // height
                                                              0, // border
                                                              format, // format
                                                              type, // type
                                                              buf); // data

                                                wtu.glErrorShouldBe(gl, gl.NO_ERROR,
                                                                    "gl.texImage2D with " + typeName + " should generate NO_ERROR");

                                                wtu.drawQuad(gl, new byte[] {255, 0, 0, 255});
                                                wtu.checkCanvas(gl, new int[] {r, g, b, a},
                                                                "texture type " + typeName + " should draw with " +
                                                                r + ", " + g + ", " + b + ", " + a);
                                            };

                                        checkType(0, 255, 0, 255, gl.UNSIGNED_BYTE, gl.RGBA,
                                                  new Uint8Array(new byte[]
                                                                 {
                                                                     0, 255, 0, 255,
                                                                     0, 255, 0, 255,
                                                                     0, 255, 0, 255,
                                                                     0, 255, 0, 255
                                                                 }));
                                        checkType(0, 0, 255, 255, gl.UNSIGNED_SHORT_4_4_4_4, gl.RGBA,
                                                  new Uint16Array(new ushort[]
                                                                  {
                                                                      255, 255,
                                                                      255, 255,
                                                                      255, 255,
                                                                      255, 255
                                                                  }));
                                        checkType(0, 255, 0, 255, gl.UNSIGNED_SHORT_5_6_5, gl.RGB,
                                                  new Uint16Array(new ushort[]
                                                                  {
                                                                      2016, 2016,
                                                                      2016, 2016,
                                                                      2016, 2016,
                                                                      2016, 2016
                                                                  }));
                                        checkType(0, 0, 255, 255, gl.UNSIGNED_SHORT_5_5_5_1, gl.RGBA,
                                                  new Uint16Array(new ushort[]
                                                                  {
                                                                      63, 63,
                                                                      63, 63,
                                                                      63, 63,
                                                                      63, 63
                                                                  }));
                                    };
                checkTypes();
            }
        }
    }
}