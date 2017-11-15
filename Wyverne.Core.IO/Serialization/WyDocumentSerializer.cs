//
// WyDocumentSerializer.cs
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
using Newtonsoft.Json;

namespace Wyverne.Core.IO.Serialization
{
	public class WyDocumentSerializer<TDoc> where TDoc : WyDocument
	{
		public JsonSerializerSettings Settings { get; set; }

		public WyDocumentSerializer(bool inline = true)
		{
			Settings = new JsonSerializerSettings() {
				Formatting = inline ? Formatting.None : Formatting.Indented,
				TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
				ConstructorHandling = ConstructorHandling.Default,
				DateParseHandling = DateParseHandling.None,
				DateFormatHandling = DateFormatHandling.IsoDateFormat,
				DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
				ObjectCreationHandling = ObjectCreationHandling.Auto,
				NullValueHandling = NullValueHandling.Ignore
			};
		}

		/// <summary>
		/// Converts a document's properties into a JSON-equivalent string
		/// </summary>
		public string GetJSONProperties(TDoc document)
		{ return GetJSONProperties(document, true); }

		/// <summary>
		/// Converts a document's properties into a JSON-equivalent string
		/// </summary>
		public string GetJSONProperties(TDoc document, bool inline)
		{
			if (document == null) throw new ArgumentNullException();
			else if (document.Properties == null) return null;

			var type = document.GetType().GetProperty("Properties").PropertyType;

			return JsonConvert.SerializeObject(document.Properties, type, Settings);
		}

		public bool LoadJSONProperties<TProperties>(TDoc document, string fromJSON)
		{
			return LoadJSONProperties(document, typeof(TProperties), fromJSON);
		}

		public bool LoadJSONProperties(TDoc document, Type propertiesType, string fromJSON)
		{
			if (document == null) throw new ArgumentNullException();
			var props  = JsonConvert.DeserializeObject(fromJSON, propertiesType, Settings);

			if (props != null) {
				document.Properties = props;
				return true;
			} else {
				return false;
			}
		}
	}
}
