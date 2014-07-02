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
using MathNet.Filtering.Filter.FIR;
using MathNet.Filtering.Filter.IIR;
using MathNet.Filtering.Filter.Median;

namespace MathNet.Filtering.Filter
{
    /// <summary>
    /// Online filters allow processing incomming samples immediately and hence
    /// provide a nearly-realtime response with a fixed delay.
    /// </summary>
    public abstract class OnlineFilter : IOnlineFilter
    {
        #region LOWPASS FILTER FACTORY
        /// <summary>
        /// Create a filter to remove high frequencies in online processing scenarios.
        /// </summary>
        public static
        OnlineFilter
        CreateLowpass(
            ImpulseResponse mode,
            double sampleRate,
            double cutoffRate,
            int order
            )
        {
            if(mode == ImpulseResponse.Finite)
            {
                double[] c = FirCoefficients.LowPass(sampleRate, cutoffRate, order >> 1);
                return new OnlineFirFilter(c);
            }

            if(mode == ImpulseResponse.Infinite)
            {
                // TODO: investigate (bandwidth)
                double[] c = IirCoefficients.LowPass(sampleRate, cutoffRate, cutoffRate);
                return new OnlineIirFilter(c);
            }

            throw new ArgumentException("mode");
        }

        /// <summary>
        /// Create a filter to remove high frequencies in online processing scenarios.
        /// </summary>
        public static
        OnlineFilter
        CreateLowpass(
            ImpulseResponse mode,
            double sampleRate,
            double cutoffRate
            )
        {
            return CreateLowpass(
                mode,
                sampleRate,
                cutoffRate,
                mode == ImpulseResponse.Finite ? 64 : 4 // order
                );
        }
        #endregion

        #region HIGHPASS FILTER FACTORY
        /// <summary>
        /// Create a filter to remove low frequencies in online processing scenarios.
        /// </summary>
        public static
        OnlineFilter
        CreateHighpass(
            ImpulseResponse mode,
            double sampleRate,
            double cutoffRate,
            int order
            )
        {
            if(mode == ImpulseResponse.Finite)
            {
                double[] c = FirCoefficients.HighPass(sampleRate, cutoffRate, order >> 1);
                return new OnlineFirFilter(c);
            }

            if(mode == ImpulseResponse.Infinite)
            {
                // TODO: investigate (bandwidth)
                double[] c = IirCoefficients.HighPass(sampleRate, cutoffRate, cutoffRate);
                return new OnlineIirFilter(c);
            }

            throw new ArgumentException("mode");
        }

        /// <summary>
        /// Create a filter to remove low frequencies in online processing scenarios.
        /// </summary>
        public static
        OnlineFilter
        CreateHighpass(
            ImpulseResponse mode,
            double sampleRate,
            double cutoffRate
            )
        {
            return CreateHighpass(
                mode,
                sampleRate,
                cutoffRate,
                mode == ImpulseResponse.Finite ? 64 : 4 // order
                );
        }
        #endregion

        #region BANDPASS FILTER FACTORY
        /// <summary>
        /// Create a filter to remove low and high frequencies in online processing scenarios.
        /// </summary>
        public static
        OnlineFilter
        CreateBandpass(
            ImpulseResponse mode,
            double sampleRate,
            double cutoffLowRate,
            double cutoffHighRate,
            int order
            )
        {
            if(mode == ImpulseResponse.Finite)
            {
                double[] c = FirCoefficients.BandPass(sampleRate, cutoffLowRate, cutoffHighRate, order >> 1);
                return new OnlineFirFilter(c);
            }

            if(mode == ImpulseResponse.Infinite)
            {
                double[] c = IirCoefficients.BandPass(sampleRate, cutoffLowRate, cutoffHighRate);
                return new OnlineIirFilter(c);
            }

            throw new ArgumentException("mode");
        }

        /// <summary>
        /// Create a filter to remove low and high frequencies in online processing scenarios.
        /// </summary>
        public static
        OnlineFilter
        CreateBandpass(
            ImpulseResponse mode,
            double sampleRate,
            double cutoffLowRate,
            double cutoffHighRate
            )
        {
            return CreateBandpass(
                mode,
                sampleRate,
                cutoffLowRate,
                cutoffHighRate,
                mode == ImpulseResponse.Finite ? 64 : 4 // order
                );
        }
        #endregion

        #region BANDSTOP FILTER FACTORY
        /// <summary>
        /// Create a filter to remove middle (all but low and high) frequencies in online processing scenarios.
        /// </summary>
        public static
        OnlineFilter
        CreateBandstop(
            ImpulseResponse mode,
            double sampleRate,
            double cutoffLowRate,
            double cutoffHighRate,
            int order
            )
        {
            if(mode == ImpulseResponse.Finite)
            {
                double[] c = FirCoefficients.BandStop(sampleRate, cutoffLowRate, cutoffHighRate, order >> 1);
                return new OnlineFirFilter(c);
            }

            if(mode == ImpulseResponse.Infinite)
            {
                double[] c = IirCoefficients.BandStop(sampleRate, cutoffLowRate, cutoffHighRate);
                return new OnlineIirFilter(c);
            }

            throw new ArgumentException("mode");
        }

        /// <summary>
        /// Create a filter to remove middle (all but low and high) frequencies in online processing scenarios.
        /// </summary>
        public static
        OnlineFilter
        CreateBandstop(
            ImpulseResponse mode,
            double sampleRate,
            double cutoffLowRate,
            double cutoffHighRate
            )
        {
            return CreateBandstop(
                mode,
                sampleRate,
                cutoffLowRate,
                cutoffHighRate,
                mode == ImpulseResponse.Finite ? 64 : 4 // order
                );
        }
        #endregion

        #region DENOISE FILTER FACTORY
        /// <summary>
        /// Create a filter to remove noise in online processing scenarios.
        /// </summary>
        /// <param name="order">
        /// Window Size, should be odd. A larger number results in a smoother
        /// response but also in a longer delay.
        /// </param>
        /// <remarks>The denoise filter is implemented as an unweighted median filter.</remarks>
        public static
        OnlineFilter
        CreateDenoise(
            int order
            )
        {
            return new OnlineMedianFilter(order);
        }
        /// <summary>
        /// Create a filter to remove noise in online processing scenarios.
        /// </summary>
        /// <remarks>The denoise filter is implemented as an unweighted median filter.</remarks>
        public static
        OnlineFilter
        CreateDenoise()
        {
            return CreateDenoise(7);
        }
        #endregion

        /// <summary>
        /// Process a single sample.
        /// </summary>
        public abstract
        double
        ProcessSample(
            double sample
            );

        /// <summary>
        /// Reset internal state (not coefficients!).
        /// </summary>
        public abstract
        void
        Reset();

        /// <summary>
        /// Process a sequence of sample.
        /// </summary>
        public virtual
        double[]
        ProcessSamples(
            double[] samples
            )
        {
            if(null == samples)
            {
                return null;
            }

            double[] ret = new double[samples.Length];
            for(int i = 0; i < samples.Length; i++)
            {
                ret[i] = ProcessSample(samples[i]);
            }
            return ret;
        }
    }
}
