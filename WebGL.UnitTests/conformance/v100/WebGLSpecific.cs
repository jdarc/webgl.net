using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class WebGLSpecific : BaseTest
    {
        [Test(Description = "")]
        public void ShouldDoMagic()
        {
            var gl = WebGLTestUtils.create3DContext(Canvas);
            var program = WebGLTestUtils.loadStandardProgram(gl);
            gl.useProgram(program);
            var vertexObject = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, vertexObject);
            gl.enableVertexAttribArray(0);
            WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR, "Setup should succeed");

            JSConsole.debug("");
            JSConsole.debug("Verify that constant color and constant alpha cannot be used together as source and destination factors in the blend function");
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.blendFunc(gl.CONSTANT_COLOR, gl.CONSTANT_ALPHA));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.blendFunc(gl.ONE_MINUS_CONSTANT_COLOR, gl.CONSTANT_ALPHA));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.blendFunc(gl.CONSTANT_COLOR, gl.ONE_MINUS_CONSTANT_ALPHA));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.blendFunc(gl.ONE_MINUS_CONSTANT_COLOR, gl.ONE_MINUS_CONSTANT_ALPHA));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.blendFunc(gl.CONSTANT_ALPHA, gl.CONSTANT_COLOR));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.blendFunc(gl.CONSTANT_ALPHA, gl.ONE_MINUS_CONSTANT_COLOR));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.blendFunc(gl.ONE_MINUS_CONSTANT_ALPHA, gl.CONSTANT_COLOR));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.blendFunc(gl.ONE_MINUS_CONSTANT_ALPHA, gl.ONE_MINUS_CONSTANT_COLOR));

            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.blendFuncSeparate(gl.CONSTANT_COLOR, gl.CONSTANT_ALPHA, gl.ONE, gl.ZERO));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.blendFuncSeparate(gl.ONE_MINUS_CONSTANT_COLOR, gl.CONSTANT_ALPHA, gl.ONE, gl.ZERO));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.blendFuncSeparate(gl.CONSTANT_COLOR, gl.ONE_MINUS_CONSTANT_ALPHA, gl.ONE, gl.ZERO));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.blendFuncSeparate(gl.ONE_MINUS_CONSTANT_COLOR, gl.ONE_MINUS_CONSTANT_ALPHA, gl.ONE, gl.ZERO));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.blendFuncSeparate(gl.CONSTANT_ALPHA, gl.CONSTANT_COLOR, gl.ONE, gl.ZERO));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.blendFuncSeparate(gl.CONSTANT_ALPHA, gl.ONE_MINUS_CONSTANT_COLOR, gl.ONE, gl.ZERO));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.blendFuncSeparate(gl.ONE_MINUS_CONSTANT_ALPHA, gl.CONSTANT_COLOR, gl.ONE, gl.ZERO));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.blendFuncSeparate(gl.ONE_MINUS_CONSTANT_ALPHA, gl.ONE_MINUS_CONSTANT_COLOR, gl.ONE, gl.ZERO));

            JSConsole.debug("");
            JSConsole.debug("Verify that in depthRange zNear <= zFar");
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.depthRange(20, 10));

            JSConsole.debug("");
            JSConsole.debug("Verify that front/back settings should be the same for stenclMask and stencilFunc");
            WebGLTestUtils.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.stencilMask(255));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.drawArrays(gl.TRIANGLES, 0, 0));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.stencilMaskSeparate(gl.FRONT, 1));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.drawArrays(gl.TRIANGLES, 0, 0));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.stencilMaskSeparate(gl.BACK, 1));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.drawArrays(gl.TRIANGLES, 0, 0));

            WebGLTestUtils.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.stencilFunc(gl.ALWAYS, 0, 255));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.drawArrays(gl.TRIANGLES, 0, 0));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.stencilFuncSeparate(gl.BACK, gl.ALWAYS, 1, 255));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.drawArrays(gl.TRIANGLES, 0, 0));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.stencilFuncSeparate(gl.FRONT, gl.ALWAYS, 1, 255));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.drawArrays(gl.TRIANGLES, 0, 0));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.stencilFuncSeparate(gl.BACK, gl.ALWAYS, 1, 1));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.INVALID_OPERATION, () => gl.drawArrays(gl.TRIANGLES, 0, 0));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.stencilFuncSeparate(gl.FRONT, gl.ALWAYS, 1, 1));
            WebGLTestUtils.shouldGenerateGLError(gl, gl.NO_ERROR, () => gl.drawArrays(gl.TRIANGLES, 0, 0));

//            Console.debug("");
//            Console.debug("Verify that IMPLEMENTATION_COLOR_READ_FORMAT and IMPLEMENTATION_COLOR_READ_TYPE are undefined");
//            wtu.shouldBeUndefined(gl.IMPLEMENTATION_COLOR_READ_FORMAT);
//            wtu.shouldBeUndefined(gl.IMPLEMENTATION_COLOR_READ_TYPE);
//
//            Console.debug("");
//            Console.debug("Verify that *LENGTH are undefined");
//            wtu.shouldBeUndefined(gl.INFO_LOG_LENGTH);
//            wtu.shouldBeUndefined(gl.SHADER_SOURCE_LENGTH);
//            wtu.shouldBeUndefined(gl.ACTIVE_UNIFORM_MAX_LENGTH);
//            wtu.shouldBeUndefined(gl.ACTIVE_ATTRIB_MAX_LENGTH);

            JSConsole.debug("");
            JSConsole.debug("Verify that UNPACK_COLORSPACE_CONVERSION_WEBGL is supported");
            WebGLTestUtils.shouldBe(() => gl.getParameter(gl.UNPACK_COLORSPACE_CONVERSION_WEBGL), gl.BROWSER_DEFAULT_WEBGL);
            gl.pixelStorei(gl.UNPACK_COLORSPACE_CONVERSION_WEBGL, (int)gl.NONE);
            WebGLTestUtils.shouldBe(() => gl.getParameter(gl.UNPACK_COLORSPACE_CONVERSION_WEBGL), gl.NONE);
            WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR, "set/get UNPACK_COLORSPACE_CONVERSION_WEBGL should generate no error");
        }
    }
}