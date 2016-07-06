using WebGL;

namespace Demo.Benchmark
{
    internal class GLProgram
    {
        internal class CachedAttributes
        {
            internal int position;
            internal int normal;
            internal int uv;
            internal int uv2;
            internal int color;
        }

        internal class CachedUniforms
        {
            internal WebGLUniformLocation viewMatrix;
            internal WebGLUniformLocation modelViewMatrix;
            internal WebGLUniformLocation projectionMatrix;
            internal WebGLUniformLocation normalMatrix;
            internal WebGLUniformLocation modelMatrix;
            internal WebGLUniformLocation cameraPosition;
        }

        internal int id;
        internal WebGLProgram program;
        internal CachedUniforms uniforms;
        internal CachedAttributes attributes;

        internal GLProgram(WebGLProgram prog)
        {
            program = prog;
        }
    }
}