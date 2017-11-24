//
// WySerializationContext.cs
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
using System.IO;
using System.Text;

namespace Wyverne.Core.IO.Serialization
{
	/// <summary>
	/// Serialization context for a serialized resource containing its type, id and data
	/// </summary>
	public struct WySerializationContext
	{
		public string Id { get; }
		public Type Type { get; }
		public WyDataChunk Data { get; }

		public long Length { get { return Data.Length; } }

		public WySerializationContext(string resourceId, Type resourceType, WyDataChunk data)
		{
			if (String.IsNullOrWhiteSpace(resourceId)) throw new InvalidResourceIdException();
			if (resourceType == null || !typeof(IWyResource).IsAssignableFrom(resourceType) || resourceType.IsInterface) 
				throw new InvalidCastException($"Resource type must be a subclass of IWyResource");
			if (data == null || data.Disposed) throw new ObjectDisposedException(nameof(data));

			this.Id = resourceId.Trim();
			this.Type = resourceType;
			this.Data = data;
		}

		/// <summary>
		/// Returns the data header as a chunk of data
		/// </summary>
		public WyDataChunk GetHeader()
		{
			if (Data == null || Data.Disposed)
				throw new ObjectDisposedException(nameof(Data));

			var dataBytes = Data.GetInternal();
			var typeBytes = Encoding.UTF8.GetBytes(Type.AssemblyQualifiedName);
			var idbytes = Encoding.UTF8.GetBytes(Id);

			// Header contains the following data
			// Data Length (long : 8)
			// Id Length (int : 4)
			// Type Length (int : 4)
			// Id (byte[])
			// Type (byte[])

			var buff = new byte[16 + typeBytes.Length + idbytes.Length];

			Array.Copy(BitConverter.GetBytes(dataBytes.LongLength), 0, buff, 0, 8);
			Array.Copy(BitConverter.GetBytes(idbytes.Length), 0, buff, 8, 4);
			Array.Copy(BitConverter.GetBytes(typeBytes.Length), 0, buff, 12, 4);
			Array.Copy(idbytes, 0, buff, 16, idbytes.Length);
			Array.Copy(typeBytes, 0, buff, 16 + idbytes.Length, typeBytes.Length);

			var dc = WyDataChunk.FromBytes(buff);
			Data.Follows(dc);
			return dc;
		}

		/// <summary>
		/// Creates a serialization context from a stream of bytes. The stream must be formatted in the following 
		/// format: [data length: 8][id length; 4][type length: 4][id][type][data]
		/// </summary>
		/// <returns>The buffer.</returns>
		/// <param name="buffer">Buffer.</param>
		public static WySerializationContext ParseStream(Stream stream)
		{
			if (stream == null || stream.Length <= 16 || !stream.CanRead)
				throw new ArgumentException("Invalid stream");
			var buff = (byte[])null;
			var strLen = stream.Length;

			// Read long
			buff = new byte[16];
			stream.Read(buff, 0, 16);
			var dataLength = BitConverter.ToInt64(buff, 0);
			var idLength = BitConverter.ToInt32(buff, 8);
			var typeLength = BitConverter.ToInt32(buff, 12);

			if (dataLength + idLength + typeLength + 16 != strLen)
				throw new FormatException();

			// Read id
			buff = new byte[idLength];
			stream.Read(buff, 0, idLength);
			var id = new String(Encoding.UTF8.GetChars(buff));

			// Read type
			buff = new byte[typeLength];
			stream.Read(buff, 0, typeLength);
			var type = new String(Encoding.UTF8.GetChars(buff));

			// Read data
			buff = new byte[dataLength];
			var index = 0;
			while (index < buff.Length) {
				var read = stream.Read(buff, index, (int)Math.Min(buff.LongLength - index, int.MaxValue));
				index += read;
				if (read == 0) break;
			}

			return new WySerializationContext(id, Type.GetType(type), WyDataChunk.FromBytes(buff));
		}
	}
}
