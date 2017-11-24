//
// WyResourceDictionary.cs
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

namespace Wyverne.Core.IO.Serialization
{
	public class WyResourceDictionary : IWyResource
	{
		private IDictionary<string, WyDataChunk> _resources;

		public WyResourceDictionary() : this(new WySerializationContext[0]) { }

		public WyResourceDictionary(IEnumerable<WySerializationContext> initializeFrom)
		{
			this._resources = new Dictionary<string, WyDataChunk>();

			foreach (var resource in initializeFrom) {
				var type = resource.Type;
				var id = resource.Id;
				var data = resource.Data;

				if (data == null || data.Disposed) throw new ObjectDisposedException(nameof(data));
				if (_resources.ContainsKey(id.ToLower())) throw new ArgumentException($"A resource with the key '{id}' already exists");

			}
		}

		/// <summary>
		/// Adds a resource to the dictionary
		/// </summary>
		public void AddResource(WySerializationContext context)
		{ AddResource(context.Id, context.Data); }

		/// <summary>
		/// Adds a resource to the dictionary
		/// </summary>
		public void AddResource(string resourceId, WyDataChunk resourceData)
		{
			if (resourceData == null || resourceData.Disposed) throw new ObjectDisposedException(nameof(resourceData));
			if (String.IsNullOrWhiteSpace(resourceId)) throw new InvalidResourceIdException();

			_resources.Add(resourceId.Trim().ToLower(), resourceData);
		}

		/// <summary>
		/// Removes a resource from the dictionary
		/// </summary>
		/// <param name="resourceId">Resource identifier.</param>
		public void RemoveResource(string resourceId)
		{
			if (String.IsNullOrWhiteSpace(resourceId)) throw new InvalidResourceIdException();

			_resources.Remove(resourceId.Trim().ToLower());
		}

		/// <summary>
		/// Returns a stream of bytes for the selected stream
		/// </summary>
		/// <returns>The resource.</returns>
		public Stream GetResource(string resourceId)
		{
			if (String.IsNullOrWhiteSpace(resourceId)) throw new InvalidResourceIdException();

			var data = _resources[resourceId.Trim().ToLower()];
			return new MemoryStream(data.GetInternal());
		}

		public WySerializationContext Serialize()
		{
			// Collect all the datas from the resources
			return default(WySerializationContext);

		}
	}


}
