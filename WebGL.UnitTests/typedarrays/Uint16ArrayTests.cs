using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class Uint16ArrayTests
    {
        [Test]
        public void ShouldCreateInstanceGivenLength()
        {
            var array = new Uint16Array(5);
            Assert.That(array.buffer, Is.Not.Null);
            Assert.That(array.length, Is.EqualTo(5));
            Assert.That(array.byteLength, Is.EqualTo(10));
            Assert.That(array.byteOffset, Is.EqualTo(0));
            Assert.That(array.bytesPerElement, Is.EqualTo(2));
        }

        [Test]
        public void ShouldCreateInstanceFromAnotherArray()
        {
            var array = new Uint16Array(new ushort[] {65535, 11, 3, 60536, 65418, 10, 132, 64736});
            Assert.That(array.buffer, Is.Not.Null);
            Assert.That(array.length, Is.EqualTo(8));
            Assert.That(array.byteLength, Is.EqualTo(16));
            Assert.That(array.byteOffset, Is.EqualTo(0));
            Assert.That(array.bytesPerElement, Is.EqualTo(2));

            Assert.That(array[0], Is.EqualTo(65535));
            Assert.That(array[1], Is.EqualTo(11));
            Assert.That(array[2], Is.EqualTo(3));
            Assert.That(array[3], Is.EqualTo(60536));
            Assert.That(array[4], Is.EqualTo(65418));
            Assert.That(array[5], Is.EqualTo(10));
            Assert.That(array[6], Is.EqualTo(132));
            Assert.That(array[7], Is.EqualTo(64736));
        }

        [Test]
        public void ShouldCreateInstanceFromIntegerArray()
        {
            var array = new Uint16Array(new ushort[] {65535, 11, 3, 60536, 65418, 10, 132, 64736});
            Assert.That(array.buffer, Is.Not.Null);
            Assert.That(array.length, Is.EqualTo(8));
            Assert.That(array.byteLength, Is.EqualTo(16));
            Assert.That(array.byteOffset, Is.EqualTo(0));
            Assert.That(array.bytesPerElement, Is.EqualTo(2));

            Assert.That(array[0], Is.EqualTo(65535));
            Assert.That(array[1], Is.EqualTo(11));
            Assert.That(array[2], Is.EqualTo(3));
            Assert.That(array[3], Is.EqualTo(60536));
            Assert.That(array[4], Is.EqualTo(65418));
            Assert.That(array[5], Is.EqualTo(10));
            Assert.That(array[6], Is.EqualTo(132));
            Assert.That(array[7], Is.EqualTo(64736));
        }

        [Test]
        public void ShouldSetValuesUsingIndexer()
        {
            var array = new Uint16Array(5);
            array[0] = 0;
            array[1] = 127;
            array[2] = 61;
            array[3] = 228;
            array[4] = 12;
            Assert.That(array[0], Is.EqualTo(0));
            Assert.That(array[1], Is.EqualTo(127));
            Assert.That(array[2], Is.EqualTo(61));
            Assert.That(array[3], Is.EqualTo(228));
            Assert.That(array[4], Is.EqualTo(12));
        }
    }
}