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

using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics;

namespace Iridium.Test
{
    /// <summary>TestMatrix tests the functionality of the 
    /// Matrix class and associated decompositions.</summary>
    /// <remarks>
    /// Detailed output is provided indicating the functionality being tested
    /// and whether the functionality is correctly implemented. Exception handling
    /// is also tested.<br/>
    /// 
    /// The test is designed to run to completion and give a summary of any implementation errors
    /// encountered. The final output should be:
    /// <BLOCKQUOTE><PRE><CODE>
    /// TestMatrix completed.
    /// Total errors reported: n1
    /// Total warning reported: n2
    /// </CODE></PRE></BLOCKQUOTE>
    /// If the test does not run to completion, this indicates that there is a 
    /// substantial problem within the implementation that was not anticipated in the test design.  
    /// The stopping point should give an indication of where the problem exists.
    /// </remarks>
    [TestFixture]
    public class LinearAlgebraTests
    {
        private static Random random = new Random();

        [Test]
        public void MultiplyByDiagonal()
        {
            Matrix A = Matrix.Create(
                new double[3, 4] {
                    {1, 2, 3, 4},
                    {3, 4, 5, 6},
                    {5, 6, 7, 8}
                    });

            double[] diagonal = new double[3] { 0, 1, 2 };

            A.Multiply(diagonal);

            Assert.AreEqual(0, A[0, 0], "#A00");
            Assert.AreEqual(0, A[0, 1], "#A01");
            Assert.AreEqual(3, A[1, 0], "#A02");
            Assert.AreEqual(4, A[1, 1], "#A03");
            Assert.AreEqual(10, A[2, 0], "#A04");
            Assert.AreEqual(12, A[2, 1], "#A05");
        }

        [Test]
        public void MultiplyByMatrix()
        {

            Matrix A = Matrix.Create(
                new double[3, 4] {
                    {10, -61, -8, -29},
                    {95, 11, -49, -47},
                    {40, -81, 91, 68}
                    });

            Matrix B = Matrix.Create(
                new double[4, 2] {
                    {72, 37},
                    {-23, 87},
                    {44, 29},
                    {98, -23}
                    });

            Matrix C = Matrix.Create(
                new double[3, 2] {
                    {-1071, -4502},
                    { -175, 4132},
                    {15411, -4492}
                    });

            Matrix P = A.Multiply(B);

            Assert.AreEqual(C.ColumnCount, P.ColumnCount, "#A00 Invalid column count in linear product.");
            Assert.AreEqual(C.RowCount, P.RowCount, "#A01 Invalid row count in linear product.");

            for(int i = 0; i < C.RowCount; i++)
                for(int j = 0; j < C.ColumnCount; j++)
                {
                    Assert.AreEqual(C[i, j], P[i, j], "#A02 Unexpected product value.");
                }
        }


