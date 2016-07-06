using System;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLUniformArrays : BaseTest
    {
        private Script vshader = new Script("vshader", "x-shader/x-vertex")
                                 {
                                     text = @"
    attribute vec4 vPosition;
    void main()
    {
        gl_Position = vPosition;
    }
"
                                 };

        private Script fshader = new Script("fshader", "x-shader/x-fragment")
                                 {
                                     text = @"
    #ifdef GL_ES
    precision mediump float;
    #endif
    uniform $type color[3];
    void main()
    {
        gl_FragColor = vec4(color[0]$elem, color[1]$elem, color[2]$elem, 1);
    }
"
                                 };

        [Test(Description = "This test ensures WebGL implementations handle uniform arrays correctly.")]
        public void ShouldDoMagic()
        {
            Func<dynamic, dynamic, dynamic, dynamic> loadShader =
                (ctx, shaderType, shaderSource) =>
                {
                    // Create the shader object
                    var shader = ctx.createShader(shaderType);
                    if (shader == null)
                    {
                        wtu.debug("*** Error: unable to create shader '" + shader + "'");
                        return null;
                    }

                    // Load the shader source
                    ctx.shaderSource(shader, shaderSource);

                    // Compile the shader
                    ctx.compileShader(shader);

                    // Check the compile status
                    var compiled = ctx.getShaderParameter(shader, ctx.COMPILE_STATUS);
                    if (!compiled)
                    {
                        // Something went wrong during compilation; get the error
                        var error = ctx.getShaderInfoLog(shader);
                        wtu.debug("*** Error compiling shader '" + shader + "':" + error);
                        ctx.deleteShader(shader);
                        return null;
                    }

                    return shader;
                };

            Func<dynamic, dynamic, dynamic, dynamic> loadProgram =
                (ctx, vertexShaderSrc, fragmentShaderSrc) =>
                {
                    var program = ctx.createProgram();
                    var vShader = loadShader(ctx, ctx.VERTEX_SHADER, vertexShaderSrc);
                    var fShader = loadShader(ctx, ctx.FRAGMENT_SHADER, fragmentShaderSrc);
                    ctx.attachShader(program, vShader);
                    ctx.attachShader(program, fShader);
                    ctx.linkProgram(program);
                    var linked = ctx.getProgramParameter(program, ctx.LINK_STATUS);
                    if (!linked)
                    {
                        // something went wrong with the link
                        var error = ctx.getProgramInfoLog(ctx.program);
                        wtu.debug("Error in program linking:" + error);
                        ctx.deleteProgram(ctx.program);
                        program = null;
                    }
                    return program;
                };

            wtu.debug("");

            var gl = wtu.create3DContext(Canvas);

            var vSrc = vshader.text;
            var fTemplate = fshader.text;

            dynamic[] typeInfos = new[]
                                  {
                                      new
                                      {
                                          type = "float",
                                          jsTypeOf = "number",
                                          setter = "uniform1fv",
                                          elem = "",
                                          numSrcValues = 3,
                                          illegalSet = (Action<WebGLUniformLocation>)null,
                                          invalidSet = (Action<WebGLUniformLocation>)(loc => gl.uniform2fv(loc, 1, 2)),
                                          srcValueAsString = (Func<dynamic, dynamic, string>)((index, srcValues) => srcValues[index].ToString()),
                                          returnValueAsString = (Func<dynamic, string>)(value => value.ToString() ?? "null"),
                                          checkType = (Func<dynamic, bool>)(value => WebGLTestUtils.isNumber(value)),
                                          checkValue = (Func<dynamic, dynamic, dynamic, bool>)((typeInfo, index, value) => typeInfo.srcValues[index] == value),
                                          srcValues = new Float32Array(new float[] {16, 15, 14}),
                                          srcValuesLess = new Float32Array(0),
                                          srcValuesNonMultiple = (Float32Array)null
                                      },
                                      new
                                      {
                                          type = "float",
                                          jsTypeOf = "number",
                                          setter = "uniform1fv",
                                          elem = "",
                                          numSrcValues = 3,
                                          illegalSet = (Action<WebGLUniformLocation>)null,
                                          invalidSet = (Action<WebGLUniformLocation>)(loc => gl.uniform2fv(loc, 1, 2)),
                                          srcValueAsString = (Func<dynamic, dynamic, string>)((index, srcValues) => srcValues[index].ToString()),
                                          returnValueAsString = (Func<dynamic, string>)(value => value.ToString() ?? "null"),
                                          checkType = (Func<dynamic, bool>)(value => wtu.isNumber(value)),
                                          checkValue = (Func<dynamic, dynamic, dynamic, bool>)((typeInfo, index, value) => typeInfo.srcValues[index] == value),
                                          srcValues = new Float32Array(new float[] {16, 15, 14}),
                                          srcValuesLess = new Float32Array(0),
                                          srcValuesNonMultiple = (Float32Array)null,
                                      },
                                      new
                                      {
                                          type = "vec2",
                                          jsTypeOf = "Float32Array",
                                          setter = "uniform2fv",
                                          elem = "[1]",
                                          numSrcValues = 3,
                                          illegalSet = (Action<WebGLUniformLocation>)(loc => gl.uniform1fv(loc, new Float32Array(new float[] {2}))),
                                          invalidSet = (Action<WebGLUniformLocation>)(loc => gl.uniform1fv(loc, 2)),
                                          srcValueAsString = (Func<dynamic, dynamic, string>)((index, srcValues) => "[" + srcValues[index * 2 + 0].ToString() + ", " + srcValues[index * 2 + 1].ToString() + "]"),
                                          returnValueAsString = (Func<dynamic, string>)(value => value == null ? "null" : "[" + value[0] + ", " + value[1] + "]"),
                                          checkType = (Func<dynamic, bool>)(value => value != null && wtu.isNumber(value.length) && value.length == 2),
                                          checkValue = (Func<dynamic, dynamic, dynamic, bool>)((typeInfo, index, value) => value != null && typeInfo.srcValues[index * 2 + 0] == value[0] && typeInfo.srcValues[index * 2 + 1] == value[1]),
                                          srcValues = new Float32Array(new float[] {16, 15, 14, 13, 12, 11}),
                                          srcValuesLess = new Float32Array(new float[] {16}),
                                          srcValuesNonMultiple = new Float32Array(new float[] {16, 15, 14, 13, 12, 11, 10}),
                                      },
                                      new
                                      {
                                          type = "vec3",
                                          jsTypeOf = "Float32Array",
                                          setter = "uniform3fv",
                                          elem = "[2]",
                                          numSrcValues = 3,
                                          illegalSet = (Action<WebGLUniformLocation>)(loc => gl.uniform1fv(loc, 2)),
                                          invalidSet = (Action<WebGLUniformLocation>)(loc => gl.uniform1fv(loc, new Float32Array(new float[] {2}))),
                                          srcValueAsString =
                                          (Func<dynamic, dynamic, string>)((index, srcValues) => "[" + srcValues[index * 3 + 0].toString() + ", " + srcValues[index * 3 + 1].toString() + ", " + srcValues[index * 3 + 2].toString() + "]"),
                                          returnValueAsString = (Func<dynamic, string>)(value => value == null ? "null" : "[" + value[0] + ", " + value[1] + ", " + value[2] + "]"),
                                          checkType = (Func<dynamic, bool>)(value => value != null && wtu.isNumber(value.length) && value.Length == 3),
                                          checkValue =
                                          (Func<dynamic, dynamic, dynamic, bool>)
                                          ((typeInfo, index, value) => value != null && typeInfo.srcValues[index * 3 + 0] == value[0] && typeInfo.srcValues[index * 3 + 1] == value[1] && typeInfo.srcValues[index * 3 + 2] == value[2]),
                                          srcValues = new Float32Array(new float[] {16, 15, 14, 13, 12, 11, 10, 9, 8}),
                                          srcValuesLess = new Float32Array(new float[] {16, 15}),
                                          srcValuesNonMultiple = new Float32Array(new float[] {16, 15, 14, 13, 12, 11, 10, 9, 8, 7}),
                                      },
                                      new
                                      {
                                          type = "vec4",
                                          jsTypeOf = "Float32Array",
                                          setter = "uniform4fv",
                                          elem = "[3]",
                                          numSrcValues = 3,
                                          illegalSet = (Action<WebGLUniformLocation>)(loc => gl.uniform1fv(loc, 2)),
                                          invalidSet = (Action<WebGLUniformLocation>)(loc => gl.uniform1fv(loc, new Float32Array(new float[] {2}))),
                                          srcValueAsString =
                                          (Func<dynamic, dynamic, string>)
                                          ((index, srcValues) => "[" + srcValues[index * 4 + 0].toString() + ", " + srcValues[index * 4 + 1].toString() + ", " + srcValues[index * 4 + 2].toString() + ", " + srcValues[index * 4 + 3].toString() + "]"),
                                          returnValueAsString = (Func<dynamic, string>)(value => value == null ? "null" : "[" + value[0] + ", " + value[1] + ", " + value[2] + ", " + value[3] + "]"),
                                          checkType = (Func<dynamic, bool>)(value => value && wtu.isNumber(value.length) && value.length == 4),
                                          checkValue =
                                          (Func<dynamic, dynamic, dynamic, bool>)
                                          ((typeInfo, index, value) => value != null && typeInfo.srcValues[index * 4 + 0] == value[0] && typeInfo.srcValues[index * 4 + 1] == value[1] && typeInfo.srcValues[index * 4 + 2] == value[2] &&
                                                                       typeInfo.srcValues[index * 4 + 3] == value[3]),
                                          srcValues = new Float32Array(new float[] {16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5}),
                                          srcValuesLess = new Float32Array(new float[] {16, 15, 14}),
                                          srcValuesNonMultiple = new Float32Array(new float[] {16, 15, 14, 13, 12, 11, 10, 9, 8, 7, 6, 5, 4}),
                                      }
                                  };

            for (var tt = 0; tt < typeInfos.Length; ++tt)
            {
                var typeInfo = typeInfos[tt];
                wtu.debug("");
                wtu.debug("check " + typeInfo.type);
                var fSrc = fTemplate.Replace("$type", typeInfo.type).Replace("$elem", typeInfo.elem);
                var program = loadProgram(gl, vSrc, fSrc);

                var numUniforms = gl.getProgramParameter(program, gl.ACTIVE_UNIFORMS);
                wtu.assertMsg(() => numUniforms == 1, "1 uniform found");
                var info = gl.getActiveUniform(program, 0);
                wtu.assertMsg(() => info.name() == "color[0]", "uniform name is 'color[0]' not 'color' as per OpenGL ES 2.0.24 section 2.10");
                var loc = gl.getUniformLocation(program, "color[0]");
                var srcValues = typeInfo.srcValues;
                var srcValuesLess = typeInfo.srcValuesLess;
                var srcValuesNonMultiple = typeInfo.srcValuesNonMultiple;

                // Try setting the value before using the program
                wtu.getMethod(gl, typeInfo.setter).Invoke(gl, new[] {loc, srcValues});
                wtu.glErrorShouldBe(gl, gl.INVALID_OPERATION, "should fail if there is no current program");

                gl.useProgram(program);
                wtu.getMethod(gl, typeInfo.setter).Invoke(gl, new[] {loc, srcValuesLess});
                wtu.glErrorShouldBe(gl, gl.INVALID_VALUE, "should fail with insufficient array size with gl." + typeInfo.setter);
                if (srcValuesNonMultiple != null)
                {
                    wtu.getMethod(gl, typeInfo.setter).Invoke(gl, new[] {loc, srcValuesNonMultiple});
                    wtu.glErrorShouldBe(gl, gl.INVALID_VALUE, "should fail with non-multiple array size with gl." + typeInfo.setter);
                }
                wtu.getMethod(gl, typeInfo.setter).Invoke(gl, new[] {loc, srcValues});
                wtu.glErrorShouldBe(gl, gl.NO_ERROR, "can set an array of uniforms with gl." + typeInfo.setter);
                var values = gl.getUniform(program, loc);
                wtu.glErrorShouldBe(gl, gl.NO_ERROR, "can call gl.getUniform");
                wtu.assertMsg(() => typeInfo.checkType(values), "gl.getUniform returns the correct type.");
                for (var ii = 0; ii < typeInfo.numSrcValues; ++ii)
                {
                    var elemLoc = gl.getUniformLocation(program, "color[" + ii + "]");
                    wtu.glErrorShouldBe(gl, gl.NO_ERROR, "can get location of element " + ii + " of array from gl.getUniformLocation");
                    var value = gl.getUniform(program, elemLoc);
                    wtu.glErrorShouldBe(gl, gl.NO_ERROR, "can get value of element " + ii + " of array from gl.getUniform");
                    wtu.assertMsg(() => typeInfo.checkValue(typeInfo, ii, value), (string)("value put in (" + typeInfo.srcValueAsString(ii, srcValues) + ") matches value pulled out (" + typeInfo.returnValueAsString(value) + ")"));
                }
                typeInfo.invalidSet(loc);
                wtu.glErrorShouldBe(gl, gl.INVALID_OPERATION, "using the wrong size of gl.Uniform fails");
                var exceptionCaught = false;
                if (typeInfo.illegalSet != null)
                {
                    try
                    {
                        typeInfo.illegalSet(loc);
                    }
                    catch (Exception)
                    {
                        exceptionCaught = true;
                    }
                    wtu.assertMsg(() => exceptionCaught, "passing non-array to glUniform*fv should throw TypeError");
                }
                gl.useProgram(null);
                wtu.glErrorShouldBe(gl, gl.NO_ERROR, "can call gl.useProgram(null)");
            }
            wtu.debug("");
        }
    }
}