#region Copyright ©2000 The MathWorks and NIST, ©2004 Joannes Vermorel

// MathNet Numerics, part of MathNet
//
// Copyright (c) 2004,	Joannes Vermorel, http://www.vermorel.com
//	
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

namespace MathNet.Numerics.LinearAlgebra
{
	
	/// <summary>LU Decomposition.</summary>
	/// <remarks>
	/// For an m-by-n matrix A with m >= n, the LU decomposition is an m-by-n
	/// unit lower triangular matrix L, an n-by-n upper triangular matrix U,
	/// and a permutation vector piv of length m so that A(piv,:) = L*U.
	/// <c> If m &lt; n, then L is m-by-m and U is m-by-n. </c>
	/// The LU decompostion with pivoting always exists, even if the matrix is
	/// singular, so the constructor will never fail.  The primary use of the
	/// LU decomposition is in the solution of square systems of simultaneous
	/// linear equations.  This will fail if IsNonSingular() returns false.
	/// </remarks>
	[Serializable]
	public class LUDecomposition
	{
		#region Class variables
		
		/// <summary>Array for internal storage of decomposition.</summary>
		private double[,] LU;
		
		/// <summary>Row dimensions.</summary>
		private int m
		{
			get { return LU.GetLength(0); }
		}
		
		/// <summary>Column dimensions.</summary>
		private int n
		{
			get { return LU.GetLength(1); }
		}
		
		/// <summary>Pivot sign.</summary>
		private int pivsign;
		
		/// <summary>Internal storage of pivot vector.</summary>
		private int[] piv;
		
		#endregion

		#region Constructor
		
		/// <summary>LU Decomposition</summary>
		/// <param name="A">  Rectangular matrix
		/// </param>
		/// <returns>     Structure to access L, U and piv.
		/// </returns>
		
		public LUDecomposition(Matrix A)
		{
			// Use a "left-looking", dot-product, Crout/Doolittle algorithm.
			
			LU = (Matrix) A.Clone();
			piv = new int[m];
			for (int i = 0; i < m; i++)
			{
				piv[i] = i;
			}
			pivsign = 1;
			//double[] LUrowi;
			double[] LUcolj = new double[m];
			
			// Outer loop.
			
			for (int j = 0; j < n; j++)
			{
				
				// Make a copy of the j-th column to localize references.
				
				for (int i = 0; i < m; i++)
				{
					LUcolj[i] = LU[i, j];
				}
				
				// Apply previous transformations.
				
				for (int i = 0; i < m; i++)
				{
					//LUrowi = LU[i];
					
					// Most of the time is spent in the following dot product.
					
					int kmax = System.Math.Min(i, j);
					double s = 0.0;
					for (int k = 0; k < kmax; k++)
					{
						s += LU[i,k] * LUcolj[k];
					}
					
					LU[i,j] = LUcolj[i] -= s;
				}
				
				// Find pivot and exchange if necessary.
				
				int p = j;
				for (int i = j + 1; i < m; i++)
				{
					if (System.Math.Abs(LUcolj[i]) > System.Math.Abs(LUcolj[p]))
					{
						p = i;
					}
				}
				if (p != j)
				{
					for (int k = 0; k < n; k++)
					{
						double t = LU[p, k]; LU[p, k] = LU[j, k]; LU[j, k] = t;
					}
					int k2 = piv[p]; piv[p] = piv[j]; piv[j] = k2;
					pivsign = - pivsign;
				}
				
				// Compute multipliers.
				
				if (j < m & LU[j, j] != 0.0)
				{
					for (int i = j + 1; i < m; i++)
					{
						LU[i, j] /= LU[j, j];
					}
				}
			}
		}
		#endregion //  Constructor
				
		#region Public Properties
		/// <summary>Indicates whether the matrix is nonsingular.</summary>
		/// <returns><c>true</c> if U, and hence A, is nonsingular.</returns>
		public bool IsNonSingular
		{
			get
			{
				for (int j = 0; j < n; j++)
					if (LU[j, j] == 0) return false;
	
				return true;
			}
		}

