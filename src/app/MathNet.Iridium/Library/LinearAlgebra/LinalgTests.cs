using System;

using MathNet.Numerics.LinearAlgebra;

//namespace MathNet.Numerics.LinearAlgebra.test
//{
//    /// <summary>TestMatrix tests the functionality of the 
//    /// Matrix class and associated decompositions.</summary>
//    /// <remarks>
//    /// Detailed output is provided indicating the functionality being tested
//    /// and whether the functionality is correctly implemented. Exception handling
//    /// is also tested.<br/>
//    /// 
//    /// The test is designed to run to completion and give a summary of any implementation errors
//    /// encountered. The final output should be:
//    /// <BLOCKQUOTE><PRE><CODE>
//    /// TestMatrix completed.
//    /// Total errors reported: n1
//    /// Total warning reported: n2
//    /// </CODE></PRE></BLOCKQUOTE>
//    /// If the test does not run to completion, this indicates that there is a 
//    /// substantial problem within the implementation that was not anticipated in the test design.  
//    /// The stopping point should give an indication of where the problem exists.
//    /// </remarks>
//    [TestFixture]
//    public class LinalgTests
//    {
//        // TODO: rewrite AllTests in a more NUnit style

//        private static Random random = new Random();

//        /// <summary>
//        /// Testing the method <see cref="Matrix.SVD"/>.
//        /// </summary>
//        [Test] public void SingularValueDecomposition()
//        {
//            for(int k = 0; k < 20; k++)
//            {
//                Matrix matrix = Matrix.Random(10, 8 + random.Next(5));

//                SingularValueDecomposition svd = matrix.SVD();

//                Matrix U = svd.LeftSingularVectors;
//                Matrix Vt = svd.RightSingularVectors; Vt.Transpose();
//                Matrix product = U * svd.S * Vt;

//                for(int i = 0; i < matrix.RowCount; i++)
//                    for(int j = 0; j < matrix.ColumnCount; j++)
//                        Assertion.AssertEquals("#A00", matrix[i,j], product[i,j], 1e-6);
//            }
//        }

//        /// <summary>An exception is thrown at the end of the process, 
//        /// if any error is encountered.</summary>
//        [Test] public void AllTests()
//        {
//            Matrix A, B, C, Z, O, I, R, S, X, SUB, M, T, SQ, DEF, SOL;
//            int errorCount = 0;
//            int warningCount = 0;
//            double tmp;
//            double[] columnwise = {1.0, 2.0, 3.0, 4.0, 5.0, 6.0, 7.0, 8.0, 9.0, 10.0, 11.0, 12.0};
//            double[] rowwise = {1.0, 4.0, 7.0, 10.0, 2.0, 5.0, 8.0, 11.0, 3.0, 6.0, 9.0, 12.0};
//            double[,] avals = {{1.0, 4.0, 7.0, 10.0}, {2.0, 5.0, 8.0, 11.0}, {3.0, 6.0, 9.0, 12.0}};
//            double[,] rankdef = avals;
//            double[,] tvals = {{1.0, 2.0, 3.0}, {4.0, 5.0, 6.0}, {7.0, 8.0, 9.0}, {10.0, 11.0, 12.0}};
//            double[,] subavals = {{5.0, 8.0, 11.0}, {6.0, 9.0, 12.0}};
//            double[,] pvals = {{1.0, 1.0, 1.0}, {1.0, 2.0, 3.0}, {1.0, 3.0, 6.0}};
//            double[,] ivals = {{1.0, 0.0, 0.0, 0.0}, {0.0, 1.0, 0.0, 0.0}, {0.0, 0.0, 1.0, 0.0}};
//            double[,] evals = {{0.0, 1.0, 0.0, 0.0}, {1.0, 0.0, 2e-7, 0.0}, {0.0, - 2e-7, 0.0, 1.0}, {0.0, 0.0, 1.0, 0.0}};
//            double[,] square = {{166.0, 188.0, 210.0}, {188.0, 214.0, 240.0}, {210.0, 240.0, 270.0}};
//            double[,] sqSolution = {{13.0}, {15.0}};
//            double[,] condmat = {{1.0, 3.0}, {7.0, 9.0}};
//            int rows = 3, cols = 4;
//            int invalidld = 5; /* should trigger bad shape for construction with val */
//            int validld = 3; /* leading dimension of intended test Matrices */
//            int nonconformld = 4; /* leading dimension which is valid, but nonconforming */
//            int ib = 1, ie = 2, jb = 1, je = 3; /* index ranges for sub Matrix */
//            int[] rowindexset = new int[]{1, 2};
//            int[] badrowindexset = new int[]{1, 3};
//            int[] columnindexset = new int[]{1, 2, 3};
//            int[] badcolumnindexset = new int[]{1, 2, 4};
//            double columnsummax = 33.0;
//            double rowsummax = 30.0;
//            double sumofdiagonals = 15;
//            double sumofsquares = 650;
			
//            // Constructors and constructor-like methods:
//            // double[], int
//            // double[,]  
//            // int, int
//            // int, int, double
//            // int, int, double[,]
//            // Create(double[,])
//            // Random(int,int)
//            // Identity(int)

