using System;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class OESTextureFloat : BaseTest
    {
        private readonly Script testFragmentShader =
            new Script("testFragmentShader", "x-shader/x-fragment")
            {
                text =
                    @"
precision mediump float;
uniform sampler2D tex;
varying vec2 texCoord;
void main()
{
    vec4 color = texture2D(tex, texCoord);
    if (abs(color.r - 10000.0) +
        abs(color.g - 10000.0) +
        abs(color.b - 10000.0) +
        abs(color.a - 10000.0) < 8.0) {
        gl_FragColor = vec4(0.0, 1.0, 0.0, 1.0);
    } else {
        gl_FragColor = vec4(1.0, 0.0, 0.0, 1.0);
    }
}"
            };

        private readonly Script positionVertexShader =
            new Script("positionVertexShader", "x-shader/x-vertex") {text = @"
attribute vec4 vPosition;
void main()
{
    gl_Position = vPosition;
}"};

        private readonly Script floatingPointFragmentShader =
            new Script("floatingPointFragmentShader", "x-shader/x-fragment") {text = @"
void main()
{
    gl_FragColor = vec4(10000.0, 10000.0, 10000.0, 10000.0);
}"};

        [Test(Description = "This test verifies the functionality of the OES_texture_float extension, if it is available.")]
        public void ShouldDoMagic()
        {
            wtu.debug("");

            var gl = wtu.create3DContext(Canvas);

            Func<dynamic> allocateTexture =
                () =>
                {
                    var texture = gl.createTexture();
                    gl.bindTexture(gl.TEXTURE_2D, texture);
                    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MIN_FILTER, (int)gl.NEAREST);
                    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_MAG_FILTER, (int)gl.NEAREST);
                    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_S, (int)gl.CLAMP_TO_EDGE);
                    gl.texParameteri(gl.TEXTURE_2D, gl.TEXTURE_WRAP_T, (int)gl.CLAMP_TO_EDGE);
                    wtu.glErrorShouldBe(gl, gl.NO_ERROR, "texture parameter setup should succeed");
                    return texture;
                };

            Action checkRenderingResults =
                () =>
                {
                    var pixels = new Uint8Array(4);
                    gl.readPixels(0, 0, 1, 1, gl.RGBA, gl.UNSIGNED_BYTE, pixels);
                    // Outputs green if OK, red if not.
                    wtu.shouldBe(() => pixels[0], (byte)0);
                    wtu.shouldBe(() => pixels[1], (byte)255);
                    wtu.shouldBe(() => pixels[2], (byte)0);
                    wtu.shouldBe(() => pixels[3], (byte)255);
                };

            Action<dynamic, dynamic> runTextureCreationTest =
                (testProgram, extensionEnabled) =>
                {
                    var expectFailure = !extensionEnabled;

                    var texture = allocateTexture();
                    // Generate data.
                    var width = 2;
                    var height = 2;
                    var numberOfChannels = 4;
                    var data = new Float32Array(width * height * numberOfChannels);
                    for (var ii = 0; ii < data.length; ++ii)
                    {
                        data[ii] = 10000;
                    }
                    gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, width, height, 0, gl.RGBA, gl.FLOAT, data);
                    if (expectFailure)
                    {
                        wtu.glErrorShouldBe(gl, gl.INVALID_ENUM, "floating-point texture allocation must be disallowed if OES_texture_float isn't enabled");
                        return;
                    }
                    else
                    {
                        wtu.glErrorShouldBe(gl, gl.NO_ERROR, "floating-point texture allocation should succeed if OES_texture_float is enabled");
                    }
                    // Verify that the texture actually works for sampling and contains the expected data.
                    gl.uniform1i(gl.getUniformLocation(testProgram, "tex"), 0);
                    wtu.drawQuad(gl);
                    checkRenderingResults();
                };

            Action<dynamic> runRenderTargetTest =
                (testProgram) =>
                {
                    var texture = allocateTexture();
                    var width = 2;
                    var height = 2;
                    var numberOfChannels = 4;
                    gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, width, height, 0, gl.RGBA, gl.FLOAT, null);
                    wtu.glErrorShouldBe(gl, gl.NO_ERROR, "floating-point texture allocation should succeed if OES_texture_float is enabled");

                    // Use this texture as a render target.
                    var fbo = gl.createFramebuffer();
                    gl.bindFramebuffer(gl.FRAMEBUFFER, fbo);
                    gl.framebufferTexture2D(gl.FRAMEBUFFER, gl.COLOR_ATTACHMENT0, gl.TEXTURE_2D, texture, 0);
                    gl.bindTexture(gl.TEXTURE_2D, null);
                    wtu.shouldBe(() => gl.checkFramebufferStatus(gl.FRAMEBUFFER), gl.FRAMEBUFFER_COMPLETE);
                    // While strictly speaking it is probably legal for a WebGL implementation to support
                    // floating-point textures but not as attachments to framebuffer objects, any such
                    // implementation is so poor that it arguably should not advertise support for the
                    // OES_texture_float extension. For this reason the conformance test requires that the
                    // framebuffer is complete here.
                    if (gl.checkFramebufferStatus(gl.FRAMEBUFFER) != gl.FRAMEBUFFER_COMPLETE)
                    {
                        return;
                    }

                    var shaders = new[] {wtu.loadShaderFromScript(gl, positionVertexShader), wtu.loadShaderFromScript(gl, floatingPointFragmentShader)};
                    var renderProgram = wtu.setupProgram(gl, shaders, new[] {"vPosition"}, new[] {0});
                    wtu.drawQuad(gl);
                    wtu.glErrorShouldBe(gl, gl.NO_ERROR, "rendering to floating-point texture should succeed");

                    // Now sample from the floating-point texture and verify we got the correct values.
                    gl.bindFramebuffer(gl.FRAMEBUFFER, null);
                    gl.bindTexture(gl.TEXTURE_2D, texture);
                    gl.useProgram(testProgram);
                    gl.uniform1i(gl.getUniformLocation(testProgram, "tex"), 0);
                    wtu.drawQuad(gl);
                    wtu.glErrorShouldBe(gl, gl.NO_ERROR, "rendering from floating-point texture should succeed");
                    checkRenderingResults();
                };

            Action attemptToForceGC = () => GC.Collect();

            Action runUniqueObjectTest =
                () =>
                {
                    wtu.debug("Testing that getExtension() returns the same object each time");
                    var extension = gl.getExtension("OES_texture_float");
                    var hashcode = extension.GetHashCode();
                    attemptToForceGC();
                    wtu.shouldBe(() => gl.getExtension("OES_texture_float").GetHashCode(), hashcode);
                };

            if (gl == null)
            {
                wtu.testFailed("WebGL context does not exist");
            }
            else
            {
                wtu.testPassed("WebGL context exists");

                var texturedShaders = new[]
                                      {
                                          wtu.setupSimpleTextureVertexShader(gl),
                                          wtu.loadShaderFromScript(gl, testFragmentShader)
                                      };
                var testProgram = wtu.setupProgram(gl, texturedShaders, new[] {"vPosition", "texCoord0"}, new[] {0, 1});
                var quadParameters = wtu.setupUnitQuad(gl, 0, 1);

                // First verify that allocation of floating-point textures fails if the extension has not been enabled yet.
                runTextureCreationTest(testProgram, false);

                if (gl.getExtension("OES_texture_float") == null)
                {
                    wtu.testPassed("No OES_texture_float support -- this is legal");
                }
                else
                {
                    wtu.testPassed("Successfully enabled OES_texture_float extension");
                    runTextureCreationTest(testProgram, true);
                    runRenderTargetTest(testProgram);
                    runUniqueObjectTest();
                }
            }
        }
    }
}