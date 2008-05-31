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
using System.Globalization;

using NUnit.Framework;

using MathNet.Numerics;
using MathNet.Numerics.LinearAlgebra;

namespace Iridium.Test
{
    [TestFixture]
    public class ComplexMatrixTest
    {
        Complex j = Complex.I;
        Complex s = new Complex(1, 1);
        Matrix ma3x2, mb3x2, mc2x2, md2x4;
        ComplexMatrix ra3x2, rb3x2, rc2x2, rd2x4;
        ComplexMatrix ia3x2, ib3x2, ic2x2, id2x4;
        ComplexMatrix ca3x2, cb3x2, cc2x2, cd2x4;

        [SetUp]
        public void TestComplexMatrix_Setup()
        {
            /*
            MATLAB:
            ma3x2 = [1 -2;-1 4;5 7]
            mb3x2 = [10 2.5;-3 -1.5;19 -6]
            mc2x2 = [1 2;3 4]
            md2x4 = [1 2 -3 12;3 3.1 4 2]
            ra3x2 = ma3x2 + 2
            rb3x2 = mb3x2 - 1
            rc2x2 = mc2x2 + 5
            rd2x4 = md2x4 * 2
            ia3x2 = (ra3x2 * 2) * j
            ib3x2 = (rb3x2 * 3 + 1) * j
            ic2x2 = (rc2x2 + 2) * j
            id2x4 = (rd2x4 - 5) * j
            ca3x2 = 2*ra3x2 - 2*ia3x2
            cb3x2 = rb3x2 + 3*ib3x2
            cc2x2 = rc2x2 + 2 - 3*ic2x2
            cd2x4 = -2*rd2x4 + id2x4 + 1-j
            */

            ma3x2 = new Matrix(new double[][] {
                new double[] { 1, -2 },
                new double[] { -1, 4 },
                new double[] { 5, 7 }});
            mb3x2 = new Matrix(new double[][] {
                new double[] { 10, 2.5 },
                new double[] { -3, -1.5 },
                new double[] { 19, -6 }});
            mc2x2 = new Matrix(new double[][] {
                new double[] { 1, 2 },
                new double[] { 3, 4 }});
            md2x4 = new Matrix(new double[][] {
                new double[] { 1, 2, -3, 12 },
                new double[] { 3, 3.1, 4, 2 }});

            ra3x2 = ComplexMatrix.Create(ma3x2) + 2;
            rb3x2 = ComplexMatrix.Create(mb3x2) - 1;
            rc2x2 = ComplexMatrix.Create(mc2x2) + 5;
            rd2x4 = ComplexMatrix.Create(md2x4) * 2;

            ia3x2 = (ra3x2 * 2) * j;
            ib3x2 = (rb3x2 * 3 + 1) * j;
            ic2x2 = (rc2x2 + 2) * j;
            id2x4 = (rd2x4 - 5) * j;

            ca3x2 = 2 * ra3x2 - 2 * ia3x2;
            cb3x2 = rb3x2 + 3 * ib3x2;
            cc2x2 = rc2x2 + 2 - 3 * ic2x2;
            cd2x4 = -2 * rd2x4 + id2x4 + (1 - j);
        }

