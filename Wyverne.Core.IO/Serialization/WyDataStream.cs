//
// WyDataStream.cs
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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Wyverne.Core.IO.Serialization
{
	/// <summary>
	/// A stream used for reading chains of WyDataChunks by buffering them one at a time. Any disposed data chunks are skipped.
	/// </summary>
	public class WyDataStream : Stream, IDisposable
	{
		public override bool CanRead { get { return _currentChunk < _chunks.Length; } }

		public override long Length { get; }

		WyDataChunk[] _chunks;
		int _currentChunk = 0;
		long _readIndex = 0;
		long _bytesRead = 0;

		object _readLock = new object();

		public WyDataStream(WyDataChunk first)
		{
			this._chunks = first?.GetChain().ToArray() ?? new WyDataChunk[0];
			this.Length = _chunks.Where(c => !c.Disposed).Sum(c => c.Length);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (buffer == null) throw new ArgumentNullException();
			if (offset < 0 || offset >= buffer.Length) throw new IndexOutOfRangeException();

			lock (_readLock) {
				// Number of bytes read by this
				var totalReadBytes = 0;

				// Where to write to within buffer
				var writeIndex = offset;

				// Cycle
				while (writeIndex < buffer.Length && _currentChunk < _chunks.Length) {
					// The current read chunk
					var chunk = _chunks[_currentChunk];

					Console.WriteLine(chunk.Length);

					// Skip disposed chunks
					if (chunk.Length <= 0 || _readIndex > chunk.Length) {
						_currentChunk++;
						_readIndex = 0;
						continue;
					}

					// The number of bytes we can read this pass
					var readBytes = (int)Math.Min(buffer.Length - writeIndex, chunk.Length - _readIndex);

					// And copy
					Array.Copy(chunk.GetInternal(), _readIndex, buffer, writeIndex, readBytes);

					// Add to variables
					totalReadBytes += readBytes;
					writeIndex += readBytes;

					// Advance and reset
					_currentChunk++;
					_readIndex = 0;
				}

				return totalReadBytes;
			}
		}

		#region Stream Support
		public override void Flush()
		{ throw new NotSupportedException(); }

		public override long Seek(long offset, SeekOrigin origin)
		{ throw new NotSupportedException(); }

		public override void SetLength(long value)
		{ throw new NotSupportedException(); }

		public override void Write(byte[] buffer, int offset, int count)
		{ throw new NotSupportedException(); }

		public override bool CanSeek => false;
		public override bool CanWrite => false;
		public override long Position { get { return this._bytesRead; } set { throw new NotSupportedException(); } }

		#region IDisposable Support
		bool disposedValue = false; // To detect redundant calls

		protected override void Dispose(bool disposing)
		{
			base.Dispose(disposing);
			if (!disposedValue) {
				if (disposing) {
					foreach (var chunk in this._chunks) {
						chunk.Dispose();
					}
				}

				_chunks = null;

				disposedValue = true;
			}
		}

		public new void Dispose()
		{
			Dispose(true);
		}
		#endregion
		#endregion
	}
}
