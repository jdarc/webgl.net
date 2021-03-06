﻿using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class Uint32ArrayTests
    {
        [Test]
        public void ShouldCreateInstanceGivenLength()
        {
            var array = new Uint32Array(5);
            Assert.That(array.buffer, Is.Not.Null);
            Assert.That(array.length, Is.EqualTo(5));
            Assert.That(array.byteLength, Is.EqualTo(20));
            Assert.That(array.byteOffset, Is.EqualTo(0));
            Assert.That(array.bytesPerElement, Is.EqualTo(4));
        }

        [Test]
        public void ShouldCreateInstanceFromAnotherArray()
        {
            var array = new Uint32Array(new uint[] {5, 134, 4294967285, 4294967291, 4294967197, 3001});
            Assert.That(array.buffer, Is.Not.Null);
            Assert.That(array.length, Is.EqualTo(6));
            Assert.That(array.byteLength, Is.EqualTo(24));
            Assert.That(array.byteOffset, Is.EqualTo(0));
            Assert.That(array.bytesPerElement, Is.EqualTo(4));

            Assert.That(array[0], Is.EqualTo(5));
            Assert.That(array[1], Is.EqualTo(134));
            Assert.That(array[2], Is.EqualTo(4294967285));
            Assert.That(array[3], Is.EqualTo(4294967291));
            Assert.That(array[4], Is.EqualTo(4294967197));
            Assert.That(array[5], Is.EqualTo(3001));
        }

        [Test]
        public void ShouldSetValuesUsingIndexer()
        {
            var array = new Uint32Array(5);
            array[0] = 8200;
            array[1] = 127;
            array[2] = 611;
            array[3] = 34528;
            array[4] = 11292;
            Assert.That(array[0], Is.EqualTo(8200));
            Assert.That(array[1], Is.EqualTo(127));
            Assert.That(array[2], Is.EqualTo(611));
            Assert.That(array[3], Is.EqualTo(34528));
            Assert.That(array[4], Is.EqualTo(11292));
        }
    }
}