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
using System.Linq;

namespace Wyverne.Core.IO.Serialization
{
	/// <summary>
	/// Interface implemented by all serialized WyResources
	/// </summary>
	public interface IWyResource
	{
		WySerializationContext Serialize();
	}

	public abstract class WyResource : IWyResource
	{
		public abstract WySerializationContext Serialize();

		/// <summary>
		/// Returns false if the passed id is blank, less than 4 characters or contains characters outside the range 0-9a-zA-Z._-
		/// </summary>
		public static bool IsValidId(string id)
		{
			id = id?.Trim();
			return (id != null && id.Length >= 4 && id.All(c => Char.IsLetterOrDigit(c) || c == '.' || c == '-' || c == '_'));
		}

		/// <summary>
		/// Trims and makes lowercase an input string, replacing any internal spaces with underscores and slashes with dots
		/// </summary>
		public static string StrToId(string str)
		{
			var id = str?
				.Trim()
				.ToLower()
				.Replace(' ', '_')
				.Replace('/', '.')
				.Replace('\\', '.');

			if (!IsValidId(id)) throw new InvalidResourceIdException();

			return id;
		}
	}
}
