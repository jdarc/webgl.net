using System;
using System.IO;

namespace THREE
{
	public class LZMA
	{
		private const uint NumStates = 12;
		private const int NumPosSlotBits = 6;
		private const uint NumLenToPosStates = 4;
		private const uint MatchMinLen = 2;
		private const int NumAlignBits = 4;
		private const uint StartPosModelIndex = 4;
		private const uint EndPosModelIndex = 14;
		private const uint NumFullDistances = 128;
		private const int NumPosStatesBitsMax = 4;
		private const uint NumPosStatesMax = 16;
		private const int NumLowLenBits = 3;
		private const int NumMidLenBits = 3;
		private const int NumHighLenBits = 8;
		private const uint NumLowLenSymbols = 8;
		private const uint NumMidLenSymbols = 8;

		private static uint getLenToPosState(uint len)
		{
			len -= MatchMinLen;
			if (len < NumLenToPosStates)
			{
				return len;
			}
			return (NumLenToPosStates - 1);
		}

		private struct State
		{
			public uint index;

			public void init()
			{
				index = 0;
			}

			public void updateChar()
			{
				if (index < 4)
				{
					index = 0;
				}
				else if (index < 10)
				{
					index -= 3;
				}
				else
				{
					index -= 6;
				}
			}

			public void updateMatch()
			{
				index = (uint)(index < 7 ? 7 : 10);
			}

			public void updateRep()
			{
				index = (uint)(index < 7 ? 8 : 11);
			}

			public void updateShortRep()
			{
				index = (uint)(index < 7 ? 9 : 11);
			}

			public bool isCharState()
			{
				return index < 7;
			}
		}

		internal class DataErrorException : ApplicationException
		{
			public DataErrorException() : base("Data Error")
			{
			}
		}

		internal class InvalidParamException : ApplicationException
		{
			public InvalidParamException() : base("Invalid Parameter")
			{
			}
		}

		private class OutWindow
		{
			private byte[] _buffer;
			private uint _pos;
			private uint _windowSize;
			private uint _streamPos;
			private Stream _stream;

			public void create(uint windowSize)
			{
				if (_windowSize != windowSize)
				{
					_buffer = new byte[windowSize];
				}
				_windowSize = windowSize;
				_pos = 0;
				_streamPos = 0;
			}

			public void init(Stream stream, bool solid)
			{
				releaseStream();
				_stream = stream;
				if (!solid)
				{
					_streamPos = 0;
					_pos = 0;
				}
			}

			public void releaseStream()
			{
				flush();
				_stream = null;
			}

			public void flush()
			{
				var size = _pos - _streamPos;
				if (size == 0)
				{
					return;
				}
				_stream.Write(_buffer, (int)_streamPos, (int)size);
				if (_pos >= _windowSize)
				{
					_pos = 0;
				}
				_streamPos = _pos;
			}

			public void copyBlock(uint distance, uint len)
			{
				var pos = _pos - distance - 1;
				if (pos >= _windowSize)
				{
					pos += _windowSize;
				}
				for (; len > 0; len--)
				{
					if (pos >= _windowSize)
					{
						pos = 0;
					}
					_buffer[_pos++] = _buffer[pos++];
					if (_pos >= _windowSize)
					{
						flush();
					}
				}
			}

			public void putByte(byte b)
			{
				_buffer[_pos++] = b;
				if (_pos >= _windowSize)
				{
					flush();
				}
			}

			public byte getByte(uint distance)
			{
				var pos = _pos - distance - 1;
				if (pos >= _windowSize)
				{
					pos += _windowSize;
				}
				return _buffer[pos];
			}
		}

		private class Decoder
		{
			public uint range;
			public uint code;
			public Stream stream;

			public void init(Stream s)
			{
				stream = s;

				code = 0;
				range = 0xFFFFFFFF;
				for (var i = 0; i < 5; i++)
				{
					code = (code << 8) | (byte)stream.ReadByte();
				}
			}

			public void releaseStream()
			{
				stream = null;
			}

			public uint decodeDirectBits(int numTotalBits)
			{
				uint result = 0;
				for (var i = numTotalBits; i > 0; i--)
				{
					range >>= 1;
					var t = (code - range) >> 31;
					code -= range & (t - 1);
					result = (result << 1) | (1 - t);

					if (range < 0x1000000)
					{
						code = (code << 8) | (byte)stream.ReadByte();
						range <<= 8;
					}
				}
				return result;
			}
		}

		private struct BitDecoder
		{
			private uint _prob;