//            print("\nTesting constructors and constructor-like methods...\n");
//            try
//            {
//                // check that exception is thrown in packed constructor with invalid length
//                A = new Matrix(columnwise, invalidld);
//                errorCount = try_failure(errorCount, "Catch invalid length in packed constructor... ", "exception not thrown for invalid input");
//            }
//            catch (System.ArgumentException e)
//            {
//                try_success("Catch invalid length in packed constructor... ", e.Message);
//            }
			
//            A = new Matrix(columnwise, validld);
//            B = new Matrix(avals);
//            tmp = B[0,0];
//            avals[0, 0] = 0.0;
//            C = B - A;
//            avals[0, 0] = tmp;
//            B = Matrix.Create(avals);
//            tmp = B[0, 0];
//            avals[0, 0] = 0.0;
//            if ((tmp - B[0, 0]) != 0.0)
//            {
//                // check that Create behaves properly
//                errorCount = try_failure(errorCount, "Create... ", "Copy not effected... data visible outside");
//            }
//            else
//            {
//                try_success("Create... ", "");
//            }
//            avals[0, 0] = columnwise[0];
//            I = new Matrix(ivals);
//            try
//            {
//                check(I, Matrix.Identity(3, 4));
//                try_success("Identity... ", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "Identity... ", "Identity Matrix not successfully created");
//                System.Console.Out.WriteLine(e.Message);
//            }
			
//            // Access Methods:
//            // getColumnDimension()
//            // getRowDimension()
//            // getArray()
//            // getArrayCopy()
//            // getColumnPackedCopy()
//            // getRowPackedCopy()
//            // get(int,int)
//            // GetMatrix(int,int,int,int)
//            // GetMatrix(int,int,int[])
//            // GetMatrix(int[],int,int)
//            // GetMatrix(int[],int[])
//            // set(int,int,double)
//            // SetMatrix(int,int,int,int,Matrix)
//            // SetMatrix(int,int,int[],Matrix)
//            // SetMatrix(int[],int,int,Matrix)
//            // SetMatrix(int[],int[],Matrix)
			
//            print("\nTesting access methods...\n");
			
//            // Various get methods
//            B = new Matrix(avals);
//            if (B.RowCount != rows)
//            {
//                errorCount = try_failure(errorCount, "getRowDimension... ", "");
//            }
//            else
//            {
//                try_success("getRowDimension... ", "");
//            }
//            if (B.ColumnCount != cols)
//            {
//                errorCount = try_failure(errorCount, "getColumnDimension... ", "");
//            }
//            else
//            {
//                try_success("getColumnDimension... ", "");
//            }
//            B = new Matrix(avals);
//            double[,] barray = (Matrix) B;
//            if (barray != avals)
//            {
//                errorCount = try_failure(errorCount, "getArray... ", "");
//            }
//            else
//            {
//                try_success("getArray... ", "");
//            }
//            barray = (Matrix) B.Clone();
//            if (barray == avals)
//            {
//                errorCount = try_failure(errorCount, "getArrayCopy... ", "data not (deep) copied");
//            }
//            try
//            {
//                check(barray, avals);
//                try_success("getArrayCopy... ", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "getArrayCopy... ", "data not successfully (deep) copied");
//                System.Console.Out.WriteLine(e.Message);
//            }

////			double[] bpacked = B.ColumnPackedCopy;
////			try
////			{
////				check(bpacked, columnwise);
////				try_success("getColumnPackedCopy... ", "");
////			}
////			catch (System.SystemException e)
////			{
////				errorCount = try_failure(errorCount, "getColumnPackedCopy... ", "data not successfully (deep) copied by columns");
////				System.Console.Out.WriteLine(e.Message);
////			}
////			bpacked = B.RowPackedCopy;
////			try
////			{
////				check(bpacked, rowwise);
////				try_success("getRowPackedCopy... ", "");
////			}
////			catch (System.SystemException e)
////			{
////				errorCount = try_failure(errorCount, "getRowPackedCopy... ", "data not successfully (deep) copied by rows");
////				System.Console.Out.WriteLine(e.Message);
////			}
//            try
//            {
//                tmp = B[B.RowCount, B.ColumnCount - 1];
//                errorCount = try_failure(errorCount, "get(int,int)... ", "OutOfBoundsException expected but not thrown");
//            }
//            catch (System.IndexOutOfRangeException e)
//            {
//                System.Console.Out.WriteLine(e.Message);
//                try
//                {
//                    tmp = B[B.RowCount - 1, B.ColumnCount];
//                    errorCount = try_failure(errorCount, "get(int,int)... ", "OutOfBoundsException expected but not thrown");
//                }
//                catch (System.IndexOutOfRangeException e1)
//                {
//                    try_success("get(int,int)... OutofBoundsException... ", "");
//                    System.Console.Out.WriteLine(e1.Message);
//                }
//            }
//            catch (System.ArgumentException e1)
//            {
//                errorCount = try_failure(errorCount, "get(int,int)... ", "OutOfBoundsException expected but not thrown");
//                System.Console.Out.WriteLine(e1.Message);
//            }
//            try
//            {
//                if (B[B.RowCount - 1, B.ColumnCount - 1] != avals[B.RowCount - 1, B.ColumnCount - 1])
//                {
//                    errorCount = try_failure(errorCount, "get(int,int)... ", "Matrix entry (i,j) not successfully retreived");
//                }
//                else
//                {
//                    try_success("get(int,int)... ", "");
//                }
//            }
//            catch (System.IndexOutOfRangeException e)
//            {
//                errorCount = try_failure(errorCount, "get(int,int)... ", "Unexpected ArrayIndexOutOfBoundsException");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            SUB = new Matrix(subavals);
//            try
//            {
//                M = B.GetMatrix(ib, ie + B.RowCount + 1, jb, je);
//                errorCount = try_failure(errorCount, "GetMatrix(int,int,int,int)... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//            }
//            catch (System.IndexOutOfRangeException e)
//            {
//                System.Console.Out.WriteLine(e.Message);
//                try
//                {
//                    M = B.GetMatrix(ib, ie, jb, je + B.ColumnCount + 1);
//                    errorCount = try_failure(errorCount, "GetMatrix(int,int,int,int)... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//                }
//                catch (System.IndexOutOfRangeException e1)
//                {
//                    try_success("GetMatrix(int,int,int,int)... ArrayIndexOutOfBoundsException... ", "");
//                    System.Console.Out.WriteLine(e1.Message);
//                }
//            }
//            catch (System.ArgumentException e1)
//            {
//                errorCount = try_failure(errorCount, "GetMatrix(int,int,int,int)... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//                System.Console.Out.WriteLine(e1.Message);
//            }
//            try
//            {
//                M = B.GetMatrix(ib, ie, jb, je);
//                try
//                {
//                    check(SUB, M);
//                    try_success("GetMatrix(int,int,int,int)... ", "");
//                }
//                catch (System.SystemException e)
//                {
//                    errorCount = try_failure(errorCount, "GetMatrix(int,int,int,int)... ", "submatrix not successfully retreived");
//                    System.Console.Out.WriteLine(e.Message);
//                }
//            }
//            catch (System.IndexOutOfRangeException e)
//            {
//                errorCount = try_failure(errorCount, "GetMatrix(int,int,int,int)... ", "Unexpected ArrayIndexOutOfBoundsException");
//                System.Console.Out.WriteLine(e.Message);
//            }
			