        [Test]
        public void SolveRobust()
        {

            Matrix A1 = Matrix.Create(
                new double[6, 2] {
                    {1, 1},
                    {1, 2},
                    {1, 2},
                    {1, -1},
                    {0, 1},
                    {2, 1}
                    });

            Matrix B1 = Matrix.Create(
                new double[6, 1] {
                    {2},
                    {2},
                    {2},
                    {2},
                    {2},
                    {2}
                    });

            Matrix X1 = A1.SolveRobust(B1);

            // [vermorel] Values have been computed with LAD function of Systat 12
            Assert.AreEqual(1.2, X1[0, 0], 1.0e-3, "#A00 Unexpected robust regression result.");
            Assert.AreEqual(0.4, X1[1, 0], 1.0e-3, "#A01 Unexpected robust regression result.");


            Matrix A2 = Matrix.Create(
                new double[6, 3] {
                    {2, -1, 2},
                    {3, 2, 0},
                    {1, 2, 4},
                    {1, -1, -1},
                    {0, 1, 2},
                    {2, 1, 1}
                    });

            Matrix B2 = Matrix.Create(
                new double[6, 1] {
                    {0},
                    {4},
                    {2},
                    {-3},
                    {2},
                    {1}
                    });

            Matrix X2 = A2.SolveRobust(B2);

            // [vermorel] Values have been computed with LAD function of Systat 12
            Assert.AreEqual(0.667, X2[0, 0], 1.0e-3, "#A02 Unexpected robust regression result.");
            Assert.AreEqual(1.0, X2[1, 0], 1.0e-3, "#A03 Unexpected robust regression result.");
            Assert.AreEqual(-0.167, X2[2, 0], 1.0e-3, "#A04 Unexpected robust regression result.");


            Matrix A3 = Matrix.Create(
                new double[10, 4] {
                    {-8, -29, 95, 11},
                    {-47, 40, -81, 91},
                    {-10, 31, -51, 77},
                    {1, 1, 55, -28},
                    {30, -27, -15, -59},
                    {72, -87, 47, -90},
                    {92, -91, -88, -48},
                    {-28, 5, 13, -10},
                    {71, 16, 83, 9 },
                    {-83, 98, -48, -19}
                    });

            Matrix B3 = Matrix.Create(
                new double[10, 1] {
                    {-49},
                    {68},
                    {95},
                    {16},
                    {-96},
                    {43},
                    {53},
                    {-82},
                    {-60},
                    {62}
                    });

            Matrix X3 = A3.SolveRobust(B3);

            // [vermorel] Values have been computed with LAD function of Systat 12
            Assert.AreEqual(-0.104, X3[0, 0], 1.0e-3, "#A05 Unexpected robust regression result.");
            Assert.AreEqual(-0.216, X3[1, 0], 1.0e-3, "#A06 Unexpected robust regression result.");
            Assert.AreEqual(-0.618, X3[2, 0], 1.0e-3, "#A07 Unexpected robust regression result.");
            Assert.AreEqual(0.238, X3[3, 0], 1.0e-3, "#A08 Unexpected robust regression result.");
        }

        /// <summary>
        /// Testing the method <see cref="Matrix.SVD"/>.
        /// </summary>
        [Test]
        public void SingularValueDecomposition()
        {
            for(int k = 0; k < 20; k++)
            {
                Matrix matrix = Matrix.Random(10, 8 + random.Next(5));

                SingularValueDecomposition svd = matrix.SingularValueDecomposition;

                Matrix U = svd.LeftSingularVectors;
                Matrix Vt = svd.RightSingularVectors; Vt.Transpose();
                Matrix product = U * svd.S * Vt;

                for(int i = 0; i < matrix.RowCount; i++)
                    for(int j = 0; j < matrix.ColumnCount; j++)
                        NumericAssert.AreAlmostEqual(matrix[i, j], product[i, j], 1e-10, "#A00");
            }
        }

        // TODO: rewrite AllTests in a more NUnit style

