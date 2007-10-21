#region Math.NET Iridium (LGPL) by Vermorel, Ruegg + Contributors
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2004-2007, Joannes Vermorel, http://www.vermorel.com
//                          Christoph Rüegg, http://christoph.ruegg.name
//
// Contribution: The MathWorks and NIST [2000]
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
using System.Text;
using MathNet.Numerics.Properties;
using MathNet.Numerics.Distributions;

namespace MathNet.Numerics.LinearAlgebra
{
    /// <summary>
    /// Real Matrix
    /// </summary>
    public interface IMatrix
    {
        /// <summary>Gets the number of rows.</summary>
        int RowCount { get; }
        /// <summary>Gets the number of columns.</summary>
        int ColumnCount { get; }
        /// <summary>Gets or set the element indexed by <c>(i, j)</c>
        /// in the <c>Matrix</c>.</summary>
        /// <param name="i">Row index.</param>
        /// <param name="j">Column index.</param>
        double this[int i, int j] { get; set; }
    }

    /// <summary>Real matrix.</summary>
    /// <remarks>
    /// The class <c>Matrix</c> provides the elementary operations
    /// on matrices (addition, multiplication, inversion, transposition, ...).
    /// Helpers to handle sub-matrices are also provided.
    /// </remarks>
    [Serializable]
    public class Matrix : ICloneable, IMatrix
    {
        private int _rowCount;
        private int _columnCount;

        /// <summary>Array for internal storage of elements.</summary>
        private double[][] _data;

        #region Constructors and static constructive methods

        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        public static double[][] CreateMatrixData(int m, int n)
        {
            double[][] data = new double[m][];
            for(int i = 0; i < m; i++)
            {
                data[i] = new double[n];
            }
            return data;
        }

        public static void GetRowColumnCount(double[][] data, out int rows, out int columns)
        {
            rows = data.Length;
            columns = (rows == 0) ? 0 : data[0].Length;
        }

        /// <summary>Construct an m-by-n matrix of zeros. </summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        public Matrix(int m, int n)
        {
            _data = CreateMatrixData(m, n);
            _rowCount = m;
            _columnCount = n;
        }

        /// <summary>Constructs a m-by-m square matrix.</summary>
        /// <param name="m">Size of the square matrix.</param>
        /// <param name="s">Diagonal value.</param>
        public Matrix(int m, double s)
        {
            _data = new double[m][];
            _rowCount = m;
            _columnCount = m;

            for(int i = 0; i < m; i++)
            {
                double[] col = new double[m];
                col[i] = s;
                _data[i] = col;
            }
        }

        /// <summary>Construct an m-by-n constant matrix.</summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        /// <param name="s">Fill the matrix with this scalar value.</param>
        public Matrix(int m, int n, double s)
        {
            _data = new double[m][];
            _rowCount = m;
            _columnCount = n;

            for(int i = 0; i < m; i++)
            {
                double[] col = new double[n];
                for(int j = 0; j < n; j++)
                {
                    col[j] = s;
                }
                _data[i] = col;
            }
        }

        /// <summary>Constructs a matrix from a 2-D array by deep-copying the provided array to the internal data stucture.</summary>
        /// <param name="A">Two-dimensional array of doubles.</param>
        [Obsolete("Use 'Matrix.Create(double[,])' or 'new Matrix(double[][])' instead")]
        public Matrix(double[,] A)
        {
            _rowCount = A.GetLength(0);
            _columnCount = A.GetLength(1);
            _data = new double[_rowCount][];

            for(int i = 0; i < _rowCount; i++)
            {
                double[] col = new double[_columnCount];
                for(int j = 0; j < _columnCount; j++)
                {
                    col[j] = A[i, j];
                }
                _data[i] = col;
            }
        }

        /// <summary>Constructs a matrix from a jagged 2-D array, directly using the provided array as internal data structure.</summary>
        /// <param name="A">Two-dimensional jagged array of doubles.</param>
        /// <exception cref="System.ArgumentException">All rows must have the same length.</exception>
        /// <seealso cref="Create"/>
        public Matrix(double[][] A)
        {
            _data = A;
            GetRowColumnCount(_data, out _rowCount, out _columnCount);
        }

