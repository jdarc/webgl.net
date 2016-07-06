using System;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class TypeConversionTest : BaseTest
    {
        [Test(Description = "Tests calling WebGL APIs with various types")]
        public void ShouldDoMagic()
        {
            var context = wtu.create3DContext(Canvas);
            var program = wtu.loadStandardProgram(context);
            var shader = wtu.loadStandardVertexShader(context);

            wtu.assertMsg(() => program != null, "Program Compiled");
            wtu.assertMsg(() => shader != null, "Shader Compiled");

            var loc = context.getUniformLocation(program, "u_modelViewProjMatrix");
            wtu.assertMsg(() => loc != null, "getUniformLocation succeeded");

            var buffer = context.createBuffer();
            context.bindBuffer(context.ARRAY_BUFFER, buffer);
            var texture = context.createTexture();
            context.bindTexture(context.TEXTURE_2D, texture);
            context.useProgram(program);

            var args = new dynamic[]
                       {
                           new {type = "number", value = 0},
                           new {type = "number", value = 2},
                           new {type = "string that is NaN", value = "foo",},
                           new {type = "string that is number", value = "2",},
                           // new { type= "null", value= null },
                           new {type = "Empty Array", value = new JSArray()},
                           new {type = "Object", value = new JSObject()},
                           new {type = "Array of Number", value = new JSArray(2)},
                           new {type = "Array of String", value = new[] {"foo"}},
                           new {type = "Array of String that is number", value = new[] {"0"}},
                           new {type = "Array of String that is number", value = new[] {"2"}},
                           new {type = "TypedArray", value = new Float32Array(1)}
                       };

            for (var i = 0; i < args.Length; ++i)
            {
                var argument = args[i].value;
                Action<Func<object>, Func<object>> func1 = wtu.shouldBeUndefined;
                Action<Func<object>, Func<object>> func2 = wtu.shouldBeNonNull;
                if (argument.ToString() == "2")
                {
                    func2 = wtu.shouldBeNull;
                }
                Action<Func<object>, Func<object>> func3 = wtu.shouldBeNull;
                wtu.debug("");
                wtu.debug("testing type of " + args[i].type + " : value = " + argument);
                func1(() => context.bindAttribLocation(program, argument, "foo"), null);
                func1(() => context.blendColor(argument, argument, argument, argument), null);
                func1(() => context.bufferData(context.ARRAY_BUFFER, argument, context.STATIC_DRAW), null);
                func1(() =>
                      {
                          context.bufferData(context.ARRAY_BUFFER, new Float32Array(10), context.STATIC_DRAW);
                          return null;
                      }, null);
                func1(() => context.bufferSubData(context.ARRAY_BUFFER, argument, new Float32Array(2)), null);
                func1(() => context.clear(argument), null);
                func1(() => context.clearColor(argument, 0, 0, 0), null);
                func1(() => context.clearColor(0, argument, 0, 0), null);
                func1(() => context.clearColor(0, 0, argument, 0), null);
                func1(() => context.clearColor(0, 0, 0, argument), null);
                func1(() => context.clearDepth(argument), null);
                func1(() => context.clearStencil(argument), null);
                func1(() => context.copyTexImage2D(context.TEXTURE_2D, argument, context.RGBA, 0, 0, 1, 1, 0), null);
                func1(() => context.copyTexImage2D(context.TEXTURE_2D, 0, context.RGBA, argument, 0, 1, 1, 0), null);
                func1(() => context.copyTexImage2D(context.TEXTURE_2D, 0, context.RGBA, 0, argument, 1, 1, 0), null);
                func1(() => context.copyTexImage2D(context.TEXTURE_2D, 0, context.RGBA, 0, 0, argument, 1, 0), null);
                func1(() => context.copyTexImage2D(context.TEXTURE_2D, 0, context.RGBA, 0, 0, 0, argument, 0), null);
                func1(() => context.copyTexSubImage2D(context.TEXTURE_2D, argument, 0, 0, 0, 0, 0, 0), null);
                func1(() => context.copyTexSubImage2D(context.TEXTURE_2D, 0, argument, 0, 0, 0, 0, 0), null);
                func1(() => context.copyTexSubImage2D(context.TEXTURE_2D, 0, 0, argument, 0, 0, 0, 0), null);
                func1(() => context.copyTexSubImage2D(context.TEXTURE_2D, 0, 0, 0, argument, 0, 0, 0), null);
                func1(() => context.copyTexSubImage2D(context.TEXTURE_2D, 0, 0, 0, 0, argument, 0, 0), null);
                func1(() => context.copyTexSubImage2D(context.TEXTURE_2D, 0, 0, 0, 0, 0, argument, 0), null);
                func1(() => context.copyTexSubImage2D(context.TEXTURE_2D, 0, 0, 0, 0, 0, 0, argument), null);
                func1(() => context.depthMask(argument), null);
                func1(() => context.depthRange(argument, 1), null);
                func1(() => context.depthRange(0, argument), null);
                func1(() => context.drawArrays(context.POINTS, argument, 1), null);
                func1(() => context.drawArrays(context.POINTS, 0, argument), null);
                //func1(() => context.drawElements(...), null);
                func1(() => context.enableVertexAttribArray(argument), null);
                func1(() => context.disableVertexAttribArray(argument), null);
                func2(() => context.getActiveAttrib(program, (uint)uint.Parse(argument.ToString())), null);
                func2(() => context.getActiveUniform(program, (uint)uint.Parse(argument.ToString())), null);
                func3(() => context.getParameter((uint)argument), null);
                func1(() => context.lineWidth(argument), null);
                func1(() => context.polygonOffset(argument, 0), null);
                func1(() => context.polygonOffset(0, argument), null);
                //func1(() => context.readPixels(...), null);
                //func1(() => context.renderbufferStorage(...), null);
                func1(() => context.sampleCoverage(argument, false), null);
                func1(() => context.sampleCoverage(0, argument), null);
                func1(() => context.scissor(argument, 0, 10, 10), null);
                func1(() => context.scissor(0, argument, 10, 10), null);
                func1(() => context.scissor(0, 0, argument, 10), null);
                func1(() => context.scissor(0, 0, 10, argument), null);
                func1(() => context.shaderSource(shader, argument), null);
                func1(() => context.stencilFunc(context.NEVER, argument, 255), null);
                func1(() => context.stencilFunc(context.NEVER, 0, argument), null);
                //func1(() => context.stencilFuncSeparate(GLenum face, GLenum func, GLint ref, GLuint mask), null);
                func1(() => context.stencilMask(argument), null);
                //func1(() => context.stencilMaskSeparate(context.FRONT, argument);
                //func1(() => context.texImage2D(GLenum target, GLint level, GLenum internalformat, GLsizei width, GLsizei height, GLint border, GLenum format, GLenum type, ArrayBufferView pixels), null);
                //func1(() => context.texParameterf(GLenum target, GLenum pname, GLfloat param), null);
                //func1(() => context.texParameteri(GLenum target, GLenum pname, GLint param), null);
                //func1(() => context.texSubImage2D(GLenum target, GLint level, GLint xoffset, GLint yoffset,GLsizei width, GLsizei height,GLenum format, GLenum type, ArrayBufferView pixels), null);
                func1(() => context.uniform1i(loc, argument), null);
                func1(() => context.uniform2i(loc, argument, 0), null);
                func1(() => context.uniform2i(loc, 0, argument), null);
                func1(() => context.uniform3i(loc, argument, 0, 0), null);
                func1(() => context.uniform3i(loc, 0, argument, 0), null);
                func1(() => context.uniform3i(loc, 0, 0, argument), null);
                func1(() => context.uniform4i(loc, argument, 0, 0, 0), null);
                func1(() => context.uniform4i(loc, 0, argument, 0, 0), null);
                func1(() => context.uniform4i(loc, 0, 0, argument, 0), null);
                func1(() => context.uniform4i(loc, 0, 0, 0, argument), null);
                func1(() => context.uniform1f(loc, argument), null);
                func1(() => context.uniform2f(loc, argument, 0), null);
                func1(() => context.uniform2f(loc, 0, argument), null);
                func1(() => context.uniform3f(loc, argument, 0, 0), null);
                func1(() => context.uniform3f(loc, 0, argument, 0), null);
                func1(() => context.uniform3f(loc, 0, 0, argument), null);
                func1(() => context.uniform4f(loc, argument, 0, 0, 0), null);
                func1(() => context.uniform4f(loc, 0, argument, 0, 0), null);
                func1(() => context.uniform4f(loc, 0, 0, argument, 0), null);
                func1(() => context.uniform4f(loc, 0, 0, 0, argument), null);
            }
        }
    }
}