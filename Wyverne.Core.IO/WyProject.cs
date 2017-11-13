//
// WyProject.cs
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

namespace Wyverne.Core.IO
{
	/// <summary>
	/// A Wyverne project containing all related documents and metadata. This is the topmost hierarchical item.
	/// </summary>
	public class WyProject
	{
		/// <summary>
		/// The name of the project
		/// </summary>
		public string Name { get; set; }

		/// <summary>
		/// An optional description for the project
		/// </summary>
		public string Description { get; set; }

		/// <summary>
		/// Dictionary of the document metadatas stored within the file. In order to load a file's contents the 
		/// Load(Guid) method must be used.
		/// </summary>
		public IDictionary<Guid, WyDocument> Documents { get; }

		public WyProject(string name)
		{
			this.Name = name;
			this.Description = "";
			this.Documents = new Dictionary<Guid, WyDocument>();
		}

		/// <summary>
		/// Disposes of all documents and sets the name and description to null. Any documents which implement the 
		/// IDisposable interface will have its Dispose method invoked.
		/// </summary>
		public void Close()
		{
			foreach (var doc in Documents.Values) {
				if (doc is IDisposable) { ((IDisposable)doc).Dispose(); }
			}

			this.Documents.Clear();
			this.Name = null;
			this.Description = null;
		}

		public static WyProject FromStream(Stream stream)
		{
			if (stream == null) throw new ArgumentNullException();
			else if (!stream.CanRead) throw new ArgumentException("Input stream must have read capabilities");

			var proj = new WyProject("TODO read this from the stream");
			return proj;
		}

		public static WyProject FromFile(string filePath)
		{ return WyProject.FromStream(File.OpenRead(filePath)); }

	}
}