//            try
//            {
//                M = B.GetMatrix(ib, ie, badcolumnindexset);
//                errorCount = try_failure(errorCount, "GetMatrix(int,int,int[])... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//            }
//            catch (System.IndexOutOfRangeException e)
//            {
//                System.Console.Out.WriteLine(e.Message);
//                try
//                {
//                    M = B.GetMatrix(ib, ie + B.RowCount + 1, columnindexset);
//                    errorCount = try_failure(errorCount, "GetMatrix(int,int,int[])... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//                }
//                catch (System.IndexOutOfRangeException e1)
//                {
//                    try_success("GetMatrix(int,int,int[])... ArrayIndexOutOfBoundsException... ", "");
//                    System.Console.Out.WriteLine(e1.Message);
//                }
//            }
//            catch (System.ArgumentException e1)
//            {
//                errorCount = try_failure(errorCount, "GetMatrix(int,int,int[])... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//                System.Console.Out.WriteLine(e1.Message);
//            }
//            try
//            {
//                M = B.GetMatrix(ib, ie, columnindexset);
//                try
//                {
//                    check(SUB, M);
//                    try_success("GetMatrix(int,int,int[])... ", "");
//                }
//                catch (System.SystemException e)
//                {
//                    errorCount = try_failure(errorCount, "GetMatrix(int,int,int[])... ", "submatrix not successfully retreived");
//                    System.Console.Out.WriteLine(e.Message);
//                }
//            }
//            catch (System.IndexOutOfRangeException e)
//            {
//                errorCount = try_failure(errorCount, "GetMatrix(int,int,int[])... ", "Unexpected ArrayIndexOutOfBoundsException");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            try
//            {
//                M = B.GetMatrix(badrowindexset, jb, je);
//                errorCount = try_failure(errorCount, "GetMatrix(int[],int,int)... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//            }
//            catch (System.IndexOutOfRangeException e)
//            {
//                System.Console.Out.WriteLine(e.Message);
//                try
//                {
//                    M = B.GetMatrix(rowindexset, jb, je + B.ColumnCount + 1);
//                    errorCount = try_failure(errorCount, "GetMatrix(int[],int,int)... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//                }
//                catch (System.IndexOutOfRangeException e1)
//                {
//                    try_success("GetMatrix(int[],int,int)... ArrayIndexOutOfBoundsException... ", "");
//                    System.Console.Out.WriteLine(e1.Message);
//                }
//            }
//            catch (System.ArgumentException e1)
//            {
//                errorCount = try_failure(errorCount, "GetMatrix(int[],int,int)... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//                System.Console.Out.WriteLine(e1.Message);
//            }
//            try
//            {
//                M = B.GetMatrix(rowindexset, jb, je);
//                try
//                {
//                    check(SUB, M);
//                    try_success("GetMatrix(int[],int,int)... ", "");
//                }
//                catch (System.SystemException e)
//                {
//                    errorCount = try_failure(errorCount, "GetMatrix(int[],int,int)... ", "submatrix not successfully retreived");
//                    System.Console.Out.WriteLine(e.Message);
//                }
//            }
//            catch (System.IndexOutOfRangeException e)
//            {
//                errorCount = try_failure(errorCount, "GetMatrix(int[],int,int)... ", "Unexpected ArrayIndexOutOfBoundsException");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            try
//            {
//                M = B.GetMatrix(badrowindexset, columnindexset);
//                errorCount = try_failure(errorCount, "GetMatrix(int[],int[])... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//            }
//            catch (System.IndexOutOfRangeException e)
//            {
//                System.Console.Out.WriteLine(e.Message);
//                try
//                {
//                    M = B.GetMatrix(rowindexset, badcolumnindexset);
//                    errorCount = try_failure(errorCount, "GetMatrix(int[],int[])... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//                }
//                catch (System.IndexOutOfRangeException e1)
//                {
//                    try_success("GetMatrix(int[],int[])... ArrayIndexOutOfBoundsException... ", "");
//                    System.Console.Out.WriteLine(e1.Message);
//                }
//            }
//            catch (System.ArgumentException e1)
//            {
//                errorCount = try_failure(errorCount, "GetMatrix(int[],int[])... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//                System.Console.Out.WriteLine(e1.Message);
//            }
//            try
//            {
//                M = B.GetMatrix(rowindexset, columnindexset);
//                try
//                {
//                    check(SUB, M);
//                    try_success("GetMatrix(int[],int[])... ", "");
//                }
//                catch (System.SystemException e)
//                {
//                    errorCount = try_failure(errorCount, "GetMatrix(int[],int[])... ", "submatrix not successfully retreived");
//                    System.Console.Out.WriteLine(e.Message);
//                }
//            }
//            catch (System.IndexOutOfRangeException e)
//            {
//                errorCount = try_failure(errorCount, "GetMatrix(int[],int[])... ", "Unexpected ArrayIndexOutOfBoundsException");
//                System.Console.Out.WriteLine(e.Message);
//            }
			
