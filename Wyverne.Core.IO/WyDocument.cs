//
// WyResourceProperties.cs
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
namespace Wyverne.Core.IO
{
	/// <summary>
	/// Base class to be implemented by all Wyverne documents. A document is a single "file" as part of a larger 
	/// Wyverne project and can have its own persistant properties and resources.
	/// </summary>
	public abstract class WyDocument
	{
		/// <summary>
		/// Document metadata including the document type
		/// </summary>
		public WyDocMetadata Metadata { get; }

		/// <summary>
		/// Object containing persistant property data. Items held in here are easy-serialized. It is not recommended
		/// to store data involving complex serialization (such as Dictionaries) or requiring binary data (such as 
		/// ImageS) within the object's properties.
		/// </summary>
		object Properties { get; set; }

		/// <summary>
		/// Initializes a new instance of the <see cref="T:Wyverne.Core.IO.WyDocument"/> class.
		/// </summary>
		/// <param name="docId">Unique document identifier</param>
		/// <param name="name">Document name</param>
		public WyDocument(Guid docId, string name)
		{
			this.Metadata = new WyDocMetadata(docId, this.GetType());
			this.Properties = new object();
		}
	}
}
