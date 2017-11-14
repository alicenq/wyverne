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
	}
}
