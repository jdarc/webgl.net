using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture(Description = "Tests getActiveAttrib for various types")]
    public class GLGetActiveAttribute : BaseTest
    {
        private static readonly Script vshader = new Script("vshader", "x-shader/x-vertex")
                                                 {
                                                     text = @"
attribute $type attr0;
void main()
{
    gl_Position = vec4(0, 0, 0, attr0$access);
}"
                                                 };

        private static readonly Script fshader = new Script("fshader", "x-shader/x-fragment")
                                                 {
                                                     text = @"
void main()
{
    gl_FragColor = vec4(0,1,0,1);
}"
                                                 };

        [Test(Description = "")]
        public void ShouldDoMagic()
        {
            var gl = WebGLTestUtils.create3DContext(Canvas);

            var tests = new[]
                        {
                            new {glType = gl.FLOAT, size = 1, type = "float", access = ""},
                            new {glType = gl.FLOAT_VEC2, size = 1, type = "vec2", access = "[1]"},
                            new {glType = gl.FLOAT_VEC3, size = 1, type = "vec3", access = "[2]"},
                            new {glType = gl.FLOAT_VEC4, size = 1, type = "vec4", access = "[3]"},
                            new {glType = gl.FLOAT_MAT2, size = 1, type = "mat2", access = "[1][1]"},
                            new {glType = gl.FLOAT_MAT3, size = 1, type = "mat3", access = "[2][2]"},
                            new {glType = gl.FLOAT_MAT4, size = 1, type = "mat4", access = "[3][3]"},
                        };

            var source = vshader.text;
            var fs = WebGLTestUtils.loadShaderFromScript(gl, fshader, gl.FRAGMENT_SHADER);
            for (var tt = 0; tt < tests.Length; ++tt)
            {
                var t = tests[tt];
                var vs = WebGLTestUtils.loadShader(
                    gl,
                    source.Replace("$type", t.type).Replace("$access", t.access),
                    gl.VERTEX_SHADER);
                var program = WebGLTestUtils.setupProgram(gl, new[] {vs, fs});
                WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR, "no errors from setup");
                var numAttribs = gl.getProgramParameter(program, gl.ACTIVE_ATTRIBUTES);
                var found = false;
                for (var ii = 0; ii < numAttribs; ++ii)
                {
                    var info = gl.getActiveAttrib(program, (uint)ii);
                    if (info.name() == "attr0")
                    {
                        found = true;
                        WebGLTestUtils.assertMsg(() => info.type() == t.glType, "type must be " + WebGLTestUtils.glEnumToString(gl, t.glType) + " was " + WebGLTestUtils.glEnumToString(gl, info.type()));
                        WebGLTestUtils.assertMsg(() => info.size() == t.size, "size must be " + t.size + " was " + info.size());
                    }
                }
                if (!found)
                {
                    WebGLTestUtils.testFailed("attrib 'attr0' not found");
                }
            }
        }
    }
}