//            // Various set methods:
//            try
//            {
//                B[B.RowCount, B.ColumnCount - 1] = 0.0;
//                errorCount = try_failure(errorCount, "set(int,int,double)... ", "OutOfBoundsException expected but not thrown");
//            }
//            catch (System.IndexOutOfRangeException e)
//            {
//                System.Console.Out.WriteLine(e.Message);
//                try
//                {
//                    B[B.RowCount - 1, B.ColumnCount] = 0.0;
//                    errorCount = try_failure(errorCount, "set(int,int,double)... ", "OutOfBoundsException expected but not thrown");
//                }
//                catch (System.IndexOutOfRangeException e1)
//                {
//                    try_success("set(int,int,double)... OutofBoundsException... ", "");
//                    System.Console.Out.WriteLine(e1.Message);
//                }
//            }
//            catch (System.ArgumentException e1)
//            {
//                errorCount = try_failure(errorCount, "set(int,int,double)... ", "OutOfBoundsException expected but not thrown");
//                System.Console.Out.WriteLine(e1.Message);
//            }
//            try
//            {
//                B[ib, jb] = 0.0;
//                tmp = B[ib, jb];
//                try
//                {
//                    check(tmp, 0.0);
//                    try_success("set(int,int,double)... ", "");
//                }
//                catch (System.SystemException e)
//                {
//                    errorCount = try_failure(errorCount, "set(int,int,double)... ", "Matrix element not successfully set");
//                    System.Console.Out.WriteLine(e.Message);
//                }
//            }
//            catch (System.IndexOutOfRangeException e1)
//            {
//                errorCount = try_failure(errorCount, "set(int,int,double)... ", "Unexpected ArrayIndexOutOfBoundsException");
//                System.Console.Out.WriteLine(e1.Message);
//            }
//            M = new Matrix(2, 3, 0.0);
//            try
//            {
//                B.SetMatrix(ib, ie + B.RowCount + 1, jb, je, M);
//                errorCount = try_failure(errorCount, "SetMatrix(int,int,int,int,Matrix)... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//            }
//            catch (System.IndexOutOfRangeException e)
//            {
//                System.Console.Out.WriteLine(e.Message);
//                try
//                {
//                    B.SetMatrix(ib, ie, jb, je + B.ColumnCount + 1, M);
//                    errorCount = try_failure(errorCount, "SetMatrix(int,int,int,int,Matrix)... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//                }
//                catch (System.IndexOutOfRangeException e1)
//                {
//                    try_success("SetMatrix(int,int,int,int,Matrix)... ArrayIndexOutOfBoundsException... ", "");
//                    System.Console.Out.WriteLine(e1.Message);
//                }
//            }
//            catch (System.ArgumentException e1)
//            {
//                errorCount = try_failure(errorCount, "SetMatrix(int,int,int,int,Matrix)... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//                System.Console.Out.WriteLine(e1.Message);
//            }
//            try
//            {
//                B.SetMatrix(ib, ie, jb, je, M);
//                try
//                {
//                    check(M - B.GetMatrix(ib, ie, jb, je), M);
//                    try_success("SetMatrix(int,int,int,int,Matrix)... ", "");
//                }
//                catch (System.SystemException e)
//                {
//                    errorCount = try_failure(errorCount, "SetMatrix(int,int,int,int,Matrix)... ", "submatrix not successfully set");
//                    System.Console.Out.WriteLine(e.Message);
//                }
//                B.SetMatrix(ib, ie, jb, je, SUB);
//            }
//            catch (System.IndexOutOfRangeException e1)
//            {
//                errorCount = try_failure(errorCount, "SetMatrix(int,int,int,int,Matrix)... ", "Unexpected ArrayIndexOutOfBoundsException");
//                System.Console.Out.WriteLine(e1.Message);
//            }
//            try
//            {
//                B.SetMatrix(ib, ie + B.RowCount + 1, columnindexset, M);
//                errorCount = try_failure(errorCount, "SetMatrix(int,int,int[],Matrix)... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//            }
//            catch (System.IndexOutOfRangeException e)
//            {
//                System.Console.Out.WriteLine(e.Message);
//                try
//                {
//                    B.SetMatrix(ib, ie, badcolumnindexset, M);
//                    errorCount = try_failure(errorCount, "SetMatrix(int,int,int[],Matrix)... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//                }
//                catch (System.IndexOutOfRangeException e1)
//                {
//                    try_success("SetMatrix(int,int,int[],Matrix)... ArrayIndexOutOfBoundsException... ", "");
//                    System.Console.Out.WriteLine(e1.Message);
//                }
//            }
//            catch (System.ArgumentException e1)
//            {
//                errorCount = try_failure(errorCount, "SetMatrix(int,int,int[],Matrix)... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//                System.Console.Out.WriteLine(e1.Message);
//            }
//            try
//            {
//                B.SetMatrix(ib, ie, columnindexset, M);
//                try
//                {
//                    check(M - B.GetMatrix(ib, ie, columnindexset), M);
//                    try_success("SetMatrix(int,int,int[],Matrix)... ", "");
//                }
//                catch (System.SystemException e)
//                {
//                    errorCount = try_failure(errorCount, "SetMatrix(int,int,int[],Matrix)... ", "submatrix not successfully set");
//                    System.Console.Out.WriteLine(e.Message);
//                }
//                B.SetMatrix(ib, ie, jb, je, SUB);
//            }
//            catch (System.IndexOutOfRangeException e1)
//            {
//                errorCount = try_failure(errorCount, "SetMatrix(int,int,int[],Matrix)... ", "Unexpected ArrayIndexOutOfBoundsException");
//                System.Console.Out.WriteLine(e1.Message);
//            }
//            try
//            {
//                B.SetMatrix(rowindexset, jb, je + B.ColumnCount + 1, M);
//                errorCount = try_failure(errorCount, "SetMatrix(int[],int,int,Matrix)... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//            }
//            catch (System.IndexOutOfRangeException e)
//            {
//                System.Console.Out.WriteLine(e.Message);
//                try
//                {
//                    B.SetMatrix(badrowindexset, jb, je, M);
//                    errorCount = try_failure(errorCount, "SetMatrix(int[],int,int,Matrix)... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//                }
//                catch (System.IndexOutOfRangeException e1)
//                {
//                    try_success("SetMatrix(int[],int,int,Matrix)... ArrayIndexOutOfBoundsException... ", "");
//                    System.Console.Out.WriteLine(e1.Message);
//                }
//            }
//            catch (System.ArgumentException e1)
//            {
//                errorCount = try_failure(errorCount, "SetMatrix(int[],int,int,Matrix)... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//                System.Console.Out.WriteLine(e1.Message);
//            }
//            try
//            {
//                B.SetMatrix(rowindexset, jb, je, M);
//                try
//                {
//                    check(M - B.GetMatrix(rowindexset, jb, je), M);
//                    try_success("SetMatrix(int[],int,int,Matrix)... ", "");
//                }
//                catch (System.SystemException e)
//                {
//                    errorCount = try_failure(errorCount, "SetMatrix(int[],int,int,Matrix)... ", "submatrix not successfully set");
//                    System.Console.Out.WriteLine(e.Message);
//                }
//                B.SetMatrix(ib, ie, jb, je, SUB);
//            }
//            catch (System.IndexOutOfRangeException e1)
//            {
//                errorCount = try_failure(errorCount, "SetMatrix(int[],int,int,Matrix)... ", "Unexpected ArrayIndexOutOfBoundsException");
//                System.Console.Out.WriteLine(e1.Message);
//            }
//            try
//            {
//                B.SetMatrix(rowindexset, badcolumnindexset, M);
//                errorCount = try_failure(errorCount, "SetMatrix(int[],int[],Matrix)... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//            }
//            catch (System.IndexOutOfRangeException e)
//            {
//                System.Console.Out.WriteLine(e.Message);
//                try
//                {
//                    B.SetMatrix(badrowindexset, columnindexset, M);
//                    errorCount = try_failure(errorCount, "SetMatrix(int[],int[],Matrix)... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//                }
//                catch (System.IndexOutOfRangeException e1)
//                {
//                    try_success("SetMatrix(int[],int[],Matrix)... ArrayIndexOutOfBoundsException... ", "");
//                    System.Console.Out.WriteLine(e1.Message);
//                }
//            }
//            catch (System.ArgumentException e1)
//            {
//                errorCount = try_failure(errorCount, "SetMatrix(int[],int[],Matrix)... ", "ArrayIndexOutOfBoundsException expected but not thrown");
//                System.Console.Out.WriteLine(e1.Message);
//            }
//            try
//            {
//                B.SetMatrix(rowindexset, columnindexset, M);
//                try
//                {
//                    check(M - B.GetMatrix(rowindexset, columnindexset), M);
//                    try_success("SetMatrix(int[],int[],Matrix)... ", "");
//                }
//                catch (System.SystemException e)
//                {
//                    errorCount = try_failure(errorCount, "SetMatrix(int[],int[],Matrix)... ", "submatrix not successfully set");
//                    System.Console.Out.WriteLine(e.Message);
//                }
//            }
//            catch (System.IndexOutOfRangeException e1)
//            {
//                errorCount = try_failure(errorCount, "SetMatrix(int[],int[],Matrix)... ", "Unexpected ArrayIndexOutOfBoundsException");
//                System.Console.Out.WriteLine(e1.Message);
//            }
			
