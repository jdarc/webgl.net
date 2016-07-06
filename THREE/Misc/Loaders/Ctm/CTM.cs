using System;
using WebGL;

namespace THREE
{
	public class CTM
	{
		public static class CompressionMethod
		{
			public const int RAW = 0x00574152;
			public const int MG1 = 0x0031474d;
			public const int MG2 = 0x0032474d;
		}

		public static class Flags
		{
			public const int NORMALS = 0x00000001;
		}

		public static readonly bool isLittleEndian = determineEndian();

		private static bool determineEndian()
		{
			var buffer = new ArrayBuffer(2);
			var bytes = new Uint8Array(buffer);
			var ints = new Uint16Array(buffer);
			bytes[0] = 1;
			return ints[0] == 1;
		}

		public class File
		{
			public FileHeader header;
			public FileBody body;

			public File(Stream stream)
			{
				load(stream);
			}

			public void load(Stream stream)
			{
				header = new FileHeader(stream);
				body = new FileBody(header);
				getReader().read(stream, body);
			}

			public dynamic getReader()
			{
				switch (header.compressionMethod)
				{
					case CompressionMethod.RAW:
						return new ReaderRAW();
					case CompressionMethod.MG1:
						return new ReaderMG1();
					case CompressionMethod.MG2:
						return new ReaderMG2();
				}

				return null;
			}
		}

		public class FileHeader
		{
			public int magic;
			public int fileFormat;
			public int compressionMethod;
			public int vertexCount;
			public int triangleCount;
			public int uvMapCount;
			public int attrMapCount;
			public int flags;
			public string comment;

			public FileHeader(Stream stream)
			{
				magic = stream.readInt32();
				fileFormat = stream.readInt32();
				compressionMethod = stream.readInt32();
				vertexCount = stream.readInt32();
				triangleCount = stream.readInt32();
				uvMapCount = stream.readInt32();
				attrMapCount = stream.readInt32();
				flags = stream.readInt32();
				comment = stream.readString();
			}

			public bool hasNormals()
			{
				return (flags & Flags.NORMALS) != 0;
			}
		}

		public class FileBody
		{
			public Uint32Array indices;
			public Float32Array vertices;
			public Float32Array normals;
			public JSArray uvMaps;
			public JSArray attrMaps;

			public FileBody(FileHeader header)
			{
				var i = header.triangleCount * 3;
				var v = header.vertexCount * 3;
				var n = header.hasNormals() ? header.vertexCount * 3 : 0;
				var u = header.vertexCount * 2;
				var a = header.vertexCount * 4;
				var j = 0;

				var data = new ArrayBuffer(
					(i + v + n + (u * header.uvMapCount) + (a * header.attrMapCount)) * 4);

				indices = new Uint32Array(data, 0, i);

				vertices = new Float32Array(data, i * 4, v);

				if (header.hasNormals())
				{
					normals = new Float32Array(data, (i + v) * 4, n);
				}

				if (header.uvMapCount > 0)
				{
					uvMaps = new JSArray();
					for (j = 0; j < header.uvMapCount;++ j)
					{
						uvMaps[j] = JSObject.create(new {uv = new Float32Array(data, (i + v + n + (j * u)) * 4, u)});
					}
				}

				if (header.attrMapCount > 0)
				{
					attrMaps = new JSArray();
					for (j = 0; j < header.attrMapCount;++ j)
					{
						attrMaps[j] = JSObject.create(new {attr = new Float32Array(data, (i + v + n + (u * header.uvMapCount) + (j * a)) * 4, a)});
					}
				}
			}
		}

		public class FileMG2Header
		{
			public int magic;
			public float vertexPrecision;
			public float normalPrecision;
			public float lowerBoundx;
			public float lowerBoundy;
			public float lowerBoundz;
			public float higherBoundx;
			public float higherBoundy;
			public float higherBoundz;
			public int divx;
			public int divy;
			public int divz;
			public float sizex;
			public float sizey;
			public float sizez;

			public FileMG2Header(Stream stream)
			{
				magic = stream.readInt32();
				vertexPrecision = stream.readFloat32();
				normalPrecision = stream.readFloat32();
				lowerBoundx = stream.readFloat32();
				lowerBoundy = stream.readFloat32();
				lowerBoundz = stream.readFloat32();
				higherBoundx = stream.readFloat32();
				higherBoundy = stream.readFloat32();
				higherBoundz = stream.readFloat32();
				divx = stream.readInt32();
				divy = stream.readInt32();
				divz = stream.readInt32();

				sizex = (higherBoundx - lowerBoundx) / divx;
				sizey = (higherBoundy - lowerBoundy) / divy;
				sizez = (higherBoundz - lowerBoundz) / divz;
			}
		}

