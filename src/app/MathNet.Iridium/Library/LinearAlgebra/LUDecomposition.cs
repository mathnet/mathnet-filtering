#region Math.NET Iridium (LGPL) by Vermorel, Ruegg + Contributors
// Math.NET Iridium, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2004-2008, Joannes Vermorel, http://www.vermorel.com
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
using MathNet.Numerics.Properties;

namespace MathNet.Numerics.LinearAlgebra
{

    /// <summary>LU Decomposition.</summary>
    /// <remarks>
    /// For an m-by-n matrix A with m >= n, the LU decomposition is an m-by-n
    /// unit lower triangular matrix L, an n-by-n upper triangular matrix U,
    /// and a permutation vector pivot of length m so that A(piv,:) = L*U.
    /// <c> If m &lt; n, then L is m-by-m and U is m-by-n. </c>
    /// The LU decomposition with pivoting always exists, even if the matrix is
    /// singular, so the constructor will never fail.  The primary use of the
    /// LU decomposition is in the solution of square systems of simultaneous
    /// linear equations.  This will fail if IsNonSingular() returns false.
    /// </remarks>
    [Serializable]
    public class LUDecomposition
    {
        #region Class variables

        /// <summary>Array for internal storage of decomposition.</summary>
        private double[][] LU;

        /// <summary>Row dimensions.</summary>
        private readonly int _rowCount;

        /// <summary>Column dimensions.</summary>
        private readonly int _columnCount;

        /// <summary>Pivot sign.</summary>
        private int pivsign;

        /// <summary>Internal storage of pivot vector.</summary>
        private int[] piv;

        #endregion

        #region Constructor

        /// <summary>LU Decomposition</summary>
        /// <param name="A">Rectangular matrix</param>
        /// <returns>Structure to access L, U and piv.</returns>
        public LUDecomposition(Matrix A)
        {
            // TODO: it is usually considered as a poor practice to execute algorithms within a constructor.

            // Use a "left-looking", dot-product, Crout/Doolittle algorithm.

            LU = A.Clone();
            _rowCount = A.RowCount;
            _columnCount = A.ColumnCount;

            piv = new int[_rowCount];
            for(int i = 0; i < _rowCount; i++)
            {
                piv[i] = i;
            }
            pivsign = 1;
            //double[] LUrowi;
            double[] LUcolj = new double[_rowCount];

            // Outer loop.

            for(int j = 0; j < _columnCount; j++)
            {

                // Make a copy of the j-th column to localize references.

                for(int i = 0; i < LUcolj.Length; i++)
                {
                    LUcolj[i] = LU[i][j];
                }

                // Apply previous transformations.

                for(int i = 0; i < LUcolj.Length; i++)
                {
                    //LUrowi = LU[i];

                    // Most of the time is spent in the following dot product.

                    int kmax = Math.Min(i, j);
                    double s = 0.0;
                    for(int k = 0; k < kmax; k++)
                    {
                        s += LU[i][k] * LUcolj[k];
                    }

                    LU[i][j] = LUcolj[i] -= s;
                }

                // Find pivot and exchange if necessary.

                int p = j;
                for(int i = j + 1; i < LUcolj.Length; i++)
                {
                    if(Math.Abs(LUcolj[i]) > Math.Abs(LUcolj[p]))
                    {
                        p = i;
                    }
                }
                if(p != j)
                {
                    for(int k = 0; k < _columnCount; k++)
                    {
                        double t = LU[p][k]; LU[p][k] = LU[j][k]; LU[j][k] = t;
                    }
                    int k2 = piv[p]; piv[p] = piv[j]; piv[j] = k2;
                    pivsign = -pivsign;
                }

                // Compute multipliers.

                if((j < _rowCount) && (LU[j][j] != 0.0))
                {
                    for(int i = j + 1; i < _rowCount; i++)
                    {
                        LU[i][j] /= LU[j][j];
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
                for(int j = 0; j < _columnCount; j++)
                    if(LU[j][j] == 0) return false;

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

                double[][] L = Matrix.CreateMatrixData(_rowCount, _columnCount);
                for(int i = 0; i < L.Length; i++)
                {
                    for(int j = 0; j < _columnCount; j++)
                    {
                        if(i > j)
                        {
                            L[i][j] = LU[i][j];
                        }
                        else if(i == j)
                        {
                            L[i][j] = 1.0;
                        }
                        else
                        {
                            L[i][j] = 0.0;
                        }
                    }
                }
                return new Matrix(L);
            }
        }

        /// <summary>Gets upper triangular factor.</summary>
        public Matrix U
        {
            get
            {
                // TODO: bad behavior of this property
                // this property does not always return the same matrix

                double[][] U = Matrix.CreateMatrixData(_columnCount, _columnCount);
                for(int i = 0; i < _columnCount; i++)
                {
                    for(int j = 0; j < _columnCount; j++)
                    {
                        if(i <= j)
                        {
                            U[i][j] = LU[i][j];
                        }
                        else
                        {
                            U[i][j] = 0.0;
                        }
                    }
                }
                return new Matrix(U);
            }
        }

        /// <summary>Gets pivot permutation vector</summary>
        public int[] Pivot
        {
            get
            {
                // TODO: bad behavior of this property
                // this property does not always return the same matrix

                int[] p = new int[_rowCount];
                for(int i = 0; i < _rowCount; i++)
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

                double[] vals = new double[_rowCount];
                for(int i = 0; i < _rowCount; i++)
                {
                    vals[i] = (double)piv[i];
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
            if(_rowCount != _columnCount)
                throw new System.ArgumentException(Resources.ArgumentMatrixSquare);

            double d = (double)pivsign;
            for(int j = 0; j < _columnCount; j++)
            {
                d *= LU[j][j];
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
            if(B.RowCount != _rowCount)
                throw new ArgumentException(Resources.ArgumentMatrixSameRowDimension, "B");
            if(!this.IsNonSingular)
                throw new InvalidOperationException(Resources.ArgumentMatrixNotSingular);

            // Copy right hand side with pivoting
            int nx = B.ColumnCount;
            Matrix Xmat = B.GetMatrix(piv, 0, nx - 1);
            double[][] X = Xmat;

            // Solve L*Y = B(piv,:)
            for(int k = 0; k < _columnCount; k++)
            {
                for(int i = k + 1; i < _columnCount; i++)
                {
                    for(int j = 0; j < nx; j++)
                    {
                        X[i][j] -= X[k][j] * LU[i][k];
                    }
                }
            }
            // Solve U*X = Y;
            for(int k = _columnCount - 1; k >= 0; k--)
            {
                for(int j = 0; j < nx; j++)
                {
                    X[k][j] /= LU[k][k];
                }
                for(int i = 0; i < k; i++)
                {
                    for(int j = 0; j < nx; j++)
                    {
                        X[i][j] -= X[k][j] * LU[i][k];
                    }
                }
            }
            return Xmat;
        }

        #endregion //  Public Methods
    }
}