        [Test]
        public void TestComplexMatrix_AdditiveTranspose()
        {
            /*
            MATLAB:
            sum_cc = ca3x2 + cb3x2
            diff_cc = ca3x2 - cb3x2
            sum_cm = ca3x2 + mb3x2
            diff_cm = ca3x2 - mb3x2
            sum_cs = ca3x2 + s
            diff_cs = ca3x2 - s
            neg_c = -ca3x2
            conj_c = conj(ca3x2)
            trans_c = ca3x2.'
            htrans_c = ca3x2'
            */

            // ComplexMatrix + ComplexMatrix
            ComplexMatrix sum_cc = new ComplexMatrix(new Complex[][] {
                new Complex[] { 15+72*j, 1.5+16.5*j },
                new Complex[] { -2-37*j, 9.5-43.5*j },
                new Complex[] { 32+137*j, 11-96*j }});
            NumericAssert.AreAlmostEqual(sum_cc, ca3x2 + cb3x2, "sum cc 1");
            ComplexMatrix sum_cc_inplace = ca3x2.Clone();
            NumericAssert.AreAlmostEqual(sum_cc, sum_cc_inplace.Add(cb3x2), "sum cc 2");
            sum_cc_inplace.AddInplace(cb3x2);
            NumericAssert.AreAlmostEqual(sum_cc, sum_cc_inplace, "sum cc 3");

            // ComplexMatrix - ComplexMatrix
            ComplexMatrix diff_cc = new ComplexMatrix(new Complex[][] {
                new Complex[] { -3-96*j, -1.5-16.5*j },
                new Complex[] { 6+29*j, 14.5-4.5*j },
                new Complex[] { -4-193*j, 25+24*j }});
            NumericAssert.AreAlmostEqual(diff_cc, ca3x2 - cb3x2, "diff cc 1");
            ComplexMatrix diff_cc_inplace = ca3x2.Clone();
            NumericAssert.AreAlmostEqual(diff_cc, diff_cc_inplace.Subtract(cb3x2), "diff cc 2");
            diff_cc_inplace.SubtractInplace(cb3x2);
            NumericAssert.AreAlmostEqual(diff_cc, diff_cc_inplace, "diff cc 3");

            // ComplexMatrix + Matrix
            ComplexMatrix sum_cm = new ComplexMatrix(new Complex[][] {
                new Complex[] { 16-12*j, 2.5 },
                new Complex[] { -1-4*j, 10.5-24*j },
                new Complex[] { 33-28*j, 12-36*j }});
            NumericAssert.AreAlmostEqual(sum_cm, ca3x2 + mb3x2, "sum cm 1");
            ComplexMatrix sum_cm_inplace = ca3x2.Clone();
            NumericAssert.AreAlmostEqual(sum_cm, sum_cm_inplace.Add(mb3x2), "sum cm 2");
            sum_cm_inplace.AddInplace(mb3x2);
            NumericAssert.AreAlmostEqual(sum_cm, sum_cm_inplace, "sum cm 3");

            // ComplexMatrix - Matrix
            ComplexMatrix diff_cm = new ComplexMatrix(new Complex[][] {
                new Complex[] { -4-12*j, -2.5 },
                new Complex[] { 5-4*j, 13.5-24*j },
                new Complex[] { -5-28*j, 24-36*j }});
            NumericAssert.AreAlmostEqual(diff_cm, ca3x2 - mb3x2, "diff cm 1");
            ComplexMatrix diff_cm_inplace = ca3x2.Clone();
            NumericAssert.AreAlmostEqual(diff_cm, diff_cm_inplace.Subtract(mb3x2), "diff cm 2");
            diff_cm_inplace.SubtractInplace(mb3x2);
            NumericAssert.AreAlmostEqual(diff_cm, diff_cm_inplace, "diff cm 3");

            // ComplexMatrix + Complex
            ComplexMatrix sum_cs = new ComplexMatrix(new Complex[][] {
                new Complex[] { 7-11*j, 1+j },
                new Complex[] { 3-3*j, 13-23*j },
                new Complex[] { 15-27*j, 19-35*j }});
            NumericAssert.AreAlmostEqual(sum_cs, ca3x2 + s, "sum cs 1");
            ComplexMatrix sum_cs_inplace = ca3x2.Clone();
            NumericAssert.AreAlmostEqual(sum_cs, sum_cs_inplace.Add(s), "sum cs 2");
            sum_cs_inplace.AddInplace(s);
            NumericAssert.AreAlmostEqual(sum_cs, sum_cs_inplace, "sum cs 3");

            // ComplexMatrix - Complex
            ComplexMatrix diff_cs = new ComplexMatrix(new Complex[][] {
                new Complex[] { 5-13*j, -1-j },
                new Complex[] { 1-5*j, 11-25*j },
                new Complex[] { 13-29*j, 17-37*j }});
            NumericAssert.AreAlmostEqual(diff_cs, ca3x2 - s, "diff cs 1");
            ComplexMatrix diff_cs_inplace = ca3x2.Clone();
            NumericAssert.AreAlmostEqual(diff_cs, diff_cs_inplace.Subtract(s), "diff cs 2");
            diff_cs_inplace.SubtractInplace(s);
            NumericAssert.AreAlmostEqual(diff_cs, diff_cs_inplace, "diff cs 3");

            // ComplexMatrix Negate
            ComplexMatrix neg_c = new ComplexMatrix(new Complex[][] {
                new Complex[] { -6+12*j, 0 },
                new Complex[] { -2+4*j, -12+24*j },
                new Complex[] { -14+28*j, -18+36*j }});
            NumericAssert.AreAlmostEqual(neg_c, -ca3x2, "neg c 1");
            ComplexMatrix neg_c_inplace = ca3x2.Clone();
            NumericAssert.AreAlmostEqual(neg_c, neg_c_inplace.Negate(), "neg c 2");
            neg_c_inplace.NegateInplace();
            NumericAssert.AreAlmostEqual(neg_c, neg_c_inplace, "neg c 3");

            // ComplexMatrix Conjugate
            ComplexMatrix conj_c = new ComplexMatrix(new Complex[][] {
                new Complex[] { 6+12*j, 0 },
                new Complex[] { 2+4*j, 12+24*j },
                new Complex[] { 14+28*j, 18+36*j }});
            NumericAssert.AreAlmostEqual(conj_c, ca3x2.Conjugate(), "conj c 1");
            ComplexMatrix conj_c_inplace = ca3x2.Clone();
            conj_c_inplace.ConjugateInplace();
            NumericAssert.AreAlmostEqual(conj_c, conj_c_inplace, "conj c 2");

            // ComplexMatrix Transpose (Non-Conjugated)
            ComplexMatrix trans_c = new ComplexMatrix(new Complex[][] {
                new Complex[] { 6-12*j, 2-4*j, 14-28*j },
                new Complex[] { 0, 12-24*j, 18-36*j }});
            NumericAssert.AreAlmostEqual(trans_c, ca3x2.Transpose(), "trans c 1");

            // ComplexMatrix Hermitian Transpose (Conjugated)
            ComplexMatrix htrans_c = new ComplexMatrix(new Complex[][] {
                new Complex[] { 6+12*j, 2+4*j, 14+28*j },
                new Complex[] { 0, 12+24*j, 18+36*j }});
            NumericAssert.AreAlmostEqual(htrans_c, ca3x2.HermitianTranspose(), "htrans c 1");

        }