//            // Array-like methods:
//            // Subtract
//            // SubtractEquals
//            // Add
//            // AddEquals
//            // ArrayLeftDivide
//            // ArrayLeftDivideEquals
//            // ArrayRightDivide
//            // ArrayRightDivideEquals
//            // arrayTimes
//            // ArrayMultiplyEquals
//            // uminus
			
//            print("\nTesting array-like methods...\n");
//            S = new Matrix(columnwise, nonconformld);
//            R = Matrix.Random(A.RowCount, A.ColumnCount);
//            A = R;
//            try
//            {
//                S = A - S;
//                errorCount = try_failure(errorCount, "Subtract conformance check... ", "nonconformance not raised");
//            }
//            catch (System.ArgumentException e)
//            {
//                try_success("Subtract conformance check... ", "");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            if ((A - R).Norm1() != 0.0)
//            {
//                errorCount = try_failure(errorCount, "Subtract... ", "(difference of identical Matrices is nonzero,\nSubsequent use of Subtract should be suspect)");
//            }
//            else
//            {
//                try_success("Subtract... ", "");
//            }
//            A = (Matrix) R.Clone();
//            A.Subtract(R);
//            Z = new Matrix(A.RowCount, A.ColumnCount);
//            try
//            {
//                A.Subtract(S);
//                errorCount = try_failure(errorCount, "SubtractEquals conformance check... ", "nonconformance not raised");
//            }
//            catch (System.ArgumentException e)
//            {
//                try_success("SubtractEquals conformance check... ", "");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            if ((A - Z).Norm1() != 0.0)
//            {
//                errorCount = try_failure(errorCount, "SubtractEquals... ", "(difference of identical Matrices is nonzero,\nSubsequent use of Subtract should be suspect)");
//            }
//            else
//            {
//                try_success("SubtractEquals... ", "");
//            }
			
