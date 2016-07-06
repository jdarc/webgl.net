using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class Float32ArrayTests
    {
        [Test]
        public void ShouldCreateInstanceGivenLength()
        {
            var array = new Float32Array(5);
            Assert.That(array.buffer, Is.Not.Null);
            Assert.That(array.length, Is.EqualTo(5));
            Assert.That(array.byteLength, Is.EqualTo(20));
            Assert.That(array.byteOffset, Is.EqualTo(0));
            Assert.That(array.bytesPerElement, Is.EqualTo(4));
        }

        [Test]
        public void ShouldCreateInstanceFromAnotherArray()
        {
            var array = new Float32Array(new[] {5.5f, 134.25f, -11.75f, -5f, -99.5f, 3001.5f});
            Assert.That(array.buffer, Is.Not.Null);
            Assert.That(array.length, Is.EqualTo(6));
            Assert.That(array.byteLength, Is.EqualTo(24));
            Assert.That(array.byteOffset, Is.EqualTo(0));
            Assert.That(array.bytesPerElement, Is.EqualTo(4));

            Assert.That(array[0], Is.EqualTo(5.5));
            Assert.That(array[1], Is.EqualTo(134.25));
            Assert.That(array[2], Is.EqualTo(-11.75));
            Assert.That(array[3], Is.EqualTo(-5));
            Assert.That(array[4], Is.EqualTo(-99.5));
            Assert.That(array[5], Is.EqualTo(3001.5));
        }

        [Test]
        public void ShouldSetValuesUsingIndexer()
        {
            var array = new Float32Array(5);
            array[0] = -8200.5f;
            array[1] = 127;
            array[2] = -611.75f;
            array[3] = -34528.0f;
            array[4] = 11292.0f;
            Assert.That(array[0], Is.EqualTo(-8200.5f));
            Assert.That(array[1], Is.EqualTo(127));
            Assert.That(array[2], Is.EqualTo(-611.75f));
            Assert.That(array[3], Is.EqualTo(-34528.0f));
            Assert.That(array[4], Is.EqualTo(11292.0f));
        }
    }
}