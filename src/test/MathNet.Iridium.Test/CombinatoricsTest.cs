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

namespace Iridium.Test
{
    [TestFixture]
    public class CombinatoricsTest
    {
        [Test]
        public void CountingVariationsTest()
        {
            Assert.AreEqual(1, Combinatorics.Variations(0, 0), "Var(0,0)");
            Assert.AreEqual(1, Combinatorics.Variations(1, 0), "Var(1,0)");
            Assert.AreEqual(0, Combinatorics.Variations(0, 1), "Var(0,1)");
            Assert.AreEqual(1, Combinatorics.Variations(10, 0), "Var(10,0)");
            Assert.AreEqual(90, Combinatorics.Variations(10, 2), "Var(10,2)");
            Assert.AreEqual(5040, Combinatorics.Variations(10, 4), "Var(10,4)");
            Assert.AreEqual(151200, Combinatorics.Variations(10, 6), "Var(10,6)");
            NumericAssert.AreAlmostEqual(3628800, Combinatorics.Variations(10, 9), 1e-5, "Var(10,9)");
            NumericAssert.AreAlmostEqual(3628800, Combinatorics.Variations(10, 10), 1e-5, "Var(10,10)");
            Assert.AreEqual(0, Combinatorics.Variations(10, 11), "Var(10,11)");
        }

        [Test]
        public void CountingVariationsWithRepetitionTest()
        {
            Assert.AreEqual(1, Combinatorics.VariationsWithRepetition(0, 0), "VarRep(0,0)");
            Assert.AreEqual(1, Combinatorics.VariationsWithRepetition(1, 0), "VarRep(1,0)");
            Assert.AreEqual(0, Combinatorics.VariationsWithRepetition(0, 1), "VarRep(0,1)");
            Assert.AreEqual(1, Combinatorics.VariationsWithRepetition(10, 0), "VarRep(10,0)");
            Assert.AreEqual(100, Combinatorics.VariationsWithRepetition(10, 2), "VarRep(10,2)");
            Assert.AreEqual(10000, Combinatorics.VariationsWithRepetition(10, 4), "VarRep(10,4)");
            Assert.AreEqual(1000000, Combinatorics.VariationsWithRepetition(10, 6), "VarRep(10,6)");
            Assert.AreEqual(1000000000, Combinatorics.VariationsWithRepetition(10, 9), "VarRep(10,9)");
            Assert.AreEqual(10000000000, Combinatorics.VariationsWithRepetition(10, 10), "VarRep(10,10)");
            Assert.AreEqual(100000000000, Combinatorics.VariationsWithRepetition(10, 11), "VarRep(10,11)");
        }

        [Test]
        public void CountingCombinationsTest()
        {
            Assert.AreEqual(1, Combinatorics.Combinations(0, 0), "Comb(0,0)");
            Assert.AreEqual(1, Combinatorics.Combinations(1, 0), "Comb(1,0)");
            Assert.AreEqual(0, Combinatorics.Combinations(0, 1), "Comb(0,1)");
            Assert.AreEqual(1, Combinatorics.Combinations(10, 0), "Comb(10,0)");
            Assert.AreEqual(45, Combinatorics.Combinations(10, 2), "Comb(10,2)");
            Assert.AreEqual(210, Combinatorics.Combinations(10, 4), "Comb(10,4)");
            Assert.AreEqual(210, Combinatorics.Combinations(10, 6), "Comb(10,6)");
            Assert.AreEqual(10, Combinatorics.Combinations(10, 9), "Comb(10,9)");
            Assert.AreEqual(1, Combinatorics.Combinations(10, 10), "Comb(10,10)");
            Assert.AreEqual(0, Combinatorics.Combinations(10, 11), "Comb(10,11)");
        }

        [Test]
        public void CountingCombinationsWithRepetitionTest()
        {
            Assert.AreEqual(1, Combinatorics.CombinationsWithRepetition(0, 0), "CombRep(0,0)");
            Assert.AreEqual(1, Combinatorics.CombinationsWithRepetition(1, 0), "CombRep(1,0)");
            Assert.AreEqual(0, Combinatorics.CombinationsWithRepetition(0, 1), "CombRep(0,1)");
            Assert.AreEqual(1, Combinatorics.CombinationsWithRepetition(10, 0), "CombRep(10,0)");
            Assert.AreEqual(55, Combinatorics.CombinationsWithRepetition(10, 2), "CombRep(10,2)");
            Assert.AreEqual(715, Combinatorics.CombinationsWithRepetition(10, 4), "CombRep(10,4)");
            Assert.AreEqual(5005, Combinatorics.CombinationsWithRepetition(10, 6), "CombRep(10,6)");
            Assert.AreEqual(48620, Combinatorics.CombinationsWithRepetition(10, 9), "CombRep(10,9)");
            Assert.AreEqual(92378, Combinatorics.CombinationsWithRepetition(10, 10), "CombRep(10,10)");
            Assert.AreEqual(167960, Combinatorics.CombinationsWithRepetition(10, 11), "CombRep(10,11)");
        }

        [Test]
        public void CountingPermutations()
        {
            Assert.AreEqual(1, Combinatorics.Permutations(0), "Perm(0)");
            Assert.AreEqual(1, Combinatorics.Permutations(1), "Perm(1)");
            Assert.AreEqual(2, Combinatorics.Permutations(2), "Perm(2)");
            Assert.AreEqual(40320, Combinatorics.Permutations(8), "Perm(8)");
            Assert.AreEqual(1307674368000, Combinatorics.Permutations(15), "Perm(15)");
            Assert.AreEqual(265252859812191058636308480000000d, Combinatorics.Permutations(30), "Perm(30)");
            NumericAssert.AreAlmostEqual(0.3856204824e216, Combinatorics.Permutations(128), 1e-6, "Perm(128)");
        }
    }
}
