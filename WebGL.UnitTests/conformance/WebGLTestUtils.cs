using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using NUnit.Framework;

namespace WebGL.UnitTests
{
    // ReSharper disable InconsistentNaming

    public static class WebGLTestUtils
    {
        private static FieldInfo[] fieldInfos;
        private static MethodInfo[] methodInfos;

        public static void glErrorShouldBe(WebGLRenderingContext gl, dynamic glError, string opt_msg = null)
        {
            opt_msg = opt_msg ?? string.Empty;
            var err = gl.getError();
            if (err != glError)
            {
                testFailed("getError expected: " + getGLErrorAsString(gl, glError) + ". Was " + getGLErrorAsString(gl, err) + " : " + opt_msg);
            }
            else
            {
                testPassed("getError was expected value: " + getGLErrorAsString(gl, glError) + " : " + opt_msg);
            }
        }

        public static void testPassed(string msg)
        {
            reportTestResultsToHarness(true, msg);
            debug(escapeHTML(msg));
        }

        public static void testFailed(string msg)
        {
            reportTestResultsToHarness(false, msg);
            debug(escapeHTML(msg));
        }

        public static void debug(string msg)
        {
            JSConsole.debug(msg);
        }

        private static string escapeHTML(string text)
        {
            return text;
        }

        public static void reportTestResultsToHarness(bool success, string msg)
        {
            if (!success)
            {
                Assert.Fail(msg);
            }
        }

        public static uint? getConstant(WebGLRenderingContext ctx, string name)
        {
            fieldInfos = fieldInfos ?? ctx.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var fieldInfo in fieldInfos)
            {
                if (fieldInfo.Name.Equals(name))
                {
                    return (uint?)fieldInfo.GetValue(ctx);
                }
            }
            return null;
        }

        public static MethodInfo getMethod(WebGLRenderingContext ctx, string name)
        {
            methodInfos = methodInfos ?? ctx.GetType().GetMethods(BindingFlags.Public | BindingFlags.Instance);
            foreach (var methodInfo in methodInfos)
            {
                if (methodInfo.Name.Equals(name))
                {
                    return methodInfo;
                }
            }
            return null;
        }

        public static string getGLErrorAsString(WebGLRenderingContext ctx, uint err)
        {
            if (err == ctx.NO_ERROR)
            {
                return "NO_ERROR";
            }
            fieldInfos = fieldInfos ?? ctx.GetType().GetFields(BindingFlags.Public | BindingFlags.Instance);
            foreach (var info in fieldInfos)
            {
                if (info.GetValue(ctx).Equals(err))
                {
                    return info.Name;
                }
            }
            return err.ToString("{0:X}");
        }

        public static void shouldGenerateGLError(WebGLRenderingContext ctx, uint glError, Action evalStr)
        {
            Exception exception = null;
            try
            {
                evalStr();
            }
            catch (Exception e)
            {
                exception = e;
            }

            if (exception != null)
            {
                testFailed(evalStr + " threw exception " + exception);
            }
            else
            {
                var err = ctx.getError();
                if (err != glError)
                {
                    testFailed(evalStr.Method + " expected: " + getGLErrorAsString(ctx, glError) + ". Was " + getGLErrorAsString(ctx, err) + ".");
                }
                else
                {
                    testPassed(evalStr.Method + " generated expected GL error: " + getGLErrorAsString(ctx, glError) + ".");
                }
            }
        }

        public static void shouldBe(Func<dynamic> _a, dynamic _b)
        {
            //            if (!(_a is string) || !(_b is string))
            //            {
            //                debug("WARN: shouldBe() expects string arguments");
            //            }

            Exception exception = null;
            dynamic _av = null;
            try
            {
                _av = _a();
            }
            catch (Exception e)
            {
                exception = e;
            }
            var _bv = _b is Delegate ? _b() : _b;

            if (exception != null)
            {
                testFailed(_a + " should be " + _bv + ". Threw exception " + exception);
            }
            else if (isResultCorrect(_av, _bv))
            {
                testPassed(_a + " is " + _b);
            }
            else if (_av.GetType() != _bv.GetType())
            {
                testFailed(_a + " should be " + _bv + ". Was " + _av + ".");
            }
            else
            {
                testFailed(_a + " should be " + _bv + " (of type " + _bv.GetType() + "). Was " + _av + " (of type " + _av.GetType() + ").");
            }
        }

