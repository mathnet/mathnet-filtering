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
	/// <summary>Cholesky Decomposition.</summary>
	/// <remarks>
	/// For a symmetric, positive definite matrix A, the Cholesky decomposition
	/// is an lower triangular matrix L so that A = L*L'.
	/// If the matrix is not symmetric or positive definite, the constructor
	/// returns a partial decomposition and sets an internal flag that may
	/// be queried by the <see cref="CholeskyDecomposition.IsSPD"/> property.
	/// </remarks>
	[Serializable]
	public class CholeskyDecomposition
	{
		#region Class variables
		
		/// <summary>Array for internal storage of decomposition.</summary>
		private double[,] L;
		
		/// <summary>Row and column dimension (square matrix).</summary>
		private int n
		{
			get { return L.GetLength(0); }
		}
		
		/// <summary>Symmetric and positive definite flag.</summary>
		private bool isspd;
		
		#endregion //  Class variables

		#region Constructor
		
		/// <summary>Cholesky algorithm for symmetric and positive definite matrix.</summary>
		/// <param name="Arg">Square, symmetric matrix.</param>
		/// <returns>Structure to access L and isspd flag.</returns>
		public CholeskyDecomposition(Matrix Arg)
		{
			// Initialize.
			double[,] A = Arg;
			L = new double[Arg.RowCount, Arg.RowCount];

			isspd = (Arg.ColumnCount == n);
			// Main loop.
			for (int j = 0; j < n; j++)
			{
				//double[] Lrowj = L[j];
				double d = 0.0;
				for (int k = 0; k < j; k++)
				{
					//double[] Lrowk = L[k];
					double s = 0.0;
					for (int i = 0; i < k; i++)
					{
						s += L[k,i] * L[j,i];
					}
					L[j,k] = s = (A[j, k] - s) / L[k, k];
					d = d + s * s;
					isspd = isspd & (A[k, j] == A[j, k]);
				}
				d = A[j, j] - d;
				isspd = isspd & (d > 0.0);
				L[j, j] = System.Math.Sqrt(System.Math.Max(d, 0.0));
				for (int k = j + 1; k < n; k++)
				{
					L[j, k] = 0.0;
				}
			}
		}
		
		#endregion //  Constructor

		/// <summary>Is the matrix symmetric and positive definite?</summary>
		/// <returns><c>true</c> if A is symmetric and positive definite.</returns>
		public bool IsSPD
		{
			// TODO: NUnit test
			get { return isspd; }
		}
		
		#region Public Methods
		
		/// <summary>Return triangular factor.</summary>
		/// <returns>L</returns>
		public Matrix GetL()
		{
			return new Matrix(L);
		}
		
		/// <summary>Solve A*X = B</summary>
		/// <param name="B">  A Matrix with as many rows as A and any number of columns.</param>
		/// <returns>X so that L*L'*X = B</returns>
		/// <exception cref="System.ArgumentException">Matrix row dimensions must agree.</exception>
		/// <exception cref="System.SystemException">Matrix is not symmetric positive definite.</exception>
		public Matrix Solve(Matrix B)
		{
			if (B.RowCount != n)
			{
				throw new System.ArgumentException("Matrix row dimensions must agree.");
			}
			if (!isspd)
			{
				throw new System.SystemException("Matrix is not symmetric positive definite.");
			}
			
			// Copy right hand side.
			double[,] X = B.Clone();
			int nx = B.ColumnCount;
			
			// Solve L*Y = B;
			for (int k = 0; k < n; k++)
			{
				for (int i = k + 1; i < n; i++)
				{
					for (int j = 0; j < nx; j++)
					{
						X[i, j] -= X[k, j] * L[i, k];
					}
				}
				for (int j = 0; j < nx; j++)
				{
					X[k, j] /= L[k, k];
				}
			}
			
			// Solve L'*X = Y;
			for (int k = n - 1; k >= 0; k--)
			{
				for (int j = 0; j < nx; j++)
				{
					X[k, j] /= L[k, k];
				}
				for (int i = 0; i < k; i++)
				{
					for (int j = 0; j < nx; j++)
					{
						X[i, j] -= X[k, j] * L[k, i];
					}
				}
			}
			return new Matrix(X);
		}
		#endregion //  Public Methods
	}
}