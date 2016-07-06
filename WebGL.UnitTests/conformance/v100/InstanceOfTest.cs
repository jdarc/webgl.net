using System;
using System.Reflection;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class InstanceOfTest : BaseTest
    {
        private Script vshader = new Script("vshader", "x-shader/x-vertex")
                                 {text = @"
attribute vec4 vPosition;
varying vec2 texCoord;
void main()
{
    gl_Position = vPosition;
}"};

        private Script fshader = new Script("fshader", "x-shader/x-fragment")
                                 {text = @"
#ifdef GL_ES
precision mediump float;
#endif
uniform vec4 color;
void main()
{
    gl_FragColor = color;
}"};

        [Test(Description = "Tests that instanceof works on WebGL objects.")]
        public void ShouldDoMagic()
        {
            wtu.debug("");
            var gl = wtu.create3DContext(Canvas);
            wtu.shouldBeTrue(() => gl is WebGLRenderingContext);
            wtu.shouldBeTrue(() => gl.createBuffer() is WebGLBuffer);
            wtu.shouldBeTrue(() => gl.createFramebuffer() is WebGLFramebuffer);
            wtu.shouldBeTrue(() => gl.createProgram() is WebGLProgram);
            wtu.shouldBeTrue(() => gl.createRenderbuffer() is WebGLRenderbuffer);
            wtu.shouldBeTrue(() => gl.createShader(gl.VERTEX_SHADER) is WebGLShader);
            wtu.shouldBeTrue(() => gl.createTexture() is WebGLTexture);

            var program = wtu.setupProgram(
                gl,
                new[]
                {
                    wtu.loadShaderFromScript(gl, vshader, gl.VERTEX_SHADER),
                    wtu.loadShaderFromScript(gl, fshader, gl.FRAGMENT_SHADER)
                },
                new[] {"vPosition"}, new[] {0});

            wtu.shouldBeTrue(() => gl.getUniformLocation(program, "color") is WebGLUniformLocation);
            wtu.shouldBeTrue(() => gl.getActiveAttrib(program, 0) is WebGLActiveInfo);
            wtu.shouldBeTrue(() => gl.getActiveUniform(program, 0) is WebGLActiveInfo);

            wtu.debug("");
            wtu.debug("Tests that those WebGL objects can not be constructed through new operator");
            wtu.debug("");

            Action<Type, string> shouldNotAllowNew =
                (objectType, objectName) =>
                {
                    var constructorInfos = objectType.GetConstructors(BindingFlags.Public | BindingFlags.Instance);
                    Assert.That(constructorInfos.Length, Is.EqualTo(0));
                };

            shouldNotAllowNew(typeof(WebGLRenderingContext), "WebGLRenderingContext");
            shouldNotAllowNew(typeof(WebGLActiveInfo), "WebGLActiveInfo");
            shouldNotAllowNew(typeof(WebGLBuffer), "WebGLBuffer");
            shouldNotAllowNew(typeof(WebGLFramebuffer), "WebGLFramebuffer");
            shouldNotAllowNew(typeof(WebGLProgram), "WebGLProgram");
            shouldNotAllowNew(typeof(WebGLRenderbuffer), "WebGLRenderbuffer");
            shouldNotAllowNew(typeof(WebGLShader), "WebGLShader");
            shouldNotAllowNew(typeof(WebGLTexture), "WebGLTexture");
            shouldNotAllowNew(typeof(WebGLUniformLocation), "WebGLUniformLocation");
        }
    }
}