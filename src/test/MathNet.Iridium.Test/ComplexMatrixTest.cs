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
        Vector v2;
        ComplexVector cv2;

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
            v2 = [5 -2]
            cv2 = [5+j, -2+3j]
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

            v2 = new Vector(new double[] { 5, -2 });
            cv2 = new ComplexVector(new Complex[] { 5 + j, -2 + 3 * j });
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
            trans_c2 = cc2x2.'
            htrans_c2 = cc2x2'
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

            // ComplexMatrix Transpose (Non-Conjugated) (Square)
            ComplexMatrix trans_c2 = new ComplexMatrix(new Complex[][] {
                new Complex[] { 8-24*j, 10-30*j },
                new Complex[] { 9-27*j, 11-33*j }});
            NumericAssert.AreAlmostEqual(trans_c2, cc2x2.Transpose(), "trans c2 1");
            ComplexMatrix trans_c2_inplace = cc2x2.Clone();
            trans_c2_inplace.TransposeInplace();
            NumericAssert.AreAlmostEqual(trans_c2, trans_c2_inplace, "trans c2 2");

            // ComplexMatrix Hermitian Transpose (Conjugated) (Square)
            ComplexMatrix htrans_c2 = new ComplexMatrix(new Complex[][] {
                new Complex[] { 8+24*j, 10+30*j },
                new Complex[] { 9+27*j, 11+33*j }});
            NumericAssert.AreAlmostEqual(htrans_c2, cc2x2.HermitianTranspose(), "htrans c2 1");
            ComplexMatrix htrans_c2_inplace = cc2x2.Clone();
            htrans_c2_inplace.HermitianTransposeInplace();
            NumericAssert.AreAlmostEqual(htrans_c2, htrans_c2_inplace, "htrans c2 2");

        }

        [Test]
        public void TestComplexMatrix_Multiplicative()
        {
            /*
            MATLAB:
            prod_cc = ca3x2 * cd2x4
            prod_cm = ca3x2 * md2x4
            prod_cs = ca3x2 * s
            prod_cc2 = cc2x2 * cc2x2
            prod_cm2 = cc2x2 * mc2x2
            prod_cs2 = cc2x2 * s
            prod_ccv = ca3x2 * cv2.'
            prod_cv = ca3x2 * v2.'
            prod_ccvdl = diag(cv2) * cc2x2
            prod_ccvdr = cc2x2 * diag(cv2)
            prod_cvdl = diag(v2) * cc2x2
            prod_cvdr = cc2x2 * diag(v2)
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

            // ComplexMatrix * ComplexMatrix (Square)
            ComplexMatrix prod_cc2 = new ComplexMatrix(new Complex[][] {
                new Complex[] { -1232-924*j, -1368-1026*j },
                new Complex[] { -1520-1140*j, -1688-1266*j }});
            NumericAssert.AreAlmostEqual(prod_cc2, cc2x2 * cc2x2, "prod cc2 1");
            NumericAssert.AreAlmostEqual(prod_cc2, cc2x2.Multiply(cc2x2), "prod cc2 2");
            ComplexMatrix prod_cc2_inplace = cc2x2.Clone();
            prod_cc2_inplace.MultiplyInplace(cc2x2);
            NumericAssert.AreAlmostEqual(prod_cc2, prod_cc2_inplace, "prod cc2 3");

            // ComplexMatrix * Matrix (Square)
            ComplexMatrix prod_cm2 = new ComplexMatrix(new Complex[][] {
                new Complex[] { 35-105*j, 52-156*j },
                new Complex[] { 43-129*j, 64-192*j }});
            NumericAssert.AreAlmostEqual(prod_cm2, cc2x2 * mc2x2, "prod cm2 1");
            NumericAssert.AreAlmostEqual(prod_cm2, cc2x2.Multiply(mc2x2), "prod cm2 2");
            ComplexMatrix prod_cm2_inplace = cc2x2.Clone();
            prod_cm2_inplace.MultiplyInplace(mc2x2);
            NumericAssert.AreAlmostEqual(prod_cm2, prod_cm2_inplace, "prod cm2 3");

            // ComplexMatrix * Complex (Square)
            ComplexMatrix prod_cs2 = new ComplexMatrix(new Complex[][] {
                new Complex[] { 32-16*j, 36-18*j },
                new Complex[] { 40-20*j, 44-22*j }});
            NumericAssert.AreAlmostEqual(prod_cs2, cc2x2 * s, "prod cs2 1");
            NumericAssert.AreAlmostEqual(prod_cs2, cc2x2.Multiply(s), "prod cs2 2");
            ComplexMatrix prod_cs2_inplace = cc2x2.Clone();
            prod_cs2_inplace.MultiplyInplace(s);
            NumericAssert.AreAlmostEqual(prod_cs2, prod_cs2_inplace, "prod cs2 3");

            // ComplexMatrix * ComplexVector (Column)
            ComplexVector prod_ccv = new ComplexVector(new Complex[] { 42 - 54 * j, 62 + 66 * j, 170 });
            NumericAssert.AreAlmostEqual(prod_ccv, ca3x2 * cv2, "prod ccv 1");
            NumericAssert.AreAlmostEqual(prod_ccv, ca3x2.MultiplyRightColumn(cv2), "prod ccv 2");

            // ComplexMatrix * Vector (Column)
            ComplexVector prod_cv = new ComplexVector(new Complex[] { 30 - 60 * j, -14 + 28 * j, 34 - 68 * j });
            NumericAssert.AreAlmostEqual(prod_cv, ca3x2 * v2, "prod cv 1");
            NumericAssert.AreAlmostEqual(prod_cv, ca3x2.MultiplyRightColumn(v2), "prod cv 2");

            // ComplexMatrix * ComplexVector (Diagonal, Left)
            ComplexMatrix prod_ccvdl = new ComplexMatrix(new Complex[][] {
                new Complex[] { 64-112*j, 72-126*j },
                new Complex[] { 70+90*j, 77+99*j }});
            NumericAssert.AreAlmostEqual(prod_ccvdl, cc2x2.MultiplyLeftDiagonal(cv2), "prod ccv dl 1");
            ComplexMatrix prod_ccvdl_inplace = cc2x2.Clone();
            prod_ccvdl_inplace.MultiplyLeftDiagonalInplace(cv2);
            NumericAssert.AreAlmostEqual(prod_ccvdl, prod_ccvdl_inplace, "prod ccv dl 2");
            NumericAssert.AreAlmostEqual(prod_ccvdl, ComplexMatrix.Diagonal(cv2) * cc2x2, "prod ccv dl 3");

            // ComplexMatrix * Vector (Diagonal, Left)
            ComplexMatrix prod_cvdl = new ComplexMatrix(new Complex[][] {
                new Complex[] { 40-120*j, 45-135*j },
                new Complex[] { -20+60*j, -22+66*j }});
            NumericAssert.AreAlmostEqual(prod_cvdl, cc2x2.MultiplyLeftDiagonal(v2), "prod cv dl 1");
            ComplexMatrix prod_cvdl_inplace = cc2x2.Clone();
            prod_cvdl_inplace.MultiplyLeftDiagonalInplace(v2);
            NumericAssert.AreAlmostEqual(prod_cvdl, prod_cvdl_inplace, "prod cv dl 2");

            // ComplexMatrix * ComplexVector (Diagonal, Right)
            ComplexMatrix prod_ccvdr = new ComplexMatrix(new Complex[][] {
                new Complex[] { 64-112*j, 63+81*j },
                new Complex[] { 80-140*j, 77+99*j }});
            NumericAssert.AreAlmostEqual(prod_ccvdr, cc2x2.MultiplyRightDiagonal(cv2), "prod ccv dr 1");
            ComplexMatrix prod_ccvdr_inplace = cc2x2.Clone();
            prod_ccvdr_inplace.MultiplyRightDiagonalInplace(cv2);
            NumericAssert.AreAlmostEqual(prod_ccvdr, prod_ccvdr_inplace, "prod ccv dr 2");
            NumericAssert.AreAlmostEqual(prod_ccvdr, cc2x2 * ComplexMatrix.Diagonal(cv2), "prod ccv dr 3");

            // ComplexMatrix * Vector (Diagonal, Right)
            ComplexMatrix prod_cvdr = new ComplexMatrix(new Complex[][] {
                new Complex[] { 40-120*j, -18+54*j },
                new Complex[] { 50-150*j, -22+66*j }});
            NumericAssert.AreAlmostEqual(prod_cvdr, cc2x2.MultiplyRightDiagonal(v2), "prod cv dr 1");
            ComplexMatrix prod_cvdr_inplace = cc2x2.Clone();
            prod_cvdr_inplace.MultiplyRightDiagonalInplace(v2);
            NumericAssert.AreAlmostEqual(prod_cvdr, prod_cvdr_inplace, "prod cv dr 2");
        }
    }
}