        /// <summary>Construct a matrix from a one-dimensional packed array</summary>
        /// <param name="vals">One-dimensional array of doubles, packed by columns (ala Fortran).</param>
        /// <param name="m">Number of rows.</param>
        /// <exception cref="System.ArgumentException">Array length must be a multiple of m.</exception>
        public Matrix(double[] vals, int m)
        {
            _rowCount = m;
            if(m == 0)
            {
                _columnCount = 0;
                if(vals.Length != 0)
                {
                    throw new ArgumentException(string.Format(Resources.ArgumentVectorLengthsMultipleOf, "m"));
                }
            }
            else
            {
                int rem;
                _columnCount = Math.DivRem(vals.Length, m, out rem);
                if(rem != 0)
                {
                    throw new ArgumentException(string.Format(Resources.ArgumentVectorLengthsMultipleOf, "m"));
                }
            }

            _data = new double[_rowCount][];
            for(int i = 0; i < _rowCount; i++)
            {
                double[] col = new double[_columnCount];
                for(int j = 0; j < _columnCount; j++)
                {
                    col[j] = vals[i + j * _rowCount];
                }
                _data[i] = col;
            }
        }

        /// <summary>Constructs a matrix from a copy of a 2-D array by deep-copy.</summary>
        /// <param name="A">Two-dimensional array of doubles.</param>
        public static Matrix Create(double[,] A)
        {
            int rows = A.GetLength(0);
            int columns = A.GetLength(1);
            double[][] newData = new double[rows][];

            for(int i = 0; i < rows; i++)
            {
                double[] col = new double[columns];
                for(int j = 0; j < columns; j++)
                {
                    col[j] = A[i, j];
                }
                newData[i] = col;
            }

            return new Matrix(newData);
        }

        /// <summary>Constructs a matrix from a copy of a 2-D array by deep-copy.</summary>
        /// <param name="A">Two-dimensional array of doubles.</param>
        public static Matrix Create(double[][] A)
        {
            return new Matrix(CloneMatrixData(A));
        }

        /// <summary>Generates identity matrix</summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of colums.</param>
        /// <returns>An m-by-n matrix with ones on the diagonal and zeros elsewhere.</returns>
        public static Matrix Identity(int m, int n)
        {
            double[][] data = new double[m][];
            int count = Math.Min(m, n);
            for(int i = 0; i < count; i++)
            {
                double[] col = new double[n];
                col[i] = 1.0;
                data[i] = col;
            }
            return new Matrix(data);
        }

        /// <summary>
        /// Generates an m-by-m matrix filled with 1.
        /// </summary>
        /// <param name="m">Number of rows = Number of columns</param>
        public static Matrix Ones(int m)
        {
            return new Matrix(m, m, 1.0);
        }

        /// <summary>
        /// Generates an m-by-m matrix filled with 0.
        /// </summary>
        /// <param name="m">Number of rows = Number of columns</param>
        public static Matrix Zeros(int m)
        {
            return new Matrix(m, m);
        }

        /// <summary>Generates matrix with random elements</summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        /// <param name="randomDistribution">Continuous Random Distribution or Source</param>
        /// <returns>An m-by-n matrix with elements distributed according to the provided distribution.</returns>
        public static Matrix Random(int m, int n, IContinuousGenerator randomDistribution)
        {
            double[][] data = new double[m][];
            for(int i = 0; i < m; i++)
            {
                double[] col = new double[n];
                for(int j = 0; j < n; j++)
                    col[j] = randomDistribution.NextDouble();
                data[i] = col;
            }
            return new Matrix(data);
        }

        /// <summary>Generates matrix with random elements</summary>
        /// <param name="m">Number of rows.</param>
        /// <param name="n">Number of columns.</param>
        /// <returns>An m-by-n matrix with uniformly distributed
        /// random elements in <c>[0, 1)</c> interval.</returns>
        public static Matrix Random(int m, int n)
        {
            return Random(m, n, new RandomSources.SystemRandomSource());
        }

        #endregion //  Constructors

        /// <summary>Gets the number of rows.</summary>
        public int RowCount
        {
            get { return _rowCount; }
        }

        /// <summary>Gets the number of columns.</summary>
        public int ColumnCount
        {
            get { return _columnCount; }
        }


        #region	 Public Methods

        #region Sub-matrices operation

