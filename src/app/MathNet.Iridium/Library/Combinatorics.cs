#region Math.NET Iridium (LGPL) by Ruegg, Vermorel
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2002-2008, Christoph Rüegg, http://christoph.ruegg.name
//                          Joannes Vermorel, http://www.vermorel.com
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
using System.Collections.Generic;
using System.Text;

using MathNet.Numerics.RandomSources;
using MathNet.Numerics.Properties;

namespace MathNet.Numerics
{
    /// <summary>
    /// Static DoublePrecision Combinatorics Helper Class
    /// </summary>
    public static class Combinatorics
    {
        #region Combinatorics: Counting

        /// <summary>
        /// Computes the number of variations without repetition. When the order matters and each object can be chosen only once.
        /// </summary>
        public static
        double
        Variations(
            int n,
            int k
            )
        {
            if(k < 0 || n < 0 || k > n)
            {
                return 0;
            }

            return Math.Floor(0.5 + Math.Exp(Fn.FactorialLn(n) - Fn.FactorialLn(n - k)));
        }

        /// <summary>
        /// Computes the number of variations with repetition. When the order matters and an object can be chosen more than once.
        /// </summary>
        public static
        double
        VariationsWithRepetition(
            int n,
            int k
            )
        {
            if(k < 0 || n < 0)
            {
                return 0;
            }

            return Math.Pow(n, k);
        }

        /// <summary>
        /// Computes the number of combinations without repetition. When the order does not matter and each object can be chosen only once.
        /// </summary>
        public static
        double
        Combinations(
            int n,
            int k
            )
        {
            return Fn.BinomialCoefficient(n, k);
        }

        /// <summary>
        /// Computes the number of combinations with repetition. When the order does not matter and an object can be chosen more than once.
        /// </summary>
        public static
        double
        CombinationsWithRepetition(
            int n,
            int k
            )
        {
            if(k < 0 || n < 0 || (n == 0 && k > 0))
            {
                return 0;
            }

            if(n == 0 && k == 0)
            {
                return 1;
            }

            return Math.Floor(0.5 + Math.Exp(Fn.FactorialLn(n + k - 1) - Fn.FactorialLn(k) - Fn.FactorialLn(n - 1)));
        }

        /// <summary>
        /// Computes the number of permutations (without repetition). 
        /// </summary>
        /// <param name="n">Number of (distinguishable) elements in the set.</param>
        public static
        double
        Permutations(
            int n
            )
        {
            return Fn.Factorial(n);
        }

        #endregion

        #region Combinatorics: Generate Randomly

        static RandomSource _random = new SystemRandomSource();

        /// <summary>
        /// Random source supporting the randomized operations.
        /// </summary>
        /// <remarks>
        /// The default value is a <see cref="SystemRandomSource"/>.
        /// </remarks>
        public static RandomSource RandomSource
        {
            get { return _random; }
            set { _random = value; }
        }

        private struct IndexedValue : IComparable<IndexedValue>
        {
            internal int Index;
            internal double Value;

            public
            IndexedValue(
                int index,
                double value
                )
            {
                this.Index = index;
                this.Value = value;
            }

            public
            int
            CompareTo(
                IndexedValue other
                )
            {
                return this.Value.CompareTo(other.Value);
            }
        }

        /// <summary>Randomly shuffles the numbers 0 to N-1.</summary>
        /// <returns>An array of length <c>N</c> that contains (in any order) the integers of the interval <c>[0, N)</c>.</returns>
        public static
        int[]
        RandomPermutation(
            int n
            )
        {
            if(n < 0)
            {
                throw new ArgumentOutOfRangeException("size", n, Resources.ArgumentNotNegative);
            }

            IndexedValue[] indexedValues = new IndexedValue[n];
            for(int i = 0; i < indexedValues.Length; i++)
            {
                indexedValues[i] = new IndexedValue(i, _random.NextDouble());
            }

            // sorting the array (act as a random permutation)
            Array.Sort(indexedValues);

            // creating and returning the permutation
            int[] permutation = new int[n];
            for(int i = 0; i < n; i++)
            {
                permutation[i] = indexedValues[i].Index;
            }

            return permutation;
        }

        /// <summary>Randomly selects some of N elements without order and repetition.</summary>
        /// <returns>boolean array of length <c>N</c>, for each item true if it is selected.</returns>
        public static
        bool[]
        RandomCombination(
            int n
            )
        {
            bool[] ret = new bool[n];
            for(int i = 0; i < ret.Length; i++)
            {
                ret[i] = _random.NextBoolean();
            }

            return ret;
        }

        /// <summary>Randomly selects k of n elements without order and repetition.</summary>
        /// <returns>boolean array of length <c>N</c>, for each item true if it is selected.</returns>
        public static
        bool[]
        RandomCombination(
            int n,
            int k
            )
        {
            bool[] selection = new bool[n];
            if(k * 3 < n) 
            {
                // just pick and try
                int selectionCount = 0;
                while(selectionCount < k)
                {
                    int index = _random.Next(n);
                    if(!selection[index])
                    {
                        selection[index] = true;
                        selectionCount++;
                    }
                }

                return selection;
            }
            else 
            {
                // based on permutation
                int[] permutation = RandomPermutation(n);
                for(int i = 0; i < k; i++)
                {
                    selection[permutation[i]] = true;
                }

                return selection;
            }
        }

