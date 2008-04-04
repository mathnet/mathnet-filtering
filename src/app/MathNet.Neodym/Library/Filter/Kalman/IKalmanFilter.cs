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
using MathNet.SignalProcessing.Properties;

namespace MathNet.SignalProcessing.Filter.Kalman
{
	/// <summary>
	/// <para>An interface to describe a Kalman Filter. A Kalman filter is a
	/// recursive solution to the general dynamic estimation problem for the
	/// important special case of linear system models and gaussian noise.
	/// </para>
	/// <para>The Kalman Filter uses a predictor-corrector structure, in which
	/// if a measurement of the system is available at time <italic>t</italic>,
	/// we first call the Predict function, to estimate the state of the system
	/// at time <italic>t</italic>. We then call the Update function to
	/// correct the estimate of state, based on the noisy measurement.</para>
	/// </summary>
	public interface IKalmanFilter
	{
		/// <summary>
		/// The covariance of the current state estimate.
		/// </summary>
		Matrix Cov
		{
			get;
		}
		/// <summary>
		/// The current best estimate of the state of the system.
		/// </summary>
		Matrix State
		{
			get;
		}

		/// <summary>
		/// Performs a prediction of the next state of the system.
		/// </summary>
		/// <param name="F">The state transition matrix.</param>
		void Predict(Matrix F);
		/// <summary>
		/// Perform a prediction of the next state of the system.
		/// </summary>
		/// <param name="F">The state transition matrix.</param>
		/// <param name="G">The linear equations to describe the effect of the noise
		/// on the system.</param>
		/// <param name="Q">The covariance of the noise acting on the system.</param>
		
		void Predict(Matrix F, Matrix G, Matrix Q);
		/// <summary>
		/// Updates the state estimate and covariance of the system based on the
		/// given measurement.
		/// </summary>
		/// <param name="z">The measurements of the system.</param>
		/// <param name="H">Linear equations to describe relationship between
		/// measurements and state variables.</param>
		/// <param name="R">The covariance matrix of the measurements.</param>
		void Update(Matrix z, Matrix H, Matrix R);
	}
	
	/// <summary>
	/// Abstract class that contains static methods to assist in the development
	/// of Kalman Filters.
	/// </summary>
	internal abstract class KalmanFilter
	{
		/// <summary>
		/// Checks that a state vector and covariance matrix are of the correct
		/// dimensions.
		/// </summary>
		/// <param name="x0">State vector.</param>
		/// <param name="P0">Covariance matrix.</param>
		/// <exception cref="System.ArgumentException">Thrown when the x0 matrix is not
		/// a column vector, or when the P0 matrix is not a square matrix of the same order
		/// as the number of state variables.</exception>
		public 
			static 
			void 
			CheckInitialParameters(
				Matrix x0, 
				Matrix P0
			)
		{
			// x0 should be a column vector
			if (x0.ColumnCount != 1)
				throw new ArgumentException(Resources.KFStateNotColumnVector, "x0");
			// P0 should be square and of same order as x0
			if (P0.ColumnCount != P0.RowCount)
				throw new ArgumentException(Resources.KFCovarianceNotSquare, "P0");
			if (P0.ColumnCount != x0.RowCount)
				throw new ArgumentException(Resources.KFCovarianceIncorrectSize, "P0");
		}
		
