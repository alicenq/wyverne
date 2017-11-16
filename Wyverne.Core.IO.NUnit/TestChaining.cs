//
// TestChaining.cs
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
using NUnit.Framework;
using Wyverne.Core.IO.Serialization;

namespace Wyverne.Core.IO.NUnit
{
	[TestFixture]
	public class TestChaining
	{
		[Test]
		public void TestChainManual()
		{
			Console.WriteLine("Run TestChainManual");

			var chunks = new[] {
				WyDataChunk.FromString("aaa"),
				WyDataChunk.FromString("bbb"),
				WyDataChunk.FromString("ccc"),
				WyDataChunk.FromString("ddd")
			};

			// Create chain
			var chunk = WyDataChunk.Chain(chunks);

			Console.WriteLine(String.Join(" ", chunks.Select(c => c.ToUTF8String())));

			// Create compount string/count by manually iterating
			var str = "";
			var count = 0;

			while (chunk != null) {
				str += chunk.ToUTF8String();
				chunk.Dispose();
				chunk = chunk.Next;
				count++;
			}

			Console.WriteLine(str);

			// Assert
			Assert.AreEqual(count, chunks.Length);
			Assert.AreEqual(str, "aaabbbcccddd");
			chunks.ToList().ForEach(c => Assert.IsTrue(c.Disposed));
		}

		[Test]
		public void TestChainEnumeration()
		{
			Console.WriteLine("Run TestChainEnumeration");

			var chunks = new[] {
				WyDataChunk.FromString("aaa"),
				WyDataChunk.FromString("bbb"),
				WyDataChunk.FromString("ccc"),
				WyDataChunk.FromString("ddd")
			};

			Console.WriteLine(String.Join(" ", chunks.Select(c => c.ToUTF8String())));

			// Create chain
			var first = WyDataChunk.Chain(chunks);

			// Create compount string/count by using IEnumerable
			var str = "";
			var chain = first.GetChain();

			foreach (var chunk in chain) {
				str += chunk.ToUTF8String();
				chunk.Dispose();
			}

			Console.WriteLine(str);

			// Assert
			Assert.AreEqual(chain.Count(), chunks.Length);
			Assert.AreEqual(str, "aaabbbcccddd");
			chunks.ToList().ForEach(c => Assert.IsTrue(c.Disposed));
		}

		[Test]
		public void TestChainCircular()
		{
			Console.WriteLine("Run TestChainCircular");

			var chunks = new[] {
				WyDataChunk.FromString("aaa"),
				WyDataChunk.FromString("bbb"),
				WyDataChunk.FromString("ccc"),
				WyDataChunk.FromString("ddd")
			};

			Console.WriteLine(String.Join(" ", chunks.Select(c => c.ToUTF8String())));

			// Create chain
			var first = WyDataChunk.ChainCircular(chunks);

			// Create compount string/count by using IEnumerable
			var str = "";
			var chunk = first;

			for (int i = 0; i < chunks.Length * 3; i++) {
				str += chunk.ToUTF8String();
				chunk = chunk.Next;
			}

			chunks.ToList().ForEach(c => c.Dispose());

			Console.WriteLine(str);

			// Assert
			Assert.AreEqual(chunks.Last().Next, chunks.First());
			Assert.AreEqual(str, "aaabbbcccddd");
			chunks.ToList().ForEach(c => Assert.IsTrue(c.Disposed));
		}

	}
}