        public static void shouldThrow(Func<object> _a, Func<object> _e = null)
        {
            Exception exception = null;
            dynamic _av = null;
            try
            {
                _av = _a();
            }
            catch (Exception e)
            {
                exception = e;
            }

            dynamic _ev = null;
            if (_e != null)
            {
                _ev = _e();
            }

            if (exception != null)
            {
                if (_e == null || exception == _ev)
                {
                    testPassed(_a + " threw exception " + exception + ".");
                }
                else
                {
                    testFailed(_a + " should throw " + _ev + ". Threw exception " + exception + ".");
                }
            }
            else if (_av == null || _av.GetType() == "undefined")
            {
                testFailed(_a + " should throw " + (_e == null ? "an exception" : _ev) + ". Was undefined.");
            }
            else
            {
                testFailed(_a + " should throw " + (_e == null ? "an exception" : _ev) + ". Was " + _av + ".");
            }
        }

        public static void shouldBeUndefined(Func<object> _a, Func<object> _e = null)
        {
            Exception exception = null;
            dynamic _av = null;
            try
            {
                _av = _a.Invoke();
            }
            catch (Exception e)
            {
                exception = e;
            }

            if (exception != null)
            {
                testFailed(_a + " should be undefined. Threw exception " + exception);
            }
            else if (_av == null)
            {
                testPassed(_a + " is undefined.");
            }
            else
            {
                testFailed(_a + " should be undefined. Was " + _av);
            }
        }

        public static void shouldBeNull(Func<object> _a, dynamic _c = null)
        {
            shouldBe(_a, null);
        }

        public static void shouldBeEmptyString(Func<object> command, Func<object> ignore = null)
        {
            shouldBe(command, String.Empty);
        }

        public static WebGLRenderingContext create3DContext(HTMLCanvasElement canvas, WebGLContextAttributes attributes = null)
        {
            //            if (canvas == null)
            //            {
            //                canvas = document.createElement("canvas");
            //            }
            WebGLRenderingContext context = null;
            try
            {
                context = (WebGLRenderingContext)canvas.getContext("webgl", attributes);
            }
            catch (Exception)
            {
            }
            if (context == null)
            {
                throw new Exception("Unable to fetch WebGL rendering context for Canvas");
            }
            return context;
        }

        public static WebGLProgram loadStandardProgram(WebGLRenderingContext context)
        {
            var program = context.createProgram();
            context.attachShader(program, loadStandardVertexShader(context));
            context.attachShader(program, loadStandardFragmentShader(context));
            context.linkProgram(program);
            return program;
        }

        public static WebGLShader loadStandardVertexShader(WebGLRenderingContext context)
        {
            return loadShader(context, "resources/vertexShader.vert", context.VERTEX_SHADER, true);
        }

        public static WebGLShader loadStandardFragmentShader(WebGLRenderingContext context)
        {
            return loadShader(context, "resources/fragmentShader.frag", context.FRAGMENT_SHADER, true);
        }

        public static bool isResultCorrect(dynamic _actual, dynamic _expected)
        {
            if (_actual == null || _expected == null)
            {
                return _actual == _expected;
            }
            if (isNumber(_expected) && _expected == 0)
            {
                return _actual == _expected && (1.0 / _actual) == (1.0 / _expected);
            }
            if (_actual.GetType() == _expected.GetType() && !(_actual is JSArray) && _actual == _expected)
            {
                return true;
            }
            if (isNumber(_expected) && double.IsNaN((double)_expected))
            {
                return isNumber(_actual) && double.IsNaN((double)_actual);
            }
            if (_expected is JSArray || _expected is TypedArray)
            {
                return areArraysEqual(_actual, _expected);
            }
            if (_expected is Array)
            {
                return areSystemArraysEqual(_actual, _expected);
            }
            return false;
        }

