using System;
using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class ArrayBufferTests
    {
        [Test]
        public void ShouldCreateBufferOfSpecificedSize()
        {
            var length = new Random(Environment.TickCount).Next(255);
            Assert.That(new ArrayBuffer(length).byteLength, Is.EqualTo(length));
        }

        [Test]
        public void ShouldLockBufferOnlyOnce()
        {
            var buffer = new ArrayBuffer(16);
            var ptr = buffer.@lock();
            Assert.That(ptr, Is.Not.EqualTo(IntPtr.Zero));
            Assert.That(ptr, Is.EqualTo(buffer.@lock()));
        }

        [Test]
        public void ShouldUnlockBufferWhenLocked()
        {
            var buffer = new ArrayBuffer(16);
            Assert.That(buffer.isLocked, Is.False);
            buffer.@lock();
            Assert.That(buffer.isLocked, Is.True);
            buffer.unlock();
            Assert.That(buffer.isLocked, Is.False);
        }

        [Test]
        public void ShouldIgnoreRedundantUnlockRequests()
        {
            var buffer = new ArrayBuffer(16);
            Assert.That(buffer.isLocked, Is.False);
            buffer.unlock();
            Assert.That(buffer.isLocked, Is.False);
            buffer.unlock();
            Assert.That(buffer.isLocked, Is.False);
        }
    }
}