		public class InterleavedStream
		{
			public Uint8Array data;
			public int offset;
			public int count;
			public int len;

			public InterleavedStream(dynamic data, int count)
			{
				this.data = new Uint8Array(data.buffer, data.byteOffset, data.byteLength);
				offset = isLittleEndian ? 3 : 0;
				this.count = count * 4;
				len = this.data.length;
			}

			public void writeByte(byte value)
			{
				data[offset] = value;

				offset += count;
				if (offset >= len)
				{
					offset -= len - 4;
					if (offset >= count)
					{
						offset -= count + (isLittleEndian ? 1 : -1);
					}
				}
			}
		}

		public class Stream
		{
			public static double TWO_POW_MINUS23 = System.Math.Pow(2, -23);
			public static double TWO_POW_MINUS126 = System.Math.Pow(2, -126);
			public string data;
			public int offset;

			public Stream(string data)
			{
				this.data = data;
				offset = 0;
			}

			public byte readByte()
			{
				return (byte)(data[offset++] & 0xff);
			}

			public int readInt32()
			{
				int i = readByte();
				i |= readByte() << 8;
				i |= readByte() << 16;
				return i | (readByte() << 24);
			}

			public float readFloat32()
			{
				int m = readByte();
				m += readByte() << 8;

				int b1 = readByte();
				int b2 = readByte();

				m += (b1 & 0x7f) << 16;
				var e = ((b2 & 0x7f) << 1) | ((b1 & 0x80) >> 7);
				var s = (b2 & 0x80) != 0 ? -1 : 1;

				if (e == 255)
				{
					return (float)(m != 0 ? double.NaN : s * double.PositiveInfinity);
				}
				if (e > 0)
				{
					return (float)(s * (1 + (m * TWO_POW_MINUS23)) * System.Math.Pow(2, e - 127));
				}
				if (m != 0)
				{
					return (float)(s * m * TWO_POW_MINUS126);
				}
				return s * 0;
			}

			public string readString()
			{
				var len = readInt32();

				offset += len;

				return data.Substring(offset - len, len);
			}

			public dynamic readArrayInt32(dynamic array)
			{
				var i = 0;
				var len = array.length;

				while (i < len)
				{
					array[i++] = readInt32();
				}

				return array;
			}

			public dynamic readArrayFloat32(dynamic array)
			{
				var i = 0;
				var len = array.length;

				while (i < len)
				{
					array[i++] = readFloat32();
				}

				return array;
			}
		}

		public class ReaderRAW
		{
			public ReaderRAW()
			{
				throw new NotImplementedException();
			}
		}

		public class ReaderMG1
		{
			public ReaderMG1()
			{
				throw new NotImplementedException();
			}
		}

		public class ReaderMG2
		{
			public FileMG2Header MG2Header;

			public void read(Stream stream, FileBody body)
			{
				MG2Header = new FileMG2Header(stream);

				readVertices(stream, body.vertices);
				readIndices(stream, body.indices);

				if (body.normals != null && body.normals.length > 0)
				{
					readNormals(stream, body);
				}
				if (body.uvMaps != null && body.uvMaps.length > 0)
				{
					readUVMaps(stream, body.uvMaps);
				}
				if (body.attrMaps != null && body.attrMaps.length > 0)
				{
					readAttrMaps(stream, body.attrMaps);
				}
			}

			public void readVertices(Stream stream, Float32Array vertices)
			{
				stream.readInt32();
				stream.readInt32();

				var interleaved = new InterleavedStream(vertices, 3);
				LZMA.decompress(stream, stream, interleaved, interleaved.data.length);

				var gridIndices = readGridIndices(stream, vertices);

				restoreVertices(vertices, MG2Header, gridIndices, MG2Header.vertexPrecision);
			}

			public Uint32Array readGridIndices(Stream stream, Float32Array vertices)
			{
				stream.readInt32();
				stream.readInt32();

				var gridIndices = new Uint32Array(vertices.length / 3);

				var interleaved = new InterleavedStream(gridIndices, 1);
				LZMA.decompress(stream, stream, interleaved, interleaved.data.length);

				restoreGridIndices(gridIndices, gridIndices.length);

				return gridIndices;
			}