			public void init()
			{
				_prob = 1024;
			}

			public uint decode(Decoder rangeDecoder)
			{
				var newBound = (rangeDecoder.range >> 11) * _prob;
				if (rangeDecoder.code < newBound)
				{
					rangeDecoder.range = newBound;
					_prob += (2048 - _prob) >> 5;
					if (rangeDecoder.range < 0x1000000)
					{
						rangeDecoder.code = (rangeDecoder.code << 8) | (byte)rangeDecoder.stream.ReadByte();
						rangeDecoder.range <<= 8;
					}
					return 0;
				}
				rangeDecoder.range -= newBound;
				rangeDecoder.code -= newBound;
				_prob -= (_prob) >> 5;
				if (rangeDecoder.range < 0x1000000)
				{
					rangeDecoder.code = (rangeDecoder.code << 8) | (byte)rangeDecoder.stream.ReadByte();
					rangeDecoder.range <<= 8;
				}
				return 1;
			}
		}

		private struct BitTreeDecoder
		{
			private readonly BitDecoder[] _odels;
			private readonly int _numBitLevels;

			public BitTreeDecoder(int numBitLevels)
			{
				_numBitLevels = numBitLevels;
				_odels = new BitDecoder[1 << numBitLevels];
			}

			public void init()
			{
				for (uint i = 1; i < (1 << _numBitLevels); i++)
				{
					_odels[i].init();
				}
			}

			public uint decode(Decoder rangeDecoder)
			{
				uint m = 1;
				for (var bitIndex = _numBitLevels; bitIndex > 0; bitIndex--)
				{
					m = (m << 1) + _odels[m].decode(rangeDecoder);
				}
				return m - ((uint)1 << _numBitLevels);
			}

			public uint reverseDecode(Decoder rangeDecoder)
			{
				uint m = 1;
				uint symbol = 0;
				for (var bitIndex = 0; bitIndex < _numBitLevels; bitIndex++)
				{
					var bit = _odels[m].decode(rangeDecoder);
					m <<= 1;
					m += bit;
					symbol |= (bit << bitIndex);
				}
				return symbol;
			}

			public static uint reverseDecode(BitDecoder[] models, UInt32 startIndex, Decoder rangeDecoder, int numBitLevels)
			{
				uint m = 1;
				uint symbol = 0;
				for (var bitIndex = 0; bitIndex < numBitLevels; bitIndex++)
				{
					var bit = models[startIndex + m].decode(rangeDecoder);
					m <<= 1;
					m += bit;
					symbol |= (bit << bitIndex);
				}
				return symbol;
			}
		}

		private class LenDecoder
		{
			private BitDecoder _choice;
			private BitDecoder _choice2;
			private readonly BitTreeDecoder[] _lowCoder = new BitTreeDecoder[NumPosStatesMax];
			private readonly BitTreeDecoder[] _midCoder = new BitTreeDecoder[NumPosStatesMax];
			private BitTreeDecoder _highCoder = new BitTreeDecoder(NumHighLenBits);
			private uint _numPosStates;

			public void create(uint numPosStates)
			{
				for (var posState = _numPosStates; posState < numPosStates; posState++)
				{
					_lowCoder[posState] = new BitTreeDecoder(NumLowLenBits);
					_midCoder[posState] = new BitTreeDecoder(NumMidLenBits);
				}
				_numPosStates = numPosStates;
			}

			public void init()
			{
				_choice = new BitDecoder();
				_choice.init();
				for (uint posState = 0; posState < _numPosStates; posState++)
				{
					_lowCoder[posState].init();
					_midCoder[posState].init();
				}
				_choice2 = new BitDecoder();
				_choice2.init();
				_highCoder.init();
			}

			public uint decode(Decoder rangeDecoder, uint posState)
			{
				if (_choice.decode(rangeDecoder) == 0)
				{
					return _lowCoder[posState].decode(rangeDecoder);
				}
				var symbol = NumLowLenSymbols;
				if (_choice2.decode(rangeDecoder) == 0)
				{
					symbol += _midCoder[posState].decode(rangeDecoder);
				}
				else
				{
					symbol += NumMidLenSymbols;
					symbol += _highCoder.decode(rangeDecoder);
				}
				return symbol;
			}
		}

		private class LiteralDecoder
		{
			private struct Decoder2
			{
				private BitDecoder[] _decoders;

				public void create()
				{
					_decoders = new BitDecoder[0x300];
				}

