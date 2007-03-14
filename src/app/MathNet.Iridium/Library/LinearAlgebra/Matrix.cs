#region Math.NET Iridium (LGPL) by MathWorks, NIST, Vermorel, Ruegg
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2007, Joannes Vermorel, http://www.vermorel.com
//                     Christoph Rüegg,  http://christoph.ruegg.name
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

namespace MathNet.Numerics.LinearAlgebra
{
	/// <summary>Real matrix.</summary>
	/// <remarks>
	/// The class <c>Matrix</c> provides the elementary operations
	/// on matrices (addition, multiplication, inversion, transposition, ...).
	/// Helpers to handle sub-matrices are also provided.
	/// </remarks>
	[Serializable]
	public class Matrix : ICloneable
	{
		// TODO: refactor Matrix.A into Matrix.array -- Done: TParker 8 Dec 2005

		/// <summary>Array for internal storage of elements.</summary>
		private double[,] array;

		// TODO: get rid of the 'm' and 'n' properties

		/// <summary>Row dimension.</summary>
		/// <seealso cref="RowCount"/>
		private int rowCount
		{
			get { return array.GetLength(0); }
		}

		/// <summary>Column dimension.</summary>
		/// <seealso cref="ColumnCount"/>
		private int columnCount
		{
			get { return array.GetLength(1); }
		}
		
		#region Constructors and static construtive methods
		
		/// <summary>Construct an m-by-n matrix of zeros. </summary>
		/// <param name="m">Number of rows.</param>
		/// <param name="n">Number of colums.</param>
		public Matrix(int m, int n)
		{
			array = new double[m, n];
		}
		
