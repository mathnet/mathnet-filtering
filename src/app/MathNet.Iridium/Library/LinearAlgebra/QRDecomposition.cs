//-----------------------------------------------------------------------
// <copyright file="QRDecomposition.cs" company="Math.NET Project">
//    Copyright (c) 2004-2008, Joannes Vermorel, Christoph Rüegg.
//    All Right Reserved.
// </copyright>
// <author>
//    Joannes Vermorel, http://www.vermorel.com
//    Christoph Rüegg, http://christoph.ruegg.name
// </author>
// <product>
//    Math.NET Iridium, part of the Math.NET Project.
//    http://mathnet.opensourcedotnet.info
// </product>
// <license type="opensource" name="LGPL" version="2 or later">
//    This program is free software; you can redistribute it and/or modify
//    it under the terms of the GNU Lesser General Public License as published 
//    by the Free Software Foundation; either version 2 of the License, or
//    any later version.
//
//    This program is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//    GNU Lesser General Public License for more details.
//
//    You should have received a copy of the GNU Lesser General Public 
//    License along with this program; if not, write to the Free Software
//    Foundation, Inc., 675 Mass Ave, Cambridge, MA 02139, USA.
// </license>
// <contribution>
//    The MathWorks
//    NIST
// </contribution>
//-----------------------------------------------------------------------

using System;

namespace MathNet.Numerics.LinearAlgebra
{
    /// <summary>
    /// QR Decomposition.
    /// </summary>
    /// <remarks>
    /// <para>
    /// For an m-by-n matrix A with m >= n, the QR decomposition is an m-by-n
    /// orthogonal matrix Q and an n-by-n upper triangular matrix R so that
    /// A = Q*R.
    /// </para>
    /// <para>
    /// The QR decomposition always exists, even if the matrix does not have
    /// full rank, so the constructor will never fail.  The primary use of the
    /// QR decomposition is in the least squares solution of non-square systems
    /// of simultaneous linear equations.  This will fail if <c>IsFullRank()</c>
    /// returns false.
    /// </para>
    /// </remarks>
    [Serializable]
    public class QRDecomposition
    {
        /// <summary>
        /// Array for internal storage of decomposition.
        /// </summary>
        double[][] QR;

        /// <summary>
        /// Array for internal storage of diagonal of R.
        /// </summary>
        double[] Rdiag;

        /// <summary>
        /// Row dimensions.
        /// </summary>
        private int M
        {
            get { return QR.Length; }
        }

        /// <summary>
        /// Column dimensions.
        /// </summary>
        private int N
        {
            get { return QR[0].Length; }
        }

        OnDemandComputation<bool> _fullRankOnDemand;
        OnDemandComputation<Matrix> _householderVectorsOnDemand;
        OnDemandComputation<Matrix> _upperTriangularFactorOnDemand;
        OnDemandComputation<Matrix> _orthogonalFactorOnDemand;

        /// <summary>
        /// Initializes a new instance of the QRDecomposition class,
        /// which decomposes the matrix by Householder reflections.
        /// </summary>
        /// <remarks>Provides access to R, the Householder vectors and computes Q.</remarks>
        /// <param name="A">Rectangular matrix</param>
        public
        QRDecomposition(
            Matrix A
            )
        {
            // TODO: it is usually considered as a poor practice to execute algorithms within a constructor.

            // Initialize.
            QR = A.Clone();
            Rdiag = new double[N];

            // Main loop.
            for(int k = 0; k < N; k++)
            {
                // Compute 2-norm of k-th column without under/overflow.
                double norm = 0;
                for(int i = k; i < M; i++)
                {
                    norm = Fn.Hypot(norm, QR[i][k]);
                }

                if(norm != 0.0)
                {
                    // Form k-th Householder vector.
                    if(QR[k][k] < 0)
                    {
                        norm = -norm;
                    }

                    for(int i = k; i < M; i++)
                    {
                        QR[i][k] /= norm;
                    }

                    QR[k][k] += 1.0;

                    // Apply transformation to remaining columns.
                    for(int j = k + 1; j < N; j++)
                    {
                        double s = 0.0;
                        for(int i = k; i < M; i++)
                        {
                            s += QR[i][k] * QR[i][j];
                        }

                        s = (-s) / QR[k][k];
                        for(int i = k; i < M; i++)
                        {
                            QR[i][j] += s * QR[i][k];
                        }
                    }
                }

                Rdiag[k] = -norm;
            }

            InitOnDemandComputations();
        }

        /// <summary>
        /// Indicates whether the matrix is full rank.
        /// </summary>
        /// <returns><c>true</c> if R, and hence A, has full rank.</returns>
        [Obsolete("FullRank has been renamed to IsFullRank. Please adapt.")]
        public bool FullRank
        {
            get
            {
                return IsFullRank;
            }
        }

        /// <summary>
        /// Indicates whether the matrix is full rank.
        /// </summary>
        /// <returns><c>true</c> if R, and hence A, has full rank.</returns>
        public bool IsFullRank
        {
            get
            {
                return _fullRankOnDemand.Compute();
            }
        }

