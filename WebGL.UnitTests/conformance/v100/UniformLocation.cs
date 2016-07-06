using System.Windows.Forms;
using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class UniformLocation : BaseTest
    {
        [Test(Description = "Tests the WebGLUniformLocation API")]
        public void ShouldDoMagic()
        {
            var contextA = WebGLTestUtils.create3DContext(new HTMLCanvasElement(new Control()));
            var contextB = WebGLTestUtils.create3DContext(new HTMLCanvasElement(new Control()));
            var programA1 = WebGLTestUtils.loadStandardProgram(contextA);
            var programA2 = WebGLTestUtils.loadStandardProgram(contextA);
            var programB = WebGLTestUtils.loadStandardProgram(contextB);
            var programS = WebGLTestUtils.loadProgram(contextA, "resources/structUniformShader.vert", "resources/fragmentShader.frag");
            var programV = WebGLTestUtils.loadProgram(contextA, "resources/floatUniformShader.vert", "resources/noopUniformShader.frag");
            var locationA = contextA.getUniformLocation(programA1, "u_modelViewProjMatrix");
            var locationB = contextB.getUniformLocation(programB, "u_modelViewProjMatrix");
            var locationSx = contextA.getUniformLocation(programS, "u_struct.x");
            var locationArray0 = contextA.getUniformLocation(programS, "u_array[0]");
            var locationArray1 = contextA.getUniformLocation(programS, "u_array[1]");
            var locationVec4 = contextA.getUniformLocation(programV, "fval4");

            var vec = new Float32Array(new float[] {1, 2, 3, 4});
            var mat = new Float32Array(new float[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16});

            WebGLTestUtils.shouldGenerateGLError(contextA, contextA.NO_ERROR, () => contextA.useProgram(programA2));
            WebGLTestUtils.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.uniformMatrix4fv(locationA, false, mat));
            WebGLTestUtils.shouldGenerateGLError(contextA, contextA.NO_ERROR, () => contextA.useProgram(programA1));
            WebGLTestUtils.shouldGenerateGLError(contextA, contextA.NO_ERROR, () => contextA.uniformMatrix4fv(locationA, false, mat));
            WebGLTestUtils.shouldGenerateGLError(contextA, contextA.NO_ERROR, () => contextA.uniformMatrix4fv(null, false, mat));

            WebGLTestUtils.shouldGenerateGLError(contextA, contextA.NO_ERROR, () => contextA.useProgram(programS));
            WebGLTestUtils.shouldGenerateGLError(contextA, contextA.NO_ERROR, () => contextA.uniform1i(locationSx, 3));
            WebGLTestUtils.shouldGenerateGLError(contextA, contextA.NO_ERROR, () => contextA.uniform1f(locationArray0, 4.0f));
            WebGLTestUtils.shouldGenerateGLError(contextA, contextA.NO_ERROR, () => contextA.uniform1f(locationArray1, 5.0f));

            WebGLTestUtils.shouldBe(() => contextA.getUniform(programS, locationSx), 3);
            WebGLTestUtils.shouldBe(() => contextA.getUniform(programS, locationArray0), 4.0f);
            WebGLTestUtils.shouldBe(() => contextA.getUniform(programS, locationArray1), 5.0f);

            WebGLTestUtils.shouldGenerateGLError(contextA, contextA.NO_ERROR, () => contextA.useProgram(programV));
            WebGLTestUtils.shouldGenerateGLError(contextA, contextA.NO_ERROR, () => contextA.uniform4fv(locationVec4, vec));
            WebGLTestUtils.shouldBe(() => contextA.getUniform(programV, locationVec4), vec);

            WebGLTestUtils.shouldBeNull(() => contextA.getUniformLocation(programV, "IDontExist"));
            WebGLTestUtils.shouldGenerateGLError(contextA, contextA.NO_ERROR, () => contextA.linkProgram(programA1));
            // After linking all boxes are bad.
            WebGLTestUtils.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.uniformMatrix4fv(locationA, false, mat));

            // after re-linking the same program, all uniform locations become invalid.
            WebGLTestUtils.shouldGenerateGLError(contextA, contextA.NO_ERROR, () => contextA.useProgram(programS));
            WebGLTestUtils.shouldGenerateGLError(contextA, contextA.NO_ERROR, () => contextA.linkProgram(programS));
            WebGLTestUtils.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.uniform1i(locationSx, 3));
            WebGLTestUtils.shouldGenerateGLError(contextA, contextA.INVALID_OPERATION, () => contextA.getUniform(programS, locationSx));

            // Retrieve the locations again, and they should be good.
            locationSx = contextA.getUniformLocation(programS, "u_struct.x");
            locationArray0 = contextA.getUniformLocation(programS, "u_array[0]");
            WebGLTestUtils.shouldGenerateGLError(contextA, contextA.NO_ERROR, () => contextA.uniform1i(locationSx, 3));
            WebGLTestUtils.shouldBe(() => contextA.getUniform(programS, locationSx), 3);
        }
    }
}