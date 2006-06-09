#region MathNet Numerics, Copyright ©2004 Joannes Vermorel

// MathNet Numerics, part of MathNet
//
// Copyright (c) 2004,	Joannes Vermorel, http://www.vermorel.com
//
//
// This program is free software; you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published 
// by the Free Software Foundation; either version 2 of the License, or
// (at your option) any later version.
//
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.
//
// You should have received a copy of the GNU Lesser General Public 
// License along with this program; if not, write to the Free Software
// Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.

#endregion

using System;
using System.Collections;
using System.Diagnostics;

namespace MathNet.Numerics.Generators
{
	/// <summary>Pseudo-random generator of permutations.</summary>
	/// <remarks>
	/// <p>A permutation is defined as an array <c>int[]</c> of
	/// length <c>N</c> that contains all the integers (in any order)
	/// of the interval <c>[0,N)</c>. The method <see cref="Next"/> is
	/// used to generated random permutation.</p>
	/// 
	/// <p>See the <a href="http://en.wikipedia.org/wiki/Permutation">WikiPedia</a>
	/// for more details about permutations.</p>
	/// </remarks>
	public class PermutationGenerator
	{
		private static Random random = new Random();

		/// <summary>Helper.</summary>
		private struct IndexedValue : IComparable
		{
            // HACK: refactor 'IndexedValue' to take advantage of .Net 2.0 with IComparable<T>

			public int Index;
			public double Value;

			public IndexedValue(int index, double value)
			{
				this.Index = index;
				this.Value = value;
			}

			public int CompareTo(object obj)
			{
				return this.Value.CompareTo(((IndexedValue) obj).Value);
			}
		}

		/// <summary>Gets the next random permutation.</summary>
		/// <remarks>A permutation is an array of length <c>N</c> that contains
		/// (in any order) the integers of the interval <c>[0, N)</c>..</remarks>
		public static int[] Next(int size)
		{
			if(size < 0) throw new ArgumentOutOfRangeException(
				"size", size, "The size must be non-negative.");

			IndexedValue[] indexedValues = new IndexedValue[size];
			for(int i = 0; i < indexedValues.Length; i++)
				indexedValues[i] = new IndexedValue(i, random.NextDouble());

			// sorting the array (act as a random permutation)
			Array.Sort(indexedValues);

			// creating and returning the permutation
			int[] permutation = new int[size];
			for(int i = 0; i < size; i++)
				permutation[i] = indexedValues[i].Index;

			return permutation;
		}

		/// <summary>Randomly shuffle <c>array</c>.</summary>
		/// <remarks>Shuffling an array is equivalent to generating a
		/// random permutation and applying this permutation to the array.</remarks>
		public static void Shuffle(Array array)
		{
			int[] permutation = Next(array.Length);

			object[] arrayCpy = new object[array.Length];
			array.CopyTo(arrayCpy, 0);

			for(int i = 0; i < array.Length; i++)
				array.SetValue(arrayCpy[i], permutation[i]);
		}

		/// <summary>Random subset of the provided collection.</summary>
		/// <param name="c">Collection from which the subset is drawn.</param>
		/// <param name="subsetCount">Expected size of the subset (may be zero).</param>
		/// <returns>A collection of size <c>subsectCount</c></returns>
		/// <remarks>No item of <c>c</c> could be drawn more than once.
		/// The order of the items in <c>c</c> is kept. The complexity
		/// of operation is <c>O(n)</c> where <c>n</c> is the number
		/// of elements in the collection <c>c</c>.</remarks>
		public static Array Subset(ICollection c, int subsetCount)
		{
			if(c == null)
				throw new ArgumentNullException("c", "Collection should not be null.");

			if(subsetCount < 0 || subsetCount > c.Count)
				throw new ArgumentOutOfRangeException("subsetCount", subsetCount,
					"Must a positive integer lower or equal to the collection size.");
			
			object[] subset = new object[subsetCount];

			/* Two cases are considered in order to avoid a quadratic
			 * behavior of the first case when subsetCount close to
			 * c.Count . */
			if(subsetCount < c.Count / 2)
			{
				bool[] abstractSubset = AbstractSubset(c.Count, subsetCount);

				int setIndex = 0, subsetIndex = 0;
				foreach(object obj in c)
				{
					if(abstractSubset[setIndex++]) subset[subsetIndex++] = obj;
				}

			}
			else
			{
				bool[] abstractSubset = AbstractSubset(c.Count, c.Count - subsetCount);

				int setIndex = 0, subsetIndex = 0;
				foreach(object obj in c)
				{
					if(!abstractSubset[setIndex++]) subset[subsetIndex++] = obj;
				}
			}

			return subset;
		}

		private static bool[] AbstractSubset(int setCount, int subsetCount)
		{
			bool[] abstractSubset = new bool[setCount];
			int selectionCount = 0;

			while(selectionCount < subsetCount)
			{
				int index = random.Next(setCount);
				if(!abstractSubset[index])
				{
					abstractSubset[index] = true;
					selectionCount++;
				}
			}
			
			return abstractSubset;
		}

//        #region NUnit testing suite
//#if DEBUG
//        /// <summary>
//        /// Testing the methods of <see cref="PermutationGenerator"/>.
//        /// </summary>
//        [TestFixture]
//        public class TestingSuite
//        {
//            /// <summary>
//            /// Testing the method <see cref="PermutationGenerator.Shuffle"/>.
//            /// </summary>
//            [Test] public void Suffle()
//            {
//                int[] array = new int[100];
//                for(int i = 0; i < array.Length; i++) array[i] = i;

//                PermutationGenerator.Shuffle(array);
//                Array.Sort(array);

//                for(int i = 0; i < array.Length; i++)
//                    Assertion.AssertEquals("#A00 Unexpected array value", i, array[i]);
//            }

//            /// <summary>
//            /// Testing the method <see cref="PermutationGenerator.Subset"/>.
//            /// </summary>
//            [Test] public void Subset()
//            {
//                int[] array = new int[100];
//                for(int i = 0; i < array.Length; i++) array[i] = i;

//                // testing 30 < 100 / 2
//                ICollection subset = PermutationGenerator.Subset(array, 30);
				
//                int lastItem = -1;
//                foreach(int item  in subset)
//                {
//                    Assertion.Assert("#A00 The order of the elements is not preserved.",
//                        item > lastItem);
					
//                    lastItem = item;
//                }


//                // testing 60 > 100 / 2
//                subset = PermutationGenerator.Subset(array, 60);
				
//                lastItem = -1;
//                foreach(int item  in subset)
//                {
//                    Assertion.Assert("#A00 The order of the elements is not preserved.",
//                        item > lastItem);
					
//                    lastItem = item;
//                }
//            }
//        }
//#endif
//        #endregion
	}
}
