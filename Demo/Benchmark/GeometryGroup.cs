using System.Collections.Generic;
using WebGL;

namespace Demo.Benchmark
{
    internal class GeometryGroup
    {
        internal List<int> faces3;
        internal List<int> faces4;
        internal int materialIndex;
        internal int vertices;
        internal int id;

        internal WebGLBuffer __webglVertexBuffer;
        internal WebGLBuffer __webglNormalBuffer;
        internal WebGLBuffer __webglFaceBuffer;

        internal Float32Array __vertexArray;
        internal Float32Array __normalArray;
        internal Uint16Array __faceArray;

        internal int __webglFaceCount;
        internal bool __inittedArrays;
    }
}