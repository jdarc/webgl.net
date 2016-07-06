using System;
using System.Drawing;
using System.Windows.Forms;
using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class GLTests
    {
        private Form _form;
        private IntPtr _display;
        private IntPtr _surface;
        private IntPtr _context;

        [TestFixtureSetUp]
        public void TestFixtureSetUp()
        {
            _form = new Form {ClientSize = new Size(1280, 800), FormBorderStyle = FormBorderStyle.FixedSingle, StartPosition = FormStartPosition.CenterScreen};
            _form.Show();

            _display = EGL.eglGetDisplay(IntPtr.Zero);

            int major, minor;
            var initialize = EGL.eglInitialize(_display, out major, out minor);
            Assert.That(initialize, Is.EqualTo(EGL.EGL_TRUE));

            int numConfigs;
            EGL.eglGetConfigs(_display, null, 0, out numConfigs);

            var configs = new IntPtr[numConfigs];
            var attribList = new[] {EGL.EGL_RED_SIZE, 8, EGL.EGL_GREEN_SIZE, 8, EGL.EGL_BLUE_SIZE, 8, EGL.EGL_ALPHA_SIZE, 8, EGL.EGL_DEPTH_SIZE, 24, EGL.EGL_STENCIL_SIZE, 8, EGL.EGL_SAMPLE_BUFFERS, 0, EGL.EGL_NONE, EGL.EGL_NONE};
            EGL.eglChooseConfig(_display, attribList, configs, configs.Length, out numConfigs);

            var config = configs[0];
            _surface = EGL.eglCreateWindowSurface(_display, config, _form.Handle, null);

            int[] contextAttribs = {EGL.EGL_CONTEXT_CLIENT_VERSION, 2, EGL.EGL_NONE, EGL.EGL_NONE};
            _context = EGL.eglCreateContext(_display, config, IntPtr.Zero, contextAttribs);
        }

        [TestFixtureTearDown]
        public void TestFixtureTearDown()
        {
            var destroyContext = EGL.eglDestroyContext(_display, _context);
            var destroySurface = EGL.eglDestroySurface(_display, _surface);
            var terminate = EGL.eglTerminate(_display);
            Assert.That(destroyContext, Is.EqualTo(EGL.EGL_TRUE));
            Assert.That(destroySurface, Is.EqualTo(EGL.EGL_TRUE));
            Assert.That(terminate, Is.EqualTo(EGL.EGL_TRUE));

            _form.Close();
            _form.Dispose();
        }

        [Test]
        public void ShouldDoSomeMagic()
        {
            MakeCurrent();
            GLES.glClearColor(0.0f, 0.0f, 0.0f, 1.0f);
            GLES.glClear(GLES.GL_COLOR_BUFFER_BIT | GLES.GL_DEPTH_BUFFER_BIT);
            SwapBuffers();
        }

        [Test]
        public void ShouldGetExtensions()
        {
            MakeCurrent();
            var result = GLES.glGetString(GLES.GL_EXTENSIONS);
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Contains.Substring("EXT"));
        }

        [Test, Ignore("")]
        public void ShouldGetBooleanv()
        {
            MakeCurrent();
            var colorMask = new byte[4];
            GLES.glGetBooleanv(GLES.GL_COLOR_WRITEMASK, colorMask);
            Assert.That(colorMask[0], Is.Not.EqualTo(GLES.GL_FALSE));
            Assert.That(colorMask[1], Is.Not.EqualTo(GLES.GL_FALSE));
            Assert.That(colorMask[2], Is.Not.EqualTo(GLES.GL_FALSE));
            Assert.That(colorMask[3], Is.Not.EqualTo(GLES.GL_FALSE));
        }

        [Test]
        public void ShouldGetShaderPrecisionFormat()
        {
            var range = new int[2];
            int precision;
            GLES.glGetShaderPrecisionFormat(GLES.GL_FRAGMENT_SHADER, GLES.GL_HIGH_INT, range, out precision);

            JSConsole.log(range[0].ToString());
            JSConsole.log(range[1].ToString());
            JSConsole.log(precision.ToString());
        }

        private void MakeCurrent()
        {
            EGL.eglMakeCurrent(_display, _surface, _surface, _context);
        }

        private void SwapBuffers()
        {
            EGL.eglSwapBuffers(_display, _surface);
        }
    }
}