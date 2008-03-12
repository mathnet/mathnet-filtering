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
    public class BugRegression
    {
        [Test]
        public void IRID77_NegativeComplexLogarithm()
        {
            Complex minusOne = -Complex.One;
            Complex piI = minusOne.NaturalLogarithm();

            Assert.AreEqual(0.0, piI.Real, 1e-8, "Re{ln(-1)} = 0");
            Assert.AreEqual(Constants.Pi, piI.Imag, 1e-8, "Im{ln(-1)} = Pi");

            Complex zero = Complex.Zero;
            Complex lnZero = zero.NaturalLogarithm();

            Assert.AreEqual(double.NegativeInfinity, lnZero.Real, "Re{ln(0)} = -infinity");
            Assert.AreEqual(0, lnZero.Imag, "Im{ln(0)} = 0");
        }

        [Test]
        public void IRID90_CholeskySolve()
        {
            Matrix i = Matrix.Identity(3, 3);

            double[][] pvals1 = { new double[] { 1.0, 1.0, 1.0 }, new double[] { 1.0, 2.0, 3.0 }, new double[] { 1.0, 3.0, 6.0 } };
            Matrix m1 = new Matrix(pvals1);
            CholeskyDecomposition cd1 = new CholeskyDecomposition(m1);

            Matrix inv1a = cd1.Solve(i);
            Matrix test1a = m1 * inv1a;
            NumericAssert.AreAlmostEqual(i, test1a, "1A");
            Matrix inv1b = m1.Inverse();
            NumericAssert.AreAlmostEqual(inv1a, inv1b, "1B");

            double[][] pvals2 = { new double[] { 25, -5, 10 }, new double[] { -5, 17, 10 }, new double[] { 10, 10, 62 } };
            Matrix m2 = new Matrix(pvals2);
            CholeskyDecomposition cd2 = new CholeskyDecomposition(m2);

            Matrix inv2a = cd2.Solve(i);
            Matrix test2a = m2 * inv2a;
            NumericAssert.AreAlmostEqual(i, test2a, "2A");
            Matrix inv2b = m2.Inverse();
            NumericAssert.AreAlmostEqual(inv2a, inv2b, "2B");
        }

        [Test]
        public void IRID107_ComplexPowerAtZero()
        {
            Complex zeroPowTwo = Complex.Zero.Power(2);
            Assert.AreEqual(0d, zeroPowTwo.Real, "Re{(0)^(2)} = 0");
            Assert.AreEqual(0d, zeroPowTwo.Imag, "Im{(0)^(2)} = 0");
        }
    }
}
