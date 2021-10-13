// <copyright file="IirCoefficients.cs" company="Math.NET">
// Math.NET Filtering, part of the Math.NET Project
// http://filtering.mathdotnet.com
// http://github.com/mathnet/mathnet-filtering
//
// Copyright (c) 2009-2019 Math.NET
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

using MathNet.Filtering.TransferFunctions;
using System;
using System.Numerics;
using MathNet.Numerics;

namespace MathNet.Filtering.Butterworth
{
    public static class IirCoefficients
    {
        /// <summary>
        /// Computes the IIR coefficients for a low-pass Butterworth filter.
        /// </summary>
        /// <param name="passbandFreq">Passband corner frequency (in Hz).</param>
        /// <param name="stopbandFreq">Stopband corner frequency (in Hz).</param>
        /// <param name="passbandRipple">Maximum allowed passband ripple.</param>
        /// <param name="stopbandAttenuation">Minimum required stopband attenuation.</param>
        /// <returns>IIR coefficients.</returns>
        /// <seealso cref="Designer.LowPass(double, double, double, double)"/>
        public static (double[] numerator, double[] denominator) LowPass(double passbandFreq, double stopbandFreq, double passbandRipple, double stopbandAttenuation)
        {
            var (n, wc) = Designer.LowPass(passbandFreq, stopbandFreq, passbandRipple, stopbandAttenuation);

            const double T = 2;
            var (gain, zeros, poles) = TransferFunction(n);

            wc = Helpers.MathFunctions.WarpFrequency(wc, T);
            (gain, zeros, poles) = TransferFunctionTransformer.LowPass(gain, zeros, poles, wc);

            return Coefficients(gain, zeros, poles, T);
        }

        /// <summary>
        /// Computes the IIR coefficients for a high-pass Butterworth filter.
        /// </summary>
        /// <param name="stopbandFreq">Stopband corner frequency (in Hz).</param>
        /// <param name="passbandFreq">Passband corner frequency (in Hz).</param>
        /// <param name="passbandRipple">Maximum allowed passband ripple.</param>
        /// <param name="stopbandAttenuation">Minimum required stopband attenuation.</param>
        /// <returns>IIR coefficients.</returns>
        /// <seealso cref="Designer.HighPass(double, double, double, double)"/>
        public static (double[] numerator, double[] denominator) HighPass(double stopbandFreq, double passbandFreq, double passbandRipple, double stopbandAttenuation)
        {
            var (n, wc) = Designer.HighPass(stopbandFreq, passbandFreq, passbandRipple, stopbandAttenuation);

            const double T = 2;
            var (gain, zeros, poles) = TransferFunction(n);

            wc = Helpers.MathFunctions.WarpFrequency(wc, T);
            (gain, zeros, poles) = TransferFunctionTransformer.HighPass(gain, zeros, poles, wc);

            return Coefficients(gain, zeros, poles, T);
        }

        /// <summary>
        /// Computes the IIR coefficients for a band-pass Butterworth filter.
        /// </summary>
        /// <param name="lowStopbandFreq">Lower stopband corner frequency (in Hz).</param>
        /// <param name="lowPassbandFreq">Lower passband corner frequency (in Hz).</param>
        /// <param name="highPassbandFreq">Higher passband corner frequency (in Hz).</param>
        /// <param name="highStopbandFreq">Higher stopband corner frequency (in Hz).</param>
        /// <param name="passbandRipple">Maximum allowed passband ripple.</param>
        /// <param name="stopbandAttenuation">Minimum required stopband attenuation.</param>
        /// <returns>IIR coefficients.</returns>
        /// <seealso cref="Designer.BandPass(double, double, double, double, double, double)"/>
        public static (double[] numerator, double[] denominator) BandPass(double lowStopbandFreq, double lowPassbandFreq, double highPassbandFreq, double highStopbandFreq, double passbandRipple, double stopbandAttenuation)
        {
            var (n, wc1, wc2) = Designer.BandPass(lowStopbandFreq, lowPassbandFreq, highPassbandFreq, highStopbandFreq, passbandRipple, stopbandAttenuation);

            const double T = 2;
            var (gain, zeros, poles) = TransferFunction(n);

            wc1 = Helpers.MathFunctions.WarpFrequency(wc1, T);
            wc2 = Helpers.MathFunctions.WarpFrequency(wc2, T);
            (gain, zeros, poles) = TransferFunctionTransformer.BandPass(gain, zeros, poles, wc1, wc2);

            return Coefficients(gain, zeros, poles, T);
        }