				public void init()
				{
					for (var i = 0; i < 0x300; i++)
					{
						_decoders[i].init();
					}
				}

				public byte decodeNormal(Decoder rangeDecoder)
				{
					uint symbol = 1;
					do symbol = (symbol << 1) | _decoders[symbol].decode(rangeDecoder); while (symbol < 0x100);
					return (byte)symbol;
				}

				public byte decodeWithMatchByte(Decoder rangeDecoder, byte matchByte)
				{
					uint symbol = 1;
					do
					{
						var matchBit = (uint)(matchByte >> 7) & 1;
						matchByte <<= 1;
						var bit = _decoders[((1 + matchBit) << 8) + symbol].decode(rangeDecoder);
						symbol = (symbol << 1) | bit;
						if (matchBit != bit)
						{
							while (symbol < 0x100)
							{
								symbol = (symbol << 1) | _decoders[symbol].decode(rangeDecoder);
							}
							break;
						}
					} while (symbol < 0x100);
					return (byte)symbol;
				}
			}

			private Decoder2[] _coders;
			private int _numPrevBits;
			private int _numPosBits;
			private uint _posMask;

			public void create(int numPosBits, int numPrevBits)
			{
				if (_coders != null && _numPrevBits == numPrevBits &&
				    _numPosBits == numPosBits)
				{
					return;
				}
				_numPosBits = numPosBits;
				_posMask = ((uint)1 << numPosBits) - 1;
				_numPrevBits = numPrevBits;
				var numStates = (uint)1 << (_numPrevBits + _numPosBits);
				_coders = new Decoder2[numStates];
				for (uint i = 0; i < numStates; i++)
				{
					_coders[i].create();
				}
			}

			public void init()
			{
				var numStates = (uint)1 << (_numPrevBits + _numPosBits);
				for (uint i = 0; i < numStates; i++)
				{
					_coders[i].init();
				}
			}

			private uint getState(uint pos, byte prevByte)
			{
				return ((pos & _posMask) << _numPrevBits) + (uint)(prevByte >> (8 - _numPrevBits));
			}

			public byte decodeNormal(Decoder rangeDecoder, uint pos, byte prevByte)
			{
				return _coders[getState(pos, prevByte)].decodeNormal(rangeDecoder);
			}

			public byte decodeWithMatchByte(Decoder rangeDecoder, uint pos, byte prevByte, byte matchByte)
			{
				return _coders[getState(pos, prevByte)].decodeWithMatchByte(rangeDecoder, matchByte);
			}
		} ;

		private readonly OutWindow _outWindow = new OutWindow();
		private readonly Decoder _rangeDecoder = new Decoder();

		private readonly BitDecoder[] _isMatchDecoders = new BitDecoder[NumStates << NumPosStatesBitsMax];
		private readonly BitDecoder[] _isRepDecoders = new BitDecoder[NumStates];
		private readonly BitDecoder[] _isRepG0Decoders = new BitDecoder[NumStates];
		private readonly BitDecoder[] _isRepG1Decoders = new BitDecoder[NumStates];
		private readonly BitDecoder[] _isRepG2Decoders = new BitDecoder[NumStates];
		private readonly BitDecoder[] _isRep0LongDecoders = new BitDecoder[NumStates << NumPosStatesBitsMax];

		private readonly BitTreeDecoder[] _posSlotDecoder = new BitTreeDecoder[NumLenToPosStates];
		private readonly BitDecoder[] _posDecoders = new BitDecoder[NumFullDistances - EndPosModelIndex];

		private BitTreeDecoder _posAlignDecoder = new BitTreeDecoder(NumAlignBits);

		private readonly LenDecoder _lenDecoder = new LenDecoder();
		private readonly LenDecoder _repLenDecoder = new LenDecoder();

		private readonly LiteralDecoder _literalDecoder = new LiteralDecoder();

		private uint _dictionarySize;
		private uint _dictionarySizeCheck;

		private uint _posStateMask;

		private readonly bool _solid;

		private LZMA()
		{
			_solid = false;
			_dictionarySize = 0xFFFFFFFF;
			for (var i = 0; i < NumLenToPosStates; i++)
			{
				_posSlotDecoder[i] = new BitTreeDecoder(NumPosSlotBits);
			}
		}

		private void setDictionarySize(uint dictionarySize)
		{
			if (_dictionarySize != dictionarySize)
			{
				_dictionarySize = dictionarySize;
				_dictionarySizeCheck = (uint)System.Math.Max(_dictionarySize, 1);
				uint blockSize = (uint)System.Math.Max(_dictionarySizeCheck, (1 << 12));
				_outWindow.create(blockSize);
			}
		}

