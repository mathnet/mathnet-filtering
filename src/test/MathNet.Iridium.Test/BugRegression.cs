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
using MathNet.Numerics.Interpolation;

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

        [Test]
        [Obsolete("The tested algorithms is obsolete, so is this test.")]
        public void IRID119_PolynomialExtrapolatePositiveDirection()
        {
            double[] x = new double[] { -6.060771484, -5.855378418, -1.794238281, -1.229428711, 0.89935791, 2.912121582, 4.699230957, 4.788347168, 7.728830566, 11.70989502 };
            double[] y = new double[] { 0.959422052, 0.959447861, 0.959958017, 0.960028946, 0.960323274, 0.960636258, 0.960914195, 0.960928023, 0.96138531, 0.962004483 };

            PolynomialInterpolationAlgorithm pia = new PolynomialInterpolationAlgorithm(10);
            SampleList sl = new SampleList(10);

            for(int i=0;i<10;i++)
            {
                sl.Add(x[i],y[i]);
            }
           
            pia.Prepare(sl);
            NumericAssert.AreAlmostEqual(0.9622, pia.Extrapolate(12), 1e-3, "extrapolate(12)");
        }

        [Test]
        public void IRID178_ComplexNumbersHashCode()
        {
            Assert.AreNotEqual(Complex.One.GetHashCode(), Complex.I.GetHashCode(), "A");
            Assert.AreNotEqual(Complex.One.GetHashCode(), (-Complex.I).GetHashCode(), "B");
            Assert.AreNotEqual((-Complex.One).GetHashCode(), Complex.I.GetHashCode(), "C");
            Assert.AreNotEqual((-Complex.One).GetHashCode(), (-Complex.I).GetHashCode(), "D");
        }

        [Test]
        public void IRID177_MatrixPseudoInverse()
        {
            Matrix a = new Matrix(new double[][] {
                new double[] { 15, 23, 44, 54 },
                new double[] { 1, 5, 9, 4 },
                new double[] { 8, 11, 4, 2 }});

            Matrix aInverse = new Matrix(new double[][] {
                new double[] { 0.00729481932863557, -0.0906433578450537, 0.0629567950756452 },
                new double[] { -0.00695248549232449, 0.0302767536403138, 0.0601374162387492 },
                new double[] { -0.00876996343998189, 0.155054444209528, -0.033311997806593 },
                new double[] { 0.0265993197732062, -0.114057602060568, -0.0159589740025151 }});

            NumericAssert.AreAlmostEqual(aInverse, a.Inverse(), "A");
            NumericAssert.AreAlmostEqual(Matrix.Transpose(aInverse), Matrix.Transpose(a).Inverse(), "B");
        }
    }
}