        /// <summary>Gets a submatrix.</summary>
        /// <param name="i0">First row index.</param>
        /// <param name="i1">Last row index (inclusive).</param>
        /// <param name="j0">First column index.</param>
        /// <param name="j1">Last column index (inclusive).</param>
        /// <returns>A(i0:i1,j0:j1)</returns>
        /// <exception cref="System.IndexOutOfRangeException">Submatrix indices</exception>
        public virtual Matrix GetMatrix(int i0, int i1, int j0, int j1)
        {
            double[][] newData = CreateMatrixData(i1 - i0 + 1, j1 - j0 + 1);
            try
            {
                for(int i = i0; i <= i1; i++)
                    for(int j = j0; j <= j1; j++)
                        newData[i - i0][j - j0] = _data[i][j];
            }
            catch(IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException(Resources.ArgumentMatrixIndexOutOfRange, e);
            }
            return new Matrix(newData);
        }

        /// <summary>Gets a submatrix.</summary>
        /// <param name="r">Array of row indices.</param>
        /// <param name="c">Array of column indices.</param>
        /// <returns>A(r(:),c(:))</returns>
        /// <exception cref="System.IndexOutOfRangeException">Submatrix indices.</exception>
        public virtual Matrix GetMatrix(int[] r, int[] c)
        {
            double[][] newData = CreateMatrixData(r.Length, c.Length);
            try
            {
                for(int i = 0; i < r.Length; i++)
                    for(int j = 0; j < c.Length; j++)
                        newData[i][j] = _data[r[i]][c[j]];
            }
            catch(IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException(Resources.ArgumentMatrixIndexOutOfRange, e);
            }
            return new Matrix(newData);
        }

        /// <summary>Get a submatrix.</summary>
        /// <param name="i0">First row index.</param>
        /// <param name="i1">Last row index (inclusive).</param>
        /// <param name="c">Array of column indices.</param>
        /// <returns>A(i0:i1,c(:))</returns>
        /// <exception cref="System.IndexOutOfRangeException">Submatrix indices.</exception>
        public virtual Matrix GetMatrix(int i0, int i1, int[] c)
        {
            double[][] newData = CreateMatrixData(i1 - i0 + 1, c.Length);
            try
            {
                for(int i = i0; i <= i1; i++)
                    for(int j = 0; j < c.Length; j++)
                        newData[i - i0][j] = _data[i][c[j]];
            }
            catch(IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException(Resources.ArgumentMatrixIndexOutOfRange, e);
            }
            return new Matrix(newData);
        }

        /// <summary>Get a submatrix.</summary>
        /// <param name="r">Array of row indices.</param>
        /// <param name="j0">First column index.</param>
        /// <param name="j1">Last column index (inclusive).</param>
        /// <returns>A(r(:),j0:j1)</returns>
        /// <exception cref="System.IndexOutOfRangeException">Submatrix indices.</exception>
        public virtual Matrix GetMatrix(int[] r, int j0, int j1)
        {
            double[][] newData = CreateMatrixData(r.Length, j1 - j0 + 1);
            try
            {
                for(int i = 0; i < r.Length; i++)
                    for(int j = j0; j <= j1; j++)
                        newData[i][j - j0] = _data[r[i]][j];
            }
            catch(IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException(Resources.ArgumentMatrixIndexOutOfRange, e);
            }
            return new Matrix(newData);
        }

        /// <summary>Set a submatrix.</summary>
        /// <param name="i0">First row index.</param>
        /// <param name="i1">Last row index (inclusive).</param>
        /// <param name="j0">First column index.</param>
        /// <param name="j1">Last column index (inclusive).</param>
        /// <param name="X">A(i0:i1,j0:j1)</param>
        /// <exception cref="System.IndexOutOfRangeException">Submatrix indices.</exception>
        public virtual void SetMatrix(int i0, int i1, int j0, int j1, IMatrix X)
        {
            try
            {
                for(int i = i0; i <= i1; i++)
                    for(int j = j0; j <= j1; j++)
                        _data[i][j] = X[i - i0, j - j0];
            }
            catch(IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException(Resources.ArgumentMatrixIndexOutOfRange, e);
            }
        }

        /// <summary>Sets a submatrix.</summary>
        /// <param name="r">Array of row indices.</param>
        /// <param name="c">Array of column indices.</param>
        /// <param name="X">A(r(:),c(:))</param>
        /// <exception cref="System.IndexOutOfRangeException">Submatrix indices</exception>
        public virtual void SetMatrix(int[] r, int[] c, IMatrix X)
        {
            try
            {
                for(int i = 0; i < r.Length; i++)
                    for(int j = 0; j < c.Length; j++)
                        _data[r[i]][c[j]] = X[i, j];
            }
            catch(IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException(Resources.ArgumentMatrixIndexOutOfRange, e);
            }
        }

