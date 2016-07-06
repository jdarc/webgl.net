using System;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLSL2TypesOfTexturesOnSameUnit : BaseTest
    {
        private static readonly Script vshader = new Script("vshader", "x-shader/x-vertex")
                                                 {text = @"
attribute vec4 vPosition;
attribute vec2 texCoord0;
varying vec2 texCoord;
void main()
{
  gl_Position = vPosition;
  texCoord = texCoord0;
}"};

        private static readonly Script fshader = new Script("fshader", "x-shader/x-fragment")
                                                 {
                                                     text =
                                                         @"
#ifdef GL_ES
precision highp float;
#endif
uniform sampler2D tex2d;
uniform samplerCube texCube;
varying vec2 texCoord;
void main()
{
  gl_FragColor =  texture2D(tex2d, texCoord) +
                  textureCube(texCube, vec3(0,1,0));
}"
                                                 };

        [Test(Description = "Tests that using 2 types of textures on the same texture unit and referencing them both in the same program fails as per OpenGL ES 2.0.24 spec section 2.10.4, Samplers subsection.")]
        public void ShouldDoMagic()
        {
            wtu.debug("");

            HTMLCanvasElement canvas2d = null; // document.getElementById("canvas2d");
            // CanvasRenderingContext2D ctx2d = null; // canvas2d.getContext<CanvasRenderingContext2D>("2d");

            var initWebGL = wtu.initWebGL(Canvas, vshader, fshader, new[] {"vPosition", "texCoord0"}, new float[] {0, 0, 0, 1}, 1);
            WebGLRenderingContext gl = initWebGL.context;
            WebGLProgram program = initWebGL.program;

            gl.disable(gl.DEPTH_TEST);
            gl.disable(gl.BLEND);

            var vertexObject = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, vertexObject);
            gl.bufferData(
                gl.ARRAY_BUFFER,
                new Float32Array(new float[]
                                 {
                                     -1, 1, 0, 1, 1, 0, -1, -1, 0,
                                     -1, -1, 0, 1, 1, 0, 1, -1, 0
                                 }),
                gl.STATIC_DRAW);
            gl.enableVertexAttribArray(0);
            gl.vertexAttribPointer(0, 3, gl.FLOAT, false, 0, 0);

            vertexObject = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, vertexObject);
            gl.bufferData(
                gl.ARRAY_BUFFER,
                new Float32Array(new float[]
                                 {
                                     0, 0, 1, 0, 0, 1,
                                     0, 1, 1, 0, 1, 1
                                 }),
                gl.STATIC_DRAW);
            gl.enableVertexAttribArray(1);
            gl.vertexAttribPointer(1, 2, gl.FLOAT, false, 0, 0);

            // Make texture unit 1 active.
            gl.activeTexture(gl.TEXTURE1);

            // Make a 2d texture
            var tex2d = gl.createTexture();
            gl.bindTexture(gl.TEXTURE_2D, tex2d);
//            ctx2d.fillStyle = "rgba(0, 0, 255, 255)";
//            ctx2d.fillRect(0, 0, 1, 1);
            gl.texImage2D(gl.TEXTURE_2D, 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, canvas2d);

            // make a cube texture
            var texCube = gl.createTexture();
//            ctx2d.fillStyle = "rgba(0, 255, 0, 64)";
//            ctx2d.fillRect(0, 0, 1, 1);
            var targets = new[]
                          {
                              gl.TEXTURE_CUBE_MAP_POSITIVE_X,
                              gl.TEXTURE_CUBE_MAP_NEGATIVE_X,
                              gl.TEXTURE_CUBE_MAP_POSITIVE_Y,
                              gl.TEXTURE_CUBE_MAP_NEGATIVE_Y,
                              gl.TEXTURE_CUBE_MAP_POSITIVE_Z,
                              gl.TEXTURE_CUBE_MAP_NEGATIVE_Z
                          };
            for (var ii = 0; ii < targets.Length; ++ii)
            {
                gl.texImage2D(targets[ii], 0, gl.RGBA, gl.RGBA, gl.UNSIGNED_BYTE, canvas2d);
            }

            var tex2dLoc = gl.getUniformLocation(program, "tex2d");
            var texCubeLoc = gl.getUniformLocation(program, "texCube");
            gl.uniform1i(tex2dLoc, 1);
            gl.uniform1i(texCubeLoc, 1);

            gl.clearColor(1, 0, 0, 1);
            gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);

            for (var ii = 0; ii < 4; ++ii)
            {
                var x = ii % 2;
                var y = Math.Floor(ii / 2f);
                gl.drawArrays(gl.TRIANGLES, 0, 6);
                wtu.glErrorShouldBe(gl, gl.INVALID_OPERATION, "drawing with 2 different targets on the same texture unit should generate INVALID_VALUE");
            }
        }
    }
}