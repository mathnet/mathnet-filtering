#region Math.NET Neodym (LGPL) by Matthew Kitchin
// Math.NET Neodym, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
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
    /// An alternate form of the Discrete Kalman Filter.
    /// </summary>
    /// <remarks>The Information filter stores and works with the inverse of the
    /// covariance matrix. The information filter has a more computationally complex
    /// prediction, and a less complex update. This makes it suitable for situations
    /// where large numbers of measurements are used for state estimates, or when
    /// the state of the system does not need to be known too frequently.</remarks>
    public class InformationFilter : IKalmanFilter
    {
        /// <summary>
        /// The covariance of the current state estimate.
        /// </summary>
        public Matrix<double> Cov
        {
            get { return J.Inverse(); }
        }

        /// <summary>
        /// The estimate of the state of the system.
        /// </summary>
        /// <remarks>Examination of system state requires an inversion of the covariance
        /// matrix for the information filter, and is quite expensive for large systems.</remarks>
        public Matrix<double> State
        {
            get { return J.Inverse()*y; }
        }

        /// <summary>
        /// The inverse of the covariance of the current state estimate.
        /// </summary>
        public Matrix<double> InverseCov
        {
            get { return J; }
        }

        /// <summary>
        /// Creates an Information Filter from a given Kalman Filter.
        /// </summary>
        /// <param name="kf">The filter used to derive the information filter.</param>
        public InformationFilter(IKalmanFilter kf)
        {
            J = kf.Cov.Inverse();
            y = J*kf.State;
            I = Matrix<double>.Build.DenseIdentity(y.RowCount, y.RowCount);
        }

        /// <summary>
        /// Creates an Information filter with the given initial state.
        /// </summary>
        /// <param name="x0">Initial estimate of state variables.</param>
        /// <param name="P0">Covariance of state variable estimates.</param>
        public InformationFilter(Matrix<double> x0, Matrix<double> P0)
        {
            KalmanFilter.CheckInitialParameters(x0, P0);

            J = P0.Inverse();
            y = J*x0;
            I = Matrix<double>.Build.DenseIdentity(y.RowCount, y.RowCount);
        }

        /// <summary>
        /// Creates an Information filter specifying whether the covariance and state
        /// have been 'inverted'.
        /// </summary>
        /// <param name="state">The initial estimate of the state of the system.</param>
        /// <param name="cov">The covariance of the initial state estimate.</param>
        /// <param name="inverted">Has covariance/state been converted to information
        /// filter form?</param>
        /// <remarks>This behaves the same as other constructors if the given boolean is false.
        /// Otherwise, in relation to the given state/covariance should satisfy:<BR></BR>
        /// <C>cov = J = P0 ^ -1, state = y = J * x0.</C></remarks>
        public InformationFilter(Matrix<double> state, Matrix<double> cov, bool inverted)
        {
            KalmanFilter.CheckInitialParameters(state, cov);

            if (inverted)
            {
                J = cov;
                y = state;
            }
            else
            {
                J = cov.Inverse();
                y = J*state;
            }

            I = Matrix<double>.Build.DenseIdentity(state.RowCount, state.RowCount);
        }

        /// <summary>
        /// Perform a discrete time prediction of the system state.
        /// </summary>
        /// <param name="F">State transition matrix.</param>
        /// <exception cref="System.ArgumentException">Thrown when the given state
        /// transition matrix does not have the same number of row/columns as there
        /// are variables in the state vector.</exception>
        public void Predict(Matrix<double> F)
        {
            KalmanFilter.CheckPredictParameters(F, this);

            // Easier just to convert back to discrete form....
            Matrix<double> p = J.Inverse();
            Matrix<double> x = p*y;

            x = F*x;
            p = F*p*F.Transpose();

            J = p.Inverse();
            y = J*x;
        }

        /// <summary>
        /// Preform a discrete time prediction of the system state.
        /// </summary>
        /// <param name="F">State transition matrix.</param>
        /// <param name="Q">A plant noise covariance matrix.</param>
        /// <exception cref="System.ArgumentException">Thrown when F and Q are not
        /// square matrices with the same number of rows and columns as there are
        /// rows in the state matrix.</exception>
        /// <remarks>Performs a prediction of the next state of the Kalman Filter,
        /// where there is plant noise. The covariance matrix of the plant noise, in
        /// this case, is a square matrix corresponding to the state transition and
        /// the state of the system.</remarks>
        public void Predict(Matrix<double> F, Matrix<double> Q)
        {
            // We will need these matrices more than once...
            Matrix<double> FI = F.Inverse();
            Matrix<double> FIT = FI.Transpose();
            Matrix<double> A = FIT*J*FI;
            Matrix<double> AQI = (A + Q.Inverse()).Inverse();

            // 'Covariance' Update
            J = A - (A*AQI*A);
            y = (I - (A*AQI))*FIT*y;
        }

        /// <summary>
        /// Perform a discrete time prediction of the system state.
        /// </summary>
        /// <param name="F">State transition matrix.</param>
        /// <param name="G">Noise coupling matrix.</param>
        /// <param name="Q">Plant noise covariance.</param>
        /// <exception cref="System.ArgumentException">Thrown when the column and row
        /// counts for the given matrices are incorrect.</exception>
        /// <remarks>
        /// Performs a prediction of the next state of the Kalman Filter, given
        /// a description of the dynamic equations of the system, the covariance of
        /// the plant noise affecting the system and the equations that describe
        /// the effect on the system of that plant noise.
        /// </remarks>
        public void Predict(Matrix<double> F, Matrix<double> G, Matrix<double> Q)
        {
            // Some matrices we will need a bit
            Matrix<double> FI = F.Inverse();
            Matrix<double> FIT = FI.Transpose();
            Matrix<double> GT = G.Transpose();
            Matrix<double> A = FIT*J*FI;
            Matrix<double> B = A*G*(GT*A*G + Q.Inverse()).Inverse();

            J = (I - B*GT)*A;
            y = (I - B*GT)*FIT*y;
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
            KalmanFilter.CheckUpdateParameters(z, H, R, this);

            // Fiddle with the matrices
            Matrix<double> HT = H.Transpose();
            Matrix<double> RI = R.Inverse();

            // Perform the update
            y = y + (HT*RI*z);
            J = J + (HT*RI*H);
        }

        /// <summary>
        /// Inverse of covariance matrix.
        /// </summary>
        protected Matrix<double> J;

        /// <summary>
        /// State of information filter.
        /// </summary>
        protected Matrix<double> y;

        /// <summary>
        /// Identity matrix used in operations.
        /// </summary>
        protected Matrix<double> I;
    }
}