        [Test]
        public void TestComplexMatrix_Multiplicative()
        {
            /*
            MATLAB:
            prod_cc = ca3x2 * cd2x4
            prod_cm = ca3x2 * md2x4
            prod_cs = ca3x2 * s
            */

            // ComplexMatrix * ComplexMatrix
            ComplexMatrix prod_cc = new ComplexMatrix(new Complex[][] {
                new Complex[] { -66+12*j, -66+72*j, -66-228*j, -66+672*j },
                new Complex[] { -154+268*j, -154+300*j, -154+308*j, -154+368*j },
                new Complex[] { -352+424*j, -352+582*j, -352+44*j, -352+1784*j }});
            NumericAssert.AreAlmostEqual(prod_cc, ca3x2 * cd2x4, "prod cc 1");
            NumericAssert.AreAlmostEqual(prod_cc, ca3x2.Multiply(cd2x4), "prod cc 2");

            // ComplexMatrix * Matrix
            ComplexMatrix prod_cm = new ComplexMatrix(new Complex[][] {
                new Complex[] { 6-12*j, 12-24*j, -18+36*j, 72-144*j },
                new Complex[] { 38-76*j,41.2-82.4*j, 42-84*j, 48-96*j },
                new Complex[] { 68-136*j, 83.8-167.6*j, 30-60*j, 204-408*j }});
            NumericAssert.AreAlmostEqual(prod_cm, ca3x2 * md2x4, "prod cm 1");
            NumericAssert.AreAlmostEqual(prod_cm, ca3x2.Multiply(md2x4), "prod cm 2");

            // ComplexMatrix * Complex
            ComplexMatrix prod_cs = new ComplexMatrix(new Complex[][] {
                new Complex[] { 18-6*j, 0 },
                new Complex[] { 6-2*j,36-12*j },
                new Complex[] { 42-14*j, 54-18*j }});
            NumericAssert.AreAlmostEqual(prod_cs, ca3x2 * s, "prod cs 1");
            NumericAssert.AreAlmostEqual(prod_cs, ca3x2.Multiply(s), "prod cs 2");
        }
    }
}