		/// <summary>Construct an m-by-n constant matrix.</summary>
		/// <param name="m">Number of rows.</param>
		/// <param name="n">Number of colums.</param>
		/// <param name="s">Fill the matrix with this scalar value.</param>
		public Matrix(int m, int n, double s)
		{
			array = new double[m, n];
			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					array[i, j] = s;
				}
			}
		}
		
		/// <summary>Constructs a matrix from a 2-D array.</summary>
		/// <param name="A">Two-dimensional array of doubles.</param>
		/// <exception cref="System.ArgumentException">All rows must have the same length.</exception>
		/// <seealso cref="Create"/>
		public Matrix(double[,] A)
		{
			this.array = A;
		}
		
		/// <summary>Construct a matrix from a one-dimensional packed array</summary>
		/// <param name="vals">One-dimensional array of doubles, packed by columns (ala Fortran).</param>
		/// <param name="m">Number of rows.</param>
		/// <exception cref="System.ArgumentException">Array length must be a multiple of m.</exception>
		public Matrix(double[] vals, int m)
		{
			int n = (m != 0?vals.Length / m:0);
			if (m * n != vals.Length)
			{
				throw new System.ArgumentException("Array length must be a multiple of m.");
			}

			array = new double[m, n];
			for (int i = 0; i < m; i++)
			{
				for (int j = 0; j < n; j++)
				{
					array[i, j] = vals[i + j * m];
				}
			}
		}

		/// <summary>Constructs a matrix from a copy of a 2-D array.</summary>
		/// <param name="A">Two-dimensional array of doubles.</param>
		public static Matrix Create(double[,] A)
		{
			return (new Matrix(A)).Clone();
		}

		/// <summary>Generates identity matrix</summary>
		/// <param name="m">Number of rows.</param>
		/// <param name="n">Number of colums.</param>
		/// <returns>An m-by-n matrix with ones on the diagonal and zeros elsewhere.</returns>
		public static Matrix Identity(int m, int n)
		{
			Matrix X = new Matrix(m, n);
			for (int i = 0; i < m; i++)
				for (int j = 0; j < n; j++)
					X[i, j] = (i == j ? 1.0 : 0.0);
			
			return X;
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
            return new Matrix(m, m, 0.0);
        }

		/// <summary>Generates matrix with random elements</summary>
		/// <param name="m">Number of rows.</param>
		/// <param name="n">Number of colums.</param>
		/// <returns>An m-by-n matrix with uniformly distributed
		/// random elements in <c>[0, 1)</c> interval.</returns>
		public static Matrix Random(int m, int n)
		{
			System.Random random = new System.Random();

			Matrix X = new Matrix(m, n);
			for (int i = 0; i < m; i++)
				for (int j = 0; j < n; j++)
					X[i, j] = random.NextDouble();

			return X;
		}

		#endregion //  Constructors
		
		/// <summary>Gets the number of rows.</summary>
		public int RowCount
		{
			get { return array.GetLength(0); }
		}

		/// <summary>Gets the number of columns.</summary>
		public int ColumnCount
		{
			get { return array.GetLength(1); }
		}

		
		#region	 Public Methods

		#region Sub-matrices operation
		
		/// <summary>Gets a submatrix.</summary>
		/// <param name="i0">Initial row index.</param>
		/// <param name="i1">Final row index.</param>
		/// <param name="j0">Initial column index.</param>
		/// <param name="j1">Final column index.</param>
		/// <returns>A(i0:i1,j0:j1)</returns>
		/// <exception cref="System.IndexOutOfRangeException">Submatrix indices</exception>
		public virtual Matrix GetMatrix(int i0, int i1, int j0, int j1)
		{
			Matrix X = new Matrix(i1 - i0 + 1, j1 - j0 + 1);
			double[,] B = X;
			try
			{
				for (int i = i0; i <= i1; i++)
				{
					for (int j = j0; j <= j1; j++)
					{
						B[i - i0, j - j0] = array[i, j];
					}
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new System.IndexOutOfRangeException("Submatrix indices", e);
			}
			return X;
		}
		
		/// <summary>Gets a submatrix.</summary>
		/// <param name="r">Array of row indices.</param>
		/// <param name="c">Array of column indices.</param>
		/// <returns>A(r(:),c(:))</returns>
		/// <exception cref="System.IndexOutOfRangeException">Submatrix indices.</exception>
		public virtual Matrix GetMatrix(int[] r, int[] c)
		{
			Matrix X = new Matrix(r.Length, c.Length);
			double[,] B = X;
			try
			{
				for (int i = 0; i < r.Length; i++)
				{
					for (int j = 0; j < c.Length; j++)
					{
						B[i, j] = array[r[i], c[j]];
					}
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new System.IndexOutOfRangeException("Submatrix indices", e);
			}
			return X;
		}
		
		/// <summary>Get a submatrix.</summary>
		/// <param name="i0">Initial row index.</param>
		/// <param name="i1">Final row index.</param>
		/// <param name="c">Array of column indices.</param>
		/// <returns>A(i0:i1,c(:))</returns>
		/// <exception cref="System.IndexOutOfRangeException">Submatrix indices.</exception>
		public virtual Matrix GetMatrix(int i0, int i1, int[] c)
		{
			Matrix X = new Matrix(i1 - i0 + 1, c.Length);
			double[,] B = X;
			try
			{
				for (int i = i0; i <= i1; i++)
				{
					for (int j = 0; j < c.Length; j++)
					{
						B[i - i0, j] = array[i, c[j]];
					}
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new System.IndexOutOfRangeException("Submatrix indices", e);
			}
			return X;
		}
		
		/// <summary>Get a submatrix.</summary>
		/// <param name="r">Array of row indices.</param>
		/// <param name="j0">Initial column index.</param>
		/// <param name="j1">Final column index.</param>
		/// <returns>A(r(:),j0:j1)</returns>
		/// <exception cref="System.IndexOutOfRangeException">Submatrix indices.</exception>
		public virtual Matrix GetMatrix(int[] r, int j0, int j1)
		{
			Matrix X = new Matrix(r.Length, j1 - j0 + 1);
			double[,] B = X;
			try
			{
				for (int i = 0; i < r.Length; i++)
				{
					for (int j = j0; j <= j1; j++)
					{
						B[i, j - j0] = array[r[i], j];
					}
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new System.IndexOutOfRangeException("Submatrix indices", e);
			}
			return X;
		}
		
		/// <summary>Set a submatrix.</summary>
		/// <param name="i0">Initial row index.</param>
		/// <param name="i1">Final row index.</param>
		/// <param name="j0">Initial column index.</param>
		/// <param name="j1">Final column index.</param>
		/// <param name="X">A(i0:i1,j0:j1)</param>
		/// <exception cref="System.IndexOutOfRangeException">Submatrix indices.</exception>
		public virtual void  SetMatrix(int i0, int i1, int j0, int j1, Matrix X)
		{
			try
			{
				for (int i = i0; i <= i1; i++)
				{
					for (int j = j0; j <= j1; j++)
					{
						array[i, j] = X[i - i0, j - j0];
					}
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new System.IndexOutOfRangeException("Submatrix indices", e);
			}
		}
		
		/// <summary>Sets a submatrix.</summary>
		/// <param name="r">Array of row indices.</param>
		/// <param name="c">Array of column indices.</param>
		/// <param name="X">A(r(:),c(:))</param>
		/// <exception cref="System.IndexOutOfRangeException">Submatrix indices</exception>
		public virtual void  SetMatrix(int[] r, int[] c, Matrix X)
		{
			try
			{
				for (int i = 0; i < r.Length; i++)
				{
					for (int j = 0; j < c.Length; j++)
					{
						array[r[i], c[j]] = X[i, j];
					}
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new System.IndexOutOfRangeException("Submatrix indices", e);
			}
		}
		
		/// <summary>Sets a submatrix.</summary>
		/// <param name="r">Array of row indices.</param>
		/// <param name="j0">Initial column index.</param>
		/// <param name="j1">Final column index.</param>
		/// <param name="X">A(r(:),j0:j1)</param>
		/// <exception cref="System.IndexOutOfRangeException">Submatrix indices</exception>
		public virtual void  SetMatrix(int[] r, int j0, int j1, Matrix X)
		{
			try
			{
				for (int i = 0; i < r.Length; i++)
				{
					for (int j = j0; j <= j1; j++)
					{
						array[r[i], j] = X[i, j - j0];
					}
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new System.IndexOutOfRangeException("Submatrix indices", e);
			}
		}
		
		/// <summary>Set a submatrix.</summary>
		/// <param name="i0">Initial row index.</param>
		/// <param name="i1">Final row index.</param>
		/// <param name="c">Array of column indices.</param>
		/// <param name="X">A(i0:i1,c(:))</param>
		/// <exception cref="System.IndexOutOfRangeException">Submatrix indices.</exception>
		public virtual void  SetMatrix(int i0, int i1, int[] c, Matrix X)
		{
			try
			{
				for (int i = i0; i <= i1; i++)
				{
					for (int j = 0; j < c.Length; j++)
					{
						array[i, c[j]] = X[i - i0, j];
					}
				}
			}
			catch (System.IndexOutOfRangeException e)
			{
				throw new System.IndexOutOfRangeException("Submatrix indices", e);
			}
		}
		

		#endregion
		
		#region Norms computations

		/// <summary>One norm</summary>
		/// <returns>Maximum column sum.</returns>
		public double Norm1()
		{
			double f = 0;
			for (int j = 0; j < columnCount; j++)
			{
				double s = 0;
				for (int i = 0; i < rowCount; i++)
				{
					s += System.Math.Abs(array[i, j]);
				}
				f = System.Math.Max(f, s);
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
			for (int i = 0; i < rowCount; i++)
			{
				double s = 0;
				for (int j = 0; j < columnCount; j++)
				{
					s += System.Math.Abs(array[i, j]);
				}
				f = System.Math.Max(f, s);
			}
			return f;
		}
		
		/// <summary>Frobenius norm</summary>
		/// <returns>Sqrt of sum of squares of all elements.</returns>
		public double NormF()
		{
			double f = 0;
			for (int i = 0; i < rowCount; i++)
			{
				for (int j = 0; j < columnCount; j++)
				{
                    f = Fn.Hypot(f, array[i, j]);
				}
			}
			return f;
		}
		

		#endregion
		
		#region Elementary linear operations

		/// <summary>In place addition of <c>m</c> to this <c>Matrix</c>.</summary>
		/// <seealso cref="operator + (Matrix, Matrix)"/>
		public virtual void Add(Matrix m)
		{
			CheckMatrixDimensions(m);
			for (int i = 0; i < this.rowCount; i++)
			{
				for (int j = 0; j < this.columnCount; j++)
				{
					this[i, j] += m[i, j];
				}
			}
		}

		/// <summary>Multiplies in place this <c>Matrix</c> by a scalar.</summary>
		/// <seealso cref="operator * (double, Matrix)"/>
		public virtual void Multiply(double s)
		{
			for (int i = 0; i < rowCount; i++)
			{
				for (int j = 0; j < columnCount; j++)
				{
					this[i, j] *= s;
				}
			}
		}
		
		/// <summary>In place substraction of <c>m</c> to this <c>Matrix</c>.</summary>
		/// <seealso cref="operator - (Matrix, Matrix)"/>
		public virtual void Subtract(Matrix m)
		{
			CheckMatrixDimensions(m);
			for (int i = 0; i < this.rowCount; i++)
			{
				for (int j = 0; j < this.columnCount; j++)
				{
					this[i, j] -= m[i, j];
				}
			}
		}

		/// <summary>In place transposition of this <c>Matrix</c>.</summary>
		/// <seealso cref="Transpose(Matrix)"/>
		public virtual void Transpose()
		{
			// TODO: test this method

			if(this.RowCount == this.ColumnCount)
			{
				// No need for array copy
				for(int i = 0; i < this.RowCount; i++)
					for(int j = i + 1; j < this.ColumnCount; j++)
					{
						double thisIJ = this[i, j];
						this[i, j] = this[j, i];
						this[j, i] = thisIJ;
					}
			}
			else
			{
				Matrix X = new Matrix(this.ColumnCount, this.RowCount);
				for (int i = 0; i < this.RowCount; i++)
					for (int j = 0; j < this.ColumnCount; j++)
						X[j, i] = this[i, j];

				this.array = X.array;
			}
		}

		/// <summary>Gets the transposition of the provided <c>Matrix</c>.</summary>
		public static Matrix Transpose(Matrix m)
		{
			Matrix X = new Matrix(m.columnCount, m.rowCount);
			for (int i = 0; i < m.rowCount; i++)
			{
				for (int j = 0; j < m.columnCount; j++)
				{
					X[j, i] = m[i, j];
				}
			}
			return X;
		}

		/// <summary>In place unary minus of the <c>Matrix</c>.</summary>
		public virtual void UnaryMinus()
		{
			for (int i = 0; i < rowCount; i++)
			{
				for (int j = 0; j < columnCount; j++)
				{
					this[i, j] = - this[i, j];
				}
			}
		}

		#endregion

		#region Array operation on matrices
		
		/// <summary>In place element-by-element multiplication.</summary>
		/// <remarks>This instance and <c>m</c> must have the same dimensions.</remarks>
		/// <seealso cref="ArrayMultiply(Matrix, Matrix)"/>
		public void ArrayMultiply(Matrix m)
		{
			this.CheckMatrixDimensions(m);

			for (int i = 0; i < this.RowCount; i++)
				for (int j = 0; j < this.ColumnCount; j++)
					this[i, j] *= m[i, j];
		}

		/// <summary>Element-by-element multiplication.</summary>
		/// <remarks><c>m1</c> and <c>m2</c> must have the same dimensions.</remarks>
		/// <seealso cref="ArrayMultiply(Matrix )"/>
		public static Matrix ArrayMultiply(Matrix m1, Matrix m2)
		{
			m1.CheckMatrixDimensions(m2);

			Matrix X = new Matrix(m1.RowCount, m1.ColumnCount);
			for (int i = 0; i < m1.RowCount; i++)
				for (int j = 0; j < m1.ColumnCount; j++)
					X[i, j] = m1[i, j] * m2[i, j];

			return X;
		}
		
		/// <summary>In place element-by-element right division, <c>A ./= B</c>.</summary>
		public void ArrayDivide(Matrix m)
		{
			this.CheckMatrixDimensions(m);

			for (int i = 0; i < this.RowCount; i++)
				for (int j = 0; j < this.ColumnCount; j++)
					this[i, j] /= m[i, j];
		}

		/// <summary>Element-by-element right division, <c>C = A./B</c>.</summary>
		public static Matrix ArrayDivide(Matrix m1, Matrix m2)
		{
			m1.CheckMatrixDimensions(m2);

			Matrix X = new Matrix(m1.RowCount, m1.ColumnCount);
			for (int i = 0; i < m1.RowCount; i++)
				for (int j = 0; j < m1.ColumnCount; j++)
					X[i, j] = m1[i, j] / m2[i, j];

			return X;
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
		
		/// <summary>Solve A*X = B</summary>
		/// <param name="B">right hand side</param>
		/// <returns>solution if A is square, least squares solution otherwise.</returns>
		public virtual Matrix Solve(Matrix B)
		{
			return (rowCount == columnCount ? (new LUDecomposition(this)).Solve(B):(new QRDecomposition(this)).Solve(B));
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
			return Solve(Identity(rowCount, rowCount));
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
			for (int i = 0; i < System.Math.Min(rowCount, columnCount); i++)
			{
				t += array[i, i];
			}
			return t;
		}
		
		#endregion //  Public Methods

		#region Operator Overloading

		/// <summary>Gets or set the element indexed by <c>(i, j)</c>
		/// in the <c>Matrix</c>.</summary>
		/// <param name="i">Row index.</param>
		/// <param name="j">Column index.</param>
		public double this [int i, int j]
		{
			get { return array[i, j]; }
			set { array[i, j] = value; }
		}

		/// <summary>Addition of matrices</summary>
		public static Matrix operator +(Matrix m1, Matrix m2)
		{
			m1.CheckMatrixDimensions(m2);
			Matrix X = new Matrix(m1.rowCount, m1.columnCount);
			for (int i = 0; i < m1.rowCount; i++)
			{
				for (int j = 0; j < m1.columnCount; j++)
				{
					X[i, j] = m1[i, j] + m2[i, j];
				}
			}
			return X;
		}

		/// <summary>Subtraction of matrices</summary>
		public static Matrix operator -(Matrix m1, Matrix m2)
		{
			m1.CheckMatrixDimensions(m2);

			Matrix X = new Matrix(m1.rowCount, m1.columnCount);
			for (int i = 0; i < m1.rowCount; i++)
			{
				for (int j = 0; j < m1.columnCount; j++)
				{
					X[i, j] = m1[i, j] - m2[i, j];
				}
			}
			return X;
		}

		/// <summary>Linear algebraic matrix multiplication.</summary>
		/// <exception cref="System.ArgumentException">Matrix inner dimensions must agree.</exception>
		public static Matrix operator *(Matrix m1, Matrix m2)
		{
			if (m2.rowCount != m1.columnCount)
			{
				throw new System.ArgumentException("Matrix inner dimensions must agree.");
			}

			Matrix X = new Matrix(m1.rowCount, m2.columnCount);
			for (int j = 0; j < m2.columnCount; j++)
			{
				for (int i = 0; i < m1.rowCount; i++)
				{
					double s = 0;
					for (int k = 0; k < m1.columnCount; k++)
					{
						s += m1[i, k] * m2[k, j];
					}
					X[i, j] = s;
				}
			}
			return X;
		}

		/// <summary>Multiplication of a matrix by a scalar, C = s*A</summary>
		public static Matrix operator * (double s, Matrix m)
		{
			Matrix X = new Matrix(m.rowCount, m.columnCount);
			for (int i = 0; i < m.rowCount; i++)
			{
				for (int j = 0; j < m.columnCount; j++)
				{
					X[i, j] = s * m[i, j];
				}
			}
			return X;
		}

        /// <summary>Multiplication of a matrix by a scalar, C = s*A</summary>
        public static Matrix operator *(Matrix m, double s)
        {
            return s * m;
        }

		/// <summary>Implicit convertion to a <c>double[,]</c> array.</summary>
		public static implicit operator double[,] (Matrix m)
		{
			return m.array;
		}

		/// <summary>
		/// Explicit convertion to a <c>double[]</c> array of a single column matrix.
		/// </summary>
		/// <param name="m">Exactly one column expected.</param>
		public static explicit operator double[] (Matrix m)
		{
			if(m.ColumnCount != 1) throw new InvalidOperationException(
				"Bad dimensions for conversion to double array");

			double[] array = new double[m.RowCount];
			for(int i = 0; i < m.RowCount; i++)
				array[i] = m[i, 0];

			return array;
		}

        /// <summary>
        /// Excplicit conversion to a <c>double</c> scalar of a single column & row (1-by-1) matrix.
        /// </summary>
        /// <param name="m">1-by-1 Matrix</param>
        public static explicit operator double (Matrix m)
        {
            if(m.ColumnCount != 1 || m.rowCount != 1) throw new InvalidOperationException(
                "Bad dimensions for conversion to double");

            return m[0, 0];
        }

		#endregion   //Operator Overloading

		#region	 Private Methods
		
		/// <summary>Check if size(A) == size(B) *</summary>
		private void  CheckMatrixDimensions(Matrix B)
		{
			if (B.rowCount != rowCount || B.columnCount != columnCount)
			{
				throw new System.ArgumentException(Resources.ArgumentMatrixSimeDimensions);
			}
		}
		#endregion //  Private Methods


		/// <summary>Returns a deep copy of this instance.</summary>
		public Matrix Clone()
		{
			Matrix X = new Matrix(rowCount, columnCount);
			for (int i = 0; i < rowCount; i++)
			{
				for (int j = 0; j < columnCount; j++)
				{
					X[i, j] = array[i, j];
				}
			}
			return X;
		}
		object ICloneable.Clone()
		{
			return Clone();
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			for(int i=0;i<rowCount;i++)
			{
				if(i==0)
					sb.Append("[[");
				else
					sb.Append(" [");
				for(int j=0;j<columnCount;j++)
				{
					if(j != 0)
						sb.Append(',');
					sb.Append(this[i,j]);
				}
				if(i==rowCount-1)
					sb.Append("]]");
				else
					sb.Append("]\n");
			}
			return sb.ToString();
		}
	}
}
