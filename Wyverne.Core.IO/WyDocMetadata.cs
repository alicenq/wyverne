//
// WyDocMetadata.cs
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
	/// Metadata associated with a Wyverne document. This is the only document data loaded when teh project file is initially opened.
	/// </summary>
	public class WyDocMetadata
	{
		/// <summary>
		/// A unique identifier for the document within the context of its parent project
		/// </summary>
		/// <value>The identifier.</value>
		public Guid Id { get; }

		/// <summary>
		/// The name of the project
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// An optional description for the project
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// The document type this metadata is for. This Type is guaranteed to extend Wyverne.Core.IO.WyDocument.
		/// </summary>
		public Type type { get; }

		public WyDocMetadata(Guid docId, Type docType)
		{
			if (!docType.IsSubclassOf(typeof(WyDocument))))
			{ throw new InvalidCastException("Document type must extend Wyverne.Core.IO.WyDocument"); }

			this.Id = docId;
			this.type = docType;
		}
	}
}
