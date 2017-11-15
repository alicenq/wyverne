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
using System.Diagnostics;
using System.Threading;

namespace Wyverne.Core.IO.NUnit
{
	[TestFixture()]
	public class TestSerialization
	{
		Random r = new Random();

		[Test()]
		public void TestUTF8String()
		{
			var test = "Hello world, this is a test";
			var uut = WyDataChunk.FromString(test);

			var len = uut.Length;
			var str = uut.ToUTF8String();

			Console.WriteLine($"{test} --({len})-> {str}");

			var arr = new byte[len];
			uut.Read(arr, 0, arr.Length);

			Assert.AreEqual(len, test.Length);
			Assert.AreEqual(len, arr.Length);
			Assert.AreEqual(str, test);
		}

		[Test()]
		public void TestPropsSerialization()
		{
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

			var pCopy = Cast(p, copy.Properties);

			Console.WriteLine(json);

			Assert.NotNull(copy.Properties);
			Assert.AreEqual(uut.Properties.GetType(), copy.Properties.GetType());

			Assert.AreNotSame(p, pCopy);
			Assert.AreNotSame(p.Array, pCopy.Array);
			Assert.AreNotSame(p.ComplexType, pCopy.ComplexType);

			Assert.AreEqual(p.String, pCopy.String);
			Assert.AreEqual(p.Int, pCopy.Int);
			Assert.AreEqual(p.Float, pCopy.Float);
			Assert.AreEqual(p.DateTime, pCopy.DateTime);
			Assert.AreEqual(p.Array, pCopy.Array);
			Assert.AreEqual(p.ComplexType.a, pCopy.ComplexType.a);
			Assert.AreEqual(p.ComplexType.b, pCopy.ComplexType.b);
			Assert.AreEqual(p.ComplexType.c, pCopy.ComplexType.c);
			Assert.AreEqual(p.ComplexType.d, pCopy.ComplexType.d);
		}

		// Helper function to trick the compiler into converting obj into an anonymous type
		private T Cast<T>(T typeHolder, object obj)
		{ return (T)obj; }
	}
}