        public static bool areArraysEqual(dynamic _a, dynamic _b)
        {
            try
            {
                if (_a.length != _b.length)
                {
                    return false;
                }
                for (var i = 0; i < _a.length; i++)
                {
                    if (_a[i] != _b[i])
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static bool areSystemArraysEqual(dynamic _a, dynamic _b)
        {
            try
            {
                if (_a.Length != _b.Length)
                {
                    return false;
                }
                for (var i = 0; i < _a.Length; i++)
                {
                    if (_a[i] != _b[i])
                    {
                        return false;
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }

        public static string stringify(double v)
        {
            if (v == 0 && 1.0 / v < 0)
            {
                return "-0";
            }
            return "" + v;
        }

        public static WebGLShader loadShader(WebGLRenderingContext ctx, dynamic shaderId, uint shaderType, bool isFile = false)
        {
            var shaderSource = "";

            if (isFile)
            {
                shaderSource = getShaderSource(shaderId);
            }
            else
            {
                var shaderScript = shaderId as Script;
                if (shaderScript == null)
                {
                    shaderSource = shaderId;
                }
                else
                {
                    if (shaderScript.type == "x-shader/x-vertex")
                    {
                        shaderType = ctx.VERTEX_SHADER;
                    }
                    else if (shaderScript.type == "x-shader/x-fragment")
                    {
                        shaderType = ctx.FRAGMENT_SHADER;
                    }
                    else if (shaderType != ctx.VERTEX_SHADER && shaderType != ctx.FRAGMENT_SHADER)
                    {
                        webglTestLog("*** Error: unknown shader type");
                        return null;
                    }

                    shaderSource = shaderScript.text;
                }
            }

            // Create the shader object
            var shader = ctx.createShader(shaderType);
            if (shader == null)
            {
                webglTestLog("*** Error: unable to create shader '" + shaderId + "'");
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
                webglTestLog("*** Error compiling shader '" + shader + "':" + error);
                ctx.deleteShader(shader);
                return null;
            }

            return shader;
        }

        private static string getShaderSource(string file)
        {
            return File.ReadAllText(file);
        }

        public static void webglTestLog(string msg)
        {
            JSConsole.log(msg);
        }

        public static bool isNumber(dynamic c)
        {
            return c is byte || c is sbyte ||
                   c is short || c is ushort ||
                   c is int || c is uint ||
                   c is long || c is ulong ||
                   c is float || c is double;
        }

        public static dynamic initWebGL(HTMLCanvasElement canvas, dynamic vshader, dynamic fshader, string[] attribs, float[] clearColor, float clearDepth, WebGLContextAttributes contextAttribs = null)
        {
            // var canvas = document.getElementById(canvasName);
            var gl = create3DContext(canvas, contextAttribs);
            if (gl == null)
            {
                throw new Exception("No WebGL context found");
            }

            // Create the program object
            var program = createProgram(gl, vshader, fshader, attribs);
            if (program == null)
            {
                return null;
            }

            gl.useProgram(program);

            gl.clearColor(clearColor[0], clearColor[1], clearColor[2], clearColor[3]);
            gl.clearDepth(clearDepth);

            gl.enable(gl.DEPTH_TEST);
            gl.enable(gl.BLEND);
            gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA);

            return new {context = gl, program};
        }

        public static WebGLProgram createProgram(WebGLRenderingContext gl, dynamic vshaders, dynamic fshaders, string[] attribs)
        {
            if (!(vshaders is JSArray))
            {
                vshaders = new Script[] {vshaders};
            }
            if (!(fshaders is JSArray))
            {
                fshaders = new Script[] {fshaders};
            }

            var shaders = new List<WebGLShader>();
            int i;

            for (i = 0; i < vshaders.Length; ++i)
            {
                var shader = loadShader(gl, vshaders[i], gl.VERTEX_SHADER);
                if (shader == null)
                {
                    return null;
                }
                shaders.Add(shader);
            }

            for (i = 0; i < fshaders.Length; ++i)
            {
                var shader = loadShader(gl, fshaders[i], gl.FRAGMENT_SHADER);
                if (shader == null)
                {
                    return null;
                }
                shaders.Add(shader);
            }

            var prog = gl.createProgram();
            for (i = 0; i < shaders.Count; ++i)
            {
                gl.attachShader(prog, shaders[i]);
            }

            if (attribs != null)
            {
                for (var index = 0; index < attribs.Length; index++)
                {
                    var s = attribs[index];
                    gl.bindAttribLocation(prog, (uint)index, attribs[index]);
                }
            }

            gl.linkProgram(prog);

            // Check the link status
            var linked = gl.getProgramParameter(prog, gl.LINK_STATUS);
            if (!linked)
            {
                // something went wrong with the link
                var error = gl.getProgramInfoLog(prog);
                webglTestLog("Error in program linking:" + error);

                gl.deleteProgram(prog);
                for (i = 0; i < shaders.Count; ++i)
                {
                    gl.deleteShader(shaders[i]);
                }
                return null;
            }

            return prog;
        }

        public static WebGLProgram setupTexturedQuad(WebGLRenderingContext gl, int opt_positionLocation = 0, int opt_texcoordLocation = 1)
        {
            var program = setupSimpleTextureProgram(gl, opt_positionLocation, opt_texcoordLocation);
            setupUnitQuad(gl, opt_positionLocation, opt_texcoordLocation);
            return program;
        }

        public static List<WebGLBuffer> setupUnitQuad(WebGLRenderingContext gl, int opt_positionLocation = 0, int opt_texcoordLocation = 1)
        {
            var objects = new List<WebGLBuffer>();

            var vertexObject = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, vertexObject);
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(new[]
                                                            {
                                                                1.0f, 1.0f, 0.0f,
                                                                -1.0f, 1.0f, 0.0f,
                                                                -1.0f, -1.0f, 0.0f,
                                                                1.0f, 1.0f, 0.0f,
                                                                -1.0f, -1.0f, 0.0f,
                                                                1.0f, -1.0f, 0.0f
                                                            }), gl.STATIC_DRAW);
            gl.enableVertexAttribArray((uint)opt_positionLocation);
            gl.vertexAttribPointer((uint)opt_positionLocation, 3, gl.FLOAT, false, 0, 0);
            objects.Add(vertexObject);

            vertexObject = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, vertexObject);
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(new[]
                                                            {
                                                                1.0f, 1.0f,
                                                                0.0f, 1.0f,
                                                                0.0f, 0.0f,
                                                                1.0f, 1.0f,
                                                                0.0f, 0.0f,
                                                                1.0f, 0.0f
                                                            }), gl.STATIC_DRAW);
            gl.enableVertexAttribArray((uint)opt_texcoordLocation);
            gl.vertexAttribPointer((uint)opt_texcoordLocation, 2, gl.FLOAT, false, 0, 0);
            objects.Add(vertexObject);
            return objects;
        }

        public static WebGLProgram setupSimpleTextureProgram(WebGLRenderingContext gl, int opt_positionLocation = 0, int opt_texcoordLocation = 1)
        {
            var vs = setupSimpleTextureVertexShader(gl);
            var fs = setupSimpleTextureFragmentShader(gl);
            if (vs == null || fs == null)
            {
                return null;
            }
            var program = setupProgram(gl, new[] {vs, fs}, new[] {"vPosition", "texCoord0"}, new[] {opt_positionLocation, opt_texcoordLocation});

            if (program == null)
            {
                gl.deleteShader(fs);
                gl.deleteShader(vs);
            }
            gl.useProgram(program);
            return program;
        }

        public static WebGLProgram setupProgram(WebGLRenderingContext gl, WebGLShader[] shaders, string[] opt_attribs = null, int[] opt_locations = null)
        {
            var program = gl.createProgram();
            for (var ii = 0; ii < shaders.Length; ++ii)
            {
                gl.attachShader(program, shaders[ii]);
            }
            if (opt_attribs != null)
            {
                for (var ii = 0; ii < opt_attribs.Length; ++ii)
                {
                    gl.bindAttribLocation(program, (uint)(opt_locations != null ? opt_locations[ii] : ii), opt_attribs[ii]);
                }
            }
            gl.linkProgram(program);

            // Check the link status
            var linked = gl.getProgramParameter(program, gl.LINK_STATUS);
            if (!linked)
            {
                // something went wrong with the link
                var lastError = gl.getProgramInfoLog(program);
                error("Error in program linking:" + lastError);

                gl.deleteProgram(program);
                return null;
            }

            gl.useProgram(program);
            return program;
        }

        public static void error(string msg)
        {
            JSConsole.error(msg);
        }

        public static WebGLShader setupSimpleTextureFragmentShader(WebGLRenderingContext gl)
        {
            return loadShader(gl, simpleTextureFragmentShader, gl.FRAGMENT_SHADER);
        }

        public static WebGLShader setupSimpleTextureVertexShader(WebGLRenderingContext gl)
        {
            return loadShader(gl, simpleTextureVertexShader, gl.VERTEX_SHADER);
        }

        private const string simpleTextureVertexShader =
            "attribute vec4 vPosition;\n" +
            "attribute vec2 texCoord0;\n" +
            "varying vec2 texCoord;\n" +
            "void main() {\n" +
            "    gl_Position = vPosition;\n" +
            "    texCoord = texCoord0;\n" +
            "}\n";

        private const string simpleTextureFragmentShader =
            "#ifdef GL_ES\n" +
            "precision mediump float;\n" +
            "#endif\n" +
            "uniform sampler2D tex;\n" +
            "varying vec2 texCoord;\n" +
            "void main() {\n" +
            "    gl_FragData[0] = texture2D(tex, texCoord);\n" +
            "}\n";

        public static WebGLProgram loadProgram(WebGLRenderingContext context, string vertexShaderPath, string fragmentShaderPath, bool isFile = true)
        {
            var program = context.createProgram();
            context.attachShader(program, loadShader(context, vertexShaderPath, context.VERTEX_SHADER, isFile));
            context.attachShader(program, loadShader(context, fragmentShaderPath, context.FRAGMENT_SHADER, isFile));
            context.linkProgram(program);
            return program;
        }

        public static WebGLShader loadShaderFromScript(WebGLRenderingContext gl, Script script, uint opt_shaderType = 0u)
        {
            var shaderSource = "";
            var shaderType = 0u;
            var shaderScript = script;
            if (shaderScript == null)
            {
                throw new Exception("*** Error: unknown script element" + script.id);
            }
            shaderSource = shaderScript.text;

            if (opt_shaderType == 0u)
            {
                if (shaderScript.type == "x-shader/x-vertex")
                {
                    shaderType = gl.VERTEX_SHADER;
                }
                else if (shaderScript.type == "x-shader/x-fragment")
                {
                    shaderType = gl.FRAGMENT_SHADER;
                }

                if (shaderType != gl.VERTEX_SHADER && shaderType != gl.FRAGMENT_SHADER)
                {
                    throw new Exception("*** Error: unknown shader type");
                }
            }

            return loadShader(gl, shaderSource, opt_shaderType != 0u ? opt_shaderType : shaderType);
        }

        public static void drawQuad(WebGLRenderingContext gl, byte[] opt_color = null)
        {
            opt_color = opt_color ?? new byte[] {255, 255, 255, 255};
            gl.clearColor(
                opt_color[0] / 255f,
                opt_color[1] / 255f,
                opt_color[2] / 255f,
                opt_color[3] / 255f);
            gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);
            gl.drawArrays(gl.TRIANGLES, 0, 6);
        }