			public void readIndices(Stream stream, Uint32Array indices)
			{
				stream.readInt32();
				stream.readInt32();

				var interleaved = new InterleavedStream(indices, 3);
				LZMA.decompress(stream, stream, interleaved, interleaved.data.length);

				restoreIndices(indices, indices.length);
			}

			public void readNormals(Stream stream, FileBody body)
			{
				stream.readInt32();
				stream.readInt32();

				var interleaved = new InterleavedStream(body.normals, 3);
				LZMA.decompress(stream, stream, interleaved, interleaved.data.length);

				var smooth = calcSmoothNormals(body.indices, body.vertices);

				restoreNormals(body.normals, smooth, MG2Header.normalPrecision);
			}

			public void readUVMaps(Stream stream, JSArray uvMaps)
			{
				var i = 0;
				for (; i < uvMaps.length;++ i)
				{
					stream.readInt32();

					uvMaps[i].name = stream.readString();
					uvMaps[i].filename = stream.readString();

					double precision = stream.readFloat32();

					stream.readInt32();

					var interleaved = new InterleavedStream(uvMaps[i].uv, 2);
					LZMA.decompress(stream, stream, interleaved, interleaved.data.length);

					CTM.restoreMap(uvMaps[i].uv, 2, precision);
				}
			}

			public void readAttrMaps(Stream stream, JSArray attrMaps)
			{
				var i = 0;
				for (; i < attrMaps.length;++ i)
				{
					stream.readInt32();

					attrMaps[i].name = stream.readString();

					var precision = stream.readFloat32();

					stream.readInt32();

					var interleaved = new InterleavedStream(attrMaps[i].attr, 4);
					LZMA.decompress(stream, stream, interleaved, interleaved.data.length);

					CTM.restoreMap(attrMaps[i].attr, 4, precision);
				}
			}
		}

		public static void restoreIndices(Uint32Array indices, int len)
		{
			var i = 3;
			if (len > 0)
			{
				indices[2] += indices[0];
			}
			for (; i < len; i += 3)
			{
				indices[i] += indices[i - 3];

				if (indices[i] == indices[i - 3])
				{
					indices[i + 1] += indices[i - 2];
				}
				else
				{
					indices[i + 1] += indices[i];
				}

				indices[i + 2] += indices[i];
			}
		}

		public static void restoreGridIndices(Uint32Array gridIndices, int len)
		{
			var i = 1;
			for (; i < len;++i)
			{
				gridIndices[i] += gridIndices[i - 1];
			}
		}

		public static void restoreVertices(Float32Array vertices, FileMG2Header grid, Uint32Array gridIndices, double precision)
		{
			var intVertices = new Uint32Array(vertices.buffer, vertices.byteOffset, vertices.length);
			double ydiv = grid.divx;
			var zdiv = ydiv * grid.divy;
			UInt32 prevGridIdx = 0x7fffffff;
			UInt32 prevDelta = 0;
			var i = 0;
			var j = 0;
			var len = gridIndices.length;

			for (; i < len; j += 3)
			{
				UInt32 gridIdx;
				var x = gridIdx = gridIndices[i++];

				var z = (uint)(x / zdiv);
				x = (uint)(x - (z * zdiv));
				var y = (uint)(x / ydiv);
				x = (uint)(x - (y * ydiv));

				var delta = intVertices[j];
				if (gridIdx == prevGridIdx)
				{
					delta += prevDelta;
				}

				vertices[j] = (float)(grid.lowerBoundx + x * grid.sizex + precision * delta);
				vertices[j + 1] = (float)(grid.lowerBoundy + y * grid.sizey + precision * intVertices[j + 1]);
				vertices[j + 2] = (float)(grid.lowerBoundz + z * grid.sizez + precision * intVertices[j + 2]);

				prevGridIdx = gridIdx;
				prevDelta = delta;
			}
		}

