#region Math.NET Neodym (LGPL) by Christoph Ruegg
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

namespace MathNet.SignalProcessing.Filter.Kalman
{
    /// <summary>
    /// <para>The <c>DiscreteTimeKalmanFilter</c> is generally used in digital computer
    /// implementations of the Kalman Filter. As the name suggests, it is used
    /// when the state of the system and updates are available at discrete points
    /// in time.</para>
    /// <para>This is the most general form of the discrete time Kalman Filter.
    /// Other, more specialised forms are available if discrete measurements are
    /// available at fixed time intervals.</para>
    /// </summary>
    /// <remarks>This implementation uses the most common form of the discrete time
    /// Kalman Filter:
    /// <code>
    /// Prediction: x(k|k-1) = F(k-1) * x(k-1|k-1)
    ///             P(k|k-1) = F(k-1)*P(k-1|k-1)*F(k-1) + G(k-1)*Q(k-1)*G'(k-1)
    /// Update:     S(k) = H(k)*P(k|k-1)*H'(k) + R(k)
    ///             K(k) = P(k|k-1)*H'(k)*S^(-1)(k)
    ///             P(k|k) = (I-K(k)*H(k))*P(k|k-1)
    ///             x(k|k) = x(k|k-1)+K(k)*(z(k)-H(k)*x(k|k-1))
    /// </code></remarks>
    public class DiscreteKalmanFilter : IKalmanFilter
    {
        #region Public Fields
        /// <summary>
        /// The covariance of the current state of the filter. Higher covariances
        /// indicate a lower confidence in the state estimate.
        /// </summary>
        public Matrix Cov
        {
            get { return this.P; }
        }
        /// <summary>
        /// The best estimate of the current state of the system.
        /// </summary>
        public Matrix State
        {
            get { return this.x; }
        }

        #endregion // Public Fields

        #region Protected Members
        /// <summary>
        /// The current state of the system.
        /// </summary>
        protected Matrix x;
        /// <summary>
        /// The current covariance of the estimated state of the system.
        /// </summary>
        protected Matrix P;

        #endregion

        #region Constructors
        /// <summary>
        /// Creates a new Discrete Time Kalman Filter with the given values for
        /// the initial state and the covariance of that state.
        /// </summary>
        /// <param name="x0">The best estimate of the initial state of the estimate.</param>
        /// <param name="P0">The covariance of the initial state estimate. If unsure
        /// about initial state, set to a large value</param>
        public
            DiscreteKalmanFilter(
                Matrix x0,
                Matrix P0)
        {
            KalmanFilter.CheckInitialParameters(x0, P0);

            this.x = x0;
            this.P = P0;
        }

        #endregion

        #region Kalman Filter Prediction

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
            this.x = F * this.x;
            this.P = (F * P * Matrix.Transpose(F));
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
            KalmanFilter.CheckPredictParameters(F, Q, this);
            // Predict the state
            this.x = F * x;
            this.P = (F * P * Matrix.Transpose(F)) + Q;
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
            KalmanFilter.CheckPredictParameters(F, G, Q, this);
            this.x = F * x;      // State prediction
            // Covariance update
            this.P = (F * P * Matrix.Transpose(F)) + (G * Q * Matrix.Transpose(G));
        }

        #endregion

        #region Kalman Filter Update

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
            // We need to use transpose of H a couple of times.
            Matrix Ht = Matrix.Transpose(H);
            Matrix I = Matrix.Identity(x.RowCount, x.RowCount);

            Matrix S = (H * P * Ht) + R;          // Measurement covariance
            Matrix K = P * Ht * S.Inverse();      // Kalman Gain
            this.P = (I - (K * H)) * P;           // Covariance update
            this.x = this.x + (K * (z - (H * this.x))); // State update
        }

        #endregion

    }
}
