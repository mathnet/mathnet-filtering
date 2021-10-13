// <copyright file="IirCoefficients.cs" company="Math.NET">
// Math.NET Filtering, part of the Math.NET Project
// http://filtering.mathdotnet.com
// http://github.com/mathnet/mathnet-filtering
//
// Copyright (c) 2009-2021 Math.NET
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

using MathNet.Filtering.IIR;

namespace MathNet.Filtering.Butterworth
{
    /// <summary>
    /// Butterworth filter design generating <see cref="OnlineIirFilter"/> objects.
    /// </summary>
    /// <seealso cref="IirCoefficients"/>
    public static class OnlineIirButterworthFilter
    {
        /// <summary>
        /// Generates an online IIR low-pass Butterworth filter.
        /// </summary>
        /// <param name="passbandFreq">Passband corner frequency (in Hz).</param>
        /// <param name="stopbandFreq">Stopband corner frequency (in Hz).</param>
        /// <param name="passbandRipple">Maximum allowed passband ripple.</param>
        /// <param name="stopbandAttenuation">Minimum required stopband attenuation.</param>
        /// <returns>Online IIR filter</returns>
        public static OnlineIirFilter LowPass(double passbandFreq, double stopbandFreq, double passbandRipple, double stopbandAttenuation)
        {
            (double[] numerator, double[] denominator) = IirCoefficients.LowPass(passbandFreq, stopbandFreq, passbandRipple, stopbandAttenuation);
            return new OnlineIirFilter(numerator, denominator);
        }

        /// <summary>
        /// Generates an online IIR high-pass Butterworth filter.
        /// </summary>
        /// <param name="stopbandFreq">Stopband corner frequency (in Hz).</param>
        /// <param name="passbandFreq">Passband corner frequency (in Hz).</param>
        /// <param name="passbandRipple">Maximum allowed passband ripple.</param>
        /// <param name="stopbandAttenuation">Minimum required stopband attenuation.</param>
        /// <returns>Online IIR filter</returns>
        public static OnlineIirFilter HighPass(double stopbandFreq, double passbandFreq, double passbandRipple, double stopbandAttenuation)
        {
            (double[] numerator, double[] denominator) = IirCoefficients.HighPass(stopbandFreq, passbandFreq, passbandRipple, stopbandAttenuation);
            return new OnlineIirFilter(numerator, denominator);
        }

        /// <summary>
        /// Generates an online IIR band-pass Butterworth filter.
        /// </summary>
        /// <param name="lowStopbandFreq">Lower stopband corner frequency (in Hz).</param>
        /// <param name="lowPassbandFreq">Lower passband corner frequency (in Hz).</param>
        /// <param name="highPassbandFreq">Higher passband corner frequency (in Hz).</param>
        /// <param name="highStopbandFreq">Higher stopband corner frequency (in Hz).</param>
        /// <param name="passbandRipple">Maximum allowed passband ripple.</param>
        /// <param name="stopbandAttenuation">Minimum required stopband attenuation.</param>
        /// <returns>Online IIR filter</returns>
        public static OnlineIirFilter BandPass(double lowStopbandFreq, double lowPassbandFreq, double highPassbandFreq, double highStopbandFreq, double passbandRipple, double stopbandAttenuation)
        {
            (double[] numerator, double[] denominator) = IirCoefficients.BandPass(lowStopbandFreq, lowPassbandFreq, highPassbandFreq, highStopbandFreq, passbandRipple, stopbandAttenuation);
            return new OnlineIirFilter(numerator, denominator);
        }

        /// <summary>
        /// Generates an online IIR band-stop Butterworth filter.
        /// </summary>
        /// <param name="lowPassbandFreq">Lower passband corner frequency (in Hz).</param>
        /// <param name="lowStopbandFreq">Lower stopband corner frequency (in Hz).</param>
        /// <param name="highStopbandFreq">Higher stopband corner frequency (in Hz).</param>
        /// <param name="highPassbandFreq">Higher passband corner frequency (in Hz).</param>
        /// <param name="passbandRipple">Maximum allowed passband ripple.</param>
        /// <param name="stopbandAttenuation">Minimum required stopband attenuation.</param>
        /// <returns>Online IIR filter</returns>
        public static OnlineIirFilter BandStop(double lowPassbandFreq, double lowStopbandFreq, double highStopbandFreq, double highPassbandFreq, double passbandRipple, double stopbandAttenuation)
        {
            (double[] numerator, double[] denominator) = IirCoefficients.BandStop(lowPassbandFreq, lowStopbandFreq, highStopbandFreq, highPassbandFreq, passbandRipple, stopbandAttenuation);
            return new OnlineIirFilter(numerator, denominator);
        }

        /// <summary>
        /// Generates an online IIR notch Butterworth filter.
        /// </summary>
        /// <param name="centralFreq">Filter central frequency.</param>
        /// <param name="Q">Quality factor.</param>
        /// <param name="passbandRipple">Maximum allowed passband ripple.</param>
        /// <param name="stopbandAttenuation">Minimum required stopband attenuation.</param>
        /// <returns>Online IIR filter</returns>
        public static OnlineIirFilter Notch(double centralFreq, double Q, double passbandRipple, double stopbandAttenuation)
        {
            (double[] numerator, double[] denominator) = IirCoefficients.Notch(centralFreq, Q, passbandRipple, stopbandAttenuation);
            return new OnlineIirFilter(numerator, denominator);
        }
    }
}