        /// <summary>Sets a submatrix.</summary>
        /// <param name="r">Array of row indices.</param>
        /// <param name="j0">First column index.</param>
        /// <param name="j1">Last column index (inclusive).</param>
        /// <param name="X">A(r(:),j0:j1)</param>
        /// <exception cref="System.IndexOutOfRangeException">Submatrix indices</exception>
        public virtual void SetMatrix(int[] r, int j0, int j1, IMatrix X)
        {
            try
            {
                for(int i = 0; i < r.Length; i++)
                    for(int j = j0; j <= j1; j++)
                        _data[r[i]][j] = X[i, j - j0];
            }
            catch(IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException(Resources.ArgumentMatrixIndexOutOfRange, e);
            }
        }

        /// <summary>Set a submatrix.</summary>
        /// <param name="i0">First row index.</param>
        /// <param name="i1">Last row index (inclusive).</param>
        /// <param name="c">Array of column indices.</param>
        /// <param name="X">A(i0:i1,c(:))</param>
        /// <exception cref="System.IndexOutOfRangeException">Submatrix indices.</exception>
        public virtual void SetMatrix(int i0, int i1, int[] c, IMatrix X)
        {
            try
            {
                for(int i = i0; i <= i1; i++)
                    for(int j = 0; j < c.Length; j++)
                        _data[i][c[j]] = X[i - i0, j];
            }
            catch(IndexOutOfRangeException e)
            {
                throw new IndexOutOfRangeException(Resources.ArgumentMatrixIndexOutOfRange, e);
            }
        }


        #endregion

        #region Norms computations

        /// <summary>One norm</summary>
        /// <returns>Maximum column sum.</returns>
        public double Norm1()
        {
            double f = 0;
            for(int j = 0; j < _columnCount; j++)
            {
                double s = 0;
                for(int i = 0; i < _rowCount; i++)
                {
                    s += Math.Abs(_data[i][j]);
                }
                f = Math.Max(f, s);
            }
            return f;
        }

        /// <summary>Two norm</summary>
        /// <returns>Maximum singular value.</returns>
        public double Norm2()
        {
            return (new SingularValueDecomposition(this).Norm2());
        }

        /// <summary>Infinity norm</summary>
        /// <returns>Maximum row sum.</returns>
        public double NormInf()
        {
            double f = 0;
            for(int i = 0; i < _rowCount; i++)
            {
                double s = 0;
                for(int j = 0; j < _columnCount; j++)
                {
                    s += Math.Abs(_data[i][j]);
                }
                f = Math.Max(f, s);
            }
            return f;
        }

        /// <summary>Frobenius norm</summary>
        /// <returns>Sqrt of sum of squares of all elements.</returns>
        public double NormF()
        {
            double f = 0;
            for(int i = 0; i < _rowCount; i++)
            {
                for(int j = 0; j < _columnCount; j++)
                {
                    f = Fn.Hypot(f, _data[i][j]);
                }
            }
            return f;
        }


        #endregion

        #region Elementary linear operations

        /// <summary>In place addition of <c>m</c> to this <c>Matrix</c>.</summary>
        /// <seealso cref="operator + (Matrix, Matrix)"/>
        public virtual void Add(IMatrix m)
        {
            CheckMatchingMatrixDimensions(this, m);

            for(int i = 0; i < _rowCount; i++)
            {
                for(int j = 0; j < _columnCount; j++)
                {
                    _data[i][j] += m[i, j];
                }
            }
        }

        /// <summary>Multiplies in place this <c>Matrix</c> by a scalar.</summary>
        /// <seealso cref="operator * (double, Matrix)"/>
        public virtual void Multiply(double s)
        {
            for(int i = 0; i < _rowCount; i++)
            {
                for(int j = 0; j < _columnCount; j++)
                {
                    _data[i][j] *= s;
                }
            }
        }