        public static void checkCanvasRect(WebGLRenderingContext gl, int x, int y, int width, int height, int[] color, string msg, int errorRange = 0)
        {
            var buf = new Uint8Array(width * height * 4);
            gl.readPixels(x, y, width, height, gl.RGBA, gl.UNSIGNED_BYTE, buf);
            for (var i = 0; i < width * height; ++i)
            {
                var offset = i * 4;
                for (var j = 0; j < color.Length; ++j)
                {
                    if (Math.Abs(buf[offset + j] - color[j]) > errorRange)
                    {
                        testFailed(msg);
                        var was = buf[offset + 0].ToString();
                        for (j = 1; j < color.Length; ++j)
                        {
                            was += "," + buf[offset + j];
                        }
                        debug("expected: " + color + " was " + was);
                        return;
                    }
                }
            }
            testPassed(msg);
        }

        public static void shouldBeTrue(Func<object> _a)
        {
            shouldBe(_a, true);
        }

        public static void shouldBeFalse(Func<object> _a)
        {
            shouldBe(_a, false);
        }

        public static void shouldBeNonNull(Func<object> _a, Func<object> _b = null)
        {
            Exception exception = null;
            dynamic _av = null;
            try
            {
                _av = _a();
            }
            catch (Exception e)
            {
                exception = e;
            }

            if (exception != null)
            {
                testFailed(_a + " should be non-null. Threw exception " + exception);
            }
            else if (_av != null)
            {
                testPassed(_a + " is non-null.");
            }
            else
            {
                testFailed(_a + " should be non-null. Was " + _av);
            }
        }

