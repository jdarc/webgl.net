using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class Int8ArrayTests
    {
        [Test]
        public void ShouldCreateInstanceGivenLength()
        {
            var array = new Int8Array(5);
            Assert.That(array.buffer, Is.Not.Null);
            Assert.That(array.length, Is.EqualTo(5));
            Assert.That(array.byteLength, Is.EqualTo(5));
            Assert.That(array.byteOffset, Is.EqualTo(0));
            Assert.That(array.bytesPerElement, Is.EqualTo(1));
        }

        [Test]
        public void ShouldCreateInstanceFromAnotherArray()
        {
            var array = new Int8Array(new sbyte[] {-1, 11, 3, 120, -118, 10, -124, -32});
            Assert.That(array.buffer, Is.Not.Null);
            Assert.That(array.length, Is.EqualTo(8));
            Assert.That(array.byteLength, Is.EqualTo(8));
            Assert.That(array.byteOffset, Is.EqualTo(0));
            Assert.That(array.bytesPerElement, Is.EqualTo(1));

            Assert.That(array[0], Is.EqualTo(-1));
            Assert.That(array[1], Is.EqualTo(11));
            Assert.That(array[2], Is.EqualTo(3));
            Assert.That(array[3], Is.EqualTo(120));
            Assert.That(array[4], Is.EqualTo(-118));
            Assert.That(array[5], Is.EqualTo(10));
            Assert.That(array[6], Is.EqualTo(-124));
            Assert.That(array[7], Is.EqualTo(-32));
        }

        [Test]
        public void ShouldCreateInstanceFromIntegerArray()
        {
            var array = new Int8Array(new sbyte[] {-1, 11, 3, 120, -118, 10, -124, -32});
            Assert.That(array.buffer, Is.Not.Null);
            Assert.That(array.length, Is.EqualTo(8));
            Assert.That(array.byteLength, Is.EqualTo(8));
            Assert.That(array.byteOffset, Is.EqualTo(0));
            Assert.That(array.bytesPerElement, Is.EqualTo(1));

            Assert.That(array[0], Is.EqualTo(-1));
            Assert.That(array[1], Is.EqualTo(11));
            Assert.That(array[2], Is.EqualTo(3));
            Assert.That(array[3], Is.EqualTo(120));
            Assert.That(array[4], Is.EqualTo(-118));
            Assert.That(array[5], Is.EqualTo(10));
            Assert.That(array[6], Is.EqualTo(-124));
            Assert.That(array[7], Is.EqualTo(-32));
        }

        [Test]
        public void ShouldSetValuesUsingIndexer()
        {
            var array = new Int8Array(5);
            array[0] = 0;
            array[1] = 127;
            array[2] = -61;
            array[3] = -128;
            array[4] = 92;
            Assert.That(array[0], Is.EqualTo(0));
            Assert.That(array[1], Is.EqualTo(127));
            Assert.That(array[2], Is.EqualTo(-61));
            Assert.That(array[3], Is.EqualTo(-128));
            Assert.That(array[4], Is.EqualTo(92));
        }
    }
}