//            A = (Matrix) R.Clone();
//            B = Matrix.Random(A.RowCount, A.ColumnCount);
//            C = A - B;
//            try
//            {
//                S = A + S;
//                errorCount = try_failure(errorCount, "Add conformance check... ", "nonconformance not raised");
//            }
//            catch (System.ArgumentException e)
//            {
//                try_success("Add conformance check... ", "");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            try
//            {
//                check(C + B, A);
//                try_success("Add... ", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "Add... ", "(C = A - B, but C + B != A)");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            C = A - B;
//            C.Add(B);
//            try
//            {
//                A.Add(S);
//                errorCount = try_failure(errorCount, "AddEquals conformance check... ", "nonconformance not raised");
//            }
//            catch (System.ArgumentException e)
//            {
//                try_success("AddEquals conformance check... ", "");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            try
//            {
//                check(C, A);
//                try_success("AddEquals... ", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "AddEquals... ", "(C = A - B, but C = C + B != A)");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            A = ((Matrix) R.Clone());
//            A.UnaryMinus();
//            try
//            {
//                check(A + R, Z);
//                try_success("UnaryMinus... ", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "uminus... ", "(-A + A != zeros)");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            A = (Matrix) R.Clone();
//            O = new Matrix(A.RowCount, A.ColumnCount, 1.0);
//            try
//            {
//                Matrix.ArrayDivide(A, S);
//                errorCount = try_failure(errorCount, "ArrayRightDivide conformance check... ", "nonconformance not raised");
//            }
//            catch (System.ArgumentException e)
//            {
//                try_success("ArrayRightDivide conformance check... ", "");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            C = Matrix.ArrayDivide(A, R);
//            try
//            {
//                check(C, O);
//                try_success("ArrayRightDivide... ", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "ArrayRightDivide... ", "(M./M != ones)");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            try
//            {
//                A.ArrayDivide(S);
//                errorCount = try_failure(errorCount, "ArrayRightDivideEquals conformance check... ", "nonconformance not raised");
//            }
//            catch (System.ArgumentException e)
//            {
//                try_success("ArrayRightDivideEquals conformance check... ", "");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            A.ArrayDivide(R);
//            try
//            {
//                check(A, O);
//                try_success("ArrayRightDivideEquals... ", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "ArrayRightDivideEquals... ", "(M./M != ones)");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            A = (Matrix) R.Clone();
//            B = Matrix.Random(A.RowCount, A.ColumnCount);
//            try
//            {
//                S = Matrix.ArrayMultiply(A, S);
//                errorCount = try_failure(errorCount, "arrayTimes conformance check... ", "nonconformance not raised");
//            }
//            catch (System.ArgumentException e)
//            {
//                try_success("arrayTimes conformance check... ", "");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            C = Matrix.ArrayMultiply(A, B);
//            try
//            {
//                C.ArrayDivide(B);
//                check(C, A);
//                try_success("arrayTimes... ", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "arrayTimes... ", "(A = R, C = A.*B, but C./B != A)");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            try
//            {
//                A.ArrayMultiply(S);
//                errorCount = try_failure(errorCount, "ArrayMultiplyEquals conformance check... ", "nonconformance not raised");
//            }
//            catch (System.ArgumentException e)
//            {
//                try_success("ArrayMultiplyEquals conformance check... ", "");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            A.ArrayMultiply(B);
//            try
//            {
//                A.ArrayDivide(B);
//                check(A, R);
//                try_success("ArrayMultiplyEquals... ", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "ArrayMultiplyEquals... ", "(A = R, A = A.*B, but A./B != R)");
//                System.Console.Out.WriteLine(e.Message);
//            }
			
