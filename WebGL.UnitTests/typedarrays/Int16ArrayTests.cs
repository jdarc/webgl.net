using System;
using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class Int16ArrayTests
    {
        [Test]
        public void ShouldCreateInstanceGivenLength()
        {
            var array = new Int16Array(5);
            Assert.That(array.buffer, Is.Not.Null);
            Assert.That(array.length, Is.EqualTo(5));
            Assert.That(array.byteLength, Is.EqualTo(10));
            Assert.That(array.byteOffset, Is.EqualTo(0));
            Assert.That(array.bytesPerElement, Is.EqualTo(2));
        }

        [Test]
        public void ShouldCreateInstanceFromAnotherArray()
        {
            var array = new Int16Array(new Int16[] {-1, 11, 3, -5000, -118, 10, 132, -800});
            Assert.That(array.buffer, Is.Not.Null);
            Assert.That(array.length, Is.EqualTo(8));
            Assert.That(array.byteLength, Is.EqualTo(16));
            Assert.That(array.byteOffset, Is.EqualTo(0));
            Assert.That(array.bytesPerElement, Is.EqualTo(2));

            Assert.That(array[0], Is.EqualTo(-1));
            Assert.That(array[1], Is.EqualTo(11));
            Assert.That(array[2], Is.EqualTo(3));
            Assert.That(array[3], Is.EqualTo(-5000));
            Assert.That(array[4], Is.EqualTo(-118));
            Assert.That(array[5], Is.EqualTo(10));
            Assert.That(array[6], Is.EqualTo(132));
            Assert.That(array[7], Is.EqualTo(-800));
        }

        [Test]
        public void ShouldCreateInstanceFromIntegerArray()
        {
            var array = new Int16Array(new Int16[] {-1, 11, 3, -5000, -118, 10, 132, -800});
            Assert.That(array.buffer, Is.Not.Null);
            Assert.That(array.length, Is.EqualTo(8));
            Assert.That(array.byteLength, Is.EqualTo(16));
            Assert.That(array.byteOffset, Is.EqualTo(0));
            Assert.That(array.bytesPerElement, Is.EqualTo(2));

            Assert.That(array[0], Is.EqualTo(-1));
            Assert.That(array[1], Is.EqualTo(11));
            Assert.That(array[2], Is.EqualTo(3));
            Assert.That(array[3], Is.EqualTo(-5000));
            Assert.That(array[4], Is.EqualTo(-118));
            Assert.That(array[5], Is.EqualTo(10));
            Assert.That(array[6], Is.EqualTo(132));
            Assert.That(array[7], Is.EqualTo(-800));
        }

        [Test]
        public void ShouldSetValuesUsingIndexer()
        {
            var array = new Int16Array(5);
            array[0] = 0;
            array[1] = 3127;
            array[2] = -821;
            array[3] = -128;
            array[4] = 192;
            Assert.That(array[0], Is.EqualTo(0));
            Assert.That(array[1], Is.EqualTo(3127));
            Assert.That(array[2], Is.EqualTo(-821));
            Assert.That(array[3], Is.EqualTo(-128));
            Assert.That(array[4], Is.EqualTo(192));
        }
    }
}