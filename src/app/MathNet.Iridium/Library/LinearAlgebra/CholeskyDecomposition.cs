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
        /// <summary>Array for internal storage of decomposition.</summary>
        Matrix _l;

        /// <summary>Symmetric and positive definite flag.</summary>
        readonly bool _isSymmetricPositiveDefinite;

        /// <summary>Cholesky algorithm for symmetric and positive definite matrix.</summary>
        /// <param name="m">Square, symmetric matrix.</param>
        /// <returns>Structure to access L and isspd flag.</returns>
        public
        CholeskyDecomposition(
            Matrix m
            )
        {
            if(m.RowCount != m.ColumnCount)
            {
                throw new InvalidOperationException(Resources.ArgumentMatrixSquare);
            }

            double[][] A = m;
            double[][] L = Matrix.CreateMatrixData(m.RowCount, m.RowCount);

            _isSymmetricPositiveDefinite = true; // ensure square

            for(int i = 0; i < L.Length; i++)
            {
                double diagonal = 0.0;
                for(int j = 0; j < i; j++)
                {
                    double sum = A[i][j];
                    for(int k = 0; k < j; k++)
                    {
                        sum -= L[j][k] * L[i][k];
                    }

                    L[i][j] = sum /= L[j][j];
                    diagonal += sum * sum;

                    _isSymmetricPositiveDefinite &= (A[j][i] == A[i][j]); // ensure symmetry
                }

                diagonal = A[i][i] - diagonal;
                L[i][i] = Math.Sqrt(Math.Max(diagonal, 0.0));

                _isSymmetricPositiveDefinite &= (diagonal > 0.0); // ensure positive definite

                // zero out resulting upper triangle.
                for(int j = i + 1; j < L.Length; j++)
                {
                    L[i][j] = 0.0;
                }
            }

            _l = new Matrix(L);
        }

        /// <summary>Is the matrix symmetric and positive definite?</summary>
        /// <returns><c>true</c> if A is symmetric and positive definite.</returns>
        public bool IsSPD
        {
            // TODO: NUnit test
            get { return _isSymmetricPositiveDefinite; }
        }

        /// <summary>Return triangular factor.</summary>
        /// <returns>L</returns>
        [Obsolete("Use the TriangularFactor property instead")]
        public
        Matrix
        GetL()
        {
            return _l;
        }

        /// <summary>
        /// Decomposition Triangular Factor Matrix (L).
        /// </summary>
        public Matrix TriangularFactor
        {
            get { return _l; }
        }

        /// <summary>Solve A*x = b</summary>
        /// <param name="b">A Vector with a dimension as high as the number of rows of A.</param>
        /// <returns>x so that L*L'*x = b</returns>
        /// <exception cref="System.ArgumentException">Matrix row dimensions must agree.</exception>
        /// <exception cref="System.InvalidOperationException">Matrix is not symmetric positive definite.</exception>
        public
        Vector
        Solve(
            Vector b
            )
        {
            if(b.Length != _l.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameRowDimension, "b");
            }

            if(!_isSymmetricPositiveDefinite)
            {
                throw new InvalidOperationException(Resources.ArgumentMatrixSymmetricPositiveDefinite);
            }

            double[][] L = _l;
            double[] bb = b;
            double[] x = new double[b.Length];

            // Solve L*y = b
            for(int i = 0; i < x.Length; i++)
            {
                double sum = bb[i];
                for(int k = i - 1; k >= 0; k--)
                {
                    sum -= L[i][k] * x[k];
                }

                x[i] = sum / L[i][i];
            }

            // Solve L'*x = y
            for(int i = x.Length - 1; i >= 0; i--)
            {
                double sum = x[i];
                for(int k = i + 1; k < x.Length; k++)
                {
                    sum -= L[k][i] * x[k];
                }

                x[i] = sum / L[i][i];
            }

            return new Vector(x);
        }

        /// <summary>Solve A*X = B</summary>
        /// <param name="B">A Matrix with as many rows as A and any number of columns.</param>
        /// <returns>X so that L*L'*X = B</returns>
        /// <exception cref="System.ArgumentException">Matrix row dimensions must agree.</exception>
        /// <exception cref="System.InvalidOperationException">Matrix is not symmetric positive definite.</exception>
        public
        Matrix
        Solve(
            Matrix B
            )
        {
            if(B.RowCount != _l.RowCount)
            {
                throw new ArgumentException(Resources.ArgumentMatrixSameRowDimension, "B");
            }

            if(!_isSymmetricPositiveDefinite)
            {
                throw new InvalidOperationException(Resources.ArgumentMatrixSymmetricPositiveDefinite);
            }

            int nx = B.ColumnCount;
            double[][] L = _l;
            double[][] BB = B;
            double[][] X = Matrix.CreateMatrixData(L.Length, nx);

            // Solve L*Y = B
            for(int i = 0; i < X.Length; i++)
            {
                for(int j = 0; j < nx; j++)
                {
                    double sum = BB[i][j];

                    for(int k = i - 1; k >= 0; k--)
                    {
                        sum -= L[i][k] * X[k][j];
                    }

                    X[i][j] = sum / L[i][i];
                }
            }

            // Solve L'*x = y
            for(int i = X.Length - 1; i >= 0; i--)
            {
                for(int j = 0; j < nx; j++)
                {
                    double sum = X[i][j];

                    for(int k = i + 1; k < X.Length; k++)
                    {
                        sum -= L[k][i] * X[k][j];
                    }

                    X[i][j] = sum / L[i][i];
                }
            }

            return new Matrix(X);
        }
    }
}