        /// <summary>In place linear algebraic matrix multiplication, D * A where
        /// D is the diagonal matrix.</summary>
        /// <param name="diagonal">Diagonal values of D.</param>
        /// <exception cref="ArgumentNullException"><c>diagonal</c> must not be null.</exception>
        /// <exception cref="ArgumentException">Matrix inner dimensions must agree.</exception>
        public virtual void Multiply(double[] diagonal)
        {
            if(diagonal == null)
                throw new ArgumentNullException("diagonal", string.Format(Resources.ArgumentNull, "diagonal"));
            if(diagonal.Length != _rowCount)
                throw new ArgumentException(Resources.ArgumentMatrixSameDimensions);

            for(int i = 0; i < _rowCount; i++)
            {
                double d = diagonal[i];
                for(int j = 0; j < _columnCount; j++)
                {
                    _data[i][j] *= d;
                }
            }
        }

        /// <summary>Linear algebraic matrix multiplication, A * B</summary>
        /// <exception cref="ArgumentNullException">B must not be null.</exception>
        /// <exception cref="ArgumentException">Matrix inner dimensions must agree.</exception>
        public Matrix Multiply(Matrix B)
        {
            if(B == null)
                throw new ArgumentNullException("B", string.Format(Resources.ArgumentNull, "B"));
            if(B.RowCount != _columnCount)
                throw new ArgumentException(Resources.ArgumentMatrixSameDimensions);

            double[][] newData = CreateMatrixData(_rowCount, B.ColumnCount);

            for(int j = 0; j < B.ColumnCount; j++)
            {
                // caching the column for performance
                double[] columnB = new double[_columnCount];
                for(int k = 0; k < _columnCount; k++)
                {
                    columnB[k] = B._data[k][j];
                }

                // making the line-to-column product
                for(int i = 0; i < _rowCount; i++)
                {
                    double s = 0;

                    for(int k = 0; k < _columnCount; k++)
                    {
                        s += _data[i][k] * columnB[k];
                    }
                    newData[i][j] = s;
                }
            }

            return new Matrix(newData);
        }

        /// <summary>In place substraction of <c>m</c> to this <c>Matrix</c>.</summary>
        /// <seealso cref="operator - (Matrix, Matrix)"/>
        public virtual void Subtract(IMatrix m)
        {
            CheckMatchingMatrixDimensions(this, m);

            for(int i = 0; i < _rowCount; i++)
            {
                for(int j = 0; j < _columnCount; j++)
                {
                    _data[i][j] -= m[i, j];
                }
            }
        }

        /// <summary>In place transposition of this <c>Matrix</c>.</summary>
        /// <seealso cref="Transpose(IMatrix)"/>
        public virtual void Transpose()
        {
            // TODO: test this method
            int m = _rowCount;
            int n = _columnCount;

            if(m == n)
            {
                // No need for array copy
                for(int i = 0; i < m; i++)
                    for(int j = i + 1; j < n; j++)
                    {
                        double thisIJ = this[i, j];
                        _data[i][j] = _data[j][i];
                        _data[j][i] = thisIJ;
                    }
            }
            else
            {
                double[][] newData = CreateMatrixData(n, m);
                for(int i = 0; i < m; i++)
                    for(int j = 0; j < n; j++)
                        newData[j][i] = _data[i][j];

                _data = newData;
                _rowCount = n;
                _columnCount = m;
            }
        }

        /// <summary>Gets the transposition of the provided <c>Matrix</c>.</summary>
        public static Matrix Transpose(IMatrix m)
        {
            double[][] newData = CreateMatrixData(m.ColumnCount, m.RowCount);
            for(int i = 0; i < m.RowCount; i++)
            {
                for(int j = 0; j < m.ColumnCount; j++)
                {
                    newData[j][i] = m[i, j];
                }
            }
            return new Matrix(newData);
        }

        /// <summary>In place unary minus of the <c>Matrix</c>.</summary>
        public virtual void UnaryMinus()
        {
            for(int i = 0; i < _rowCount; i++)
            {
                for(int j = 0; j < _columnCount; j++)
                {
                    _data[i][j] = -_data[i][j];
                }
            }
        }

        #endregion

        #region Array operation on matrices

        /// <summary>In place element-by-element multiplication.</summary>
        /// <remarks>This instance and <c>m</c> must have the same dimensions.</remarks>
        /// <seealso cref="ArrayMultiply(IMatrix, IMatrix)"/>
        public void ArrayMultiply(IMatrix m)
        {
            CheckMatchingMatrixDimensions(this, m);

            for(int i = 0; i < _rowCount; i++)
                for(int j = 0; j < _columnCount; j++)
                    _data[i][j] *= m[i, j];
        }

