using System;
using NUnit.Framework;

namespace WebGL.UnitTests
{
    [TestFixture]
    public class DataViewTest
    {
        [Test]
        public void ShouldCorrectlyReadAndWriteAllSupportedTypes()
        {
            var view = new DataView(new ArrayBuffer(8));
            view.setInt8(0, -9);
            Assert.That(view.getInt8(0), Is.EqualTo(-9));
            view.setUint8(0, 32);
            Assert.That(view.getUint8(0), Is.EqualTo(32));
            view.setInt16(0, -1332);
            Assert.That(view.getInt16(0), Is.EqualTo(-1332));
            view.setUint16(0, 8912);
            Assert.That(view.getUint16(0), Is.EqualTo(8912));
            view.setInt32(0, -807434011);
            Assert.That(view.getInt32(0), Is.EqualTo(-807434011));
            view.setUint32(0, 1902846222);
            Assert.That(view.getUint32(0), Is.EqualTo(1902846222));
            view.setFloat32(0, 192.420111f);
            Assert.That(view.getFloat32(0), Is.EqualTo(192.420111f));
            view.setFloat64(0, -999822222.391283112);
            Assert.That(view.getFloat64(0), Is.EqualTo(-999822222.391283112));
        }

        [Test]
        public void ShouldCreateViewOverEntireBuffer()
        {
            var view = new DataView(new ArrayBuffer(16));
            view.setInt32(0, 52);
            view.setInt32(4, -84);
            view.setInt32(8, 64);
            view.setInt32(12, 34);
            Assert.That(view.getInt32(0), Is.EqualTo(52));
            Assert.That(view.getInt32(4), Is.EqualTo(-84));
            Assert.That(view.getInt32(8), Is.EqualTo(64));
            Assert.That(view.getInt32(12), Is.EqualTo(34));
        }

        [Test]
        public void ShouldCreateViewOverSectionOfBuffer()
        {
            var buffer = new ArrayBuffer(8);
            var view = new DataView(buffer);
            view.setInt8(0, 52);
            view.setInt8(1, -84);
            view.setInt8(2, 64);
            view.setInt8(3, 34);
            view.setInt8(4, -102);
            view.setInt8(5, 103);
            view.setInt8(6, -23);
            view.setInt8(7, -64);
            var smallView = new DataView(buffer, 4, 1);
            Assert.That(smallView.getInt8(0), Is.EqualTo(-102));
        }

        [Test]
        public void ShouldStoreBytesInCorrectSequence()
        {
            var buffer = new ArrayBuffer(8);
            var view = new DataView(buffer);
            view.setFloat32(0, 5.4f);
            view.setFloat32(4, -7.3f);
            Assert.That(buffer.data[0], Is.EqualTo(205));
            Assert.That(buffer.data[1], Is.EqualTo(204));
            Assert.That(buffer.data[2], Is.EqualTo(172));
            Assert.That(buffer.data[3], Is.EqualTo(64));
            Assert.That(buffer.data[4], Is.EqualTo(154));
            Assert.That(buffer.data[5], Is.EqualTo(153));
            Assert.That(buffer.data[6], Is.EqualTo(233));
            Assert.That(buffer.data[7], Is.EqualTo(192));
        }

        [Test]
        public void ShouldThrowExceptionViewRangeIsBeyondEndOfBuffer()
        {
            Assert.That(() => new DataView(new ArrayBuffer(8), 5, 8), Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]		
        public void ShouldThrowExceptionWhenReadingBeyondEndOfView()
        {
            var view = new DataView(new ArrayBuffer(32), 4, 4);
            Assert.That(() => view.getInt32(11), Throws.TypeOf<IndexOutOfRangeException>());
        }

        [Test]		
        public void ShouldThrowExceptionWhenViewTooSmallForType()
        {
            var view = new DataView(new ArrayBuffer(7));
            Assert.That(() => view.getFloat64(0), Throws.TypeOf<IndexOutOfRangeException>());
        }
    }
}