using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class WebGLRenderingContextTest
    {
        private Form _form;
        private HTMLCanvasElement _canvas;
        private WebGLRenderingContext _context;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _form = new Form
                    {
                        ClientSize = new Size(640, 480),
                        AutoScaleDimensions = new SizeF(6F, 13F),
                        AutoScaleMode = AutoScaleMode.Font,
                        Name = "MainForm",
                        StartPosition = FormStartPosition.CenterScreen,
                        Text = "WebGL.NET"
                    };
            _form.Show();
            _canvas = new HTMLCanvasElement(_form);
            _context = (WebGLRenderingContext)_canvas.getContext("webgl");
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            _canvas.Dispose();
            _form.Close();
        }

        [Test]
        public void ShouldReturnDrawingBufferDimensions()
        {
            var clientSize = _form.ClientSize;

            var width = _context.drawingBufferWidth();
            var height = _context.drawingBufferHeight();

            Assert.That(width, Is.EqualTo(clientSize.Width));
            Assert.That(height, Is.EqualTo(clientSize.Height));
        }

        [Test]
        public void ShouldGetSupportedExtensions()
        {
            var expected = new List<string>
                           {
                               "OES_texture_float",
                               "OES_standard_derivatives",
                               "WEBKIT_EXT_texture_filter_anisotropic",
                               "WEBGL_lose_context",
                               "WEBGL_compressed_texture_s3tc",
                               "WEBGL_depth_texture"
                           };

            var supportedExtensions = _context.getSupportedExtensions();
            Assert.That(supportedExtensions, Is.Not.Null);
            foreach (var extension in expected)
            {
                Assert.That(supportedExtensions, Contains.Item(extension));
            }
        }

        [Test]
        public void ShouldReportLostContext()
        {
            Assert.That(_context.isContextLost(), Is.False);
        }

        [Test]
        public void ShouldGetContextAttributes()
        {
            var attributes = _context.getContextAttributes();
            Assert.That(attributes.alpha(), Is.True);
            Assert.That(attributes.antialias(), Is.False);
            Assert.That(attributes.depth(), Is.True);
            Assert.That(attributes.premultipliedAlpha(), Is.True);
            Assert.That(attributes.preserveDrawingBuffer(), Is.False);
            Assert.That(attributes.stencil(), Is.False);
        }
    }
}