//            // LA methods:
//            // Transpose
//            // Multiply
//            // Condition
//            // Rank
//            // Determinant
//            // trace
//            // Norm1
//            // norm2
//            // normF
//            // normInf
//            // Solve
//            // solveTranspose
//            // Inverse
//            // chol
//            // Eigen
//            // lu
//            // qr
//            // svd 

//            print("\nTesting linear algebra methods...\n");
//            A = new Matrix(columnwise, 3);
//            T = new Matrix(tvals);
//            T = Matrix.Transpose(A);
//            try
//            {
//                check(Matrix.Transpose(A), T);
//                try_success("Transpose...", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "Transpose()...", "Transpose unsuccessful");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            Matrix.Transpose(A);
//            try
//            {
//                check(A.Norm1(), columnsummax);
//                try_success("Norm1...", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "Norm1()...", "incorrect norm calculation");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            try
//            {
//                check(A.NormInf(), rowsummax);
//                try_success("normInf()...", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "normInf()...", "incorrect norm calculation");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            try
//            {
//                check(A.NormF(), System.Math.Sqrt(sumofsquares));
//                try_success("normF...", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "normF()...", "incorrect norm calculation");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            try
//            {
//                check(A.Trace(), sumofdiagonals);
//                try_success("trace()...", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "trace()...", "incorrect trace calculation");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            try
//            {
//                check(A.GetMatrix(0, A.RowCount - 1, 0, A.RowCount - 1).Determinant(), 0.0);
//                try_success("Determinant()...", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "Determinant()...", "incorrect determinant calculation");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            SQ = new Matrix(square);
//            try
//            {
//                check(A * Matrix.Transpose(A), SQ);
//                try_success("Multiply(Matrix)...", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "Multiply(Matrix)...", "incorrect Matrix-Matrix product calculation");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            try
//            {
//                check(0.0 * A, Z);
//                try_success("Multiply(double)...", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "Multiply(double)...", "incorrect Matrix-scalar product calculation");
//                System.Console.Out.WriteLine(e.Message);
//            }
			
//            A = new Matrix(columnwise, 4);
//            QRDecomposition QR = A.QRD();
//            R = QR.R;
//            try
//            {
//                check(A, QR.Q * R);
//                try_success("QRDecomposition...", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "QRDecomposition...", "incorrect QR decomposition calculation");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            SingularValueDecomposition SVD = A.SVD();
//            try
//            {
//                check(A, SVD.LeftSingularVectors * (SVD.S * Matrix.Transpose(SVD.RightSingularVectors)));
//                try_success("SingularValueDecomposition...", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "SingularValueDecomposition...", "incorrect singular value decomposition calculation");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            DEF = new Matrix(rankdef);
//            try
//            {
//                check(DEF.Rank(), System.Math.Min(DEF.RowCount, DEF.ColumnCount) - 1);
//                try_success("Rank()...", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "Rank()...", "incorrect Rank calculation");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            B = new Matrix(condmat);
//            SVD = B.SVD();
//            double[] singularvalues = SVD.SingularValues;
//            try
//            {
//                check(B.Condition(), singularvalues[0] / singularvalues[System.Math.Min(B.RowCount, B.ColumnCount) - 1]);
//                try_success("Condition()...", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "Condition()...", "incorrect condition number calculation");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            int n = A.ColumnCount;
//            A = A.GetMatrix(0, n - 1, 0, n - 1);
//            A[0, 0] = 0.0;
//            LUDecomposition LU = A.LUD();
//            try
//            {
//                check(A.GetMatrix(LU.Pivot, 0, n - 1), LU.L * LU.U);
//                try_success("LUDecomposition...", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "LUDecomposition...", "incorrect LU decomposition calculation");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            X = A.Inverse();
//            try
//            {
//                check(A * X, Matrix.Identity(3, 3));
//                try_success("Inverse()...", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "Inverse()...", "incorrect Inverse calculation");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            O = new Matrix(SUB.RowCount, 1, 1.0);
//            SOL = new Matrix(sqSolution);
//            SQ = SUB.GetMatrix(0, SUB.RowCount - 1, 0, SUB.RowCount - 1);
//            try
//            {
//                check(SQ.Solve(SOL), O);
//                try_success("Solve()...", "");
//            }
//            catch (System.ArgumentException e1)
//            {
//                errorCount = try_failure(errorCount, "Solve()...", e1.Message);
//                System.Console.Out.WriteLine(e1.Message);
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "Solve()...", e.Message);
//                System.Console.Out.WriteLine(e.Message);
//            }
//            A = new Matrix(pvals);
//            CholeskyDecomposition Chol = A.chol();
//            Matrix L = Chol.GetL();
//            try
//            {
//                check(A, L * Matrix.Transpose(L));
//                try_success("CholeskyDecomposition...", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "CholeskyDecomposition...", "incorrect Cholesky decomposition calculation");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            X = Chol.Solve(Matrix.Identity(3, 3));
//            try
//            {
//                check(A * X, Matrix.Identity(3, 3));
//                try_success("CholeskyDecomposition Solve()...", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "CholeskyDecomposition Solve()...", "incorrect Choleskydecomposition Solve calculation");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            EigenvalueDecomposition Eig = A.Eigen();
//            Matrix D = Eig.BlockDiagonal;
//            Matrix V = Eig.EigenVectors;
//            try
//            {
//                check(A * V, V * D);
//                try_success("EigenvalueDecomposition (symmetric)...", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "EigenvalueDecomposition (symmetric)...", "incorrect symmetric Eigenvalue decomposition calculation");
//                System.Console.Out.WriteLine(e.Message);
//            }
//            A = new Matrix(evals);
//            Eig = A.Eigen();
//            D = Eig.BlockDiagonal;
//            V = Eig.EigenVectors;
//            try
//            {
//                check(A * V, V * D);
//                try_success("EigenvalueDecomposition (nonsymmetric)...", "");
//            }
//            catch (System.SystemException e)
//            {
//                errorCount = try_failure(errorCount, "EigenvalueDecomposition (nonsymmetric)...", "incorrect nonsymmetric Eigenvalue decomposition calculation");
//                System.Console.Out.WriteLine(e.Message);
//            }
			
