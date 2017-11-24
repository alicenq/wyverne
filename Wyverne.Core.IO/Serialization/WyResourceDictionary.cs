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
using System.Linq;

namespace Wyverne.Core.IO.Serialization
{
	public class WyResourceDictionary : IWyResource
	{
		private IDictionary<string, IWyResource> _resources;

		public WyResourceDictionary()
		{
			this._resources = new Dictionary<string, IWyResource>();
		}

		/// <summary>
		/// Saves an existing resource to the dictionary without serializing it. If a resource already exists
		/// with the same key an exception is thrown.
		/// </summary>
		public void AddResource(string resourceId, IWyResource resource)
		{
			if (resource == null) throw new ArgumentNullException();
			var key = WyResource.StrToId(resourceId);

			if (this._resources.ContainsKey(key)) throw new ArgumentException($"A resource with the key '{key}' already exists");

			this._resources[key] = resource;
		}

		/// <summary>
		/// Creates a new resource from a serialization context by calling its Constructor taking one parameter of
		/// type byte[], and adds it to the dictionary. If a resource already exists with the same key an exception is thrown.
		/// </summary>
		public IWyResource LoadResource(WySerializationContext context)
		{
			// Disposed check
			if (context.Data == null || context.Data.Disposed)
				throw new ObjectDisposedException(nameof(context.Data));

			// Get the data we need
			var key = WyResource.StrToId(context.Id);
			var type = context.Type;
			var res = (IWyResource)null;

			if (this._resources.ContainsKey(key)) throw new ArgumentException($"A resource with the key '{key}' already exists");

			// Attempt to create a new instance
			using (context.Data) {
				// Get the constructor
				var ctor = type.GetConstructor(new[] { typeof(byte[]) });
				if (ctor == null) throw new System.Reflection.TargetException($"Target type '{type}' must contain a constructor which accepts a single argument of type byte[]");

				// Load data and create new resource
				var arg = context.Data.GetInternal();
				res = (IWyResource)ctor.Invoke(new[] { arg ?? new byte[0] });
			}

			// Save it
			if (res != null) {
				this._resources[key] = res;
			}

			return res;
		}

		/// <summary>
		/// Removes a resource from the dictionary
		/// </summary>
		/// <param name="resourceId">Resource identifier.</param>
		public void RemoveResource(string resourceId)
		{
			var key = WyResource.StrToId(resourceId);
			_resources.Remove(key);
		}

		/// <summary>
		/// Retrieves a resource by id or null if it doesn't exist
		/// </summary>
		/// <returns>The resource.</returns>
		public IWyResource GetResource(string resourceId)
		{
			var key = WyResource.StrToId(resourceId);
			if (_resources.ContainsKey(key)) return _resources[key];
			else return null;
		}

		/// <summary>
		/// Retrieves an array of all the resources of a given type
		/// </summary>
		public T[] GetResources<T>() where T : IWyResource
		{ return _resources.Values.Where(r => r.GetType() == typeof(T)).Cast<T>().ToArray(); }

		/// <summary>
		/// Retrieves an array of all resources
		/// </summary>
		/// <returns>The resources.</returns>
		public IWyResource[] GetResources()
		{ return _resources.Values.ToArray();}

		public WySerializationContext Serialize()
		{
			// TODO write this
			return default(WySerializationContext);

		}
	}


}
