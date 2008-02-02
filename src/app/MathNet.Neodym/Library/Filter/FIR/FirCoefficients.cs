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

namespace MathNet.SignalProcessing.Filter.FIR
{
	/// <summary>
	/// FirCoefficients provides basic coefficient evaluation
	/// algorithms for the four most important filter types for
	/// Finite Impulse Response (FIR) Filters.
	/// </summary>
	public static class FirCoefficients
	{
		/// <summary>
		/// Calculates FIR LowPass Filter Coefficients.
		/// </summary>
		/// <param name="samplingRate">Samples per unit.</param>
		/// <param name="cutoff">Cutoff frequency in samples per unit.</param>
		/// <param name="halforder">halforder Q, so that Order N = 2*Q+1. Usually between 20 and 150.</param>
		/// <returns>The calculated filter coefficients.</returns>
		public static double[] LowPass(double samplingRate, double cutoff, int halforder)
		{
			double nu = 2d*cutoff/samplingRate; //normalized frequency
			int order = 2*halforder+1;
			double[] c = new double[order];
			c[halforder] = nu;
			for(int i=0,n=halforder;i<halforder;i++,n--)
			{
				double npi = n*Math.PI;
				c[i] = Math.Sin(npi*nu)/npi;
				c[n+halforder] = c[i];
			}
			return c;
		}

		/// <summary>
		/// Calculates FIR HighPass Filter Coefficients.
		/// </summary>
		/// <param name="samplingRate">Samples per unit.</param>
		/// <param name="cutoff">Cutoff frequency in samples per unit.</param>
		/// <param name="halforder">halforder Q, so that Order N = 2*Q+1</param>
		/// <returns>The calculated filter coefficients.</returns>
		public static double[] HighPass(double samplingRate, double cutoff, int halforder)
		{
			double nu = 2d*cutoff/samplingRate; //normalized frequency
			int order = 2*halforder+1;
			double[] c = new double[order];
			c[halforder] = 1-nu;
			for(int i=0,n=halforder;i<halforder;i++,n--)
			{
				double npi = n*Math.PI;
				c[i] = -Math.Sin(npi*nu)/npi;
				c[n+halforder] = c[i];
			}
			return c;
		}

		/// <summary>
		/// Calculates FIR Bandpass Filter Coefficients.
		/// </summary>
		/// <param name="samplingRate">Samples per unit.</param>
		/// <param name="cutoffLow">Low Cutoff frequency in samples per unit.</param>
		/// <param name="cutoffHigh">High Cutoff frequency in samples per unit.</param>
		/// <param name="halforder">halforder Q, so that Order N = 2*Q+1</param>
		/// <returns>The calculated filter coefficients.</returns>
		public static double[] BandPass(double samplingRate, double cutoffLow, double cutoffHigh, int halforder)
		{
			double nu1 = 2d*cutoffLow/samplingRate; //normalized low frequency
			double nu2 = 2d*cutoffHigh/samplingRate; //normalized high frequency
			int order = 2*halforder+1;
			double[] c = new double[order];
			c[halforder] = nu2-nu1;
			for(int i=0,n=halforder;i<halforder;i++,n--)
			{
				double npi = n*Math.PI;
				c[i] = (Math.Sin(npi*nu2)-Math.Sin(npi*nu1))/npi;
				c[n+halforder] = c[i];
			}
			return c;
		}

		/// <summary>
		/// Calculates FIR Bandstop Filter Coefficients.
		/// </summary>
		/// <param name="samplingRate">Samples per unit.</param>
		/// <param name="cutoffLow">Low Cutoff frequency in samples per unit.</param>
		/// <param name="cutoffHigh">High Cutoff frequency in samples per unit.</param>
		/// <param name="halforder">halforder Q, so that Order N = 2*Q+1</param>
		/// <returns>The calculated filter coefficients.</returns>
		public static double[] BandStop(double samplingRate, double cutoffLow, double cutoffHigh, int halforder)
		{
			double nu1 = 2d*cutoffLow/samplingRate; //normalized low frequency
			double nu2 = 2d*cutoffHigh/samplingRate; //normalized high frequency
			int order = 2*halforder+1;
			double[] c = new double[order];
			c[halforder] = 1-(nu2-nu1);
			for(int i=0,n=halforder;i<halforder;i++,n--)
			{
				double npi = n*Math.PI;
				c[i] = (Math.Sin(npi*nu1)-Math.Sin(npi*nu2))/npi;
				c[n+halforder] = c[i];
			}
			return c;
		}
	}
}