		private void setLiteralProperties(int lp, int lc)
		{
			if (lp > 8)
			{
				throw new InvalidParamException();
			}
			if (lc > 8)
			{
				throw new InvalidParamException();
			}
			_literalDecoder.create(lp, lc);
		}

		private void setPosBitsProperties(int pb)
		{
			if (pb > NumPosStatesBitsMax)
			{
				throw new InvalidParamException();
			}
			var numPosStates = (uint)1 << pb;
			_lenDecoder.create(numPosStates);
			_repLenDecoder.create(numPosStates);
			_posStateMask = numPosStates - 1;
		}

		private void init(Stream inStream, Stream outStream)
		{
			_rangeDecoder.init(inStream);
			_outWindow.init(outStream, _solid);

			uint i;
			for (i = 0; i < NumStates; i++)
			{
				for (uint j = 0; j <= _posStateMask; j++)
				{
					var index = (i << NumPosStatesBitsMax) + j;
					_isMatchDecoders[index].init();
					_isRep0LongDecoders[index].init();
				}
				_isRepDecoders[i].init();
				_isRepG0Decoders[i].init();
				_isRepG1Decoders[i].init();
				_isRepG2Decoders[i].init();
			}

			_literalDecoder.init();
			for (i = 0; i < NumLenToPosStates; i++)
			{
				_posSlotDecoder[i].init();
			}

			for (i = 0; i < NumFullDistances - EndPosModelIndex; i++)
			{
				_posDecoders[i].init();
			}

			_lenDecoder.init();
			_repLenDecoder.init();
			_posAlignDecoder.init();
		}

		private void decode(Stream inStream, Stream outStream, Int64 outSize)
		{
			init(inStream, outStream);

			var state = new State();
			state.init();
			uint rep0 = 0, rep1 = 0, rep2 = 0, rep3 = 0;

			UInt64 nowPos64 = 0;
			var outSize64 = (UInt64)outSize;
			if (nowPos64 < outSize64)
			{
				if (_isMatchDecoders[state.index << NumPosStatesBitsMax].decode(_rangeDecoder) != 0)
				{
					throw new DataErrorException();
				}
				state.updateChar();
				var b = _literalDecoder.decodeNormal(_rangeDecoder, 0, 0);
				_outWindow.putByte(b);
				nowPos64++;
			}
			while (nowPos64 < outSize64)
			{
				var posState = (uint)nowPos64 & _posStateMask;
				if (_isMatchDecoders[(state.index << NumPosStatesBitsMax) + posState].decode(_rangeDecoder) == 0)
				{
					byte b;
					var prevByte = _outWindow.getByte(0);
					if (!state.isCharState())
					{
						b = _literalDecoder.decodeWithMatchByte(_rangeDecoder, (uint)nowPos64, prevByte, _outWindow.getByte(rep0));
					}
					else
					{
						b = _literalDecoder.decodeNormal(_rangeDecoder, (uint)nowPos64, prevByte);
					}
					_outWindow.putByte(b);
					state.updateChar();
					nowPos64++;
				}
				else
				{
					uint len;
					if (_isRepDecoders[state.index].decode(_rangeDecoder) == 1)
					{
						if (_isRepG0Decoders[state.index].decode(_rangeDecoder) == 0)
						{
							if (_isRep0LongDecoders[(state.index << NumPosStatesBitsMax) + posState].decode(_rangeDecoder) == 0)
							{
								state.updateShortRep();
								_outWindow.putByte(_outWindow.getByte(rep0));
								nowPos64++;
								continue;
							}
						}
						else
						{
							UInt32 distance;
							if (_isRepG1Decoders[state.index].decode(_rangeDecoder) == 0)
							{
								distance = rep1;
							}
							else
							{
								if (_isRepG2Decoders[state.index].decode(_rangeDecoder) == 0)
								{
									distance = rep2;
								}
								else
								{
									distance = rep3;
									rep3 = rep2;
								}
								rep2 = rep1;
							}
							rep1 = rep0;
							rep0 = distance;
						}
						len = _repLenDecoder.decode(_rangeDecoder, posState) + MatchMinLen;
						state.updateRep();
					}
					else
					{
						rep3 = rep2;
						rep2 = rep1;
						rep1 = rep0;
						len = MatchMinLen + _lenDecoder.decode(_rangeDecoder, posState);
						state.updateMatch();
						var posSlot = _posSlotDecoder[getLenToPosState(len)].decode(_rangeDecoder);
						if (posSlot >= StartPosModelIndex)
						{
							var numDirectBits = (int)((posSlot >> 1) - 1);
							rep0 = ((2 | (posSlot & 1)) << numDirectBits);
							if (posSlot < EndPosModelIndex)
							{
								rep0 += BitTreeDecoder.reverseDecode(_posDecoders, rep0 - posSlot - 1, _rangeDecoder, numDirectBits);
							}
							else
							{
								rep0 += (_rangeDecoder.decodeDirectBits(numDirectBits - NumAlignBits) << NumAlignBits);
								rep0 += _posAlignDecoder.reverseDecode(_rangeDecoder);
							}
						}
						else
						{
							rep0 = posSlot;
						}
					}
					if (rep0 >= nowPos64 || rep0 >= _dictionarySizeCheck)
					{
						if (rep0 == 0xFFFFFFFF)
						{
							break;
						}
						throw new DataErrorException();
					}
					_outWindow.copyBlock(rep0, len);
					nowPos64 += len;
				}
			}
			_outWindow.flush();
			_outWindow.releaseStream();
			_rangeDecoder.releaseStream();
		}

