#region Math.NET Neodym (LGPL) by Christoph Ruegg
// Math.NET Neodym, part of the Math.NET Project
// https://www.mathdotnet.com
//
// Copyright (c) 2008, Matthew Kitchin
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

using MathNet.Numerics.LinearAlgebra;

namespace MathNet.Filtering.Kalman
{
    /// <summary>
    /// An alternate form of the discrete time kalman filter, less prone to roundoff
    /// errors.
    /// </summary>
    /// <remarks>Square root filtering is designed to produce more stable covariance
    /// matrices by performing decomposition of the covariance matrix to ensure that
    /// roundoff errors do not occur.
    /// <para>This particular implementation stores the covariance in a UDU' decomposed
    /// form, and uses a Thornton UD update and a Bierman observational update algorithm.
    /// This means that there are no square roots performed as part of this.</para>
    /// </remarks>
    public class SquareRootFilter : IKalmanFilter
    {
        /// <summary>
        /// The estimate of the current state of the system.
        /// </summary>
        public Matrix<double> State
        {
            get { return x; }
        }

        /// <summary>
        /// The covariance of the state estimate.
        /// </summary>
        public Matrix<double> Cov
        {
            get { return U*D*U.Transpose(); }
        }

        /// <summary>
        /// Creates a square root filter with given initial state.
        /// </summary>
        /// <param name="x0">Initial state estimate.</param>
        /// <param name="P0">Covariance of initial state estimate.</param>
        public SquareRootFilter(Matrix<double> x0, Matrix<double> P0)
        {
            KalmanFilter.CheckInitialParameters(x0, P0);

            // Decompose the covariance matrix
            Matrix<double>[] UDU = UDUDecomposition(P0);
            U = UDU[0];
            D = UDU[1];
            x = x0;
        }

        /// <summary>
        /// Performs a prediction of the state of the system after a given transition.
        /// </summary>
        /// <param name="F">State transition matrix.</param>
        /// <exception cref="System.ArgumentException">Thrown when the given state
        /// transition matrix does not have the same number of row/columns as there
        /// are variables in the state vector.</exception>
        public void Predict(Matrix<double> F)
        {
            KalmanFilter.CheckPredictParameters(F, this);

            Matrix<double> tmpG = Matrix<double>.Build.Dense(x.RowCount, 1);
            Matrix<double> tmpQ = Matrix<double>.Build.Dense(1, 1, 1.0d);
            Predict(F, tmpG, tmpQ);
        }

        /// <summary>
        /// Performs a prediction of the state of the system after a given transition.
        /// </summary>
        /// <param name="F">State transition matrix.</param>
        /// <param name="G">Noise coupling matrix.</param>
        /// <param name="Q">Noise covariance matrix.</param>
        /// <exception cref="System.ArgumentException">Thrown when the given matrices
        /// are not the correct dimensions.</exception>
        public void Predict(Matrix<double> F, Matrix<double> G, Matrix<double> Q)
        {
            KalmanFilter.CheckPredictParameters(F, G, Q, this);

            // Update the state.. that is easy!!
            x = F*x;

            // Get all the sized and create storage
            int n = x.RowCount;
            int p = G.ColumnCount;
            Matrix<double> outD = Matrix<double>.Build.Dense(n, n); // Updated diagonal matrix
            Matrix<double> outU = Matrix<double>.Build.DenseIdentity(n, n); // Updated upper unit triangular

            // Get the UD Decomposition of the process noise matrix
            Matrix<double>[] UDU = UDUDecomposition(Q);
            Matrix<double> Uq = UDU[0];
            Matrix<double> Dq = UDU[1];

            // Combine it with the noise coupling matrix
            Matrix<double> Gh = G*Uq;

            // Convert state transition matrix
            Matrix<double> PhiU = F*U;

            // Ready to go..
            for (int i = n - 1; i >= 0; i--)
            {
                // Update the i'th diagonal of the covariance matrix
                double sigma = 0.0d;

                for (int j = 0; j < n; j++)
                {
                    sigma = sigma + (PhiU[i, j]*PhiU[i, j]*D[j, j]);
                }

                for (int j = 0; j < p; j++)
                {
                    sigma = sigma + (Gh[i, j]*Gh[i, j]*Dq[j, j]);
                }

                outD[i, i] = sigma;

                // Update the i'th row of the upper triangular of covariance
                for (int j = 0; j < i; j++)
                {
                    sigma = 0.0d;

                    for (int k = 0; k < n; k++)
                    {
                        sigma = sigma + (PhiU[i, k]*D[k, k]*PhiU[j, k]);
                    }

                    for (int k = 0; k < p; k++)
                    {
                        sigma = sigma + (Gh[i, k]*Dq[k, k]*Gh[j, k]);
                    }

                    outU[j, i] = sigma/outD[i, i];

                    for (int k = 0; k < n; k++)
                    {
                        PhiU[j, k] = PhiU[j, k] - (outU[j, i]*PhiU[i, k]);
                    }

                    for (int k = 0; k < p; k++)
                    {
                        Gh[j, k] = Gh[j, k] - (outU[j, i]*Gh[i, k]);
                    }
                }
            }

            // Update the covariance
            U = outU;
            D = outD;
        }