        /// <summary>
        /// Gets the Householder vectors.
        /// </summary>
        /// <returns>Lower trapezoidal matrix whose columns define the reflections.</returns>
        public Matrix H
        {
            get
            {
                return _householderVectorsOnDemand.Compute();
            }
        }

        /// <summary>
        /// Gets the upper triangular factor
        /// </summary>
        public Matrix R
        {
            get
            {
                return _upperTriangularFactorOnDemand.Compute();
            }
        }

        /// <summary>
        /// Gets the (economy-sized) orthogonal factor.
        /// </summary>
        public Matrix Q
        {
            get
            {
                return _orthogonalFactorOnDemand.Compute();
            }
        }

        /// <summary>
        /// Least squares solution of A*X = B.
        /// </summary>
        /// <param name="B">A Matrix with as many rows as A and any number of columns.</param>
        /// <returns>X that minimizes the two norm of Q*R*X-B.</returns>
        /// <exception cref="System.ArgumentException">Matrix row dimensions must agree.</exception>
        /// <exception cref="System.SystemException"> Matrix is rank deficient.</exception>
        public
        Matrix
        Solve(
            Matrix B
            )
        {
            if(B.RowCount != M)
            {
                throw new ArgumentException(Properties.LocalStrings.ArgumentMatrixSameRowDimension, "B");
            }

            if(!IsFullRank)
            {
                throw new InvalidOperationException(Properties.LocalStrings.ArgumentMatrixNotRankDeficient);
            }

            // Copy right hand side
            int nx = B.ColumnCount;
            double[][] X = B.Clone();

            // Compute Y = transpose(Q)*B
            for(int k = 0; k < N; k++)
            {
                for(int j = 0; j < nx; j++)
                {
                    double s = 0.0;
                    for(int i = k; i < M; i++)
                    {
                        s += QR[i][k] * X[i][j];
                    }

                    s = (-s) / QR[k][k];
                    for(int i = k; i < M; i++)
                    {
                        X[i][j] += s * QR[i][k];
                    }
                }
            }

            // Solve R*X = Y;
            for(int k = N - 1; k >= 0; k--)
            {
                for(int j = 0; j < nx; j++)
                {
                    X[k][j] /= Rdiag[k];
                }

                for(int i = 0; i < k; i++)
                {
                    for(int j = 0; j < nx; j++)
                    {
                        X[i][j] -= X[k][j] * QR[i][k];
                    }
                }
            }

            return new Matrix(X).GetMatrix(0, N - 1, 0, nx - 1);
        }

        void
        InitOnDemandComputations()
        {
            _fullRankOnDemand = new OnDemandComputation<bool>(ComputeFullRank);
            _householderVectorsOnDemand = new OnDemandComputation<Matrix>(ComputeHouseholderVectors);
            _upperTriangularFactorOnDemand = new OnDemandComputation<Matrix>(ComputeUpperTriangularFactor);
            _orthogonalFactorOnDemand = new OnDemandComputation<Matrix>(ComputeOrthogonalFactor);
        }

        bool
        ComputeFullRank()
        {
            for(int j = 0; j < N; j++)
            {
                if(Rdiag[j] == 0.0)
                {
                    return false;
                }
            }

            return true;
        }

        Matrix
        ComputeHouseholderVectors()
        {
            double[][] H = Matrix.CreateMatrixData(M, N);
            for(int i = 0; i < M; i++)
            {
                for(int j = 0; j < N; j++)
                {
                    if(i >= j)
                    {
                        H[i][j] = QR[i][j];
                    }
                    else
                    {
                        H[i][j] = 0.0;
                    }
                }
            }

            return new Matrix(H);
        }

        Matrix
        ComputeUpperTriangularFactor()
        {
            double[][] R = Matrix.CreateMatrixData(Math.Min(N, M), N);
            for(int i = 0; i < Math.Min(N, M); i++)
            {
                double rowSign = Math.Sign(Rdiag[i]);
                double[] Ri = R[i];
                for(int j = 0; j < Ri.Length; j++)
                {
                    if(i < j)
                    {
                        Ri[j] = rowSign * QR[i][j];
                    }
                    else if(i == j)
                    {
                        Ri[j] = rowSign * Rdiag[i];
                    }
                    else
                    {
                        Ri[j] = 0.0;
                    }
                }
            }

            return new Matrix(R);
        }

        Matrix
        ComputeOrthogonalFactor()
        {
            double[][] Q = Matrix.CreateMatrixData(M, Math.Min(M, N));
            for(int k = Math.Min(M, N) - 1; k >= 0; k--)
            {
                Q[k][k] = 1.0;
                for(int j = k; j < Math.Min(M, N); j++)
                {
                    if(QR[k][k] == 0.0)
                    {
                        continue;
                    }

                    double s = 0.0;
                    for(int i = k; i < M; i++)
                    {
                        s += QR[i][k] * Q[i][j];
                    }

                    s = (-s) / QR[k][k];
                    for(int i = k; i < M; i++)
                    {
                        Q[i][j] += s * QR[i][k];
                    }
                }

                double columnSign = Math.Sign(Rdiag[k]);
                for(int i = 0; i < M; i++)
                {
                    Q[i][k] *= columnSign;
                }
            }

            return new Matrix(Q);
        }
    }
}