        public static void assertMsg(Func<bool> assertion, string msg)
        {
            if (assertion())
            {
                testPassed(msg);
            }
            else
            {
                testFailed(msg);
            }
        }

        public static string glEnumToString(WebGLRenderingContext gl, uint value)
        {
            return value.ToString();
        }

        public static void checkCanvas(WebGLRenderingContext gl, int[] color, string msg)
        {
            checkCanvasRect(gl, 0, 0, gl.canvas.width, gl.canvas.height, color, msg);
        }

        public static WebGLTexture loadTexture(WebGLRenderingContext gl, string url, Action<Image> callback)
        {
            var texture = gl.createTexture();
            gl.bindTexture(gl.TEXTURE_2D, texture);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, (int)gl.NEAREST);
            gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, (int)gl.NEAREST);
            var image = new Image();
            image.addEventListener("onload", evt =>
                                             {
                                                 gl.bindTexture(gl.TEXTURE_2D, texture);
                                                 gl.pixelStorei(gl.UNPACK_FLIP_Y_WEBGL, 1);
                                                 gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, image.imageData);
                                                 callback(image);
                                             });
            image.src = url;
            return texture;
        }

        public static void finishTest()
        {
        }

        public static void fillTexture(WebGLRenderingContext gl, WebGLTexture tex, int width, int height, byte[] color, int opt_level = 0)
        {
            var numPixels = width * height;
            var size = numPixels * 4;
            var buf = new Uint8Array(size);
            for (var ii = 0; ii < numPixels; ++ii)
            {
                var off = ii * 4;
                buf[off + 0] = color[0];
                buf[off + 1] = color[1];
                buf[off + 2] = color[2];
                buf[off + 3] = color[3];
            }
            gl.bindTexture(gl.TEXTURE_2D, tex);
            gl.texImage2D(
                gl.TEXTURE_2D, opt_level, gl.RGBA, width, height, 0,
                gl.RGBA, gl.UNSIGNED_BYTE, buf);
        }

        public static void shouldBeNonZero(Func<dynamic> _a)
        {
            Exception exception = null;
            dynamic _av = null;
            try
            {
                _av = _a();
            }
            catch (Exception e)
            {
                exception = e;
            }

            if (exception != null)
            {
                testFailed(_a + " should be non-zero. Threw exception " + exception);
            }
            else if (_av != 0)
            {
                testPassed(_a + " is non-zero.");
            }
            else
            {
                testFailed(_a + " should be non-zero. Was " + _av);
            }
        }
    }

    // ReSharper restore InconsistentNaming
}