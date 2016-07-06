using System;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class InvalidPassedParams : BaseTest
    {
        [Test(Description = "Test for invalid passed parameters")]
        public void ShouldDoMagic()
        {
            var context = wtu.create3DContext(Canvas);

            wtu.debug("");
            wtu.debug("Test createShader()");
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.createShader(context.FRAGMENT_SHADER));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.createShader(context.VERTEX_SHADER));
            wtu.shouldGenerateGLError(context, context.INVALID_ENUM, () => context.createShader(0));
            wtu.shouldGenerateGLError(context, context.INVALID_ENUM, () => context.createShader(context.TRIANGLES));

            wtu.debug("");
            wtu.debug("Test clear()");
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.clear(desktopGL["ACCUM_BUFFER_BIT"]));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.clear(desktopGL["ACCUM_BUFFER_BIT"] | context.COLOR_BUFFER_BIT));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.clear(desktopGL["ACCUM_BUFFER_BIT"] | context.COLOR_BUFFER_BIT | context.DEPTH_BUFFER_BIT | context.STENCIL_BUFFER_BIT));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.clear(context.COLOR_BUFFER_BIT | context.DEPTH_BUFFER_BIT | context.STENCIL_BUFFER_BIT));

            wtu.debug("");
            wtu.debug("Test bufferData()");
            var buffer = context.createBuffer();
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.bindBuffer(context.ARRAY_BUFFER, buffer));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.bufferData(context.ARRAY_BUFFER, 16, context.STREAM_DRAW));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.bufferData(context.ARRAY_BUFFER, 16, context.STATIC_DRAW));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.bufferData(context.ARRAY_BUFFER, 16, context.DYNAMIC_DRAW));
            wtu.shouldGenerateGLError(context, context.INVALID_ENUM, () => context.bufferData(context.ARRAY_BUFFER, 16, desktopGL["STREAM_READ"]));
            wtu.shouldGenerateGLError(context, context.INVALID_ENUM, () => context.bufferData(context.ARRAY_BUFFER, 16, desktopGL["STREAM_COPY"]));
            wtu.shouldGenerateGLError(context, context.INVALID_ENUM, () => context.bufferData(context.ARRAY_BUFFER, 16, desktopGL["STATIC_READ"]));
            wtu.shouldGenerateGLError(context, context.INVALID_ENUM, () => context.bufferData(context.ARRAY_BUFFER, 16, desktopGL["STATIC_COPY"]));
            wtu.shouldGenerateGLError(context, context.INVALID_ENUM, () => context.bufferData(context.ARRAY_BUFFER, 16, desktopGL["DYNAMIC_READ"]));
            wtu.shouldGenerateGLError(context, context.INVALID_ENUM, () => context.bufferData(context.ARRAY_BUFFER, 16, desktopGL["DYNAMIC_COPY"]));

            wtu.debug("");
            wtu.debug("Test [copy]Tex[Sub]Image2D with negative offset/width/height");
            var tex = context.createTexture();
            var pixels = new Uint8Array(2 * 2 * 4);
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.bindTexture(context.TEXTURE_2D, tex));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.texImage2D(context.TEXTURE_2D, 0, context.RGBA, -16, -16, 0, context.RGBA, context.UNSIGNED_BYTE, null));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.texImage2D(context.TEXTURE_2D, 0, context.RGBA, 16, 16, 0, context.RGBA, context.UNSIGNED_BYTE, null));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.texSubImage2D(context.TEXTURE_2D, 0, -1, -1, 2, 2, context.RGBA, context.UNSIGNED_BYTE, pixels));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.texSubImage2D(context.TEXTURE_2D, 0, 0, 0, -1, -1, context.RGBA, context.UNSIGNED_BYTE, pixels));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.texSubImage2D(context.TEXTURE_2D, 0, 0, 0, 2, 2, context.RGBA, context.UNSIGNED_BYTE, pixels));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.copyTexImage2D(context.TEXTURE_2D, 0, context.RGBA, 0, 0, -1, -1, 0));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.copyTexImage2D(context.TEXTURE_2D, 0, context.RGBA, 0, 0, 16, 16, 0));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.copyTexSubImage2D(context.TEXTURE_2D, 0, -1, -1, 0, 0, 2, 2));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.copyTexSubImage2D(context.TEXTURE_2D, 0, 0, 0, 0, 0, -1, -1));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.copyTexSubImage2D(context.TEXTURE_2D, 0, 0, 0, 0, 0, 2, 2));

            wtu.debug("");
            wtu.debug("Test renderbufferStorage() with negative width/height");
            var renderbuffer = context.createRenderbuffer();
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.bindRenderbuffer(context.RENDERBUFFER, renderbuffer));
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.renderbufferStorage(context.RENDERBUFFER, context.RGBA4, -2, -2));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.renderbufferStorage(context.RENDERBUFFER, context.RGBA4, 16, 16));

            wtu.debug("");
            wtu.debug("Test scissor() with negative width/height");
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.scissor(0, 0, -2, -2));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.scissor(0, 0, 16, 16));

            wtu.debug("");
            wtu.debug("Test viewport() with negative width/height");
            wtu.shouldGenerateGLError(context, context.INVALID_VALUE, () => context.viewport(0, 0, -2, -2));
            wtu.shouldGenerateGLError(context, context.NO_ERROR, () => context.viewport(0, 0, 16, 16));

            wtu.debug("");
            wtu.debug("Set up a program to test invalid characters");
            var invalidSet = new[] {"\"", "$", "`", "@", "\\", "'"};
            const string validUniformName = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ_1234567890";
            const string validAttribName = "abcdefghijklmnopqrstuvwxyz_ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

            Func<dynamic, dynamic, dynamic> generateShaderSource = (opt_invalidIdentifierChar, opt_invalidCommentChar) =>
                                                                   {
                                                                       var invalidIdentifierString = "";
                                                                       var invalidCommentString = "";
                                                                       if (opt_invalidIdentifierChar != null)
                                                                       {
                                                                           invalidIdentifierString += opt_invalidIdentifierChar;
                                                                       }
                                                                       if (opt_invalidCommentChar != null)
                                                                       {
                                                                           invalidCommentString += opt_invalidCommentChar;
                                                                       }
                                                                       return "uniform float " + validUniformName + invalidIdentifierString + ";\n"
                                                                              + "varying float " + validAttribName + ";\n"
                                                                              + "void main() {\n"
                                                                              + validAttribName + " = " + validUniformName + ";\n"
                                                                              + "gl_Position = vec4(0.0, 0.0, 0.0, 1.0); }\n"
                                                                              + "//.+-/*%<>[](){}^|&~=!:;,?# " + invalidCommentString;
                                                                   };
            var vShader = context.createShader(context.VERTEX_SHADER);
            context.shaderSource(vShader, generateShaderSource(null, null));
            context.compileShader(vShader);
            wtu.shouldBe(() => context.getError(), context.NO_ERROR);
            var fShader = context.createShader(context.FRAGMENT_SHADER);
            context.shaderSource(fShader, "precision highp float;\n"
                                          + "varying float " + validAttribName + ";\n"
                                          + "void main() {\n"
                                          + "gl_FragColor = vec4(" + validAttribName + ", 0.0, 0.0, 1.0); }");
            context.compileShader(fShader);
            wtu.shouldBe(() => context.getError(), context.NO_ERROR);
            var program = context.createProgram();
            context.attachShader(program, vShader);
            context.attachShader(program, fShader);
            context.linkProgram(program);
            wtu.shouldBeTrue(() => context.getProgramParameter(program, context.LINK_STATUS));
            wtu.shouldBe(() => context.getError(), context.NO_ERROR);
            context.bindAttribLocation(program, 1, validAttribName);
            wtu.shouldBe(() => context.getError(), context.NO_ERROR);
            context.getAttribLocation(program, validAttribName);
            wtu.shouldBe(() => context.getError(), context.NO_ERROR);
            context.getUniformLocation(program, validUniformName);
            wtu.shouldBe(() => context.getError(), context.NO_ERROR);

            wtu.debug("");
            wtu.debug("Test shaderSource() with invalid characters");
            for (var i = 0; i < invalidSet.Length; ++i)
            {
                var validShaderSource = generateShaderSource(null, invalidSet[i]);
                context.shaderSource(vShader, validShaderSource);
                wtu.shouldBe(() => context.getError(), context.NO_ERROR);
                var invalidShaderSource = generateShaderSource(invalidSet[i], null);
                context.shaderSource(vShader, invalidShaderSource);
                wtu.shouldBe(() => context.getError(), context.INVALID_VALUE);
            }

            wtu.debug("");
            wtu.debug("Test bindAttribLocation() with invalid characters");
            for (var i = 0; i < invalidSet.Length; ++i)
            {
                var invalidName = validAttribName + invalidSet[i];
                context.bindAttribLocation(program, 1, invalidName);
                wtu.shouldBe(() => context.getError(), context.INVALID_VALUE);
            }

            wtu.debug("");
            wtu.debug("Test getAttribLocation() with invalid characters");
            for (var i = 0; i < invalidSet.Length; ++i)
            {
                var invalidName = validAttribName + invalidSet[i];
                context.getAttribLocation(program, invalidName);
                wtu.shouldBe(() => context.getError(), context.INVALID_VALUE);
            }

            wtu.debug("");
            wtu.debug("Test getUniformLocation() with invalid characters");
            for (var i = 0; i < invalidSet.Length; ++i)
            {
                var invalidName = validUniformName + invalidSet[i];
                context.getUniformLocation(program, invalidName);
                wtu.shouldBe(() => context.getError(), context.INVALID_VALUE);
            }

            wtu.debug("");
        }
    }
}