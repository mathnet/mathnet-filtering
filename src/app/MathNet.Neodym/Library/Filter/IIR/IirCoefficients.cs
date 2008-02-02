#region Math.NET Neodym (LGPL) by Christoph Ruegg
// Math.NET Neodym, part of the Math.NET Project
// http://mathnet.opensourcedotnet.info
//
// Copyright (c) 2001-2008, Christoph Rüegg,  http://christoph.ruegg.name
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

namespace MathNet.SignalProcessing.Filter.IIR
{
	/// <summary>
	/// IirCoefficients provides basic coefficient evaluation
	/// algorithms for the four most important filter types for
	/// Infinite Impulse Response (IIR) Filters.
	/// </summary>
	public static class IirCoefficients
	{
		private static double[] BuildCoefficients(double beta, double gamma, double alpha, double mu, double sigma)
		{
			return new double[] {2d*alpha,2d*gamma,-2d*beta,1,mu,sigma};
		}

		private static void BetaGamma(out double beta, out double gamma, out double theta, double sampling, double cutoff, double lowHalfPower, double highHalfPower)
		{
			double tan = Math.Tan(Math.PI*(highHalfPower-lowHalfPower)/sampling);
			beta = 0.5d*(1-tan)/(1+tan);
			theta = 2*Math.PI*cutoff/sampling;
			gamma = (0.5d+beta)*Math.Cos(theta);
		}

		/// <summary>
		/// Calculates IIR LowPass Filter Coefficients.
		/// </summary>
		/// <param name="samplingRate">Samples per unit.</param>
		/// <param name="cutoff">Cutoff frequency in samples per unit.</param>
		/// <param name="width">bandwidth in samples per unit.</param>
		/// <returns>The calculated filter coefficients.</returns>
		public static double[] LowPass(double samplingRate, double cutoff, double width)
		{
			double beta, gamma, theta;
			BetaGamma(out beta,out gamma,out theta,samplingRate,cutoff,0d,width);
			return BuildCoefficients(beta,gamma,(0.5d+beta-gamma)*0.25d,2,1);
		}

		/// <summary>
		/// Calculates IIR HighPass Filter Coefficients.
		/// </summary>
		/// <param name="samplingRate">Samples per unit.</param>
		/// <param name="cutoff">Cutoff frequency in samples per unit.</param>
		/// <param name="width">bandwidth in samples per unit.</param>
		/// <returns>The calculated filter coefficients.</returns>
        public static double[] HighPass(double samplingRate, double cutoff, double width)
		{
			double beta, gamma, theta;
			BetaGamma(out beta,out gamma,out theta,samplingRate,cutoff,0d,width);
			return BuildCoefficients(beta,gamma,(0.5d+beta+gamma)*0.25d,-2,1);
		}

		/// <summary>
		/// Calculates IIR Bandpass Filter Coefficients.
		/// </summary>
		/// <param name="samplingRate">Samples per unit.</param>
		/// <param name="cutoffLow">Low Cutoff frequency in samples per unit.</param>
		/// <param name="cutoffHigh">High Cutoff frequency in samples per unit.</param>
		/// <returns>The calculated filter coefficients.</returns>
        public static double[] BandPass(double samplingRate, double cutoffLow, double cutoffHigh)
		{
			double beta, gamma, theta;
			BetaGamma(out beta,out gamma,out theta,samplingRate,(cutoffLow+cutoffHigh)*0.5d,cutoffLow,cutoffHigh);
			return BuildCoefficients(beta,gamma,(0.5d-beta)*0.5d,0,-1);
		}

		/// <summary>
		/// Calculates IIR Bandstop Filter Coefficients.
		/// </summary>
		/// <param name="samplingRate">Samples per unit.</param>
		/// <param name="cutoffLow">Low Cutoff frequency in samples per unit.</param>
		/// <param name="cutoffHigh">High Cutoff frequency in samples per unit.</param>
		/// <returns>The calculated filter coefficients.</returns>
        public static double[] BandStop(double samplingRate, double cutoffLow, double cutoffHigh)
		{
			double beta, gamma, theta;
			BetaGamma(out beta,out gamma,out theta,samplingRate,(cutoffLow+cutoffHigh)*0.5d,cutoffLow,cutoffHigh);
			return BuildCoefficients(beta,gamma,(0.5d+beta)*0.5d,-2*Math.Cos(theta),1);
		}
	}
}