        /// <summary>
        /// Updates the state of the system based on the given noisy measurements,
        /// a description of how those measurements relate to the system, and a
        /// covariance <c>Matrix</c> to describe the noise of the system.
        /// </summary>
        /// <param name="z">The measurements of the system.</param>
        /// <param name="H">Measurement model.</param>
        /// <param name="R">Covariance of measurements.</param>
        /// <exception cref="System.ArgumentException">Thrown when given matrices
        /// are of the incorrect size.</exception>
        public void Update(Matrix<double> z, Matrix<double> H, Matrix<double> R)
        {
            // Diagonalize the given covariance matrix R
            Matrix<double>[] UDU = UDUDecomposition(R);
            Matrix<double> RU = UDU[0];
            Matrix<double> RD = UDU[1];
            Matrix<double> iRU = RU.Inverse();
            Matrix<double> zh = iRU*z;
            Matrix<double> Hh = iRU*H;

            // Perform a scalar update for each measurement
            for (int i = 0; i < z.RowCount; i++)
            {
                // Get sub-matrix of H
                Matrix<double> subH = Hh.SubMatrix(i, 1, 0, Hh.ColumnCount);
                Update(zh[i, 0], subH, RD[i, i]);
            }
        }

        void Update(double z, Matrix<double> H, double R)
        {
            Matrix<double> a = U.Transpose()*H.Transpose();
            Matrix<double> b = D*a;
            double dz = z - (H*x)[0, 0];
            double alpha = R;
            double gamma = 1d/alpha;

            for (int j = 0; j < x.RowCount; j++)
            {
                double beta = alpha;
                alpha = alpha + (a[j, 0]*b[j, 0]);
                double lambda = -a[j, 0]*gamma;
                gamma = 1d/alpha;
                D[j, j] = beta*gamma*D[j, j];

                for (int i = 0; i < j; i++)
                {
                    beta = U[i, j];
                    U[i, j] = beta + (b[i, 0]*lambda);
                    b[i, 0] = b[i, 0] + (b[j, 0]*beta);
                }
            }

            double dzs = gamma*dz;
            x = x + (dzs*b);
        }

        static Matrix<double>[] UDUDecomposition(Matrix<double> Arg)
        {
            // Initialize some values
            int n = Arg.RowCount; // Number of elements in matrix
            double sigma; // Temporary value
            double[,] aU = new double[n, n];
            double[,] aD = new double[n, n];

            for (int j = n - 1; j >= 0; j--)
            {
                for (int i = j; i >= 0; i--)
                {
                    sigma = Arg[i, j];

                    for (int k = j + 1; k < n; k++)
                    {
                        sigma = sigma - (aU[i, k]*aD[k, k]*aU[j, k]);
                    }

                    if (i == j)
                    {
                        aD[j, j] = sigma;
                        aU[j, j] = 1d;
                    }
                    else
                    {
                        aU[i, j] = sigma/aD[j, j];
                    }
                }
            }

            // Create the output... first Matrix is L, next is D
            var outMats = new Matrix<double>[2];
            outMats[0] = Matrix<double>.Build.DenseOfArray(aU);
            outMats[1] = Matrix<double>.Build.DenseOfArray(aD);
            return outMats;
        }

        /// <summary>
        /// Upper unit triangular matrix of decomposed covariance.
        /// </summary>
        protected Matrix<double> U;

        /// <summary>
        /// Diagonal matrix of decomposed covariance.
        /// </summary>
        protected Matrix<double> D;

        /// <summary>
        /// State estimate of system.
        /// </summary>
        protected Matrix<double> x;
    }
}