        /// <summary>An exception is thrown at the end of the process, 
        /// if any error is encountered.</summary>
        [Test]
        public void AllTests()
        {
            Matrix A, B, C, Z, O, I, R, S, X, SUB, M, T, SQ, DEF, SOL;
            double tmp;
            double[] columnwise = { 1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0 };
            double[] rowwise = { 1.0, 4.0, 7.0, 10.0, 2.0, 5.0, 8.0, 11.0, 3.0, 6.0, 9.0, 12.0 };
            double[][] avals = { new double[] { 1.0, 4.0, 7.0, 10.0 }, new double[] { 2.0, 5.0, 8.0, 11.0 }, new double[] { 3.0, 6.0, 9.0, 12.0 } };
            double[][] rankdef = avals;
            double[][] tvals = { new double[] { 1.0, 2.0, 3.0 }, new double[] { 4.0, 5.0, 6.0 }, new double[] { 7.0, 8.0, 9.0 }, new double[] { 10.0, 11.0, 12.0 } };
            double[][] subavals = { new double[] { 5.0, 8.0, 11.0 }, new double[] { 6.0, 9.0, 12.0 } };
            double[][] pvals = { new double[] { 25, -5, 10 }, new double[] { -5, 17, 10 }, new double[] { 10, 10, 62 } };
            double[][] ivals = { new double[] { 1.0, 0.0, 0.0, 0.0 }, new double[] { 0.0, 1.0, 0.0, 0.0 }, new double[] { 0.0, 0.0, 1.0, 0.0 } };
            double[][] evals = { new double[] { 0.0, 1.0, 0.0, 0.0 }, new double[] { 1.0, 0.0, 2e-7, 0.0 }, new double[] { 0.0, -2e-7, 0.0, 1.0 }, new double[] { 0.0, 0.0, 1.0, 0.0 } };
            double[][] square = { new double[] { 166.0, 188.0, 210.0 }, new double[] { 188.0, 214.0, 240.0 }, new double[] { 210.0, 240.0, 270.0 } };
            double[][] sqSolution = { new double[] { 13.0 }, new double[] { 15.0 } };
            double[][] condmat = { new double[] { 1.0, 3.0 }, new double[] { 7.0, 9.0 } };
            int rows = 3, cols = 4;
            int invalidld = 5; /* should trigger bad shape for construction with val */
            int validld = 3; /* leading dimension of intended test Matrices */
            int nonconformld = 4; /* leading dimension which is valid, but nonconforming */
            int ib = 1, ie = 2, jb = 1, je = 3; /* index ranges for sub Matrix */
            int[] rowindexset = new int[] { 1, 2 };
            int[] badrowindexset = new int[] { 1, 3 };
            int[] columnindexset = new int[] { 1, 2, 3 };
            int[] badcolumnindexset = new int[] { 1, 2, 4 };
            double columnsummax = 33.0;
            double rowsummax = 30.0;
            double sumofdiagonals = 15;
            double sumofsquares = 650;

            #region Testing constructors and constructor-like methods

            // Constructors and constructor-like methods:
            // double[], int
            // double[,]  
            // int, int
            // int, int, double
            // int, int, double[,]
            // Create(double[,])
            // Random(int,int)
            // Identity(int)

            try
            {
                // check that exception is thrown in packed constructor with invalid length
                A = new Matrix(columnwise, invalidld);
                Assert.Fail("Catch invalid length in packed constructor: exception not thrown for invalid input");
            }
            catch(ArgumentException) { }

            A = new Matrix(columnwise, validld);
            B = new Matrix(avals);
            tmp = B[0, 0];
            avals[0][0] = 0.0;
            C = B - A;
            avals[0][0] = tmp;
            B = Matrix.Create(avals);
            tmp = B[0, 0];
            avals[0][0] = 0.0;
            Assert.AreEqual((tmp - B[0, 0]), 0.0, "Create");

            avals[0][0] = columnwise[0];
            I = new Matrix(ivals);
            NumericAssert.AreAlmostEqual(I, Matrix.Identity(3, 4), "Identity");

            #endregion

            #region Testing access methods

            // Access Methods:
            // getColumnDimension()
            // getRowDimension()
            // getArray()
            // getArrayCopy()
            // getColumnPackedCopy()
            // getRowPackedCopy()
            // get(int,int)
            // GetMatrix(int,int,int,int)
            // GetMatrix(int,int,int[])
            // GetMatrix(int[],int,int)
            // GetMatrix(int[],int[])
            // set(int,int,double)
            // SetMatrix(int,int,int,int,Matrix)
            // SetMatrix(int,int,int[],Matrix)
            // SetMatrix(int[],int,int,Matrix)
            // SetMatrix(int[],int[],Matrix)

            // Various get methods
            B = new Matrix(avals);
            Assert.AreEqual(B.RowCount, rows, "getRowDimension");
            Assert.AreEqual(B.ColumnCount, cols, "getColumnDimension");

            B = new Matrix(avals);
            double[][] barray = (Matrix)B;
            Assert.AreSame(barray, avals, "getArray");

            barray = B.Clone();
            Assert.AreNotSame(barray, avals, "getArrayCopy");
            NumericAssert.AreAlmostEqual(new Matrix(barray), B, "getArrayCopy II");

            //            double[] bpacked = B.ColumnPackedCopy;
            //            try
            //            {
            //                check(bpacked, columnwise);
            //                try_success("getColumnPackedCopy... ", "");
            //            }
            //            catch (System.SystemException e)
            //            {
            //                errorCount = try_failure(errorCount, "getColumnPackedCopy... ", "data not successfully (deep) copied by columns");
            //                System.Console.Out.WriteLine(e.Message);
            //            }
            //            bpacked = B.RowPackedCopy;
            //            try
            //            {
            //                check(bpacked, rowwise);
            //                try_success("getRowPackedCopy... ", "");
            //            }
            //            catch (System.SystemException e)
            //            {
            //                errorCount = try_failure(errorCount, "getRowPackedCopy... ", "data not successfully (deep) copied by rows");
            //                System.Console.Out.WriteLine(e.Message);
            //            }

            try
            {
                tmp = B[B.RowCount, B.ColumnCount - 1];
                Assert.Fail("get(int,int): OutOfBoundsException expected but not thrown");
            }
            catch(IndexOutOfRangeException) { }
            try
            {
                tmp = B[B.RowCount - 1, B.ColumnCount];
                Assert.Fail("get(int,int): OutOfBoundsException expected but not thrown II");
            }
            catch(IndexOutOfRangeException) { }
            Assert.AreEqual(B[B.RowCount - 1, B.ColumnCount - 1], avals[B.RowCount - 1][B.ColumnCount - 1], "get(int,int)");

            SUB = new Matrix(subavals);
            try
            {
                M = B.GetMatrix(ib, ie + B.RowCount + 1, jb, je);
                Assert.Fail("GetMatrix(int,int,int,int): IndexOutOfBoundsException expected but not thrown");
            }
            catch(IndexOutOfRangeException) { }
            try
            {
                M = B.GetMatrix(ib, ie, jb, je + B.ColumnCount + 1);
                Assert.Fail("GetMatrix(int,int,int,int): IndexOutOfBoundsException expected but not thrown II");
            }
            catch(IndexOutOfRangeException) { }

            M = B.GetMatrix(ib, ie, jb, je);
            NumericAssert.AreAlmostEqual(SUB, M, "GetMatrix(int,int,int,int)");

            try
            {
                M = B.GetMatrix(ib, ie, badcolumnindexset);
                Assert.Fail("GetMatrix(int,int,int[]): IndexOutOfBoundsException expected but not thrown");
            }
            catch(IndexOutOfRangeException) { }
            try
            {
                M = B.GetMatrix(ib, ie + B.RowCount + 1, columnindexset);
                Assert.Fail("GetMatrix(int,int,int[]): IndexOutOfBoundsException expected but not thrown II");
            }
            catch(IndexOutOfRangeException) { }

            M = B.GetMatrix(ib, ie, columnindexset);
            NumericAssert.AreAlmostEqual(SUB, M, "GetMatrix(int,int,int[])");

            try
            {
                M = B.GetMatrix(badrowindexset, jb, je);
                Assert.Fail("GetMatrix(int[],int,int): IndexOutOfBoundsException expected but not thrown");
            }
            catch(IndexOutOfRangeException) { }
            try
            {
                M = B.GetMatrix(rowindexset, jb, je + B.ColumnCount + 1);
                Assert.Fail("GetMatrix(int[],int,int): IndexOutOfBoundsException expected but not thrown II");
            }
            catch(IndexOutOfRangeException) { }

            M = B.GetMatrix(rowindexset, jb, je);
            NumericAssert.AreAlmostEqual(SUB, M, "GetMatrix(int[],int,int)");

            try
            {
                M = B.GetMatrix(badrowindexset, columnindexset);
                Assert.Fail("GetMatrix(int[],int[]): IndexOutOfBoundsException expected but not thrown");
            }
            catch(IndexOutOfRangeException) { }
            try
            {
                M = B.GetMatrix(rowindexset, badcolumnindexset);
                Assert.Fail("GetMatrix(int[],int[]): IndexOutOfBoundsException expected but not thrown II");
            }
            catch(IndexOutOfRangeException) { }

            M = B.GetMatrix(rowindexset, columnindexset);
            NumericAssert.AreAlmostEqual(SUB, M, "GetMatrix(int[],int[])");

            // Various set methods:
            try
            {
                B[B.RowCount, B.ColumnCount - 1] = 0.0;
                Assert.Fail("set(int,int,double): IndexOutOfBoundsException expected but not thrown");
            }
            catch(IndexOutOfRangeException) { }
            try
            {
                B[B.RowCount - 1, B.ColumnCount] = 0.0;
                Assert.Fail("set(int,int,double): IndexOutOfBoundsException expected but not thrown II");
            }
            catch(IndexOutOfRangeException) { }

            B[ib, jb] = 0.0;
            tmp = B[ib, jb];
            NumericAssert.AreAlmostEqual(tmp, 0.0, "set(int,int,double)");

            M = new Matrix(2, 3, 0.0);
            try
            {
                B.SetMatrix(ib, ie + B.RowCount + 1, jb, je, M);
                Assert.Fail("SetMatrix(int,int,int,int,Matrix): IndexOutOfBoundsException expected but not thrown");
            }
            catch(IndexOutOfRangeException) { }
            try
            {
                B.SetMatrix(ib, ie, jb, je + B.ColumnCount + 1, M);
                Assert.Fail("SetMatrix(int,int,int,int,Matrix): IndexOutOfBoundsException expected but not thrown II");
            }
            catch(IndexOutOfRangeException) { }

            B.SetMatrix(ib, ie, jb, je, M);
            NumericAssert.AreAlmostEqual(M - B.GetMatrix(ib, ie, jb, je), M, "SetMatrix(int,int,int,int,Matrix)");
            B.SetMatrix(ib, ie, jb, je, SUB);
            try
            {
                B.SetMatrix(ib, ie + B.RowCount + 1, columnindexset, M);
                Assert.Fail("SetMatrix(int,int,int[],Matrix): IndexOutOfBoundsException expected but not thrown");
            }
            catch(IndexOutOfRangeException) { }
            try
            {
                B.SetMatrix(ib, ie, badcolumnindexset, M);
                Assert.Fail("SetMatrix(int,int,int[],Matrix): IndexOutOfBoundsException expected but not thrown II");
            }
            catch(IndexOutOfRangeException) { }

            B.SetMatrix(ib, ie, columnindexset, M);
            NumericAssert.AreAlmostEqual(M - B.GetMatrix(ib, ie, columnindexset), M, "SetMatrix(int,int,int[],Matrix)");
            B.SetMatrix(ib, ie, jb, je, SUB);
            try
            {
                B.SetMatrix(rowindexset, jb, je + B.ColumnCount + 1, M);
                Assert.Fail("SetMatrix(int[],int,int,Matrix): IndexOutOfBoundsException expected but not thrown");
            }
            catch(IndexOutOfRangeException) { }
            try
            {
                B.SetMatrix(badrowindexset, jb, je, M);
                Assert.Fail("SetMatrix(int[],int,int,Matrix): IndexOutOfBoundsException expected but not thrown II");
            }
            catch(IndexOutOfRangeException) { }

            B.SetMatrix(rowindexset, jb, je, M);
            NumericAssert.AreAlmostEqual(M - B.GetMatrix(rowindexset, jb, je), M, "SetMatrix(int[],int,int,Matrix)");

            B.SetMatrix(ib, ie, jb, je, SUB);
            try
            {
                B.SetMatrix(rowindexset, badcolumnindexset, M);
                Assert.Fail("SetMatrix(int[],int[],Matrix): IndexOutOfBoundsException expected but not thrown");
            }
            catch(IndexOutOfRangeException) { }
            try
            {
                B.SetMatrix(badrowindexset, columnindexset, M);
                Assert.Fail("SetMatrix(int[],int[],Matrix): IndexOutOfBoundsException expected but not thrown");
            }
            catch(IndexOutOfRangeException) { }

            B.SetMatrix(rowindexset, columnindexset, M);
            NumericAssert.AreAlmostEqual(M - B.GetMatrix(rowindexset, columnindexset), M, "SetMatrix(int[],int[],Matrix)");

            #endregion

            #region Testing array-like methods

            // Array-like methods:
            // Subtract
            // SubtractEquals
            // Add
            // AddEquals
            // ArrayLeftDivide
            // ArrayLeftDivideEquals
            // ArrayRightDivide
            // ArrayRightDivideEquals
            // arrayTimes
            // ArrayMultiplyEquals
            // uminus

            S = new Matrix(columnwise, nonconformld);
            R = Matrix.Random(A.RowCount, A.ColumnCount);
            A = R;
            try
            {
                S = A - S;
                Assert.Fail("Subtract conformance check: nonconformance not raised");
            }
            catch(ArgumentException) { }
            Assert.AreEqual((A - R).Norm1(), 0.0, "Subtract: difference of identical Matrices is nonzero,\nSubsequent use of Subtract should be suspect");

            A = R.Clone();
            A.Subtract(R);
            Z = new Matrix(A.RowCount, A.ColumnCount);
            try
            {
                A.Subtract(S);
                Assert.Fail("SubtractEquals conformance check: nonconformance not raised");
            }
            catch(ArgumentException) { }
            Assert.AreEqual((A - Z).Norm1(), 0.0, "SubtractEquals: difference of identical Matrices is nonzero,\nSubsequent use of Subtract should be suspect");

            A = R.Clone();
            B = Matrix.Random(A.RowCount, A.ColumnCount);
            C = A - B;
            try
            {
                S = A + S;
                Assert.Fail("Add conformance check: nonconformance not raised");
            }
            catch(ArgumentException) { }
            NumericAssert.AreAlmostEqual(C + B, A, "Add");

            C = A - B;
            C.Add(B);
            try
            {
                A.Add(S);
                Assert.Fail("AddEquals conformance check: nonconformance not raised");
            }
            catch(ArgumentException) { }
            NumericAssert.AreAlmostEqual(C, A, "AddEquals");

            A = ((Matrix)R.Clone());
            A.UnaryMinus();
            NumericAssert.AreAlmostEqual(A + R, Z, "UnaryMinus");

            A = (Matrix)R.Clone();
            O = new Matrix(A.RowCount, A.ColumnCount, 1.0);
            try
            {
                Matrix.ArrayDivide(A, S);
                Assert.Fail("ArrayRightDivide conformance check: nonconformance not raised");
            }
            catch(ArgumentException) { }

            C = Matrix.ArrayDivide(A, R);
            NumericAssert.AreAlmostEqual(C, O, "ArrayRightDivide");
            try
            {
                A.ArrayDivide(S);
                Assert.Fail("ArrayRightDivideEquals conformance check: nonconformance not raised");
            }
            catch(ArgumentException) { }

            A.ArrayDivide(R);
            NumericAssert.AreAlmostEqual(A, O, "ArrayRightDivideEquals");

            A = (Matrix)R.Clone();
            B = Matrix.Random(A.RowCount, A.ColumnCount);
            try
            {
                S = Matrix.ArrayMultiply(A, S);
                Assert.Fail("arrayTimes conformance check: nonconformance not raised");
            }
            catch(ArgumentException) { }

            C = Matrix.ArrayMultiply(A, B);
            C.ArrayDivide(B);
            NumericAssert.AreAlmostEqual(C, A, "arrayTimes");
            try
            {
                A.ArrayMultiply(S);
                Assert.Fail("ArrayMultiplyEquals conformance check: nonconformance not raised");
            }
            catch(ArgumentException) { }

            A.ArrayMultiply(B);
            A.ArrayDivide(B);
            NumericAssert.AreAlmostEqual(A, R, "ArrayMultiplyEquals");

            #endregion

            #region Testing linear algebra methods

            // LA methods:
            // Transpose
            // Multiply
            // Condition
            // Rank
            // Determinant
            // trace
            // Norm1
            // norm2
            // normF
            // normInf
            // Solve
            // solveTranspose
            // Inverse
            // chol
            // Eigen
            // lu
            // qr
            // svd 

            A = new Matrix(columnwise, 3);
            T = new Matrix(tvals);
            T = Matrix.Transpose(A);
            NumericAssert.AreAlmostEqual(Matrix.Transpose(A), T, "Transpose");
            NumericAssert.AreAlmostEqual(A.Norm1(), columnsummax, "Norm1");
            NumericAssert.AreAlmostEqual(A.NormInf(), rowsummax, "NormInf");
            NumericAssert.AreAlmostEqual(A.NormF(), Math.Sqrt(sumofsquares), "NormF");
            NumericAssert.AreAlmostEqual(A.Trace(), sumofdiagonals, "Trace");
            NumericAssert.AreAlmostEqual(A.GetMatrix(0, A.RowCount - 1, 0, A.RowCount - 1).Determinant(), 0.0, "Determinant");

            SQ = new Matrix(square);
            NumericAssert.AreAlmostEqual(A * Matrix.Transpose(A), SQ, "Multiply(Matrix)");
            NumericAssert.AreAlmostEqual(0.0 * A, Z, "Multiply(double)");

            A = new Matrix(columnwise, 4);
            QRDecomposition QR = A.QRDecomposition;
            R = QR.R;
            NumericAssert.AreAlmostEqual(A, QR.Q * R, "QRDecomposition");

            SingularValueDecomposition SVD = A.SingularValueDecomposition;
            NumericAssert.AreAlmostEqual(A, SVD.LeftSingularVectors * (SVD.S * Matrix.Transpose(SVD.RightSingularVectors)), "SingularValueDecomposition");

            DEF = new Matrix(rankdef);
            NumericAssert.AreAlmostEqual(DEF.Rank(), Math.Min(DEF.RowCount, DEF.ColumnCount) - 1, "Rank");

            B = new Matrix(condmat);
            SVD = B.SingularValueDecomposition;
            double[] singularvalues = SVD.SingularValues;
            NumericAssert.AreAlmostEqual(B.Condition(), singularvalues[0] / singularvalues[Math.Min(B.RowCount, B.ColumnCount) - 1], "Condition");

            int n = A.ColumnCount;
            A = A.GetMatrix(0, n - 1, 0, n - 1);
            A[0, 0] = 0.0;
            LUDecomposition LU = A.LUDecomposition;
            NumericAssert.AreAlmostEqual(A.GetMatrix(LU.Pivot, 0, n - 1), LU.L * LU.U, "LUDecomposition");

            X = A.Inverse();
            NumericAssert.AreAlmostEqual(A * X, Matrix.Identity(3, 3), "Inverse");

            O = new Matrix(SUB.RowCount, 1, 1.0);
            SOL = new Matrix(sqSolution);
            SQ = SUB.GetMatrix(0, SUB.RowCount - 1, 0, SUB.RowCount - 1);
            NumericAssert.AreAlmostEqual(SQ.Solve(SOL), O, "Solve");

            A = new Matrix(pvals);
            CholeskyDecomposition Chol = A.CholeskyDecomposition;
            Matrix L = Chol.TriangularFactor;
            NumericAssert.AreAlmostEqual(A, L * Matrix.Transpose(L), "CholeskyDecomposition");

            X = Chol.Solve(Matrix.Identity(3, 3));
            NumericAssert.AreAlmostEqual(A * X, Matrix.Identity(3, 3), "CholeskyDecomposition Solve");

            EigenvalueDecomposition Eig = A.EigenvalueDecomposition;
            Matrix D = Eig.BlockDiagonal;
            Matrix V = Eig.EigenVectors;
            NumericAssert.AreAlmostEqual(A * V, V * D, "EigenvalueDecomposition (symmetric)");

            A = new Matrix(evals);
            Eig = A.EigenvalueDecomposition;
            D = Eig.BlockDiagonal;
            V = Eig.EigenVectors;
            NumericAssert.AreAlmostEqual(A * V, V * D, "EigenvalueDecomposition (nonsymmetric)");

            #endregion
        }
    }
}
