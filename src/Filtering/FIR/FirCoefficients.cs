// <copyright file="FirCoefficients.cs" company="Math.NET">
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

namespace MathNet.Filtering.FIR
{
    /// <summary>
    /// FirCoefficients provides basic coefficient evaluation
    /// algorithms for the four most important filter types for
    /// Finite Impulse Response (FIR) Filters.
    ///
    /// Default filter order estimation:
    /// transition bandwidth is 25% of the lower passband edge,
    /// but not lower than 2 Hz, where possible (for bandpass,
    /// highpass, and bandstop) and distance from passband edge
    /// to critical frequency (DC, Nyquist) otherwise.
    /// </summary>
    public static class FirCoefficients
    {
        /// <summary>
        /// Calculates FIR LowPass Filter Coefficients.
        /// </summary>
        /// <param name="samplingRate">Samples per unit.</param>
        /// <param name="cutoff">Cutoff frequency in samples per unit.</param>
        /// <param name="halforder">half-order Q, so that Order N = 2*Q+1. 0 for default order.</param>
        /// <returns>The calculated filter coefficients.</returns>
        public static double[] LowPass(double samplingRate, double cutoff, int halforder = 0)
        {
            double nu = 2d*cutoff/samplingRate; // normalized frequency

            // Default filter order
            if (halforder == 0)
            {
                const double TRANSWINDRATIO = 0.25;
                double maxDf = samplingRate / 2 - cutoff;
                double df = (cutoff * TRANSWINDRATIO > 2) ? cutoff * TRANSWINDRATIO : 2;
                df = (df < maxDf) ? df : maxDf;
                halforder = (int)Math.Ceiling(3.3 / (df / samplingRate) / 2);
            }

            int order = 2*halforder + 1;
            var c = new double[order];
            c[halforder] = nu;

            for (int i = 0, n = halforder; i < halforder; i++, n--)
            {
                double npi = n*Math.PI;
                c[i] = Math.Sin(npi*nu)/npi;
                c[n + halforder] = c[i];
            }

            return c;
        }

        /// <summary>
        /// Calculates FIR HighPass Filter Coefficients.
        /// </summary>
        /// <param name="samplingRate">Samples per unit.</param>
        /// <param name="cutoff">Cutoff frequency in samples per unit.</param>
        /// <param name="halforder">half-order Q, so that Order N = 2*Q+1. 0 for default order.</param>
        /// <returns>The calculated filter coefficients.</returns>
        public static double[] HighPass(double samplingRate, double cutoff, int halforder = 0)
        {
            double nu = 2d*cutoff/samplingRate; // normalized frequency

            // Default filter order
            if (halforder == 0)
            {
                const double TRANSWINDRATIO = 0.25;
                double maxDf = cutoff;
                double df = (maxDf * TRANSWINDRATIO > 2) ? maxDf * TRANSWINDRATIO : 2;
                df = (df < maxDf) ? df : maxDf;
                halforder = (int)Math.Ceiling(3.3 / (df / samplingRate) / 2);
            }

            int order = 2*halforder + 1;
            var c = new double[order];
            c[halforder] = 1 - nu;

            for (int i = 0, n = halforder; i < halforder; i++, n--)
            {
                double npi = n*Math.PI;
                c[i] = -Math.Sin(npi*nu)/npi;
                c[n + halforder] = c[i];
            }

            return c;
        }

        /// <summary>
        /// Calculates FIR Bandpass Filter Coefficients.
        /// </summary>
        /// <param name="samplingRate">Samples per unit.</param>
        /// <param name="cutoffLow">Low Cutoff frequency in samples per unit.</param>
        /// <param name="cutoffHigh">High Cutoff frequency in samples per unit.</param>
        /// <param name="halforder">half-order Q, so that Order N = 2*Q+1. 0 for default order.</param>
        /// <returns>The calculated filter coefficients.</returns>
        public static double[] BandPass(double samplingRate, double cutoffLow, double cutoffHigh, int halforder = 0)
        {
            double nu1 = 2d*cutoffLow/samplingRate; // normalized low frequency
            double nu2 = 2d*cutoffHigh/samplingRate; // normalized high frequency

            // Default filter order
            if (halforder == 0)
            {
                const double TRANSWINDRATIO = 0.25;
                double maxDf = (cutoffLow < samplingRate / 2 - cutoffHigh) ? cutoffLow : samplingRate / 2 - cutoffHigh;
                double df = (cutoffLow * TRANSWINDRATIO > 2) ? cutoffLow * TRANSWINDRATIO : 2;
                df = (df < maxDf) ? df : maxDf;
                halforder = (int)Math.Ceiling(3.3 / (df / samplingRate) / 2);
            }

            int order = 2*halforder + 1;
            var c = new double[order];
            c[halforder] = nu2 - nu1;

            for (int i = 0, n = halforder; i < halforder; i++, n--)
            {
                double npi = n*Math.PI;
                c[i] = (Math.Sin(npi*nu2) - Math.Sin(npi*nu1))/npi;
                c[n + halforder] = c[i];
            }

            return c;
        }

        /// <summary>
        /// Calculates FIR Bandstop Filter Coefficients.
        /// </summary>
        /// <param name="samplingRate">Samples per unit.</param>
        /// <param name="cutoffLow">Low Cutoff frequency in samples per unit.</param>
        /// <param name="cutoffHigh">High Cutoff frequency in samples per unit.</param>
        /// <param name="halforder">half-order Q, so that Order N = 2*Q+1. 0 for default order.</param>
        /// <returns>The calculated filter coefficients.</returns>
        public static double[] BandStop(double samplingRate, double cutoffLow, double cutoffHigh, int halforder = 0)
        {
            double nu1 = 2d*cutoffLow/samplingRate; // normalized low frequency
            double nu2 = 2d*cutoffHigh/samplingRate; // normalized high frequency

            // Default filter order
            if (halforder == 0)
            {
                const double TRANSWINDRATIO = 0.25;
                double maxDf = (cutoffHigh - cutoffLow) / 2;
                double df = (maxDf * TRANSWINDRATIO > 2) ? maxDf * TRANSWINDRATIO : 2;
                df = (df < maxDf) ? df : maxDf;
                halforder = (int)Math.Ceiling(3.3 / (df / samplingRate) / 2);
            }

            int order = 2*halforder + 1;
            var c = new double[order];
            c[halforder] = 1 - (nu2 - nu1);

            for (int i = 0, n = halforder; i < halforder; i++, n--)
            {
                double npi = n*Math.PI;
                c[i] = (Math.Sin(npi*nu1) - Math.Sin(npi*nu2))/npi;
                c[n + halforder] = c[i];
            }

            return c;
        }
    }
}