        /// <summary>
        /// Computes the IIR coefficients for a band-stop Butterworth filter.
        /// </summary>
        /// <param name="lowPassbandFreq">Lower passband corner frequency (in Hz).</param>
        /// <param name="lowStopbandFreq">Lower stopband corner frequency (in Hz).</param>
        /// <param name="highStopbandFreq">Higher stopband corner frequency (in Hz).</param>
        /// <param name="highPassbandFreq">Higher passband corner frequency (in Hz).</param>
        /// <param name="passbandRipple">Maximum allowed passband ripple.</param>
        /// <param name="stopbandAttenuation">Minimum required stopband attenuation.</param>
        /// <returns>IIR coefficients.</returns>
        /// <seealso cref="Designer.BandStop(double, double, double, double, double, double)"/>
        public static (double[] numerator, double[] denominator) BandStop(double lowPassbandFreq, double lowStopbandFreq, double highStopbandFreq, double highPassbandFreq, double passbandRipple, double stopbandAttenuation)
        {
            var (n, wc1, wc2) = Designer.BandStop(lowPassbandFreq, lowStopbandFreq, highStopbandFreq, highPassbandFreq, passbandRipple, stopbandAttenuation);

            const double T = 2;
            var (gain, zeros, poles) = TransferFunction(n);

            wc1 = Helpers.MathFunctions.WarpFrequency(wc1, T);
            wc2 = Helpers.MathFunctions.WarpFrequency(wc2, T);
            (gain, zeros, poles) = TransferFunctionTransformer.BandStop(gain, zeros, poles, wc1, wc2);

            return Coefficients(gain, zeros, poles, T);
        }

        /// <summary>
        /// Computes the IIR coefficients for a notch Butterworth filter.
        /// </summary>
        /// <param name="centralFreq">Filter central frequency.</param>
        /// <param name="Q">Quality factor.</param>
        /// <param name="passbandRipple">Maximum allowed passband ripple.</param>
        /// <param name="stopbandAttenuation">Minimum required stopband attenuation.</param>
        /// <returns>IIR coefficients.</returns>
        /// <seealso cref="Designer.Notch(double, double, double, double)"/>
        public static (double[] numerator, double[] denominator) Notch(double centralFreq, double Q, double passbandRipple, double stopbandAttenuation)
        {
            var (n, wc1, wc2) = Designer.Notch(centralFreq, Q, passbandRipple, stopbandAttenuation);

            const double T = 2;
            var (gain, zeros, poles) = TransferFunction(n);

            wc1 = Helpers.MathFunctions.WarpFrequency(wc1, T);
            wc2 = Helpers.MathFunctions.WarpFrequency(wc2, T);
            (gain, zeros, poles) = TransferFunctionTransformer.BandStop(gain, zeros, poles, wc1, wc2);

            return Coefficients(gain, zeros, poles, T);
        }

        /// <summary>
        /// Computes the transfer function for a generic Butterworth filter.
        /// </summary>
        /// <param name="n">Order of the filter.</param>
        /// <returns>The triplet gain, zeros and poles of the transfer function</returns>
        private static (double gain, Complex[] zeros, Complex[] poles) TransferFunction(uint n)
        {
            var zeros = new Complex[0];
            var poles = new Complex[n];
            for (int i = 0; i < n; i++)
            {
                poles[i] = Complex.Exp(Complex.ImaginaryOne * Math.PI * ((2 * (i + 1)) + n - 1) / (2 * n));
            }

            if ((n & 1) == 1) // if n is odd
            {
                var target = (n + 1) / 2;
                poles[target - 1] = -1;
            }

            return (1, zeros, poles);
        }

        /// <summary>
        /// Returns the list of IIR coefficients for a generic Butterworth filter.
        /// </summary>
        /// <param name="gain">Filter gain.</param>
        /// <param name="zeros">Filter zeros list.</param>
        /// <param name="poles">Filter poles list.</param>
        /// <param name="T">Sampling time (inverse of sampling frequency).</param>
        /// <returns>The list of IIR coefficients.</returns>
        private static (double[] numerator, double[] denominator) Coefficients(double gain, Complex[] zeros, Complex[] poles, double T)
        {
            (gain, zeros, poles) = BilinearTransform.Apply(gain, zeros, poles, T);

            double[] numerator = Generate.Map(Helpers.MathFunctions.PolynomialCoefficients(zeros), num => (num * gain).Real);
            double[] denominator = Generate.Map(Helpers.MathFunctions.PolynomialCoefficients(poles), den => den.Real);

            return (numerator, denominator);
        }
    }
}