//            print("\nTestMatrix completed.\n");
//            print("Total errors reported: " + System.Convert.ToString(errorCount) + "\n");
//            print("Total warnings reported: " + System.Convert.ToString(warningCount) + "\n");

//            if(errorCount > 0) throw new Exception("Errors reported.");
//        }
		

//        #region private utility routines
		
//        /// <summary>Check magnitude of difference of scalars. *</summary>
//        private static void  check(double x, double y)
//        {
//            double eps = System.Math.Pow(2.0, - 52.0);
//            if (x == 0 & System.Math.Abs(y) < 10 * eps)
//                return ;
//            if (y == 0 & System.Math.Abs(x) < 10 * eps)
//                return ;
//            if (System.Math.Abs(x - y) > 10 * eps * System.Math.Max(System.Math.Abs(x), System.Math.Abs(y)))
//            {
//                throw new System.SystemException("The difference x-y is too large: x = " + x.ToString() + "  y = " + y.ToString());
//            }
//        }
		
//        /// <summary>Check norm of difference of "vectors". *</summary>
//        private static void  check(double[] x, double[] y)
//        {
//            if (x.Length == y.Length)
//            {
//                for (int i = 0; i < x.Length; i++)
//                {
//                    check(x[i], y[i]);
//                }
//            }
//            else
//            {
//                throw new System.SystemException("Attempt to compare vectors of different lengths");
//            }
//        }
		
//        /// <summary>Check norm of difference of arrays. *</summary>
//        private static void  check(double[,] x, double[,] y)
//        {
//            Matrix A = new Matrix(x);
//            Matrix B = new Matrix(y);
//            check(A, B);
//        }
		
//        /// <summary>Check norm of difference of Matrices. *</summary>
//        private static void  check(Matrix X, Matrix Y)
//        {
//            double eps = System.Math.Pow(2.0, - 52.0);
//            if (X.Norm1() == 0.0 & Y.Norm1() < 10 * eps)
//                return ;
//            if (Y.Norm1() == 0.0 & X.Norm1() < 10 * eps)
//                return ;
//            if ((X - Y).Norm1() > 1000 * eps * System.Math.Max(X.Norm1(), Y.Norm1()))
//            {
//                throw new System.SystemException("The norm of (X-Y) is too large: " + (X - Y).Norm1().ToString());
//            }
//        }
		
//        /// <summary>Shorten spelling of print. *</summary>
//        private static void  print(System.String s)
//        {
//            System.Console.Out.Write(s);
//        }
		
//        /// <summary>Print appropriate messages for successful outcome try *</summary>
//        private static void  try_success(System.String s, System.String e)
//        {
//            print(">    " + s + "success\n");
//            if ((System.Object) e != (System.Object) "")
//            {
//                print(">      Message: " + e + "\n");
//            }
//        }
//        /// <summary>Print appropriate messages for unsuccessful outcome try *</summary>
//        private static int try_failure(int count, System.String s, System.String e)
//        {
//            print(">    " + s + "*** failure ***\n>      Message: " + e + "\n");
//            return ++count;
//        }
		
//        /// <summary>Print appropriate messages for unsuccessful outcome try *</summary>
//        private static int try_warning(int count, System.String s, System.String e)
//        {
//            print(">    " + s + "*** warning ***\n>      Message: " + e + "\n");
//            return ++count;
//        }

//        #endregion
//    }
//}
