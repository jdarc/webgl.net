using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class Float64ArrayTests
    {
        [Test]
        public void shouldCreateInstanceGivenLength()
        {
            var array = new Float64Array(5);
            Assert.That(array.buffer, Is.Not.Null);
            Assert.That(array.length, Is.EqualTo(5));
            Assert.That(array.byteLength, Is.EqualTo(40));
            Assert.That(array.byteOffset, Is.EqualTo(0));
            Assert.That(array.bytesPerElement, Is.EqualTo(8));
        }

        [Test]
        public void shouldCreateInstanceFromAnotherArray()
        {
            var array = new Float64Array(new[] {5.5, 134.25, -11.75, -5, -99.5, 3001.5});
            Assert.That(array.buffer, Is.Not.Null);
            Assert.That(array.length, Is.EqualTo(6));
            Assert.That(array.byteLength, Is.EqualTo(48));
            Assert.That(array.byteOffset, Is.EqualTo(0));
            Assert.That(array.bytesPerElement, Is.EqualTo(8));

            Assert.That(array[0], Is.EqualTo(5.5));
            Assert.That(array[1], Is.EqualTo(134.25));
            Assert.That(array[2], Is.EqualTo(-11.75));
            Assert.That(array[3], Is.EqualTo(-5));
            Assert.That(array[4], Is.EqualTo(-99.5));
            Assert.That(array[5], Is.EqualTo(3001.5));
        }

        [Test]
        public void shouldSetValuesUsingIndexer()
        {
            var array = new Float64Array(5);
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