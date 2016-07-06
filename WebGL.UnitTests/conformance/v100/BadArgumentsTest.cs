using System;
using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class BadArgumentsTest : BaseTest
    {
        private class ValueThrows
        {
            public readonly dynamic Value;
            public readonly bool Throws;

            public ValueThrows(dynamic value, bool throws)
            {
                Value = value;
                Throws = throws;
            }
        }

        [Test(Description = "Tests calling WebGL APIs with wrong argument types")]
        public void ShouldDoMagic()
        {
            var context = WebGLTestUtils.create3DContext(Canvas);
            var program = WebGLTestUtils.loadStandardProgram(context);
            var shader = WebGLTestUtils.loadStandardVertexShader(context);
            Action<WebGLRenderingContext, uint, Action> shouldGenerateGLError = WebGLTestUtils.shouldGenerateGLError;

            Assert.That(program != null, "Program Compiled");
            Assert.That(shader != null, "Shader Compiled");

            var loc = context.getUniformLocation(program, "u_modelViewProjMatrix");
            Assert.That(loc != null, "getUniformLocation succeeded");

            var arguments = new[]
                            {
                                new ValueThrows("foo", true),
                                new ValueThrows(0, true),
                                new ValueThrows(null, false)
                            };

            Action<string> shouldBeEmptyString = (command) => Assert.That(command, Is.EqualTo(string.Empty));

            for (var i = 0; i < arguments.Length; ++i)
            {
                Action<Func<object>, Func<object>> func;
                Action<Func<object>, Func<object>> func2;
                Action<Func<object>, Func<object>> func3;
                if (arguments[i].Throws)
                {
                    func = WebGLTestUtils.shouldThrow;
                    func2 = WebGLTestUtils.shouldThrow;
                    func3 = WebGLTestUtils.shouldThrow;
                }
                else
                {
                    func = WebGLTestUtils.shouldBeUndefined;
                    func2 = WebGLTestUtils.shouldBeNull;
                    func3 = WebGLTestUtils.shouldBeEmptyString;
                }

                var argument = arguments[i].Value;
                func(() => context.compileShader(argument), null);
                func(() => context.linkProgram(argument), null);
                func(() => context.attachShader(program, argument), null);
                func(() => context.attachShader(argument, shader), null);
                func(() => context.detachShader(program, argument), null);
                func(() => context.detachShader(argument, shader), null);
                func(() => context.useProgram(argument), null);
                func(() => context.shaderSource(argument, "foo"), null);
                func(() => context.bindAttribLocation(argument, 0, "foo"), null);
                func(() => context.bindBuffer(context.ARRAY_BUFFER, argument), null);
                func(() => context.bindFramebuffer(context.FRAMEBUFFER, argument), null);
                func(() => context.bindRenderbuffer(context.RENDERBUFFER, argument), null);
                func(() => context.bindTexture(context.TEXTURE_2D, argument), null);
                func(() => context.framebufferRenderbuffer(context.FRAMEBUFFER, context.DEPTH_ATTACHMENT, context.RENDERBUFFER, argument), null);
                func(() => context.framebufferTexture2D(context.FRAMEBUFFER, context.COLOR_ATTACHMENT0, context.TEXTURE_2D, argument, 0), null);
                func(() => context.uniform2fv(argument, new Float32Array(new[] {0.0f, 0.0f})), null);
                func(() => context.uniform2iv(argument, new Int32Array(new[] {0, 0})), null);
                func(() => context.uniformMatrix2fv(argument, false, new Float32Array(new[] {0.0f, 0.0f, 0.0f, 0.0f})), null);

                func2(() => context.getProgramParameter(argument, 0), null);
                func2(() => context.getShaderParameter(argument, 0), null);
                func2(() => context.getUniform(argument, loc), null);
                func2(() => context.getUniform(program, argument), null);
                func2(() => context.getUniformLocation(argument, "u_modelViewProjMatrix"), null);

                func3(() => context.getProgramInfoLog(argument), null);
                func3(() => context.getShaderInfoLog(argument), null);
                func3(() => context.getShaderSource(argument), null);
            }
        }
    }
}