        /// <summary>Element-by-element multiplication.</summary>
        /// <remarks><c>m1</c> and <c>m2</c> must have the same dimensions.</remarks>
        /// <seealso cref="ArrayMultiply(IMatrix )"/>
        public static Matrix ArrayMultiply(IMatrix m1, IMatrix m2)
        {
            CheckMatchingMatrixDimensions(m1, m2);

            double[][] newData = CreateMatrixData(m1.RowCount, m1.ColumnCount);
            for(int i = 0; i < m1.RowCount; i++)
                for(int j = 0; j < m1.ColumnCount; j++)
                    newData[i][j] = m1[i, j] * m2[i, j];

            return new Matrix(newData);
        }

        /// <summary>In place element-by-element right division, <c>A ./= B</c>.</summary>
        public void ArrayDivide(IMatrix m)
        {
            CheckMatchingMatrixDimensions(this, m);

            for(int i = 0; i < _rowCount; i++)
                for(int j = 0; j < _columnCount; j++)
                    _data[i][j] /= m[i, j];
        }

        /// <summary>Element-by-element right division, <c>C = A./B</c>.</summary>
        public static Matrix ArrayDivide(IMatrix m1, IMatrix m2)
        {
            CheckMatchingMatrixDimensions(m1, m2);

            double[][] newData = CreateMatrixData(m1.RowCount, m1.ColumnCount);
            for(int i = 0; i < m1.RowCount; i++)
                for(int j = 0; j < m1.ColumnCount; j++)
                    newData[i][j] = m1[i, j] / m2[i, j];

            return new Matrix(newData);
        }

        #endregion

        #region Decompositions

        /// <summary>LU Decomposition</summary>
        /// <seealso cref="LUDecomposition"/>
        public virtual LUDecomposition LUD()
        {
            return new LUDecomposition(this);
        }

        /// <summary>QR Decomposition</summary>
        /// <returns>QRDecomposition</returns>
        /// <seealso cref="QRDecomposition"/>
        public virtual QRDecomposition QRD()
        {
            return new QRDecomposition(this);
        }

        /// <summary>Cholesky Decomposition</summary>
        /// <seealso cref="CholeskyDecomposition"/>
        public virtual CholeskyDecomposition chol()
        {
            return new CholeskyDecomposition(this);
        }

        /// <summary>Singular Value Decomposition</summary>
        /// <seealso cref="SingularValueDecomposition"/>
        public virtual SingularValueDecomposition SVD()
        {
            return new SingularValueDecomposition(this);
        }

        /// <summary>Eigenvalue Decomposition</summary>
        /// <seealso cref="EigenvalueDecomposition"/>
        public virtual EigenvalueDecomposition Eigen()
        {
            return new EigenvalueDecomposition(this);
        }

        #endregion

        /// <summary>Solve A*X = B against a Least Square (L2) criterion.</summary>
        /// <param name="B">right hand side</param>
        /// <returns>solution if A is square, least squares solution otherwise.</returns>
        /// <exception cref="InvalidOperationException">Matrix rank is deficient.</exception>
        public virtual Matrix Solve(Matrix B)
        {
            return (_rowCount == _columnCount ? (new LUDecomposition(this)).Solve(B) : (new QRDecomposition(this)).Solve(B));
        }

        /// <summary>Solve A*X = B against a Least Absolute Deviation (L1) criterion.</summary>
        /// <param name="B">right hand side</param>
        /// <returns>The implementation relies on the IRLS (iterated Re-weighted Least Square) algorithm.</returns>
        /// <exception cref="InvalidOperationException">Matrix rank is deficient.</exception>
        public virtual Matrix SolveRobust(Matrix B)
        {
            if(_rowCount == _columnCount)
            {
                return ((new LUDecomposition(this)).Solve(B));
            }

            double eta = 1.0e-12; // cut-off value to avoid instabilities in the IRLS convergence
            double epsilon = 1.0e-6; // convergence threshold to stop the iteration
            int maxIteration = 100;

            Matrix A = this;
            Matrix tA = this.Clone(); tA.Transpose();
            Matrix X = null;

            // G is a diagonal matrix - G is initialized as the identity matrix
            double[] G = new double[A.RowCount];
            for(int i = 0; i < G.Length; i++)
                G[i] = 1;

            //IRLS loop
            double maxChange = double.MaxValue;
            for(int k = 0; k < maxIteration && maxChange > epsilon; k++)
            {
                Matrix GA = this.Clone(); GA.Multiply(G);
                Matrix GB = B.Clone(); GB.Multiply(G);

                Matrix Ak = tA.Multiply(GA);
                Matrix Bk = tA.Multiply(GB);

                Matrix Xk = Ak.Solve(Bk);
                if(X != null)
                {
                    maxChange = double.MinValue;
                    for(int i = 0; i < X.RowCount; i++)
                    {
                        maxChange = Math.Max(maxChange, Math.Abs(X[i, 0] - Xk[i, 0]));
                    }
                }
                X = Xk;

                Matrix Rk = A.Multiply(Xk); Rk.UnaryMinus(); Rk.Add(B);

                // updating the weighting matrix
                for(int i = 0; i < B.RowCount; i++)
                {
                    double r = Math.Abs(Rk[i, 0]);
                    if(r < eta) r = eta;
                    G[i] = 1.0 / r;
                }
            }

            return X;
        }

