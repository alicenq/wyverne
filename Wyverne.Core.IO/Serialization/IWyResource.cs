//
// IWyResource.cs
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
namespace Wyverne.Core.IO.Serialization
{
	/// <summary>
	/// Interface implemented by all serialized WyResources
	/// </summary>
	public interface IWyResource
	{
		WySerializationContext Serialize();
	}

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
			if (resourceType == null || !resourceType.IsInstanceOfType(typeof(IWyResource))) throw new InvalidCastException("Resource type must be subclass of IWyResource");
			if (data == null || data.Disposed) throw new ObjectDisposedException(nameof(data));

			this.Id = resourceId.Trim();
			this.Type = resourceType;
			this.Data = data;
		}
	}
}
