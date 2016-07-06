using System;
using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class EGLTests
    {
        private IntPtr _display;

        [SetUp]
        public void SetUp()
        {
            _display = EGL.eglGetDisplay(IntPtr.Zero);
            Assert.That(_display, Is.Not.EqualTo(IntPtr.Zero));

            int major;
            int minor;
            var initialize = EGL.eglInitialize(_display, out major, out minor);
            Assert.That(initialize, Is.EqualTo(EGL.EGL_TRUE));
            Assert.That(major, Is.Not.EqualTo(0));
            Assert.That(minor, Is.Not.EqualTo(0));
        }

        [TearDown]
        public void TearDown()
        {
            var terminate = EGL.eglTerminate(_display);
            Assert.That(terminate, Is.EqualTo(EGL.EGL_TRUE));
        }

        [Test]
        public void ShouldGetError()
        {
            var error = EGL.eglGetError();
            Assert.That(error, Is.EqualTo(EGL.EGL_SUCCESS));
        }

        [Test]
        public void ShouldQueryString()
        {
            Assert.That(EGL.eglQueryString(_display, EGL.EGL_CLIENT_APIS), Is.EqualTo("OpenGL_ES"));
            Assert.That(EGL.eglQueryString(_display, EGL.EGL_VENDOR), Is.Not.EqualTo(string.Empty));
            Assert.That(EGL.eglQueryString(_display, EGL.EGL_VERSION), Is.StringContaining("1.4"));
            Assert.That(EGL.eglQueryString(_display, EGL.EGL_EXTENSIONS), Is.StringContaining("EXT"));
        }

        [Test]
        public void ShouldGetConfigs()
        {
            int numConfigs;
            var result = EGL.eglGetConfigs(_display, null, 0, out numConfigs);
            Assert.That(result, Is.EqualTo(EGL.EGL_TRUE));
            Assert.That(numConfigs, Is.GreaterThan(0));

            var configs = new IntPtr[numConfigs];
            result = EGL.eglGetConfigs(_display, configs, numConfigs, out numConfigs);
            Assert.That(result, Is.EqualTo(EGL.EGL_TRUE));
            foreach (var config in configs)
            {
                Assert.That(config, Is.Not.EqualTo(IntPtr.Zero));
            }
        }

        [Test]
        public void ShouldGetProcAddress()
        {
            var procAddress = EGL.eglGetProcAddress("glGetGraphicsResetStatusEXT");
            Assert.That(procAddress, Is.Not.EqualTo(IntPtr.Zero));
        }
    }
}