        /// <summary>Solve X*A = B, which is also A'*X' = B'</summary>
        /// <param name="B">right hand side</param>
        /// <returns>solution if A is square, least squares solution otherwise.</returns>
        public virtual Matrix SolveTranspose(Matrix B)
        {
            return Transpose(this).Solve(Transpose(B));
        }

        /// <summary>Matrix inverse or pseudoinverse.</summary>
        /// <returns> inverse(A) if A is square, pseudoinverse otherwise.</returns>
        public virtual Matrix Inverse()
        {
            return Solve(Identity(_rowCount, _rowCount));
        }

        /// <summary>Matrix determinant</summary>
        public virtual double Determinant()
        {
            return new LUDecomposition(this).Determinant();
        }

        /// <summary>Matrix rank</summary>
        /// <returns>effective numerical rank, obtained from SVD.</returns>
        public virtual int Rank()
        {
            return new SingularValueDecomposition(this).Rank();
        }

        /// <summary>Matrix condition (2 norm)</summary>
        /// <returns>ratio of largest to smallest singular value.</returns>
        public virtual double Condition()
        {
            return new SingularValueDecomposition(this).Condition();
        }

        /// <summary>Matrix trace.</summary>
        /// <returns>sum of the diagonal elements.</returns>
        public virtual double Trace()
        {
            double t = 0;
            for(int i = 0; i < Math.Min(_rowCount, _columnCount); i++)
            {
                t += _data[i][i];
            }
            return t;
        }

        #endregion //  Public Methods

        #region Operator Overloading

        /// <summary>Gets or set the element indexed by <c>(i, j)</c>
        /// in the <c>Matrix</c>.</summary>
        /// <param name="i">Row index.</param>
        /// <param name="j">Column index.</param>
        public double this[int i, int j]
        {
            get { return _data[i][j]; }
            set { _data[i][j] = value; }
        }

        /// <summary>Addition of matrices</summary>
        public static Matrix operator +(Matrix m1, Matrix m2)
        {
            CheckMatchingMatrixDimensions(m1, m2);

            double[][] newData = CreateMatrixData(m1.RowCount, m1.ColumnCount);
            for(int i = 0; i < m1.RowCount; i++)
            {
                for(int j = 0; j < m1.ColumnCount; j++)
                {
                    newData[i][j] = m1[i, j] + m2[i, j];
                }
            }
            return new Matrix(newData);
        }

        /// <summary>Subtraction of matrices</summary>
        public static Matrix operator -(Matrix m1, Matrix m2)
        {
            CheckMatchingMatrixDimensions(m1, m2);

            double[][] newData = CreateMatrixData(m1.RowCount, m1.ColumnCount);
            for(int i = 0; i < m1.RowCount; i++)
            {
                for(int j = 0; j < m1.ColumnCount; j++)
                {
                    newData[i][j] = m1[i, j] - m2[i, j];
                }
            }
            return new Matrix(newData);
        }

        /// <summary>Linear algebraic matrix multiplication.</summary>
        /// <exception cref="System.ArgumentException">Matrix inner dimensions must agree.</exception>
        public static Matrix operator *(Matrix m1, Matrix m2)
        {
            if(m2.RowCount != m1.ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameDimensions);
            }

            double[][] newData = CreateMatrixData(m1.RowCount, m2.ColumnCount);
            for(int j = 0; j < m2.ColumnCount; j++)
            {
                for(int i = 0; i < m1.RowCount; i++)
                {
                    double s = 0;
                    for(int k = 0; k < m1.ColumnCount; k++)
                    {
                        s += m1[i, k] * m2[k, j];
                    }
                    newData[i][j] = s;
                }
            }
            return new Matrix(newData);
        }

