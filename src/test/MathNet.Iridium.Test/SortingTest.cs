#region Math.NET Iridium (LGPL) by Ruegg
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2002-2008, Christoph Rüegg, http://christoph.ruegg.name
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

using NUnit.Framework;

using MathNet.Numerics;
using MathNet.Numerics.RandomSources;

namespace Iridium.Test
{
    [TestFixture]
    public class SortingTest
    {
        [Test]
        public void TestRandomTupleArraySorting()
        {
            const int len = 0x1 << 10;
            SystemRandomSource random = new SystemRandomSource();

            int[] keys = new int[len];
            int[] items = new int[len];
            int[] keysCopy = new int[len];

            for(int i = 0; i < keys.Length; i++)
            {
                keys[i] = random.Next();
                keysCopy[i] = keys[i];
                items[i] = -keys[i];
            }

            Sorting.Sort(keys, items);

            for(int i = 1; i < keys.Length; i++)
            {
                Assert.IsTrue(keys[i] >= keys[i - 1], "Sort Order - " + i.ToString());
                Assert.AreEqual(-keys[i], items[i], "Items Permutation - " + i.ToString());
            }
            for(int i = 0; i < keysCopy.Length; i++)
            {
                Assert.IsTrue(Array.IndexOf(keys, keysCopy[i]) >= 0, "All keys still there - " + i.ToString());
            }
        }

        [Test]
        public void TestRandomTupleListSorting()
        {
            const int len = 0x1 << 10;
            SystemRandomSource random = new SystemRandomSource();

            List<int> keys = new List<int>(len);
            List<int> items = new List<int>(len);
            int[] keysCopy = new int[len];

            for(int i = 0; i < len; i++)
            {
                int value = random.Next();
                keys.Add(value);
                keysCopy[i] = value;
                items.Add(-value);
            }

            Sorting.Sort(keys, items);

            for(int i = 1; i < len; i++)
            {
                Assert.IsTrue(keys[i] >= keys[i - 1], "Sort Order - " + i.ToString());
                Assert.AreEqual(-keys[i], items[i], "Items Permutation - " + i.ToString());
            }
            for(int i = 0; i < keysCopy.Length; i++)
            {
                Assert.IsTrue(keys.IndexOf(keysCopy[i]) >= 0, "All keys still there - " + i.ToString());
            }
        }

        [Test]
        public void TestRandomTripleArraySorting()
        {
            const int len = 0x1 << 10;
            SystemRandomSource random = new SystemRandomSource();

            int[] keys = new int[len];
            int[] items1 = new int[len];
            int[] items2 = new int[len];
            int[] keysCopy = new int[len];

            for(int i = 0; i < keys.Length; i++)
            {
                keys[i] = random.Next();
                keysCopy[i] = keys[i];
                items1[i] = -keys[i];
                items2[i] = keys[i] >> 2;
            }

            Sorting.Sort(keys, items1, items2);

            for(int i = 1; i < keys.Length; i++)
            {
                Assert.IsTrue(keys[i] >= keys[i - 1], "Sort Order - " + i.ToString());
                Assert.AreEqual(-keys[i], items1[i], "Items1 Permutation - " + i.ToString());
                Assert.AreEqual(keys[i] >> 2, items2[i], "Items2 Permutation - " + i.ToString());
            }
            for(int i = 0; i < keysCopy.Length; i++)
            {
                Assert.IsTrue(Array.IndexOf(keys, keysCopy[i]) >= 0, "All keys still there - " + i.ToString());
            }
        }
    }
}