		public static void restoreNormals(Float32Array normals, Float32Array smooth, double precision)
		{
			var intNormals = new Uint32Array(normals.buffer, normals.byteOffset, normals.length);
			var i = 0;
			var k = normals.length;
			const double PI_DIV_2 = 3.141592653589793238462643 * 0.5;

			for (; i < k; i += 3)
			{
				var ro = intNormals[i] * precision;
				double phi = intNormals[i + 1];

				if (phi == 0)
				{
					normals[i] = (float)(smooth[i] * ro);
					normals[i + 1] = (float)(smooth[i + 1] * ro);
					normals[i + 2] = (float)(smooth[i + 2] * ro);
				}
				else
				{
					double theta;
					if (phi <= 4)
					{
						theta = (intNormals[i + 2] - 2) * PI_DIV_2;
					}
					else
					{
						theta = ((intNormals[i + 2] * 4 / phi) - 2) * PI_DIV_2;
					}

					phi *= precision * PI_DIV_2;
					var sinPhi = ro * System.Math.Sin(phi);

					var nx = sinPhi * System.Math.Cos(theta);
					var ny = sinPhi * System.Math.Sin(theta);
					var nz = ro * System.Math.Cos(phi);

					double bz = smooth[i + 1];
					double by = smooth[i] - smooth[i + 2];

					var len = System.Math.Sqrt(2 * bz * bz + by * by);
					if (len > 1e-20)
					{
						by /= len;
						bz /= len;
					}

					normals[i] = (float)(smooth[i] * nz + (smooth[i + 1] * bz - smooth[i + 2] * by) * ny - bz * nx);
					normals[i + 1] = (float)(smooth[i + 1] * nz - (smooth[i + 2] + smooth[i]) * bz * ny + by * nx);
					normals[i + 2] = (float)(smooth[i + 2] * nz + (smooth[i] * by + smooth[i + 1] * bz) * ny + bz * nx);
				}
			}
		}

		public static void restoreMap(Float32Array map, int count, double precision)
		{
			var intMap = new Uint32Array(map.buffer, map.byteOffset, map.length);
			var i = 0;
			var len = map.length;

			for (; i < count;++ i)
			{
				long delta = 0;

				int j;
				for (j = i; j < len; j += count)
				{
					var value = intMap[j];

					delta += (value & 1) != 0 ? -((value + 1) >> 1) : value >> 1;

					map[j] = (float)(delta * precision);
				}
			}
		}

		public static Float32Array calcSmoothNormals(Uint32Array indices, Float32Array vertices)
		{
			var smooth = new Float32Array(vertices.length);

			var k = indices.length;
			for (var i = 0; i < k;)
			{
				var indx = (int)(indices[i++] * 3);
				var indy = (int)(indices[i++] * 3);
				var indz = (int)(indices[i++] * 3);

				double v1x = vertices[indy] - vertices[indx];
				double v2x = vertices[indz] - vertices[indx];
				double v1y = vertices[indy + 1] - vertices[indx + 1];
				double v2y = vertices[indz + 1] - vertices[indx + 1];
				double v1z = vertices[indy + 2] - vertices[indx + 2];
				double v2z = vertices[indz + 2] - vertices[indx + 2];

				var nx = v1y * v2z - v1z * v2y;
				var ny = v1z * v2x - v1x * v2z;
				var nz = v1x * v2y - v1y * v2x;

				var len = System.Math.Sqrt(nx * nx + ny * ny + nz * nz);
				if (len > 1e-10)
				{
					nx /= len;
					ny /= len;
					nz /= len;
				}

				smooth[indx] = (float)(smooth[indx] + nx);
				smooth[indx + 1] = (float)(smooth[indx + 1] + ny);
				smooth[indx + 2] = (float)(smooth[indx + 2] + nz);
				smooth[indy] = (float)(smooth[indy] + nx);
				smooth[indy + 1] = (float)(smooth[indy + 1] + ny);
				smooth[indy + 2] = (float)(smooth[indy + 2] + nz);
				smooth[indz] = (float)(smooth[indz] + nx);
				smooth[indz + 1] = (float)(smooth[indz + 1] + ny);
				smooth[indz + 2] = (float)(smooth[indz + 2] + nz);
			}

			k = smooth.length;
			for (var i = 0; i < k; i += 3)
			{
				var len = System.Math.Sqrt(smooth[i] * smooth[i] + smooth[i + 1] * smooth[i + 1] + smooth[i + 2] * smooth[i + 2]);
				if (len > 1e-10)
				{
					smooth[i] = (float)(smooth[i] / len);
					smooth[i + 1] = (float)(smooth[i + 1] / len);
					smooth[i + 2] = (float)(smooth[i + 2] / len);
				}
			}

			return smooth;
		}
	}
}
