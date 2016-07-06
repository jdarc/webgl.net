using System.Drawing;
using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLGetCalls : BaseTest
    {
        public override Size PreferredSize
        {
            get { return new Size(2, 2); }
        }

        [Test(Description = "This test ensures basic functionality of the underlying graphics library")]
        public void ShouldDoMagic()
        {
            var context = WebGLTestUtils.create3DContext(Canvas);
            if (context == null)
            {
                WebGLTestUtils.testFailed("context does not exist");
            }
            else
            {
                WebGLTestUtils.testPassed("context exists");

                WebGLTestUtils.shouldBe(() => context.getParameter(context.ACTIVE_TEXTURE), context.TEXTURE0);
                WebGLTestUtils.shouldBe(() => (context.getParameter(context.ALIASED_LINE_WIDTH_RANGE)[0] == 1) || (context.getParameter(context.ALIASED_LINE_WIDTH_RANGE)[1] == 1), true);
                WebGLTestUtils.shouldBe(() => (context.getParameter(context.ALIASED_POINT_SIZE_RANGE)[0] == 1) || (context.getParameter(context.ALIASED_POINT_SIZE_RANGE)[1] == 1), true);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.ARRAY_BUFFER_BINDING), null);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.BLEND), false);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.BLEND_COLOR), new Int32Array(new[] {0, 0, 0, 0}));
                WebGLTestUtils.shouldBe(() => context.getParameter(context.BLEND_DST_ALPHA), 0);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.BLEND_DST_RGB), 0);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.BLEND_EQUATION_ALPHA), context.FUNC_ADD);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.BLEND_EQUATION_RGB), context.FUNC_ADD);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.BLEND_SRC_ALPHA), 1u);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.BLEND_SRC_RGB), 1u);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.COLOR_CLEAR_VALUE), new Int32Array(new[] {0, 0, 0, 0}));
                WebGLTestUtils.shouldBe(() => context.getParameter(context.COLOR_WRITEMASK), new[] {true, true, true, true});
                WebGLTestUtils.shouldBe(() => context.getParameter(context.CULL_FACE), false);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.CULL_FACE_MODE), context.BACK);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.CURRENT_PROGRAM), null);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.DEPTH_CLEAR_VALUE), 1f);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.DEPTH_FUNC), context.LESS);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.DEPTH_RANGE), new Int32Array(new[] {0, 1}));
                WebGLTestUtils.shouldBe(() => context.getParameter(context.DEPTH_TEST), false);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.DEPTH_WRITEMASK), true);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.DITHER), true);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.ELEMENT_ARRAY_BUFFER_BINDING), null);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.FRONT_FACE), context.CCW);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.GENERATE_MIPMAP_HINT), context.DONT_CARE);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.LINE_WIDTH), 1f);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.POLYGON_OFFSET_FACTOR), 0);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.POLYGON_OFFSET_FILL), false);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.POLYGON_OFFSET_UNITS), 0);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.RENDERBUFFER_BINDING), null);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.SAMPLE_COVERAGE_INVERT), false);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.SAMPLE_COVERAGE_VALUE), 1f);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.SCISSOR_BOX)[0], 0);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.SCISSOR_BOX)[1], 0);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.SCISSOR_TEST), false);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.STENCIL_BACK_FAIL), context.KEEP);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.STENCIL_BACK_FUNC), context.ALWAYS);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.STENCIL_BACK_PASS_DEPTH_FAIL), context.KEEP);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.STENCIL_BACK_PASS_DEPTH_PASS), context.KEEP);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.STENCIL_BACK_REF), 0);

                WebGLTestUtils.shouldBe(() => context.getParameter(context.STENCIL_BACK_VALUE_MASK), 0xFFFFFFFF);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.STENCIL_BACK_WRITEMASK), 0xFFFFFFFF);

                // If EXT_packed_depth_stencil is supported, STENCIL_BITS > 0; otherwise, STENCIL_BITS == 0.
                WebGLTestUtils.shouldBe(() => context.getParameter(context.STENCIL_BITS) >= 0, true);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.STENCIL_CLEAR_VALUE), 0);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.STENCIL_FAIL), context.KEEP);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.STENCIL_FUNC), context.ALWAYS);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.STENCIL_PASS_DEPTH_FAIL), context.KEEP);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.STENCIL_PASS_DEPTH_PASS), context.KEEP);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.STENCIL_REF), 0);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.STENCIL_TEST), false);

                WebGLTestUtils.shouldBe(() => context.getParameter(context.STENCIL_VALUE_MASK), 0xFFFFFFFF);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.STENCIL_WRITEMASK), 0xFFFFFFFF);

                WebGLTestUtils.shouldBe(() => context.getParameter(context.TEXTURE_BINDING_2D), null);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.TEXTURE_BINDING_CUBE_MAP), null);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.UNPACK_ALIGNMENT), 4);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.UNPACK_FLIP_Y_WEBGL), false);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.UNPACK_PREMULTIPLY_ALPHA_WEBGL), false);
                WebGLTestUtils.shouldBe(() => context.getParameter(context.VIEWPORT), new Int32Array(new[] {0, 0, 2, 2}));
                //WebGLTestUtils.shouldBeNull(() => context.getParameter(context.NUM_COMPRESSED_TEXTURE_FORMATS));

                WebGLTestUtils.shouldBeTrue(() => context.getParameter(context.MAX_COMBINED_TEXTURE_IMAGE_UNITS) >= 8);
                WebGLTestUtils.shouldBeTrue(() => context.getParameter(context.MAX_CUBE_MAP_TEXTURE_SIZE) >= 16);
                WebGLTestUtils.shouldBeTrue(() => context.getParameter(context.MAX_FRAGMENT_UNIFORM_VECTORS) >= 16);
                WebGLTestUtils.shouldBeTrue(() => context.getParameter(context.MAX_RENDERBUFFER_SIZE) >= 1);
                WebGLTestUtils.shouldBeTrue(() => context.getParameter(context.MAX_TEXTURE_IMAGE_UNITS) >= 8);
                WebGLTestUtils.shouldBeTrue(() => context.getParameter(context.MAX_TEXTURE_SIZE) >= 64);
                WebGLTestUtils.shouldBeTrue(() => context.getParameter(context.MAX_VARYING_VECTORS) >= 8);
                WebGLTestUtils.shouldBeTrue(() => context.getParameter(context.MAX_VERTEX_ATTRIBS) >= 8);
                WebGLTestUtils.shouldBeTrue(() => context.getParameter(context.MAX_VERTEX_TEXTURE_IMAGE_UNITS) >= 0);
                WebGLTestUtils.shouldBeTrue(() => context.getParameter(context.MAX_VERTEX_UNIFORM_VECTORS) >= 128);

                WebGLTestUtils.shouldBeTrue(() => context.getParameter(context.MAX_VIEWPORT_DIMS)[0] >= Canvas.width);
                WebGLTestUtils.shouldBeTrue(() => context.getParameter(context.MAX_VIEWPORT_DIMS)[1] >= Canvas.height);
            }
        }
    }
}