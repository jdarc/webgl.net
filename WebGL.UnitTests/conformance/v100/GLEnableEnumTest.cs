using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLEnableEnumTest : BaseTest
    {
        [Test(Description = "This test ensures WebGL implementations allow OpenGL ES 2.0 features to be turned on but not non OpenGL ES 2.0 features.")]
        public void ShouldDoMagic()
        {
            var gl = WebGLTestUtils.create3DContext(Canvas);
            if (gl == null)
            {
                WebGLTestUtils.testFailed("context does not exist");
            }
            else
            {
                WebGLTestUtils.testPassed("context exists");

                var invalidEnums = new[]
                                   {
                                       "ALPHA_TEST",
                                       "CLIP_PLANE0",
                                       "CLIP_PLANE1",
                                       "COLOR_LOGIC_OP",
                                       "COLOR_MATERIAL",
                                       "COLOR_SUM",
                                       "COLOR_TABLE",
                                       "FOG",
                                       "HISTOGRAM",
                                       "INDEX_LOGIC_OP",
                                       "LIGHT0",
                                       "LIGHT1",
                                       "LIGHTING",
                                       "LINE_SMOOTH",
                                       "LINE_STIPPLE",
                                       "MAP1_COLOR_4",
                                       "MAP1_INDEX",
                                       "MAP1_NORMAL",
                                       "MAP1_TEXTURE_COORD_1",
                                       "MAP1_TEXTURE_COORD_2",
                                       "MAP1_TEXTURE_COORD_3",
                                       "MAP1_TEXTURE_COORD_4",
                                       "MAP1_VERTEX_3",
                                       "MAP1_VERTEX_4",
                                       "MAP2_COLOR_4",
                                       "MAP2_INDEX",
                                       "MAP2_NORMAL",
                                       "MAP2_TEXTURE_COORD_1",
                                       "MAP2_TEXTURE_COORD_2",
                                       "MAP2_TEXTURE_COORD_3",
                                       "MAP2_TEXTURE_COORD_4",
                                       "MAP2_VERTEX_3",
                                       "MAP2_VERTEX_4",
                                       "MINMAX",
                                       "MULTISAMPLE",
                                       "NORMALIZE",
                                       "POINT_SMOOTH",
                                       "POINT_SPRITE",
                                       "POLYGON_OFFSET_LINE",
                                       "POLYGON_OFFSET_POINT",
                                       "POLYGON_SMOOTH",
                                       "POLYGON_STIPPLE",
                                       "POST_COLOR_MATRIX_COLOR_TABLE",
                                       "POST_CONVOLUTION_COLOR_TABLE",
                                       "RESCALE_NORMAL",
                                       "SAMPLE_ALPHA_TO_ONE",
                                       "TEXTURE_1D",
                                       "TEXTURE_2D",
                                       "TEXTURE_3D",
                                       "TEXTURE_CUBE_MAP",
                                       "TEXTURE_GEN_Q",
                                       "TEXTURE_GEN_R",
                                       "TEXTURE_GEN_S",
                                       "TEXTURE_GEN_T",
                                       "VERTEX_PROGRAM_POINT_SIZE",
                                       "VERTEX_PROGRAM_TWO_SIDE"
                                   };

                for (var ii = 0; ii < invalidEnums.Length; ++ii)
                {
                    var name = invalidEnums[ii];
                    JSConsole.log(name);
                    gl.enable(desktopGL.ContainsKey(name) ? desktopGL[name] : 0);
                    WebGLTestUtils.glErrorShouldBe(gl, gl.INVALID_ENUM, "gl.enable must set INVALID_ENUM when passed GL_" + name);
                }

                var validEnums = new[]
                                 {
                                     gl.BLEND,
                                     gl.CULL_FACE,
                                     gl.DEPTH_TEST,
                                     gl.DITHER,
                                     gl.POLYGON_OFFSET_FILL,
                                     gl.SAMPLE_ALPHA_TO_COVERAGE,
                                     gl.SAMPLE_COVERAGE,
                                     gl.SCISSOR_TEST,
                                     gl.STENCIL_TEST
                                 };

                for (var ii = 0; ii < validEnums.Length; ++ii)
                {
                    gl.enable(validEnums[ii]);
                    WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR, "gl.enable must succeed when passed gl." + validEnums[ii]);
                }
            }
        }
    }
}