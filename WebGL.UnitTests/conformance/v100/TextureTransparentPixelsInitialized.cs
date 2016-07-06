using System.Drawing;
using NUnit.Framework;
using wtu = WebGL.UnitTests.WebGLTestUtils;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class TextureTransparentPixelsInitialized : BaseTest
    {
        [Test(Description = "Tests there is no garbage in transparent regions of images uploaded as textures")]
        public void ShouldDoMagic()
        {
            WebGLRenderingContext gl = null;
            WebGLUniformLocation textureLoc = null;
            var successfullyParsed = false;

            gl = wtu.create3DContext(Canvas);
            var program = wtu.setupTexturedQuad(gl);
            gl.clearColor(0.5f, 0.5f, 0.5f, 1);
            gl.clearDepth(1);

            textureLoc = gl.getUniformLocation(program, "tex");

            // The input texture has 8 characters; take the leftmost one
            var coeff = 1.0f / 8.0f;
            var texCoords = new Float32Array(new[]
                                             {
                                                 coeff, 1.0f,
                                                 0.0f, 1.0f,
                                                 0.0f, 0.0f,
                                                 coeff, 1.0f,
                                                 0.0f, 0.0f,
                                                 coeff, 0.0f
                                             });

            var vbo = gl.createBuffer();
            gl.bindBuffer(gl.ARRAY_BUFFER, vbo);
            gl.bufferData(gl.ARRAY_BUFFER, texCoords, gl.STATIC_DRAW);
            gl.enableVertexAttribArray(1);
            gl.vertexAttribPointer(1, 2, gl.FLOAT, false, 0, 0);

            var texture = wtu.loadTexture(gl, "resources/bug-32888-texture.png", x => { });

            gl.clear(gl.COLOR_BUFFER_BIT | gl.DEPTH_BUFFER_BIT);
            gl.enable(gl.BLEND);
            gl.blendFunc(gl.SRC_ALPHA, gl.ONE_MINUS_SRC_ALPHA);
            // Bind the texture to texture unit 0
            gl.bindTexture(gl.TEXTURE_2D, texture);
            // Point the uniform sampler to texture unit 0
            gl.uniform1i(textureLoc, 0);
            // Draw the triangles
            wtu.drawQuad(gl, new byte[] {0, 0, 0, 255});

            // Spot check a couple of 2x2 regions in the upper and lower left
            // corners; they should be the rgb values in the texture.
            var color = new[] {0, 0, 0};
            wtu.debug("Checking lower left corner");
            wtu.checkCanvasRect(gl, 1, gl.canvas.height - 3, 2, 2, color, "shouldBe " + color);
            wtu.debug("Checking upper left corner");
            wtu.checkCanvasRect(gl, 1, 1, 2, 2, color, "shouldBe " + color);

            wtu.finishTest();
        }

        public override Size PreferredSize
        {
            get { return new Size(32, 32); }
        }
    }
}