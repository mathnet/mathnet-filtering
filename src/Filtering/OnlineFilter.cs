// <copyright file="OnlineFilter.cs" company="Math.NET">
// Math.NET Filtering, part of the Math.NET Project
// http://filtering.mathdotnet.com
// http://github.com/mathnet/mathnet-filtering
//
// Copyright (c) 2009-2014 Math.NET
//
// Permission is hereby granted, free of charge, to any person
// obtaining a copy of this software and associated documentation
// files (the "Software"), to deal in the Software without
// restriction, including without limitation the rights to use,
// copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the
// Software is furnished to do so, subject to the following
// conditions:
//
// The above copyright notice and this permission notice shall be
// included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
// </copyright>

using System;
using MathNet.Filtering.FIR;
using MathNet.Filtering.IIR;
using MathNet.Filtering.Median;

namespace MathNet.Filtering
{
    /// <summary>
    /// Online filters allow processing incoming samples immediately and hence
    /// provide a nearly real-time response with a fixed delay.
    /// </summary>
    public abstract class OnlineFilter : IOnlineFilter
    {
        /// <summary>
        /// Create a filter to remove high frequencies in online processing scenarios.
        /// </summary>
        public static OnlineFilter CreateLowpass(ImpulseResponse mode, double sampleRate, double cutoffRate, int order)
        {
            if (mode == ImpulseResponse.Finite)
            {
                double[] c = FirCoefficients.LowPass(sampleRate, cutoffRate, order >> 1);
                return new OnlineFirFilter(c);
            }

            if (mode == ImpulseResponse.Infinite)
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
        public static OnlineFilter CreateLowpass(ImpulseResponse mode, double sampleRate, double cutoffRate)
        {
            return CreateLowpass(
                mode,
                sampleRate,
                cutoffRate,
                mode == ImpulseResponse.Finite ? 64 : 4); // order
        }

        /// <summary>
        /// Create a filter to remove low frequencies in online processing scenarios.
        /// </summary>
        public static OnlineFilter CreateHighpass(ImpulseResponse mode, double sampleRate, double cutoffRate, int order)
        {
            if (mode == ImpulseResponse.Finite)
            {
                double[] c = FirCoefficients.HighPass(sampleRate, cutoffRate, order >> 1);
                return new OnlineFirFilter(c);
            }

            if (mode == ImpulseResponse.Infinite)
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
        public static OnlineFilter CreateHighpass(ImpulseResponse mode, double sampleRate, double cutoffRate)
        {
            return CreateHighpass(
                mode,
                sampleRate,
                cutoffRate,
                mode == ImpulseResponse.Finite ? 64 : 4); // order
        }

        /// <summary>
        /// Create a filter to remove low and high frequencies in online processing scenarios.
        /// </summary>
        public static OnlineFilter CreateBandpass(ImpulseResponse mode, double sampleRate, double cutoffLowRate, double cutoffHighRate, int order)
        {
            if (mode == ImpulseResponse.Finite)
            {
                double[] c = FirCoefficients.BandPass(sampleRate, cutoffLowRate, cutoffHighRate, order >> 1);
                return new OnlineFirFilter(c);
            }

            if (mode == ImpulseResponse.Infinite)
            {
                double[] c = IirCoefficients.BandPass(sampleRate, cutoffLowRate, cutoffHighRate);
                return new OnlineIirFilter(c);
            }

            throw new ArgumentException("mode");
        }

        /// <summary>
        /// Create a filter to remove low and high frequencies in online processing scenarios.
        /// </summary>
        public static OnlineFilter CreateBandpass(ImpulseResponse mode, double sampleRate, double cutoffLowRate, double cutoffHighRate)
        {
            return CreateBandpass(
                mode,
                sampleRate,
                cutoffLowRate,
                cutoffHighRate,
                mode == ImpulseResponse.Finite ? 64 : 4); // order
        }

        /// <summary>
        /// Create a filter to remove middle (all but low and high) frequencies in online processing scenarios.
        /// </summary>
        public static OnlineFilter CreateBandstop(ImpulseResponse mode, double sampleRate, double cutoffLowRate, double cutoffHighRate, int order)
        {
            if (mode == ImpulseResponse.Finite)
            {
                double[] c = FirCoefficients.BandStop(sampleRate, cutoffLowRate, cutoffHighRate, order >> 1);
                return new OnlineFirFilter(c);
            }

            if (mode == ImpulseResponse.Infinite)
            {
                double[] c = IirCoefficients.BandStop(sampleRate, cutoffLowRate, cutoffHighRate);
                return new OnlineIirFilter(c);
            }

            throw new ArgumentException("mode");
        }

        /// <summary>
        /// Create a filter to remove middle (all but low and high) frequencies in online processing scenarios.
        /// </summary>
        public static OnlineFilter CreateBandstop(ImpulseResponse mode, double sampleRate, double cutoffLowRate, double cutoffHighRate)
        {
            return CreateBandstop(
                mode,
                sampleRate,
                cutoffLowRate,
                cutoffHighRate,
                mode == ImpulseResponse.Finite ? 64 : 4); // order
        }

        /// <summary>
        /// Create a filter to remove noise in online processing scenarios.
        /// </summary>
        /// <param name="order">
        /// Window Size, should be odd. A larger number results in a smoother
        /// response but also in a longer delay.
        /// </param>
        /// <remarks>The de-noise filter is implemented as an unweighted median filter.</remarks>
        public static OnlineFilter CreateDenoise(int order)
        {
            return new OnlineMedianFilter(order);
        }

        /// <summary>
        /// Create a filter to remove noise in online processing scenarios.
        /// </summary>
        /// <remarks>The de-noise filter is implemented as an unweighted median filter.</remarks>
        public static OnlineFilter CreateDenoise()
        {
            return CreateDenoise(7);
        }

        /// <summary>
        /// Process a single sample.
        /// </summary>
        public abstract double ProcessSample(double sample);

        /// <summary>
        /// Reset internal state (not coefficients!).
        /// </summary>
        public abstract void Reset();

        /// <summary>
        /// Process a sequence of sample.
        /// </summary>
        public virtual double[] ProcessSamples(double[] samples)
        {
            if (null == samples)
            {
                return null;
            }

            double[] ret = new double[samples.Length];
            for (int i = 0; i < samples.Length; i++)
            {
                ret[i] = ProcessSample(samples[i]);
            }

            return ret;
        }
    }
}
