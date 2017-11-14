//
// WyDataChunk.cs
//
// Author:
//       Alice Quiros <email@aliceq.me>
//
// Copyright (c) 2017 2017 Alice N. Quiros
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Wyverne.Core.IO.Serialization
{
	/// <summary>
	/// Class representing an array of byte data which implements the disposable interface which also allows 
	/// single-linkedlist style linking of other data chunks
	/// </summary>
	public class WyDataChunk : IDisposable
	{

		/// <summary>
		/// Returns the number of bytes in the buffer. If this has been disposed then -1 is returned.
		/// </summary>
		public long Length => _buffer?.Length ?? -1;

		/// <summary>
		/// The next data chunk in the series
		/// </summary>
		/// <value>The next.</value>
		public WyDataChunk Next { get; set; }

		object _opLock;
		byte[] _buffer;

		public WyDataChunk() : this(null) { }

		private WyDataChunk(byte[] fromBuffer)
		{
			this._opLock = new object();
			this._buffer = fromBuffer ?? new byte[0];
		}

		/// <summary>
		/// Returns the UTF8 encoded string representation of the data
		/// </summary>
		public string ToUTF8String()
		{ return new string(Encoding.UTF8.GetChars(_buffer)); }

		/// <summary>
		/// Returns the ASCII encoded string representation of the data
		/// </summary>
		public string ToASCIIString()
		{ return new string(Encoding.ASCII.GetChars(_buffer)); }

		public override string ToString()
		{ return Length >= 0 ? "{ Disposed }" : $"{{ Bytes: {Length} }}"; }

		/// <summary>
		/// Reads the byte at the given offset
		/// </summary>
		public byte ReadAt(int offset)
		{ return ReadAt((long)offset); }

		/// <summary>
		/// Reads the byte at the given offset
		/// </summary>
		public byte ReadAt(long offset)
		{
			if (_buffer == null) throw new ObjectDisposedException("WyDataChunk Buffer");
			return _buffer[offset];
		}

		/// <summary>
		/// Reads the byte at the given offset and returns true on success
		/// </summary>
		public bool TryReadAt(int offset, out byte data)
		{ return TryReadAt((long)offset, out data); }

		/// <summary>
		/// Reads the byte at the given offset and returns true on success
		/// </summary>
		public bool TryReadAt(long offset, out byte data)
		{
			lock (_opLock) {
				if (_buffer == null || offset < 0 || offset >= _buffer.Length) {
					data = 0;
					return false;
				} else {
					data = _buffer[offset];
					return true;
				}
			}
		}

		/// <summary>
		/// Reads a series of bytes beginning at a given offset and returns a byte array. If more data is requested 
		/// than is possible then the returned array will be smaller than requested.
		public byte[] ReadAt(int offset, int count)
		{ return ReadAt((long)offset, count); }

		/// <summary>
		/// Reads a series of bytes beginning at a given offset and returns a byte array. If more data is requested 
		/// than is possible then the returned array will be smaller than requested.
		public byte[] ReadAt(long offset, int count)
		{
			lock (_opLock) {
				if (_buffer == null) throw new ObjectDisposedException("WyDataChunk Buffer");
				if (count <= 0) return new byte[0];

				var data = new byte[Math.Min(_buffer.Length, offset + count) - offset];
				Array.Copy(_buffer, offset, data, 0, data.Length);
				return data;
			}
		}

		///<summary>
		/// Reads a number of bytes into an existing array
		/// </summary>
		/// <returns>The number of bytes copied</returns>
		/// <param name="destinationArray">The destination array</param>
		/// <param name="destinationIndex">The index to begin writing to</param>
		/// <param name="count">The number of bytes desired</param>
		public int Read(byte[] destinationArray, int destinationIndex, int count)
		{ return Read((long)0, destinationArray, destinationIndex, count); }

		/// <summary>
		/// Reads a number of bytes into an existing array
		/// </summary>
		/// <returns>The number of bytes copied</returns>
		/// <param name="sourceIndex">The index to begin copying from</param>
		/// <param name="destinationArray">The destination array</param>
		/// <param name="destinationIndex">The index to begin writing to</param>
		/// <param name="count">The number of bytes desired</param>
		public int Read(int sourceIndex, byte[] destinationArray, int destinationIndex, int count)
		{ return Read((long)sourceIndex, destinationArray, destinationIndex, count); }

		/// <summary>
		/// Reads a number of bytes into an existing array
		/// </summary>
		/// <returns>The number of bytes copied</returns>
		/// <param name="sourceIndex">The index to begin copying from</param>
		/// <param name="destinationArray">The destination array</param>
		/// <param name="destinationIndex">The index to begin writing to</param>
		/// <param name="count">The number of bytes desired</param>
		public int Read(long sourceIndex, byte[] destinationArray, int destinationIndex, int count)
		{
			if (_buffer == null) throw new ObjectDisposedException("WyDataChunk Buffer");

			lock (_opLock) {
				if (destinationArray == null
					|| sourceIndex < 0
					|| sourceIndex >= _buffer.Length
					|| destinationIndex < 0
					|| destinationIndex >= destinationArray.Length
					|| count <= 0)
					return 0;

				var readCount = Math.Min(_buffer.Length, sourceIndex + count) - sourceIndex;
				var writeCount = Math.Min(destinationArray.Length, destinationIndex + count) - destinationIndex;
				var minCount = (int)Math.Min(readCount, writeCount);

				Array.Copy(_buffer, sourceIndex, destinationArray, destinationIndex, minCount);

				return minCount;
			}
		}

		/// <summary>
		/// Returns a series of chained WyDataChunks by following the <i>Next</i> property until null is reached or
		/// a circular chain is detected
		/// </summary>
		public IEnumerable<WyDataChunk> GetChain()
		{
			var chunks = new HashSet<WyDataChunk>();
			var chain = new List<WyDataChunk>();

			var node = this;
			while (node != null && !chunks.Contains(node)) {
				chain.Add(node);
				chunks.Add(node);
			}

			return chain.AsEnumerable();
		}

		/// <summary>
		/// Creates a data chunk from an array of bytes
		/// </summary>
		public static WyDataChunk FromBytes(byte[] bytes)
		{ return new WyDataChunk(bytes); }

		/// <summary>
		/// Creates a data chunk from a string using a specified encoding
		/// </summary>
		public static WyDataChunk FromString(string str, Encoding encoding)
		{ return new WyDataChunk(encoding.GetBytes(str)); }

		/// <summary>
		/// Creates a data chunk from a string using UTF8 encoding
		/// </summary>
		public static WyDataChunk FromString(string str)
		{ return FromString(str, Encoding.UTF8); }

		/// <summary>
		/// Creates a data chunk from a stream's entire contents
		/// </summary>
		public static WyDataChunk FromStream(Stream stream)
		{ return FromStream(stream, stream.Length); }

		/// <summary>
		/// Creates a data chunk from a stream
		/// </summary>
		public static WyDataChunk FromStream(Stream stream, long count)
		{
			if (stream.CanRead == false) throw new ArgumentException("Stream must have read capabilities");
			var buffer = new byte[Math.Max(count, 0)];
			stream.Read(buffer, 0, buffer.Length);
			return new WyDataChunk(buffer);
		}

		public void Dispose()
		{
			if (_buffer != null) {
				lock (_opLock) {
					var gen = GC.GetGeneration(this._buffer);

					this._buffer = null;

					GC.Collect(gen, GCCollectionMode.Default);
				}
			}
		}
	}
}
