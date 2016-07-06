using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLDrawelements : BaseTest
    {
        [Test(Description = "")]
        public void ShouldDoMagic()
        {
        }
    }

    [TestFixture]
    public class GLDrawElements : BaseTest
    {
        private readonly Script vshader = new Script("vshader", "x-shader/x-vertex") {text = @"        
attribute vec4 vPosition;
void main()
{
    gl_Position = vPosition;
}"};

        private readonly Script fshader = new Script("fshader", "x-shader/x-fragment") {text = @"        
void main()
{
    gl_FragColor = vec4(1.0,0.0,0.0,1.0);
}"};

        [Test(Description = "")]
        public void DrawElementsTest()
        {
            WebGLRenderingContext gl = WebGLTestUtils.initWebGL(Canvas, vshader, fshader, new[] {"vPosition"}, new float[] {0, 0, 0, 1}, 1).context;

            var vertexObject = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, vertexObject);
            gl.bufferData(gl.ARRAY_BUFFER, new Float32Array(new[] {0f, 0.5f, 0f, -0.5f, -0.5f, 0f, 0.5f, -0.5f, 0f}), gl.STATIC_DRAW);
            gl.enableVertexAttribArray(0);
            gl.vertexAttribPointer(0, 3, gl.FLOAT, false, 0, 0);

            vertexObject = gl.createBuffer();
            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, vertexObject);
            gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, new Uint16Array(new ushort[] {0, 1, 2}), gl.STATIC_DRAW);

            checkDrawElements(gl, gl.TRIANGLES, 3, gl.UNSIGNED_SHORT, gl.NO_ERROR, "can call gl.DrawElements with UNSIGNED_SHORT");

            vertexObject = gl.createBuffer();
            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, vertexObject);
            gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, new Uint8Array(new byte[] {0, 1, 2, 0}), gl.STATIC_DRAW);

            checkDrawElements(gl,
                              gl.TRIANGLES, 3, gl.UNSIGNED_BYTE,
                              gl.NO_ERROR, "can call gl.DrawElements with UNSIGNED_BYTE");
            checkDrawElements(gl,
                              desktopGL["QUAD_STRIP"], 4, gl.UNSIGNED_BYTE,
                              gl.INVALID_ENUM, "gl.DrawElements with QUAD_STRIP should return INVALID_ENUM");
            checkDrawElements(gl,
                              desktopGL["QUADS"], 4, gl.UNSIGNED_BYTE,
                              gl.INVALID_ENUM, "gl.DrawElements with QUADS should return INVALID_ENUM");
            checkDrawElements(gl,
                              desktopGL["POLYGON"], 4, gl.UNSIGNED_BYTE,
                              gl.INVALID_ENUM, "gl.DrawElements with POLYGON should return INVALID_ENUM");

            vertexObject = gl.createBuffer();
            gl.bindBuffer(gl.ELEMENT_ARRAY_BUFFER, vertexObject);
            gl.bufferData(gl.ELEMENT_ARRAY_BUFFER, new Uint32Array(new uint[] {0, 1, 2}), gl.STATIC_DRAW);

            checkDrawElements(gl,
                              gl.TRIANGLES, 3, gl.UNSIGNED_INT,
                              gl.INVALID_ENUM, "gl.DrawElements should return INVALID_ENUM with UNSIGNED_INT");
        }

        private static void checkDrawElements(WebGLRenderingContext gl, uint mode, int count, uint type, uint expect, string msg)
        {
            gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);
            gl.drawElements(mode, count, type, 0);
            WebGLTestUtils.glErrorShouldBe(gl, expect, msg);
        }
    }
}