        /// <summary>Multiplication of a matrix by a scalar, C = s*A</summary>
        public static Matrix operator *(double s, Matrix m)
        {
            double[][] newData = CreateMatrixData(m.RowCount, m.ColumnCount);
            for(int i = 0; i < m.RowCount; i++)
            {
                for(int j = 0; j < m.ColumnCount; j++)
                {
                    newData[i][j] = s * m[i, j];
                }
            }
            return new Matrix(newData);
        }

        /// <summary>Multiplication of a matrix by a scalar, C = s*A</summary>
        public static Matrix operator *(Matrix m, double s)
        {
            return s * m;
        }

        /// <summary>Implicit convertion to a <c>double[,]</c> array.</summary>
        [Obsolete("Convert to double[][] instead.")]
        public static explicit operator double[,](Matrix m)
        {
            double[,] newData = new double[m.RowCount, m.ColumnCount];
            for(int i = 0; i < m.RowCount; i++)
                for(int j = 0; i < m.ColumnCount; j++)
                    newData[i, j] = m[i, j];
            return newData;
        }

        /// <summary>Implicit convertion to a <c>double[][]</c> array.</summary>
        public static implicit operator double[][](Matrix m)
        {
            return m._data;
        }

        /// <summary>
        /// Explicit convertion to a <c>double[]</c> array of a single column matrix.
        /// </summary>
        /// <param name="m">Exactly one column expected.</param>
        public static explicit operator double[](Matrix m)
        {
            if(m.ColumnCount != 1) throw new InvalidOperationException(
                Resources.ArgumentMatrixSingleColumn);

            double[] array = new double[m.RowCount];
            for(int i = 0; i < m.RowCount; i++)
                array[i] = m[i, 0];

            return array;
        }

        /// <summary>
        /// Excplicit conversion to a <c>double</c> scalar of a single column and row (1-by-1) matrix.
        /// </summary>
        /// <param name="m">1-by-1 Matrix</param>
        public static explicit operator double(Matrix m)
        {
            if(m.ColumnCount != 1 || m.RowCount != 1) throw new InvalidOperationException(
                Resources.ArgumentMatrixSingleColumnRow);

            return m[0, 0];
        }

        #endregion   //Operator Overloading

        #region	 Private Methods

        /// <summary>Check if size(A) == size(B) *</summary>
        private static void CheckMatchingMatrixDimensions(IMatrix A, IMatrix B)
        {
            if(A.RowCount != B.RowCount || A.ColumnCount != B.ColumnCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameDimensions);
            }
        }
        #endregion //  Private Methods

        private static double[][] CloneMatrixData(double[][] data)
        {
            int rows, columns;
            GetRowColumnCount(data, out rows, out columns);
            double[][] newData = new double[rows][];
            for(int i = 0; i < rows; i++)
            {
                double[] col = new double[columns];
                for(int j = 0; j < columns; j++)
                {
                    col[j] = data[i][j];
                }
                newData[i] = col;
            }
            return newData;
        }
        /// <summary>Returns a deep copy of this instance.</summary>
        public Matrix Clone()
        {
            return new Matrix(CloneMatrixData(_data));
        }

        object ICloneable.Clone()
        {
            return Clone();
        }

        public static bool AlmostEqual(Matrix X, Matrix Y, double relativeAccuracy)
        {
            return Number.AlmostEqualNorm(X.Norm1(), Y.Norm1(), (X - Y).Norm1(), relativeAccuracy);
        }
        public static bool AlmostEqual(Matrix X, Matrix Y)
        {
            return Number.AlmostEqualNorm(X.Norm1(), Y.Norm1(), (X - Y).Norm1(), 10 * Number.DefaultRelativeAccuracy);
        }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();
            for(int i = 0; i < _rowCount; i++)
            {
                if(i == 0)
                    sb.Append("[[");
                else
                    sb.Append(" [");
                for(int j = 0; j < _columnCount; j++)
                {
                    if(j != 0)
                        sb.Append(',');
                    sb.Append(_data[i][j]);
                }
                if(i == _rowCount - 1)
                    sb.Append("]]");
                else
                    sb.AppendLine("]");
            }
            return sb.ToString();
        }
    }
}
