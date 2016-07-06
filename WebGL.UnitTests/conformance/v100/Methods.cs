using System;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class Methods : BaseTest
    {
        [Test(Description = "This test ensures that the WebGL context has all the methods in the specification.")]
        public void ShouldDoMagic()
        {
            var methods = new[]
                          {
                              "canvas",
                              "getContextAttributes",
                              "activeTexture",
                              "attachShader",
                              "bindAttribLocation",
                              "bindBuffer",
                              "bindFramebuffer",
                              "bindRenderbuffer",
                              "bindTexture",
                              "blendColor",
                              "blendEquation",
                              "blendEquationSeparate",
                              "blendFunc",
                              "blendFuncSeparate",
                              "bufferData",
                              "bufferSubData",
                              "checkFramebufferStatus",
                              "clear",
                              "clearColor",
                              "clearDepth",
                              "clearStencil",
                              "colorMask",
                              "compileShader",
                              "copyTexImage2D",
                              "copyTexSubImage2D",
                              "createBuffer",
                              "createFramebuffer",
                              "createProgram",
                              "createRenderbuffer",
                              "createShader",
                              "createTexture",
                              "cullFace",
                              "deleteBuffer",
                              "deleteFramebuffer",
                              "deleteProgram",
                              "deleteRenderbuffer",
                              "deleteShader",
                              "deleteTexture",
                              "depthFunc",
                              "depthMask",
                              "depthRange",
                              "detachShader",
                              "disable",
                              "disableVertexAttribArray",
                              "drawArrays",
                              "drawElements",
                              "enable",
                              "enableVertexAttribArray",
                              "finish",
                              "flush",
                              "framebufferRenderbuffer",
                              "framebufferTexture2D",
                              "frontFace",
                              "generateMipmap",
                              "getActiveAttrib",
                              "getActiveUniform",
                              "getAttachedShaders",
                              "getAttribLocation",
                              "getParameter",
                              "getBufferParameter",
                              "getError",
                              "getFramebufferAttachmentParameter",
                              "getProgramParameter",
                              "getProgramInfoLog",
                              "getRenderbufferParameter",
                              "getShaderParameter",
                              "getShaderInfoLog",
                              "getShaderSource",
                              "getTexParameter",
                              "getUniform",
                              "getUniformLocation",
                              "getVertexAttrib",
                              "getVertexAttribOffset",
                              "hint",
                              "isBuffer",
                              "isEnabled",
                              "isFramebuffer",
                              "isProgram",
                              "isRenderbuffer",
                              "isShader",
                              "isTexture",
                              "lineWidth",
                              "linkProgram",
                              "pixelStorei",
                              "polygonOffset",
                              "readPixels",
                              "renderbufferStorage",
                              "sampleCoverage",
                              "scissor",
                              "shaderSource",
                              "stencilFunc",
                              "stencilFuncSeparate",
                              "stencilMask",
                              "stencilMaskSeparate",
                              "stencilOp",
                              "stencilOpSeparate",
                              "texImage2D",
                              "texParameterf",
                              "texParameteri",
                              "texSubImage2D",
                              "uniform1f",
                              "uniform1fv",
                              "uniform1i",
                              "uniform1iv",
                              "uniform2f",
                              "uniform2fv",
                              "uniform2i",
                              "uniform2iv",
                              "uniform3f",
                              "uniform3fv",
                              "uniform3i",
                              "uniform3iv",
                              "uniform4f",
                              "uniform4fv",
                              "uniform4i",
                              "uniform4iv",
                              "uniformMatrix2fv",
                              "uniformMatrix3fv",
                              "uniformMatrix4fv",
                              "useProgram",
                              "validateProgram",
                              "vertexAttrib1f",
                              "vertexAttrib1fv",
                              "vertexAttrib2f",
                              "vertexAttrib2fv",
                              "vertexAttrib3f",
                              "vertexAttrib3fv",
                              "vertexAttrib4f",
                              "vertexAttrib4fv",
                              "vertexAttribPointer",
                              "viewport"
                          };

            Func<WebGLRenderingContext, string, bool> assertProperty =
                (v, p) =>
                {
                    try
                    {
                        if (wtu.getMethod(v, p) == null)
                        {
                            wtu.testFailed("Property does not exist: " + p);
                            return false;
                        }
                        return true;
                    }
                    catch (Exception e)
                    {
                        wtu.testFailed("Trying to access the property '" + p + "' threw an error: " + e.ToString());
                    }
                    return false;
                };

            wtu.debug("");
            wtu.debug("Canvas.getContext");

            var gl = wtu.create3DContext(Canvas);
            var passed = true;
            for (var i = 0; i < methods.Length; i++)
            {
                var r = assertProperty(gl, methods[i]);
                passed = passed && r;
            }
            if (passed)
            {
                wtu.testPassed("All WebGL methods found.");
            }

            //var extended = false;
            //foreach (var i in gl) {
            //  if (i.match(/^[a-z]/) && methods.indexOf(i) == -1) {
            //    if (!extended) {
            //      extended = true;
            //      wtu.debug("Also found the following extra methods:");
            //    }
            //    wtu.debug(i);
            //  }
            //}

            wtu.debug("");
        }
    }
}