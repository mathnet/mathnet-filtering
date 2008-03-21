using System;
using MathNet.Numerics.LinearAlgebra;

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
}
