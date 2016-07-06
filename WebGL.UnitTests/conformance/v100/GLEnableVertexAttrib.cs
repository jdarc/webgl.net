using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLEnableVertexAttrib : BaseTest
    {
        private static readonly Script vshader = new Script("vshader", "x-shader/x-vertex") {text = @"
attribute vec4 vPosition;
void main()
{
    gl_Position = vPosition;
}"};

        private static readonly Script fshader = new Script("fshader", "x-shader/x-fragment") {text = @"
void main()
{
    gl_FragColor = vec4(1.0,0.0,0.0,1.0);
}"};

        [Test(Description = "")]
        public void ShouldDoMagic()
        {
            WebGLRenderingContext gl = WebGLTestUtils.initWebGL(Canvas, vshader, fshader, new[] {"vPosition"}, new float[] {0, 0, 0, 1}, 1).context;
            gl.viewport(0, 0, 50, 50);

            var vertexObject = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, vertexObject);
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(new[] {0f, 0.5f, 0f, -0.5f, -0.5f, 0f, 0.5f, -0.5f, 0f}), gl.STATIC_DRAW);
            gl.enableVertexAttribArray(0);
            gl.vertexAttribPointer(0, 3, gl.FLOAT, false, 0, 0);

            gl.enableVertexAttribArray(3);
            WebGLTestUtils.glErrorShouldBe(gl, gl.NO_ERROR);

            gl.drawArrays(gl.TRIANGLES, 0, 3);
            WebGLTestUtils.glErrorShouldBe(gl, gl.INVALID_OPERATION);
        }
    }
}