using System;
using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLGetActiveUniform : BaseTest
    {
        private static readonly Script _vshader = new Script("vshader", "x-shader/x-vertex")
                                                  {
                                                      text = @"
void main()
{
    gl_Position = vec4(0, 0, 0, 1);
}"
                                                  };

        private static readonly Script _fshader = new Script("fshader", "x-shader/x-fragment")
                                                  {
                                                      text = @"
precision mediump float;
uniform $type uniform0;
void main()
{
    gl_FragColor = vec4(0,$access,0,1);
}"
                                                  };

        private static readonly Script _fshaderA = new Script("fshaderA", "x-shader/x-fragment")
                                                   {
                                                       text = @"
precision mediump float;
uniform float uniform0;
void main()
{
    gl_FragColor = vec4(0,uniform0,0,1);
}"
                                                   };

        private static readonly Script _fshaderB = new Script("fshaderB", "x-shader/x-fragment")
                                                   {
                                                       text = @"
precision mediump float;
uniform float uniform0;
uniform float uniform1;
void main()
{
    gl_FragColor = vec4(0,uniform0,uniform1,1);
}"
                                                   };

        private static WebGLProgram createProgram(WebGLRenderingContext gl, WebGLShader vs, string source, string type, string access)
        {
            var fs = WebGLTestUtils.loadShader(
                gl,
                source.Replace("$type", type).Replace("$access", access),
                gl.FRAGMENT_SHADER);
            var program = WebGLTestUtils.setupProgram(gl, new[] {vs, fs});
            WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR, "no errors from setup");
            return program;
        }

        [Test(Description = "Tests getActiveUniform for various types")]
        public void ShouldDoMagic()
        {
            var gl = WebGLTestUtils.create3DContext(Canvas);

            var tests = new object[]
                        {
                            new {glType = gl.FLOAT, size = 1, type = "float", access = "uniform0"},
                            new {glType = gl.FLOAT_VEC2, size = 1, type = "vec2", access = "uniform0[1]"},
                            new {glType = gl.FLOAT_VEC3, size = 1, type = "vec3", access = "uniform0[2]"},
                            new {glType = gl.FLOAT_VEC4, size = 1, type = "vec4", access = "uniform0[3]"},
                            new {glType = gl.FLOAT_MAT2, size = 1, type = "mat2", access = "uniform0[1][1]"},
                            new {glType = gl.FLOAT_MAT3, size = 1, type = "mat3", access = "uniform0[2][2]"},
                            new {glType = gl.FLOAT_MAT3, size = 1, type = "mat3", access = "uniform0[2][2]"},
                            new {glType = gl.FLOAT_MAT4, size = 1, type = "mat4", access = "uniform0[3][3]"},
                            new {glType = gl.INT, size = 1, type = "int", access = "float(uniform0)"},
                            new {glType = gl.INT_VEC2, size = 1, type = "ivec2", access = "float(uniform0[1])"},
                            new {glType = gl.INT_VEC3, size = 1, type = "ivec3", access = "float(uniform0[2])"},
                            new {glType = gl.INT_VEC4, size = 1, type = "ivec4", access = "float(uniform0[3])"},
                            new {glType = gl.BOOL, size = 1, type = "bool", access = "float(uniform0)"},
                            new {glType = gl.BOOL_VEC2, size = 1, type = "bvec2", access = "float(uniform0[1])"},
                            new {glType = gl.BOOL_VEC3, size = 1, type = "bvec3", access = "float(uniform0[2])"},
                            new {glType = gl.BOOL_VEC4, size = 1, type = "bvec4", access = "float(uniform0[3])"},
                            new
                            {
                                glType = gl.SAMPLER_2D,
                                size = 1,
                                type = "sampler2D",
                                access = "texture2D(uniform0, vec2(0,0)).x"
                            },
                            new
                            {
                                glType = gl.SAMPLER_CUBE,
                                size = 1,
                                type = "samplerCube",
                                access = "textureCube(uniform0, vec3(0,1,0)).x"
                            }
                        };

            var vs = WebGLTestUtils.loadShaderFromScript(gl, _vshader, gl.VERTEX_SHADER);
            var source = _fshader.text;

            for (var tt = 0; tt < tests.Length; ++tt)
            {
                dynamic t = tests[tt];
                var program = createProgram(gl, vs, source, t.type, t.access);
                var numUniforms = gl.getProgramParameter(program, gl.ACTIVE_UNIFORMS);
                var found = false;
                for (var ii = 0; ii < numUniforms; ++ii)
                {
                    var info = gl.getActiveUniform(program, (uint)ii);
                    if (info.name() == "uniform0")
                    {
                        found = true;

                        WebGLTestUtils.assertMsg((Func<bool>)(() => info.type() == t.glType), "type must be " + WebGLTestUtils.glEnumToString(gl, t.glType) + " was " + WebGLTestUtils.glEnumToString(gl, info.type()));
                        WebGLTestUtils.assertMsg((Func<bool>)(() => info.size() == t.size), "size must be " + t.size + " was " + info.size());
                    }
                }
                if (!found)
                {
                    WebGLTestUtils.testFailed("uniform 'uniform0' not found");
                }
            }

            var p1 = WebGLTestUtils.setupProgram(gl, new[] {vs, WebGLTestUtils.loadShaderFromScript(gl, _fshaderA, gl.FRAGMENT_SHADER)});
            WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR, "no errors from program A");
            var p2 = WebGLTestUtils.setupProgram(gl, new[] {vs, WebGLTestUtils.loadShaderFromScript(gl, _fshaderB, gl.FRAGMENT_SHADER)});
            WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR, "no errors from program B");
            var l1 = gl.getUniformLocation(p1, "uniform0");
            WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR, "no errors getting location of uniform0 p1");
            var l2 = gl.getUniformLocation(p2, "uniform0");
            WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR, "no errors getting location of uniform0 p2");

            gl.useProgram(p2);
            gl.uniform1f(l2, 1);
            WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR, "no errors setting uniform 0");
            gl.uniform1f(l1, 2);
            WebGLTestUtils.glErrorShouldBe(gl, gl.INVALID_OPERATION, "setting a uniform using a location from another program");
        }
    }
}