		/// <summary>Gets lower triangular factor.</summary>
		public Matrix L
		{
			get
			{
				// TODO: bad behavior of this property
				// this property does not always return the same matrix

				Matrix X = new Matrix(m, n);
				double[,] L = X;
				for (int i = 0; i < m; i++)
				{
					for (int j = 0; j < n; j++)
					{
						if (i > j)
						{
							L[i, j] = LU[i, j];
						}
						else if (i == j)
						{
							L[i, j] = 1.0;
						}
						else
						{
							L[i, j] = 0.0;
						}
					}
				}
				return X;
			}
		}

		/// <summary>Gets upper triangular factor.</summary>
		public Matrix U
		{
			get
			{
				// TODO: bad behavior of this property
				// this property does not always return the same matrix

				Matrix X = new Matrix(n, n);
				double[,] U = X;
				for (int i = 0; i < n; i++)
				{
					for (int j = 0; j < n; j++)
					{
						if (i <= j)
						{
							U[i, j] = LU[i, j];
						}
						else
						{
							U[i, j] = 0.0;
						}
					}
				}
				return X;
			}
		}

		/// <summary>Gets pivot permutation vector</summary>
		public int[] Pivot
		{
			get
			{
				// TODO: bad behavior of this property
				// this property does not always return the same matrix

				int[] p = new int[m];
				for (int i = 0; i < m; i++)
				{
					p[i] = piv[i];
				}
				return p;
			}
		}

		/// <summary>Returns pivot permutation vector as a one-dimensional double array.</summary>
		public double[] DoublePivot
		{
			get
			{
				// TODO: bad behavior of this property
				// this property does not always return the same matrix

				double[] vals = new double[m];
				for (int i = 0; i < m; i++)
				{
					vals[i] = (double) piv[i];
				}
				return vals;
			}
		}

		#endregion
		
		#region Public Methods
		
		/// <summary>Determinant</summary>
		/// <returns>det(A)</returns>
		/// <exception cref="System.ArgumentException">Matrix must be square</exception>
		public double Determinant()
		{
			if (m != n)
			{
				throw new System.ArgumentException("Matrix must be square.");
			}
			double d = (double) pivsign;
			for (int j = 0; j < n; j++)
			{
				d *= LU[j, j];
			}
			return d;
		}
		
		/// <summary>Solve A*X = B</summary>
		/// <param name="B">A Matrix with as many rows as A and any number of columns.</param>
		/// <returns>X so that L*U*X = B(piv,:)</returns>
		/// <exception cref="System.ArgumentException">Matrix row dimensions must agree.</exception>
		/// <exception cref="System.SystemException">Matrix is singular.</exception>
		public Matrix Solve(Matrix B)
		{
			if (B.RowCount != m)
			{
				throw new System.ArgumentException("Matrix row dimensions must agree.");
			}
			if (!this.IsNonSingular)
			{
				throw new System.SystemException("Matrix is singular.");
			}
			
			// Copy right hand side with pivoting
			int nx = B.ColumnCount;
			Matrix Xmat = B.GetMatrix(piv, 0, nx - 1);
			double[,] X = Xmat;
			
			// Solve L*Y = B(piv,:)
			for (int k = 0; k < n; k++)
			{
				for (int i = k + 1; i < n; i++)
				{
					for (int j = 0; j < nx; j++)
					{
						X[i, j] -= X[k, j] * LU[i, k];
					}
				}
			}
			// Solve U*X = Y;
			for (int k = n - 1; k >= 0; k--)
			{
				for (int j = 0; j < nx; j++)
				{
					X[k, j] /= LU[k, k];
				}
				for (int i = 0; i < k; i++)
				{
					for (int j = 0; j < nx; j++)
					{
						X[i, j] -= X[k, j] * LU[i, k];
					}
				}
			}
			return Xmat;
		}

		#endregion //  Public Methods
	}
}