        /// <summary>Randomly selects k of n elements with repetition but without order.</summary>
        /// <returns>integer array of length <c>N</c>, for each item the number of times it was selected.</returns>
        public static
        int[]
        RandomCombinationWithRepetition(
            int n,
            int k
            )
        {
            int[] ret = new int[n];
            for(int i = 0; i < k; i++)
            {
                ret[_random.Next(n)]++;
            }

            return ret;
        }

        /// <summary>Randomly selects k of n elements with order but without repetition.</summary>
        /// <returns>An array of length <c>K</c> that contains the indices of the selections as integers of the interval <c>[0, N)</c>.</returns>
        public static
        int[]
        RandomVariation(
            int n,
            int k
            )
        {
            int[] selection = new int[k];
            int[] permutation = RandomPermutation(n);
            for(int i = 0; i < k; i++)
            {
                selection[i] = permutation[i];
            }

            return selection;
        }

        /// <summary>Randomly selects k of n elements with order and repetition.</summary>
        /// <returns>An array of length <c>K</c> that contains the indices of the selections as integers of the interval <c>[0, N)</c>.</returns>
        public static
        int[]
        RandomVariationWithRepetition(
            int n,
            int k
            )
        {
            int[] ret = new int[k];
            for(int i = 0; i < ret.Length; i++)
            {
                ret[i] = _random.Next(n);
            }

            return ret;
        }

        #endregion

        #region Combinatorics: Applied Random Generation

        /// <summary>
        /// Randomly permutes <c>array</c> to <c>target</c>.
        /// Both arrays should have the same size but can not be the same instance.
        /// </summary>
        /// <remarks>
        /// Shuffling an array is equivalent to generating a
        /// random permutation and applying this permutation to the array.
        /// </remarks>
        /// <param name="source">The data list to shuffle.</param>
        /// <param name="target">The resulting shuffled output data list.</param>
        public static
        void
        RandomShuffle<T>(
            IList<T> source,
            IList<T> target
            )
        {
            int len = Math.Min(source.Count, target.Count);
            int[] permutation = RandomPermutation(len);
            for(int i = 0; i < len; i++)
            {
                target[i] = source[permutation[i]];
            }
        }

        /// <summary>
        /// Randomly shuffles <c>array</c>.
        /// </summary>
        /// <remarks>
        /// Shuffling an array is equivalent to generating a
        /// random permutation and applying this permutation to the array.
        /// </remarks>
        /// <param name="array">The data list to shuffle.</param>
        public static
        void
        RandomShuffle<T>(
            IList<T> array
            )
        {
            T[] arrayCopy = new T[array.Count];
            array.CopyTo(arrayCopy, 0);
            RandomShuffle(arrayCopy, array);
        }

        /// <summary>
        /// Randomly returns a subset of size <c>numberToSelect</c> of <c>array</c>
        /// in random order without repetition.
        /// </summary>
        /// <param name="array">The data list to choose from.</param>
        /// <param name="numberToSelect">The size of the subset. Must be smaller or equal to the array length.</param>
        public static
        T[]
        RandomSubsetVariation<T>(
            IList<T> array,
            int numberToSelect
            )
        {
            T[] ret = new T[numberToSelect];
            int[] indices = RandomVariation(array.Count, numberToSelect);
            for(int i = 0; i < ret.Length; i++)
            {
                ret[i] = array[indices[i]];
            }

            return ret;
        }

        /// <summary>
        /// Randomly returns a subset of size <c>numberToSelect</c> of <c>array</c>
        /// in random order with repetition.
        /// </summary>
        /// <param name="array">The data list to choose from.</param>
        /// <param name="numberToSelect">The size of the subset. Must be smaller or equal to the array length.</param>
        public static
        T[]
        RandomSubsetVariationWithRepetition<T>(
            IList<T> array,
            int numberToSelect
            )
        {
            T[] ret = new T[numberToSelect];
            int[] indices = RandomVariationWithRepetition(array.Count, numberToSelect);
            for(int i = 0; i < ret.Length; i++)
            {
                ret[i] = array[indices[i]];
            }

            return ret;
        }

        /// <summary>
        /// Randomly returns a subset of size <c>numberToSelect</c> of <c>array</c>
        /// in preserved order without repetition.
        /// </summary>
        /// <param name="array">The data list to choose from.</param>
        /// <param name="numberToSelect">The size of the subset. Must be smaller or equal to the array length.</param>
        public static
        T[]
        RandomSubsetCombination<T>(
            IList<T> array,
            int numberToSelect
            )
        {
            T[] ret = new T[numberToSelect];
            bool[] filter = RandomCombination(array.Count, numberToSelect);
            for(int i = 0, j = 0; i < filter.Length; i++)
            {
                if(filter[i])
                {
                    ret[j++] = array[i];
                }
            }

            return ret;
        }

        /// <summary>
        /// Randomly returns a subset of size <c>numberToSelect</c> of <c>array</c>
        /// in preserved order with repetition.
        /// </summary>
        /// <param name="array">The data list to choose from.</param>
        /// <param name="numberToSelect">The size of the subset. Must be smaller or equal to the array length.</param>
        public static
        T[]
        RandomSubsetCombinationWithRepetition<T>(
            IList<T> array,
            int numberToSelect
            )
        {
            T[] ret = new T[numberToSelect];
            int[] filter = RandomCombinationWithRepetition(array.Count, numberToSelect);
            for(int i = 0, j = 0; i < filter.Length; i++)
            {
                for(int z = 0; z < filter[i]; z++)
                {
                    ret[j++] = array[i];
                }
            }

            return ret;
        }

        #endregion
    }
}
