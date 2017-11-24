//
// TestSerialization.cs
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
using NUnit.Framework;
using System;
using Wyverne.Core.IO.Serialization;

namespace Wyverne.Core.IO.NUnit
{
	[TestFixture]
	public class TestSerialization
	{
		[Test]
		public void TestUTF8String()
		{
			Console.WriteLine("Run TestUTF8String");

			var test = "Hello world, this is a test";
			var uut = WyDataChunk.FromString(test);

			var len = uut.Length;
			var str = uut.ToUTF8String();

			var arr = new byte[len];
			uut.Read(arr, 0, arr.Length);

			Assert.AreEqual(len, test.Length);
			Assert.AreEqual(len, arr.Length);
			Assert.AreEqual(str, test);
		}

		[Test]
		public void TestPropsSerialization()
		{
			Console.WriteLine("Run TestPropsSerialization");

			var s = new WyDocumentSerializer<TestWyDocument>();
			var p = new {
				String = Environment.MachineName + "-" + DateTime.Now.ToString(),
				Int = 42,
				Float = (float)Math.PI,
				Array = new[] { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34 },
				DateTime = DateTime.Now,
				ComplexType = new {
					a = true,
					b = 12345,
					c = "hello world",
					d = Guid.NewGuid()
				}
			};
			var uut = new TestWyDocument(p);
			var copy = new TestWyDocument(null);

			var json = s.GetJSONProperties(uut);

			s.LoadJSONProperties(copy, p.GetType(), json);

			Console.WriteLine(json);

			Assert.NotNull(copy.Properties);
			Assert.AreEqual(uut.Properties.GetType(), copy.Properties.GetType());
			Assert.AreEqual(uut.Properties.GetType(), copy.Properties.GetType());

			Assert.AreNotSame(uut.Properties, copy.Properties);

			Assert.AreEqual(s.GetJSONProperties(uut), s.GetJSONProperties(copy));
		}

		[Test]
		public void TestPropsToUTF8()
		{
			Console.WriteLine("Run TestPropsToUTF8");

			var s = new WyDocumentSerializer<TestWyDocument>();
			var p = new {
				String = Environment.MachineName + "-" + Guid.NewGuid() + ":" + DateTime.Now,
				Data = new byte[] { 0, 1, 2, 4, 8, 16, 32, 64, 128 },
				Data2 = new byte[] { 0, 1, 1, 2, 3, 5, 8, 13, 21, 34, 55, 89, 164 }
			};
			var uut = new TestWyDocument(p);
			var copy = new TestWyDocument(null);

			// Convert to string first
			var json = s.GetJSONProperties(uut);

			// Then convert to binary data
			using (var data = WyDataChunk.FromString(json)) {
				// Print length
				var size = System.Text.Encoding.UTF8.GetBytes(p.String).Length + p.Data.Length + p.Data2.Length;

				Console.WriteLine($"Estimated={size}");
				Console.WriteLine($"Str=({json.Length}) {json}");
				Console.WriteLine($"Bytes={data.Length} ({(float)data.Length  / size:0.000} x original)");

				// Now create that data stream back into properties
				s.LoadJSONProperties(copy, p.GetType(), data.ToUTF8String());
			}

			// And assert
			Assert.NotNull(copy.Properties);
			Assert.AreEqual(uut.Properties.GetType(), copy.Properties.GetType());


			Assert.AreEqual(s.GetJSONProperties(uut), s.GetJSONProperties(copy));
		}

		[Test]
		public void TestSerializationContext()
		{
			Console.WriteLine("Run TestSerializationContext");

			// Create context
			var data = WyDataChunk.FromString("abcdefghijklmnopqrstuvwxyz");
			var context = new WySerializationContext("Resources/Test1", typeof(WyResource), data);

			// Turn into into a byte stream
			var str = new WyDataStream(context.GetHeader());

			// Create a new context
			var newContext = WySerializationContext.ParseStream(str);

			// Assert equality
			Assert.AreNotSame(context, newContext);
			Assert.AreEqual(context.Id, newContext.Id);
			Assert.AreEqual(context.Type, newContext.Type);
			Assert.AreEqual(context.Length, newContext.Length);
			Assert.AreEqual(context.Data.ToUTF8String(), newContext.Data.ToUTF8String());
		}
	}
}