		private void setDecoderProperties(byte[] properties)
		{
			if (properties.Length < 5)
			{
				throw new InvalidParamException();
			}
			var lc = properties[0] % 9;
			var remainder = properties[0] / 9;
			var lp = remainder % 5;
			var pb = remainder / 5;
			if (pb > NumPosStatesBitsMax)
			{
				throw new InvalidParamException();
			}
			UInt32 dictionarySize = 0;
			for (var i = 0; i < 4; i++)
			{
				dictionarySize += ((UInt32)(properties[1 + i])) << (i * 8);
			}
			setDictionarySize(dictionarySize);
			setLiteralProperties(lp, lc);
			setPosBitsProperties(pb);
		}

		private class MyInterleavedStream : Stream
		{
			private readonly CTM.InterleavedStream _stream;

			public MyInterleavedStream(CTM.InterleavedStream stream)
			{
				_stream = stream;
			}

			public override void Flush()
			{
				throw new NotImplementedException();
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotImplementedException();
			}

			public override void SetLength(long value)
			{
				throw new NotImplementedException();
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				throw new NotImplementedException();
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				for (var i = 0; i < count; i++)
				{
					_stream.writeByte(buffer[offset + i]);
				}
			}

			public override bool CanRead
			{
				get { throw new NotImplementedException(); }
			}
			public override bool CanSeek
			{
				get { throw new NotImplementedException(); }
			}
			public override bool CanWrite
			{
				get { throw new NotImplementedException(); }
			}
			public override long Length
			{
				get { throw new NotImplementedException(); }
			}
			public override long Position
			{
				get { throw new NotImplementedException(); }
				set { throw new NotImplementedException(); }
			}
		}

		private class MyStream : Stream
		{
			private readonly CTM.Stream _stream;

			public MyStream(CTM.Stream stream)
			{
				_stream = stream;
			}

			public override void Flush()
			{
				throw new NotImplementedException();
			}

			public override long Seek(long offset, SeekOrigin origin)
			{
				throw new NotImplementedException();
			}

			public override void SetLength(long value)
			{
				throw new NotImplementedException();
			}

			public override int Read(byte[] buffer, int offset, int count)
			{
				for (var i = 0; i < count; i++)
				{
					buffer[offset + i] = _stream.readByte();
				}
				return count;
			}

			public override void Write(byte[] buffer, int offset, int count)
			{
				throw new NotImplementedException();
			}

			public override bool CanRead
			{
				get { return true; }
			}
			public override bool CanSeek
			{
				get { return true; }
			}
			public override bool CanWrite
			{
				get { return false; }
			}
			public override long Length
			{
				get { return _stream.data.Length; }
			}
			public override long Position
			{
				get { return _stream.offset; }
				set { _stream.offset = (int)value; }
			}
		}

		public static bool decompress(CTM.Stream properties, CTM.Stream inStream, CTM.InterleavedStream outStream, int outSize)
		{
			var decoder = new LZMA();

			decoder.setDecoderProperties(new[]
			{
				properties.readByte(),
				properties.readByte(),
				properties.readByte(),
				properties.readByte(),
				properties.readByte()
			});

			decoder.decode(new MyStream(inStream), new MyInterleavedStream(outStream), outSize);
			return true;
		}
	}
}
