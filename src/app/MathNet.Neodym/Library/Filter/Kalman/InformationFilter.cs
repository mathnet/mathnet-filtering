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

using System;
using MathNet.Numerics.LinearAlgebra;
using MathNet.SignalProcessing.Filter.Kalman;

namespace MathNet.SignalProcessing.Filter.Kalman
{
    /// <summary>
    /// An alternate form of the Discrete Kalman Filter.
    /// </summary>
    /// <remarks>The Information filter stores and works with the inverse of the
    /// covariance matrix. The information filter has a more computationally complex
    /// prediction, and a less complex update. This makes it suitable for situations
    /// where large numbers of measurements are used for state estimates, or when
    /// the state of the system does not need to be known too frequently.</remarks>
    public class InformationFilter :
        IKalmanFilter
    {

        #region Public members

        /// <summary>
        /// The covariance of the current state estimate.
        /// </summary>
        public Matrix Cov
        {
            get { return (this.J.Inverse()); }
        }

        /// <summary>
        /// The estimate of the state of the system.
        /// </summary>
        /// <remarks>Examination of system state requires an inversion of the covariance
        /// matrix for the information filter, and is quite expensive for large systems.</remarks>
        public Matrix State
        {
            get { return (this.J.Inverse() * this.y); }
        }

        /// <summary>
        /// The inverse of the covariance of the current state estimate.
        /// </summary>
        public Matrix InverseCov
        {
            get { return this.J; }
        }

        #endregion

        #region Contructors

        /// <summary>
        /// Creates an Information Filter from a given Kalman Filter.
        /// </summary>
        /// <param name="kf">The filter used to derive the information filter.</param>
        public
        InformationFilter(
            IKalmanFilter kf
            )
        {
            this.J = kf.Cov.Inverse();
            this.y = this.J * kf.State;
            this.I = Matrix.Identity(this.y.RowCount, this.y.RowCount);
        }

        /// <summary>
        /// Creates an Information filter with the given initial state.
        /// </summary>
        /// <param name="x0">Initial estimate of state variables.</param>
        /// <param name="P0">Covaraince of state variable estimates.</param>
        public
        InformationFilter(
            Matrix x0,
            Matrix P0
            )
        {
            KalmanFilter.CheckInitialParameters(x0, P0);

            this.J = P0.Inverse();
            this.y = this.J * x0;
            this.I = Matrix.Identity(this.y.RowCount, this.y.RowCount);
        }

        /// <summary>
        /// Creates an Information filter specifying whether the covariance and state
        /// have been 'inverted'.
        /// </summary>
        /// <param name="state">The initial estimate of the state of the system.</param>
        /// <param name="cov">The covariance of the initial state estimate.</param>
        /// <param name="inverted">Has covariance/state been converted to information
        /// filter form?</param>
        /// <remarks>This behaves the same as other contructors if the given boolean is false.
        /// Otherwise, in relation to the given state/covariance should satisfy:<BR></BR>
        /// <C>cov = J = P0 ^ -1, state = y = J * x0.</C></remarks>
        public
        InformationFilter(
            Matrix state,
            Matrix cov,
            bool inverted
            )
        {
            KalmanFilter.CheckInitialParameters(state, cov);

            if(inverted)
            {
                this.J = cov;
                this.y = state;
            }
            else
            {
                this.J = cov.Inverse();
                this.y = this.J * state;
            }
            this.I = Matrix.Identity(state.RowCount, state.RowCount);
        }

        #endregion

        #region Information Filter Prediction

        /// <summary>
        /// Perform a discrete time prediction of the system state.
        /// </summary>
        /// <param name="F">State transition matrix.</param>
        /// <exception cref="System.ArgumentException">Thrown when the given state
        /// transition matrix does not have the same number of row/columns as there
        /// are variables in the state vector.</exception>
        public
        void
        Predict(
            Matrix F
            )
        {
            KalmanFilter.CheckPredictParameters(F, this);

            // Easier just to convert back to discrete form....
            Matrix P = this.J.Inverse();
            Matrix x = P * this.y;

            x = F * x;
            P = F * P * Matrix.Transpose(F);

            this.J = P.Inverse();
            this.y = this.J * x;
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
        public
        void
        Predict(
            Matrix F,
            Matrix Q
            )
        {
            // We will need these matrices more than once...
            Matrix FI = F.Inverse();
            Matrix FIT = Matrix.Transpose(FI);
            Matrix A = FIT * J * FI;
            Matrix AQI = (A + Q.Inverse()).Inverse();

            // 'Covariance' Update
            this.J = A - (A * AQI * A);
            this.y = (this.I - (A * AQI)) * FIT * this.y;
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
        public
        void
        Predict(
            Matrix F,
            Matrix G,
            Matrix Q
            )
        {
            // Some matrices we will need a bit
            Matrix FI = F.Inverse();
            Matrix FIT = Matrix.Transpose(FI);
            Matrix GT = Matrix.Transpose(G);
            Matrix A = FIT * this.J * FI;
            Matrix B = A * G * (GT * A * G + Q.Inverse()).Inverse();

            this.J = (I - B * GT) * A;
            this.y = (I - B * GT) * FIT * this.y;
        }

        #endregion

        #region Information Filter Update

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
        public
        void
        Update(
            Matrix z,
            Matrix H,
            Matrix R
            )
        {
            KalmanFilter.CheckUpdateParameters(z, H, R, this);

            // Fiddle with the matrices
            Matrix HT = Matrix.Transpose(H);
            Matrix RI = R.Inverse();

            // Perform the update
            this.y = this.y + (HT * RI * z);
            this.J = this.J + (HT * RI * H);
        }

        #endregion

        #region Protected members

        /// <summary>
        /// Inverse of covariance matrix.
        /// </summary>
        protected Matrix J;

        /// <summary>
        /// State of information filter.
        /// </summary>
        protected Matrix y;

        /// <summary>
        /// Identity matrix used in operations.
        /// </summary>
        protected Matrix I;

        #endregion
    }
}