		/// <summary>
		/// Checks that the given matrices for prediction are the correct dimensions.
		/// </summary>
		/// <param name="F">State transition matrix.</param>
		/// <param name="G">Noise coupling matrix.</param>
		/// <param name="Q">Noise process covariance.</param>
		/// <param name="filter">Filter being predicted.</param>
		/// <exception cref="System.ArgumentException">Thrown when:
		/// <list type="bullet"><item>F is non-square with same rows/cols as state
		/// the number of state variables.</item>
		/// <item>G does not have same number of columns as number of state variables
		/// and rows as Q.</item>
		/// <item>Q is non-square.</item>
		/// </list></exception>
		public 
			static 
			void 
			CheckPredictParameters(
				Matrix F, 
				Matrix G, 
				Matrix Q, 
				IKalmanFilter filter
			)
		{
			// State transition should be n-by-n matrix (n is number of state variables)
			if ((F.ColumnCount != F.RowCount) || (F.ColumnCount != filter.State.RowCount))
				throw new ArgumentException(Resources.KFStateTransitionMalformed, "F");
			// Noise coupling should be n-by-p (p is rows in Q)
			if ((G.RowCount != filter.State.RowCount) || (G.ColumnCount != Q.RowCount))
				throw new ArgumentException(Resources.KFNoiseCouplingMalformed, "G");
			// Noise covariance should be p-by-p
			if (Q.ColumnCount != Q.RowCount)
				throw new ArgumentException(Resources.KFNoiseCovarianceMalformed, "Q");
		}
		
		/// <summary>
		/// Checks that the given prediction matrices are the correct dimension.
		/// </summary>
		/// <param name="F">State transition matrix.</param>
		/// <param name="Q">Noise covariance matrix.</param>
		/// <param name="filter">Filter being predicted.</param>
		/// <exception cref="System.ArgumentException">Thrown when either transition
		/// or process noise matrices are non-square and/or have a number of rows/cols not
		/// equal to the number of state variables for the filter.</exception>
		public 
			static 
			void 
			CheckPredictParameters(
				Matrix F, 
				Matrix Q, 
				IKalmanFilter filter
			)
		{
			// State transition should be n-by-n matrix (n is number of state variables)
			if ((F.ColumnCount != F.RowCount) || (F.ColumnCount != filter.State.RowCount))
				throw new ArgumentException(Resources.KFStateTransitionMalformed, "F");
			if ((Q.ColumnCount != Q.RowCount) || (Q.ColumnCount != filter.State.RowCount))
				throw new ArgumentException(Resources.KFSquareNoiseCouplingMalformed, "Q");
		}
		
		/// <summary>
		/// Checks the state transition matrix is the correct dimension.
		/// </summary>
		/// <param name="F">State transition matrix.</param>
		/// <param name="filter">Filter being predicted.</param>
		/// <exception cref="System.ArgumentException">Thrown when the transition
		/// matrix is non-square or does not have the same number of rows/cols as there
		/// are state variables in the given filter.</exception>
		public 
			static 
			void 
			CheckPredictParameters(
				Matrix F, 
				IKalmanFilter filter
			)
		{
			// State transition should be n-by-n matrix (n is number of state variables)
			if ((F.ColumnCount != F.RowCount) || (F.ColumnCount != filter.State.RowCount))
				throw new ArgumentException(Resources.KFStateTransitionMalformed, "F");			
		}
		
		/// <summary>
		/// Checks the given update parameters are of the correct dimension.
		/// </summary>
		/// <param name="z">Measurement matrix.</param>
		/// <param name="H">Measurement sensitivity matrix.</param>
		/// <param name="R">Measurement covariance matrix.</param>
		/// <exception cref="System.ArgumentException">Thrown when:
		/// <list type="bullet"><item>z is not a column vector.</item>
		/// <item>H does not have same number of rows as z and columns as R.</item>
		/// <item>R is non square.</item></list></exception>
		public
			static 
			void 
			CheckUpdateParameters(
				Matrix z, 
				Matrix H, 
				Matrix R, 
				IKalmanFilter filter
			)
		{
			if (z.ColumnCount != 1)
				throw new ArgumentException(Resources.KFMeasurementVectorMalformed,"z");
			if ((H.RowCount != z.RowCount) || (H.ColumnCount != filter.State.RowCount))
				throw new ArgumentException(Resources.KFMeasureSensitivityMalformed,"H");
			if ((R.ColumnCount != R.RowCount) || (R.ColumnCount != z.RowCount))
				throw new ArgumentException(Resources.KFMeasureCovarainceMalformed,"R");
		}
	}
}


