using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLObjectGetCalls : BaseTest
    {
        [Test(Description = "Test of get calls against GL objects like getBufferParameter, etc.")]
        public void ShouldDoMagic()
        {
            var gl = wtu.create3DContext(Canvas);

            var standardVert = wtu.loadStandardVertexShader(gl);
            var standardFrag = wtu.loadStandardFragmentShader(gl);
            var standardProgram = gl.createProgram();
            gl.attachShader(standardProgram, standardVert);
            gl.attachShader(standardProgram, standardFrag);
            gl.linkProgram(standardProgram);
            var shaders = gl.getAttachedShaders(standardProgram);
            wtu.shouldBe(() => shaders.Length, 2);
            wtu.shouldBeTrue(() => shaders[0] == standardVert && shaders[1] == standardFrag || shaders[1] == standardVert && shaders[0] == standardFrag);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            wtu.shouldBeNull(() => gl.getAttachedShaders(null));
            wtu.glErrorShouldBe(gl, gl.INVALID_VALUE);
            // wtu.shouldThrow(() => gl.getAttachedShaders(standardVert));
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);

            // Test getBufferParameter
            var buffer = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
            gl.bufferData(gl.ARRAY_BUFFER, 16, gl.DYNAMIC_DRAW);
            wtu.shouldBe(() => gl.getBufferParameter(gl.ARRAY_BUFFER, gl.BUFFER_SIZE), 16);
            wtu.shouldBe(() => gl.getBufferParameter(gl.ARRAY_BUFFER, gl.BUFFER_USAGE), gl.DYNAMIC_DRAW);

            // Test getFramebufferAttachmentParameter
            var texture = gl.createTexture();
            gl.bindTexture(gl.TEXTURE_2D, texture);
            gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, 2, 2, 0, gl.RGBA, gl.UNSIGNED_BYTE,
                          new Uint8Array(new byte[]
                                         {
                                             0, 0, 0, 255,
                                             255, 255, 255, 255,
                                             255, 255, 255, 255,
                                             0, 0, 0, 255
                                         }));
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, (int)gl.LINEAR);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, (int)gl.LINEAR);
            gl.bindTexture(gl.TEXTURE_2D, null);
            var framebuffer = gl.createFramebuffer();
            gl.bindFramebuffer(gl.FRAMEBUFFER, framebuffer);
            gl.framebufferTexture2D(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.TEXTURE_2D, texture, 0);
            var renderbuffer = gl.createRenderbuffer();
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            gl.bindRenderbuffer(gl.RENDERBUFFER, renderbuffer);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            gl.renderbufferStorage(gl.RENDERBUFFER, gl.DEPTH_COMPONENT16, 2, 2);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            gl.framebufferRenderbuffer(gl.FRAMEBUFFER, gl.DEPTH_ATTACHMENT, gl.RENDERBUFFER, renderbuffer);

            // FIXME: on some machines (in particular the WebKit commit bots) the
            // framebuffer status is FRAMEBUFFER_UNSUPPORTED; more investigation
            // is needed why this is the case, because the FBO allocated
            // internally by the WebKit implementation has almost identical
            // parameters to this one. See https://bugs.webkit.org/show_bug.cgi?id=31843.
            wtu.shouldBe(() => gl.checkFramebufferStatus(gl.FRAMEBUFFER), gl.FRAMEBUFFER_COMPLETE);
            wtu.shouldBe(() => gl.getFramebufferAttachmentParameter(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.FRAMEBUFFER_ATTACHMENT_OBJECT_TYPE), (int)gl.TEXTURE);
            wtu.shouldBe(() => gl.getFramebufferAttachmentParameter(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.FRAMEBUFFER_ATTACHMENT_OBJECT_NAME), texture);
            wtu.shouldBe(() => gl.getFramebufferAttachmentParameter(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.FRAMEBUFFER_ATTACHMENT_TEXTURE_LEVEL), 0);
            wtu.shouldBe(() => gl.getFramebufferAttachmentParameter(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.FRAMEBUFFER_ATTACHMENT_TEXTURE_CUBE_MAP_FACE), 0);

            wtu.shouldBe(() => gl.getFramebufferAttachmentParameter(gl.FRAMEBUFFER, gl.DEPTH_ATTACHMENT, gl.FRAMEBUFFER_ATTACHMENT_OBJECT_TYPE), (int)gl.RENDERBUFFER);
            wtu.shouldBe(() => gl.getFramebufferAttachmentParameter(gl.FRAMEBUFFER, gl.DEPTH_ATTACHMENT, gl.FRAMEBUFFER_ATTACHMENT_OBJECT_NAME), renderbuffer);

            // Test getProgramParameter
            wtu.shouldBe(() => gl.getProgramParameter(standardProgram, gl.DELETE_STATUS), false);
            wtu.shouldBe(() => gl.getProgramParameter(standardProgram, gl.LINK_STATUS), true);
            wtu.shouldBe(() => gl.getProgramParameter(standardProgram, gl.VALIDATE_STATUS).GetType(), typeof(bool));
            wtu.shouldBe(() => gl.getProgramParameter(standardProgram, gl.ATTACHED_SHADERS), 2);
            wtu.shouldBe(() => gl.getProgramParameter(standardProgram, gl.ACTIVE_ATTRIBUTES), 2);
            wtu.shouldBe(() => gl.getProgramParameter(standardProgram, gl.ACTIVE_UNIFORMS), 1);

            // Test getRenderbufferParameter
            wtu.shouldBe(() => gl.getRenderbufferParameter(gl.RENDERBUFFER, gl.RENDERBUFFER_WIDTH), 2);
            wtu.shouldBe(() => gl.getRenderbufferParameter(gl.RENDERBUFFER, gl.RENDERBUFFER_HEIGHT), 2);
            // Note: we can't test the actual value of the internal format since
            // the implementation is allowed to change it.
            wtu.shouldBeNonZero(() => gl.getRenderbufferParameter(gl.RENDERBUFFER, gl.RENDERBUFFER_INTERNAL_FORMAT));
            wtu.shouldBeNonZero(() => gl.getRenderbufferParameter(gl.RENDERBUFFER, gl.RENDERBUFFER_DEPTH_SIZE));
            var colorbuffer = gl.createRenderbuffer();
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            gl.bindRenderbuffer(gl.RENDERBUFFER, renderbuffer);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            gl.renderbufferStorage(gl.RENDERBUFFER, gl.RGBA4, 2, 2);
            wtu.shouldBeNonZero(() => gl.getRenderbufferParameter(gl.RENDERBUFFER, gl.RENDERBUFFER_RED_SIZE));
            wtu.shouldBeNonZero(() => gl.getRenderbufferParameter(gl.RENDERBUFFER, gl.RENDERBUFFER_GREEN_SIZE));
            wtu.shouldBeNonZero(() => gl.getRenderbufferParameter(gl.RENDERBUFFER, gl.RENDERBUFFER_BLUE_SIZE));
            wtu.shouldBeNonZero(() => gl.getRenderbufferParameter(gl.RENDERBUFFER, gl.RENDERBUFFER_ALPHA_SIZE));

            // Test getShaderParameter
            wtu.shouldBe(() => gl.getShaderParameter(standardVert, gl.SHADER_TYPE), gl.VERTEX_SHADER);
            wtu.shouldBe(() => gl.getShaderParameter(standardVert, gl.DELETE_STATUS), false);
            wtu.shouldBe(() => gl.getShaderParameter(standardVert, gl.COMPILE_STATUS), true);

            // Test getTexParameter
            gl.bindFramebuffer(gl.FRAMEBUFFER, framebuffer);
            gl.bindTexture(gl.TEXTURE_2D, texture);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, (int)gl.NEAREST);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, (int)gl.NEAREST);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, (int)gl.CLAMP_TO_EDGE);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, (int)gl.CLAMP_TO_EDGE);
            wtu.shouldBe(() => gl.getTexParameter(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER), gl.NEAREST);
            wtu.shouldBe(() => gl.getTexParameter(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER), gl.NEAREST);
            wtu.shouldBe(() => gl.getTexParameter(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S), gl.CLAMP_TO_EDGE);
            wtu.shouldBe(() => gl.getTexParameter(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T), gl.CLAMP_TO_EDGE);

            // Test getUniform with all variants of data types
            // Boolean uniform variables
            var boolProgram = wtu.loadProgram(gl, "resources/boolUniformShader.vert", "resources/noopUniformShader.frag");
            wtu.shouldBe(() => gl.getProgramParameter(boolProgram, gl.LINK_STATUS), true);
            var bvalLoc = gl.getUniformLocation(boolProgram, "bval");
            var bval2Loc = gl.getUniformLocation(boolProgram, "bval2");
            var bval3Loc = gl.getUniformLocation(boolProgram, "bval3");
            var bval4Loc = gl.getUniformLocation(boolProgram, "bval4");
            gl.useProgram(boolProgram);
            gl.uniform1i(bvalLoc, 1);
            gl.uniform2i(bval2Loc, 1, 0);
            gl.uniform3i(bval3Loc, 1, 0, 1);
            gl.uniform4i(bval4Loc, 1, 0, 1, 0);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            wtu.shouldBe(() => gl.getUniform(boolProgram, bvalLoc), true);
            wtu.shouldBe(() => gl.getUniform(boolProgram, bval2Loc), new[] {true, false});
            wtu.shouldBe(() => gl.getUniform(boolProgram, bval3Loc), new[] {true, false, true});
            wtu.shouldBe(() => gl.getUniform(boolProgram, bval4Loc), new[] {true, false, true, false});
            // Integer uniform variables
            var intProgram = wtu.loadProgram(gl, "resources/intUniformShader.vert", "resources/noopUniformShader.frag");
            wtu.shouldBe(() => gl.getProgramParameter(intProgram, gl.LINK_STATUS), true);
            var ivalLoc = gl.getUniformLocation(intProgram, "ival");
            var ival2Loc = gl.getUniformLocation(intProgram, "ival2");
            var ival3Loc = gl.getUniformLocation(intProgram, "ival3");
            var ival4Loc = gl.getUniformLocation(intProgram, "ival4");
            gl.useProgram(intProgram);
            gl.uniform1i(ivalLoc, 1);
            gl.uniform2i(ival2Loc, 2, 3);
            gl.uniform3i(ival3Loc, 4, 5, 6);
            gl.uniform4i(ival4Loc, 7, 8, 9, 10);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            wtu.shouldBe(() => gl.getUniform(intProgram, ivalLoc), 1);
            wtu.shouldBe(() => gl.getUniform(intProgram, ival2Loc), new Int32Array(new[] {2, 3}));
            wtu.shouldBe(() => gl.getUniform(intProgram, ival3Loc), new Int32Array(new[] {4, 5, 6}));
            wtu.shouldBe(() => gl.getUniform(intProgram, ival4Loc), new Int32Array(new[] {7, 8, 9, 10}));
            // Float uniform variables
            var floatProgram = wtu.loadProgram(gl, "resources/floatUniformShader.vert", "resources/noopUniformShader.frag");
            wtu.shouldBe(() => gl.getProgramParameter(floatProgram, gl.LINK_STATUS), true);
            var fvalLoc = gl.getUniformLocation(floatProgram, "fval");
            var fval2Loc = gl.getUniformLocation(floatProgram, "fval2");
            var fval3Loc = gl.getUniformLocation(floatProgram, "fval3");
            var fval4Loc = gl.getUniformLocation(floatProgram, "fval4");
            gl.useProgram(floatProgram);
            gl.uniform1f(fvalLoc, 11);
            gl.uniform2f(fval2Loc, 12, 13);
            gl.uniform3f(fval3Loc, 14, 15, 16);
            gl.uniform4f(fval4Loc, 17, 18, 19, 20);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            wtu.shouldBe(() => gl.getUniform(floatProgram, fvalLoc), 11f);
            wtu.shouldBe(() => gl.getUniform(floatProgram, fval2Loc), new Float32Array(new[] {12f, 13f}));
            wtu.shouldBe(() => gl.getUniform(floatProgram, fval3Loc), new Float32Array(new[] {14f, 15f, 16f}));
            wtu.shouldBe(() => gl.getUniform(floatProgram, fval4Loc), new Float32Array(new[] {17f, 18f, 19f, 20f}));
            // Sampler uniform variables
            var samplerProgram = wtu.loadProgram(gl, "resources/noopUniformShader.vert", "resources/samplerUniformShader.frag");
            wtu.shouldBe(() => gl.getProgramParameter(samplerProgram, gl.LINK_STATUS), true);
            var s2DValLoc = gl.getUniformLocation(samplerProgram, "s2D");
            var sCubeValLoc = gl.getUniformLocation(samplerProgram, "sCube");
            gl.useProgram(samplerProgram);
            gl.uniform1i(s2DValLoc, 0);
            gl.uniform1i(sCubeValLoc, 1);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            wtu.shouldBe(() => gl.getUniform(samplerProgram, s2DValLoc), 0);
            wtu.shouldBe(() => gl.getUniform(samplerProgram, sCubeValLoc), 1);
            // Matrix uniform variables
            var matProgram = wtu.loadProgram(gl, "resources/matUniformShader.vert", "resources/noopUniformShader.frag");
            wtu.shouldBe(() => gl.getProgramParameter(matProgram, gl.LINK_STATUS), true);
            var mval2Loc = gl.getUniformLocation(matProgram, "mval2");
            var mval3Loc = gl.getUniformLocation(matProgram, "mval3");
            var mval4Loc = gl.getUniformLocation(matProgram, "mval4");
            gl.useProgram(matProgram);
            gl.uniformMatrix2fv(mval2Loc, false, 1, 2, 3, 4);
            gl.uniformMatrix3fv(mval3Loc, false, 5, 6, 7, 8, 9, 10, 11, 12, 13);
            gl.uniformMatrix4fv(mval4Loc, false, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23, 24, 25, 26, 27, 28, 29);
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
            wtu.shouldBe(() => gl.getUniform(matProgram, mval2Loc), new Float32Array(new[] {1f, 2f, 3f, 4f}));
            wtu.shouldBe(() => gl.getUniform(matProgram, mval3Loc), new Float32Array(new[] {5f, 6f, 7f, 8f, 9f, 10f, 11f, 12f, 13f}));
            wtu.shouldBe(() => gl.getUniform(matProgram, mval4Loc), new Float32Array(new[] {14f, 15f, 16f, 17f, 18f, 19f, 20f, 21f, 22f, 23f, 24f, 25f, 26f, 27f, 28f, 29f}));

            // Test getVertexAttrib
            var array = new Float32Array(new float[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16});
            gl.bindBuffer(gl.ARRAY_BUFFER, buffer);
            gl.bufferData(gl.ARRAY_BUFFER, array, gl.DYNAMIC_DRAW);
            // Vertex attribute 0 is special in that it has no current state, so
            // fetching GL_CURRENT_VERTEX_ATTRIB generates an error. Use attribute
            // 1 for these tests instead.
            gl.enableVertexAttribArray(1);
            gl.vertexAttribPointer(1, 4, gl.FLOAT, false, 0, 0);
            wtu.shouldBe(() => gl.getVertexAttrib(1, gl.VERTEX_ATTRIB_ARRAY_BUFFER_BINDING), buffer);
            wtu.shouldBe(() => gl.getVertexAttrib(1, gl.VERTEX_ATTRIB_ARRAY_ENABLED), true);
            wtu.shouldBe(() => gl.getVertexAttrib(1, gl.VERTEX_ATTRIB_ARRAY_SIZE), 4);
            // Stride MUST be the value the user put in.
            wtu.shouldBe(() => gl.getVertexAttrib(1, gl.VERTEX_ATTRIB_ARRAY_STRIDE), 0);
            wtu.shouldBe(() => gl.getVertexAttrib(1, gl.VERTEX_ATTRIB_ARRAY_TYPE), gl.FLOAT);
            wtu.shouldBe(() => gl.getVertexAttrib(1, gl.VERTEX_ATTRIB_ARRAY_NORMALIZED), false);
            gl.vertexAttribPointer(1, 4, gl.FLOAT, false, 36, 12);
            wtu.shouldBe(() => gl.getVertexAttrib(1, gl.VERTEX_ATTRIB_ARRAY_STRIDE), 36);
            wtu.shouldBe(() => gl.getVertexAttribOffset(1, gl.VERTEX_ATTRIB_ARRAY_POINTER), 12L);
            gl.disableVertexAttribArray(1);
            wtu.shouldBe(() => gl.getVertexAttrib(1, gl.VERTEX_ATTRIB_ARRAY_ENABLED), false);
            gl.vertexAttrib4f(1, 5, 6, 7, 8);
            wtu.shouldBe(() => gl.getVertexAttrib(1, gl.CURRENT_VERTEX_ATTRIB), new JSArray(5, 6, 7, 8));
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);

            // Test cases where name == 0
            gl.deleteTexture(texture);
            wtu.shouldBeNull(() => gl.getFramebufferAttachmentParameter(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.FRAMEBUFFER_ATTACHMENT_OBJECT_NAME));
            gl.deleteRenderbuffer(renderbuffer);
            wtu.shouldBeNull(() => gl.getFramebufferAttachmentParameter(gl.FRAMEBUFFER, gl.DEPTH_ATTACHMENT, gl.FRAMEBUFFER_ATTACHMENT_OBJECT_NAME));
            gl.deleteBuffer(buffer);
            wtu.shouldBeNull(() => gl.getVertexAttrib(1, gl.VERTEX_ATTRIB_ARRAY_BUFFER_BINDING));
            wtu.glErrorShouldBe(gl, gl.NO_ERROR);
        }
    }
}