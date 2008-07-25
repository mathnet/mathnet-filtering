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
using MathNet.Numerics.LinearAlgebra;

namespace Iridium.Test
{
    [TestFixture]
    public class CodeSamples
    {
        [Test]
        public void CodeSample_Combinatorics_Permutation()
        {
            int[] numbers = new int[] { 1, 2, 3, 4, 5 };
            int count = numbers.Length;

            NumericAssert.AreAlmostEqual(120.0, Combinatorics.Permutations(count), "perm(5)");

            int[] permutation = new int[count];
            Combinatorics.RandomShuffle(numbers, permutation);
        }

        [Test]
        public void CodeSample_LinearAlgebra_Eigen()
        {
            Matrix m = new Matrix(new double[][] {
                new double[] { 10.0, -18.0 },
                new double[] { 6.0, -11.0 }});

            ComplexVector eigenValues = m.EigenValues;
            NumericAssert.AreAlmostEqual(1.0, eigenValues[0].Real, "Re{eigenvalueA}");
            NumericAssert.AreAlmostEqual(0.0, eigenValues[0].Imag, "Im{eigenvalueA}");
            NumericAssert.AreAlmostEqual(-2.0, eigenValues[1].Real, "Re{eigenvalueB}");
            NumericAssert.AreAlmostEqual(0.0, eigenValues[1].Imag, "Im{eigenvalueB}");

            Matrix eigenVectors = m.EigenVectors;
            NumericAssert.AreAlmostEqual(.8944271910, eigenVectors[0, 0], 1e-9, "eigenvectorA[0]");
            NumericAssert.AreAlmostEqual(.4472135955, eigenVectors[1, 0], 1e-9, "eigenvectorA[1]");
            NumericAssert.AreAlmostEqual(6.708203936, eigenVectors[0, 1], 1e-9, "eigenvectorB[0]");
            NumericAssert.AreAlmostEqual(4.472135956, eigenVectors[1, 1], 1e-9, "eigenvectorB[1]");
        }
    }
}
